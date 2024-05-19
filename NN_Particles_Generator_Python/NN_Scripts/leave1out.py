import numpy as np
import torch
from GANStructure import Generator
from NN_Scripts.ParticlesJsonParser import ParticlesJsonParserTool
from NN_Scripts.SaveNNparams import Hyperparameters
import Config


@torch.no_grad()
def leave1out(generator, all_samples, all_tags):
    from sklearn.neighbors import KNeighborsClassifier
    def generate(labels):
        noise = torch.randn(1, generator.noise_size).cuda()
        generated_data = generator(noise, labels)
        generatedList = generated_data.cpu().detach().numpy()[0]
        return generatedList

    all_count = len(all_samples)
    train_count = int(all_count * 0.8)
    train_samples = all_samples[0:train_count]
    train_tags = all_tags[0:train_count]

    test_samples = all_samples[train_count:]
    test_tags = all_tags[train_count:]

    test_count = len(test_samples)

    train_tags_tensor = torch.tensor(train_tags, dtype=torch.float32, device='cuda')
    generated_samples = [generate(train_tags_tensor[i].unsqueeze(0)) for i in range(train_count)]

    concated_samples = np.concatenate((train_samples, generated_samples), axis=0)
    real_label = 1
    generated_label = 0
    labels = [real_label] * train_count + [generated_label] * train_count

    neigh = KNeighborsClassifier(n_neighbors=1)
    neigh.fit(concated_samples, labels)

    test_tags_tensor = torch.tensor(test_tags, dtype=torch.float32, device='cuda')
    real_test_results = []
    predict_generated_samples = [generate(test_tags_tensor[i].unsqueeze(0)) for i in range(test_count)]
    generated_results = []
    for i in range(test_count):
        real_test_results.append(neigh.predict(np.expand_dims(test_samples[i], axis=0)))
        generated_results.append(neigh.predict(np.expand_dims(predict_generated_samples[i], axis=0)))

    # print(real_test_results)
    # print(generated_results)
    right_results_count = 0
    for i in range(test_count):
        if real_test_results[i] == real_label:
            right_results_count += 1

        if generated_results[i] == generated_label:
            right_results_count += 1

    return right_results_count / float(test_count * 2)


def test_leane1out():
    epoch_num = 999
    generator_saved_epoch_file = Config.get_generator_savename(epoch_num)

    parser_tool = ParticlesJsonParserTool()
    parser_tool.load_dataset(Config.DATASET_FILE)

    loaded_hyperparameters = Hyperparameters.load_from_file(Config.SAVE_PATH_OBJECT / "hyperparameters.json")

    # Гиперпараметры
    noise_size = loaded_hyperparameters.noise_size
    start_hidden_size = loaded_hyperparameters.start_hidden_size
    before_hidden_layers_count = loaded_hyperparameters.g_before_hidden_layers_count
    output_size_floats = loaded_hyperparameters.output_size_floats
    output_size_enables = loaded_hyperparameters.output_size_enables

    # Создание нового экземпляра генератора и дискриминатора
    generator = Generator(noise_size, start_hidden_size, before_hidden_layers_count, output_size_floats).cuda()

    # Загрузка сохраненных параметров
    generator.load_state_dict(torch.load(generator_saved_epoch_file))

    # Перевод моделей в режим инференса (не требующий градиентов)
    generator.eval()

    listsOfFloats = parser_tool.normalized_particles_floats_list
    listsOfFloats = listsOfFloats[0:150]

    print(leave1out(generator, listsOfFloats, ))

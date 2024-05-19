import numpy as np
import torch
from matplotlib import pyplot as plt

from NN_Scripts.GANStructure import Generator, Discriminator
from NN_Scripts.GenerationHelper import GenerationHelper
from NN_Scripts.ParticlesJsonParser import ParticlesJsonParserTool
from NN_Scripts.SaveNNparams import TrainLinks


def measure_discriminator_from_params(train_links: TrainLinks,
                                      noise_size: int,
                                      samples_count,
                                      epoch_i: int):
    generation_helper = GenerationHelper(train_links.parser_tool,
                                         noise_size,
                                         train_links.generator,
                                         train_links.discriminator)

    fake_results, real_results, fake_mean_set = measure_discriminator(generation_helper, samples_count)
    return fake_results, real_results, fake_mean_set


def measure_discriminator_pregenerated(generation_helper: GenerationHelper, samples_count, generated_data, real_data,
                                       real_labels):
    fake_mean_set = set()
    real_mean_set = set()

    for i in range(len(generated_data)):
        generated = generated_data[i]
        real = real_data[i]

        fake_mean = round(sum(generated) / len(generated), 3)
        # print("generated:", generated)
        # print("fake_mean:", fake_mean)
        real_mean = round(sum(real) / len(real), 3)
        fake_mean_set.add(fake_mean)
        real_mean_set.add(real_mean)

        # print("label:", int(real_labels[i][2]), "|real_mean:", real_mean, "|fake_mean:", fake_mean)

    print("real_mean unique len:", len(real_mean_set))
    print("fake_mean unique len:", len(fake_mean_set))

    real_labels = np.array(real_labels, dtype=np.float32)
    real_labels = torch.from_numpy(real_labels).cuda()

    real_data = np.array(real_data, dtype=np.float32)
    real_data = torch.from_numpy(real_data).cuda()

    generated_data = np.array(generated_data, dtype=np.float32)
    generated_data = torch.from_numpy(generated_data).cuda()

    # min_value = min(np.min(real_data_numpy), np.min(generated_data_numpy))
    # print("min_value", min_value)
    # real_data_numpy += abs(min_value)
    # generated_data_numpy += abs(min_value)

    fake_results = []
    real_results = []

    for i in range(samples_count):
        generated_sample = generated_data[i].unsqueeze(0)
        real_sample = real_data[i].unsqueeze(0)
        cur_labels = real_labels[i].unsqueeze(0)

        fake_g = generation_helper.discriminator_calculate(generated_sample, cur_labels).cpu().detach().numpy()
        real_g = generation_helper.discriminator_calculate(real_sample, cur_labels).cpu().detach().numpy()
        fake_results.append(fake_g)
        real_results.append(real_g)

    return fake_results, real_results, fake_mean_set


def measure_discriminator(generation_helper: GenerationHelper, samples_count):
    real_data, real_labels = generation_helper.get_random_real_data_with_labels(samples_count)
    generated_data = generation_helper.generate_samples_by_labels(samples_count, real_labels)

    return measure_discriminator_pregenerated(generation_helper, samples_count, generated_data, real_data, real_labels)


def calculate_for_epoch(epoch_num, samples_count):
    generation_helper = GenerationHelper.from_file(epoch_num)
    return measure_discriminator(generation_helper, samples_count)


def sigmoid(x):
    return 1 / (1 + np.exp(-np.clip(x, -500, 500)))


def show_gistogramm(epoch, fake_results, real_results):
    fake_results = sigmoid(np.array(fake_results))
    real_results = sigmoid(np.array(real_results))

    test_range = (0, 1)
    plt.hist(fake_results, color='red', edgecolor='black',
             bins=int(100), alpha=0.5, label='Сгенерированные', range=test_range)
    plt.hist(real_results, color='green', edgecolor='black',
             bins=int(100), alpha=0.5, label='Датасет', range=test_range)

    # plt.xlabel('critic - вероятность реальности')
    plt.xlabel('Оценка дискриминатора. Вероятность реальности')
    plt.ylabel('Частота')
    plt.title(f'Распределение результатов (Эпоха {epoch})')
    plt.legend()

    plt.tight_layout()
    plt.show()


if __name__ == '__main__':
    epochs = [i for i in range(2, 404, 3)]

    for e in epochs:
        print("Epoch", e)
        fake_results, real_results, fake_mean_set = calculate_for_epoch(0, 20)
        show_gistogramm(e, fake_results, real_results)
        print()

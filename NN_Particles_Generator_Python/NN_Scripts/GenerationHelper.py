import random
import torch
from NN_Scripts import Config
from NN_Scripts.GANStructure import LabelsLayersData, Generator, Discriminator
from NN_Scripts.ParticlesJsonParser import ParticlesJsonParserTool
from NN_Scripts.SaveNNparams import Hyperparameters

class GenerationHelper:

    @classmethod
    def from_file(cls, epoch_num):
        generator_saved_epoch_file = Config.get_generator_savename(epoch_num)
        discriminator_saved_epoch_file = Config.get_discriminator_savename(epoch_num)

        parser_tool = ParticlesJsonParserTool()
        parser_tool.load_dataset(Config.DATASET_FILE)

        loaded_hyperparameters = Hyperparameters.load_from_file(Config.SAVE_PATH_OBJECT / "hyperparameters.json")

        # Гиперпараметры
        noise_size = loaded_hyperparameters.noise_size
        d_start_hidden_size = loaded_hyperparameters.d_start_hidden_size
        g_start_hidden_size = loaded_hyperparameters.g_start_hidden_size
        before_hidden_layers_count = loaded_hyperparameters.g_before_hidden_layers_count
        d_before_hidden_layers_count = loaded_hyperparameters.d_before_hidden_layers_count
        output_size_floats = loaded_hyperparameters.output_size_floats

        generator = Generator(noise_size, g_start_hidden_size, before_hidden_layers_count,
                              output_size_floats).cuda()
        generator.load_state_dict(torch.load(generator_saved_epoch_file))

        discriminator = Discriminator(output_size_floats, d_start_hidden_size,
                                      d_before_hidden_layers_count).cuda()
        discriminator.load_state_dict(torch.load(discriminator_saved_epoch_file))

        return GenerationHelper(parser_tool, noise_size, generator, discriminator)

    def __init__(self,
                 parser_tool: ParticlesJsonParserTool,
                 noise_size: int,
                 generator: Generator,
                 discriminator: Discriminator) -> None:
        self.parser_tool = parser_tool
        self.noise_size = noise_size
        self.generator = generator
        self.discriminator = discriminator

    @torch.no_grad()
    def generate_samples(self, samples_count):
        labels_data = LabelsLayersData()
        test_labels = labels_data.get_labels_tensor(0).cuda()

        return self.generate_samples_by_labels(samples_count, [test_labels] * samples_count)

    @torch.no_grad()
    def get_random_real_data_indexes(self, samples_count):
        random_indices = random.sample(range(len(self.parser_tool.normalized_particles_floats_list)), samples_count)
        return random_indices

    @torch.no_grad()
    def get_random_real_data_with_labels(self, samples_count):
        random_indices = random.sample(range(len(self.parser_tool.normalized_particles_floats_list)), samples_count)

        # Получение случайных элементов по индексам
        random_elements = [self.parser_tool.normalized_particles_floats_list[i] for i in random_indices]
        random_labels = [self.parser_tool.tags_list[i] for i in random_indices]

        return random_elements, random_labels

    @torch.no_grad()
    def generate_samples_by_labels(self, samples_count, labels_list):
        generated_lists = []
        labels_data = LabelsLayersData()

        for i in range(samples_count):
            curLabels = labels_list[i]
            labels_tensor = labels_data.get_labels_tensor_bylist(curLabels)
            noise = torch.randn(1, self.noise_size).cuda()
            generated_data = self.generator(noise, labels_tensor)
            generatedList = generated_data.cpu().detach().numpy()[0]
            generated_lists.append(generatedList)

        return generated_lists

    @torch.no_grad()
    def discriminator_calculate(self, data, labels):
        output = self.discriminator(data, labels).reshape(-1)

        # wgan loss
        return -torch.mean(output)

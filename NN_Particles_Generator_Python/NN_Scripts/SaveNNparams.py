import json

from ParticlesJsonParser import ParticlesJsonParserTool


class TrainLinks:

    def __init__(self,
                 real_data_loader,
                 d_optimizer,
                 g_optimizer,
                 d_loss,
                 g_loss,
                 generator,
                 discriminator,
                 listsOfFloats,
                 listOfTags,
                 parser_tool: ParticlesJsonParserTool) -> None:

        self.real_data_loader = real_data_loader
        self.d_optimizer = d_optimizer
        self.g_optimizer = g_optimizer
        self.d_loss = d_loss
        self.g_loss = g_loss
        self.generator = generator
        self.discriminator = discriminator
        self.listsOfFloats = listsOfFloats
        self.listOfTags = listOfTags
        self.parser_tool = parser_tool


class TrainParameters:

    def __init__(self, num_epochs: int, batch_size: int, critic_iterations: int, generator_iterations: int,
                 weight_clip: float) -> None:
        self.num_epochs = num_epochs
        self.batch_size = batch_size
        self.critic_iterations = critic_iterations
        self.generator_iterations = generator_iterations
        self.weight_clip = weight_clip


class Hyperparameters:
    def __init__(self, noise_size,
                 g_start_hidden_size,
                 d_start_hidden_size,
                 g_before_hidden_layers_count,
                 d_before_hidden_layers_count,
                 output_size_floats):
        self.noise_size = noise_size
        self.g_start_hidden_size = g_start_hidden_size
        self.d_start_hidden_size = d_start_hidden_size
        self.g_before_hidden_layers_count = g_before_hidden_layers_count
        self.d_before_hidden_layers_count = d_before_hidden_layers_count
        self.output_size_floats = output_size_floats

    def save_to_file(self, file_name):
        hyperparameters = {
            "noise_size": self.noise_size,
            "g_start_hidden_size": self.g_start_hidden_size,
            "d_start_hidden_size": self.d_start_hidden_size,
            "g_before_hidden_layers_count": self.g_before_hidden_layers_count,
            "d_before_hidden_layers_count": self.d_before_hidden_layers_count,
            "output_size_floats": self.output_size_floats,
        }
        with open(file_name, 'w') as json_file:
            json.dump(hyperparameters, json_file, indent=4)

    @classmethod
    def load_from_file(cls, file_name):
        try:
            with open(file_name, 'r') as json_file:
                data = json.load(json_file)
            return cls(
                data["noise_size"],
                data["g_start_hidden_size"],
                data["d_start_hidden_size"],
                data["g_before_hidden_layers_count"],
                data["d_before_hidden_layers_count"],
                data["output_size_floats"])
        except FileNotFoundError:
            print(f"Файл {file_name} не найден.")
            return None

# # Пример использования класса
# hyperparameters = Hyperparameters(10, 512, 4495)
#
# # Сохранение параметров в файл
# hyperparameters.save_to_file("hyperparameters.json")
# print("Гиперпараметры сохранены в файле 'hyperparameters.json'.")
#
# # Загрузка параметров из файла
# loaded_hyperparameters = Hyperparameters.load_from_file("hyperparameters.json")
# if loaded_hyperparameters:
#     print("Извлеченные гиперпараметры:")
#     print(f"input_size: {loaded_hyperparameters.input_size}")
#     print(f"hidden_size: {loaded_hyperparameters.hidden_size}")
#     print(f"output_size: {loaded_hyperparameters.output_size}")

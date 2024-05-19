import os
import sys

os.chdir(os.path.dirname(os.path.abspath(__file__)))
print(os.path.dirname(os.path.abspath(__file__)))

import torch
from GANStructure import Generator, Discriminator, LabelsLayersData
from ParticlesJsonParser import ParticlesJsonParserTool, GenerateJson
from SaveNNparams import Hyperparameters
import Config

if len(sys.argv) != 5:
    print("Usage: python script.py <epoch> <number1> <number2> <number3>")
    raise Exception('Usage: python script.py <epoch> <number1> <number2> <number3>')
    exit()
try:
    generate_epoch = int(sys.argv[1])
    input_form = int(sys.argv[2])
    input_element = int(sys.argv[3])
    input_color_group = int(sys.argv[4])

    print("generate_epoch - " + str(generate_epoch))
    print("input_color_group - " + str(input_color_group))
    print("input_element - " + str(input_element))
    print("input_form - " + str(input_form))
except ValueError:
    print("Error: Please provide valid integer numbers.")
    raise Exception('Error: Please provide valid integer numbers.')
    exit()

epoch_num = generate_epoch
generator_saved_epoch_file = Config.get_generator_savename(epoch_num)

parser_tool = ParticlesJsonParserTool()
parser_tool.load_dataset(Config.DATASET_FILE)

loaded_hyperparameters = Hyperparameters.load_from_file(Config.SAVE_PATH_OBJECT / "hyperparameters.json")

# Гиперпараметры
noise_size = loaded_hyperparameters.noise_size
g_start_hidden_size = loaded_hyperparameters.g_start_hidden_size
d_start_hidden_size = loaded_hyperparameters.d_start_hidden_size
before_hidden_layers_count = loaded_hyperparameters.g_before_hidden_layers_count
d_before_hidden_layers_count = loaded_hyperparameters.d_before_hidden_layers_count
output_size_floats = loaded_hyperparameters.output_size_floats

# Создание нового экземпляра генератора и дискриминатора
generator = Generator(noise_size, g_start_hidden_size, before_hidden_layers_count, output_size_floats).cuda()

# Загрузка сохраненных параметров
generator.load_state_dict(torch.load(generator_saved_epoch_file))

# Перевод моделей в режим инференса (не требующий градиентов)
generator.eval()

labels_data = LabelsLayersData()
labels = labels_data.get_labels_tensor(input_color_group)

noise = torch.randn(1, noise_size).cuda()
generated_data = generator(noise, labels)
print("Generated Data:", generated_data.cpu().detach().numpy())

discriminator = Discriminator(output_size_floats, d_start_hidden_size, d_before_hidden_layers_count).cuda()

generatedList = generated_data.cpu().detach().numpy()[0]

denormilized_generated = parser_tool.normalizer.denormalize([generatedList])[0]

GenerateJson(denormilized_generated, parser_tool.get_json_example())
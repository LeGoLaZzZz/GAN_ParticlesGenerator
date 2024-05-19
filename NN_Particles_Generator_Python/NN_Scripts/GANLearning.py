import torch
import torch.nn as nn
import torch.optim as optim
import numpy as np
from torch.utils.data import DataLoader
from GANStructure import Generator, Discriminator, LabelsLayersData
from NN_Scripts.CustomDataset import CustomDataset
from NN_Scripts.ParticlesJsonParser import ParticlesJsonParserTool
import matplotlib.pyplot as plt
from torchinfo import summary
from NN_Scripts.SaveNNparams import Hyperparameters, TrainLinks, TrainParameters
import Config
from NN_Scripts.TrainStatusLogger import TrainStatusLogger
from NN_Scripts.main_train_defs import wgan_loss_train

parser_tool = ParticlesJsonParserTool()
parser_tool.load_dataset(Config.DATASET_FILE)
listsOfFloats = parser_tool.normalized_particles_floats_list
listsOfFloats = listsOfFloats[0:700]
listOfTags = parser_tool.tags_list[0:700]

print("example floats len(" + str(len(listsOfFloats[0])) + "):" + str(listsOfFloats[0]))
print("example tags:" + str(listOfTags[0]))
print("final Len listsOfFloats:" + str(len(listsOfFloats)))

# Гиперпараметры
noise_size = 109  # Размер входного шумового вектора для генератора
g_start_hidden_size = 32  # Размер начального скрытого noise_size->start_hidden_size
d_start_hidden_size = 8 # Размер начального скрытого noise_size->start_hidden_size
g_before_hidden_layers_count = 5  # noise_size -> 3: 128 * 2^0 = 128 -> 128 * 2^1 = 256 -> 128 * 2^2 = 512 -> 128 * 2^3 = 1024
d_before_hidden_layers_count = 1  # noise_size -> 3: 128 * 2^0 = 128 -> 128 * 2^1 = 256 -> 128 * 2^2 = 512 -> 128 * 2^3 = 1024
output_size_floats = 183

# Обучение GAN
num_epochs = 300
batch_size = 4

# Wgan
learning_rate: float = 5e-5
critic_iterations: int = 5
generator_iterations: int = 1
weight_clip: float = 10

hyperparameters = Hyperparameters(noise_size,
                                  g_start_hidden_size,
                                  d_start_hidden_size,
                                  g_before_hidden_layers_count,
                                  d_before_hidden_layers_count,
                                  output_size_floats)

hyperparameters.save_to_file(Config.SAVE_PATH_OBJECT / "hyperparameters.json")
# Преобразование данных в тензоры PyTorch
full_real_data_tensor = np.array(listsOfFloats, dtype=np.float32)
full_real_data_tensor = torch.from_numpy(full_real_data_tensor)

full_real_labels_tensor = np.array(listOfTags, dtype=np.float32)
full_real_labels_tensor = torch.from_numpy(full_real_labels_tensor)

# Создание генератора и дискриминатора и перемещение их на GPU
generator = Generator(noise_size, g_start_hidden_size, g_before_hidden_layers_count, output_size_floats).cuda()
discriminator = Discriminator(output_size_floats, d_start_hidden_size, d_before_hidden_layers_count).cuda()

# Определение функции потерь и оптимизаторов
criterion = nn.MSELoss()
d_optimizer = optim.Adam(discriminator.parameters(), lr=learning_rate, betas=(0.5, 0.999))
g_optimizer = optim.Adam(generator.parameters(), lr=learning_rate, betas=(0.5, 0.999))

modules_count = 7
customDataset = CustomDataset(modules_count, parser_tool, full_real_data_tensor, full_real_labels_tensor)

real_data_loader = DataLoader(customDataset, batch_size=batch_size, shuffle=True)

d_losses_list = []
g_losses_list = []
leave1out_results_y = []
leave1out_epoch_x = []
difference_results_y = []
difference_epoch_x = []

labels_data = LabelsLayersData()

total_g_input_size = noise_size + labels_data.get_total_embedding_size()

start_time = 0
debug_always_ON_d_learn = False

d_loss = torch.zeros(1)
g_loss = torch.zeros(1)

d_epoch_wait_delays = [(40, 60), (100, 450), (500, 800), (900, 1200), (1300, 3000), (1700, 1900), (2000, 2500),
                       (3500, 4500)]

train_links = TrainLinks(real_data_loader, d_optimizer, g_optimizer, d_loss, g_loss, generator, discriminator,
                         listsOfFloats, listOfTags, parser_tool)
train_parameters = TrainParameters(num_epochs, batch_size, critic_iterations, generator_iterations, weight_clip)

logger = TrainStatusLogger(train_links, hyperparameters)

wgan_loss_train(hyperparameters, train_links, train_parameters, logger)
# default_loss_train(hyperparameters, train_links, train_parameters, logger)


logger.show_final_losses_plot()

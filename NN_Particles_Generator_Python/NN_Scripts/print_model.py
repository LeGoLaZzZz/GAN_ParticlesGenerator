from collections import defaultdict

import torch
from termcolor import colored

from NN_Scripts import Config
from NN_Scripts.GANStructure import Generator
from NN_Scripts.SaveNNparams import Hyperparameters


def beautiful_int(i):
    i = str(i)
    return ".".join(reversed([i[max(j, 0):j + 3] for j in range(len(i) - 3, -3, -3)]))


# Counting how many parameters does our model have
def model_num_params(model, verbose_all=True, verbose_only_learnable=False):
    sum_params = 0
    sum_learnable_params = 0
    submodules = defaultdict(lambda: [0, 0])
    for name, param in model.named_parameters():
        num_params = param.numel()
        if verbose_all or (verbose_only_learnable and param.requires_grad):
            print(
                colored(
                    '{: <65} ~  {: <9} params ~ grad: {}'.format(
                        name,
                        beautiful_int(num_params),
                        param.requires_grad,
                    ),
                    {True: "green", False: "red"}[param.requires_grad],
                )
            )
        sum_params += num_params
        sm = name.split(".")[0]
        submodules[sm][0] += num_params
        if param.requires_grad:
            sum_learnable_params += num_params
            submodules[sm][1] += num_params
    print(
        f'\nIn total:\n  - {beautiful_int(sum_params)} params\n  - {beautiful_int(sum_learnable_params)} learnable params'
    )

    for sm, v in submodules.items():
        print(
            f"\n . {sm}:\n .   - {beautiful_int(submodules[sm][0])} params\n .   - {beautiful_int(submodules[sm][1])} learnable params"
        )
    return sum_params, sum_learnable_params




loaded_hyperparameters = Hyperparameters.load_from_file(Config.SAVE_PATH_OBJECT / "hyperparameters.json")

noise_size = loaded_hyperparameters.noise_size
d_start_hidden_size = loaded_hyperparameters.d_start_hidden_size
g_start_hidden_size = loaded_hyperparameters.g_start_hidden_size
before_hidden_layers_count = loaded_hyperparameters.g_before_hidden_layers_count
d_before_hidden_layers_count = loaded_hyperparameters.d_before_hidden_layers_count
output_size_floats = loaded_hyperparameters.output_size_floats

generator = Generator(noise_size, g_start_hidden_size, before_hidden_layers_count, output_size_floats).cuda()

epoch_num = 95
generator_saved_epoch_file = Config.get_generator_savename(epoch_num)

generator.load_state_dict(torch.load(generator_saved_epoch_file))

model_num_params(generator)
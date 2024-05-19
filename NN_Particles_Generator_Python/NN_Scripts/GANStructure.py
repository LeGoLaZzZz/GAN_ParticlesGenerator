import torch
import torch.nn as nn
import random


class LabelsLayersData:

    def __init__(self) -> None:
        self.num_color_groups = 4
        self.embedding_dim_color_groups = 16

    def get_total_embedding_size(self) -> int:
        return self.embedding_dim_color_groups

    def generate_random_labels_tensor(self):
        labels_raw = self.generate_random_labels()
        train_tags_tensor = torch.tensor(labels_raw, dtype=torch.float32, device='cuda')
        return train_tags_tensor.unsqueeze(0)

    def get_labels_tensor(self, color_group):
        train_tags_tensor = torch.tensor(self.get_labels_list(color_group), dtype=torch.float32, device='cuda')
        return train_tags_tensor.unsqueeze(0)

    def get_labels_tensor_bylist(self, labels_list):
        train_tags_tensor = torch.tensor(labels_list, dtype=torch.float32, device='cuda')
        return train_tags_tensor.unsqueeze(0)

    def get_labels_list(self, color_group):
        labels_raw = [0, 0, color_group]  # 0 and 1 indexes additional slots for labels
        return labels_raw

    def generate_random_labels(self):
        color_group = random.randint(0, self.num_color_groups - 1)
        return self.get_labels_list(color_group)


class Generator(nn.Module):
    def __init__(self,
                 noise_size,
                 start_hidden_size,
                 before_hidden_layers_count,
                 output_size_floats):
        super(Generator, self).__init__()

        labels_data = LabelsLayersData()

        # self.embedding_form = nn.Embedding(labels_data.num_forms, labels_data.embedding_dim_form)
        self.embedding_color_group = nn.Embedding(labels_data.num_color_groups, labels_data.embedding_dim_color_groups)
        self.noise_size = noise_size
        total_g_input_size = noise_size + labels_data.get_total_embedding_size()
        # 10, 256, 512, 1024
        sizes = [total_g_input_size] + [start_hidden_size * 2 ** n for n in range(before_hidden_layers_count + 1)]

        layers_hidden = []
        # layers_hidden.append(nn.MultiheadAttention())
        for n in range(1, len(sizes)):
            layers_hidden.append(nn.Linear(sizes[n - 1], sizes[n]))
            layers_hidden.append(nn.InstanceNorm1d(sizes[n]))
            layers_hidden.append(nn.ReLU())
            layers_hidden.append(nn.Dropout1d(0.1))

        self.layers_before_hidden = nn.Sequential(*layers_hidden)

        last_hidden_size = start_hidden_size * 2 ** before_hidden_layers_count

        self.layers_after_hidden_floats = nn.Sequential(
            nn.Linear(last_hidden_size, output_size_floats),
            nn.Tanh(),
        )

        print("Generator:")
        print(self.layers_before_hidden)
        print(self.layers_after_hidden_floats)

    def forward(self, x, labels):
        # embedded_form = self.embedding_form(labels[:, 0].long().cuda())
        embedded_color_group = self.embedding_color_group(labels[:, 2].long().cuda())

        a1 = x
        # a1 = torch.cat((x, embedded_form, embedded_color_group), dim=1).cuda()
        a2 = torch.cat((a1, embedded_color_group), dim=1)

        h = self.layers_before_hidden(a2).cuda()
        fvalues = self.layers_after_hidden_floats(h).cuda()
        return fvalues


class Discriminator(nn.Module):
    def __init__(self, input_size, start_hidden_size, before_hidden_layers_count):
        super(Discriminator, self).__init__()

        labels_data = LabelsLayersData()

        # self.embedding_form = nn.Embedding(labels_data.num_forms, labels_data.embedding_dim_form)
        self.embedding_color_group = nn.Embedding(labels_data.num_color_groups, labels_data.embedding_dim_color_groups)

        sizes = [1] + [start_hidden_size * 2 ** n for n in range(before_hidden_layers_count + 1)] + \
                [input_size + labels_data.get_total_embedding_size()]
        sizes.reverse()

        hlayers = []
        for n in range(1, len(sizes)):
            hlayers.append(nn.Linear(sizes[n - 1], sizes[n]))

            if n != len(sizes) - 1:
                hlayers.append(nn.InstanceNorm1d(sizes[n]))
                hlayers.append(nn.LeakyReLU(0.2))

        self.layers = nn.Sequential(*hlayers)

        print("Discriminator:")
        print(self.layers)

    def forward(self, x, labels):

        # embedded_form = self.embedding_form(labels[:, 0].long().cuda()).cuda()
        embedded_color_group = self.embedding_color_group(labels[:, 2].long().cuda()).cuda()

        h1 = x
        # h1 = torch.cat((x, embedded_form), dim=1)
        h2 = torch.cat((h1, embedded_color_group), dim=1)

        return self.layers(h2)

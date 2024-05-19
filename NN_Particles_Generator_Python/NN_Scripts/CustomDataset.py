import torch
from torch import Tensor
from torch.utils.data import TensorDataset

from NN_Scripts.ParticlesJsonParser import ParticlesJsonParserTool


class CustomDataset(TensorDataset):

    def __init__(self, modules_count: int, parser: ParticlesJsonParserTool, tensors_particles: Tensor,
                 tensors_labels: Tensor):
        super().__init__(*tensors_particles)
        self.modules_count = modules_count
        self.parser = parser
        self.tensors_particles = tensors_particles
        self.tensors_labels = tensors_labels

    def __getitem__(self, index: int):
        i_tensor = self.tensors_particles[index]
        i_tensor_labels = self.tensors_labels[index]
        noise = torch.randn(i_tensor.size())
        # noise = torch.normal(0, 0.01, size=returned.size)

        noise *= 0.01
        for i in range(self.modules_count):
            noise[i] = 0

        # return_tensor = i_tensor + noise
        return i_tensor, i_tensor_labels

    def __len__(self):
        return len(self.tensors_particles)

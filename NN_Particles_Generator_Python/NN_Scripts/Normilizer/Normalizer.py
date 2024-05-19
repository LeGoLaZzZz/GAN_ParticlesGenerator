import os
from abc import ABC, abstractmethod

from sklearn.preprocessing import MinMaxScaler

class Normalizer(ABC):
    @abstractmethod
    def fit(self, data):
        pass

    @abstractmethod
    def get_mins(self):
        pass

    @abstractmethod
    def get_maxs(self):
        pass

    @abstractmethod
    def normalize(self, data):
        pass

    @abstractmethod
    def denormalize(self, data):
        pass


class Normalizer_MinMaxScaler(Normalizer):

    def __init__(self, scale_to_min, scale_to_max) -> None:
        self.scaler = MinMaxScaler(feature_range=(scale_to_min, scale_to_max), clip=True)

    def fit(self, data):
        self.scaler.fit(data)

    def get_mins(self):
        return self.scaler.data_min_

    def get_maxs(self):
        return self.scaler.data_max_

    def normalize(self, data):
        return self.scaler.transform(data)

    def denormalize(self, data):
        return self.scaler.inverse_transform(data)


# I assume that there will only be a list of lists of floats to enter
class Normalizer_CustomMinMaxScaler(Normalizer):

    def __init__(self, scale_to_min, scale_to_max):
        self.mins = None
        self.maxs = None
        self.scale_to_min = scale_to_min
        self.scale_to_max = scale_to_max

    def fit(self, data):
        num_elements = len(data[0])
        mins = [float('inf')] * num_elements
        maxs = [float('-inf')] * num_elements

        ii = 0
        for array in data:
            ii += 1
            for i in range(num_elements):
                mins[i] = min(mins[i], array[i])
                maxs[i] = max(maxs[i], array[i])

        self.mins = mins
        self.maxs = maxs

    def get_mins(self):
        return self.mins

    def get_maxs(self):
        return self.maxs

    def normalize(self, data):
        normalized_data = []
        i = -1
        for particle in data:
            floatIndex = -1
            i += 1
            normalized_data.append([])
            for value in particle:
                floatIndex += 1
                append_value = value
                normalized_value = lerp_float_value(append_value,
                                                    self.mins[floatIndex], self.maxs[floatIndex],
                                                    self.scale_to_min, self.scale_to_max)
                normalized_data[i].append(normalized_value)
        return normalized_data

    def denormalize(self, data):
        unlerped_values = []

        for i in range(len(data)):
            unlerped_values.append(
                lerp_float_value(data[i], self.scale_to_min, self.scale_to_max, self.mins[i], self.maxs[i]))

        return unlerped_values


def lerp_float_value(floatToMatch, initialMin, initalMax, newMin, newMax):
    min_val = initialMin
    max_val = initalMax

    new_min_val = newMin
    new_max_val = newMax

    if newMin == newMax:
        return newMin

    if initialMin == initalMax and newMin <= initialMin <= newMax:
        return initialMin

    if initialMin == initalMax:
        return (newMin + newMax) / 2

    # y = (x - min_val) / (max_val - min_val) * (new_max_val - new_min_val) + new_min_val
    floatToMatch = (floatToMatch - min_val) / (max_val - min_val) * (new_max_val - new_min_val) + new_min_val
    return floatToMatch

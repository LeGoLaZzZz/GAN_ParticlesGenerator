import json
import os
import string
from Normilizer.Normalizer import Normalizer_MinMaxScaler, Normalizer_CustomMinMaxScaler


class ParticlesJsonParserTool:

    def __init__(self):
        self.tags_list = []
        self.normalizer = None
        self.initial_particles_floats_list = []
        self.normalized_particles_floats_list = []
        self.example_json_particle = None

    def load_dataset(self, dataset_json_path):
        self.initial_particles_floats_list = self.get_list_of_particles_floats_from_full_data(dataset_json_path)
        self.init_min_max_values()

    def init_min_max_values(self):
        self.normalizer = Normalizer_MinMaxScaler(-1, 1)
        # self.normalizer = Normalizer_CustomMinMaxScaler(-1, 1)

        self.normalizer.fit(self.initial_particles_floats_list)
        self.normalized_particles_floats_list = self.normalizer.normalize(self.initial_particles_floats_list)

    def debug_normalizer_output(self):

        test_original = self.initial_particles_floats_list[123].copy()
        test_original[11] = -100
        test_normalized = self.normalizer.normalize([test_original])[0]

        print("___test_original__")
        GenerateJson(test_original, self.get_json_example(), "test_original.json")

        print("___test_normalized__")
        GenerateJson(test_normalized, self.get_json_example(), "test_normalized.json")

        print("___get_maxs__")
        GenerateJson(self.normalizer.get_maxs(), self.get_json_example(), "max_values.json")

        print("___get_mins__")
        GenerateJson(self.normalizer.get_mins(), self.get_json_example(), "min_values.json")

    def get_list_of_particles_floats_from_full_data(self, jsonPath, need_print=False):
        with open(jsonPath, "r") as file:
            data_dict = json.load(file)
            allParticles = data_dict['particlesJson']
            self.example_json_particle = allParticles[15]

            result_listOfListsOfFloats = get_list_of_particles_floats(allParticles)

            if need_print:
                print("_____")
                print("loaded " + str(len(result_listOfListsOfFloats)) + " particles with lens:")
                print(len(result_listOfListsOfFloats[0]))
                print("example parsed floats: " + str(result_listOfListsOfFloats[0]))
                # for floats in listOfListsOfFloats:
                #     print(len(floats))
                print("_____")

            if data_dict.__contains__("tagsJson"):
                tagsJson = data_dict['tagsJson']
                self.tags_list = get_list_of_particles_floats(tagsJson)

                if need_print:
                    print("_____")
                    print("loaded " + str(len(self.tags_list)) + " tags with lens:")
                    print(len(self.tags_list[0]))
                    print("example parsed tags: " + str(self.tags_list[0]))
                    # for floats in listOfListsOfFloats:
                    #     print(len(floats))
                    print("_____")

            return result_listOfListsOfFloats

    def get_json_example(self):
        return self.example_json_particle

    def generate_single_fake(self):
        import random

        fake_element = []

        for i in range(len(self.normalizer.get_mins)):
            _min = self.normalizer.get_mins[i]
            _max = self.normalizer.get_maxs[i]

            fake_element.append(
                _min + random.random() * (_max - _min)
            )

        return fake_element

    def generate_synth_data(self, synth_percentage):
        import math
        synth_elements = math.floor(len(self.initial_particles_floats_list) * synth_percentage)
        for i in range(synth_elements):
            self.initial_particles_floats_list += [self.generate_single_fake()]


def get_list_of_particles_floats(particlesJsonsLists):
    listOfListsOfFloats = []
    for particleJson in particlesJsonsLists:
        new_floats = GetFloatsFromJsonString(particleJson)
        listOfListsOfFloats.append(new_floats)

    return listOfListsOfFloats


def GetJsonExampleFromFullParticlesJson(jsonPath="fullData.json"):
    with open(jsonPath, "r") as file:
        data_dict = json.load(file)

    particlesJson = data_dict['particlesJson']
    for particleJson in particlesJson:
        return particleJson


def GetFloatsFromJsonString(jsonString: string):
    data_dict = json.loads(jsonString)

    # Extract float values from the entire JSON structure
    float_values = extract_float_values(data_dict)
    # print(len(float_values))
    # Print the list of float values
    # print(float_values)
    # print(float_values[176])
    return float_values


def GetFloatsFromJsonFile(jsonPath):
    # Open the JSON file for reading
    with open(jsonPath, "r") as file:
        return GetFloatsFromJsonString(file.read())


def debug_parse_key_value(key, value):
    if key.find("123321") != -1:
        print(f"{key}: {value}")


# Function to recursively extract float values from a dictionary
def extract_float_values(obj, debug_key: str = None):
    float_list = []
    if isinstance(obj, dict):
        for key, value in obj.items():
            float_list.extend(
                extract_float_values(value, (debug_key + "/" + key if debug_key else key) + f" ({len(float_list)})"))
    elif isinstance(obj, list):
        for item in obj:
            float_list.extend(extract_float_values(item))
    elif isinstance(obj, (int, float)):
        newFloat = float(obj)
        newFloat = round(float(newFloat), 6)
        if debug_key != None:
            debug_parse_key_value(debug_key + f" ({len(float_list)}, int/float)", newFloat)
        float_list.append(newFloat)
    elif isinstance(obj, bool):
        newFloat = 1 if obj else 0
        if debug_key != None:
            debug_parse_key_value(debug_key + f" ({len(float_list)}, bool)", newFloat)
        float_list.append(newFloat)
    elif isinstance(obj, str):
        newFloat = float(obj)
        newFloat = round(float(newFloat), 6)
        float_list.append(newFloat)
    else:
        print("Value of json obj not valid")
        print(obj)
    return float_list


def GenerateJson(generatedArray, jsonExample, file_name="output.json", ignore_bools=False):
    data_dict = json.loads(jsonExample)
    fill_data_dict(data_dict, generatedArray, [0], ignore_bools)
    rounded = round_floats(data_dict, 4)

    # Сохранить результат в файл
    with open(file_name, "w") as outfile:
        json.dump(rounded, outfile, ensure_ascii=False, indent=4)

    return rounded


def fill_data_dict(data, gen_array, index, ignore_bools=False):
    """
    Рекурсивно заполняет JSON-структуру данными из gen_array.
    :param data: часть JSON-структуры (словарь или список) для заполнения.
    :param gen_array: массив с данными для заполнения.
    :param index: список с одним элементом, который используется для отслеживания текущего индекса в gen_array.
                  Используется список для сохранения изменений индекса в рекурсивных вызовах.
    """

    if isinstance(data, dict):
        for key in data:
            if isinstance(data[key], (dict, list)):
                fill_data_dict(data[key], gen_array, index)
            else:
                if index[0] < len(gen_array):
                    if isinstance(data[key], bool) and not ignore_bools:
                        data[key] = bool(gen_array[index[0]] >= 0.5)
                    else:
                        data[key] = round(gen_array[index[0]], 5)
                    index[0] += 1
    elif isinstance(data, list):
        for i in range(len(data)):
            if isinstance(data[i], (dict, list)):
                fill_data_dict(data[i], gen_array, index)
            else:
                if index[0] < len(gen_array):
                    if isinstance(data[i], bool) and not ignore_bools:
                        data[i] = bool(gen_array[index[0]] >= 0.5)
                    else:
                        data[i] = round(gen_array[index[0]], 5)
                    index[0] += 1


# testjson = GetJsonExampleFromFullParticlesJson("../NN_Scripts/fullData.json")
# print("testjson" + testjson)
#
# floats = GetFloatsFromJsonString(testjson)
# print(floats)

def round_floats(o, round_count):
    if isinstance(o, float):
        return round(o, round_count)
    if isinstance(o, dict):
        return {k: round_floats(v, round_count) for k, v in o.items()}
    if isinstance(o, (list, tuple)):
        return [round_floats(x, round_count) for x in o]
    return o

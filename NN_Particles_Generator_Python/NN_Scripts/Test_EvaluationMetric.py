import numpy as np
from matplotlib import pyplot as plt
from sklearn.ensemble import RandomForestClassifier
from sklearn.manifold import TSNE
from sklearn.model_selection import train_test_split

from NN_Scripts.GenerationHelper import GenerationHelper
import seaborn as sns


def get_unique_ids(tags):
    unique_ids = [0] * len(tags)

    def generate_unique_id_form_and_color(tag):
        form = tag.item(0)
        color_group = tag.item(2)
        i = form
        j = color_group
        return 0.5 * (i + j - 2) * (i + j - 1) + i

    def generate_unique_id_color_group(tag):
        color_group = tag.item(2)
        return color_group

    i = 0
    for tag in tags:
        unique_ids[i] = int(generate_unique_id_color_group(tag))
        i += 1

    return unique_ids


class TSNEResultData:

    def __init__(self, tsne_metric, tsne_points, labels, epoch) -> None:
        self.tsne_metric = tsne_metric
        self.tsne_points = tsne_points
        self.labels = labels
        self.epoch = epoch


def evaluation_metric_TSNE_from_file(epoch_i):
    generation_helper = GenerationHelper.from_file(epoch_i)

    return evaluation_metric_TSNE(generation_helper, epoch_i)


def plot_dataset_TSNE(generation_helper: GenerationHelper):
    samples_count = 600  # нужно много, чтобы нормально
    real_data, real_labels = generation_helper.get_random_real_data_with_labels(samples_count)
    real_data = np.array(real_data)

    real_labels = np.array(real_labels)
    real_labels = get_unique_ids(real_labels)
    data_train, data_test, labels_train, labels_test = train_test_split(real_data, real_labels, test_size=0.10,
                                                                        random_state=42)
    tsne_data = get_classification_metric(data_train, data_test, labels_train, labels_test, -999)

    color_map = {0: 'red', 1: 'blue', 2: 'green', 3: 'purple', }
    plt.figure(figsize=(10, 10))
    plt.title("TSNE Dataset. TSNE metric:" + str(tsne_data.tsne_metric))
    labels_to_scatter = tsne_data.labels
    sns.scatterplot(x=tsne_data.tsne_points[:, 0], y=tsne_data.tsne_points[:, 1],
                    hue=labels_to_scatter, legend="full", palette=color_map)
    plt.show()


def evaluation_metric_TSNE(generation_helper: GenerationHelper, epoch_i) -> TSNEResultData:
    samples_count = 600  # нужно много, чтобы нормально
    real_data, real_labels = generation_helper.get_random_real_data_with_labels(samples_count)
    generated_data = generation_helper.generate_samples_by_labels(samples_count, real_labels)
    generated_data = np.array(generated_data)
    real_labels = np.array(real_labels)
    real_labels = get_unique_ids(real_labels)
    data_train, data_test, labels_train, labels_test = train_test_split(generated_data, real_labels, test_size=0.30,
                                                                        random_state=42)

    tsne_data = get_classification_metric(data_train, data_test, labels_train, labels_test, epoch_i)
    return tsne_data


def get_classification_metric(train_data, test_data, train_labels, test_labels, epoch_i) -> TSNEResultData:
    points_train = TSNE_get_points(train_data, train_labels, epoch_i)
    points_test = TSNE_get_points(test_data, test_labels, epoch_i)

    clf = RandomForestClassifier(n_estimators=200)
    clf.fit(points_train, train_labels)
    result = clf.predict(points_test)
    test_tsneMetric = np.mean(test_labels == result)

    print(f'tsneMetric test: {test_tsneMetric}')

    tsne_data = TSNEResultData(test_tsneMetric, points_test, test_labels, epoch_i)
    return tsne_data


def TSNE_get_points(generated_data, labels, epoch_i):
    points = TSNE(n_components=2, learning_rate='auto', init='random', perplexity=10).fit_transform(generated_data)
    return points


import seaborn as sns
from NN_Scripts import Config
from NN_Scripts.GenerationHelper import GenerationHelper
from NN_Scripts.SaveNNparams import Hyperparameters, TrainLinks, TrainParameters
from NN_Scripts.Test_EvaluationMetric import evaluation_metric_TSNE, TSNEResultData, plot_dataset_TSNE
from NN_Scripts.Discriminator_evaluation import measure_discriminator_from_params, show_gistogramm
from NN_Scripts.leave1out import leave1out
import matplotlib.pyplot as plt

import torch


class TrainStatusLogger:

    def __init__(self, train_links: TrainLinks, hyperparameters: Hyperparameters) -> None:
        self.last_tsne_data: TSNEResultData
        self.train_links = train_links
        self.hyperparameters = hyperparameters
        self.leave1out_results_y = []
        self.leave1out_epoch_x = []
        self.d_losses_list = []
        self.g_losses_list = []
        self.difference_results_y = []
        self.difference_epoch_x = []
        self.unique_mean_results_y = []
        self.unique_mean_epoch_x = []
        self.tsne_y = []
        self.tsne_epoch_x = []

    def train_log_callback(self, epoch_i: int, epoches_total: int):

        if epoch_i == 0:
            generation_helper = GenerationHelper(self.train_links.parser_tool,
                                                 self.hyperparameters.noise_size,
                                                 self.train_links.generator,
                                                 self.train_links.discriminator)
            plot_dataset_TSNE(generation_helper)

        need_print_temp_results = (epoch_i + 1) % (4) == 0
        if not need_print_temp_results:
            need_print_temp_results = epoch_i == 0

        self.log_epoch_losses()

        if need_print_temp_results:
            print(f'Epoch [{epoch_i}/{epoches_total}]')

            self.evaluation_metric(epoch_i)
            self.log_nn_weights(epoch_i)
            self.log_leave1out(epoch_i)
            self.log_epoch_temp_results(epoch_i, epoches_total)
            # log_difference(epoch)

    def log_epoch_losses(self):
        if self.train_links.d_loss is None:
            self.d_losses_list.append(0)
        else:
            self.d_losses_list.append(self.train_links.d_loss.item())

        self.g_losses_list.append(self.train_links.g_loss.item())

    def log_nn_weights(self, epoch_i: int):
        torch.save(self.train_links.generator.state_dict(), Config.get_generator_savename(epoch_i))
        torch.save(self.train_links.discriminator.state_dict(), Config.get_discriminator_savename(epoch_i))

    def log_epoch_temp_results(self, epoch_i: int, num_epochs: int):
        print(f'D_loss: {self.train_links.d_loss.item():.4f}, G_loss: {self.train_links.g_loss.item():.4f}')

        samples_count = 20
        fake_results, real_results, fake_mean_set = measure_discriminator_from_params(self.train_links,
                                                                                      self.hyperparameters.noise_size,
                                                                                      samples_count, epoch_i)
        self.unique_mean_results_y.append(len(fake_mean_set))
        self.unique_mean_epoch_x.append(epoch_i)

        plt.clf()

        plt.figure(figsize=(20, 10))
        # Первый график
        plt.subplot(2, 2, 1)
        self.losses_plot()

        # Второй график
        plt.subplot(2, 2, 2)
        plt.plot(self.tsne_epoch_x, self.tsne_y, label='tsne', color='blue')
        plt.xlabel('tsne_epoch_x')
        plt.ylabel('tsne_y')
        plt.title('График tsne')
        plt.legend()


        plt.subplot(2, 2, 4)
        color_map = {0: 'red', 1: 'blue', 2: 'green', 3: 'purple'}
        plt.title("TSNE Epoch" + str(self.last_tsne_data.epoch) + " metric: " + str(self.last_tsne_data.tsne_metric))
        sns.scatterplot(x=self.last_tsne_data.tsne_points[:, 0], y=self.last_tsne_data.tsne_points[:, 1],
                        hue=self.last_tsne_data.labels, palette=color_map)

        plt.subplot(2, 2, 3)
        show_gistogramm(epoch_i, fake_results, real_results)



    def log_leave1out(self, epoch_i: int):
        leave1out_output = leave1out(self.train_links.generator, self.train_links.listsOfFloats,
                                     self.train_links.listOfTags)
        self.leave1out_results_y.append(leave1out_output)
        self.leave1out_epoch_x.append(epoch_i)
        print(f'leave1out: {leave1out_output}')

    def show_final_losses_plot(self):

        fig, ax1 = plt.subplots()
        self.losses_plot()
        self.save_loss_graph()
        self.print_best_tsne()

    def print_best_tsne(self):
        # Объединяем значения из обоих списков в пары
        combined_data = list(zip(self.tsne_y, self.tsne_epoch_x))

        # Сортируем пары по значениям из tsne_y
        sorted_data = sorted(combined_data, key=lambda x: x[0], reverse=True)

        print("Значения tsne_y и их пары из tsne_epoch_x:")
        for pair in sorted_data:
            print(pair)

    def losses_plot(self):

        ax1 = plt.gca()
        fig = plt.gcf()
        ax1.set_title("Generator and Discriminator Loss During Training")

        ax1.set_xlabel("iterations")
        ax1.set_ylabel("Loss")
        ax1.plot(self.d_losses_list, label="D")
        ax1.plot(self.g_losses_list, label="G")

        color = 'tab:red'
        ax2 = ax1.twinx()  # instantiate a second axes that shares the same x-axis
        ax2.plot(self.unique_mean_epoch_x, self.unique_mean_results_y, alpha=.1, label="unique_mean_results",
                 color=color)
        ax2.set_ylabel("unique_mean_results", color=color)
        ax2.tick_params(axis='y', labelcolor=color)

        color = 'tab:green'
        ax3 = ax1.twinx()
        ax3.plot(self.leave1out_epoch_x, self.leave1out_results_y, alpha=.1, marker='o', label="classifier",
                 color=color)
        ax3.set_ylabel("leave1out", color=color)
        ax3.tick_params(axis='y', labelcolor=color)

        # Добавление подписей к точкам с чередованием
        for i, (x, y) in enumerate(zip(self.leave1out_epoch_x, self.leave1out_results_y)):
            offset = (0, 10) if i % 2 == 0 else (0, -14)  # Изменение смещения в зависимости от индекса
            plt.annotate(f'({x})', (x, y), textcoords="offset points", alpha=.1, xytext=offset, ha='center')

        lines_labels = [ax.get_legend_handles_labels() for ax in fig.axes]
        lines, labels = [sum(lol, []) for lol in zip(*lines_labels)]
        ax1.legend(lines, labels)

    def save_loss_graph(self):
        fig1 = plt.gcf()
        fig1.savefig(Config.SAVE_PATH_OBJECT / "loss_graph.png")
        plt.show()

    def evaluation_metric(self, epoch_i):

        generation_helper = GenerationHelper(self.train_links.parser_tool,
                                             self.hyperparameters.noise_size,
                                             self.train_links.generator,
                                             self.train_links.discriminator)

        tsne_data = evaluation_metric_TSNE(generation_helper, epoch_i)
        self.tsne_y.append(tsne_data.tsne_metric)
        self.tsne_epoch_x.append(epoch_i)
        self.last_tsne_data = tsne_data
        return tsne_data
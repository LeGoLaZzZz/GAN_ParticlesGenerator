import torch
from NN_Scripts.SaveNNparams import Hyperparameters, TrainLinks, TrainParameters
from NN_Scripts.TrainStatusLogger import TrainStatusLogger


def wgan_loss_train(hyper_parameters: Hyperparameters, train_links: TrainLinks, train_parameters: TrainParameters,
                    logger: TrainStatusLogger):
    num_epochs = train_parameters.num_epochs
    critic_iterations = train_parameters.critic_iterations
    generator_iterations = train_parameters.generator_iterations
    batch_size = train_parameters.batch_size
    weight_clip = train_parameters.weight_clip

    noise_size = hyper_parameters.noise_size

    real_data_loader = train_links.real_data_loader
    d_optimizer = train_links.d_optimizer
    g_optimizer = train_links.g_optimizer
    generator = train_links.generator
    discriminator = train_links.discriminator

    for epoch in range(num_epochs):

        for real_batch, real_labels in real_data_loader:
            real = real_batch.cuda()
            labels = real_labels.cuda()

            for _ in range(critic_iterations):
                noise = torch.randn(batch_size, noise_size).cuda()
                fake = generator(noise, labels)
                critic_real = discriminator(real, labels).reshape(-1)
                critic_fake = discriminator(fake, labels).reshape(-1)

                # wgan loss
                train_links.d_loss = -(torch.mean(critic_real) - torch.mean(critic_fake))

                discriminator.zero_grad()
                train_links.d_loss.backward(retain_graph=True)
                d_optimizer.step()

                for parameter in discriminator.parameters():
                    parameter.data.clamp_(-weight_clip, weight_clip)

            for _ in range(generator_iterations):
                noise = torch.randn(batch_size, noise_size).cuda()
                fake = generator(noise, labels)

                output = discriminator(fake, labels).reshape(-1)

                # wgan loss
                train_links.g_loss = -torch.mean(output)

                generator.zero_grad()
                train_links.g_loss.backward(retain_graph=True)
                g_optimizer.step()

        logger.train_log_callback(epoch, num_epochs)


def default_loss_train(hyper_parameters: Hyperparameters, train_links: TrainLinks, train_parameters: TrainParameters,
                       logger: TrainStatusLogger):
    num_epochs = train_parameters.num_epochs
    critic_iterations = train_parameters.critic_iterations
    generator_iterations = train_parameters.generator_iterations
    batch_size = train_parameters.batch_size
    weight_clip = train_parameters.weight_clip

    noise_size = hyper_parameters.noise_size

    real_data_loader = train_links.real_data_loader
    d_optimizer = train_links.d_optimizer
    g_optimizer = train_links.g_optimizer
    generator = train_links.generator
    discriminator = train_links.discriminator
    criterion = train_links.criterion

    for epoch in range(num_epochs):
        need_print_temp_results = (epoch + 1) % (num_epochs / 20) == 0

        for real_batch, real_labels in real_data_loader:

            real = real_batch.cuda()
            labels = real_labels.cuda()

            for _ in range(critic_iterations):
                noise = torch.randn(batch_size, noise_size).cuda()
                fake = generator(noise, labels)
                critic_real = discriminator(real, labels).reshape(-1)
                critic_fake = discriminator(fake, labels).reshape(-1)

                loss_discriminator_real = criterion(critic_real, torch.ones_like(critic_real))
                loss_discriminator_fake = criterion(critic_fake, torch.zeros_like(critic_fake))

                discriminator.zero_grad()  # ???

                train_parameters.d_loss = (loss_discriminator_fake + loss_discriminator_real) / 2
                train_parameters.d_loss.backward(retain_graph=True)
                d_optimizer.step()

            for _ in range(generator_iterations):
                noise = torch.randn(batch_size, noise_size).cuda()
                fake = generator(noise, labels)

                output = discriminator(fake, labels).reshape(-1)
                train_parameters.g_loss = criterion(output, torch.ones_like(output))

                generator.zero_grad()  # ???
                # g_optimizer.zero_grad()
                train_parameters.g_loss.backward(retain_graph=True)
                g_optimizer.step()

        logger.log_epoch_losses(train_links)

        if need_print_temp_results:
            logger.log_nn_weights(epoch, train_links)
            logger.log_epoch_temp_results(epoch, num_epochs, train_links)
            logger.log_leave1out(epoch, train_links)
            # log_difference(epoch)

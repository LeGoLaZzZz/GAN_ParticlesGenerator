from pathlib import Path

NN_NAME = r"TempEpoches"
SAVE_PATH = r"..\BackUps"
SAVE_PATH_OBJECT = Path(SAVE_PATH) / NN_NAME

DATASET_FILE = r"datasets/Dataset_MinMax_Gradient_Forms.json"

def get_generator_savename(epoch_num: int) -> Path:
    return SAVE_PATH_OBJECT / f"generator_epoch_{epoch_num}.pth"


def get_discriminator_savename(epoch_num: int) -> Path:
    return SAVE_PATH_OBJECT / f"discriminator_epoch_{epoch_num}.pth"

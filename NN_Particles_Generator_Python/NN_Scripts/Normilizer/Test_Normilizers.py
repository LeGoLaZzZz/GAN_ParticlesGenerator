from NN_Scripts import Config
from NN_Scripts.ParticlesJsonParser import ParticlesJsonParserTool


parser_tool = ParticlesJsonParserTool()
print("LOAD DATASET")
parser_tool.load_dataset("../"+Config.DATASET_FILE)

print("debug_normalizer_output___________________________")
parser_tool.debug_normalizer_output()


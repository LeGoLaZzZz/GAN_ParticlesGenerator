using NNParticleSystemGenerator.DataSetGenerator.Editor;
using UnityEngine;

namespace NNParticleSystemGenerator
{
    public class GridParticleCell : MonoBehaviour
    {
        private int _indexInDataset;
        private DatasetParticles _datasetParticles;

        public int IndexInDataset => _indexInDataset;
        public ParticleTags GetTags() => _datasetParticles.ParsedDataset[_indexInDataset].tags;

        public void Initialize(int indexInDataset, DatasetParticles datasetParticles)
        {
            _datasetParticles = datasetParticles;
            _indexInDataset = indexInDataset;
        }

        public void UpdateTags(ParticleTags tags)
        {
            _datasetParticles.UpdateTags(_indexInDataset, tags);
        }

        public void UpdateForm(ParticleForm form)
        {
            var particleTags = _datasetParticles.ParsedDataset[_indexInDataset].tags;
            particleTags.form = form;
            _datasetParticles.UpdateTags(_indexInDataset, particleTags);
        }
        
        public void UpdateColorGroup(ParticleColorGroup colorGroup)
        {
            var particleTags = _datasetParticles.ParsedDataset[_indexInDataset].tags;
            particleTags.colorGroup = colorGroup;
            _datasetParticles.UpdateTags(_indexInDataset, particleTags);
        }
    }
}
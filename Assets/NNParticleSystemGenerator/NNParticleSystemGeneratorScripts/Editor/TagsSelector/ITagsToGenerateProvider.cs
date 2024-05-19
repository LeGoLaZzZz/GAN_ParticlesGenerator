using System.Collections.Generic;
using NNParticleSystemGenerator.DataSetGenerator.Editor;

namespace NNParticleSystemGenerator.Editor
{
    public interface ITagsToGenerateProvider
    {
        public List<ParticleTags> GetTagsToGenerate();
        public TagsProvidedType GetTagsProvidedType();
    }
}
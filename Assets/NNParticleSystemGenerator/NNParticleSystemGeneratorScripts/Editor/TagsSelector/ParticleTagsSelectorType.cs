using System.Collections.Generic;
using NNParticleSystemGenerator.DataSetGenerator.Editor;

namespace NNParticleSystemGenerator.Editor
{
    public abstract class ParticleTagsSelectorType : ITagsToGenerateProvider, IEditorPrefsSaveLoad
    {
        public abstract void Draw();
        public abstract List<ParticleTags> GetTagsToGenerate();
        public abstract TagsProvidedType GetTagsProvidedType();

        public abstract void SaveEditorPrefs();
        public abstract void LoadEditorPrefs();
    }
}
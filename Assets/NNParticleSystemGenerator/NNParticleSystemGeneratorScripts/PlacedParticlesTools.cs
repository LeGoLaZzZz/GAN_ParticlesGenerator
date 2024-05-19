using NNParticleSystemGenerator.DataSetGenerator.Editor;
using SmartAttributes.InspectorButton;
using SmartAttributes.MultiDraft.Attributes;
using UnityEditor;
using UnityEngine;

[InspectorButtonClass]
public class PlacedParticlesTools : MonoBehaviour
{
    [SerializeField, Required] private GridDatasetPlacer gridDatasetPlacer;
    [SerializeField] private ParticleForm particleForm = ParticleForm.NoneRandom;

    [InspectorButton("ALERT_SetAllForms")]
    private void ALERT_SetAllForms()
    {
        Undo.RecordObject(gridDatasetPlacer.DatasetParticles, "ALERT_ClearAllForms");
        foreach (var gridParticleCell in gridDatasetPlacer.GridParticleCells)
        {
            gridParticleCell.UpdateForm(particleForm);
        }
    }
}
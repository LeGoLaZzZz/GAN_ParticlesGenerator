using System;
using System.Collections.Generic;
using System.Linq;
using NNParticleSystemGenerator;
using NNParticleSystemGenerator.DataSetGenerator.Editor;
using SmartAttributes.InspectorButton;
using SmartAttributes.MultiDraft.Attributes;
using UnityEditor;
using UnityEngine;

[InspectorButtonClass]
public class GridDatasetPlacer : MonoBehaviour
{
    [SerializeField, Required] private ParticleSpawner particleSpawner;
    [SerializeField, Required] private GridPlacer gridPlacer;
    [SerializeField, Required] private DatasetParticles datasetParticles;
    [SerializeField, Required] private Transform spawnParent;
    [SerializeField] private int placeIndexFrom;
    [SerializeField] private int placeCount = 50;
    public List<GridParticleCell> GridParticleCells { get; private set; }
    public DatasetParticles DatasetParticles => datasetParticles;

    [InspectorButton("SpawnGrid Next")]
    public void SpawnNext()
    {
        placeIndexFrom += placeCount;
        if (placeIndexFrom > datasetParticles.ParsedDatasetParticles.Count)
            placeIndexFrom = datasetParticles.ParsedDatasetParticles.Count;
        SpawnGrid();
    }

    [InspectorButton("SpawnGrid Previous")]
    public void SpawnPrevious()
    {
        placeIndexFrom -= placeCount;
        if (placeIndexFrom < 0) placeIndexFrom = 0;
        SpawnGrid();
    }

    [InspectorButton("SpawnGrid")]
    public void SpawnGrid()
    {
        Clear();

        var fromIndex = placeIndexFrom;
        var toIndex = Mathf.Clamp((placeIndexFrom + placeCount), 0, datasetParticles.ParsedDatasetParticles.Count);
        var particles = new List<GameObject>();
        GridParticleCells = new List<GridParticleCell>();
        for (int i = fromIndex; i < toIndex; i++)
        {
            var particle = datasetParticles.ParsedDatasetParticles[i];

            var instance = particleSpawner.SpawnParticle(i, datasetParticles, spawnParent);
            var gridParticleCell = instance.gameObject.AddComponent<GridParticleCell>();

            GridParticleCells.Add(gridParticleCell);
            gridParticleCell.Initialize(i, datasetParticles);
            var instanceMain = instance.main;
            instanceMain.loop = true;
            instanceMain.duration = 3;

            instance.transform.position = Vector3.zero;
            DestroyChildren(instance.transform);
            particles.Add(instance.gameObject);
        }

        gridPlacer.PlaceGrid(particles);
        Selection.objects = particles.ToArray();
    }

    private void DestroyChildren(Transform parent)
    {
        var childCount = parent.childCount;
        for (var i = 0; i < childCount; i++)
        {
            DestroyImmediate(parent.GetChild(0).gameObject);
        }
    }

    [InspectorButton("Clear")]
    private void Clear()
    {
        DestroyChildren(spawnParent);
    }

    [InspectorButton("Debug_PlaceGroups")]
    private void Debug_PlaceGroups()
    {
        Clear();

        var fromIndex = placeIndexFrom;
        var particles = new List<GameObject>();
        GridParticleCells = new List<GridParticleCell>();

        var particleForms = Enum.GetValues(typeof(ParticleForm)).Cast<ParticleForm>().ToList();

        foreach (var form in particleForms)
        {
            var particlesForm = GetParticlesIndexesByForm(form, placeCount / particleForms.Count, fromIndex);
            foreach (var systemIndex in particlesForm)
            {
                var instance = particleSpawner.SpawnParticle(systemIndex, datasetParticles, spawnParent);
                var gridParticleCell = instance.gameObject.AddComponent<GridParticleCell>();
                GridParticleCells.Add(gridParticleCell);

                gridParticleCell.Initialize(systemIndex, datasetParticles);
                var instanceMain = instance.main;
                instanceMain.loop = true;
                instanceMain.duration = 3;

                instance.transform.position = Vector3.zero;
                DestroyChildren(instance.transform);
                particles.Add(instance.gameObject);
            }
        }

        gridPlacer.PlaceGrid(particles);
        Selection.objects = particles.ToArray();

        List<int> GetParticlesIndexesByForm(ParticleForm form, int count, int fromIndex)
        {
            var returnParticles = new List<int>();
            for (int i = fromIndex; i < datasetParticles.ParsedDataset.Count; i++)
            {
                if (returnParticles.Count >= count)
                    return returnParticles;

                var particleTagPair = datasetParticles.ParsedDataset[i];
                if (particleTagPair.tags.form == form)
                    returnParticles.Add(i);
            }

            return returnParticles;
        }
    }
}
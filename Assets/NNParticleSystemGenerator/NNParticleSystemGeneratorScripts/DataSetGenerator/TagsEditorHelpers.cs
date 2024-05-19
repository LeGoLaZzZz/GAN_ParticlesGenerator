using System;
using System.Collections.Generic;
using UnityEngine;

namespace NNParticleSystemGenerator.DataSetGenerator.Editor
{
    using UnityEngine;

    public static class ParticleColorAnalyzer
    {
        private static readonly Dictionary<ParticleColorGroup, Color[]> centroids =
            new Dictionary<ParticleColorGroup, Color[]>()
            {
                {
                    ParticleColorGroup.Orange_Red_Yellow,
                    new Color[]
                    {
                        new Color(1f, 0f, 0.1f),
                        new Color(1f, 0f, 0.2f),
                        new Color(1f, 0f, 0.3f),
                        new Color(1f, 0f, 0f),
                        new Color(1f, 0.1f, 0f),
                        new Color(1f, 0.2f, 0f),
                        new Color(1f, 0.3f, 0f),
                        new Color(1f, 0.4f, 0f),
                        new Color(1f, 0.5f, 0f),
                        new Color(1f, 0.6f, 0f),
                        new Color(1f, 0.7f, 0f),
                        new Color(1f, 0.8f, 0f),
                        new Color(1f, 0.9f, 0f),
                        new Color(1f, 1f, 0f),
                        new Color(0.9f, 0.9f, 0f),
                        new Color(0.8f, 0.8f, 0f),
                        new Color(0.7f, 0.7f, 0f),
                        new Color(0.6f, 0.6f, 0f),
                        new Color(0.5f, 0.5f, 0f),
                        new Color(1f, 1f, 0.1f),
                        new Color(1f, 1f, 0.2f),
                        new Color(1f, 1f, 0.3f),
                        new Color(1f, 1f, 0.4f),
                        new Color(1f, 1f, 0.5f),
                        new Color(1f, 0.9f, 0.9f),
                        new Color(1f, 0.9f, 0.8f),
                        new Color(1f, 0.9f, 0.7f),
                        new Color(1f, 0.9f, 0.6f),
                        new Color(1f, 0.9f, 0.5f),
                        new Color(1f, 0.9f, 0.4f),
                        new Color(1f, 0.9f, 0.3f),
                        new Color(1f, 0.9f, 0.2f),
                        new Color(1f, 0.9f, 0.1f),
                        new Color(1f, 0.8f, 0.8f),
                        new Color(1f, 0.8f, 0.7f),
                        new Color(1f, 0.8f, 0.6f),
                        new Color(1f, 0.8f, 0.5f),
                        new Color(1f, 0.8f, 0.4f),
                        new Color(1f, 0.8f, 0.3f),
                        new Color(1f, 0.8f, 0.2f),
                        new Color(1f, 0.8f, 0.1f),
                        new Color(1f, 0.7f, 0.7f),
                        new Color(1f, 0.7f, 0.6f),
                        new Color(1f, 0.7f, 0.5f),
                        new Color(1f, 0.7f, 0.4f),
                        new Color(1f, 0.7f, 0.3f),
                        new Color(1f, 0.7f, 0.2f),
                        new Color(1f, 0.7f, 0.1f),
                        new Color(1f, 0.6f, 0.6f),
                        new Color(1f, 0.6f, 0.5f),
                        new Color(1f, 0.6f, 0.4f),
                        new Color(1f, 0.6f, 0.3f),
                        new Color(1f, 0.6f, 0.2f),
                        new Color(1f, 0.6f, 0.1f),
                        new Color(1f, 0.5f, 0.5f),
                        new Color(1f, 0.5f, 0.4f),
                        new Color(1f, 0.5f, 0.3f),
                        new Color(1f, 0.5f, 0.2f),
                        new Color(1f, 0.5f, 0.1f),
                        new Color(1f, 0.4f, 0.4f),
                        new Color(1f, 0.4f, 0.3f),
                        new Color(1f, 0.4f, 0.2f),
                        new Color(1f, 0.4f, 0.1f),
                        new Color(1f, 0.3f, 0.3f),
                        new Color(1f, 0.3f, 0.2f),
                        new Color(1f, 0.3f, 0.1f),
                        new Color(1f, 0.8f, 0.8f),
                        new Color(1f, 0.7f, 0.7f),
                        new Color(1f, 0.6f, 0.6f),
                        new Color(1f, 0.5f, 0.5f),
                        new Color(1f, 0.4f, 0.4f),
                        new Color(1f, 0.3f, 0.3f),
                        new Color(1f, 0.2f, 0.2f),
                        new Color(1f, 0.1f, 0.1f),
                        new Color(0.9f, 0.1f, 0.1f),
                        new Color(0.9f, 0.2f, 0.2f),
                        new Color(0.9f, 0.3f, 0.3f),
                        new Color(0.9f, 0.4f, 0.4f),
                        new Color(0.9f, 0.5f, 0.5f),
                        new Color(0.9f, 0.6f, 0.6f),
                        new Color(0.9f, 0.7f, 0.7f),
                        new Color(0.8f, 0.1f, 0.1f),
                        new Color(0.7f, 0.1f, 0.1f),
                        new Color(0.7f, 0.6f, 0.1f),
                        new Color(0.7f, 0.6f, 0.5f),
                        new Color(0.7f, 0.5f, 0.5f),
                        new Color(0.7f, 0.5f, 0.6f),
                        new Color(0.6f, 0.1f, 0.1f),
                        new Color(0.5f, 0.1f, 0.1f),
                        new Color(0.4f, 0.1f, 0.1f),
                        new Color(0.3f, 0.1f, 0.1f),
                        new Color(0.2f, 0.1f, 0.1f),
                        new Color(0.9f, 0.5f, 0f),
                        new Color(0.8f, 0.5f, 0f),
                        new Color(0.7f, 0.5f, 0f),
                        new Color(0.9f, 0.3f, 0f),
                        new Color(0.9f, 0.4f, 0f),
                        new Color(0.6f, 0.4f, 0f),
                    }
                },
                {
                    ParticleColorGroup.Green, new Color[]
                    {
                        new Color(0.1f, 1f, 0.1f),
                        new Color(0.2f, 1f, 0.2f),
                        new Color(0.3f, 1f, 0.3f),
                        new Color(0.4f, 1f, 0.4f),
                        new Color(0.5f, 1f, 0.5f),
                        new Color(0.6f, 1f, 0.6f),
                        new Color(0.7f, 1f, 0.7f),
                        new Color(0.8f, 1f, 0.8f),
                        new Color(0f, 1f, 0f),
                        new Color(0.1f, 1f, 0f),
                        new Color(0.2f, 1f, 0f),
                        new Color(0.3f, 1f, 0f),
                        new Color(0.4f, 1f, 0f),
                        new Color(0.5f, 1f, 0f),
                        new Color(0.6f, 1f, 0f),
                        new Color(0.7f, 1f, 0f),
                        new Color(0f, 1f, 0.6f),
                        new Color(0f, 1f, 0.5f),
                        new Color(0f, 1f, 0.4f),
                        new Color(0f, 1f, 0.3f),
                        new Color(0f, 1f, 0.2f),
                        new Color(0f, 0.5f, 0f),
                        new Color(0f, 0.4f, 0f),
                        new Color(0f, 0.3f, 0f),
                        new Color(0f, 0.2f, 0f),
                        new Color(0f, 0.1f, 0f),
                        new Color(0.1f, 0.2f, 0f),
                        new Color(0.2f, 0.3f, 0f),
                        new Color(0.3f, 0.4f, 0f),
                        new Color(0.4f, 0.5f, 0f),
                        new Color(0.5f, 0.6f, 0f),
                        new Color(0.6f, 0.7f, 0f),
                        new Color(0.7f, 0.8f, 0f),
                        new Color(0.1f, 0.2f, 0.1f),
                        new Color(0.2f, 0.3f, 0.2f),
                        new Color(0.3f, 0.4f, 0.3f),
                        new Color(0.4f, 0.5f, 0.4f),
                        new Color(0.5f, 0.6f, 0.5f),
                        new Color(0.6f, 0.7f, 0.6f),
                    }
                },
                {
                    ParticleColorGroup.Blue_White, new Color[]
                    {
                        new Color(1f, 1f, 1f),
                        new Color(0.9f, 0.9f, 0.9f),
                        new Color(0.8f, 0.8f, 0.8f),
                        new Color(0.7f, 0.7f, 0.7f),
                        new Color(0.6f, 0.6f, 0.6f),
                        new Color(0.5f, 0.5f, 0.5f),
                        new Color(0.4f, 0.4f, 0.4f),
                        new Color(0.3f, 0.3f, 0.3f),
                        new Color(0.2f, 0.2f, 0.2f),
                        new Color(0.1f, 0.1f, 0.1f),
                        new Color(0f, 0f, 0f),
                        new Color(0.9f, 1f, 1f),
                        new Color(0.8f, 1f, 1f),
                        new Color(0.7f, 1f, 1f),
                        new Color(0.6f, 1f, 1f),
                        new Color(0.5f, 1f, 1f),
                        new Color(0.4f, 1f, 1f),
                        new Color(0.3f, 1f, 1f),
                        new Color(0.2f, 1f, 1f),
                        new Color(0.1f, 1f, 1f),
                        new Color(0f, 1f, 1f),
                        new Color(0.9f, 0.9f, 1f),
                        new Color(0f, 0.9f, 1.0f),
                        new Color(0f, 0.8f, 1.0f),
                        new Color(0f, 0.7f, 1.0f),
                        new Color(0f, 0.6f, 1.0f),
                        new Color(0f, 0.5f, 1.0f),
                        new Color(0f, 0.4f, 1.0f),
                        new Color(0f, 0.3f, 1.0f),
                        new Color(0f, 0.2f, 1.0f),
                        new Color(0f, 0.1f, 1.0f),
                        new Color(0f, 0f, 1f),
                        new Color(0f, 0f, 0.9f),
                        new Color(0f, 0f, 0.8f),
                        new Color(0f, 0f, 0.7f),
                        new Color(0f, 0f, 0.6f),
                        new Color(0f, 0f, 0.5f),
                        new Color(0f, 0f, 0.4f),
                        new Color(0f, 0f, 0.3f),
                        new Color(0f, 0f, 0.2f),
                        new Color(0f, 0f, 0.1f),
                    }
                },
                {
                    ParticleColorGroup.Purple,
                    new Color[]
                    {
                        new Color(1f, 0.1f, 1f),
                        new Color(1f, 0.2f, 1f),
                        new Color(1f, 0.3f, 1f),
                        new Color(1f, 0.4f, 1f),
                        new Color(1f, 0.5f, 1f),
                        new Color(1f, 0.6f, 1f),
                        new Color(1f, 0f, 1f),
                        new Color(1f, 0f, 0.9f),
                        new Color(1f, 0f, 0.8f),
                        new Color(1f, 0f, 0.7f),
                        new Color(1f, 0f, 0.6f),
                        new Color(1f, 0f, 0.5f),
                        new Color(1f, 0f, 0.4f),
                        new Color(0.9f, 0f, 1f),
                        new Color(0.8f, 0f, 1f),
                        new Color(0.7f, 0f, 1f),
                        new Color(0.6f, 0f, 1f),
                        new Color(0.4f, 0f, 1f),
                        new Color(0.4f, 0f, 1f),
                        new Color(0.3f, 0f, 1f),
                        new Color(0.9f, 0f, 0.9f),
                        new Color(0.8f, 0f, 0.8f),
                        new Color(0.7f, 0f, 0.7f),
                        new Color(0.6f, 0f, 0.6f),
                        new Color(0.5f, 0f, 0.5f),
                        new Color(0.4f, 0f, 0.4f),
                        new Color(0.3f, 0f, 0.3f),
                        new Color(0.2f, 0f, 0.2f),
                        new Color(0.1f, 0f, 0.1f),
                    }
                }
            };
        // Define centroids for each color group

        public static ParticleColorGroup ClassifyColorGroup(Color color)
        {
            ParticleColorGroup closestGroup = ParticleColorGroup.Blue_White; // Default group
            float minDistance = float.MaxValue;

            foreach (var kvp in centroids)
            {
                foreach (Color centroid in kvp.Value)
                {
                    float distance = ColorDistance(color, centroid);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        closestGroup = kvp.Key;
                    }
                }
            }

            return closestGroup;
        }

        // Function to calculate Euclidean distance between two colors
        private static float ColorDistance(Color a, Color b)
        {
            float rDiff = a.r - b.r;
            float gDiff = a.g - b.g;
            float bDiff = a.b - b.b;
            return Mathf.Sqrt(rDiff * rDiff + gDiff * gDiff + bDiff * bDiff);
        }


        public static Color GetAverageColor(ParticleSystem particleSystem)
        {
            ParticleSystem.MainModule mainModule = particleSystem.main;
            ParticleSystem.ColorOverLifetimeModule colorOverLifetimeModule = particleSystem.colorOverLifetime;

            // Calculate average color from main module
            Color mainModuleAverageColor = CalculateAverageColorFromGradient(mainModule.startColor);

            // Calculate average color from color over lifetime module
            Color colorOverLifetimeAverageColor = CalculateAverageColorFromGradient(colorOverLifetimeModule.color);

            Color overallAverageColor = mainModuleAverageColor;
            if (colorOverLifetimeModule.enabled)
            {
                overallAverageColor = (mainModuleAverageColor + colorOverLifetimeAverageColor) / 2f;
            }

            // Return the overall average color
            return overallAverageColor;
        }

        public static Color CalculateAverageColorFromGradient(ParticleSystem.MinMaxGradient minMaxGradient)
        {
            int sampleCount = 10; // Adjust the sample count as needed

            Color sumColor = Color.black;
            for (int i = 0; i < sampleCount; i++)
            {
                float time = i / (sampleCount - 1f); // Evaluate at different times
                sumColor += minMaxGradient.Evaluate(time);
            }

            // Calculate the average color
            Color averageColor = sumColor / sampleCount;

            return averageColor;
        }
    }
}
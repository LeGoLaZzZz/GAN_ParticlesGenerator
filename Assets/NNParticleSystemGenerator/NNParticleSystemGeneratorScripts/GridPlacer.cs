using System.Collections.Generic;
using SmartAttributes.InspectorButton;
using UnityEngine;


[InspectorButtonClass]
public class GridPlacer : MonoBehaviour
{
    [SerializeField] private int rows = 5; // Number of rows in the grid
    [SerializeField] private int columns = 5; // Number of columns in the grid
    [SerializeField] private float offsetX = 1f; // Offset between columns
    [SerializeField] private float offsetY = 1f; // Offset between rows

    [SerializeField] private List<GameObject> debug_gameObjects;

    [InspectorButton("PlaceGrid")]
    public void Debug_PlaceGrid()
    {
        PlaceGrid(debug_gameObjects);
    }
    
    public void PlaceGrid(List<GameObject> gameObjects)
    {
        int objectIndex = 0; // Index for cycling through the prefabs list
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                Vector3 spawnPosition = new Vector3(col * offsetX, row * offsetY, 0f);
                var objectToPlace = gameObjects[objectIndex % gameObjects.Count];
                objectToPlace.transform.position = spawnPosition;
                objectIndex++;
                if (objectIndex >= gameObjects.Count)
                    return;
            }
        }
    }
}
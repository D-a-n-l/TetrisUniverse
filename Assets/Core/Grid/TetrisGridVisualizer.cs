using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrisGridVisualizer : MonoBehaviour
{
    public GameObject gridCubePrefab; // Префаб для отображения ячейки сетки
    public int width = 10;
    public int height = 20;
    public int depth = 10;

    void Start()
    {
        CreateGrid();
    }

    private void CreateGrid()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < depth; z++)
                {
                    Vector3 position = new Vector3(x, y, z);
                    Instantiate(gridCubePrefab, position, Quaternion.identity, transform);
                }
            }
        }
    }
}
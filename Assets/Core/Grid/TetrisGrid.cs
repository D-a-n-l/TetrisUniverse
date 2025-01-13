using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrisGrid : MonoBehaviour
{
    [SerializeField]
    private Vector3Int _radius;

    public Vector3Int Radius => _radius;

    private Transform[,,] grid; // ��������� ������ ��� �������� ������

    private void Start()
    {
        grid = new Transform[_radius.x, _radius.y, _radius.z];

        GlobalEvents.OnMovementFinished += ClearFullRows;
    }

    // ��������, ��������� �� ������� ������ �����
    public bool IsInsideGrid(Vector3 position)
    {
        return position.x >= 0 && position.x < _radius.x &&
               position.y >= 0 && position.y < _radius.y &&
               position.z >= 0 && position.z < _radius.z;
    }

    // ��������, ������ �� ������
    public bool IsCellOccupied(Vector3 position)
    {
        if (!IsInsideGrid(position)) return true; // �� ��������� ����� ������ ������
        return grid[(int)position.x, (int)position.y, (int)position.z] != null;
    }

    // ���������� ����� � �����
    public void AddBlockToGrid(Transform block)
    {
        Vector3 position = RoundVector(block.position);

        if (IsInsideGrid(position))
        {
            grid[(int)position.x, (int)position.y, (int)position.z] = block;
        }
    }

    // �������� ����� �� �����
    public void RemoveBlockFromGrid(Transform block)
    {
        Vector3 position = RoundVector(block.position);
        if (IsInsideGrid(position))
        {
            grid[(int)position.x, (int)position.y, (int)position.z] = null;
        }
    }

    // ���������� ������� �� ����� �����
    private Vector3 RoundVector(Vector3 vector)
    {
        return new Vector3(Mathf.Round(vector.x), Mathf.Round(vector.y), Mathf.Round(vector.z));
    }

    // ������� ��������� ����������� ������ (�� ������ Y)
    public void ClearFullRows()
    {
        for (int y = 0; y < _radius.y; y++)
        {
            if (IsRowFull(y))
            {
                ClearRow(y);
                MoveRowsDown(y);
            }
        }
    }

    // ��������, ��������� �� ������
    private bool IsRowFull(int y)
    {
        for (int x = 0; x < _radius.x; x++)
        {
            for (int z = 0; z < _radius.z; z++)
            {
                if (grid[x, y, z] == null)
                {
                    return false;
                }
            }
        }
        return true;
    }

    // ������� ������
    private void ClearRow(int y)
    {
        for (int x = 0; x < _radius.x; x++)
        {
            for (int z = 0; z < _radius.z; z++)
            {
                if (grid[x, y, z] != null)
                {
                    Destroy(grid[x, y, z].gameObject);
                    grid[x, y, z] = null;
                }
            }
        }
    }

    // ����� ����� ����
    private void MoveRowsDown(int clearedRow)
    {
        for (int y = clearedRow; y < _radius.y - 1; y++)
        {
            for (int x = 0; x < _radius.x; x++)
            {
                for (int z = 0; z < _radius.z; z++)
                {
                    if (grid[x, y + 1, z] != null)
                    {
                        Transform block = grid[x, y + 1, z];
                        grid[x, y + 1, z] = null;
                        grid[x, y, z] = block;
                        block.position += Vector3.down;
                    }
                }
            }
        }
    }
}
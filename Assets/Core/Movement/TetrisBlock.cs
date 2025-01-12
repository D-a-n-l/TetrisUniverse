using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class TetrisBlock : MonoBehaviour
{
    public float fallSpeed = 1.0f; // �������� �������
    private float fallTimer;

    [SerializeField]
    private Transform[] _tiles;

    private TetrisGrid _grid;

    [Inject]
    private void Construct(TetrisGrid grid)
    {
        _grid = grid;
    }

    void Update()
    {
        HandleInput();
        HandleFalling();
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow)) Move(Vector3.left);
        else if (Input.GetKeyDown(KeyCode.RightArrow)) Move(Vector3.right);
        else if (Input.GetKeyDown(KeyCode.W)) Move(Vector3.forward);
        else if (Input.GetKeyDown(KeyCode.S)) Move(Vector3.back);
        else if (Input.GetKeyDown(KeyCode.DownArrow)) Move(Vector3.down);
        else if (Input.GetKeyDown(KeyCode.UpArrow)) Rotate(Vector3.up);
        else if (Input.GetKeyDown(KeyCode.Q)) Rotate(Vector3.left);
        else if (Input.GetKeyDown(KeyCode.E)) Rotate(Vector3.forward);
    }

    private void HandleFalling()
    {
        fallTimer += Time.deltaTime;
        if (fallTimer >= fallSpeed)
        {
            if (!Move(Vector3.down)) // ���� ��������� ���� ������, ���������� ������
            {
                AddToGrid();
                //CheckForCompleteRows();
                SpawnNextBlock();
            }
            fallTimer = 0;
        }
    }

    private bool Move(Vector3 direction)
    {
        transform.position += direction;
        if (!IsValidPosition())
        {
            transform.position -= direction; // ������� �� �����
            return false;
        }
        return true;
    }

    private void Rotate(Vector3 axis)
    {
        transform.Rotate(axis * 90);
        if (!IsValidPosition())
        {
            transform.Rotate(-axis * 90); // �������� ��������
        }
    }

    private bool IsValidPosition()
    {
        foreach (Transform child in transform)
        {
            Vector3 position = RoundVector(child.position);
            if (!_grid.IsInsideGrid(position) || _grid.IsCellOccupied(position))
            {
                return false;
            }
        }
        return true;
    }

    private void AddToGrid()
    {
        foreach (Transform child in _tiles)
        {
            child.SetParent(null); // ���������� ���� �� ��������

            Vector3 position = RoundVector(child.position);
            if (_grid.IsInsideGrid(position))
            {
                _grid.AddBlockToGrid(child); // ��������� �������� ���� � �����
            }

        }

        Destroy(gameObject); // ������� ������ ������ ������, ����� �������� � �����
    }

    private void CheckForCompleteRows()
    {
        _grid.ClearFullRows();
    }

    private void SpawnNextBlock()
    {
        Spawner.Instance.SpawnBlock(); // �����������, ���� GameManager ��� ���������� �������
    }

    private Vector3 RoundVector(Vector3 vector)
    {
        return new Vector3(Mathf.Round(vector.x), Mathf.Round(vector.y), Mathf.Round(vector.z));
    }
}
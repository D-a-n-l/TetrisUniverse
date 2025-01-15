using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class TetrisBlock : MonoBehaviour
{
    public float fallSpeed = 1.0f; // Скорость падения
    private float fallTimer;

    [SerializeField]
    private Transform[] _tiles;

    private TetrisGrid _grid;

    [Inject]
    private void Construct(TetrisGrid grid)
    {
        _grid = grid;
    }

    private void Start()
    {
        StopAllCoroutines();

        StartCoroutine(Fall());

        PlayerButtons.Instance.Forward.onClick.AddListener(() =>
        {
            Move(Vector3.forward);
        });

        PlayerButtons.Instance.Left.onClick.AddListener(() =>
        {
            Move(Vector3.left);
        });

        PlayerButtons.Instance.Back.onClick.AddListener(() =>
        {
            Move(Vector3.back);
        });

        PlayerButtons.Instance.Right.onClick.AddListener(() =>
        {
            Move(Vector3.right);
        });

        PlayerButtons.Instance.RotateX.onClick.AddListener(() =>
        {
            Rotate(Vector3.left);
        });

        PlayerButtons.Instance.RotateZ.onClick.AddListener(() =>
        {
            Rotate(Vector3.forward);
        });

        PlayerButtons.Instance.Fall.OnPressed.AddListener(NewMethod);

        PlayerButtons.Instance.Fall.OnDown.AddListener(() => { StopAllCoroutines(); });

        PlayerButtons.Instance.Fall.OnUp.AddListener(() => { StopAllCoroutines(); StartCoroutine(Fall()); });
    }

    //void Update()
    //{
    //    HandleInput();
    //    HandleFalling();
    //}

    private void NewMethod()
    {
        Move(Vector3.down);

        if (!Move(Vector3.down)) // Если двигаться вниз нельзя, остановить фигуру
        {
            AddToGrid();
            //CheckForCompleteRows();
            GlobalEvents.OnMovementFinished?.Invoke();
        }
    }

    public IEnumerator Fall()
    {
        yield return new WaitForSeconds(1f);

        Move(Vector3.down);

        if (!Move(Vector3.down)) // Если двигаться вниз нельзя, остановить фигуру
        {
            AddToGrid();
            //CheckForCompleteRows();
            GlobalEvents.OnMovementFinished?.Invoke();
        }

        StartCoroutine(Fall());
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
            if (!Move(Vector3.down)) // Если двигаться вниз нельзя, остановить фигуру
            {
                AddToGrid();
                //CheckForCompleteRows();
                GlobalEvents.OnMovementFinished?.Invoke();
            }
            fallTimer = 0;
        }
    }

    private bool Move(Vector3 direction)
    {
        transform.position += direction;

        if (!IsValidPosition())
        {
            transform.position -= direction; // Вернуть на место
            return false;
        }
        return true;
    }

    private void Rotate(Vector3 axis)
    {
        transform.Rotate(axis * 90);

        if (!IsValidPosition())
        {
            transform.Rotate(-axis * 90); // Отменить вращение
        }
    }

    private bool IsValidPosition()
    {
        foreach (Transform child in transform)
        {
            Vector3 position = RoundVector(child.position);
            //print($"{!_grid.IsInsideGrid(position)}  {_grid.IsCellOccupied(position)}");
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
            child.SetParent(null); // Отвязываем блок от родителя

            Vector3 position = RoundVector(child.position);
            if (_grid.IsInsideGrid(position))
            {
                _grid.AddBlockToGrid(child); // Добавляем дочерний блок в сетку
            }

        }

        Destroy(gameObject); // Удаляем только объект фигуры, блоки остаются в сетке
    }

    private void CheckForCompleteRows()
    {
        _grid.ClearFullRows();
    }

    private Vector3 RoundVector(Vector3 vector)
    {
        return new Vector3(Mathf.Round(vector.x), Mathf.Round(vector.y), Mathf.Round(vector.z));
    }
}
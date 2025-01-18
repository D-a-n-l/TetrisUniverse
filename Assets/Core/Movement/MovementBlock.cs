using System.Collections;
using UnityEngine;
using Zenject;

public class MovementBlock : MonoBehaviour
{
    [SerializeField]
    private float _timeFall = 1f;

    [SerializeField]
    private Transform[] _tiles;

    private TetrisGrid _grid;

    private WaitForSeconds _waitFall;

    private Vector3 _directionFall = new Vector3(0, -1, 0);

    [Inject]
    private void Construct(TetrisGrid grid)
    {
        _grid = grid;
    }

    private void Start()
    {
        _waitFall = new WaitForSeconds(_timeFall);

        StopAllCoroutines();

        StartCoroutine(FallByTime());

        PlayerButtons.Instance.RemoveAllListeners();

        PlayerButtons.Instance.Forward.onClick.AddListener(() =>
        {
            Move(new Vector3(CameraMovement.LocalForward.x, 0f, CameraMovement.LocalForward.y));
        });

        PlayerButtons.Instance.Left.onClick.AddListener(() =>
        {
            Move(new Vector3(-CameraMovement.LocalRight.x, 0f, -CameraMovement.LocalRight.y));
        });

        PlayerButtons.Instance.Back.onClick.AddListener(() =>
        {
            Move(new Vector3(-CameraMovement.LocalForward.x, 0f, -CameraMovement.LocalForward.y));
        });

        PlayerButtons.Instance.Right.onClick.AddListener(() =>
        {
            Move(new Vector3(CameraMovement.LocalRight.x, 0f, CameraMovement.LocalRight.y));
        });

        PlayerButtons.Instance.RotateX.onClick.AddListener(() =>
        {
            Rotate(Vector3.left);
        });

        PlayerButtons.Instance.RotateZ.onClick.AddListener(() =>
        {
            Rotate(Vector3.forward);
        });

        PlayerButtons.Instance.Fall.OnPressed.AddListener(Fall);

        PlayerButtons.Instance.Fall.OnDown.AddListener(StopAllCoroutines);

        PlayerButtons.Instance.Fall.OnUp.AddListener(() => { StopAllCoroutines(); StartCoroutine(FallByTime()); });
    }

    private void Fall()
    {
        if (!Move(_directionFall))
        {
            AddToGrid();

            GlobalEvents.OnMovementFinished?.Invoke();
        }
    }

    public IEnumerator FallByTime()
    {
        yield return _waitFall;

        Fall();

        StartCoroutine(FallByTime());
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
            Vector3 position = MathfCalculations.RoundVector(child.position);

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

            Vector3 position = MathfCalculations.RoundVector(child.position);

            if (_grid.IsInsideGrid(position))
            {
                _grid.AddBlockToGrid(child); // Добавляем дочерний блок в сетку
            }

        }

        Destroy(gameObject); // Удаляем только объект фигуры, блоки остаются в сетке
    }
}
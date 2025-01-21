using GG.Infrastructure.Utils.Swipe;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;

public class MovementFigure : MonoBehaviour
{
    [SerializeField]
    private float _timeFall = 1f;

    [SerializeField]
    private MeshRenderer[] _tiles;

    public MeshRenderer[] Tiles => _tiles;

    private TetrisGrid _grid;

    private WaitForSeconds _waitFall;

    private Vector3 _directionFall = new Vector3(0, -1, 0);

    private bool _isSwipe = false;

    [Inject]
    private void Construct(TetrisGrid grid)
    {
        _grid = grid;
    }

    public void IsSwipe(bool isSwipe)
    {
        _isSwipe = isSwipe;
    }

    private void OnSwipe(string swipe)
    {
        switch (swipe)
        {
            case "Left":
                Move(Vector2.right);
                break;
            case "Right":
                Move(Vector2.left);
                break;
            case "Down":
                Move(Vector2.down);
                break;
            case "DownLeft":
                Move(Vector2.down);
                break;
            case "DownRight":
                Move(Vector2.down);
                break;
        }
    }

    private void Start()
    {
        _waitFall = new WaitForSeconds(_timeFall);

        StopAllCoroutines();

        StartCoroutine(FallByTime());

        if (_isSwipe == true)
        {
            SwipeListener.Instance.OnSwipe.AddListener(OnSwipe);

            SwipeListener.Instance.OnTouch.AddListener(() => Rotate(Vector3.forward));

            return;
        }

        PlayerButtons.Instance.RemoveAllListeners();

        PlayerButtons.Instance.Forward.OnPressed.AddListener(() =>
        {
            Move(new Vector3(CameraMovement.LocalForward.x, 0f, CameraMovement.LocalForward.y));
        });

        PlayerButtons.Instance.Left.OnPressed.AddListener(() =>
        {
            Move(new Vector3(-CameraMovement.LocalRight.x, 0f, -CameraMovement.LocalRight.y));
        });

        PlayerButtons.Instance.Back.OnPressed.AddListener(() =>
        {
            Move(new Vector3(-CameraMovement.LocalForward.x, 0f, -CameraMovement.LocalForward.y));
        });

        PlayerButtons.Instance.Right.OnPressed.AddListener(() =>
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
        for (int i = 0; i < _tiles.Length; i++)
        {
            _tiles[i].transform.SetParent(null); // Отвязываем блок от родителя

            Vector3 position = MathfCalculations.RoundVector(_tiles[i].transform.position);

            if (_grid.IsInsideGrid(position))
            {
                _grid.AddBlockToGrid(_tiles[i].transform); // Добавляем дочерний блок в сетку
            }

        }

        GlobalEvents.OnMovementFinished?.Invoke(_tiles.Length);

        Destroy(gameObject); // Удаляем только объект фигуры, блоки остаются в сетке
    }
}
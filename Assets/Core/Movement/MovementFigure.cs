using System.Collections;
using UnityEngine;
using Zenject;

public class MovementFigure : MonoBehaviour
{
    [SerializeField]
    private float _timeFall = 1f;

    [SerializeField]
    private Figure _block;

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
        for (int i = 0; i < _block.Tiles.Length; i++)
        {
            _block.Tiles[i].transform.SetParent(null); // ���������� ���� �� ��������

            Vector3 position = MathfCalculations.RoundVector(_block.Tiles[i].transform.position);

            if (_grid.IsInsideGrid(position))
            {
                _grid.AddBlockToGrid(_block.Tiles[i].transform); // ��������� �������� ���� � �����
            }

        }

        GlobalEvents.OnMovementFinished?.Invoke(_block.Tiles.Length);

        Destroy(gameObject); // ������� ������ ������ ������, ����� �������� � �����
    }
}
using GG.Infrastructure.Utils.Swipe;
using System.Collections;
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

    private WaitForSeconds _waitCheck;

    private GameObject _previewObject;

    private Vector3[] _offsets = new Vector3[]
    {
        Vector3.left,
        Vector3.right,
        Vector3.down
    };

    private bool _isSwipe = false;

    private bool _isMove = true;

    [Inject]
    private void Construct(TetrisGrid grid)
    {
        _grid = grid;
    }

    private void Start()
    {
        _waitFall = new WaitForSeconds(_timeFall);

        _waitCheck = new WaitForSeconds(0.1f);

        PlayerButtons.Instance.RemoveAllListeners();

        SwipeListener.Instance.RemoveAllListener();

        CreatePreviewObject();

        StopAllCoroutines();

        StartCoroutine(FallByTime());

        StartCoroutine(Check());

        if (_isSwipe == true)
        {
            SwipeListener.Instance.OnSwipe.AddListener(SwipeMove);

            SwipeListener.Instance.OnTouch.AddListener(() =>
            {
                Rotate(transform, Vector3.forward, false);

                Rotate(_previewObject.transform, Vector3.forward, true);
            });

            return;
        }

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
            Rotate(transform, Vector3.left, false);

            Rotate(_previewObject.transform, Vector3.left, true);
        });

        PlayerButtons.Instance.RotateZ.onClick.AddListener(() =>
        {
            Rotate(transform, Vector3.forward, false);

            Rotate(_previewObject.transform, Vector3.forward, true);
        });

        PlayerButtons.Instance.Fall.OnPressed.AddListener(Fall);

        PlayerButtons.Instance.Fall.OnDown.AddListener(StopAllCoroutines);

        PlayerButtons.Instance.Fall.OnUp.AddListener(() => { StopAllCoroutines(); StartCoroutine(FallByTime()); });
    }

    public void IsSwipe(bool isSwipe)
    {
        _isSwipe = isSwipe;
    }

    private Material CreatePreviewMaterial()
    {
        Material previewMaterial = new Material(Shader.Find("Standard"));
        previewMaterial.color = new Color(1f, 1f, 1f, 0.25f); // Полупрозрачный белый
        return previewMaterial;
    }

    private IEnumerator Check()
    {
        yield return _waitCheck;

        if (!IsValidPosition(transform, Vector3.down))
        {
            _isMove = false;
        }

        StartCoroutine(Check());
    }

    private void Fall()
    {
        if (!Move(Vector3.down))
        {
            AddToGrid();
        }
    }

    private IEnumerator FallByTime()
    {
        yield return _waitFall;

        Fall();

        StartCoroutine(FallByTime());
    }

    private void CreatePreviewObject()
    {
        _previewObject = Instantiate(gameObject, transform.position, transform.rotation);
        Destroy(_previewObject.GetComponent<MovementFigure>()); // Убираем логику движения
        foreach (Transform child in _previewObject.transform)
        {
            Renderer renderer = child.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material = CreatePreviewMaterial();
            }
        }

        UpdatePreviewObject();
    }

    private void UpdatePreviewObject()
    {
        if (_previewObject == null)
            return;

        Vector3 startPosition = transform.position;

        _previewObject.transform.position = startPosition;

        // Падение вниз до нижней точки
        while (IsValidPosition(_previewObject.transform, Vector3.down))
        {
            _previewObject.transform.position += Vector3.down;
        }

        // Откат на одну позицию вверх
        //previewObject.transform.position += Vector3.down;
    }

    private void SwipeMove(string swipe)
    {
        if (_isMove == false)
            return;

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

        UpdatePreviewObject();
    }

    private bool Move(Vector3 direction)
    {
        if (_isMove == false)
            return false;

        transform.position += direction;

        if (!IsValidPosition(transform, Vector3.zero))
        {
            transform.position -= direction; // Вернуть на место

            return false;
        }

        return true;
    }

    private void Rotate(Transform transformObject, Vector3 axis, bool isPreviewObject)
    {
        if (_isMove == false)
            return;

        transformObject.Rotate(axis * 90);

        if (isPreviewObject == true)
            UpdatePreviewObject();

        if (!IsValidPosition(transformObject, Vector3.zero))
        {
            if (!TryWallKick(transformObject))
            {
                transformObject.Rotate(-axis * 90);

                if (isPreviewObject == true)
                    UpdatePreviewObject();
            }
        }
    }

    private bool TryWallKick(Transform transformObject)
    {
        foreach (Vector3 offset in _offsets)
        {
            transformObject.position += offset;

            if (IsValidPosition(transformObject, Vector3.zero))
            {
                return true; // Если сдвиг удался, оставить фигуру в новом положении
            }

            transformObject.position -= offset; // Откат, если сдвиг не помог
        }

        return false; // Не удалось сдвинуть
    }

    private bool IsValidPosition(Transform transformObject, Vector3 increasePosition)
    {
        foreach (Transform child in transformObject)
        {
            Vector3 position = MathfCalculations.RoundVector(child.position + increasePosition);

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

        Destroy(_previewObject);
    }
}
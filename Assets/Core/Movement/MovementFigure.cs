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

    private GameObject[] ghostBlocks; // ������ ��� �������� "����������" ������

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

    private GameObject previewObject; // ������ ��� ������

    private void CreatePreviewObject()
    {
        previewObject = Instantiate(gameObject, transform.position, transform.rotation);
        previewObject.GetComponent<MovementFigure>().enabled = false; // ������� ������ ��������
        foreach (Transform child in previewObject.transform)
        {
            Renderer renderer = child.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material = CreatePreviewMaterial();
            }
        }
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
        CreatePreviewObject();
        StopAllCoroutines();

        StartCoroutine(FallByTime());

        if (_isSwipe == true)
        {
            SwipeListener.Instance.OnSwipe.AddListener(OnSwipe);

            SwipeListener.Instance.OnTouch.AddListener(() =>
            {
                RotateA(Vector3.forward, transform);

                previewObject.transform.Rotate(Vector3.forward * 90);
                UpdatePreviewObject();
                if (!IsValidPosition(previewObject.transform))
                {
                    if (!TryWallKick(previewObject.transform))
                    {
                        previewObject.transform.Rotate(-Vector3.forward * 90); // �������� ��������
                    }
                }
            });

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
            RotateA(Vector3.left, transform);
            previewObject.transform.Rotate(Vector3.left * 90);
            UpdatePreviewObject();
            if (!IsValidPosition(previewObject.transform))
            {
                if (!TryWallKick(previewObject.transform))
                {
                    previewObject.transform.Rotate(-Vector3.left * 90); // �������� ��������
                }
            }

        });

        PlayerButtons.Instance.RotateZ.onClick.AddListener(() =>
        {
            RotateA(Vector3.forward, transform);
            previewObject.transform.Rotate(Vector3.forward * 90);
            UpdatePreviewObject();
            if (!IsValidPosition(previewObject.transform))
            {
                if (!TryWallKick(previewObject.transform))
                {
                    previewObject.transform.Rotate(-Vector3.forward * 90); // �������� ��������
                }
            }
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
    void Update()
    {

        UpdatePreviewObject();
    }

    private bool TryWallKick(Transform transformObj)
    {
        // ��������� ����������� ��� ������
        Vector3[] offsets = new Vector3[]
        {
        Vector3.left,     // ����� �����
        Vector3.right,    // ����� ������
        };

        foreach (Vector3 offset in offsets)
        {
            transformObj.position += offset;

            if (IsValidPosition(transformObj))
            {
                // ���� ����� ������, �������� ������ � ����� ���������
                return true;
            }

            // �����, ���� ����� �� �����
            transformObj.position -= offset;
        }

        return false; // �� ������� ��������
    }


    private Material CreatePreviewMaterial()
    {
        Material previewMaterial = new Material(Shader.Find("Standard"));
        previewMaterial.color = new Color(1f, 1f, 1f, 0.25f); // �������������� �����
        return previewMaterial;
    }


    private void UpdatePreviewObject()
    {
        if (previewObject == null) return;

        Vector3 startPosition = transform.position;
        previewObject.transform.position = startPosition;

        // ������� ���� �� ������ �����
        while (IsValidPosition(previewObject.transform))
        {
            previewObject.transform.position += Vector3.down;
        }

        // ����� �� ���� ������� �����
        previewObject.transform.position += Vector3.up;
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

        if (!IsValidPosition(transform))
        {
            transform.position -= direction; // ������� �� �����

            return false;
        }

        return true;
    }

    private void RotateA(Vector3 axis, Transform transformObj)
    {
        transformObj.Rotate(axis * 90);

        if (!IsValidPosition(transformObj))
        {
            if (!TryWallKick(transformObj))
            {
                transformObj.Rotate(-axis * 90); // �������� ��������
            }
        }
    }

    private bool IsValidPosition(Transform transformOb)
    {
        foreach (Transform child in transformOb)
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
            _tiles[i].transform.SetParent(null); // ���������� ���� �� ��������

            Vector3 position = MathfCalculations.RoundVector(_tiles[i].transform.position);

            if (_grid.IsInsideGrid(position))
            {
                _grid.AddBlockToGrid(_tiles[i].transform); // ��������� �������� ���� � �����
            }

        }

        GlobalEvents.OnMovementFinished?.Invoke(_tiles.Length);

        Destroy(gameObject); // ������� ������ ������ ������, ����� �������� � �����
        Destroy(previewObject);
    }
}
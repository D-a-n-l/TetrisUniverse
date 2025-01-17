using UnityEngine;
using Zenject;

public class VisualGrid : MonoBehaviour
{
    [SerializeField]
    private LineRenderer _line;

    [SerializeField]
    private Vector3 _offset;

    private GameObject _last;

    private TetrisGrid _grid;

    [Inject]
    private void Construct(TetrisGrid grid)
    {
        _grid = grid;
    }

    private void Start()
    {
        CreateGrid();

        GlobalEvents.OnEditedGrid += CreateGrid;
    }

    public void CreateGrid()
    {
        DestroyLastGrid();

        Vector3 coordPosition = Vector3.zero + _offset;

        Transform parent = new GameObject("VisualGrid").transform;

        _last = parent.gameObject;

        GenerateVerticalGrid1(coordPosition, Vector3.right, Vector3.forward, _grid.Radius.x, _grid.Radius.z, parent);
        //GenerateVerticalGrid(coordPosition, Vector3.right, Vector3.forward, Spawner.Instance.Radius.x, Spawner.Instance.Radius.z, parent);
        //GenerateVerticalGrid2(coordPosition, Vector3.forward, Vector3.right, Spawner.Instance.Radius.z, Spawner.Instance.Radius.x, parent);

        LineRenderer lineRenderer = Instantiate(_line, parent);

        lineRenderer.transform.localPosition = coordPosition;

        Vector3[] positions = new Vector3[4];

        positions[0] = new Vector3(0, 0, 0);
        positions[1] = new Vector3(_grid.Radius.x, 0, 0);
        positions[2] = new Vector3(_grid.Radius.x, 0, _grid.Radius.z);
        positions[3] = new Vector3(0, 0, _grid.Radius.z);

        lineRenderer.positionCount = 4;

        lineRenderer.SetPositions(positions);

        lineRenderer.loop = true;

        LineRenderer lineRenderer2 = Instantiate(_line, parent);

        lineRenderer2.transform.localPosition = coordPosition;

        Vector3[] positions2 = new Vector3[4];

        positions2[0] = new Vector3(0, _grid.Radius.y, 0);
        positions2[1] = new Vector3(_grid.Radius.x, _grid.Radius.y, 0);
        positions2[2] = new Vector3(_grid.Radius.x, _grid.Radius.y, _grid.Radius.z);
        positions2[3] = new Vector3(0, _grid.Radius.y, _grid.Radius.z);

        lineRenderer2.positionCount = 4;

        lineRenderer2.SetPositions(positions2);

        lineRenderer2.loop = true;
    }

    public void DestroyLastGrid()
    {
        if (_last != null)
        {
            Destroy(_last);
        }
    }

    private void GenerateVerticalGrid1(Vector3 coordPosition, Vector3 direction1, Vector3 direction2, int size1, int size2, Transform parent)
    {
        for (int i = 0; i <= size1; i+=size1)
        {
            LineRenderer[] lineRenderers = new LineRenderer[2];

            for (int j = 0; j < 2; ++j)
            {
                lineRenderers[j] = Instantiate(_line, parent);
                lineRenderers[j].transform.localPosition = coordPosition;
            }

            Vector3[] positions = { direction1 * i, Vector3.up * _grid.Radius.y + direction1 * i };

            lineRenderers[0].SetPositions(positions);

            for (int j = 0; j < 2; ++j)
                positions[j] += direction2 * size2;

            lineRenderers[1].SetPositions(positions);
        }
    }

    private void GenerateVerticalGrid(Vector3 coordPosition, Vector3 direction1, Vector3 direction2,  int size1, int size2, Transform parent)
    {
        for(int i = 0; i <= size1; ++i)
        {
            LineRenderer[] lineRenderers = new LineRenderer[2];

            for(int j = 0; j < 2; ++j)
            {
                lineRenderers[j] = Instantiate(_line, parent);
                lineRenderers[j].transform.localPosition = coordPosition;
            }
            
            Vector3[] positions = { direction1 * i, Vector3.up * _grid.Radius.y + direction1 * i };

            lineRenderers[0].SetPositions(positions);

            for(int j = 0; j < 2; ++j) 
                positions[j] += direction2 * size2;
                
            lineRenderers[1].SetPositions(positions);
        }
    }

    private void GenerateVerticalGrid2(Vector3 coordPosition, Vector3 direction1, Vector3 direction2, int size1, int size2, Transform parent)
    {
        for (int i = 1; i < size1; ++i)
        {
            LineRenderer[] lineRenderers = new LineRenderer[2];

            for (int j = 0; j < 2; ++j)
            {
                lineRenderers[j] = Instantiate(_line, parent);
                lineRenderers[j].transform.localPosition = coordPosition;
            }

            Vector3[] positions = { direction1 * i, Vector3.up * _grid.Radius.y + direction1 * i };

            lineRenderers[0].SetPositions(positions);

            for (int j = 0; j < 2; ++j)
                positions[j] += direction2 * size2;

            lineRenderers[1].SetPositions(positions);
        }
    }
}
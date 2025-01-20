using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class Bootstrap : MonoBehaviour
{
    private TetrisGrid _grid;

    private Spawner _spawner;

    private Vector3Int _lastRadius;

    [Inject]
    public void Construct(TetrisGrid grid, Spawner spawner)
    {
        _grid = grid;

        _spawner = spawner;
    }

    public void Spawn()
    {
        _spawner.Spawn();
    }

    public void SetOrthographic()
    {
        _lastRadius = _grid.Radius;

        _grid.SetRadius(0, 10);

        _grid.SetRadius(1, 20);

        _grid.SetRadius(2, 1);

        Vector3 positionCamera = new Vector3((_grid.Radius.x / 2) - 0.5f, _grid.Radius.y / 2, 100);

        CameraMovement.Instance.SetOrthographic(true, positionCamera);
    }

    public void UnsetOrthographic()
    {
        _grid.SetRadius(0, (uint) _lastRadius.x);

        _grid.SetRadius(1, (uint) _lastRadius.y);

        _grid.SetRadius(2, (uint) _lastRadius.z);

        CameraMovement.Instance.SetOrthographic(false, Vector3.zero);
    }

    public void IncreaseRadius(int side)
    {
        _grid.IncreaseRadius(side);
    }

    public void DecreaseRadius(int side)
    {
        _grid.DecreaseRadius(side);
    }
}
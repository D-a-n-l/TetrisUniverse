using System;
using UnityEngine;

public class TetrisGrid : IDisposable
{
    private Transform[,,] _grid;

    private Transform _rootSpawn;

    public Vector3Int Radius { get; private set; } = new Vector3Int(12, 12, 12);

    private Vector3Int BaseRadius = new Vector3Int(3, 3, 3);

    private const int _radiusChange = 1;

    private float _positionChange = (float) _radiusChange / 2;

    public bool IsEnableVisualGrid { get; set; } = true;

    public Action<string> OnRadiusX;

    public Action<string> OnRadiusY;

    public Action<string> OnRadiusZ;

    public TetrisGrid(Transform rootSpawn)
    {
        _rootSpawn = rootSpawn;

        _grid = new Transform[Radius.x, Radius.y, Radius.z];

        GlobalEvents.OnMovementFinished += (int value) => ClearFullRows();
    }

    public void Dispose()
    {
        GlobalEvents.OnMovementFinished -= (int value) => ClearFullRows();
    }

    public void IncreaseRadius(int side)
    {
        switch (side)
        {
            case 0:
                Radius = new Vector3Int(Radius.x + _radiusChange, Radius.y, Radius.z);

                _rootSpawn.position = new Vector3(_rootSpawn.position.x + _positionChange, _rootSpawn.position.y, _rootSpawn.position.z);

                CameraMovement.Instance.IncreaseDistanceBetweenCameraAndTarget(_radiusChange);

                OnRadiusX?.Invoke(Radius.x.ToString());

                break;
            case 1:
                Radius = new Vector3Int(Radius.x, Radius.y + _radiusChange, Radius.z);

                _rootSpawn.position = new Vector3(_rootSpawn.position.x, _rootSpawn.position.y + _positionChange, _rootSpawn.position.z);

                OnRadiusY?.Invoke(Radius.y.ToString());

                break;
            case 2:
                Radius = new Vector3Int(Radius.x, Radius.y, Radius.z + _radiusChange);

                _rootSpawn.position = new Vector3(_rootSpawn.position.x, _rootSpawn.position.y, _rootSpawn.position.z + _positionChange);

                CameraMovement.Instance.IncreaseDistanceBetweenCameraAndTarget(_radiusChange);

                OnRadiusZ?.Invoke(Radius.z.ToString());

                break;
        }

        _grid = new Transform[Radius.x, Radius.y, Radius.z];

        if (IsEnableVisualGrid == false)
            return;

        GlobalEvents.OnEditedGrid?.Invoke();
    }

    public void DecreaseRadius(int side)
    {
        switch (side)
        {
            case 0:
                if (Radius.x - _radiusChange < BaseRadius.x)
                    return;

                Radius = new Vector3Int(Radius.x - _radiusChange, Radius.y, Radius.z);

                _rootSpawn.position = new Vector3(_rootSpawn.position.x - _positionChange, _rootSpawn.position.y, _rootSpawn.position.z);

                CameraMovement.Instance.IncreaseDistanceBetweenCameraAndTarget(-_radiusChange);

                OnRadiusX?.Invoke(Radius.x.ToString());

                break;
            case 1:
                if (Radius.y - _radiusChange < BaseRadius.y)
                    return;

                Radius = new Vector3Int(Radius.x, Radius.y - _radiusChange, Radius.z);

                _rootSpawn.position = new Vector3(_rootSpawn.position.x, _rootSpawn.position.y - _positionChange, _rootSpawn.position.z);

                OnRadiusY?.Invoke(Radius.y.ToString());

                break;
            case 2:
                if (Radius.z - _radiusChange < BaseRadius.z)
                    return;

                Radius = new Vector3Int(Radius.x, Radius.y, Radius.z - _radiusChange);

                _rootSpawn.position = new Vector3(_rootSpawn.position.x, _rootSpawn .position.y, _rootSpawn.position.z - _positionChange);

                CameraMovement.Instance.IncreaseDistanceBetweenCameraAndTarget(-_radiusChange);

                OnRadiusZ?.Invoke(Radius.z.ToString());

                break;
        }

        _grid = new Transform[Radius.x, Radius.y, Radius.z];

        if (IsEnableVisualGrid == false)
            return;

        GlobalEvents.OnEditedGrid?.Invoke();
    }

    // Проверка, находится ли позиция внутри сетки
    public bool IsInsideGrid(Vector3 position)
    {
        return position.x >= 0 && position.x < Radius.x &&
               position.y >= 0 && position.y <= Radius.y &&
               position.z >= 0 && position.z < Radius.z;
    }

    // Проверка, занята ли ячейка
    public bool IsCellOccupied(Vector3 position)
    {
        if (!IsInsideGrid(position)) return true; // За пределами сетки всегда занято

        if ((int)position.y == Radius.y) return false; //когда фигура выше раудиса

        return _grid[(int)position.x, (int)position.y, (int)position.z] != null;
    }

    // Добавление блока в сетку
    public void AddBlockToGrid(Transform block)
    {
        Vector3 position = MathfCalculations.RoundVector(block.position);

        if (IsInsideGrid(position))
        {
            _grid[(int)position.x, (int)position.y, (int)position.z] = block;
        }
    }

    // Удаление блока из сетки
    public void RemoveBlockFromGrid(Transform block)
    {
        Vector3 position = MathfCalculations.RoundVector(block.position);

        if (IsInsideGrid(position))
        {
            _grid[(int)position.x, (int)position.y, (int)position.z] = null;
        }
    }

    // Очистка полностью заполненной строки (по высоте Y)
    public void ClearFullRows()
    {
        for (int y = 0; y < Radius.y; y++)
        {
            if (IsRowFull(y))
            {
                ClearRow(y);

                MoveRowsDown(y);
            }
        }
    }

    // Проверка, заполнена ли строка
    private bool IsRowFull(int y)
    {
        for (int x = 0; x < Radius.x; x++)
        {
            for (int z = 0; z < Radius.z; z++)
            {
                if (_grid[x, y, z] == null)
                {
                    return false;
                }
            }
        }

        return true;
    }

    // Очистка строки
    private void ClearRow(int y)
    {
        for (int x = 0; x < Radius.x; x++)
        {
            for (int z = 0; z < Radius.z; z++)
            {
                if (_grid[x, y, z] != null)
                {
                    UnityEngine.Object.Destroy(_grid[x, y, z].gameObject);

                    _grid[x, y, z] = null;
                }
            }
        }

        GlobalEvents.OnDeletedRow?.Invoke(Radius.x * Radius.z);
    }

    // Сдвиг строк вниз
    private void MoveRowsDown(int clearedRow)
    {
        for (int y = clearedRow; y < Radius.y - 1; y++)
        {
            for (int x = 0; x < Radius.x; x++)
            {
                for (int z = 0; z < Radius.z; z++)
                {
                    if (_grid[x, y + 1, z] != null)
                    {
                        Transform block = _grid[x, y + 1, z];

                        _grid[x, y + 1, z] = null;

                        _grid[x, y, z] = block;

                        block.position += Vector3.down;
                    }
                }
            }
        }
    }
}
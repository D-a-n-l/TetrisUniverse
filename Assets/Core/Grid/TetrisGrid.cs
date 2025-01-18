using System;
using UnityEngine;

public class TetrisGrid : MonoBehaviour
{
    [SerializeField]
    private Vector3Int _radius;

    public Vector3Int Radius => _radius;

    private Transform[,,] _grid;

    public bool IsEnableVisualGrid { get; set; } = true;

    private Vector3Int BaseRadius = new Vector3Int(3, 3, 3);

    private const int _radiusChange = 1;

    private float _positionChange = (float) _radiusChange / 2;

    public Action OnRadiusX;

    public Action OnRadiusY;

    public Action OnRadiusZ;

    private void Start()
    {
        _grid = new Transform[_radius.x, _radius.y, _radius.z];

        GlobalEvents.OnMovementFinished += ClearFullRows;
    }

    public void IncreaseRadius(int side)
    {
        switch (side)
        {
            case 0:
                _radius = new Vector3Int(_radius.x + _radiusChange, _radius.y, _radius.z);

                transform.position = new Vector3(transform.position.x + _positionChange, transform.position.y, transform.position.z);

                CameraMovement.instance.IncreaseDistanceBetweenCameraAndTarget(_radiusChange);

                OnRadiusX?.Invoke();

                break;
            case 1:
                _radius = new Vector3Int(_radius.x, _radius.y + _radiusChange, _radius.z);

                transform.position = new Vector3(transform.position.x, transform.position.y + _positionChange, transform.position.z);

                OnRadiusY?.Invoke();

                break;
            case 2:
                _radius = new Vector3Int(_radius.x, _radius.y, _radius.z + _radiusChange);

                transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + _positionChange);

                CameraMovement.instance.IncreaseDistanceBetweenCameraAndTarget(_radiusChange);

                OnRadiusZ?.Invoke();

                break;
        }

        _grid = new Transform[_radius.x, _radius.y, _radius.z];

        if (IsEnableVisualGrid == false)
            return;

        GlobalEvents.OnEditedGrid?.Invoke();
    }

    public void DecreaseRadius(int side)
    {
        switch (side)
        {
            case 0:
                if (_radius.x - _radiusChange < BaseRadius.x)
                    return;

                _radius = new Vector3Int(_radius.x - _radiusChange, _radius.y, _radius.z);

                transform.position = new Vector3(transform.position.x - _positionChange, transform.position.y, transform.position.z);

                CameraMovement.instance.IncreaseDistanceBetweenCameraAndTarget(-_radiusChange);

                OnRadiusX?.Invoke();

                break;
            case 1:
                if (_radius.y - _radiusChange < BaseRadius.y)
                    return;

                _radius = new Vector3Int(_radius.x, _radius.y - _radiusChange, _radius.z);

                transform.position = new Vector3(transform.position.x, transform.position.y - _positionChange, transform.position.z);

                OnRadiusY?.Invoke();

                break;
            case 2:
                if (_radius.z - _radiusChange < BaseRadius.z)
                    return;

                _radius = new Vector3Int(_radius.x, _radius.y, _radius.z - _radiusChange);

                transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - _positionChange);

                CameraMovement.instance.IncreaseDistanceBetweenCameraAndTarget(-_radiusChange);

                OnRadiusZ?.Invoke();

                break;
        }

        _grid = new Transform[_radius.x, _radius.y, _radius.z];

        if (IsEnableVisualGrid == false)
            return;

        GlobalEvents.OnEditedGrid?.Invoke();
    }

    // Проверка, находится ли позиция внутри сетки
    public bool IsInsideGrid(Vector3 position)
    {
        return position.x >= 0 && position.x < _radius.x &&
               position.y >= 0 && position.y <= _radius.y &&
               position.z >= 0 && position.z < _radius.z;
    }

    // Проверка, занята ли ячейка
    public bool IsCellOccupied(Vector3 position)
    {
        if (!IsInsideGrid(position)) return true; // За пределами сетки всегда занято

        if ((int)position.y == _radius.y) return false; //когда фигура выше раудиса

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
        for (int y = 0; y < _radius.y; y++)
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
        for (int x = 0; x < _radius.x; x++)
        {
            for (int z = 0; z < _radius.z; z++)
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
        for (int x = 0; x < _radius.x; x++)
        {
            for (int z = 0; z < _radius.z; z++)
            {
                if (_grid[x, y, z] != null)
                {
                    Destroy(_grid[x, y, z].gameObject);

                    _grid[x, y, z] = null;
                }
            }
        }
    }

    // Сдвиг строк вниз
    private void MoveRowsDown(int clearedRow)
    {
        for (int y = clearedRow; y < _radius.y - 1; y++)
        {
            for (int x = 0; x < _radius.x; x++)
            {
                for (int z = 0; z < _radius.z; z++)
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
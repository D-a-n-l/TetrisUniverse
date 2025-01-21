using System;
using Unity.VisualScripting;
using UnityEngine;

public class Spawner : IDisposable
{
    private Transform _root;

    private PresetColors _presetColors;

    private MovementFigure[] _figures;

    private TetrisGrid _grid;

    private int _randomFigure = 0;

    private bool _isFirstSpawn = true;

    private bool _isCanSpawn = true;

    public bool IsClassical = false;

    public Action<MovementFigure> OnSpawned;

    public Spawner(Transform root, PresetColors presetColors, MovementFigure[] figures, TetrisGrid grid)
    {
        _grid = grid;

        _root = root;

        _root.position = new Vector3(((float)_grid.Radius.x / 2) - .5f, -.5f, ((float)_grid.Radius.z / 2) - .5f);

        _presetColors = presetColors;

        _figures = figures;

        GlobalEvents.OnMovementFinished += (int value) => Spawn();
    }

    public void Dispose()
    {
        GlobalEvents.OnMovementFinished -= (int value) => Spawn();
    }

    public void Spawn()
    {
        if (_isCanSpawn == false)
            return;

        if (_isFirstSpawn == true)
        {
            _randomFigure = UnityEngine.Random.Range(0, _figures.Length);

            _isFirstSpawn = false;

            _presetColors.Random();
        }

        MovementFigure currentFigure = _figures[_randomFigure];

        Vector3 spawnPosition = new Vector3(
            _grid.Radius.x / 2,
            _grid.Radius.y,
            _grid.Radius.z / 2);

        for (int i = 0; i < currentFigure.Tiles.Length; i++)
        {
            currentFigure.Tiles[i].sharedMaterial = _presetColors.Set();
        }

        UnityEngine.Object.Instantiate(currentFigure, spawnPosition, currentFigure.transform.rotation, _root).IsSwipe(IsClassical);

        _randomFigure = UnityEngine.Random.Range(0, _figures.Length);

        _presetColors.Random();

        for (int i = 0; i < _figures[_randomFigure].Tiles.Length; i++)
        {
            _figures[_randomFigure].Tiles[i].sharedMaterial = _presetColors.Set();
        }

        OnSpawned?.Invoke(_figures[_randomFigure]);
    }
}
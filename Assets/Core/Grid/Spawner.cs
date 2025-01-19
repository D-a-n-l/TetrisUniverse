using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class Spawner : MonoBehaviour
{
    [SerializeField]
    private PresetColors _presetColors;

    [SerializeField]
    private Figure[] _figures;

    private TetrisGrid _grid;

    private int randomFigure = 0;

    private bool _isFirstSpawn = true;

    private bool _isCanSpawn = true;

    public Action<Figure> OnSpawned;

    [Inject]
    private void Construct(TetrisGrid grid)
    {
        _grid = grid;
    }

    private void Start()
    {
        GlobalEvents.OnMovementFinished += Spawn;

        Init();
    }

    public void Init()
    {
        transform.position = new Vector3((float)_grid.Radius.x / 2, 0, (float)_grid.Radius.z / 2);
    }

    public void Spawn()
    {
        if (_isCanSpawn == false)
            return;

        if (_isFirstSpawn == true)
        {
            randomFigure = UnityEngine.Random.Range(0, _figures.Length);

            _isFirstSpawn = false;

            _presetColors.Random();
        }

        Figure currentFigure = _figures[randomFigure];

        Vector3 spawnPosition = new Vector3(
            _grid.Radius.x / 2,
            _grid.Radius.y,
            _grid.Radius.z / 2);

        //currentFigure.GetComponent<RotationFigureMany>().IsRandomRotation = false;

        for (int i = 0; i < currentFigure.Tiles.Length; i++)
        {
            currentFigure.Tiles[i].sharedMaterial = _presetColors.Set();
        }

        Instantiate(currentFigure, spawnPosition, currentFigure.transform.rotation);

        randomFigure = UnityEngine.Random.Range(0, _figures.Length);

        _presetColors.Random();

        for (int i = 0; i < _figures[randomFigure].Tiles.Length; i++)
        {
            _figures[randomFigure].Tiles[i].sharedMaterial = _presetColors.Set();
        }

        OnSpawned?.Invoke(_figures[randomFigure]);
    }
}
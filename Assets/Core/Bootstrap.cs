using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class Bootstrap : MonoBehaviour
{
    private TetrisGrid _grid;

    private Spawner _spawner;

    [Inject]
    public void Construct(TetrisGrid grid, Spawner spawner)
    {
        _grid = grid;

        _spawner = spawner;
    }

    //private void Start()
    //{
    //    Score score = FindAnyObjectByType<Score>();
    //    score.Init();
    //}

    public void Spawn()
    {
        _spawner.Spawn();
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
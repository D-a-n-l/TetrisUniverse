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


}
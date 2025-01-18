using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class Spawner : MonoBehaviour
{
    [SerializeField]
    private MovementBlock[] _blocks;

    private TetrisGrid _grid;

    public MovementBlock lastBlock;

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
        int randomIndex = Random.Range(0, _blocks.Length);

        Vector3 spawnPosition = new Vector3(
            _grid.Radius.x / 2,
            _grid.Radius.y,
            _grid.Radius.z / 2);

        lastBlock = Instantiate(_blocks[randomIndex], spawnPosition, _blocks[randomIndex].transform.rotation);
    }
}
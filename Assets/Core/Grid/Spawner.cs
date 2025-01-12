using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;
using Zenject;

public class Spawner : MonoBehaviour
{
    public static Spawner Instance;

    [SerializeField]
    private TetrisBlock[] _blocks;

    private TetrisGrid _grid;

    [Inject]
    private void Construct(TetrisGrid grid)
    {
        _grid = grid;
    }

    private void Awake()
    {
        Instance = this;

        SpawnBlock();
    }

    public void SpawnBlock()
    {
        int randomIndex = Random.Range(0, _blocks.Length);

        Vector3 spawnPosition = new Vector3(
            _grid.Radius.x / 2,
            _grid.Radius.y,
            _grid.Radius.z / 2);

        Instantiate(_blocks[randomIndex], spawnPosition, Quaternion.identity);
    }
}

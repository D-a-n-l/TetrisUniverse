using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class Spawner : MonoBehaviour
{
    [SerializeField]
    private TetrisBlock[] _blocks;

    private TetrisGrid _grid;

    public TetrisBlock lastBlock;

    [Inject]
    private void Construct(TetrisGrid grid)
    {
        _grid = grid;
    }

    private void Start()
    {
        transform.position = new Vector3(_grid.Radius.x / 2, 0, _grid.Radius.z / 2);//init

        GlobalEvents.OnMovementFinished += Spawn;
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

    private void OnDrawGizmos()
    {
        if (_grid == null)
            return;

        Gizmos.color = Color.red;

        Vector3 spawnPosition = new Vector3(
            _grid.Radius.x / 2,
            _grid.Radius.y / 2,
            _grid.Radius.z / 2);

        Gizmos.DrawCube(spawnPosition, _grid.Radius);
    }
}
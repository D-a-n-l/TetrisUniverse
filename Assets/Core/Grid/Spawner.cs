using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public static Spawner Instance; // Singleton

    public GameObject[] blockPrefabs; // ������� �����
    public Transform spawnPoint; // ����� ������

    private void Awake()
    {
        Instance = this;

        SpawnBlock();
    }

    public void SpawnBlock()
    {
        int randomIndex = Random.Range(0, blockPrefabs.Length);
        Instantiate(blockPrefabs[randomIndex], spawnPoint.position, Quaternion.identity);
    }
}

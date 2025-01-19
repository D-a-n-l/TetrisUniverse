using UnityEngine;

public class Figure : MonoBehaviour
{
    [SerializeField]
    private MeshRenderer[] _tiles;

    public MeshRenderer[] Tiles => _tiles;
}
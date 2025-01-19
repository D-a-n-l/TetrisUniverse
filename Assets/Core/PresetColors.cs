using UnityEngine;

public class PresetColors : MonoBehaviour
{
    [SerializeField]
    private Material _material;

    [Space(5)]
    [SerializeField]
    private Color[] _colors = new Color[3];

    private Color _last;

    public Material Set()
    {
        Material newMaterial = new Material(_material)
        {
            color = _last
        };

        return newMaterial;
    }

    public Color Random()
    {
        int randomColors = UnityEngine.Random.Range(0, _colors.Length);

        while (_colors[randomColors] == _last) 
        {
            randomColors = UnityEngine.Random.Range(0, _colors.Length);
        }

        return _last = _colors[randomColors];
    }
}
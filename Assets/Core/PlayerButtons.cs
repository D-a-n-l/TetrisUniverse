using UnityEngine;
using UnityEngine.UI;

public class PlayerButtons : MonoBehaviour
{
    public static PlayerButtons Instance { get; private set; }

    [field: SerializeField]
    public Button Forward { get; private set; }

    [field: SerializeField]
    public Button Left { get; private set; }

    [field: SerializeField]
    public Button Back { get; private set; }

    [field: SerializeField]
    public Button Right { get; private set; }

    [field: SerializeField]
    public PressedButton Fall { get; private set; }

    [field: SerializeField]
    public Button RotateX { get; private set; }

    [field: SerializeField]
    public Button RotateZ { get; private set; }

    private void Awake()
    {
        Instance = this;

        RemoveAllListeners();
    }

    public void RemoveAllListeners()
    {
        Forward.onClick.RemoveAllListeners();

        Left.onClick.RemoveAllListeners();

        Back.onClick.RemoveAllListeners();

        Right.onClick.RemoveAllListeners();

        Fall.OnDown.RemoveAllListeners();

        Fall.OnUp.RemoveAllListeners();

        Fall.OnPressed.RemoveAllListeners();

        RotateX.onClick.RemoveAllListeners();

        RotateZ.onClick.RemoveAllListeners();
    }
}
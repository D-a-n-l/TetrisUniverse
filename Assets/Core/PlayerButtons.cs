using UnityEngine;
using UnityEngine.UI;

public class PlayerButtons : MonoBehaviour
{
    public static PlayerButtons Instance { get; private set; }

    [field: SerializeField]
    public PressedButton Forward { get; private set; }

    [field: SerializeField]
    public PressedButton Left { get; private set; }

    [field: SerializeField]
    public PressedButton Back { get; private set; }

    [field: SerializeField]
    public PressedButton Right { get; private set; }

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
        Forward.OnPressed.RemoveAllListeners();

        Left.OnPressed.RemoveAllListeners();

        Back.OnPressed.RemoveAllListeners();

        Right.OnPressed.RemoveAllListeners();

        Fall.OnDown.RemoveAllListeners();

        Fall.OnUp.RemoveAllListeners();

        Fall.OnPressed.RemoveAllListeners();

        RotateX.onClick.RemoveAllListeners();

        RotateZ.onClick.RemoveAllListeners();
    }
}
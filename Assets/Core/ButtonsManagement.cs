using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ButtonsManagement : MonoBehaviour
{
    [SerializeField]
    private PresetButton[] _buttons;

    private void Awake()
    {
        foreach (var presetButton in _buttons)
        {
            presetButton.Button.onClick.AddListener(presetButton.Event.Invoke);
        }
    }
}

[System.Serializable]
public class PresetButton
{
    public Button Button;

    public UnityEvent Event;
}
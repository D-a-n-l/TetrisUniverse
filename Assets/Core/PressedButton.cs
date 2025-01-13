using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class PressedButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IUpdateSelectedHandler
{
    [SerializeField]
    private float cooldown = 0;

    private float nextPress = 0;

    private bool isPressed = false;

    public UnityEvent OnDown;

    public UnityEvent OnUp;

    public UnityEvent OnPressed;

    public void OnPointerDown(PointerEventData eventData)
    {
        isPressed = true;

        OnDown?.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPressed = false;

        OnUp?.Invoke();
    }

    public void OnUpdateSelected(BaseEventData eventData)
    {
        if (isPressed == true)
        {
            Press();
        }
    }

    private void Press()
    {
        if (Time.time > nextPress)
        {
            nextPress = Time.time + cooldown;

            OnPressed?.Invoke();
        }
    }
}
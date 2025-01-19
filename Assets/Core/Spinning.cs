using UnityEngine;

public class Spinning : MonoBehaviour
{
    [SerializeField]
    private float _speed = 20f;

    private static float currentRotation = 0;

    public bool IsSpin = true;

    private void Update()
    {
        if (IsSpin == false)
            return;

        currentRotation += _speed * Time.deltaTime;

        currentRotation %= 360;

        transform.localRotation = Quaternion.Euler(0, currentRotation, 0);
    }
}
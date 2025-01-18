using UnityEngine;
using UnityEngine.Events;

public abstract class Stat : MonoBehaviour
{
    [field: Min(0.001f)]
    [field: SerializeField]
    public int Max { get; private set; }

    public int Current { get; protected set; }

    public UnityEvent OnDecreased;

    public UnityEvent OnIncreased;

    public UnityEvent OnZeroing;

    private void Start() => Init();

    public abstract void Init();

    public abstract void Decrease(int value);

    public abstract void Increase(int value);
}
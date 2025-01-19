using System;

public class Score : Stat
{
    public override void Init()
    {
        Increase(0);

        GlobalEvents.OnMovementFinished += Increase;

        GlobalEvents.OnDeletedRow += Increase;
    }

    public void Zeroing()
    {
        Decrease(Current);
    }

    public override void Decrease(int value)
    {
        if (value < 0)
            throw new ArgumentOutOfRangeException(nameof(value), "value must be positive.");

        Current -= value;

        OnDecreased.Invoke();

        if (Current <= 0)
        {
            Current = 0;

            OnZeroing.Invoke();
        }
    }

    public override void Increase(int value)
    {
        if (value < 0)
            throw new ArgumentOutOfRangeException(nameof(value), "value must be positive.");

        Current += value;

        OnIncreased.Invoke();
    }
}
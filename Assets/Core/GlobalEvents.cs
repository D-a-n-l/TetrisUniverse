using System.Collections;
using System;

public static class GlobalEvents
{
    public static Action<int> OnMovementFinished;

    public static Action<int> OnDeletedRow;

    public static Action OnEditedGrid;

    public static Action OnGameOver;
}
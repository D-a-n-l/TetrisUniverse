using UnityEngine;

public class MathfCalculations
{
    public static Vector3 RoundVector(Vector3 vector)
    {
        return new Vector3(Mathf.Round(vector.x), Mathf.Round(vector.y), Mathf.Round(vector.z));
    }

    public static Vector3Int RotateRoundToInt(Quaternion rotation, Vector3Int vector)
    {
        Vector3 newVector = rotation * vector;
        return new Vector3Int(Mathf.RoundToInt(newVector.x), Mathf.RoundToInt(newVector.y), Mathf.RoundToInt(newVector.z));
    }
}
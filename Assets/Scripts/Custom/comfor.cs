using UnityEngine;

[System.Serializable]
public static class comfor
{
    public static Vector3 GetVector(Vector3 from, Vector3 to)
    {
        return (to - from).normalized;
    }

    public static float GetSqr(Vector3 a, Vector3 b)
    {
        return (a - b).sqrMagnitude;
    }
}

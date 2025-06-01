using UnityEngine;

public static class CustomGravityForCamera
{
    private static Vector3 currentGravityDir = Vector3.down;

    public static void SetGravity(Vector3 direction)
    {
        currentGravityDir = direction.normalized;
    }

    public static Vector3 GetCurrentGravityUp()
    {
        return -currentGravityDir; // up is opposite of gravity
    }

    public static Vector3 GetGravity()
    {
        return currentGravityDir;
    }
}

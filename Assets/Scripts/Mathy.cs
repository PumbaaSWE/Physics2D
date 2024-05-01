using UnityEngine;

public static class Mathy
{

    /// <summary>
    /// r is radius, h is h and needs to be 0<h<r
    /// </summary>
    /// <param name="r"></param>
    /// <param name="h"></param>
    /// <returns></returns>
    public static float AreaOfCircleSegment(float r, float h)
    {
        float r2 = r * r;
        float rh = r - h;
        float a = r2 * Mathf.Acos(1-h/r) - rh * Mathf.Sqrt(r2 - rh * rh);
        return a;
    }

    public static float AreaOfCircle(float r)
    {
        return Mathf.PI * r * r;
    }

    public static Vector3 ToVec3(this Vector2 v, float z = 0)
    {
        return new Vector3(v.x, v.y, z);
    }
    public static Vector3 WithY(this Vector3 v, float y = 0)
    {
        return new Vector3(v.x, y, v.z);
    }
}

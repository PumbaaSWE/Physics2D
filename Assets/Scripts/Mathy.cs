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

    public static Vector3 WithZ(this Vector3 v, float z = 0)
    {
        return new Vector3(v.x, v.y, z);
    }

 
    public static float Cross(Vector2 a, Vector2 b)
    {
        return a.x * b.y - a.y * b.x;
    }

    /// <summary>
    /// 2D
    /// </summary>
    /// <param name="a"></param>
    /// <param name="s"></param>
    /// <returns></returns>
    public static Vector2 Cross(Vector2 a, float s)
    {
        return new Vector2(s * a.y, -s * a.x);
    }
    /// <summary>
    /// Fake 2d cross prod, think it's 3d with 0s filled..
    /// </summary>
    /// <param name="s"></param>
    /// <param name="a"></param>
    /// <returns></returns>
    public static Vector2 Cross(float s, Vector2 a)
    {
        return new Vector2(-s * a.y, s * a.x);
    }

}

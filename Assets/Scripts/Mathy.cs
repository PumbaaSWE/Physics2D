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
}

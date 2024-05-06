using System;
using System.Collections.Generic;
using UnityEngine;

public static class HullUtils
{

    public const int DEGENERATE = -1;
    public const int CW = 0;
    public const int CCW = 1;


    public static bool CreateHull(List<Vector2> points, ref Vector2[] hull)
    {
        if(points.Count < 3 || points.Count > 255)
        {
            return false;
        }
        int winding = CheckWinding(points);
        if (winding == DEGENERATE) return false;
        //we have valid geometry
        hull = points.ToArray();

        if(winding == CCW)
        {
            Array.Reverse(hull);
        }
        Recenter(hull);
        return true;
    }

    /// <summary>
    /// Moves all points to have geometric center in origo
    /// </summary>
    /// <param name="hull"></param>
    public static Vector2 Recenter(in Vector2[] hull)
    {
        Vector2 c = hull[0];
        for (int i = 1; i < hull.Length; i++)
        {
            c += hull[i];
        }
        c /= hull.Length;
        for (int i = 0; i < hull.Length; i++)
        {
            hull[i] -= c;
        }
        return c;
    }

    public static int CheckWinding(List<Vector2> points)
    {
        int n = points.Count;
        Vector2 e0 = points[n - 2] - points[n-1];
        Vector2 e1 = points[0] - points[n-1];
        float s0 = Mathf.Sign(Mathy.Cross(e0, e1));
        for (int i = 0; i < n-1; i++)
        {
            e0 = -e1;
            e1 = points[i + 1] - points[i];
            float s1 = Mathf.Sign(Mathy.Cross(e0, e1));
            if (s0 != s1) return DEGENERATE;
            //s0 = s1;
        }
        if(s0 == 1f)
        {
            return CCW;
        }  
        return CW;
    }

    public static int CheckWinding(List<Vector3> points)
    {
        int n = points.Count;
        if (n < 3 || n > 255) return DEGENERATE;
        Vector2 e0 = points[n - 2] - points[n - 1];
        Vector2 e1 = points[0] - points[n - 1];
        float s0 = Mathf.Sign(Vector3.Cross(e0, e1).z);
        for (int i = 0; i < n - 1; i++)
        {
            e0 = -e1;
            e1 = points[i + 1] - points[i];
            float s1 = Mathf.Sign(Vector3.Cross(e0, e1).z);
            if (s0 != s1) return DEGENERATE;
            //s0 = s1;
        }
        if (s0 == 1f)
        {
            return CW;
        }
        return CCW;
    }

    //assumes correct hull
    public static Mesh GetMesh(Vector2[] hull)
    {
        Mesh mesh = new Mesh();
        //List<Vector3> points = new List<Vector3>();
        Vector3[] verts = new Vector3[hull.Length];
        int[] tris = new int[hull.Length * 3];
        Vector2 min = new Vector2(float.MaxValue, float.MaxValue);
        Vector2 max = new Vector2(float.MinValue, float.MinValue);
        Vector2[] uvs = new Vector2[hull.Length];
        for (int i = 0; i < hull.Length; i++)
        {
            Vector2 v = hull[i];
            //points.Add(v);
            verts[i] = v;
            min = Mathy.Min(v, min);
            max = Mathy.Max(v, max);
        }
        for (int i = 1; i < hull.Length-1; i++)
        {
            tris[i*3] = 0;
            tris[i*3] = i;
            tris[i*3 + 1] = i + 1;
        }

        //compute uvs
        float width = max.x - min.x;
        float height = max.y - min.y;

        float size = Mathf.Max(width, height);

        for (int i = 0; i < hull.Length; i++)
        {
            Vector2 v = hull[i];
            float x = v.x - min.x;
            float y = v.y - min.y;
            uvs[i] = new Vector2(x/ size, y/ size);
        }

        //mesh.SetVertices(points);
        mesh.vertices = verts;
        mesh.triangles = tris;
        mesh.uv = uvs;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        //mesh.Optimize();
        return mesh;
    }
}

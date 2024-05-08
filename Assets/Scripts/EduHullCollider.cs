using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.U2D;

public class EduHullCollider : MonoBehaviour
{

    public Vector2[] vertices;
    public float dencity = 1;
    public Vector2[] world;
    bool dirty;
    //ReadOnlySpan<Vector2> a;
    SpriteRenderer sr;
    public Vector2 Center => transform.position;

    // Start is called before the first frame update
    void Start()
    {
        //CreateSquare();
        sr = GetComponent<SpriteRenderer>();
    }

    public ReadOnlySpan<Vector2> GetWorldCoords()
    {
        if(!dirty) return world;
        for (int i = 0; i < vertices.Length; i++)
        {
            world[i] = transform.TransformPoint(vertices[i]);
        }
        dirty = false;
        return world;
    }

    /// <summary>
    /// Finds the vertex/point furthest along given direction and the distance on the projection axis aka Dot(direction, vertex)
    /// </summary>
    /// <param name="direction">Direction to find point in</param>
    /// <param name="max">Distance on the projection axis aka Dot(direction, vertex)</param>
    /// <returns>Vector2 vertex furthest along given direction</returns>
    public Vector2 SupportPoint(Vector2 direction, out float max)
    {
        max = float.MinValue;
        Vector2 bestVertex = Vector2.zero;

        //we need to do dirty check
        if(dirty) GetWorldCoords();

        for (int i = 0; i < world.Length; i++)
        {
            Vector2 vertex = world[i];
            float projection = Vector2.Dot(direction, vertex);
            if (projection > max)
            {
                max = projection;
                bestVertex = vertex;
            }
        }
        return bestVertex;
    }

    public void Intersecting()
    {
        if (sr)
        {
            sr.color = Color.red;
            StartCoroutine(ResetColor(0.1f));
        }
    }

    private IEnumerator ResetColor(float t)
    {
        yield return new WaitForSeconds(t);
        sr.color = Color.white;
    }

    public bool CreateFromList(List<Vector2> points) {

        //check validity
        if(points.Count < 3 || points.Count > 255) return false;
        int winding = IsConvex(points);
        if(winding == -1) return false; //non convex

        //we have a convex shape
        vertices = points.ToArray();
        world = points.ToArray();
        if (winding != 0)
        {
            Array.Reverse(vertices);
        }

        //compute center
        Vector2 center = vertices[0];
        for (int i = 1; i < vertices.Length; i++)
        {
            center += vertices[i];
        }
        center /= vertices.Length;
        //reset verts around center
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] -= center;
        }
        transform.position = center;

        //compute inertia and mass
        TryCreateSpriteShape();

        TryComputeMassAndInertia();
        return true;
    }


    public int IsConvex(List<Vector2> points)
    {
        Vector2 e0 = points[^1] - points[0];
        Vector2 e1 = points[1] - points[0];
        int sign = Math.Sign(Mathy.Cross(e0,e1));
        for (int i = 1; i < points.Count; i++)
        {
            e0 = points[i-1] - points[i];
            e1 = points[(i+1) % points.Count] - points[i];
            int sign2 = Math.Sign(Mathy.Cross(e0, e1));
            if(sign != sign2) return -1;
        }

        if(sign > 0) return 1;

        return 0;
    }

    public void TryCreateSpriteShape()
    {
        if (TryGetComponent(out SpriteShapeController sc))
        {
            sc.spline.Clear();
            //sc.spline.GetPointCount();
            for (int i = 0; i < vertices.Length; i++)
            {
                sc.spline.InsertPointAt(i, vertices[i]);
                sc.spline.SetHeight(i, .1f);
                sc.spline.SetTangentMode(i, ShapeTangentMode.Linear);
                sc.spriteShapeRenderer.localBounds.Encapsulate(vertices[i]);
            }
            //var a = sc.BakeMesh();
            sc.spriteShapeRenderer.ResetBounds();
        }
    }

    public void TryComputeMassAndInertia()
    {
        if (TryGetComponent(out EduRigidBody rb))
        {
            Vector2 c = Vector2.zero;
            float a = 0;
            for (int i = 0; i < vertices.Length; i++)
            {
                Vector2 e0 = vertices[i] - c;
                Vector2 e1 = vertices[(i+1) % vertices.Length] - c;
                a += Mathy.Cross(e0, e1) / 2;
            }


            rb.mass = a * dencity;


            Vector2 min = new Vector2(float.MaxValue, float.MaxValue);
            Vector2 max = new Vector2(float.MinValue, float.MinValue);
            for (int i = 0; i < vertices.Length; i++)
            {
                Vector2 v = vertices[i];
                min = Mathy.Min(v, min);
                max = Mathy.Max(v, max);
            }

            //compute uvs
            float width = max.x - min.x;
            float height = max.y - min.y;






            rb.inertia = rb.mass * (width*width + height*height) / 12; // approx rectangle
        }
    }
    

    public void CreateSquare()
    {
        vertices = new Vector2[4];
        world = new Vector2[vertices.Length];
        float e = 0.5f;
        vertices[0] = new Vector2(-e, -e);
        vertices[1] = new Vector2(e, -e);
        vertices[2] = new Vector2(e, e);
        vertices[3] = new Vector2(-e, e);





        float m = e * 4 * dencity; //w*l=V
        float l = e * 2;
        float I = l * l * 2 * m / 12f; //(1/12)*m*(l^2 + b^2)

        if (TryGetComponent(out EduRigidBody rb))
        {
            //ComputeMassInertia(rb);
            rb.mass = m;
            rb.inertia = I;
        }

        TryCreateSpriteShape();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        dirty = true;
    }

    private void OnValidate()
    {
        //CreateSquare();
    }

    private void OnDrawGizmos()
    {
        if (vertices == null) return;
        if (vertices.Length < 2) return;
        Gizmos.color = Color.green;
        //Gizmos.DrawLineStrip(vertices, true);
        Vector3 p0 = transform.TransformPoint(vertices[vertices.Length - 1]);
        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 p1 = transform.TransformPoint(vertices[i]);// + Center;
            Gizmos.DrawLine(p0, p1);
            p0 = p1;
        }
        Gizmos.DrawLine(Center, transform.TransformPoint(vertices[0]));
    }

    internal void SetHull(Vector2[] verts)
    {
        vertices = verts;
        world = new Vector2[vertices.Length];
        TryComputeMassAndInertia();
        if (TryGetComponent(out MeshFilter filter))
        {
            filter.sharedMesh = HullUtils.GetMesh(verts);
        }
    }
}

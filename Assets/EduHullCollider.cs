using System;
using UnityEngine;
using UnityEngine.U2D;

public class EduHullCollider : MonoBehaviour
{

    public Vector2[] vertices;
    public float dencity = 1;

    public Vector2 Center => transform.position;

    // Start is called before the first frame update
    void Start()
    {
        //CreateSquare();
    }

    public void CreateSquare()
    {
        vertices = new Vector2[4];
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

        if (TryGetComponent(out SpriteShapeController sc))
        {
            sc.spline.Clear();
            //sc.spline.GetPointCount();
            for (int i = 0; i < 4; i++)
            {
                sc.spline.InsertPointAt(i, vertices[i]);
                sc.spline.SetHeight(i, .1f);
                sc.spline.SetTangentMode(i, ShapeTangentMode.Linear);
            }
            //var a = sc.BakeMesh();
            //sc.spriteShapeRenderer.
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnValidate()
    {
        CreateSquare();
    }

    private void OnDrawGizmos()
    {
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
}

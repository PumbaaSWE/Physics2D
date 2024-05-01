using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EduSolver))]
public class EduCollisionDetection : MonoBehaviour
{

    public List<EduCollision> collisions;
    public EduSolver solver;
    public int numLines;
    public int numCircles;
    
    
    // Start is called before the first frame update
    void Start()
    {
        collisions = new List<EduCollision> ();
        solver = GetComponent<EduSolver> ();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        EduCircleCollider[] circles = FindObjectsOfType<EduCircleCollider>();
        EduLineCollider[] lines = FindObjectsOfType<EduLineCollider>();
        numCircles = circles.Length;
        numLines = lines.Length;
        //this is a very simple setup, we could just do detection and collect all collitions and send that to a solver that solves based on
        //detected colision points/depth/normals and rigidbody data after
        //right now we also don not have correct data in collisions, and "shapes" are different classes, and some do not have RB, so need to handle that
        collisions.Clear();

        //All circles vs rest of static, non-moving stuff
        for (int i = 0; i < lines.Length; i++)
        {
            for (int c = 0; c < circles.Length; c++)
            {
                if (CircleLineCollision(circles[c], lines[i], out EduCollision collision))
                {
                    //resolve line vs circle
                    //collision.A = circles[c].gameObject;
                    //collision.B = lines[i].gameObject;
                    collision.B = circles[c].GetComponent<EduRigidBody>();
                    collision.A = null;// lines[i].GetComponent<EduRigidBody>();
                    collisions.Add(collision);
                    //Debug.DrawLine(collision.Position, collision.Position + collision.Normal, Color.red);
                }
            }
        }
        //All circles vs rest of circles
        for (int i = 0; i < circles.Length - 1; i++)
        {         
            for (int c = i + 1; c < circles.Length; c++)
            {
                if (CircleCircleCollision(circles[c], circles[i], out EduCollision collision))
                {
                    //resolve circle vs circle
                    collision.A = circles[c].GetComponent<EduRigidBody>();
                    collision.B = circles[i].GetComponent<EduRigidBody>();
                    collisions.Add(collision);
                }
            }
        }

        solver.Solve(collisions);

    }

    public bool CircleCircleCollision(EduCircleCollider c1, EduCircleCollider c2, out EduCollision collision)
    {
        Vector2 d = c2.transform.position - c1.transform.position; //relative position -> d for delta
        float sqrDist = Vector2.SqrMagnitude(d);
        float r = c1.ScaledRadius() + c2.ScaledRadius();

        if(sqrDist <= r * r)
        {  
            collision = new EduCollision();
            float dist = Mathf.Sqrt(sqrDist);
            collision.Position = d / 2; // in relation to c1... (c1+c2)/2 for world pos

            if (Mathf.Abs(dist) < 0.0001f) collision.Normal = Vector2.up;
            else collision.Normal = d / dist; //d.normalized;

            collision.Depth = r - dist; //amount of overlap
            return true;
        }
        collision = default;
        return false;
    }

    public bool CircleLineCollision(EduCircleCollider c, EduLineCollider l, out EduCollision collision)
    {
        Vector2 cp = l.ClosestPoint(c.Center);
        Vector2 d = c.Center - cp;

        //Debug.DrawLine(c.Center, cp, Color.magenta);


        float sqrDist = Vector2.SqrMagnitude(d);
        if(sqrDist <= c.ScaledRadius() * c.ScaledRadius())
        {
            //we have collided with the line;
            collision = new EduCollision();
            float dist = Mathf.Sqrt(sqrDist);
            collision.Position = cp;
            collision.Normal = d / dist; //d.normalized;
            collision.Depth = c.ScaledRadius() - dist; //amount of overlap
            return true;
        }
        collision = default;
        return false;
    }
}

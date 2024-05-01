using UnityEngine;

public class EduCollisionDetection : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        EduCircleCollider[] circles = FindObjectsOfType<EduCircleCollider>();
        EduLineCollider[] lines = FindObjectsOfType<EduLineCollider>();

        //this is a very simple setup, we could just do detection and collect all collitions and send that to a solver that solves based on
        //detected colision points/depth/normals and rigidbody data after
        //right now we also don not have correct data in collisions, and "shapes" are different classes, and some do not have RB, so need to handle that

        for (int i = 0; i < lines.Length; i++)
        {
            for (int c = 0; c < circles.Length; c++)
            {
                if (CircleLineCollision(circles[c], lines[i], out EduCollision collision))
                {
                    //resolve line vs circle
                }
            }
        }
        for (int i = 0; i < circles.Length - 1; i++)
        {         
            for (int c = i + 1; c < circles.Length; c++)
            {
                if (CircleCircleCollision(circles[c], circles[i], out EduCollision collision))
                {
                    //resolve circle vs circle
                }
            }
        }

    }

    public bool CircleCircleCollision(EduCircleCollider c1, EduCircleCollider c2, out EduCollision collision)
    {
        Vector2 d = c2.transform.position - c1.transform.position; //relative position -> d for delta
        float sqrDist = Vector2.SqrMagnitude(d);
        float r = c1.radius + c2.radius;

        if(sqrDist <= r * r)
        {  
            collision = new EduCollision();
            float dist = Mathf.Sqrt(sqrDist);
            collision.Position = d / 2; // in relation to c1... (c1+c2)/2 for world pos
            collision.Normal = d / dist; //d.normalized;
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
        float sqrDist = Vector2.SqrMagnitude(d);
        if(sqrDist <= c.radius * c.radius)
        {
            //we have collided with the line;
            collision = new EduCollision();
            float dist = Mathf.Sqrt(sqrDist);
            collision.Position = d / 2; // in relation to c1... (c1+c2)/2 for world pos
            collision.Normal = d / dist; //d.normalized;
            collision.Depth = c.radius - dist; //amount of overlap
            return true;
        }
        collision = default;
        return false;
    }
}

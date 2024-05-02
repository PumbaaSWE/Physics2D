using UnityEngine;

public class EduLineCollider : MonoBehaviour
{
    
    public float length = 1;
    public Vector2 p1;
    public Vector2 p2;
    public Vector2 Normal => transform.up;
    
    // Start is called before the first frame update
    void Start()
    {
        UpdatePoints();
    }

    

    // Update is called once per frame
    void FixedUpdate()
    {
        UpdatePoints(); // really only needed if position/rotation changes...
    }

    private void UpdatePoints()
    {
        float hl = length / 2;
        p1 = transform.position - transform.right * hl;
        p2 = transform.position + transform.right * hl;
    }

    public Vector2 ClosestPoint(Vector2 p)
    {
        Vector2 d = p2 - p1;
        float t = Vector2.Dot(p - p1, d);
        if (t <= 0.0f)
        {
            return p1;
        }
        else
        {
            float denom = Vector2.Dot(d, d);
            if (t >= denom)
            {
                return p2;
            }
            else
            {
                t /= denom;
                return p1 + t * d;
            }
        }
    }


    private void OnValidate()
    {
        UpdatePoints();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(p1, .1f);
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(p2, .1f);
    }

    public void OnDrawGizmos()
    {
        float hl = length / 2;
        Vector3 a = transform.position - transform.right * hl;
        Vector3 b = transform.position + transform.right * hl;
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(a, b);
    }
}

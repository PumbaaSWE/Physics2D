using UnityEngine;

public class AnalyticalLines : MonoBehaviour
{
    public float dt = 0.01f;
    public int segments = 100;
    public Vector2 v0;
    public Vector2 v;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 p0 = transform.position;
        for (int i = 0; i < segments; i++)
        {
            float t = dt * i;
            float x = v0.x * t;
            float y = v0.y * t + 0.5f * -9.82f * t * t;
            Vector2 p1 = new(x, y);
            Debug.DrawLine(p0, p1, Color.white);
            p0 = p1;
        }
    }

    public void OnDrawGizmos()
    {
        Vector2 p0 = transform.position;
        for (int i = 1; i <= segments; i++)
        {
            float t = dt * i;
            float x = v0.x * t;
            float y = v0.y * t + 0.5f * -9.82f * t * t;
            Vector2 p1 = new Vector2(x, y);
            Gizmos.DrawLine(p0, p1);
            p0 = p1;
        }
    }
}

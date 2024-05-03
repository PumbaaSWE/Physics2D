using UnityEngine;

public class EduWindZone : MonoBehaviour
{
    public float width = 5;
    public float height = 5;

    public int rows = 5;
    public int columns = 5;

    public float maxStrength = 1;
    public float minStrength = 1;

    Vector3[,] field;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void CreateField()
    {
        field = new Vector3[rows, columns];
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                field[i, j] = Random.onUnitSphere.WithZ();// *Random.Range(minStrength, minStrength);
            }
        }
        DoWhirlwind();
    }

    public void DoWhirlwind()
    {
        float h = height / rows;
        float w = width / columns;
        Vector3 center = new Vector3(width / 2 - w/2, height / 2-h/2) + transform.position;
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                Vector3 pos = new Vector3(w * j, h * i) + transform.position; // our pos
                Vector3 d = pos - center;
                Vector3 dir = Vector3.Cross(Vector3.forward, d) * minStrength;
                dir -= d * maxStrength;
                field[i, j] = dir;
            }
        }
    }

    public void DoStraight()
    {
        Vector3 dir  = Random.onUnitSphere.WithZ() * maxStrength;
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                field[i, j] = dir;
            }
        }
    }


    public Vector2 Sample(Vector3 pos)
    {
        pos -= transform.position;
        int x = (int)(pos.x / width * columns);
        int y = (int)(pos.y / height * rows);

        Debug.DrawLine(pos + transform.position, transform.position);

        if (x < 0 || x >= columns) return Vector2.zero;
        if (y < 0 || y >= rows) return Vector2.zero;

        Debug.Log("x = " + x + " y=" + y);

        Vector3 dir = field[y, x];

        Debug.DrawLine(pos + transform.position, pos + transform.position + dir);

        return dir;
        //return new Vector2(0, 0);
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnValidate()
    {
        CreateField();
    }

    //private Vector2 PosX(int row, int col)
    //{
    //    return
    //}

    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        float h = height / rows;
        float w = width / columns;

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                Vector3 p0 = new Vector3(w * j, h * i) + transform.position;
                Gizmos.DrawLine(p0, p0 + field[i, j]/5f);
                Gizmos.DrawWireSphere(p0, .1f);
            }
        }
    }
}

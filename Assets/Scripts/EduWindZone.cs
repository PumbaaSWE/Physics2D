using UnityEngine;

public class EduWindZone : MonoBehaviour
{
    public float width = 5;
    public float height = 5;

    public int rows = 5;
    public int columns = 5;

    [Header("They do different things based on FieldType")]
    public float maxStrength = 1;
    public float minStrength = 1;

    public float param1 = 1;
    public float param2 = 1;

    public enum FieldType { WhirlWind, Stright, Perlin, Random }
    public FieldType fieldType = FieldType.WhirlWind;

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
                field[i, j] = Random.onUnitSphere.WithZ().normalized * Random.Range(minStrength, minStrength);
            }
        }
        DoSelected();
        
    }

    //Kan bryta ut FieldType till egen klass/inteface/scriptable object så man kan skapa olika typer, eller nått
    public void DoSelected()
    {
        switch (fieldType)
        {
            case FieldType.WhirlWind:
                DoWhirlwind();
                break;
            case FieldType.Stright:
                DoStraight();
                break;
            case FieldType.Perlin:
                DoPerlin();
                break;
            case FieldType.Random:
                break;
            default:
                break;
        }
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
        Vector3 dir  = new Vector3(maxStrength, minStrength, 0);
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                field[i, j] = dir;
            }
        }
    }

    public void DoPerlin()
    {
        //Vector3 dir = Random.onUnitSphere.WithZ() * maxStrength;
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                float c = Mathf.PerlinNoise(i * minStrength + param1, j * minStrength + param2);
                float cx = Mathf.PerlinNoise((i + 1) * minStrength + param1, j * minStrength + param2);
                float cy = Mathf.PerlinNoise(i * minStrength + param1, (j + 1) * minStrength + param2);


                field[i, j] = new Vector3(cx-c, cy-c) * maxStrength;
            }
        }
    }


    public Vector2 Sample(Vector3 pos)
    {
        pos -= transform.position;

        //we are outside the windzone I think
        if (pos.x < 0 || pos.x >= width) return Vector2.zero;
        if (pos.y < 0 || pos.y >= height) return Vector2.zero;

        int x = (int)(pos.x / width * columns);
        int y = (int)(pos.y / height * rows);

        Vector3 dir = GetWindVectorAt(x, y);

        Debug.DrawLine(pos + transform.position, pos + transform.position + dir);

        return dir;
        //return new Vector2(0, 0);
    }

    public Vector2 SampleBilinear(Vector3 pos)
    {
        
        pos -= transform.position;

        //we are outside the windzone
        if (pos.x < 0 || pos.x >= width) return Vector2.zero;
        if (pos.y < 0 || pos.y >= height) return Vector2.zero;

        int x = (int)(pos.x / width * columns);
        int y = (int)(pos.y / height * rows);

        Vector3 dir = GetWindVectorAt(x,y);
        dir += GetWindVectorAt(x+1, y);
        dir += GetWindVectorAt(x+1, y+1);
        dir += GetWindVectorAt(x, y+1);
        //kunde lerpat mellan varje vector för smooth transitions? nu är den som sample nästan...  palla overkill


        //Debug.DrawLine(pos + transform.position, pos + transform.position + GetWindVectorAt(x, y), Color.cyan);
        //Debug.DrawLine(pos + transform.position, pos + transform.position + GetWindVectorAt(x, y+1), Color.cyan);
        //Debug.DrawLine(pos + transform.position, pos + transform.position + GetWindVectorAt(x+1, y+1), Color.cyan);
        //Debug.DrawLine(pos + transform.position, pos + transform.position + GetWindVectorAt(x+1, y), Color.cyan);

        //Debug.DrawLine(pos + transform.position, pos + transform.position + dir * 0.25f, Color.magenta);

        return dir * 0.25f;
    }


    public Vector3 GetWindVectorAt(int x, int y)
    {
        if (x < 0 || x >= columns) return Vector2.zero;
        if (y < 0 || y >= rows) return Vector2.zero;
        Vector3 dir = field[y, x];
        return dir;
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

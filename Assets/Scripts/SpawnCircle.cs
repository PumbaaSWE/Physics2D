using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(LineRenderer))]
public class SpawnCircle : MonoBehaviour
{
    public GameObject circle;
    public GameObject eduLine;
    public float minLength = .5f;

    public EduHullCollider hull;
    Vector3 posB;
    Vector3 posA;

    LineRenderer lineRenderer;
    Camera cam;
    List<Vector3> points = new List<Vector3>();

    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = .1f;
    }

    // Update is called once per frame
    void Update()
    {
        cam = Camera.main;
        if (Input.GetKey(KeyCode.P))
        {
            DrawPolygon();
        }
        else if (Input.GetKeyDown(KeyCode.P))
        {
            lineRenderer.enabled = true;
            points.Clear();
        }
        else if (Input.GetKeyUp(KeyCode.P))
        {
            lineRenderer.positionCount = 2;
            lineRenderer.enabled = false;
            //lineRenderer.
            points.Clear();
        }
        else
        {
            SpawnCirclesAndLines();
        }

    }

    private void SpawnCirclesAndLines()
    {
        if (circle && Input.GetMouseButtonDown(0))
        {


            Vector3 pos = cam.ScreenToWorldPoint(Input.mousePosition);

            Instantiate(circle, pos.WithZ(), Quaternion.identity);
        }
        if (Input.GetMouseButtonDown(1))
        {
            posA = cam.ScreenToWorldPoint(Input.mousePosition).WithZ();
            lineRenderer.enabled = true;
        }

        if (Input.GetMouseButton(1))
        {
            posB = cam.ScreenToWorldPoint(Input.mousePosition).WithZ();
            lineRenderer.SetPosition(0, posA);
            lineRenderer.SetPosition(1, posB);
        }
        if (Input.GetMouseButtonUp(1))
        {
            float dist = Vector3.Distance(posA, posB);
            if (dist > minLength)
            {
                Vector3 pos = (posA + posB) / 2;
                GameObject go = Instantiate(eduLine, pos.WithZ(), Quaternion.LookRotation(Vector3.forward, Vector3.Cross(Vector3.forward, posB - posA)));
                go.GetComponent<EduLineCollider>().length = dist;
            }
            lineRenderer.enabled = false;
        }
    }


    private void DrawPolygon()
    {
        Vector3 point = cam.ScreenToWorldPoint(Input.mousePosition).WithZ();
        if (Input.GetMouseButtonDown(0))
        {
            if (points.Count > 2 && Vector3.Distance(point, points[0]) < 0.5f)
            {
                //Debug.Log("Create Polygon");
                EduHullCollider h = Instantiate(hull);
                List<Vector2> points2d = new List<Vector2>(); //annoying!!!!
                for (int i = 0; i < points.Count; i++)
                {
                    points2d.Add(points[i]); 
                }
                if (h.CreateFromList(points2d)) //this should be checked first... 
                {
                    Debug.Log("Created Polygon Successfully");
                }
            }
            else
            {
                points.Add(point); //could do more checks to prevent degenerate mesh
            }
        }
        if(points.Count > 0)
        {
            lineRenderer.positionCount = points.Count + 1;
            lineRenderer.enabled = true;
            lineRenderer.SetPositions(points.ToArray());
            lineRenderer.SetPosition(points.Count, point);
        }
        else
        {
            lineRenderer.enabled = false;
        }
    }
}

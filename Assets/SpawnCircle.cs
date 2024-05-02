using UnityEngine;
[RequireComponent(typeof(LineRenderer))]
public class SpawnCircle : MonoBehaviour
{
    public GameObject circle;
    public GameObject eduLine;
    public float minLength = .5f;
    Vector3 posB;
    Vector3 posA;

    LineRenderer lineRenderer;

    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = .1f;
    }

    // Update is called once per frame
    void Update()
    {
        Camera cam = Camera.main;
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

        if(Input.GetMouseButton(1))
        {
            posB = cam.ScreenToWorldPoint(Input.mousePosition).WithZ();
            lineRenderer.SetPosition(0,posA);
            lineRenderer.SetPosition(1,posB);
        }
        if (Input.GetMouseButtonUp(1))
        {
            float dist = Vector3.Distance(posA, posB);
            if (dist > minLength)
            {
                Vector3 pos = (posA + posB) / 2;
                GameObject go = Instantiate(eduLine, pos.WithZ(), Quaternion.LookRotation(Vector3.forward, Vector3.Cross(Vector3.forward, posB-posA)));
                go.GetComponent<EduLineCollider>().length = dist;
            }
            lineRenderer.enabled = false;
        }
    }
}

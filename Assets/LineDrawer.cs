using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineDrawer : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public EduLineCollider lineCollider;
    
    // Start is called before the first frame update
    void Start()
    {
        lineCollider = GetComponent<EduLineCollider>();
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
    }

    // Update is called once per frame
    void Update()
    {
        lineRenderer.SetPosition(0, lineCollider.p1);
        lineRenderer.SetPosition(1, lineCollider.p2);
    }
}

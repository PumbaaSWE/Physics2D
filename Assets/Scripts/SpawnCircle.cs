using System;
using System.Collections.Generic;
using System.Linq;
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

    public ParticleSystem particlePrefab;
    public EduExplosionObject explosionObject;

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

        if (Input.GetKeyDown(KeyCode.E))
        {
            EduRigidBody[] rbs = FindObjectsOfType<EduRigidBody>();
            Vector3 pos = cam.ScreenToWorldPoint(Input.mousePosition).WithZ();
            if(particlePrefab)Instantiate(particlePrefab, pos, Quaternion.identity);
            for (int i = 0; i < rbs.Length; i++)
            {
                rbs[i].ApplyExplosionForce(500, pos, 2.05f);
            }
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            EduRigidBody[] rbs = FindObjectsOfType<EduRigidBody>();
            Vector3 pos = cam.ScreenToWorldPoint(Input.mousePosition).WithZ();
            if (explosionObject)
            {
                EduExplosionObject e = Instantiate(explosionObject, pos, Quaternion.identity);
                e.DestroyOnExplode = true;
            }
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            EduForces ef = FindFirstObjectByType<EduForces>();
            if(ef) ef.UseGravity = !ef.UseGravity;
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
            if (points.Count > 2 && Vector3.Distance(point, points[0]) < minLength)
            {
                //Debug.Log("Try Create Polygon");
                int winding = HullUtils.CheckWinding(points);
                if (winding != HullUtils.DEGENERATE)
                {
                    //Debug.Log("Create Hull");
                    Vector2[] verts = new Vector2[points.Count];
                    //for (int i = 0; i < points.Count; i++)
                    //{
                    //    verts[i] = points[i];
                    //}
                    verts[0] = points[0];
                    if (winding == HullUtils.CCW)
                    {
                        for (int i = 1; i < points.Count; i++)
                        {
                            verts[i] = points[i];
                        }
                    }
                    else
                    {
                        //Array.Reverse(verts);
                        for (int i = 1; i < points.Count; i++)
                        {
                            verts[i] = points[points.Count - i];
                        }
                        // Debug.Log("winding CW " + winding);
                    }
                    Vector2 c = HullUtils.Recenter(verts);

                    EduHullCollider h = Instantiate(hull, c, Quaternion.identity);
                    h.SetHull(verts);
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

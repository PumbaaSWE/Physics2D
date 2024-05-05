using System;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using Unity.VisualScripting;
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
                    collision.A = null;// lines[i].GetComponent<EduRigidBody>();
                    collision.B = circles[c].GetComponent<EduRigidBody>();
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

        EduHullCollider[] hulls = FindObjectsOfType<EduHullCollider>();
        for (int a = 0; a < hulls.Length - 1; a++)
        {
            for (int b = a + 1; b < hulls.Length; b++)
            {
                if (HullHullCollision(hulls[a], hulls[b], out EduCollision collision, out bool swap))
                {
                    //whay do we swap? why not flip the normal? Refrence plane needs to be on the correct side!

                    hulls[a].Intersecting(); //debug callback

                    if (swap)
                    {
                        collision.A = hulls[b].GetComponent<EduRigidBody>();
                        collision.B = hulls[a].GetComponent<EduRigidBody>();
                    }
                    else
                    {
                        collision.A = hulls[a].GetComponent<EduRigidBody>();
                        collision.B = hulls[b].GetComponent<EduRigidBody>();
                    }
                    collisions.Add(collision);
                }
            }
        }

        for (int c = 0; c < circles.Length; c++)
        {
            for (int h = 0; h < hulls.Length; h++)
            {
                if (CircleHullCollision(circles[c], hulls[h], out EduCollision collision))
                {
                    collision.A = circles[c].GetComponent<EduRigidBody>();
                    collision.B = hulls[h].GetComponent<EduRigidBody>();

                    collisions.Add(collision);
                }
            }
        }


            solver.Solve(collisions);

    }



    private bool CircleHullCollision(EduCircleCollider c, EduHullCollider h, out EduCollision collision)
    {

        Vector2 axis = c.Center - h.Center;

        float r = c.ScaledRadius();
        float min = ProjectCircle(c.Center, r, axis); //negate the radius as the support points are calculated backwards
        var points = h.GetWorldCoords();
        SupportPoint(points, axis, out float max);

        //Debug.DrawLine(Vector3.zero, axis, Color.white);
        //Debug.DrawLine(h.Center, c.Center, Color.yellow);

        //Debug.DrawLine(c.Center, c.Center + axis.normalized * min, Color.blue);
        //Debug.DrawLine(h.Center, h.Center + axis.normalized * max, Color.red);

        float distance = min - max; //signed distance, negative means overlapping on this axis
        collision = default;
        if (distance > 0) return false;
        //Debug.Log("Disttance = " + distance);

        Vector2 normal = Vector2.up;
        float bestDist = float.MaxValue;
        Vector2 prev = points[^1];
        for (int i = 0; i < points.Length; i++)
        {
            Vector2 curr = points[i];
            Vector2 faceNormal = -new Vector2(-(curr.y - prev.y), curr.x - prev.x);

            max = ProjectCircle(c.Center, r, faceNormal); //like get support point but for circle (and not the actual point)

            min = Vector2.Dot(faceNormal, curr); //the distance along the axis of current face
            distance = min - max; //signed distance, positive means overlapping on this axis *Opposite Hull vs Hull because outward facing normals!

            if (distance < 0) return false; // if negative we can exit early because we found a separating axis! -> no collision

            if (bestDist > distance)
            {
                bestDist = distance;
                normal = faceNormal;
            }
            prev = curr;
        }
        collision = new EduCollision
        {
            Normal = -normal.normalized,
            Position = c.Center - normal * bestDist, //eh
            Depth = bestDist,
            ContactCount = 1
        };

        return true;
    }

    private static float ProjectCircle(Vector2 center, float radius, Vector2 axis)
    {
        Vector2 direction = axis.normalized;
        return Vector2.Dot(axis, center - direction * radius);

        //Dot(axis, center - direction * radius) is the real formula... I made this one up myself to avoid normalizing the axis 

        //float d = Vector2.Dot(axis, center);
        //return d - Mathf.Sign(d) * radius;
    }

    public bool CircleCircleCollision(EduCircleCollider c1, EduCircleCollider c2, out EduCollision collision)
    {
        Vector2 d = c2.transform.position - c1.transform.position; //relative position -> d for delta A->B
        float sqrDist = Vector2.SqrMagnitude(d);
        float r = c1.ScaledRadius() + c2.ScaledRadius();

        if(sqrDist <= r * r)
        {  
            collision = new EduCollision();
            float dist = Mathf.Sqrt(sqrDist);
            collision.Position = (c2.transform.position + c1.transform.position) / 2; // world pos

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
        Vector2 d = c.Center - cp; //line->ball

        //Debug.DrawLine(c.Center, cp, Color.magenta);


        float sqrDist = Vector2.SqrMagnitude(d);
        if(sqrDist <= c.ScaledRadius() * c.ScaledRadius())
        {
            //we have collided with the line;
            collision = new EduCollision();
            float dist = Mathf.Sqrt(sqrDist);
            collision.Position = cp;
            collision.Normal = d / dist; //d.normalized; normal is line->ball 
            collision.Depth = c.ScaledRadius() - dist; //amount of overlap
            return true;
        }
        collision = default;
        return false;
    }


    public static bool HullHullCollision(EduHullCollider hullA, EduHullCollider hullB, out EduCollision m, out bool swap)
    {
        m = default;
        swap = false;

        Query queryA = QueryFaceDirections(hullA, hullB);
        if (queryA.separation < 0.0f)
        {
            return false;
        }
        Query queryB = QueryFaceDirections(hullB, hullA);
        if (queryB.separation < 0.0f)
        {
            return false;
        }
        //we have Overlap

        m = new EduCollision();
        float depth = queryA.separation;
        Vector2 normal = queryA.face.n;
        Face reference = queryA.face;
        Face incident = queryB.face;


        if (Math.Abs(Math.Abs(queryA.separation) - Math.Abs(queryB.separation)) > 0.1f)
        {
            //some sort of bias was to be added to favour one axis
        }
        if (queryB.separation < depth)
        {
            depth = queryB.separation;
            normal = queryB.face.n;
            reference = queryB.face;
            incident = queryA.face;
            swap = true;
        }

        m.Depth = depth;
        m.Normal = normal;

        //we established witch shaped should be reference and incident, now do the clipping
        //this "algorithm" I found online... I cannot clip, also you could probably do it nicer and better and faster

        Vector2 refv = reference.b - reference.a;
        refv.Normalize();

        float o = Vector2.Dot(refv, reference.a);
        Clip(incident.a, incident.b, refv, o);
        if (clippingPoints.Count < 2) return true; // nah

        o = Vector2.Dot(refv, reference.b);
        Clip(clippingPoints[0], clippingPoints[1], -refv, -o);
        if (clippingPoints.Count < 2) return true; // nah

        float max = Vector2.Dot(-normal, reference.a);
        // make sure the final points are not past this maximum
        if (Vector2.Dot(-normal, clippingPoints[0]) - max < 0.0)
        {
            m.Position = clippingPoints[1];
            m.ContactCount = 1;
        }
        else if (Vector2.Dot(-normal, clippingPoints[1]) - max < 0.0)
        {
            m.Position = clippingPoints[0];
            m.ContactCount = 1;
        }
        else
        {
            m.Position = clippingPoints[0];
            m.Position2 = clippingPoints[1];
            m.ContactCount = 2;
        }

        return true;
    }

    private static List<Vector2> clippingPoints = new List<Vector2>(2);
    public static void Clip(Vector2 a, Vector2 b, Vector2 n, float t)
    {
        clippingPoints.Clear();
        float d1 = Vector2.Dot(n, a) - t;
        float d2 = Vector2.Dot(n, b) - t;
        if (d1 >= 0f) { clippingPoints.Add(a); }// add a
        if (d2 >= 0f) { clippingPoints.Add(b); } // add b
        if (d1 * d2 < 0f)
        {
            Vector2 e = b - a;
            // compute the location along e
            float u = d1 / (d1 - d2);
            e *= u;
            e += a;
            // add the point
            clippingPoints.Add(e);

        }
    }


    public static Query QueryFaceDirections(EduHullCollider hullA, EduHullCollider hullB)
    {
        float bestDistance = float.MinValue;
        int bestIndex = 0;
        Face bestFace = default;
        ReadOnlySpan<Vector2> points = hullA.GetWorldCoords();
        Vector2 prev = points[^1];
        for (int i = 0; i < points.Length; i++)
        {
            Vector2 curr = points[i];
            Vector2 faceNormal = new Vector2(-(curr.y - prev.y), curr.x - prev.x).normalized;      //for simple overlap check this does not need to be normalized, but now we compare with other queries for reasonsl later so consistancy is needed                                                              
            SupportPoint(hullB.GetWorldCoords(), faceNormal, out float max);

            float min = Vector2.Dot(faceNormal, curr);
            float distance = min - max;


            if (distance > bestDistance)
            {
                bestDistance = distance;
                bestIndex = i;
                bestFace = new Face(prev, curr, -faceNormal);
            }
            prev = curr;
        }

        return new Query(bestDistance, bestIndex, bestFace);
    }

    /// <summary>
    /// Finds the vertex/point furthest along given direction and the distance on the projection axis aka Dot(direction, vertex)
    /// </summary>
    /// <param name="direction">Direction to find point in</param>
    /// <param name="max">Distance on the projection axis aka Dot(direction, vertex)</param>
    /// <returns>Vector2 vertex furthest along given direction</returns>
    public static Vector2 SupportPoint(in ReadOnlySpan<Vector2> points, Vector2 direction, out float max)
    {
        max = float.MinValue;
        Vector2 bestVertex = Vector2.zero;
        for (int i = 0; i < points.Length; i++)
        {
            Vector2 vertex = points[i];
            float projection = Vector2.Dot(direction, vertex);
            if (projection > max)
            {
                max = projection;
                bestVertex = vertex;
            }
        }
        return bestVertex;
    }
}

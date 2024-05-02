using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class EduForces : MonoBehaviour
{

    public bool UseGravity = true;
    public Vector2 gravity = new Vector2(0, -9.82f);
    public bool UseBrakeTorque = false;
    public float brakeTorque = 0;
    public bool UseWind = false;
    public Vector2 wind = new Vector2(0, 0);
    public bool UseBouyance = false;
    public float fluidDencity = 1;
    public float fluidLevel = 0;
    private LineRenderer lineRenderer;



    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = .1f;

        DoForces();

    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawLine(new Vector3(-1000, fluidLevel, -0.1f), new Vector3(1000, fluidLevel, -0.1f), Color.red);

        if (UseBouyance && lineRenderer)
        {

            lineRenderer.SetPosition(0, new Vector3(-1000, fluidLevel, -0.1f));
            lineRenderer.SetPosition(1, new Vector3(1000, fluidLevel, -0.1f));
        }

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        DoForces();
    }

    private void DoForces()
    {
        EduRigidBody[] rbs = FindObjectsOfType<EduRigidBody>();
        for (int i = 0; i < rbs.Length; i++)
        {
            EduRigidBody rb = rbs[i];
            if (UseGravity) ApplyGravity(rb);
            if (UseBrakeTorque) ApplyBrorque(rb);
            if (UseWind) ApplyWind(rb);
            if (UseBouyance) ApplyFloaty(rb);
        }
    }

    private void ApplyBrorque(EduRigidBody rb)
    {
        rb.ApplyTorque(brakeTorque);
    }

    private void ApplyFloaty(EduRigidBody rb)
    {
        //
        float dy = rb.transform.position.y - fluidLevel;
        float r = rb.GetComponent<EduCircleCollider>().ScaledRadius(); // eh..
        float V = Mathy.AreaOfCircle(r); 
        if (dy >= r)
        {
            //no floating force, we above the surface
            Debug.Log("Fully above dy = " + dy);
            return;
        }
        else if (dy <= -r)
        {
            //we are fully submerged
            Debug.Log("Fully submerged dy = " + dy);
            //return;
        }
        else
        {
            if(dy > 0)
            {
                V = Mathy.AreaOfCircleSegment(r, r-dy);
            }
            else
            {
                V -= Mathy.AreaOfCircleSegment(r, r+dy);
            }
        }
        //Debug.Log(V / Mathy.AreaOfCircle(r) + " Area submerged");
        Debug.DrawLine(rb.transform.position, rb.transform.position.WithY(fluidLevel), Color.magenta);
        //Fl = Vobj * pfluid * g;
        Vector2 F = V * fluidDencity * -gravity;
        rb.ApplyForce(F);
    }

    private void ApplyGravity(EduRigidBody rb)
    {
        rb.ApplyForce(gravity * rb.mass); //times mass because it's an acceleration
    }

    private void ApplyWind(EduRigidBody rb)
    {
        rb.ApplyForce(wind);
    }
}

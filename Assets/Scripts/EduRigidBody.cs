using UnityEngine;

public class EduRigidBody : MonoBehaviour
{
    [Header("Properies")]
    public float restitution = 1;
    public int skipFrames = 0;
    private int skipped;

    [Header("Computed with Collider")]
    public float mass = 1;
    public float inertia = 1;


    [Header("Internals")]
    public Vector2 velocity;
    public float angularVelocity;
    public Vector2 accumulatedForces;
    public float accumulatedTorques;

    public float Ek;
    public float Ekr;
    public float Ep;
    public float Etot;


    
    // Start is called before the first frame update
    void Start()
    {
        skipped = skipFrames; //check this
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        if(++skipped > skipFrames) skipped = 0;
        if (skipped == 0) {
            
            float dt = (skipFrames + 1) * Time.fixedDeltaTime;
        

            //semi-Euler
            SemiEuler(dt);
        }

        Ek = mass * velocity.sqrMagnitude / 2;
        Ekr = inertia * angularVelocity * angularVelocity / 2;
        Ep = mass * 9.82f * transform.position.y;
        Etot = Ek + Ekr + Ep;

        //reset forces
        accumulatedTorques = 0;
        accumulatedForces = Vector2.zero;
    }


    public void SemiEuler(float dt)
    {
        //compute new velocities
        velocity += accumulatedForces / mass * dt; // v = v0 + at; och a = F/m;
        angularVelocity += accumulatedTorques / inertia * dt; // w = w0 + at; och a = T/I;

        //compute positions and rotations
        transform.position = transform.position + new Vector3(velocity.x * dt, velocity.y * dt);
        transform.Rotate(0, 0, angularVelocity * dt);
    }

    public void ApplyForce(Vector2 force)
    {
        accumulatedForces += force;
    }

    public void ApplyTorque(float torque)
    {
        accumulatedTorques += torque;
    }

    public void ApplyImpulse(Vector2 j)
    {
        //TODO
    }
}

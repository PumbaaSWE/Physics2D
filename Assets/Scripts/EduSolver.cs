using System.Collections.Generic;
using UnityEngine;

public class EduSolver : MonoBehaviour
{
    public int iterations = 1;
    [Range(0, .1f)] public float separationSlop = 0.03f;
    [Range(0,1)]public float erp = 0.5f;

    [Header("Advanced Settings")]
    public bool DoAdvanced;
    public bool DoFriction;
    public bool Rotation;

    [Header("Debugging Stuff")]
    public bool DynamicFriction;
    public bool StaticFriction;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Solve(List<EduCollision> collisions)
    {

        for (int i = 0; i < iterations; i++)
        {
            for (int c = 0; c < collisions.Count; c++)
            {
                if(!DoAdvanced) collisions[c].Solve(erp, separationSlop);
                else Solve(collisions[c], i==-1);
            }
        }
    }

    private void Solve(EduCollision collision, bool print)
    {
        EduRigidBody A = collision.A;
        EduRigidBody B = collision.B;

        // its possible for a rigid body to be null
        // if both we go back!
        if (!A && !B) return;

        float sf = Mathf.Sqrt((A ? A.staticFriction : .8f) * (B ? B.staticFriction : .8f));
        float df = Mathf.Sqrt((A ? A.dynamicFriction : .6f) * (B ? B.dynamicFriction : .6f));

        //if we iterate this could be done once...
        Vector2 cA = A ? A.transform.position : Vector2.zero;
        Vector2 cB = B ? B.transform.position : Vector2.zero;
        //Vector2 vA = A ? A.velocity : Vector2.zero;
        //Vector2 vB = B ? B.velocity : Vector2.zero;
        //float wA = A ? A.angularVelocity : 0;
        //float wB = B ? B.angularVelocity : 0;

        float eA = A ? A.restitution : 1; //perfect bouncyness if rb is missing
        float eB = B ? B.restitution : 1;

        float e = eA * eB; //whatever method

        float invMassA = A ? 1 / A.mass : 0;
        float invMassB = B ? 1 / B.mass : 0;

        float invMassSum = (invMassA + invMassB); //this == mA*mB/(mA+mB)

        float invInA = A ? 1 / A.inertia : 0;
        float invInB = B ? 1 / B.inertia : 0;
        Vector2 normal = collision.Normal;

        //this is the part we need to iterate

        float invContactCount = 1.0f / collision.ContactCount;

        for (int i = 0; i < collision.ContactCount; i++)
        {
            Vector2 contact = i == 0 ? collision.Position : collision.Position2;
            Vector2 ra = contact - cA;
            Vector2 rb = contact - cB;

            Vector2 vA = A ? A.velocity : Vector2.zero;
            Vector2 vB = B ? B.velocity : Vector2.zero;
            float wA = A ? A.angularVelocity : 0;
            float wB = B ? B.angularVelocity : 0;
            Vector2 relVelA = Mathy.Cross(wA, ra); //v = w x r    -> linear tangental velocity at contact point from rotation
            Vector2 relVelB = Mathy.Cross(wB, rb);

            //Vector2 relVel = vB + relVelB - vA - relVelA; // actual relative velocity at cp
            Vector2 rv = vB + relVelB - vA - relVelA;

            float contactVel = Vector2.Dot(rv, normal);
            if (contactVel > 0) continue; // we are separating -> check next point

            float crA = Mathy.Cross(ra, normal);
            float crB = Mathy.Cross(rb, normal);

            float rotationalStuff = crA * crA * invInA + crB * crB * invInB;

            if (print) Debug.Log("rotationalStuff = " + rotationalStuff + "\n" + " Ball = " + A);


            if (!Rotation) rotationalStuff = 0;

            float j = (1 + e) * contactVel;

            j /= invMassSum + rotationalStuff;
            j *= invContactCount;

            Vector2 impulse = j * normal;

            if (A)
            {
                A.velocity += impulse * invMassA;
                if (Rotation) A.angularVelocity += invInA * Mathy.Cross(ra, impulse);
            }
            if (B)
            {
                B.velocity -= impulse * invMassB;
                if (Rotation) B.angularVelocity -= invInB * Mathy.Cross(rb, impulse);
            }

            if(!DoFriction) continue;


            //recompute this as velocities changed (??)
            vA = A ? A.velocity : Vector2.zero;
            vB = B ? B.velocity : Vector2.zero;
            wA = A ? A.angularVelocity : 0;
            wB = B ? B.angularVelocity : 0;
            relVelA = Mathy.Cross(wA, ra);
            relVelB = Mathy.Cross(wB, rb);
            rv = vB + relVelB - vA - relVelA; // -(a+b) = - a - b


            contactVel = Vector2.Dot(rv, normal);

            Vector2 tangent = (rv - (contactVel * normal)).normalized;
            float jt = -Vector2.Dot(rv, tangent);

            Debug.DrawRay(contact, tangent, Color.magenta);

            jt /= invMassSum + rotationalStuff;
            jt *= invContactCount;

            if (Mathf.Abs(jt) < 0.001f) continue; //to tiny impulse to care

            //
            Vector2 tangentImpulse;
            if (Mathf.Abs(jt) < -j * sf)
            {
                tangentImpulse = tangent * -jt;
                //Debug.Log("static friction if(jt < j * sf) : jt = " + jt + " j = " + j);
                //Debug.DrawRay(contact + Vector2.up * 0.01f, tangentImpulse, Color.blue);
                if (StaticFriction) tangentImpulse = Vector2.zero;
            }
            else
            {
                tangentImpulse = df * -j * tangent; //Ff = µd * Fn in tangent direction
                //Debug.Log("Dynamic Friction else: jt = " + jt + " j = " + j);
                //Debug.DrawRay(contact+Vector2.up*0.01f, tangentImpulse, Color.red);
                if(DynamicFriction) tangentImpulse = Vector2.zero;
            }
            if (A)
            {
                A.velocity += tangentImpulse * invMassA;
                if (Rotation) A.angularVelocity += invInA * Mathy.Cross(ra, tangentImpulse);
            }
            if (B)
            {
                B.velocity -= tangentImpulse * invMassB;
                if (Rotation) B.angularVelocity -= invInB * Mathy.Cross(rb, tangentImpulse);
            }
        }


        //positional correction
        float p = erp * Mathf.Max(collision.Depth - separationSlop, 0.0f) / invMassSum;
        if (A) A.transform.position -= (p * invMassA * normal).ToVec3();
        if (B) B.transform.position += (p * invMassB * normal).ToVec3();
    }

    private void ApplyImpulse(EduRigidBody body, float invMass, float invIneria, Vector2 impulse, Vector2 r)
    {
        body.velocity += impulse * invMass;
        body.angularVelocity += invIneria * Mathy.Cross(r, impulse);
    }

}

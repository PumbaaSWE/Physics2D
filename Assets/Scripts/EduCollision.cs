using UnityEngine;

public sealed class EduCollision
{
    public Vector2 Position;
    public Vector2 Position2;
    public int ContactCount = 1;
    public Vector2 Normal;
    public float Depth;
    public EduRigidBody A;
    public EduRigidBody B;


    public void Solve(float erp, float slop)
    {
        // its possible for a rigid body to be null
        // if both we go back!
        if(!A && !B) return;

        Vector2 rv = (B ? B.velocity : Vector2.zero) - (A ? A.velocity : Vector2.zero);
        float contactVel = Vector2.Dot(rv, Normal);

        if (contactVel > 0) return; // we are separating

        float invMassA = A ? 1 / A.mass : 0;
        float invMassB = B ? 1 / B.mass : 0;

        float eA = A ? A.restitution : 1; //perfect bouncyness if rb is missing
        float eB = B ? B.restitution : 1;

        float e = eA * eB; //whatever method

        float J = (1 + e) * contactVel;

        float invInvMassSum = 1f / (invMassA + invMassB); //this == mA*mB/(mA+mB)

        J *= invInvMassSum; 

        //float value;
        //if (A && B)
        //{
        //    value = (A.mass * B.mass) / (A.mass + B.mass);
        //}
        //else
        //{
        //    value = A ? A.mass : B.mass; //lim_(x->inf) (a*x)/(a+x)  is = a...
        //}


        /*
         * ab/(a+b) = 1/(1/a+1/b)
         * 
         * 1/(1/a+1/b) => 1/(b/ab + a/ab) => 1/((a+b)/ab) => ab/(a+b)
         */

        Vector2 impulse = J * Normal;
        if (A) A.velocity += impulse * invMassA;
        if (B) B.velocity -= impulse * invMassB;

        //positional correction
        float p =  erp * Mathf.Max(Depth - slop, 0.0f) * invInvMassSum;
        if (A) A.transform.position -= ToVec3(p * invMassA * Normal);
        if (B) B.transform.position += ToVec3(p * invMassB * Normal);
    }

    public static Vector3 ToVec3(Vector2 v, float z = 0)
    {
        return new Vector3(v.x, v.y, z);
    }
}

using UnityEngine;

public class EduCollision
{
    public Vector2 Position;
    public Vector2 Normal;
    public float Depth;
    public EduRigidBody A;
    public EduRigidBody B;


    public void Solve(float erp)
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

        float value;
        if (A && B)
        {
            value = (A.mass * B.mass) / (A.mass + B.mass);
        }
        else
        {
            value = A ? A.mass : B.mass; //lim_(x->inf) (a*x)/(a+x)  is = a...
        }

        Vector2 impulse = J * value * Normal;
        if (A) A.velocity += impulse * invMassA;
        if (B) B.velocity -= impulse * invMassB;

        float p =  erp * value * Depth;
        if (A) A.transform.position -= ToVec3(p * invMassA * Normal);
        if (B) B.transform.position += ToVec3(p * invMassB * Normal);
    }

    public static Vector3 ToVec3(Vector2 v, float z = 0)
    {
        return new Vector3(v.x, v.y, z);
    }
}

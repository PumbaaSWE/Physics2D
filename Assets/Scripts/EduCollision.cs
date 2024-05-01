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

        float eA = A ? A.restitution : 1; //perfect bouncyness
        float eB = B ? B.restitution : 1;

        float e = eA * eB; //whatever method


        //float massSum = (B ? B.mass : 0) + (A ? A.mass : 0);
        //float massProd = (B ? B.mass : 1) * (A ? A.mass : 1);
        float J = (1 + e) * contactVel;
        //J /= invMassA + invMassB; //I dont get this... was found on lnks

        float value = 0;//need it for pos corr later
        if(A && B)
        {
            value = (A.mass * B.mass) / (A.mass + B.mass);
        }
        else
        {
            value = A ? A.mass : B.mass; //lim_(x->inf) (a*x)/(a+x)  is = a...
        }

        Vector2 impulse = J * value * Normal;

        Debug.DrawRay(Position, impulse, Color.cyan);

        if (A) A.velocity += impulse * invMassA;
        if (B) B.velocity -= impulse * invMassB;

        //to correct positions I need the transform of objects!!!
        float p =  erp * value * Depth;
        if (A) A.transform.position -= ToVec3(p * invMassA * Normal);
        if (B) B.transform.position += ToVec3(p * invMassB * Normal);

    }

    public static Vector3 ToVec3(Vector2 v, float z = 0)
    {
        return new Vector3(v.x, v.y, z);
    }
}

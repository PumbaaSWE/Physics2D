using System;
using UnityEngine;

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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
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

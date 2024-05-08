using TMPro;
using UnityEngine;

public class BallStopper : MonoBehaviour
{

    public EduRigidBody rb;
    public Vector3 offset;
    public TextMeshProUGUI text;
    float time = 0;
    float predictedTime = 0;
    EduForces forces;
    bool a = true;

    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
        forces = FindObjectOfType<EduForces>();
        //predictedTime = ComputeTime(rb);
    }

    // Update is called once per frame
    void Update()
    {
        if (a)
        {
            predictedTime = ComputeTime(rb);
            a = false;
        }
        if (rb)
        {
            time = Time.time;
            Camera cam = Camera.main;
            transform.position = cam.WorldToScreenPoint(rb.transform.position + offset);
            text.text = "AngularVel: " + rb.angularVelocity + "\nInertia: " + rb.inertia + "\nVelocity: " + rb.velocity + "\nMass: " + rb.mass + "\nPredicted Time: " + predictedTime.ToString("0.00") + "\nTime: " + time.ToString("0.00");
            if(rb.angularVelocity <= 0)
            {
                enabled = false;
                rb.enabled = false;
            }
        }
    }

    private float ComputeTime(EduRigidBody rb)
    {
        float t = 0;
        if (rb)
        {
            Debug.Log("angularVelocity = " + rb.angularVelocity + " brakeTorque = " + forces.brakeTorque + " inertia = " + rb.inertia);
            t = -rb.angularVelocity / (forces.brakeTorque / rb.inertia);
        }
        return t;
    }
}
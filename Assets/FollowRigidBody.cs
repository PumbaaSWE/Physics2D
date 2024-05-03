using TMPro;
using UnityEngine;

public class FollowRigidBody : MonoBehaviour
{
    
    public EduRigidBody rb;
    public Vector3 offset;
    public TextMeshProUGUI text;
    
    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        if (rb)
        {
            Camera cam = Camera.main;
            transform.position = cam.WorldToScreenPoint(rb.transform.position + offset);
            text.text = "AngularVel: " + rb.angularVelocity + "\nInertia: " + rb.inertia + "\nVelocity: " + rb.velocity + "\nMass: " + rb.mass;

        }
    }
}

using UnityEngine;

public class EduCircleCollider : MonoBehaviour
{

    public float radius = 1;
    [Tooltip("kg/m^3")]public float dencity = 1;

    public Vector2 Center => transform.position;
    
    // Start is called before the first frame update
    void Start()
    {
        if(TryGetComponent(out EduRigidBody rb))
        {
            ComputeMassInertia(rb);
        }
    }

    public void ComputeMassInertia(EduRigidBody rb)
    {
        float r = ScaledRadius();
        float m = r * r * Mathf.PI * dencity;
        float I = m * r * r / 2;
        rb.mass = m;
        rb.inertia = I;
    }

    public float ScaledRadius()
    {
        return radius * Mathf.Max(transform.lossyScale.x, transform.lossyScale.y);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, ScaledRadius());
    }
}

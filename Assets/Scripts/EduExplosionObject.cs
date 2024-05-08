using UnityEngine;

public class EduExplosionObject : MonoBehaviour
{

    public bool DestroyOnExplode;
    public float radius = 2;
    public float force = 5000;
    public float time = 1;
    private float timer = 0;
    public ParticleSystem particlePrefab;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if(time <= timer)
        {
            Explode();


            if (DestroyOnExplode)Destroy(gameObject);
            timer = 0;
        }
    }

    private void Explode()
    {
        EduRigidBody[] rbs = FindObjectsOfType<EduRigidBody>();
        Vector3 pos = transform.position;
        if (particlePrefab) Instantiate(particlePrefab, pos, Quaternion.identity);
        for (int i = 0; i < rbs.Length; i++)
        {
            rbs[i].ApplyExplosionForce(force, pos, radius);
        }
    }
}

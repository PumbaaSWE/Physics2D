using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EduSolver : MonoBehaviour
{
    public int iterations = 1;
    public float separationSlop = 0.03f;
    [Range(0,1)]public float erp = 0.5f;

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
                collisions[c].Solve(erp);
            }
        }
    }

}

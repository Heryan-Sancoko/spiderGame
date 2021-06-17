using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lerpToPosition : MonoBehaviour
{

    public Transform lerpToThis;
    public float lerpAmount = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, lerpToThis.position, lerpAmount);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamBehaviour : MonoBehaviour
{

    [SerializeField]
    private Transform mHeroTransform;
    [SerializeField]
    private float speed = 1.0f;
    [SerializeField]
    private Transform camAnchorXpivot;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.Rotate(new Vector3(0, Input.GetAxis("Mouse X"), 0) * Time.deltaTime * speed);

        camAnchorXpivot.Rotate(new Vector3(-Input.GetAxis("Mouse Y"), 0, 0) * Time.deltaTime * speed);

        transform.position = mHeroTransform.position;
    }
}

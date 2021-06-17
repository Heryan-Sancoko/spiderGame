using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroBehaviour : MonoBehaviour
{
    private Rigidbody rbody;
    [SerializeField]
    private Transform camAnchorY;
    [SerializeField]
    private Transform mCamHolder;
    [SerializeField]
    private float Y = 0;
    [SerializeField]
    private float X = 0;
    [SerializeField]
    private float mSpeed = 10;
    [SerializeField]
    private bool isGrounded = true;

    [SerializeField]
    private float sphereCheckRadius = 2;
    [SerializeField]
    private float sphereCheckDist = 2;
    [SerializeField]
    private LayerMask sphereCheckMask;

    private lerpToPosition mLerpScript;
    private float originalLerpAmount;
    [SerializeField]
    private Transform mTether;
    [SerializeField]
    private SwingBehaviour mSwingBehave;
    private Rigidbody swingRBody;
    private bool amTethering = false;
    public float currentSpeed = 0;
    public float topSpeed = 50;
    [SerializeField]
    private LineRenderer mline;


    // Start is called before the first frame update
    void Start()
    {
        rbody = GetComponent<Rigidbody>();
        mLerpScript = GetComponent<lerpToPosition>();
        swingRBody = mSwingBehave.gameObject.GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        mSwingBehave.topSpeed = topSpeed;
        originalLerpAmount = mLerpScript.lerpAmount;
        Physics.IgnoreLayerCollision(9, 10, true);
        Physics.IgnoreLayerCollision(11, 11, true);
    }

    // Update is called once per frame
    void Update()
    {
        X = Input.GetAxis("Horizontal");
        Y = Input.GetAxis("Vertical");

        GroundedCheck();

        if (Input.GetButtonDown("Fire1"))
        {
            RaycastHit hit;
            if (Physics.Raycast(mCamHolder.position, mCamHolder.forward, out hit, 999, sphereCheckMask))
            {
                //rbody.velocity = Vector3.ClampMagnitude(rbody.velocity, 50f);

                if (mSwingBehave.tetherNodeList.Count != 0)
                {
                    mSwingBehave.tetherNodeList.Clear();
                    mSwingBehave.firstNode = null;
                    mSwingBehave.lastNode = null;
                }

                amTethering = true;
                mTether.gameObject.SetActive(true);
                rbody.useGravity = false;
                mTether.position = hit.point + (hit.normal * 0.5f);

                mSwingBehave.AddTetherNode(hit.point + (hit.normal * 1), null, null, true, true);

                swingRBody.gameObject.SetActive(true);
                swingRBody.transform.position = transform.position;
                swingRBody.rotation = transform.rotation;
                swingRBody.velocity = rbody.velocity;
                rbody.velocity = Vector3.zero;
                mSwingBehave.mTetherDist = Vector3.Distance(transform.position, mTether.position);
                mLerpScript.enabled = true;
                mLerpScript.lerpAmount = 1;
                mline.enabled = true;

            }
        }

        if (Input.GetButtonUp("Fire1"))
        {
            if (mTether.gameObject.activeSelf)
            {
                rbody.velocity = swingRBody.velocity;
            }
            amTethering = false;
            rbody.useGravity = true;
            mTether.gameObject.SetActive(false);
            swingRBody.gameObject.SetActive(false);
            mLerpScript.enabled = false;

            mline.enabled = false;
        }

        if (!amTethering)
        {
            rbody.AddForce(camAnchorY.forward * (mSpeed * Y));
            rbody.AddForce(camAnchorY.right * (mSpeed * X));
            currentSpeed = rbody.velocity.magnitude;
        }
        else
        {
            swingRBody.AddForce(camAnchorY.forward * (mSpeed * Y));
            swingRBody.AddForce(camAnchorY.right * (mSpeed * X));
            currentSpeed = swingRBody.velocity.magnitude;
            mline.SetPosition(0, transform.position);
            mline.SetPosition(1, mTether.position);
            mLerpScript.lerpAmount = Mathf.Lerp(mLerpScript.lerpAmount, originalLerpAmount, 0.01f);


        }
        
        
    }

    private void GroundedCheck()
    {
        RaycastHit hit;
        if (Physics.SphereCast(transform.position, sphereCheckRadius, Vector3.down, out hit, sphereCheckDist, sphereCheckMask))
        {
            isGrounded = true;
            if (Input.GetKeyDown(KeyCode.Space))
            {
                rbody.velocity = rbody.velocity + (Vector3.up * 30);
            }

        }
        else
        {
            isGrounded = false;
        }
    }

    //private void OnDrawGizmos()
    //{
    //    RaycastHit hit;
    //    if (Physics.SphereCast(transform.position, sphereCheckRadius, Vector3.down, out hit, sphereCheckDist, sphereCheckMask))
    //    {
    //        Gizmos.DrawSphere(hit.point, sphereCheckRadius);
    //    }
    //    else
    //    {
    //        Gizmos.DrawSphere(transform.position + (Vector3.down * sphereCheckDist), sphereCheckRadius);
    //    }
    //}


}

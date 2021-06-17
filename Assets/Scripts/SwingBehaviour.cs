using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwingBehaviour : MonoBehaviour
{
    [SerializeField]
    private Transform mTetherPoint;
    [SerializeField]
    private Transform mPenultiTetherPoint;
    [SerializeField]
    private GameObject mPrefabSphere;
    public float mTetherDist;
    [SerializeField]
    private float curDist;
    private Rigidbody rbody;
    public float topSpeed = 50;
    public LayerMask mlayerMask;
    public LayerMask mUnwindMask;
    public GameObject mFirstHook;
    public TetherNode lastNode;
    public TetherNode firstNode;
    
    
    [System.Serializable]
    public class TetherNode
    {
        public Vector3 tPos;
        public TetherNode nextNode = null;
        public TetherNode prevNode = null;
        public bool isLastNode = false;
        public bool isFirstNode = false;
        public GameObject mSphere;
    }
    
    public List<TetherNode> tetherNodeList;

    public bool tetherMeBaby = true;

    // Start is called before the first frame update
    void Start()
    {
        rbody = GetComponent<Rigidbody>();
        mTetherDist = Vector3.Distance(transform.position, mTetherPoint.position);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        tetherMechanics();
    }

    private void LateUpdate()
    {
        //tetherMechanics();
        curDist = Vector3.Distance(transform.position, mTetherPoint.position);
    }

    private void tetherMechanics()
    {
        
        RaycastHit hit;
        if (Physics.Raycast(transform.position, (mTetherPoint.position - transform.position).normalized, out hit, 99999, mlayerMask))
        {
            if (hit.transform.gameObject != mTetherPoint.gameObject)
            {
                AddTetherNode(hit.point, null, lastNode, true, false);
            }
        }

        if (tetherNodeList.Count > 1)
        {
            RaycastHit hit2;
            if (Physics.Raycast(transform.position, (mPenultiTetherPoint.position - transform.position).normalized, out hit2, 99999, mlayerMask))
            {
                if (firstNode != lastNode)
                {
                    if (hit2.transform.gameObject == mPenultiTetherPoint.gameObject)
                        RemoveTetherNodeAtEnd();
                }
            }
        }

        if (tetherMeBaby)
        {
            //lookat tether point
            transform.LookAt(mTetherPoint);

            ////if yVel < 0, y vel*2
            //if (rbody.velocity.y < 0)
            //{
            Vector3 newVel = rbody.velocity;
            newVel *= 1.02f;
            rbody.velocity = newVel;
            //}

            //take velocity, convert it to local
            //space.
            Vector3 locVel = transform.InverseTransformDirection(rbody.velocity);

            //Take Local zVel and save it as new
            //variable "oldZ"
            float oldZ = locVel.z;

            float originalMagnitude = locVel.magnitude;

            //Make local zVel 0
            locVel.z = 0;

            //local velocity *= (1 + (oldZ*0.5f))
            locVel = locVel.normalized * (originalMagnitude * 0.985f);

            locVel = Vector3.ClampMagnitude(locVel, topSpeed);

            //Convert back to world velocity
            rbody.velocity = transform.TransformDirection(locVel);
        }
    }

    public void AddTetherNode(Vector3 tPos, TetherNode nextNode, TetherNode prevNode, bool isLastNode, bool isFirstNode)
    {
        TetherNode newTNode = new TetherNode();
        newTNode.tPos = tPos;
        newTNode.nextNode = nextNode;
        newTNode.prevNode = lastNode;
        newTNode.isLastNode = isLastNode;
        newTNode.isFirstNode = isFirstNode;
        tetherNodeList.Add(newTNode);

        if (isFirstNode)
        {
            firstNode = newTNode;
            mPenultiTetherPoint.position = tPos;
        }
        else
        {
            lastNode.nextNode = newTNode;
            mPenultiTetherPoint.position = lastNode.tPos;
        }

        lastNode = newTNode;

        mTetherPoint.position = tPos; // move tether point to next node pos

        //newTNode.mSphere = Instantiate<GameObject>(mPrefabSphere, tPos, Quaternion.identity);
        //LineRenderer mline = mPrefabSphere.GetComponent<LineRenderer>();
        //mline.SetPosition(0, newTNode.mSphere.transform.position);
        //if (prevNode != null)
        //{
        //    mline.SetPosition(1, prevNode.mSphere.transform.position);
        //    Instantiate<GameObject>(mPrefabSphere, prevNode.mSphere.transform.position, Quaternion.identity);
        //}
        Debug.Log("tethernode added");
    }

    public void RemoveTetherNodeAtEnd()
    {
        //Destroy(lastNode.mSphere);
        //Debug.Log("sphere destroyed");
        TetherNode newLastTNode = lastNode.prevNode;
        newLastTNode.nextNode = null;
        newLastTNode.isLastNode = true;
        if (newLastTNode.prevNode != null)
        {
            mPenultiTetherPoint.transform.position = newLastTNode.prevNode.tPos;
        }
        else
        {
            Debug.Log("PrevNode is NULL");
        }
        

        tetherNodeList.Remove(tetherNodeList[tetherNodeList.Count - 1]);

        lastNode = newLastTNode;

        mTetherPoint.position = lastNode.tPos; // move tether point to last node pos

        

        Debug.Log("tethernode removed");
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, (mPenultiTetherPoint.position - transform.position).normalized);
    }
}

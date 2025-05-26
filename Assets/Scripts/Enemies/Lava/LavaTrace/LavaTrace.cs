using UnityEngine;

public class LavaTrace : MonoBehaviour
{

    public GameObject objectToInstantiate;
    public Vector3 hingeAnchor = new(0, 0.5f, 0);
    public Vector3 hingeAxis = Vector3.up;
    public bool useConnectedAnchor = true;
    public Vector3 connectedAnchor = Vector3.zero;
    private GameObject previousObj = null;





    private void Start()
    {
        InstantiateObjects();
    }

    private void InstantiateObjects()
    {
        for (int i = 0; i < 5; i++)
        {


            GameObject newObj = Instantiate(objectToInstantiate, transform.position + new Vector3(5, 0, 5), Quaternion.identity);


            HingeJoint hingeJoint = newObj.AddComponent<HingeJoint>();

            hingeJoint.anchor = hingeAnchor;
            hingeJoint.axis = hingeAxis;

            if (previousObj != null)
            {
                hingeJoint.connectedBody = previousObj.GetComponent<Rigidbody>();
            }
            previousObj = newObj;

            if (useConnectedAnchor)
            {
                hingeJoint.connectedAnchor = connectedAnchor;
            }
            hingeJoint.useLimits = true;
            hingeJoint.limits = new JointLimits()
            {
                min = -60,
                max = 60,
                bounceMinVelocity = 0,
                bounciness = 0
            };

            newObj.name = "lavaFluid" + i;
        }


    }

}

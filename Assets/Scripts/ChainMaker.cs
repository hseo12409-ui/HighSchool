using UnityEngine;

//[ExecuteInEditMode]
public class ChainMaker : MonoBehaviour
{
    [SerializeField] GameObject[] chainPreFab;
    [SerializeField] GameObject endPreFab;

    [SerializeField] int chaincount;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [ContextMenu("Chain")]
    public void Chain()
    {
        //Debug.Log("Generate");
        Vector3 chainPos = transform.position + Vector3.up * -0.1f;
        HingeJoint2D joint = null;
        for (int i = 0; i < chaincount; i++)
        {
            if (i !< chaincount - 1)
            {
                joint = Instantiate(i % 2 == 0 ? chainPreFab[0] : chainPreFab[1], chainPos, Quaternion.identity, transform.parent).GetComponent<HingeJoint2D>();
            }else
            {
                joint = Instantiate(endPreFab, chainPos, Quaternion.identity, transform.parent).GetComponent<HingeJoint2D>();
            }

            joint.connectedBody = transform.parent.GetChild(i).GetComponent<Rigidbody2D>();
            chainPos += Vector3.up * -0.2f;
        }
    }
}

using UnityEngine;

public enum StuffType
{
    Hebi,
    Light
}

public class Stuff : MonoBehaviour
{
    public string stuffName;

    public StuffType throwType;

    [SerializeField] GameObject Particle;

    public Transform P;

    public float size;
    [Tooltip("가벼우면 숫자 높고 무거우면 숫자 낮게(날아가는 속도 조절땜에 쩔수 없음ㅇㅇ)")] public float weight;
    [SerializeField] float fallStart;
    public float fallDis;

    public bool isthrow;
    public bool iscontrol;
    public bool isfall;
    public bool vib = true;

    [SerializeField] LayerMask g;
    [SerializeField] LayerMask e;

    Rigidbody2D Srb;
    BoxCollider2D Sbc;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        Srb = GetComponent<Rigidbody2D>();
        Sbc = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Srb.linearVelocityY < 0 && !isfall)
        {
            isfall = true;
            fallStart = transform.position.y;
        }
    }

    public void Des(Vector3 point)
    {
        //float d = 1 / Vector3.Distance(transform.position, P.position) * 6f;                      //거리에 비례해서 줄어듬        
        //DualSense.instance.Hapticvoid(1 * weight * d, 1 * d, 0.9f, 3, 3, true, 1);

        if (throwType == StuffType.Hebi)
        {
            RaycastHit2D[] hitenemy = Physics2D.BoxCastAll(transform.position, new Vector3(6f * size, size, 0), 0, Vector2.zero, 0, e);
            for (int i = 0; i < hitenemy.Length; ++i)
            {
                Rigidbody2D Erb = hitenemy[i].collider.GetComponent<Rigidbody2D>();

                float d = 1 / Vector3.Distance(transform.position, hitenemy[i].collider.transform.position) * 8f;                      //거리에 비례해서 줄어듬        
                if (transform.position.x < hitenemy[i].collider.transform.position.x)
                {
                    Erb.linearVelocityX = d + 6;

                    //Erb.linearVelocityX = 10;                                                 //거리 상관없이 독같이
                }
                else
                {
                    Erb.linearVelocityX = -d - 6;

                    //Erb.linearVelocityX = -10;                                                //거리 상관없이 똑같이
                }
            }
        }

        //DualSense.instance.Hapticvoid(1 * weight, 1, 0.9f, 3, 3, true, 1);                          //항상 똑같음
        Vector2 fxdir = (transform.position - new Vector3(point.x, point.y, 0)).normalized;
        float dir = Mathf.Atan2(fxdir.y, fxdir.x) * Mathf.Rad2Deg;
        Instantiate(Particle, point, Quaternion.Euler(0, 0, dir), null);
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground") || collision.collider.CompareTag("Wall") || collision.collider.CompareTag("Stuff"))
        {
            isfall = false;
            fallDis = fallStart - transform.position.y;

            if (collision.collider.CompareTag("Stuff"))
            {
                Stuff st = collision.transform.GetComponent<Stuff>();
                if (fallDis > 8)
                {
                    st.Des(collision.contacts[0].point);
                }                    

                if (fallDis > 6)
                {
                    if (vib) DualSense.instance.Hapticvoid(1 * weight * fallDis / 3, 1 * fallDis / 3, 0.7f, 3, 3);                    
                }
                else
                {
                    if (vib) DualSense.instance.Hapticvoid(1 * weight * fallDis / 7, 1 * fallDis / 7, 0.07f * fallDis, 3, 3);                          //항상 똑같음                    
                }
            }
            else
            {
                if (fallDis > 6)
                {
                    if (vib) DualSense.instance.Hapticvoid(1 * weight * fallDis / 7, 1 * fallDis / 7, 0.7f, 3, 3);                    
                }
                else
                {
                    if (vib) DualSense.instance.Hapticvoid(1 * weight * fallDis / 15, 1 * fallDis / 15, 0.07f * fallDis, 3, 3);                          //항상 똑같음                    
                }
            }
            if (fallDis > 8)
            {
                Des(collision.contacts[0].point);
            }

            fallStart = 0;
            fallDis = 0;
        }        

        if (collision.collider.CompareTag("Ground"))
        {
            RaycastHit2D groundhit = Physics2D.BoxCast(transform.position - new Vector3(0, Sbc.size.y / 2, 0), new Vector2(Sbc.size.x, 1), 0, Vector2.zero, 0, g);
            if (groundhit && !iscontrol)
            {
                //Debug.Log("Kinematic");
                Srb.bodyType = RigidbodyType2D.Kinematic;
                Srb.linearVelocity = Vector2.zero;
                Srb.angularVelocity = 0;
                //transform.parent = null;
            }
        }

        if (collision.collider.CompareTag("Player"))
        {
            isfall = false;
            fallDis = fallStart - transform.position.y;            
            fallStart = 0;
            fallDis = 0;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            RaycastHit2D groundhit = Physics2D.BoxCast(transform.position - new Vector3(0, Sbc.size.y / 2, 0), new Vector2(Sbc.size.x, 1), 0, Vector2.zero, 0, g);
            if (!groundhit && !iscontrol)
            {
                //Debug.Log("Dynamic");
                Srb.bodyType = RigidbodyType2D.Dynamic;
            }
        }

        //if (collision.collider.CompareTag("Ground"))
        //{
        //    Debug.Log("Dynamic");
        //    Srb.bodyType = RigidbodyType2D.Dynamic;
        //}
    }
}

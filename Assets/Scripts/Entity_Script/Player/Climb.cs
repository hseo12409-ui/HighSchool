using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Climb : MonoBehaviour
{
    [SerializeField] Transform climbhelpob;

    [SerializeField] GameObject perfectclimbeffect;
    [SerializeField] GameObject perfectclimbtiming;

    [SerializeField, Tooltip("마지막으로 누른 등반 키(-1 : L2, 1 : R2)")] int lastclimbkey;

    [SerializeField, Tooltip("한변 벽탈때 움직일 거리")] float climbdis;              //한변 벽탈때 움직일 거리
    [SerializeField, Tooltip("한변 벽탈때 움직일 거리")] float climbX;
    [SerializeField, Tooltip("한변 벽탈때 움직일 거리")] float climbperfecttime;

    [SerializeField, Tooltip("aim")] Vector3 nextclimbpos;
    [SerializeField, Tooltip("aim")] Vector3 climbpos;

    [SerializeField, Tooltip("이동 가능")] bool canclimbup = true;
    [Tooltip("이동 가능")] public bool canclimb;    
    [SerializeField, Tooltip("이동 가능")] bool isclimbkeyup = true;    
    [SerializeField, Tooltip("이동 가능")] bool isnextclimbend;

    [SerializeField] LayerMask groundCopy;
    [SerializeField] LayerMask climbGroundCopy;
    [SerializeField] LayerMask wallCopy;

    Player p;
    PlayerAnimation Panim;
    PlayerMoveStuff Pstuff;
    BoxCollider2D Pbc;
    ParticleSystem perfectclimbfx;

    Coroutine ClimbUpvari;
    public Coroutine ClimbEndvari;
    Coroutine ClimbHelpervari;
    Coroutine ClimbSlipvari;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        p = GetComponent<Player>();
        Panim = GetComponent<PlayerAnimation>();
        Pstuff = GetComponent<PlayerMoveStuff>();
        Pbc = GetComponent<BoxCollider2D>();
        perfectclimbfx = perfectclimbeffect.GetComponent<ParticleSystem>();

        groundCopy = p.ground;
        climbGroundCopy = p.climbground;
        wallCopy = p.wall;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (ClimbEndvari != null && !isnextclimbend)
        {
            transform.position = Vector3.Lerp(transform.position, climbpos, 0.6f);
        }else if (((p.currentState == PlayerState.CLIMB || isnextclimbend) && p.currentState != PlayerState.CLIMBSLIP))
        {
            transform.position = Vector3.Lerp(transform.position, climbpos, 0.15f);
        }
    }

    private void LateUpdate()
    {
        //Debug.Log($"lateUpdate : {transform.position}");
    }

    public void LIteClimb(Vector2 dir)
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position + new Vector3(Pbc.offset.x, Pbc.offset.y + Pbc.size.y / 2 - 0.12f, 0), dir, 0.1f + Pbc.size.x / 2, climbGroundCopy);
        RaycastHit2D hit1 = Physics2D.Raycast(transform.position + new Vector3(Pbc.offset.x, Pbc.offset.y + Pbc.size.y / 2 + 0.715f, 0), dir, 0.1f + Pbc.size.x / 2, climbGroundCopy);                //0.715f는 플레이어가 올라갈 거리

        if (hit && !hit1 && ClimbEndvari == null && !Physics2D.BoxCast(transform.position - new Vector3(0, 0.7f, 0), new Vector3(0.6f, 0.1f, 0), 0, Vector2.zero, 0, climbGroundCopy))
        {
            Debug.Log($"hit.point : {hit.point}");
            Debug.Log($"hit1.point : {hit1.point}");            
            Vector2Int tempPos = Vector2Int.FloorToInt(hit.point);
            Debug.Log($"tempPos : {tempPos}");
            climbpos = new Vector3(hit.point.x + (Pbc.size.x / 2) * -p.lookingdir, tempPos.y + 0.5f, 0);            
            ClimbEndvari = StartCoroutine(ClimbEnd());
        }

        RaycastHit2D climbray = Physics2D.Raycast(transform.position + Vector3.up * 0.5f, dir, 0.6f, wallCopy);                    //벽타기 판정.안붙어잇을때만.최초 1회
        if (p.currentState != PlayerState.CLIMB && canclimb && climbray && !Pstuff.isstuffcharge)
        {                                                                   //최초 1회만
            canclimbup = true;            
            isnextclimbend = false;
            perfectclimbtiming.SetActive(true);
            climbpos = transform.position;
            climbX = transform.position.x;
            lastclimbkey = -1;
            p.ChangePlayerState(PlayerState.CLIMB);                             //Enter
            DualSense.instance.TriggerFeedback(true, true, 0, 1);
            if (ClimbSlipvari != null)
            {
                StopCoroutine(ClimbSlipvari);
                ClimbSlipvari = null;
            }
            ClimbSlipvari = StartCoroutine(ClimbSlip());
            Panim.SetTrigger("ClimbStart");
            Panim.SetBool("isClimb", true);
            //UIManager.Instance.SetClimbUI(true, false, true);
            //플레이어 climb애니메이션 설정
        }
    }

    public void ClimbStart(Vector2 dir)
    {
        //RaycastHit2D climbray = Physics2D.Raycast(transform.position + Vector3.up * 0.5f, dir, 0.6f, wallCopy);                    //벽타기 판정.안붙어잇을때만.최초 1회
        //if (!isclimb && canclimb && climbray && !Pstuff.isstuffcharge)
        //{                                                                   //최초 1회만
        //    canclimbup = true;
        //    isclimb = true;
        //    isnextclimbend = false;
        //    perfectclimbtiming.SetActive(true);
        //    climbpos = transform.position;
        //    climbX = transform.position.x;
        //    lastclimbkey = -1;
        //    p.ChangePlayerState(PlayerState.CLIMB);                             //Enter
        //    DualSense.instance.TriggerFeedback(true, true, 0, 1);
        //    if (ClimbSlipvari != null)
        //    {
        //        StopCoroutine(ClimbSlipvari);
        //        ClimbSlipvari = null;
        //    }
        //    ClimbSlipvari = StartCoroutine(ClimbSlip());
        //    //UIManager.Instance.SetClimbUI(true, false, true);
        //    //플레이어 climb애니메이션 설정
        //}
    }

    public void ClimbUp(int pressClimbKey)
    {
        //Debug.Log(lastclimbkey);
        //Debug.Log(pressClimbKey);
        if (lastclimbkey == -pressClimbKey && canclimbup && canclimb && isclimbkeyup && ClimbEndvari == null)
        {
            Panim.SetTrigger("Climb");
            //Debug.Log("climbup");
            RaycastHit2D climbendcheck = Physics2D.BoxCast(climbpos + Vector3.up * climbdis, new Vector2(0.8f, 0.7f), 1, Vector2.zero, 0, climbGroundCopy);
            
            if (climbperfecttime < 0.4f)
            {
                perfectclimbfx.Play();
                if (climbdis < 1.8f)
                {
                    climbdis += 0.15f;
                }
                DualSense.instance.Hapticvoid(1, 1, 0.07f, 2, 2);
                DualSense.instance.SetLightBarvoid("Green", 0, true);
            }
            else
            {
                DualSense.instance.Hapticvoid(0.2f, 0.5f, 0.03f, 2, 2);
                DualSense.instance.SetLightBarvoid("Blue", 0, true);
            }
            p.ChangePlayerState(PlayerState.CLIMB);
            if (ClimbSlipvari != null)
            {
                StopCoroutine(ClimbSlipvari);
                ClimbSlipvari = null;
            }
            ClimbSlipvari = StartCoroutine(ClimbSlip());            
            isclimbkeyup = false;
            if (climbendcheck)
            {                
                //climbpos = new Vector3(climbX, Mathf.RoundToInt(climbendcheck.transform.position.y) + 2.5f, 0);                

                RaycastHit2D hit = Physics2D.Raycast(new Vector3(transform.position.x, climbendcheck.point.y, 0), Vector2.right * p.lookingdir, 0.1f + Pbc.size.x / 2, climbGroundCopy);
                if (!hit) hit = Physics2D.Raycast(new Vector3(transform.position.x, climbendcheck.point.y - 0.1f, 0), Vector2.right * p.lookingdir, 0.1f + Pbc.size.x / 2, climbGroundCopy);
                Debug.Log($"vector : {new Vector3(transform.position.x, climbendcheck.point.y, 0)}");
                //RaycastHit2D hit1 = Physics2D.Raycast(transform.position + new Vector3(Pbc.offset.x, Pbc.offset.y + Pbc.size.y / 2 + 0.715f + climbdis, 0), Vector2.right * p.lookingdir, 0.1f + Pbc.size.x / 2, climbGroundCopy);                //0.715f는 플레이어가 올라갈 거리

                if (hit && ClimbEndvari == null && !Physics2D.BoxCast(transform.position - new Vector3(0, 0.7f, 0), new Vector3(0.6f, 0.1f, 0), 0, Vector2.zero, 0, climbGroundCopy))
                {
                    Debug.Log($"hit.point : {hit.point}");
                    //Debug.Log($"hit1.point : {hit1.point}");
                    Vector2Int tempPos = Vector2Int.FloorToInt(hit.point);
                    Debug.Log($"tempPos : {tempPos}");
                    isnextclimbend = true;
                    climbpos = new Vector3(hit.point.x + (Pbc.size.x / 2) * -p.lookingdir, tempPos.y + 0.5f, 0);
                    ClimbEndvari = StartCoroutine(ClimbEnd());
                }
            }
            else if (!isnextclimbend)
            {
                climbpos = new Vector3(climbX, transform.position.y + climbdis, 0);
            }
        }
    }

    public void ClimbDown(int pressClimbKey)
    {
        if (ClimbEndvari == null)
        {
            isclimbkeyup = true;
            lastclimbkey = pressClimbKey;
            canclimbup = false;
            ClimbUpvari = StartCoroutine(SetCanClimbUp(0.3f));
            climbperfecttime = 0;
            if (ClimbHelpervari != null)
            {
                StopCoroutine(ClimbHelpervari);
                ClimbHelpervari = null;
            }
            ClimbHelpervari = StartCoroutine(ClimbHelper());
        }        
    }

    void SetCanClimb()
    {
        canclimbup = true;
    }

    IEnumerator ClimbHelper()
    {
        climbhelpob.localScale = new Vector3(1, 1, 1);

        float sizeDownDur = 0.8f;
        float accumulatedTime = 0;

        while (accumulatedTime < sizeDownDur)
        {
            accumulatedTime += Time.deltaTime;
            climbperfecttime += Time.deltaTime;            
            float sizePerTime = accumulatedTime / sizeDownDur;
            //Debug.Log(accumulatedTime/ sizeDownDur);
            climbhelpob.localScale = Vector3.Lerp(new Vector3(1, 1, 1), Vector3.zero, sizePerTime);

            yield return null;
        }

        climbhelpob.localScale = Vector3.zero;
    }

    public IEnumerator ClimbSlip(bool instant = false)
    {
        if (!instant)
        {
            yield return new WaitForSeconds(1);
        }
        //Debug.Log("Slip");        
        p.ChangePlayerState(PlayerState.CLIMBSLIP);
        DualSense.instance.Hapticvoid(0.02f, 0.1f, 100, 1, 1);
        DualSense.instance.SetLightBarvoid("Red", 0, false);
        ClimbSlipvari = null;
    }

    IEnumerator SetCanClimbUp(float wait)
    {
        yield return new WaitForSeconds(wait);
        canclimbup = true;
        ClimbUpvari = null;
    }

    public IEnumerator ClimbEnd(float stoptime = 2f)
    {
        if (ClimbEndvari != null) yield break;
        if (stoptime == 2f)
        {
            p.ChangePlayerState(PlayerState.CLIMB);                                 //Enter
            Panim.SetTrigger("liteClimbTri");
            Panim.SetBool("liteClimb", true);
        }        
        perfectclimbtiming.SetActive(false);
        climbdis = 1;        
        canclimbup = false;        
        DualSense.instance.StopVib();
        DualSense.instance.ResetTrigger(true, true);
        //UIManager.Instance.SetClimbUI(false, true, true);

        if (ClimbUpvari != null)
        {
            StopCoroutine(ClimbUpvari);
        }

        if (ClimbSlipvari != null)
        {
            StopCoroutine(ClimbSlipvari);
            ClimbSlipvari = null;
        }
        //플레이어 climb 종료 애니메이션 설정        
        yield return new WaitForSeconds(stoptime - 0.8f);                                      //올라가는 애님 재생시간 기본 1.5정도?
        StartCoroutine(CamShake.instance.Shake(0.8f, 0.3f, 0.05f, 3, 1));
        DualSense.instance.Hapticvoid(0.2f, 0.2f, 0.1f, 3, 3);
        yield return new WaitForSeconds(0.8f);        
        //p.isground = true;  
        isnextclimbend = false;        
        RaycastHit2D climbendpos = Physics2D.Raycast(transform.position + new Vector3(1 * p.lookingdir, 1, 0), Vector2.down, 3, groundCopy);
        //Debug.DrawRay(transform.position + new Vector3(1 * p.lookingdir, 1, 0), Vector2.down, Color.blue, 10);
        if (stoptime == 2f)
        {
            Debug.Log($"point : {climbendpos.point}");
            transform.position = climbendpos.point + new Vector2(0, 0.715f);
        }
        Panim.SetBool("liteClimb", false);
        Panim.SetBool("isClimb", false);
        ClimbEndvari = null;
        p.ChangePlayerState(PlayerState.NORMAL);                                                //climbExit
    }
}

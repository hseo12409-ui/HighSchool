using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMoveStuff : MonoBehaviour
{
    [SerializeField] public GameObject aimstuff;
    [SerializeField] public List<GameObject> detectStuffList;
    [SerializeField] public GameObject chargestuff;

    [SerializeField, Tooltip("aim")] Vector2 stuffpos;
    [SerializeField, Tooltip("")] Vector2 lastCursorPosition;

    [SerializeField, Tooltip("aim")] float stuffRot;
    [SerializeField] float moveStuffRange = 5;                                          //게임 시작하면 자동으로 제곱
    private float squareMoveStuffRange;

    [SerializeField, Tooltip("이동 가능")] public bool canmovestuff;    
    [SerializeField, Tooltip("조준 변경중")] public bool isstuffcharge;    
    [SerializeField, Tooltip("물건 각도 초기화")] public bool isSetStuffRo;
    [SerializeField, Tooltip("투척함")] bool isthrow;

    Player p;
    PlayerAnimation Panim;
    Climb Pclimb;
    PlayerObserve Pobserve;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        Panim = GetComponent<PlayerAnimation>();
        p = GetComponent<Player>();
        Pclimb = GetComponent<Climb>();
        Pobserve = GetComponent<PlayerObserve>();

        squareMoveStuffRange = moveStuffRange * moveStuffRange;
    }

    // Update is called once per frame
    void Update()
    {
        if (p.isaim)
        {
            if (PlayerInputManager.instance.controlType == ControlTypeEnum.GAMEPAD)
            {
                if (aimstuff == null)
                {
                    p.ChangePlayerState(PlayerState.NORMAL);
                    p.crosshair.gameObject.SetActive(false);
                    aimstuff = null;
                    DualSense.instance.ResetTrigger(false, true);
                }

                if (Mathf.Abs(p.aimdir.x) + Mathf.Abs(p.aimdir.y) > 0.8f)
                {
                    if (!Pobserve.ischangeaim)
                    {
                        Pobserve.ischangeaim = true;
                        Stuff stuffst = aimstuff.transform.GetComponent<Stuff>();
                        Debug.Log("RayCast");
                        RaycastHit2D[] hits = Physics2D.BoxCastAll(aimstuff.transform.position, new Vector2(1, 2), Mathf.Atan2(Mathf.Abs(p.aimvector.y), Mathf.Abs(p.aimvector.x)) * Mathf.Rad2Deg, p.aimvector, 20, p.stuff);
                        if (hits != null)
                        {
                            GameObject aimObserveTarget = null;
                            float shortestDistance = float.MaxValue;
                            foreach (var hit in hits)
                            {
                                float disSqr = comfor.GetSqr(transform.position, hit.transform.position);
                                if (shortestDistance != 0 && shortestDistance > disSqr && hit.collider.gameObject != aimstuff && detectStuffList.Contains(hit.collider.gameObject))
                                {
                                    aimObserveTarget = hit.collider.gameObject;
                                    shortestDistance = disSqr;
                                    Debug.Log($"name : {aimObserveTarget}, distance : {Mathf.Sqrt(disSqr)}");
                                }
                            }

                            if (aimObserveTarget != null)
                            {
                                Stuff lastObserve = aimstuff.GetComponent<Stuff>();
                                aimstuff = aimObserveTarget;
                                p.crosshair.position = aimstuff.transform.position;
                            }
                        }
                    }
                }
                else
                {
                    Pobserve.ischangeaim = false;
                }
            }else
            {
                Vector3 mousePosWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Collider2D[] hit = Physics2D.OverlapCircleAll(mousePosWorld, 0.5f, p.stuff);
                //Debug.Log("KM FIND" + mousePosWorld);

                foreach (var hits in hit)
                {
                    if (hits.gameObject != aimstuff && detectStuffList.Contains(hits.gameObject))
                    {
                        Stuff lastObserve = aimstuff.GetComponent<Stuff>();
                        aimstuff = hits.gameObject;
                        p.crosshair.position = aimstuff.transform.position;
                    }
                }
            }
        }
    }

    void FixedUpdate()
    {
        if (isstuffcharge && chargestuff != null)
        {
            if (PlayerInputManager.instance.controlType == ControlTypeEnum.GAMEPAD)
            {
                Debug.Log($"aimdir ; {p.aimdir}");
                stuffpos = chargestuff.transform.position + p.aimdir / 4;
            }else
            {
                //Vector3 mouseDir = (p.mousePosition - p.lastMousePosition).normalized / 10;
                //stuffpos = chargestuff.transform.position + mouseDir;

                stuffpos = chargestuff.transform.position + p.mouseAimdir / 4;
            }

            float distance = Vector3.Distance(transform.position, stuffpos);
            Rigidbody2D Srb = chargestuff.GetComponent<Rigidbody2D>();

            chargestuff.transform.rotation = Quaternion.Lerp(chargestuff.transform.rotation, Quaternion.Euler(0, 0, stuffRot), 0.15f);
            //chargestuff.transform.rotation = Quaternion.Euler(0, 0, Mathf.Lerp(chargestuff.transform.rotation.z, stuffRot, 0.1f));

            if (distance < moveStuffRange)
            {
                //chargestuff.transform.position = Vector3.MoveTowards(chargestuff.transform.position, stuffpos, 1);
                Srb.MovePosition(stuffpos);
            }
            else
            {
                Vector3 stuffdir = comfor.GetVector(transform.position, chargestuff.transform.position);
                Srb.MovePosition(transform.position + stuffdir * moveStuffRange);
                Debug.Log("max");
                //chargestuff.transform.position = Vector3.MoveTowards(chargestuff.transform.position, transform.position + stuffdir * 12, 1);
            }
        }
    }

    public void StuffTurn()
    {
        if (p.currentState == PlayerState.MOVESTUFF)
        {
            if (!isSetStuffRo && chargestuff.transform.rotation.z > 3f)
            {
                isSetStuffRo = true;
                stuffRot = 0;
                //Debug.Log("Set Zero");
            }
            else
            {
                stuffRot += 45;
                if (stuffRot == 360)
                {
                    stuffRot = 0;
                }
                //Debug.Log("Plus 45");
            }
        }
    }

    public void DetectStuff()
    {        
        p.SetPlayerRBType(RigidbodyType2D.Static);
        if (aimstuff != null && comfor.GetSqr(transform.position, aimstuff.transform.position) < squareMoveStuffRange)
        {
            RaycastHit2D[] hit = Physics2D.CircleCastAll(transform.position, moveStuffRange, Vector2.zero, 0, p.stuff);
            detectStuffList.Clear();
            foreach (var ga in hit)
            {
                if (comfor.GetSqr(transform.position, ga.transform.position) < squareMoveStuffRange) detectStuffList.Add(ga.transform.gameObject);
            }

            p.crosshair.position = aimstuff.transform.position;
            Stuff st = aimstuff.GetComponent<Stuff>();
            p.crosshair.gameObject.SetActive(true);
            DualSense.instance.TriggerFeedback(false, true, 0, 8);                 //스으으으으으윽
            UIManager.instance.StuffNameControl(true, st.stuffName);            
        }
        else
        {
            RaycastHit2D[] hit = Physics2D.CircleCastAll(transform.position, moveStuffRange, Vector2.zero, 0, p.stuff);
            detectStuffList.Clear();
            foreach (var ga in hit)
            {
                if (comfor.GetSqr(transform.position, ga.transform.position) < squareMoveStuffRange) detectStuffList.Add(ga.transform.gameObject);
            }
            //hit = Physics2D.BoxCastAll(transform.position, new Vector2(30, 20), 0, Vector2.zero, 0, stuff);
            if (hit.Length != 0)
            {
                //Debug.Log("1");
                foreach (var hits in hit)
                {
                    if (aimstuff != hits.collider.gameObject)
                    {
                        //Debug.Log("3");
                        //Debug.Log(i);
                        Stuff stuffst = hits.transform.GetComponent<Stuff>();
                        if (!stuffst.isthrow)
                        {
                            //Debug.Log("4");
                            aimstuff = hits.transform.gameObject;
                            p.crosshair.position = aimstuff.transform.position;
                            Stuff st = aimstuff.GetComponent<Stuff>();
                            p.crosshair.gameObject.SetActive(true);
                            DualSense.instance.TriggerFeedback(false, true, 0, 8);                 //스으으으으으윽
                            UIManager.instance.StuffNameControl(true, st.stuffName);                            
                            break;
                        }
                    }
                }
                Pobserve.ischangeaim = true;
                //Debug.Log(hit[1].collider);
                //Debug.Log("Ang");
            }
        }        
    }

    public void StartStuff()
    {        
        isstuffcharge = true;
        p.crosshair.gameObject.SetActive(false);
        chargestuff = aimstuff;
        BoxCollider2D stuffbc = chargestuff.GetComponent<BoxCollider2D>();
        //stuffbc.enabled = false;
        Stuff stuffst = chargestuff.GetComponent<Stuff>();
        stuffst.iscontrol = true;
        Rigidbody2D stuffrb = chargestuff.GetComponent<Rigidbody2D>();
        stuffrb.gravityScale = 0;
        stuffrb.linearVelocity = Vector3.zero;
        stuffrb.bodyType = RigidbodyType2D.Dynamic;
        stuffrb.sharedMaterial = null;
        stuffrb.freezeRotation = true;
        if (p.currentState != PlayerState.TRAP) p.ChangePlayerState(PlayerState.MOVESTUFF);
        if (PlayerInputManager.instance.controlType == ControlTypeEnum.KEYBOARD)
        {
            //lastCursorPosition = p.mousePosition;
            PlayerInputManager.instance.LockCursor(true);
        }
    }

    public void EndStuff(float pressDuration)
    {
        if (p.currentState != PlayerState.TRAP) p.SetPlayerRBType(RigidbodyType2D.Dynamic);
        if (PlayerInputManager.instance.controlType == ControlTypeEnum.KEYBOARD)
        {            
            PlayerInputManager.instance.LockCursor(false);
            Mouse.current.WarpCursorPosition(Camera.main.WorldToScreenPoint(stuffpos));
        }

        //aimstuff = null;
        //chargestuff = null;
        //isstuffcharge = false;
        //isSetStuffRo = false;
        
        if (!isstuffcharge)
        {
            if (p.isaim)
            {
                isstuffcharge = true;
                if (p.currentState != PlayerState.TRAP) p.ChangePlayerState(PlayerState.NORMAL);
                p.crosshair.gameObject.SetActive(false);
                //DualSense.instance.ResetTrigger(false, true);
            }
        }
        else
        {            
            Stuff stuffst = chargestuff.GetComponent<Stuff>();
            Rigidbody2D stuffrb = chargestuff.GetComponent<Rigidbody2D>();
            stuffrb.gravityScale = 3;
            //stuffrb.sharedMaterial = StuffMaterial;
            stuffst.iscontrol = false;
            stuffrb.bodyType = RigidbodyType2D.Dynamic;
            stuffrb.freezeRotation = false;
            if (pressDuration < 0.3f)
            {
                //Debug.Log("Drop");
                //Debug.Log(pressduration);
            }
            else
            {
                //Debug.Log("Throw");
                //Debug.Log($"pressduration : {pressduration}");
                Vector3 throwvector = comfor.GetVector(transform.position, chargestuff.transform.position);
                if (pressDuration > 8)
                {
                    pressDuration = 8;
                }
                stuffrb.linearVelocity = throwvector * pressDuration * 20;
            }
            isstuffcharge = false;            
            isSetStuffRo = false;
            aimstuff = null;
            DualSense.instance.ResetTrigger(false, true);
            UIManager.instance.StuffNameControl(false, "");
            if (p.currentState != PlayerState.TRAP) p.ChangePlayerState(PlayerState.NORMAL);
        }
    }

    public IEnumerator LightStuffThrow(Transform throwstuff, Stuff stuff)
    {
        Vector3 throwdir = Vector3.zero;
        RaycastHit2D[] throwhitpoint = Physics2D.RaycastAll(throwstuff.position, throwdir, 100, ~p.player);
        float throwhitdistance = 100;
        int hitnum = 0;
        int j = 0;
        if (throwhitpoint.Length > 0)
        {
            foreach (var hitpoint in throwhitpoint)
            {
                j++;
                if (hitpoint.transform != transform && hitpoint.transform != throwstuff)
                {
                    hitnum = j;
                    throwhitdistance = Vector3.Distance(hitpoint.point, throwstuff.position);
                    break;
                }
            }
        }

        float oncedis = throwhitdistance / 100;
        for (int i = 0; i < 100; i++)
        {
            if (throwstuff != null)
            {
                throwstuff.position = throwstuff.position + throwdir * oncedis;
            }
            else
            {
                yield break;
            }
            yield return new WaitForSeconds(0.004f);
        }
        if (throwhitpoint.Length > 0)
        {
            stuff.Des(throwhitpoint[hitnum].point);
        }
    }
}

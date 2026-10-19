using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using Unity.Physics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public enum PlayerState
{
    NORMAL,
    MOVESTUFF,
    OBSERVE,
    AIM,
    CLIMB,
    CLIMBSLIP,
    MAKEPOTION,
    TRAP,
    PLAYTIMELINE,
    READGUIDE,
    SELECTTRAVELSTOP
}

public class Player : MonoBehaviour                                 //지금은 PlayerInput이랑 PlayerMovement역할까지 하는중.나중에 분리
{
    [SerializeField] Transform campos;    

    [SerializeField] public Transform crosshair;
    [SerializeField] Transform manualaim;
    [SerializeField] Transform manualaimob;
    [SerializeField] Transform mousedir;
    [SerializeField] Transform mousedirob;
    [SerializeField] Transform endAutoMove;

    [SerializeField] GameObject cinemachineCameraObject;
    [SerializeField] GameObject cinemachineTargetGroupObject;
    [SerializeField] GameObject trapobject;    

    [SerializeField] GameObject TestSquare;

    [SerializeField, Tooltip("카메라 y축 이동 민감도")] float camysensitivity;
    [SerializeField, Tooltip("이동 속도")] float movespeed;
    [SerializeField, Tooltip("점프 강도")] float jumpforce;
    [SerializeField, Tooltip("x축 이동감도")] float moveInput;
    [SerializeField, Tooltip("aim")] public float aimInput;
    [SerializeField, Tooltip("")] float pressstarttime;
    [SerializeField, Tooltip("추락 시작")] float fallstart;             //falldis 계산후 바로 초기화
    [SerializeField, Tooltip("추락 길이")] float falldis;
    [SerializeField, Tooltip("확대 값")] float zoomValue;

    [SerializeField, Tooltip("보는 방향")] public int lookingdir;
    [Tooltip("")] public int interationpriority;

    [SerializeField, Tooltip("aim")] public Vector3 aimvector;
    [SerializeField, Tooltip("aim")] public Vector3 aimdir;
    [SerializeField, Tooltip("aim")] public Vector3 mouseAimdir;
    [SerializeField, Tooltip("aim")] public Vector2 mousePosition;
    [SerializeField, Tooltip("aim")] public Vector2 lastMousePosition;                                  //전 프레임 마우스 위치
    [SerializeField, Tooltip("aim")] Vector3 stuffvel;
    [Tooltip("aim")] public Vector3 itemDropPosition;
    [Tooltip("aim")] public Vector3 autoMovePosition;
    Vector3 tempCamPos;

    [SerializeField, Tooltip("이동 가능")] bool canmove;
    [SerializeField, Tooltip("이동 가능")] bool canAutoMove;
    [SerializeField, Tooltip("조준중")] public bool isjump;
    [field: SerializeField, Tooltip("조준중")] public bool isaim { get; private set; }
    [SerializeField, Tooltip("지면에 접촉중")] public bool isground;
    [field: SerializeField, Tooltip("이동 가능")] public bool isObserveAim { get; private set; }
    [SerializeField, Tooltip("이동중")] bool ismove;
    [SerializeField, Tooltip("버리기 시작")] bool isDropStart;
    [SerializeField, Tooltip("버리기 시작")] bool isDropKeyPress;
    [SerializeField, Tooltip("stuff에 깔린 상태")] bool istrap;    
    [SerializeField, Tooltip("줌중")] bool isZoomIn;

    [Header("SetCamPos")]
    [SerializeField] Transform SetCamPosTransform;
    [SerializeField, Tooltip("카메라 위치 정해졌는지")] bool isSetCamPos;
    [SerializeField, Tooltip("카메라 위치 잠겼는지")] bool isLockCamPos;
    [SerializeField, Tooltip("줌만")] bool onlyZoom;
    [SerializeField, Tooltip("락된 위치")] Vector3 lockPos;
    [SerializeField, Tooltip("추가된 위치")] Vector3 camExPos;
    [SerializeField, Tooltip("SetCamPos 확대 값")] float camPosZoomValue;
    [SerializeField] GameObject setObject;

    [Header("Layer")]
    [field: SerializeField] public PlayerState currentState {get; private set;}

    [field: SerializeField] public LayerMask camRay { get; private set; }    
    [field: SerializeField] public LayerMask ground { get; private set; }
    [field: SerializeField] public LayerMask climbground { get; private set; }
    [field: SerializeField] public LayerMask wall { get; private set; }
    [field: SerializeField] public LayerMask player { get; private set; }
    [field: SerializeField] public LayerMask stuff { get; private set; }
    [field: SerializeField] public LayerMask itemUse { get; private set; }
    [field: SerializeField] public LayerMask enemy { get; private set; }

    Coroutine StepVibvari;
    Coroutine ClimbUpvari;
    Coroutine ClimbEndvari;
    Coroutine ClimbHelpervari;
    Coroutine ClimbSlipvari;

    public Rigidbody2D Prb;
    CapsuleCollider2D Pcc;
    PlayerAnimation Panim;
    Climb Pclimb;
    PlayerMoveStuff Pstuff;
    PlayerObserve Pobserve;
    PlayerParticle Pparticle;
    PlayerStandTile Pstand;
    SpriteRenderer Pspr;
    public InteractScript interat;
    CinemachineCamera cineCam;
    //CinemachineGroupFraming group;
    CinemachineTargetGroup targetGroup;    

    [SerializeField] PhysicsMaterial2D StuffMaterial;

    readonly RaycastHit2D[] nonAlloc = new RaycastHit2D[20];

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        //DontDestroyOnLoad(gameObject);

        Prb = GetComponent<Rigidbody2D>();
        Pcc = GetComponent<CapsuleCollider2D>();
        Panim = GetComponent<PlayerAnimation>();
        Pspr = GetComponentInChildren<SpriteRenderer>();
        Pclimb = GetComponent<Climb>();
        Pstuff = GetComponent<PlayerMoveStuff>();
        Pobserve = GetComponent<PlayerObserve>();
        Pparticle = GetComponent<PlayerParticle>();
        Pstand = GetComponent<PlayerStandTile>();
        cineCam = cinemachineCameraObject.GetComponent<CinemachineCamera>();
        targetGroup = cinemachineTargetGroupObject.GetComponent<CinemachineTargetGroup>();        
        
        //Debug.Log($"Distance : {Vector3.Distance(transform.position, TestSquare.transform.position)}");
        //Debug.Log($"sqrMagnitude : {Mathf.Sqrt((transform.position - TestSquare.transform.position).sqrMagnitude)}");
    }

    private void Start()
    {
        //DualSense.instance.TriggerFeedback(true, false, 0, 1);
        //DualSense.instance.WeaponTrigger(false, true, 4, 7, 8);
    }

    void Update()
    {
        if (moveInput != 0 && canmove && currentState != PlayerState.MAKEPOTION)
        {
            if (moveInput == 1)
            {
                Pspr.flipX = false;
                //Debug.Log(Pspr.flipX);
            }
            else
            {
                Pspr.flipX = true;
                //Debug.Log(Pspr.flipX);
            }
        }

        Panim.SetBool("isground", isground);
        Panim.SetFloat("yvel", Prb.linearVelocityY);

        //fallstart 추락 시작 위치 설정
        {
            if (fallstart == 0 && Prb.linearVelocityY < -1)
            {
                //Debug.Log("fallstart set");
                fallstart = transform.position.y;
            }
        }
    }

    void FixedUpdate()
    {
        //캠포스 위치
        {            
            if (!isZoomIn)                                                                                                                                   //줌 안했을때에만 작동
            {
                if (isSetCamPos) cineCam.Lens.OrthographicSize = Mathf.Lerp(cineCam.Lens.OrthographicSize, camPosZoomValue, 0.1f);
                else cineCam.Lens.OrthographicSize = Mathf.Lerp(cineCam.Lens.OrthographicSize, 5.5f, 0.1f);
            }
            else if (isZoomIn) cineCam.Lens.OrthographicSize = Mathf.Lerp(cineCam.Lens.OrthographicSize, zoomValue, 0.1f);                                                      //캠 렌즈(확대) 조절            

            if (currentState != PlayerState.PLAYTIMELINE) SetCamPos();
        }

        if (Pclimb.ClimbEndvari == null && !isaim && !isObserveAim && currentState == PlayerState.NORMAL && !canAutoMove)                                                      //뛸 수 있는지 판단
        {
            canmove = true;
            if (moveInput != 0)
            {
                //walkParticleObject.transform.localPosition = new Vector3(lookingdir < 0 ? 0.3f : -0.3f, -0.6f, 0);
                //walkParticleObject.transform.rotation = Quaternion.Euler(lookingdir < 0 ? -170 : -10, -90, 0);
                lookingdir = (int)moveInput;
                Prb.linearVelocityX = moveInput * movespeed;                                    //가속도주는 코드
                if (!ismove)
                {
                    if (isground)
                    {
                        ismove = true;
                        if (StepVibvari != null)
                        {
                            StopCoroutine(StepVibvari);
                            StepVibvari = null;
                        }
                        StepVibvari = StartCoroutine(StepVib(0.4f));
                    }
                }
                else
                {
                    if (!isground)
                    {
                        if (StepVibvari != null)
                        {
                            StopCoroutine(StepVibvari);
                            StepVibvari = null;
                        }
                    }
                }
            }
            else
            {
                ismove = false;
                if (StepVibvari != null)
                {
                    StopCoroutine(StepVibvari);
                    StepVibvari = null;
                }
            }
        }
        else
        {
            canmove = false;
            Debug.Log("Fale");
        }
        if (currentState != PlayerState.PLAYTIMELINE) Panim.SetBool("isrun", ismove);

        if (canAutoMove)
        {
            transform.position = Vector3.MoveTowards(transform.position, autoMovePosition, 0.057f);
            ismove = true;
            if (comfor.GetSqr(transform.position, autoMovePosition) < 0.01f)
            {
                canAutoMove = false;
                ismove = false;
                if (StepVibvari != null)
                {
                    StopCoroutine(StepVibvari);
                    StepVibvari = null;
                }
            }
        }
    }    

    public void OnMove(Vector2 move)
    {        
        //Debug.Log("D");
        if (move.x > 0.5f)
        {
            //moveInput = Vector3.right;
            moveInput = 1;
        }
        else if (move.x < -0.5f)
        {
            //moveInput = Vector3.left;
            moveInput = -1;
        } else
        {
            //moveInput = Vector3.zero;
            moveInput = 0;
        }
    }

    public void OnAim(Vector2 aim)
    {        
        aimdir = aim;
        if (aimdir != Vector3.zero)
        {            
            Vector3 temp = comfor.GetVector(transform.position, transform.position + aimdir);
            mousedir.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(temp.y, temp.x) * Mathf.Rad2Deg);
            aimvector = comfor.GetVector(transform.position, mousedirob.position);
        }
        if (Mathf.Abs(aimdir.x) > 0.7f)
        {
            aimInput = Mathf.Sign(aimdir.x);
        }else
        {
            aimInput = 0;
        }
    }

    public void OnMouseAim(Vector2 mouse)
    {
        mouseAimdir = mouse;
        if (mouseAimdir != Vector3.zero)
        {
            Vector3 temp = comfor.GetVector(transform.position, transform.position + mouseAimdir);
            mousedir.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(temp.y, temp.x) * Mathf.Rad2Deg);
            aimvector = comfor.GetVector(transform.position, mousedirob.position);
        }
        if (Mathf.Abs(mouseAimdir.x) > 0.7f)
        {
            aimInput = Mathf.Sign(mouseAimdir.x);
        }
        else
        {
            aimInput = 0;
        }
    }

    public void SetMousePosition(Vector2 mousePos)
    {
        lastMousePosition = mousePosition;
        mousePosition = mousePos;
    }

    public void OnInteraction()             //세모
    {
        PotionManager.instance.Makepotion('y');
        if (interat != null && currentState == PlayerState.NORMAL && !Pobserve.isObserve)
        {
            interat.interatfeedback();
            DualSense.instance.Hapticvoid(0.04f, 0.5f, 0.04f, 2, 2);
        }
    }

    public void OnSquareEnd(float presstime)
    {
        if (ItemManager.instance.selectInventoryType == 0)
        {
            //isDropKeyPress = false;
            ItemManager.instance.RenewItemCanUse(false);
            //float pressduration = Time.time - squarestart;
            if (presstime < 1.5f)
            {
                int hitCount = Physics2D.CircleCastNonAlloc(transform.position, 1.5f, Vector2.zero, nonAlloc, 0, itemUse);
                GameObject holdItem = ItemManager.instance.itemList[ItemManager.instance.selectSlot];
                if (hitCount > 0)
                {
                    for (int i = 0; i < hitCount; i++)
                    {
                        if (holdItem != null)
                        {
                            ItemUse use = nonAlloc[i].collider.GetComponent<ItemUse>();
                            int itemCode = holdItem.GetComponent<ItemState>().itemCode;
                            //Debug.Log($"Ang : {itemCast[i].collider}");
                            if (use.canuse)
                            {
                                if (use.needItem == itemCode)                                               //ItemManager의 ItemUse 메소드도 포함
                                {
                                    use.itemUseEvent.Invoke();
                                    Debug.Log("Use");
                                    break;
                                }
                                else if (use.needItem == 0)
                                {
                                    use.itemUseEvent.Invoke();
                                    break;
                                }
                                else
                                {
                                    UIManager.instance.ItemUsingText(false, itemCode);
                                    break;
                                }
                            }
                        }
                        else
                        {
                            UIManager.instance.ItemUsingText(false);
                            break;
                        }
                    }
                    ItemManager.instance.ItemDropCancel();
                    //Debug.Log("Use");
                }
                else if (holdItem != null)
                {
                    ItemState item = holdItem.GetComponent<ItemState>();
                    //Debug.Log($"{item.canUseSolo}");
                    if (item.canUseSolo)
                    {
                        item.itemUseEvent.Invoke();
                        UIManager.instance.ItemUsingText(true, item.itemCode);
                        ItemManager.instance.ItemUse();
                    }
                    else
                    {
                        UIManager.instance.ItemUsingText(false);
                    }
                }
            }
            else
            {
                ItemManager.instance.ItemDrop(false, 0, itemDropPosition);
                ItemManager.instance.ItemDropCancel();
                //Debug.Log("Drop");
            }
        }
    }

    public void AimEquipmentStart(int equipmentCode)
    {
        switch (equipmentCode)
        {
            case 1:
                if (!Pstuff.isstuffcharge && Pstuff.canmovestuff && currentState != PlayerState.CLIMB)
                {
                    isaim = true;
                    Pstuff.DetectStuff();
                }                    
                break;

            case 2:
                isObserveAim = true;
                Pobserve.DetectObserve();
                break;
        }
    }

    public void AimEquipmentEnd(int equipmentCode)
    {
        switch (equipmentCode)
        {
            case 1:
                isaim = false;
                crosshair.gameObject.SetActive(false);
                DualSense.instance.ResetTrigger(false, true);
                UIManager.instance.StuffNameControl(false, "");
                ChangePlayerState(PlayerState.NORMAL);
                Prb.bodyType = RigidbodyType2D.Dynamic;
                break;

            case 2:
                isObserveAim = false;
                Pobserve.EndObserve();
                break;
        }
    }

    public void UseEquipmentStart(int equipmentCode)
    {
        switch (equipmentCode)
        {
            case 1:
                if (!Pstuff.isstuffcharge && Pstuff.canmovestuff)                                                                                 //충전안된 상태에서 누름
                {
                    if (isaim)
                    {
                        Pstuff.StartStuff();
                        //Debug.Log("Dynamic");
                    }
                }
                else
                {
                    pressstarttime = Time.time;
                    float pressduration = Time.time - pressstarttime;
                    Pstuff.EndStuff(pressduration);
                }
                break;

            case 2:
                Pobserve.StartObserve();
                break;
        }        
    }

    public void UseEquipmentEnd(int equipmentCode)
    {

    }
    
    public void AutoMove(Vector3 pos)
    {
        autoMovePosition = pos;
        canAutoMove = true;
        ismove = true;
        if (transform.position.x < autoMovePosition.x)
        {
            Debug.Log("NoFlip");
            Pspr.flipX = false;
        }else
        {
            Debug.Log("Flip");
            Pspr.flipX = true;
        }
    }

    public void EndWalk()
    {
        autoMovePosition = endAutoMove.position;
        canAutoMove = true;
        ismove = true;
        if (transform.position.x < autoMovePosition.x)
        {
            Debug.Log("NoFlip");
            Pspr.flipX = false;
        }
        else
        {
            Debug.Log("Flip");
            Pspr.flipX = true;
        }
    }

    public void SetCamPos()
    {
        int hit = Physics2D.RaycastNonAlloc(transform.position + Vector3.down * 0.5f, Vector2.down, nonAlloc, Mathf.Infinity, camRay);         //campos위치 설정
        if (hit > 0)
        {
            RaycastHit2D camGround = nonAlloc[0];

            for (int i = 1; i < hit; i++)
            {
                if (nonAlloc[i].distance < camGround.distance)
                {
                    camGround = nonAlloc[i];
                }
            }

            if (((transform.position.y - camGround.point.y) > camysensitivity) || (camGround.point.y == 0))
            {
                tempCamPos = new Vector3(transform.position.x, transform.position.y + 2f, -8) + camExPos;
                //Debug.Log("Up");
            }
            else
            {
                tempCamPos = new Vector3(transform.position.x, camGround.point.y + 6f, -8) + camExPos;
                //Debug.Log($"Normal, CamGround.point : {camGround.point}");
            }
        }

        if (!isSetCamPos || (isSetCamPos && onlyZoom))
        {
            campos.position = tempCamPos;
        }else if (!onlyZoom)
        {
            if (!isLockCamPos) campos.position = new Vector3(SetCamPosTransform.position.x, SetCamPosTransform.position.y, -8) + camExPos;
            else campos.position = new Vector3(lockPos.x == 0 ? transform.position.x : lockPos.x, lockPos.y == 0 ? tempCamPos.y : lockPos.y, -8) + camExPos;
        }
    }

    public void ZoomIn(GameObject zoomInObject, float camLens)
    {
        isZoomIn = true;
        zoomValue = camLens;
        Debug.Log("Zoom");
    }

    public void ZoomOut()
    {
        isZoomIn = false;
        zoomValue = 5.5f;
    }

    public void FocusWith(GameObject FocusObject)
    {
        targetGroup.AddMember(FocusObject.transform, 10, 1);
    }

    public void EndFocus(GameObject FocusObject)
    {
        targetGroup.RemoveMember(FocusObject.transform);
    }

    public void MakePotionAnim(bool isLeft, int animNum)
    {
        Pspr.flipX = isLeft;
        Panim.ResetTrigger("makePotion");
        Panim.SetInt("potionNum", animNum);
        Panim.SetTrigger("makePotion");        
        Debug.Log("Sssss");
    }

    public void MakePotionEnd()
    {
        Panim.SetTrigger("makePotionEnd");
    }

    public void SetPlayerRBType(RigidbodyType2D type)
    {
        Prb.bodyType = type;
    }

    //플레이어 상태 변경
    public void ChangePlayerState(PlayerState newState)
    {
        if (currentState == newState) return;

        ExitState(newState);                                                    //내부적으론 currentState사용

        currentState = newState;

        EnterState();
    }

    //---------------------------------------------------------------------아래로 Enter

    void ExitState(PlayerState newState)
    {
        switch (currentState)                           //바뀌기 전에 전 상태 이탈 ExitState
        {
            case PlayerState.NORMAL:
                ExitNormal();
                break;

            case PlayerState.MOVESTUFF:
                ExitMovestuff();
                break;

            case PlayerState.OBSERVE:
                ExitObserve();
                break;

            case PlayerState.CLIMB:
                if (newState != PlayerState.CLIMBSLIP) ExitClimb();
                break;

            case PlayerState.CLIMBSLIP:
                ExitClimbSlip();
                break;

            case PlayerState.MAKEPOTION:
                ExitMakepotion();
                break;

            case PlayerState.TRAP:
                ExitTrap();
                break;
        }
    }

    void EnterState()
    {
        switch (currentState)                           //바뀌고 나서 상태 진입 EnterState
        {
            case PlayerState.NORMAL:
                EnterNormal();
                break;

            case PlayerState.MOVESTUFF:
                EnterMovestuff();
                break;

            case PlayerState.OBSERVE:
                EnterObserve();
                break;

            case PlayerState.CLIMB:
                EnterClimb();
                break;

            case PlayerState.CLIMBSLIP:
                EnterClimbSlip();
                break;

            case PlayerState.MAKEPOTION:
                EnterMakepotion();
                break;

            case PlayerState.TRAP:
                EnterTrap();
                break;
        }
    }

    void EnterNormal()
    {        
        Prb.gravityScale = 3;
        Prb.bodyType = RigidbodyType2D.Dynamic;
        Debug.Log("Normal");
    }    

    void EnterMovestuff()
    {
        isaim = true;
        if (StepVibvari != null)
        {
            StopCoroutine(StepVibvari);
            StepVibvari = null;
        }        
        ismove = false;
    }

    void EnterObserve()
    {
        isObserveAim = true;
        if (StepVibvari != null)
        {
            StopCoroutine(StepVibvari);
            StepVibvari = null;
        }        
        ismove = false;
    }    

    void EnterClimb()
    {
        isjump = false;
        Panim.SetBool("isjump", false);
        Prb.gravityScale = 0;
        Prb.linearVelocity = Vector3.zero;
    }

    void EnterClimbSlip()
    {
        Prb.gravityScale = 1.5f;
        Debug.Log("Slip");
    }

    void EnterMakepotion()
    {

    }    

    void EnterTrap()
    {
        istrap = true;
        SetPlayerRBType(RigidbodyType2D.Static);
    }
    //---------------------------------------------------------------------아래로 Exit

    void ExitNormal()
    {
        ismove = false;
        if (StepVibvari != null)
        {
            StopCoroutine(StepVibvari);
            StepVibvari = null;
        }
    }

    void ExitMovestuff()
    {
        isaim = false;
    }

    void ExitObserve()
    {
        isObserveAim = false;
    }

    void ExitClimb()
    {
        Prb.gravityScale = 3;
        Debug.Log("ExitClimb");
    }

    void ExitClimbSlip()
    {
        Prb.gravityScale = 3;
        Debug.Log("ExitClimbSlip");
    }

    void ExitMakepotion()
    {

    }

    void ExitTrap()
    {
        SetPlayerRBType(RigidbodyType2D.Dynamic);
    }

    //플레이어 상태 변경

    //기본 이동
    public void Jump()
    {
        if (currentState != PlayerState.CLIMB)
        {
            if (isground && canmove && currentState == PlayerState.NORMAL)                                                     //기본 점프
            {
                Panim.SetBool("isjump", true);
                Panim.SetTrigger("jump");
                Prb.linearVelocityY = 0;
                Prb.AddForceY(jumpforce, ForceMode2D.Impulse);
                Pparticle.JumpStartParticleOn();
                isjump = true;
                isground = false;
                ismove = false;
                if (StepVibvari != null)
                {
                    StopCoroutine(StepVibvari);
                    StepVibvari = null;
                }
            }
        }else if(Pclimb.ClimbEndvari == null)                                                                                     //등반 점프
        {
            isjump = true;
            Prb.linearVelocity = new Vector3(0.6f, 0.8f, 0) * jumpforce / 10 * -lookingdir;
            //Prb.AddForce(new Vector3(1, 0.7f, 0) * jumpforce, ForceMode2D.Impulse);
            DualSense.instance.Hapticvoid(0, 1, 0.06f, 2, 2);
            if (ClimbEndvari != null)
            {
                StopCoroutine(ClimbEndvari);
                ClimbEndvari = null;
            }
            StartCoroutine(Pclimb.ClimbEnd(0));
            Debug.Log("climbjump");
        }
    }

    //텔포
    public void Teleport(Transform telPos)
    {
        transform.position = telPos.position;
        campos.position = telPos.position;
    }

    IEnumerator StepVib(float dur)
    {
        while (true)
        {
            DualSense.instance.Hapticvoid(0, 0.004f, 0.06f, 0, 1);
            Pparticle.WalkParticleOn(new Vector3(lookingdir < 0 ? 0.3f : -0.3f, -0.6f, 0), new Vector3(lookingdir < 0 ? -170 : -10, -90, 0));
            yield return new WaitForSeconds(dur);
            DualSense.instance.Hapticvoid(0.004f, 0, 0.04f, 1, 0);
            Pparticle.WalkParticleOn(new Vector3(lookingdir < 0 ? 0.3f : -0.3f, -0.6f, 0), new Vector3(lookingdir < 0 ? -170 : -10, -90, 0));
            yield return new WaitForSeconds(dur);
        }
    }
    //기본 이동

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        //Gizmos.DrawCube(transform.position - new Vector3(0, 0.75f, 0), new Vector3(0.6f, 0.175f, 0));
    }



    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Stuff"))
        {
            if (Physics2D.BoxCast(transform.position - new Vector3(0, 0.7f, 0), new Vector3(0.6f, 0.3f, 0), 0, Vector2.zero, 0, ground))
            {
                if (currentState != PlayerState.CLIMB)
                {
                    Pparticle.JumpEndParticleOn();

                    if (StepVibvari != null)
                    {
                        StopCoroutine(StepVibvari);
                        StepVibvari = null;
                    }
                    if (ismove) StepVibvari = StartCoroutine(StepVib(0.4f));

                    falldis = fallstart - transform.position.y;
                    fallstart = 0;

                    RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 3, ground);
                    if (hit == collision.collider && currentState != PlayerState.PLAYTIMELINE)
                    {
                        //Debug.Log(falldis);
                        if (falldis > 0.2f)                                                    //떨어진 거리가 0.2이상일때
                        {
                            //Debug.Log(falldis);
                            if (falldis < 1.8f)
                            {
                                DualSense.instance.Hapticvoid(0.2f * falldis, 0.003f * falldis, 0.1f, 3, 3);
                                //Debug.Log("Fallvib");
                            }
                            else
                            {
                                DualSense.instance.Hapticvoid(0.2f * falldis, 0.003f * falldis, 0.03f * falldis, 3, 3);
                                //Debug.Log("Fallvib");
                            }
                            StartCoroutine(CamShake.instance.Shake(0.05f, 0.1f * falldis, 0.05f, 3, 1));
                        }
                    }
                }
            }                    
        }

        if (collision.gameObject.CompareTag("Stuff"))
        {
            if (Pstuff.isstuffcharge)
            {
                if (Physics2D.BoxCast(transform.position - new Vector3(0, 0.7f, 0), new Vector3(0.6f, 0.1f, 0), 0, Vector2.zero, 0, stuff))
                {
                    //Debug.Log("False");
                    Rigidbody2D stuffrb = Pstuff.chargestuff.GetComponent<Rigidbody2D>();
                    stuffrb.linearVelocity = Vector3.zero;
                    stuffrb.gravityScale = 3;
                    stuffrb.sharedMaterial = StuffMaterial;
                    stuffrb.bodyType = RigidbodyType2D.Dynamic;
                    stuffrb.freezeRotation = false;
                    Pstuff.isstuffcharge = false;                    
                    Pstuff.isSetStuffRo = false;
                    Stuff stuffst = Pstuff.chargestuff.GetComponent<Stuff>();
                    stuffst.iscontrol = false;
                    Pstuff.aimstuff = null;
                    Pstuff.chargestuff = null;
                    DualSense.instance.ResetTrigger(false, true);
                    UIManager.instance.StuffNameControl(false, "");
                    if (currentState == PlayerState.CLIMB)
                    {
                        if (ClimbEndvari != null)
                        {
                            StopCoroutine(ClimbEndvari);
                            ClimbEndvari = null;
                        }
                        StartCoroutine(Pclimb.ClimbEnd(0));
                    }
                }
            }

            if (Physics2D.BoxCast(transform.position + new Vector3(0, 0.7f, 0), new Vector3(0.6f, 0.2f, 0), 0, Vector2.zero, 0, stuff) && isground)                     //깔림
            {
                Debug.Log("D");
                Prb.linearVelocityX = 0;
                Prb.linearVelocityY -= 2;
                Rigidbody2D Srb = collision.transform.GetComponent<Rigidbody2D>();
                Srb.gravityScale = 3;
                Srb.linearVelocity = Vector3.zero;
                Srb.bodyType = RigidbodyType2D.Kinematic;
                Srb.freezeRotation = false;
                Stuff stuffst = collision.collider.GetComponent<Stuff>();
                stuffst.iscontrol = false;
                BoxCollider2D Sbc = collision.transform.GetComponent<BoxCollider2D>();
                collision.transform.parent = transform;
                int hitCount = Physics2D.RaycastNonAlloc(transform.position, Vector3.down, nonAlloc, 2, ground);
                if (hitCount > 0) collision.transform.position = new Vector3(collision.transform.position.x, nonAlloc[0].point.y + Pcc.size.y + Sbc.size.y / 2, 0);
                Debug.Log(new Vector3(collision.transform.position.x, transform.position.y + Sbc.size.y / 2 + 0.5f, 0));
                //Pstuff.aimstuff = null;
                //Pstuff.chargestuff = null;
                //Pstuff.isstuffcharge = false;                
                //Pstuff.isSetStuffRo = false;
                ChangePlayerState(PlayerState.TRAP);
                Pstuff.EndStuff(0);
                trapobject = collision.gameObject;
                UIManager.instance.StuffNameControl(false, "");
                if (stuffst.fallDis > 6)
                {
                    DualSense.instance.Hapticvoid(1 * stuffst.weight * falldis / 7, 1 * falldis / 7, 0.7f, 3, 3);
                    //Debug.Log(1 * weight * falldis / 3);
                }
                else
                {
                    DualSense.instance.Hapticvoid(1 * stuffst.weight * falldis / 15, 1 * falldis / 15, 0.07f * falldis, 3, 3);                          //항상 똑같음
                                                                                                                                                //Debug.Log(1 * weight * falldis / 7);
                }
            }

            //if (Physics2D.BoxCast(transform.position + new Vector3(0, -0.5f, 0), new Vector3(0.9f, 0.175f, 0), 0, Vector2.zero, 0, stuff))
            //{
            //    Rigidbody2D Srb = collision.transform.GetComponent<Rigidbody2D>();
            //    Srb.gravityScale = 3;
            //    Srb.linearVelocityY = 0;
            //}
        }

        if (collision.gameObject.CompareTag("Wall"))
        {
            Prb.linearVelocityX = 0;
        }

        //if (Prb.linearVelocityY < 0 && (collision.gameObject.layer & 1 << climbground.value | 1 << wall.value) != 0) Pclimb.LIteClimb(new Vector2(lookingdir, 0));
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Stuff"))
        {
            if (Physics2D.BoxCast(transform.position - new Vector3(0, 0.7f, 0), new Vector3(0.6f, 0.3f, 0), 0, Vector2.zero, 0, ground))
            {
                if (currentState != PlayerState.CLIMB)
                {                    
                    if (isjump)
                    {
                        isjump = false;
                        Panim.SetBool("isjump", false);
                    }
                    isground = true;                    
                }
            }

            if (Physics2D.BoxCast(transform.position - new Vector3(0, 0.7f, 0), new Vector3(0.6f, 0.1f, 0), 0, Vector2.zero, 0, ground) && currentState == PlayerState.CLIMBSLIP)                      //미끄러져서 착지
            {
                if (ClimbEndvari != null)
                {
                    StopCoroutine(ClimbEndvari);
                    ClimbEndvari = null;
                }
                StartCoroutine(Pclimb.ClimbEnd(0));
                ChangePlayerState(PlayerState.NORMAL);
                fallstart = transform.position.y;
            }
        }

        // Debug.Log("stay");
        if (Prb.linearVelocityY < -2 && currentState != PlayerState.CLIMBSLIP && (collision.gameObject.layer & 1 << climbground.value | 1 << wall.value) != 0) Pclimb.LIteClimb(new Vector2(lookingdir, 0));

        if (collision.gameObject.CompareTag("Ground"))
        {
            Pstand.RewnewStandTile();
        }
        //if (Prb.linearVelocityY < 0 && (collision.gameObject.layer & 1 << climbground.value | 1 << wall.value) != 0) Pclimb.ClimbStart(new Vector2(lookingdir, 0));

        if (collision.gameObject.CompareTag("Stuff"))
        {
            if (trapobject == collision.collider.gameObject && isground)
            {
                //transform.parent = null;
                collision.transform.parent = null;
                Rigidbody2D Srb = collision.transform.GetComponent<Rigidbody2D>();
                Stuff stuff = collision.transform.GetComponent<Stuff>();
                if (stuff.iscontrol)
                {
                    Srb.gravityScale = 0;
                    Srb.angularDamping = 0;
                }
                else
                {
                    Srb.gravityScale = 3;
                }
                Srb.linearVelocityY = 0;
                Srb.bodyType = RigidbodyType2D.Dynamic;
                istrap = false;
                trapobject = null;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isground = Physics2D.BoxCast(transform.position - new Vector3(0, 0.7f, 0), new Vector3(0.6f, 0.1f, 0), 0, Vector2.zero, 0, ground);
            //transform.parent = null;
        }

        if (collision.gameObject.CompareTag("Stuff"))
        {
            if (trapobject == collision.collider.gameObject && isground)
            {
                //transform.parent = null;
                collision.transform.parent = null;
                Rigidbody2D Srb = collision.transform.GetComponent<Rigidbody2D>();
                Stuff stuff = collision.transform.GetComponent<Stuff>();
                if (stuff.iscontrol)
                {
                    Srb.gravityScale = 0;
                    Srb.angularDamping = 0;
                }
                else
                {
                    Srb.gravityScale = 3;
                }
                Srb.linearVelocityY = 0;
                Srb.bodyType = RigidbodyType2D.Dynamic;
                istrap = false;
                trapobject = null;
            }
        }

        //if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Stuff"))
        //{
        //    transform.parent = null;
        //}
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("CamPos"))
        {
            setObject = collision.gameObject;
            SetCampos scp = setObject.GetComponent<SetCampos>();
            SetCamPosTransform = scp.Pos;
            isLockCamPos = scp.isLockPos;
            camPosZoomValue = scp.zoomValue;
            lockPos = scp.camPos;
            camExPos = scp.exPos;
            isSetCamPos = true;
            onlyZoom = scp.onlyZoom;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("CamPos"))
        {
            if (setObject == collision.gameObject)
            {                
                SetCamPosTransform = null;
                camPosZoomValue = 0;
                lockPos = Vector3.zero;
                camExPos = Vector3.zero;
                isSetCamPos = false;
                onlyZoom = false;
            }            
        }
    }
}

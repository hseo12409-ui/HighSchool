using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerObserve : MonoBehaviour
{
    [SerializeField] GameObject observeTarget;
    [SerializeField] List<GameObject> observeList = new List<GameObject>();

    [SerializeField, Tooltip("조준 변경중")] public bool ischangeaim;
    
    [field : SerializeField, Tooltip("이동 가능")] public bool isObserve {  get; private set; }

    [SerializeField] float observeRange = 6;                                            //게임 시작하면 자동으로 제곱함(최적화 때문)
    private float squareObserveRange;

    readonly RaycastHit2D[] nonAlloc = new RaycastHit2D[20];
    readonly Collider2D[] nonAllocC = new Collider2D[20];

    Player p;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        p = GetComponent<Player>();
        squareObserveRange = observeRange * observeRange;
    }

    // Update is called once per frame
    void Update()
    {
        if (p.isObserveAim)
        {
            if (PlayerInputManager.instance.controlType == ControlTypeEnum.GAMEPAD)
            {
                if (Mathf.Abs(p.aimdir.x) + Mathf.Abs(p.aimdir.y) > 0.8f)
                {
                    if (!ischangeaim)
                    {
                        ischangeaim = true;
                        observeState forExpos = observeTarget.GetComponent<observeState>();
                        int count = Physics2D.BoxCastNonAlloc(observeTarget.transform.position + forExpos.observePos, new Vector2(1, 2), Mathf.Atan2(Mathf.Abs(p.aimvector.y), Mathf.Abs(p.aimvector.x)) * Mathf.Rad2Deg, p.aimvector, nonAlloc, 20);
                        Debug.Log(count);
                        if (count != 0)
                        {
                            GameObject aimObserveTarget = null;
                            float shortestDistance = float.MaxValue;
                            for (int i = 0; i < count; i++)
                            {
                                Debug.Log("Ay");
                                Debug.Log(nonAlloc[i].transform.gameObject.name);
                                float disSqr = comfor.GetSqr(transform.position, nonAlloc[i].transform.position);
                                if (shortestDistance != 0 && shortestDistance > disSqr && nonAlloc[i].collider.gameObject.TryGetComponent<observeState>(out _) && nonAlloc[i].collider.gameObject != observeTarget && observeList.Contains(nonAlloc[i].collider.gameObject))
                                {
                                    aimObserveTarget = nonAlloc[i].collider.gameObject;
                                    shortestDistance = disSqr;
                                }
                            }

                            if (aimObserveTarget != null)
                            {
                                observeState lastObserve = observeTarget.GetComponent<observeState>();
                                lastObserve.isObserve = false;
                                p.EndFocus(observeTarget);
                                p.ZoomOut();
                                UIManager.instance.OnObserveUI(false, "", "", "", null, null);
                                observeTarget = aimObserveTarget;
                                isObserve = false;
                                //stat.isObserve = true;
                                //stat.RewnewObserveUI(true);
                                UIManager.instance.OnObserveIcon(observeList, aimObserveTarget, true);
                            }
                        }
                    }
                }
                else
                {
                    ischangeaim = false;
                }
            }else
            {
                Vector3 mousePosWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Collider2D[] hit = Physics2D.OverlapCircleAll(mousePosWorld, 0.5f);
                //Debug.Log("KM FIND" + mousePosWorld);

                foreach (var hits in hit)
                {
                    if (hits.gameObject.TryGetComponent<observeState>(out _) && hits.gameObject != observeTarget && observeList.Contains(hits.gameObject))
                    {
                        Debug.Log($"Target : {observeTarget}, hits : {hits.gameObject}");
                        observeState lastObserve = observeTarget.GetComponent<observeState>();
                        lastObserve.isObserve = false;
                        p.EndFocus(observeTarget);
                        p.ZoomOut();
                        UIManager.instance.OnObserveUI(false, "", "", "", null, null);
                        observeTarget = hits.gameObject;
                        isObserve = false;
                        //stat.isObserve = true;
                        //stat.RewnewObserveUI(true);
                        UIManager.instance.OnObserveIcon(observeList, observeTarget, true);                        
                        break;
                    }
                }
            }
        }
    }

    public void DetectObserve()
    {
        if (observeTarget != null && comfor.GetSqr(transform.position, observeTarget.transform.position) < squareObserveRange)                                //사정거리를 제곱한 값
        {
            RaycastHit2D[] hit = Physics2D.CircleCastAll(transform.position, observeRange, Vector2.zero, 0);
            foreach (var hits in hit)                                                   //실제로 관찰이 가능한 오브젝트만 걸러내는 과정
            {
                if (hits.collider.TryGetComponent<observeState>(out _) && comfor.GetSqr(transform.position, hits.transform.position) < squareObserveRange)
                {
                    observeList.Add(hits.transform.gameObject);
                }
            }            
            DualSense.instance.TriggerFeedback(false, true, 0, 8);
            UIManager.instance.OnObserveIcon(observeList, observeTarget, true);
        }
        else
        {            
            RaycastHit2D[] hit = Physics2D.CircleCastAll(transform.position, observeRange, Vector2.zero, 0);
            if (hit != null)
            {
                foreach (var hits in hit)                                                   //실제로 관찰이 가능한 오브젝트만 걸러내는 과정
                {
                    if (hits.collider.TryGetComponent<observeState>(out _) && comfor.GetSqr(transform.position, hits.transform.position) < squareObserveRange)
                    {
                        observeList.Add(hits.transform.gameObject);
                    }
                }
                if (observeList.Count > 0)
                {
                    observeTarget = observeList[0].transform.gameObject;                    
                    DualSense.instance.TriggerFeedback(false, true, 0, 8);
                    UIManager.instance.OnObserveIcon(observeList, observeList[0], true);
                }
            }
        }
    }

    public void StartObserve()
    {
        if (observeList != null && UIManager.instance.observeTarget != observeTarget)
        {
            observeState stat = observeTarget.transform.gameObject.GetComponent<observeState>();
            isObserve = true;
            stat.isObserve = true;
            stat.RewnewObserveUI(true);
            p.ZoomIn(observeTarget, 3);
            p.FocusWith(observeTarget);
            p.ChangePlayerState(PlayerState.OBSERVE);
        }
    }

    public void EndObserve()
    {
        if (observeTarget != null)
        {
            observeState stat = observeTarget.transform.gameObject.GetComponent<observeState>();
            UIManager.instance.OnObserveIcon(null, observeList[0], false);
            DualSense.instance.ResetTrigger(false, true);
            p.EndFocus(observeTarget);
            p.ZoomOut();
            //observeTarget = null;
            isObserve = false;
            p.ChangePlayerState(PlayerState.NORMAL);
            stat.isObserve = false;
            UIManager.instance.OnObserveUI(false, "", "", "", null, null);
            observeList.Clear();
        }
    }
}

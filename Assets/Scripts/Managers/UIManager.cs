using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Rendering;
using UnityEngine.Timeline;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [Header("Player")]
    [SerializeField] GameObject player;        

    [Header("자잘이")]
    [SerializeField] GameObject fpsText;
    [SerializeField] GameObject climbUI;
    [SerializeField, Tooltip("Interaction")] GameObject Interation;        
    [SerializeField] GameObject addHerbTuto;

    [Header("")]
    [SerializeField] GameObject[] tutorialList;
    [SerializeField] bool[] tutorialWatchList;

    [Header("ItemEarn")]
    [SerializeField, Tooltip("PreFab")] GameObject itemEarn;
    [SerializeField] GameObject itemEarnParent;
    [SerializeField] GameObject itemEarnParticle;

    [Header("ItemUse")]
    [SerializeField] GameObject itemUseText;
    [SerializeField] GameObject itemUse;

    [Header("Inventory")]
    [SerializeField] GameObject inventoryItemL;
    [SerializeField] GameObject inventoryItemM;
    [SerializeField] GameObject inventoryItemR;
    [SerializeField] GameObject inventoryCountL;
    [SerializeField] GameObject inventoryCountM;

    [Header("Observe")]
    [SerializeField] List<GameObject> observeIconList = new List<GameObject>();
    [SerializeField] List<GameObject> observe = new List<GameObject>();
    [SerializeField] Transform observeIconParent;
    [SerializeField] GameObject[] observeEffectObject;
    [SerializeField] GameObject observeUI;
    [SerializeField] Sprite[] observeSprite;
    public GameObject observeTarget;

    [Header("MoveStuff")]
    [SerializeField] GameObject stuffName;

    [Header("Guide UI")]
    [SerializeField] private GuideUIform[] guideUI;
    [Tooltip("순서 u, d, l, r / y, a, x, b / L1, L2, R1, R2 / L, R")] public Sprite[] controllerGuiedUI;                            //순서 u, d, l, r / y, a, x, b / L1, L2, R1, R2 / L, R
    [Tooltip("순서 u, d, l, r / y, a, x, b / L1, L2, R1, R2 / L, R")] public Sprite[] keyboardMouseGuiedUI;    

    [Header("ItemStateList")]
    public Sprite[] itemImageList;
    public Sprite[] equipmentImageList;    
    [SerializeField] string[] itemNameList;

    [Header("Potion")]
    [SerializeField] GameObject potionGaugeParent;
    [SerializeField] GameObject potionGaugePreFab;

    [SerializeField] Sprite[] potionEffectImage;
    [SerializeField] string[] potionEffectText;

    [SerializeField] List<GameObject> potionGaugeObjectList = new List<GameObject>();
    [SerializeField] List<Coroutine> potionGaugeCoroutineList = new List<Coroutine>();
    //[SerializeField] List<Image> potionGaugeList = new List<Image>();    

    [Header("TimeLIneAsset")]
    [SerializeField] TimelineAsset potionUITimeline;
    [SerializeField] TimelineAsset potionUIOffTimeline;
    [SerializeField] TimelineAsset observeTimeline;

    private float deltaTime = 0.0f;

    public int interatPriority;

    [Header("GUIDE")]
    [SerializeField] GameObject[] watchGuideArray;
    [SerializeField] GameObject watchGuide;
    [SerializeField] int pageNum;

    Dictionary<string, GameObject[]> guideLIst = new Dictionary<string, GameObject[]>();
    [SerializeField] GameObject[] makePotionGuide;

    [Header("Travel")]
    [SerializeField] GameObject travelTutorial;
    [SerializeField] GameObject travelMap;
    [SerializeField] GameObject selectTravelStopUI;
    [SerializeField] GameObject[] travelStopUI;
    [SerializeField] bool isWatchTutorial;
    TrackingScript selectTracking;

    TextMeshProUGUI fpsTxt;
    TextMeshProUGUI stuffTxt;
    TextMeshProUGUI itemUseTxt;
    TextMeshProUGUI itemCountTextL;
    TextMeshProUGUI itemCountTextM;
    TextMeshProUGUI observeText1;
    TextMeshProUGUI observeText2;
    TextMeshProUGUI observeText3;
    ParticleSystem itemParticle;
    List<ParticleSystem> observeEffect = new List<ParticleSystem>();
    Image itemL;
    Image itemM;
    Image itemR;
    TrackingScript observeTracking;
    Player P;

    TrackingScript Interattracking;
    PlayableDirector timeline;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        
        fpsTxt = fpsText.GetComponent<TextMeshProUGUI>();
        if (fpsTxt == null )
        {
             Debug.Log("D");
        }

        stuffTxt = stuffName.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        itemParticle = itemEarnParticle.GetComponent<ParticleSystem>();
        Interattracking = Interation.GetComponent<TrackingScript>();
        itemL = inventoryItemL.GetComponent<Image>();
        itemM = inventoryItemM.GetComponent<Image>();
        itemR = inventoryItemR.GetComponent<Image>();
        itemCountTextL = inventoryCountL.GetComponent<TextMeshProUGUI>();
        itemCountTextM = inventoryCountM.GetComponent<TextMeshProUGUI>();
        timeline = GetComponent<PlayableDirector>();
        observeText1 = observeUI.transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>();
        observeText2 = observeUI.transform.GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>();
        observeText3 = observeUI.transform.GetChild(2).GetChild(1).GetComponent<TextMeshProUGUI>();
        observeTracking = observeUI.GetComponent<TrackingScript>();

        selectTracking = selectTravelStopUI.GetComponent<TrackingScript>();
        P = player.GetComponent<Player>();
        for (int i = 0; i < observeEffectObject.Length; i++)
        {            
            observeEffect.Add(observeEffectObject[i].GetComponent<ParticleSystem>());
        }
        itemUseTxt = itemUseText.GetComponent<TextMeshProUGUI>();

        //가이드 지정
        {
            guideLIst.Add("makePotionGuide", makePotionGuide);          //포션 가이드
        }
    }

    // Update is called once per frame
    void Update()
    {
        // 프레임 오차를 줄이기 위해 deltaTime 누적 계산
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;

        //if (Input.GetKeyDown(KeyCode.J))
        //{
        //    AddPotionGuage(0, 60);
        //}
    }

    void OnGUI()
    {
        int w = Screen.width, h = Screen.height;

        GUIStyle style = new GUIStyle();
        Rect rect = new Rect(20, 20, w, h * 2 / 100);

        style.alignment = TextAnchor.UpperLeft;
        style.fontSize = h * 3 / 100; // 해상도에 맞춘 글자 크기
        style.normal.textColor = Color.green; // 초록색 글씨

        float msec = deltaTime * 1000.0f;
        float fps = 1.0f / deltaTime;

        // 밀리초와 FPS를 동시에 표시
        string text = string.Format("{1:0.} fps ({0:0.0} ms)", msec, fps);

        fpsTxt.text = text;
        //GUI.Label(rect, text, style);
    }

    public void SetClimbUI(bool OnOff, bool leftalpha, bool rightalpha)
    {
        climbUI.SetActive(OnOff);
        if (leftalpha)
        {
            Image L2Image = climbUI.transform.GetChild(0).GetComponent<Image>();
            L2Image.color = new Vector4(1, 1, 1, 1);
        }else
        {
            Image L2Image = climbUI.transform.GetChild(0).GetComponent<Image>();
            L2Image.color = new Vector4(1, 1, 1, 0.5f);
        }

        if (rightalpha)
        {
            Image R2Image = climbUI.transform.GetChild(1).GetComponent<Image>();
            R2Image.color = new Vector4(1, 1, 1, 1);
        }
        else
        {
            Image R2Image = climbUI.transform.GetChild(1).GetComponent<Image>();
            R2Image.color = new Vector4(1, 1, 1, 0.5f);
        }
    }

    public void StuffNameControl(bool onoff, string name)
    {
        stuffTxt.text = name;
        stuffName.SetActive(onoff);        
    }

    public void InterationSet(bool onoff, int priority, Vector3 expos, Transform trackingtransform)
    {        
        if (onoff)
        {
            if (interatPriority <= priority)
            {
                interatPriority = priority;
                Interattracking.trackingobject = trackingtransform;
                Interattracking.exPos = expos;
                Interation.SetActive(onoff);
            }
        }else
        {
            interatPriority = 0;
            Interation.SetActive(onoff);
        }
    }

    public IEnumerator EarnItem(int itemCode, int itemCount)
    {        
        GameObject UI = Instantiate(itemEarn, itemEarnParent.transform);
        TextMeshProUGUI itemtxt = UI.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        Image itemimage = UI.transform.GetChild(1).GetComponentInChildren<Image>();
        itemtxt.text = $"{itemNameList[itemCode]} x{itemCount} 획득함";
        itemimage.sprite = itemImageList[itemCode];        
        UI.transform.SetAsFirstSibling();
        UI.SetActive(true);
        yield return new WaitForSeconds(2.3f);
        Destroy(UI);
        //itemparticle.Play();
    }

    public void Inventoryrenew(int itemCodeL, int itemCodeM, int itemCodeR, int itemCountL, int itemCountM)
    {
        int selectInventory = ItemManager.instance.selectInventoryType;        
        itemL.sprite = selectInventory == 0 ? itemImageList[itemCodeL] : equipmentImageList[itemCodeL];
        itemM.sprite = selectInventory == 0 ? itemImageList[itemCodeM] : equipmentImageList[itemCodeM];
        itemR.sprite = selectInventory == 0 ? itemImageList[itemCodeR] : equipmentImageList[itemCodeR];

        if (itemCountL == 0)
        {
            itemCountTextL.text = "";
        }
        else
        {
            itemCountTextL.text = selectInventory == 0 ? $"x{itemCountL}" : "";
        }

        if (itemCountM == 0)
        {
            itemCountTextM.text = "";            
        }
        else
        {
            itemCountTextM.text = selectInventory == 0 ? $"x{itemCountM}" : "";
        }        
    }

    public void KeyPress(GameObject pressob, bool tf)
    {   
        Debug.Log($"drop : {tf}");
        Animator a = pressob.GetComponent<Animator>();
        a.SetBool("ispress", tf);
    }

    public void ItemCanUse(GameObject holdItem)
    {
        if (holdItem != null && ItemManager.instance.selectInventoryType == 0)
        {
            ItemState item = holdItem.GetComponent<ItemState>();
            if (item.canUse || item.canUseSolo)
            {
                itemUse.SetActive(true);
            }
            else
            {
                itemUse.SetActive(false);
            }
        }else
        {
            itemUse.SetActive(false);
        }
    }

    public void ItemUsingText(bool canuse, int itemcode = 0)
    {
        itemUseText.SetActive(true);
        alphacontroller alpha = itemUseText.GetComponent<alphacontroller>();        
        if (canuse)                                                                 //사용함
        {
            itemUseTxt.text = $"{itemNameList[itemcode]}를 사용했습니다";
            //Debug.Log("D");
        }
        else                                                                       //사용못함
        {
            itemUseTxt.text = "상호작용할 물체가 없습니다";
        }
        alpha.AlphaUp();
    }

    public void InteractionText(int infor)                                                 //못쓴 사유 0번 : 잠겨있음
    {
        itemUseText.SetActive(true);
        alphacontroller alpha = itemUseText.GetComponent<alphacontroller>();
        alpha.AlphaUp();
        switch (infor)
        {
            case 0:
                itemUseTxt.text = "잠겨있습니다";
                break;
        }
    }

    public void PotionTimeline(bool onoff)
    {        
        timeline.playableAsset = onoff ? potionUITimeline : potionUIOffTimeline;
        timeline.Play();        
    }

    public void SetAddHerbTuto(bool addHerbOnoff)
    {
        addHerbTuto.SetActive(addHerbOnoff);
    }

    public void OnObserveUI(bool onOff, string infor1, string infor2, string infor3, GameObject target, float[] wait)
    {        
        //if (onOff) observeUI.transform.position = Camera.main.WorldToScreenPoint(target.transform.position);
        if (infor1 != "")
        {
            observeText1.text = infor1;
            observeUI.transform.GetChild(0).gameObject.SetActive(true);
            if (wait != null) StartCoroutine(EnableEffect(wait, 0));
        }
        else
        {
            observeUI.transform.GetChild(0).gameObject.SetActive(false);
        }

        if (infor2 != "")
        {
            observeText2.text = infor2;
            observeUI.transform.GetChild(1).gameObject.SetActive(true);
            if (wait != null) StartCoroutine(EnableEffect(wait, 1));
        }
        else
        {
            observeUI.transform.GetChild(1).gameObject.SetActive(false);
        }

        if (infor3 != "")
        {
            observeText3.text = infor3;
            observeUI.transform.GetChild(2).gameObject.SetActive(true);
            if (wait != null) StartCoroutine(EnableEffect(wait, 2));
        }
        else
        {
            observeUI.transform.GetChild(2).gameObject.SetActive(false);
        }

        if (observeTarget != null)
        {
            if (observeTarget != target)
            {
                timeline.playableAsset = observeTimeline;
                timeline.Play();
                //Debug.Log($"target : {observeTarget}");
            }            
        }else
        {
            timeline.playableAsset = observeTimeline;
            timeline.Play();
            //Debug.Log($"target : {observeTarget}");
        }
        
        observeTarget = target;
        if (observeTarget != null)
        {
            observeTracking.trackingobject = target.transform;
            observeTracking.exPos = target.GetComponent<observeState>().observePos;
        }        
        observeUI.SetActive(onOff);
    }

    public void ObserveText(string infor1, string infor2, string infor3)
    {
        observeText1.text = infor1;
        observeText2.text = infor2;
        observeText3.text = infor3;
    }

    IEnumerator EnableEffect(float[] time, int index)
    {        
        yield return new WaitForSeconds(time[index]);
        //Debug.Log("Effect Play");
        observeEffect[index].Play();
    }

    public void OnObserveIcon(List<GameObject> observeObjects, GameObject targetObject, bool onOff)
    {        
        if (observeObjects != null)
        {
            observe = observeObjects;
            if (observeObjects.Count > observeIconList.Count)                                                //현재 있는 observeIconList의 수가 주변에 감지된 관찰 가능 오브젝트(list)의 수보다 적을때
            {
                int supplementCount = observeObjects.Count - observeIconList.Count;
                for (int i = 0; i < supplementCount; i++)                       //observeiconList수 보충
                {
                    //Debug.Log("보충");
                    observeIconList.Add(Instantiate(observeIconList[0], observeIconParent));
                }                
            }

            for (int i = 0; i < observeObjects.Count; i++)                                                //observeIcon에 오브젝트 할당
            {
                if (onOff)
                {
                    TrackingScript tracking = observeIconList[i].GetComponent<TrackingScript>();
                    tracking.trackingobject = observeObjects[i].transform;
                    tracking.exPos = observeObjects[i].GetComponent<observeState>().observePos;
                }
                if (targetObject != null)
                {
                    //Debug.Log("now : " + observeObjects[i]);
                    //Debug.Log("target : " + targetObject);
                    if (targetObject == observeObjects[i])
                    {
                        observeIconList[i].GetComponent<Image>().sprite = observeSprite[1];
                    }
                    else
                    {
                        observeIconList[i].GetComponent<Image>().sprite = observeSprite[0];
                    }
                }
                observeIconList[i].SetActive(onOff);
            }
        }
        else
        {
            for (int i = 0; i < observeIconList.Count; i++)                                                    //observeIcon 전부 비활성화
            {
                observeIconList[i].SetActive(onOff);
            }
        }
    }

    public void ChangeGuideUI(bool iscontroller)                                //바뀐거 기준
    {
        if (iscontroller)
        {
            for (int i = 0;i < guideUI.Length; i++)                                 //순서 u, d, l, r / y, a, x, b / L1, L2, R1, R2 / L, R
            {
                Image UIImage = guideUI[i].GetComponent<Image>();
                GuideUIform UIform = guideUI[i].GetComponent<GuideUIform>();
                UIImage.sprite = UIform.controller;
            }
        }else
        {
            for (int i = 0; i < guideUI.Length; i++)                                 //순서 u, d, l, r / y, a, x, b / L1, L2, R1, R2 / L, R
            {
                Image UIImage = guideUI[i].GetComponent<Image>();
                GuideUIform UIform = guideUI[i].GetComponent<GuideUIform>();
                UIImage.sprite = UIform.keyboard;
            }
        }
    }

    public void AddPotionGuage(int potionNum, float duration, bool isOverlap, int potionLIstNumFromManager)
    {
        if (!isOverlap)
        {
            potionGaugeObjectList.Add(Instantiate(potionGaugePreFab, potionGaugeParent.transform));
            int potionListNum = potionGaugeObjectList.Count - 1;
            potionGaugeObjectList[potionListNum].GetComponentInChildren<Image>().sprite = potionEffectImage[potionNum];
            potionGaugeObjectList[potionListNum].GetComponentInChildren<TextMeshProUGUI>().text = potionEffectText[potionNum];
            potionGaugeCoroutineList.Add(StartCoroutine(RewnewPotionGuage(potionListNum, duration)));
        }else
        {
            StopCoroutine(potionGaugeCoroutineList[potionLIstNumFromManager]);
            potionGaugeCoroutineList[potionLIstNumFromManager] = StartCoroutine(RewnewPotionGuage(potionLIstNumFromManager, duration));
        }
        //potionGaugeList.Add(potionGaugeObjectList[potionListNum].transform.GetChild(2).transform.GetChild(0).GetComponentInChildren<Image>());        
    }

    public void RemovePotionGuage(int potionListNum)
    {
        Destroy(potionGaugeObjectList[potionListNum]);
        StopCoroutine(potionGaugeCoroutineList[potionListNum]);
        potionGaugeObjectList.RemoveAt(potionListNum);
        potionGaugeCoroutineList.RemoveAt(potionListNum);
    }

    IEnumerator RewnewPotionGuage(int potionListNum, float duration)
    {
        float timer = duration;

        Transform gauge = potionGaugeObjectList[potionListNum].transform.GetChild(2).transform.GetChild(0).transform.GetChild(0);

        while (timer > 0)
        {
            timer -= Time.deltaTime;

            gauge.localScale = new Vector2(timer / duration, 1);

            yield return null;
        }
    }

    public void GuideOn(string guideType)
    {
        watchGuideArray = guideLIst[guideType];
        watchGuide = watchGuideArray[0];
        watchGuide.transform.parent.gameObject.SetActive(true);
        watchGuide.SetActive(true);
        pageNum = 0;
        P.ChangePlayerState(PlayerState.READGUIDE);
    }

    public void ChangePage(int value)
    {
        if (watchGuideArray.Length > 0)
        {
            if (value < 0)                                                      //value가 -일때(전페이지)
            {
                if (pageNum != 0)
                {
                    Debug.Log("previous Page");
                    watchGuideArray[pageNum].SetActive(false);
                    pageNum -= 1;
                    watchGuide = watchGuideArray[pageNum];
                    watchGuideArray[pageNum].SetActive(true);
                }
            }
            else                                                                //value가 +일때(다음페이지)
            {
                if (pageNum < watchGuideArray.Length - 1)
                {
                    Debug.Log("next Page");
                    watchGuideArray[pageNum].SetActive(false);
                    pageNum += 1;
                    watchGuide = watchGuideArray[pageNum];
                    watchGuideArray[pageNum].SetActive(true);
                }
            }
        }
    }

    public void GuideOff()
    {
        watchGuide.transform.parent.gameObject.SetActive(false);
        watchGuideArray[pageNum].SetActive(false);
        watchGuide = null;
        watchGuideArray = null;
        pageNum = 0;
        P.ChangePlayerState(PlayerState.NORMAL);
    }

    public void TutorialOn(int tutorialNum)
    {
        if (!tutorialWatchList[tutorialNum])
        {
            tutorialList[tutorialNum].SetActive(true);
        }
    }

    public void TutorialOff(int tutorialNum)
    {
        tutorialList[tutorialNum].SetActive(false);
    }

    public void TravelMapUIOnOff(bool value)
    {
        travelMap.SetActive(value);
    }

    public void ChangeTravelStopUI(int stopNum)
    {
        selectTracking.ChangeTrackingTarget(travelStopUI[stopNum]);        
    }
}
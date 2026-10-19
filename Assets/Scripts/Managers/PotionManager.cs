using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Rendering.Universal;
using UnityEngine.U2D.IK;
using UnityEngine.Events;
using UnityEngine.Timeline;
using UnityEngine.Playables;

public class PotionManager : MonoBehaviour
{
    public static PotionManager instance;
    
    [SerializeField] GameObject player;
    [SerializeField] GameObject cauldronItem;
    [SerializeField] GameObject cauldronFire;
    [SerializeField] GameObject dust;
    [SerializeField] GameObject endDust;    
    [SerializeField] GameObject splash;
    [SerializeField] GameObject potionStirEffectPar;

    public List<int> drankPotion = new List<int>();

    [Header("MakePotion")]
    [SerializeField] string[] recipeList;                                           //u위 화살표, d아래 화살표, l왼쪽 화살표, r오른쪽 화살표, (편의를 위해 xbox기준으로 키 적음(x, y, a, b))
    [SerializeField] int[] fireStrength;                                            //불 온도 0약불, 1중불, 2강불
    [SerializeField] GameObject[] herbRecipeList;
    [SerializeField] GameObject[] potionList;                                       //다 만들고 나오는 포션    

    public List<GameObject> herbList = new List<GameObject>();                                                                            //들어간 허브 리스트
    public string recipe;
    [SerializeField] GameObject[] herbRecipe;                                                       //포션 만드는데 들어가는 허브 레시피
    public int recipeOrder;                                                                         //몇번째 레시피인지 확인
    [SerializeField] int recipeKeyOrder;                                                            //몇번째 조리 과정인지 확인
    public bool startMaking;
    public bool addHerb;
    public bool canStir;
    [SerializeField] UnityEvent stopMakePotionEvent;

    [SerializeField] GameObject potion;
    [SerializeField] GameObject potionLid;
    [SerializeField] GameObject potionChangeLight;

    [SerializeField] Vector4 potionColor;
    [SerializeField] Vector3[] potionColorList;    
    [SerializeField] Vector3[] lightColor;

    [SerializeField] TimelineAsset makePotionEnd;

    Coroutine remove0;
    Coroutine remove1;

    Player p;
    Climb pclimb;
    ItemUse itemUse;
    InteractScript interact;
    Light2D light;
    Light2D potionLight;    
    alphacontroller alpha;
    ParticleSystem dustPar;
    ParticleSystem endDustPar;
    ParticleSystem bigDustPar;
    ParticleSystem splashPar;
    SpriteRenderer potionSpr;
    Animator Panim;
    PlayableDirector timeline;
    Fire fire;


    void Awake()
    {        
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        p = player.GetComponent<Player>();
        pclimb = p.GetComponent<Climb>();
        itemUse = cauldronItem.GetComponent<ItemUse>();
        light = cauldronItem.transform.GetChild(0).GetComponent<Light2D>();
        potionLight = potionChangeLight.GetComponent<Light2D>();        
        alpha = cauldronItem.transform.GetChild(0).GetComponent<alphacontroller>();
        interact = cauldronItem.GetComponent<InteractScript>();
        dustPar = dust.GetComponent<ParticleSystem>();
        endDustPar = endDust.GetComponent<ParticleSystem>();
        fire = cauldronFire.GetComponent<Fire>();
        splashPar = splash.GetComponent<ParticleSystem>();
        potionSpr = potion.GetComponent<SpriteRenderer>();
        Panim = player.GetComponentInChildren<Animator>();
        timeline = GetComponent<PlayableDirector>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void DrinkPotion(GameObject potion)
    {
        Debug.Log("drink");
        ItemState item = potion.GetComponent<ItemState>();
        float potiondur = item.potionDuration;
        int potionnum = item.potionCode;
        bool already = false;

        for (int i = 0; i < drankPotion.Count; i++)                             //한번 더 먹어서 시간 초기화
        {
            if (drankPotion[i] == potionnum)
            {
                Debug.Log("중복");
                already = true;                
                UIManager.instance.AddPotionGuage(potionnum, potiondur, true, i);
                switch (potionnum)
                {
                    case 0:
                        if (remove0 != null)
                        {
                            StopCoroutine(remove0);
                        }
                        remove0 = StartCoroutine(RemovePotion(potiondur, potionnum));
                        break;

                    case 1:
                        if (remove1 != null)
                        {
                            StopCoroutine(remove1);
                        }
                        remove1 = StartCoroutine(RemovePotion(potiondur, potionnum));
                        break;
                }
            }
        }
        
        if (!already)                                                           //아직 안먹었을때
        {
            Debug.Log("초회차");
            drankPotion.Add(potionnum);
            UIManager.instance.AddPotionGuage(potionnum, potiondur, false, 0);
            switch (potionnum)
            {
                case 0 : 
                    if (remove0 != null)
                    {
                        StopCoroutine(remove0);
                    }
                    remove0 = StartCoroutine(RemovePotion(potiondur, potionnum));
                    break;

                case 1 :
                    if (remove1 != null)
                    {
                        StopCoroutine(remove1);
                    }
                    remove1 = StartCoroutine(RemovePotion(potiondur, potionnum));
                    break;
            }
        }

        switch (potionnum)                                                      //포션 효과 설정
        {
            case 0:
                pclimb.canclimb = true;
                break;

            case 1:
                break;
        }
    }

    IEnumerator RemovePotion(float potiondur, int potionnum)
    {
        yield return new WaitForSeconds(potiondur);
        for (int i = 0; i < drankPotion.Count; i++)
        {
            if (drankPotion[i] == potionnum)
            {
                drankPotion.RemoveAt(i);
            }
        }

        switch (potionnum)                                              //효과 리셋
        {
            case 0:
                remove0 = null;
                pclimb.canclimb = false;
                UIManager.instance.RemovePotionGuage(potionnum);
                if (p.currentState == PlayerState.CLIMB) StartCoroutine(pclimb.ClimbSlip(true));
                break;

            case 1:
                remove1= null;
                break;
        }
    }

    //물약 제작

    public void Startmakepotion(int recipenum)
    {
        recipeOrder = recipenum;
        recipeKeyOrder = 0;
        recipe = "";
        herbList.Clear();
        dustPar.Play();
        startMaking = true;        
        itemUse.canuse = true;
        addHerb = true;

        //pcom.Panim.SetTrigger("MakePotion");
        p.Prb.linearVelocity = Vector3.zero;        
        //pcom.cauldronPosition = new Vector3(cauldronItem.transform.position.x, p.transform.position.y, 0);
        p.Prb.gravityScale = 0;        
        interact.caninteract = false;
        p.AutoMove(cauldronItem.transform.position + Vector3.up * 0.42f);
        p.ChangePlayerState(PlayerState.MAKEPOTION);
        p.ZoomIn(null, 4.5f);
        //potion.GetComponent<SpriteRenderer>().enabled = true;
        UIManager.instance.SetAddHerbTuto(true);
        UIManager.instance.PotionTimeline(true);
    }

    public void StopMakePotion(bool isCompletePotion = false)
    {
        if (startMaking)
        {            
            if (!isCompletePotion)
            {
                p.ZoomOut();
                herbList.Clear();
                dustPar.Stop();
                addHerb = true;
                canStir = true;
                itemUse.canuse = false;
                startMaking = false;                

                p.Prb.gravityScale = 3;
                interact.caninteract = true;
                p.ChangePlayerState(PlayerState.NORMAL);
                UIManager.instance.PotionTimeline(false);
                //potion.GetComponent<SpriteRenderer>().enabled = false;                                
                stopMakePotionEvent.Invoke();
                p.MakePotionEnd();
            }
            else
            {
                StartCoroutine(PotionMakeScene());
            }
        }        
    }

    IEnumerator PotionMakeScene()
    {        
        canStir = false;        
        StartCoroutine(CamShake.instance.Shake(0.2f, 0.2f, 0.025f, 80, 2));
        DualSense.instance.Hapticvoid(0.1f, 0.1f, 2, 3, 3);
        yield return new WaitForSeconds(1);
        p.MakePotionEnd();
        stopMakePotionEvent.Invoke();
        dustPar.Stop();
        yield return new WaitForSeconds(1);
        p.ChangePlayerState(PlayerState.PLAYTIMELINE);
        timeline.playableAsset = makePotionEnd;
        timeline.Play();
        p.ZoomIn(null, 1);                
        yield return new WaitForSeconds(9.4f);        

        herbList.Clear();        
        addHerb = true;
        itemUse.canuse = false;
        startMaking = false;        

        p.Prb.gravityScale = 3;
        interact.caninteract = true;
        p.ChangePlayerState(PlayerState.NORMAL);
        p.ZoomOut();
        UIManager.instance.PotionTimeline(false);
        //potion.GetComponent<SpriteRenderer>().enabled = false;
        ItemManager.instance.ItemAdd(Instantiate(potionList[recipeOrder - 1]));        
    }

    public void PotionColorChange()
    {
        int randomPotionColorIndex = Random.Range(0, potionColorList.Length);                                           //포션 색 변경
        Debug.Log(randomPotionColorIndex);
        potionColor = new Vector4(potionColorList[randomPotionColorIndex].x / 256, potionColorList[randomPotionColorIndex].y / 256, potionColorList[randomPotionColorIndex].z / 256, 1);
        potionLight.color = potionColor;
        potionSpr.color = potionColor;
    }

    public void AddHerb()
    {
        if (addHerb)
        {
            herbList.Add(Instantiate(ItemManager.instance.itemList[ItemManager.instance.selectSlot]));
            ItemState state = null;
            for (int i = 0; i < herbRecipeList.Length; i++)
            {
                if (herbRecipeList[i].transform.childCount == herbList.Count)
                {
                    int recipeCorrect = 0;
                    for (int j = 0; j < herbRecipeList[i].transform.childCount; j++)
                    {
                        ItemState checkState = herbRecipeList[i].transform.GetChild(j).GetComponent<ItemState>();
                        state = herbList[j].GetComponent<ItemState>();
                        if (checkState.itemCode == state.itemCode) recipeCorrect++;                        
                    }                    
                    if (recipeCorrect == herbRecipeList[i].transform.childCount)
                    {
                        if (fireStrength[i + 1] == fire.fireStrength)
                        {
                            recipeOrder = i + 1;
                        }                        
                        break;
                    }
                    else
                    {
                        recipeOrder = 0;                        
                    }
                }
                else
                {
                    recipeOrder = 0;
                }
            }

            GameObject throwObject = Instantiate(ItemManager.instance.itemList[ItemManager.instance.selectSlot]);
            Destroy(throwObject, 3);
            InteractScript interact = throwObject.GetComponent<InteractScript>();
            interact.caninteract = false;
            throwObject.transform.GetChild(0).gameObject.SetActive(false);
            throwObject.transform.position = p.transform.position;
            throwObject.SetActive(true);
            ItemState throwItem = throwObject.GetComponent<ItemState>();
            throwObject.GetComponent<Animator>().enabled = false;
            throwObject.GetComponent<SpriteRenderer>().sprite = UIManager.instance.itemImageList[throwItem.itemCode];
            Rigidbody2D throwrb = throwObject.AddComponent<Rigidbody2D>();
            throwrb.gravityScale = 3;
            throwrb.AddForceY(8, ForceMode2D.Impulse);
        }
    }

    public void AddHerbEnd()
    {
        StartCoroutine(AddHerbEndCoroutine());
    }

    IEnumerator AddHerbEndCoroutine()
    {
        yield return null;
        if (startMaking)
        {
            itemUse.canuse = false;
            addHerb = false;

            if (herbList.Count > 0 && recipeOrder != 0) recipe = recipeList[recipeOrder];
            UIManager.instance.SetAddHerbTuto(false);
            p.MakePotionAnim(false, 4);
        }
    }

    public void Makepotion(char inputkey)
    {                
        if (startMaking && !addHerb && canStir)
        {            
            if (inputkey == 'x' || inputkey == 'l')
            {
                PotionStirEffectLR(true);                
            }
            else if (inputkey == 'b' || inputkey == 'r')
            {
                PotionStirEffectLR(false);
            }

            switch (inputkey)
            {
                case 'x':
                    p.MakePotionAnim(false, 2);
                    break;

                case 'b':
                    p.MakePotionAnim(false, 3);
                    break;


                case 'l':
                    p.MakePotionAnim(true, 3);
                    break;

                case 'r':
                    p.MakePotionAnim(true, 2);
                    break;
            }

            if (recipe != "")
            {
                if (inputkey == recipe[recipeKeyOrder]) recipeKeyOrder += 1;

                if (recipe.Length == recipeKeyOrder)
                {
                    StopMakePotion(true);
                }
            }
        }
    }

    void PotionStirEffectLR(bool isLeft)
    {
        GameObject effect = GameObjectPoolManager.instance.GetPoolObject(PoolType.potionStirLR);
        effect.transform.parent = potionStirEffectPar.transform;
        effect.transform.localPosition = Vector3.zero;
        effect.transform.localScale = new Vector3(1, 1.1f, 1);
        SpriteRenderer effectSpr = effect.GetComponent<SpriteRenderer>();
        effectSpr.flipX = isLeft;
        effectSpr.color = potionColor;
        GameObjectPoolManager.instance.ReturnPoolObject(effect, PoolType.potionStirLR, 0.6f);
    }

    void PotionStirEffectFB(bool isLeft)
    {
        GameObject effect = GameObjectPoolManager.instance.GetPoolObject(PoolType.potionStirFB);
        effect.transform.parent = potionStirEffectPar.transform;
        effect.transform.localPosition = Vector3.zero;
        effect.transform.localScale = new Vector3(1, 1.1f, 1);
        SpriteRenderer effectSpr = effect.GetComponent<SpriteRenderer>();
        effectSpr.flipX = isLeft;
        effectSpr.color = potionColor;
        GameObjectPoolManager.instance.ReturnPoolObject(effect, PoolType.potionStirFB, 0.5f);
    }
}

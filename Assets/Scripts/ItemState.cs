using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public enum ITEMTYPE
{
    BASIC,                                              //기본
    HERB,                                               //약재
    POTION                                              //포션
}

public class ItemState : MonoBehaviour
{
    [SerializeField, Tooltip("itemType이 HERB일때에만")] float herbGrowTime;

    public int itemCode;
    public int itemCount;

    public bool canpick;
    public bool canoverlap;
    public bool canUse;
    public bool canUseSolo;    

    public ITEMTYPE itemType;

    [SerializeField] Sprite[] herbstatesprite;

    public UnityEvent itemUseEvent;

    [Header("Potion")]
    public int potionCode;                                      //포션일때만 씀
    public float potionDuration;

    SpriteRenderer spr;
    InteractScript intS;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        spr = GetComponent<SpriteRenderer>();
        intS = GetComponent<InteractScript>();
    }    

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeSprite(Sprite c)
    {
        if (canpick && itemType == ITEMTYPE.HERB)
        {
            spr.sprite = c;
        }
    }

    public IEnumerator Grow()                                           //허브류에만 사용
    {        
        spr.sprite = herbstatesprite[1];
        canpick = false;
        intS.caninteract = false;
        yield return new WaitForSeconds(herbGrowTime);
        spr.sprite = herbstatesprite[0];
        canpick = true;
        intS.caninteract = true;
    }
}

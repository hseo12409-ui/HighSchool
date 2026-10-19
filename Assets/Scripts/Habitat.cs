using System.Collections;
using UnityEngine;

public class Habitat : MonoBehaviour
{
    public int herbCode;                                  //자라는 허브 종류
    public int herbCount;                                 //자라는 허브 갯수

    public float herbGrowTime;                            //자라는 속도

    public bool canpick = true;

    [SerializeField] Sprite[] herbStateSprite;                      //0번은 평소 상태, 1번은 채집하고서 자라기까지의 모습
    [SerializeField] Sprite outlineSprite;

    public GameObject ItemPreFab;

    SpriteRenderer spr;
    InteractScript interact;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        spr = GetComponent<SpriteRenderer>();
        interact = GetComponent<InteractScript>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetOutline(bool isin)
    {
        if (canpick) spr.sprite = isin ? outlineSprite : herbStateSprite[0];
    }

    public void ColletHerb()
    {
        spr.sprite = herbStateSprite[1];
        canpick = false;
        interact.caninteract = false;
        StartCoroutine(GrowHerb());
    }

    IEnumerator GrowHerb()
    {
        yield return new WaitForSeconds(herbGrowTime);
        spr.sprite = herbStateSprite[0];
        canpick = true;
        interact.caninteract = true;
    }
}

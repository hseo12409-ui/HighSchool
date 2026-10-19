using System.Collections.Generic;
using UnityEngine;

public class large_motar : MonoBehaviour
{
    [SerializeField] int leftSpace = 5;
    [SerializeField] int needNumberOfCrush = 3;
    [SerializeField] int numberOfCrush;
    [SerializeField] int[] herbList;
    [SerializeField] int[] toChangeHerbList;    

    [SerializeField] Sprite[] herbSpriteList;
    [SerializeField] Sprite[] toChangeHerbSpriteList;

    [SerializeField] GameObject[] placeHerbList;
    [SerializeField] GameObject[] herbColliderList;
    [SerializeField] GameObject[] herbCrushParticle;

    [SerializeField] GameObject[] toChangeHerbColliderList;

    [SerializeField] Transform[] herbPos;

    [SerializeField] LayerMask motarFloorLayer;
    [SerializeField] LayerMask itemLayer;

    Rigidbody2D herbRb;
    PolygonCollider2D herbCollider;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            Crush();
        }
    }

    public void Interact()
    {
        if (leftSpace == 0)                                                     //자리가 안 남았을때
        {
            CollectHerb();
        }else                                                                   //자리가 남았을 때
        {
            AddHerb();
        }
    }

    public void AddHerb()
    {
        numberOfCrush = 0;
        bool isAddHerb = false;        
        for (int i = 0; i < herbList.Length; i++)
        {
            GameObject holdItem = ItemManager.instance.itemList[ItemManager.instance.selectSlot];
            if (holdItem != null)
            {
                ItemState itemState = holdItem.GetComponent<ItemState>();
                if (itemState.itemCode == herbList[i])
                {
                    if (itemState.itemCount == 1)
                    {
                        ItemManager.instance.itemList[ItemManager.instance.selectSlot] = null;
                    }
                    else
                    {
                        itemState.itemCount--;
                        holdItem = Instantiate(holdItem);
                        itemState = holdItem.GetComponent<ItemState>();
                        itemState.itemCount = 1;
                    }
                    SpriteRenderer spr = holdItem.GetComponent<SpriteRenderer>();
                    spr.sprite = herbSpriteList[i];
                    spr.sortingOrder = 10;
                    holdItem.GetComponent<Animator>().enabled = false;
                    placeHerbList[5 - leftSpace] = holdItem;
                    holdItem.transform.position = herbPos[5 - leftSpace--].position;
                    holdItem.SetActive(true);
                    holdItem.transform.GetChild(0).gameObject.SetActive(false);
                    ItemManager.instance.SlotChange(0);
                    isAddHerb = true;
                    holdItem.GetComponent<BoxCollider2D>().enabled = false;

                    herbCollider = herbColliderList[i].GetComponent<PolygonCollider2D>();
                    PolygonCollider2D itemC = holdItem.AddComponent<PolygonCollider2D>();
                    itemC.excludeLayers = herbCollider.excludeLayers;
                    itemC.forceReceiveLayers = herbCollider.forceReceiveLayers;
                    itemC.forceSendLayers = herbCollider.forceSendLayers;
                    itemC.points = herbCollider.points;

                    herbRb = herbColliderList[i].GetComponent<Rigidbody2D>();
                    Rigidbody2D itemR = holdItem.AddComponent<Rigidbody2D>();
                    itemR.excludeLayers = herbRb.excludeLayers;
                    break;
                }
            }
        }
        if (!isAddHerb)
        {
            CollectHerb();
        }
    }

    public void CollectHerb()
    {
        numberOfCrush = 0;
        List<GameObject> herbs = new List<GameObject>();
        for (int i = 0; i < placeHerbList.Length; i++)
        {
            if (placeHerbList[i] != null)
            {
                GameObject herb = placeHerbList[i];
                ItemState herbState = herb.GetComponent<ItemState>();
                herbs.Add(placeHerbList[i]);
                for (int j = 0; j < herbs.Count; j++)
                {
                    ItemState checkState = herbs[j].GetComponent<ItemState>();
                    if (herbs[j] != herb && herbState.itemCode == checkState.itemCode)
                    {
                        Destroy(herb);
                        herbs.RemoveAt(herbs.Count - 1);
                        checkState.itemCount++;
                    }
                }
                leftSpace++;
            }
        }

        for (int i = 0; i < herbs.Count; i++)
        {
            Destroy(herbs[i].GetComponent<PolygonCollider2D>());
            Destroy(herbs[i].GetComponent<Rigidbody2D>());            
            herbs[i].transform.GetChild(0).gameObject.SetActive(true);
            herbs[i].GetComponent<BoxCollider2D>().enabled = true;
            herbs[i].GetComponent<SpriteRenderer>().sortingOrder = 13;
            herbs[i].GetComponent<Animator>().enabled = true;
            herbs[i].transform.rotation = Quaternion.Euler(0, 0, 0);
            ItemManager.instance.ItemAdd(herbs[i]);
            placeHerbList[i] = null;
        }
    }

    public void Crush()
    {
        for (int i = 0; i < placeHerbList.Length; i++)
        {
            if (placeHerbList[i] != null)
            {                
                ItemState checkstate = placeHerbList[i].GetComponent<ItemState>();
                for (int j = 0; j < herbList.Length; j++)
                {
                    if (herbList[j] == checkstate.itemCode)
                    {                        
                        checkstate.itemCode = toChangeHerbList[j];
                        SpriteRenderer spr = placeHerbList[i].GetComponent<SpriteRenderer>();
                        spr.sprite = toChangeHerbSpriteList[j];

                        herbCollider = toChangeHerbColliderList[j].GetComponent<PolygonCollider2D>();
                        PolygonCollider2D itemC = placeHerbList[i].AddComponent<PolygonCollider2D>();
                        itemC.excludeLayers = herbCollider.excludeLayers;
                        itemC.forceReceiveLayers = herbCollider.forceReceiveLayers;
                        itemC.forceSendLayers = herbCollider.forceSendLayers;
                        itemC.points = herbCollider.points;
                        break;
                    }
                }
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //DualSense.instance.Hapticvoid(0.5f, 0.5f, 0.1f, 4, 4);
        if (collision.collider.CompareTag("largeMotar"))
        {            
            RaycastHit2D downCircle = Physics2D.CircleCast(collision.transform.GetChild(0).position, 0.2f, Vector2.zero, 0, itemLayer);
            RaycastHit2D upCircle = Physics2D.CircleCast(collision.transform.GetChild(1).position, 0.2f, Vector2.zero, 0, itemLayer);

            StartCoroutine(CamShake.instance.Shake(0.1f, 0.3f, 0.05f, 3, 1));
            DualSense.instance.Hapticvoid(0.5f, 0.5f, 0.1f, 4, 4);                          //항상 똑같음
            if (upCircle || downCircle)
            {                                
                if (upCircle)
                {                    
                    if (placeHerbList[0] != null)
                    {
                        numberOfCrush += 1;
                        Instantiate(herbCrushParticle[0], placeHerbList[0].transform.position, Quaternion.Euler(-90, 0, 0));
                        if (numberOfCrush >= needNumberOfCrush)
                        {
                            numberOfCrush = 0;
                            Instantiate(herbCrushParticle[0], placeHerbList[0].transform.position, Quaternion.Euler(-90, 0, 0));
                            Instantiate(herbCrushParticle[0], placeHerbList[0].transform.position, Quaternion.Euler(-90, 0, 0));
                            DualSense.instance.Hapticvoid(1, 1, 0.4f, 5, 5);
                            Crush();
                        }
                    }
                }else
                {                    
                    if (placeHerbList[0] != null)
                    {
                        numberOfCrush += 1;
                        Instantiate(herbCrushParticle[0], placeHerbList[0].transform.position, Quaternion.Euler(-90, 0, 0));
                        if (numberOfCrush >= needNumberOfCrush)
                        {
                            numberOfCrush = 0;
                            Instantiate(herbCrushParticle[0], placeHerbList[0].transform.position, Quaternion.Euler(-90, 0, 0));
                            Instantiate(herbCrushParticle[0], placeHerbList[0].transform.position, Quaternion.Euler(-90, 0, 0));
                            DualSense.instance.Hapticvoid(1, 1, 0.4f, 5, 5);
                            Crush();
                        }
                    }
                }
            }            
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        
    }
}

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public class setItemDropPosition : MonoBehaviour
{
    Player p;

    [SerializeField] bool changeItem;
    [SerializeField] bool lighton;

    [SerializeField] Transform[] dropPositionList;
    [SerializeField] GameObject[] fillItemList;
    [SerializeField] List<bool> dropPositionFillList = new List<bool>();

    [SerializeField] int[] changeItemList;
    [SerializeField] Sprite[] changeSprite;
    [SerializeField] Vector3[] changeCollideroffset;
    [SerializeField] Vector3[] changeCollidersize;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        for (int i = 0; i < dropPositionList.Length; i++)
        {
            dropPositionFillList.Add(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            p = collision.GetComponent<Player>();

            for (int i = 0; i < dropPositionList.Length; i++)
            {
                if (!dropPositionFillList[i])
                {
                    p.itemDropPosition = dropPositionList[i].position;
                    //dropPositionFillList[i] = true;
                    break;
                }
            }
        }
        
        if (collision.gameObject.CompareTag("Item"))
        {
            Debug.Log(collision.name);
            ItemState item = collision.GetComponent<ItemState>();
            for (int i = 0; i < changeItemList.Length; i++)
            {
                if (item.itemCode == changeItemList[i])
                {
                    Debug.Log("ChangeSprite");
                    Animator anim = collision.GetComponent<Animator>();
                    anim.enabled = false;
                    SpriteRenderer spr = collision.GetComponent<SpriteRenderer>();
                    spr.sprite = changeSprite[i];
                }
            }

            for (int i = 0; i < dropPositionList.Length; i++)
            {
                if (!dropPositionFillList[i])
                {
                    Debug.Log(i);
                    dropPositionFillList[i] = true;
                    fillItemList[i] = collision.gameObject;
                    collision.transform.GetChild(0).gameObject.SetActive(lighton);
                    if (dropPositionList.Length > i + 1)
                    {
                        p.itemDropPosition = dropPositionList[i + 1].position;
                    }
                    break;
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            p.itemDropPosition = Vector3.zero;
        }

        if (collision.gameObject.CompareTag("Item"))
        {
            for (int i = 0; i < fillItemList.Length; i++)
            {
                if (fillItemList[i] == null)
                {
                    dropPositionFillList[i] = false;                    
                    break;
                }
            }
        }
    }
}

using UnityEngine;
using UnityEngine.Events;

public class ItemUse : MonoBehaviour
{
    public int needItem;

    public bool canuse;

    public UnityEvent itemUseEvent;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        itemUseEvent.AddListener(() => ItemManager.instance.ItemUse());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CanUseBoolSet(bool usebool)
    {
        canuse = usebool;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (canuse)
            {
                ItemManager.instance.RenewItemCanUse(true, needItem);
            }else
            {
                ItemManager.instance.RenewItemCanUse(false);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {            
            ItemManager.instance.RenewItemCanUse(false);
        }
    }
}

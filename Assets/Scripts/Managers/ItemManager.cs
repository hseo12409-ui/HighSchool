using Steamworks;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public static ItemManager instance;

    [SerializeField] GameObject P;
    [SerializeField] GameObject itemPreFab;
    [SerializeField] GameObject itemDropPress;

    //public List<int> itemlist = new List<int>();
    public GameObject[] itemList;
    public int[] equipmentList;

    //public int[] itemList;                                      //6개
    public int selectInventoryType = 0;                           //0은 아이템 인벤토리 1은 장비 인벤토리
    public int selectSlot;
    public int selectEquipmentSlot;

    [SerializeField] LayerMask ground;

    ItemState Itemstate;
    InteractScript interact;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    private void Start()
    {
        SlotChange(0);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SlotTypeChange()
    {
        selectInventoryType = selectInventoryType == 0 ? 1 : 0;
        SlotChange(0);
    }

    public void ItemAdd(GameObject item)
    {
        bool isEarn = false;
        ItemState itemstate = item.GetComponent<ItemState>();
        int itemCode = itemstate.itemCode;
        int itemCount = itemstate.itemCount;
        for (int i = 0; i < itemList.Length; i++)
        {
            if (itemList[i] != null)
            {
                ItemState checkitem = itemList[i].GetComponent<ItemState>();

                if ((checkitem.itemCode == itemCode && itemstate.canoverlap) && itemstate.canpick)
                {
                    isEarn = true;
                    if ((checkitem.itemCode == itemCode && itemstate.canoverlap))
                    {
                        Debug.Log("같은 아이템(겹침)");
                        Destroy(item);
                        checkitem.itemCount += itemCount;
                    }else
                    {
                        itemList[i] = item;
                        itemList[i].SetActive(false);
                    }
                    //itemCountList[i] += itemCount;
                    //itemCanOverlap[i] = itemstate.canoverlap;
                    StartCoroutine(UIManager.instance.EarnItem(itemCode, itemCount));
                    SlotChange(0);                    
                    break;
                }
            }
        }

        if (!isEarn)
        {
            for (int i = 0; i < itemList.Length; i++)
            {
                if (itemList[i] == null)
                {
                    itemList[i] = item;
                    itemList[i].SetActive(false);
                    StartCoroutine(UIManager.instance.EarnItem(itemCode, itemCount));
                    SlotChange(0);
                    break;
                }
            }
        }
    }

    public void HerbAdd(GameObject herbHabitat)
    {
        bool isEarn = false;
        Habitat herbstate = herbHabitat.GetComponent<Habitat>();        
        if (herbstate.canpick)
        {
            herbstate.ColletHerb();
            GameObject herb = Instantiate(herbstate.ItemPreFab);
            herb.SetActive(false);
            InteractScript interact = herb.GetComponent<InteractScript>();
            int itemCode = herbstate.herbCode;
            int itemCount = herbstate.herbCount;
            for (int i = 0; i < itemList.Length; i++)
            {
                if (itemList[i] != null)
                {
                    ItemState checkitem = itemList[i].GetComponent<ItemState>();

                    if ((checkitem.itemCode == itemCode))
                    {
                        isEarn = true;
                        Debug.Log("같은 아이템(겹침)");
                        Destroy(herb);
                        checkitem.itemCount += itemCount;
                        //itemCountList[i] += itemCount;
                        //itemCanOverlap[i] = itemstate.canoverlap;
                        StartCoroutine(UIManager.instance.EarnItem(itemCode, itemCount));
                        SlotChange(0);
                        break;
                    }
                }
            }

            if (!isEarn)
            {
                for (int i = 0; i < itemList.Length; i++)
                {
                    if (itemList[i] == null)
                    {
                        itemList[i] = herb;
                        interact = herb.GetComponent<InteractScript>();
                        interact.interactEvent.AddListener(() => ItemAdd(herb));
                        Debug.Log("AddListener");
                        StartCoroutine(UIManager.instance.EarnItem(itemCode, itemCount));
                        SlotChange(0);
                        break;
                    }
                }
            }
        }
    }

    public void SlotChange(int lr)                                                                          //매개변수 0으로 하면 슬롯 갱신
    {
        if (selectInventoryType == 0)
        {
            if (lr < 0)                                                         //-1일때 왼쪽
            {
                if (selectSlot == 0)
                {
                    selectSlot = 5;
                }
                else
                {
                    selectSlot -= 1;
                }
            }
            else if (lr > 0)
            {
                if (selectSlot == 5)
                {
                    selectSlot = 0;
                }
                else
                {
                    selectSlot += 1;
                }
            }
        }else
        {
            if (lr < 0)                                                         //-1일때 왼쪽
            {
                if (selectEquipmentSlot == 0)
                {
                    selectEquipmentSlot = 5;
                }
                else
                {
                    selectEquipmentSlot -= 1;
                }
            }
            else if (lr > 0)
            {
                if (selectEquipmentSlot == 5)
                {
                    selectEquipmentSlot = 0;
                }
                else
                {
                    selectEquipmentSlot += 1;
                }
            }
        }

        int slotcodeR;
        if (selectInventoryType == 0) slotcodeR = selectSlot + 1;
        else slotcodeR = selectEquipmentSlot + 1;
        if (slotcodeR > 5)
        {
            slotcodeR = 0;
        }

        int slotcodeL;
        if (selectInventoryType == 0) slotcodeL = selectSlot - 1;
        else slotcodeL = selectEquipmentSlot - 1;
        if (slotcodeL < 0)
        {
            slotcodeL = 5;
        }

        int itemCodeL = 0;
        int itemCode = 0;
        int itemCodeR = 0;

        int itemCountL = 0;
        int itemCount = 0;

        if (selectInventoryType == 0)
        {
            if (itemList[slotcodeL] != null)
            {
                ItemState itemstateL = itemList[slotcodeL].GetComponent<ItemState>();
                itemCodeL = itemstateL.itemCode;
            }
            if (itemList[selectSlot] != null)
            {
                ItemState itemstate = itemList[selectSlot].GetComponent<ItemState>();
                itemCode = itemstate.itemCode;
            }
            if (itemList[slotcodeR] != null)
            {
                ItemState itemstateR = itemList[slotcodeR].GetComponent<ItemState>();
                itemCodeR = itemstateR.itemCode;
            }

            if (itemList[selectSlot] != null)
            {
                ItemState itemstate = itemList[selectSlot].GetComponent<ItemState>();
                itemCount = itemstate.itemCount;
            }
            if (itemList[slotcodeL] != null)
            {
                ItemState itemstateL = itemList[slotcodeL].GetComponent<ItemState>();
                itemCountL = itemstateL.itemCount;
            }
        }else
        {
            itemCodeL = equipmentList[slotcodeL];
            itemCode = equipmentList[selectEquipmentSlot];
            itemCodeR = equipmentList[slotcodeR];
        }

        if (selectInventoryType == 0) UIManager.instance.Inventoryrenew(itemCodeL, itemCode, itemCodeR, itemCountL, itemCount);
        else UIManager.instance.Inventoryrenew(itemCodeL, itemCode, itemCodeR, 1, 1);
        RenewItemCanUse();
    }

    public void RenewItemCanUse(bool canuse = false, int needitem = 0)                    //0 : 그냥 true, canuse가 false면 검증단계 없이 그냥 바로 UI비활성화
    {
        if (itemList[selectSlot] != null)
        {
            ItemState item = itemList[selectSlot].GetComponent<ItemState>();
            if (canuse)
            {
                if (needitem == 0)
                {
                    item.canUse = true;
                }
                else if (needitem == item.itemCode)
                {
                    item.canUse = true;
                }
                else
                {
                    item.canUse = false;
                }
            }
            else
            {
                item.canUse = false;
            }
            UIManager.instance.ItemCanUse(itemList[selectSlot]);
        }else
        {
            UIManager.instance.ItemCanUse(null);
        }
    }

    public void ItemUse()
    {
        Debug.Log("tryuse");
        if (itemList[selectSlot] != null)
        {
            ItemState itemstate = itemList[selectSlot].GetComponent<ItemState>();
            //UIManager.Instance.ItemUsingText(itemstate.itemCode, false);
            UIManager.instance.ItemUsingText(true, itemstate.itemCode);
            if (itemstate.itemCount == 1)
            {
                Destroy(itemList[selectSlot]);
                itemList[selectSlot] = null;
            }
            else
            {
                itemstate.itemCount -= 1;
            }
            SlotChange(0);
        }
    }

    public void ItemDrop(bool onlyPress, int itemCode = 0, Vector3 dropPosition = default)
    {
        if (itemList[selectSlot] != null || itemCode != 0 && selectInventoryType == 0)
        {
            UIManager.instance.KeyPress(itemDropPress, true);
            if (!onlyPress)
            {
                GameObject ItemObject = null;                
                if (itemCode != 0)
                {                    
                    if (dropPosition == Vector3.zero)
                    {
                        RaycastHit2D dropray = Physics2D.Raycast(P.transform.position, Vector2.down, Mathf.Infinity, ground);
                        ItemObject = Instantiate(itemPreFab, dropray.point + Vector2.up * 0.05f, Quaternion.identity, null);
                    }
                    else
                    {
                        ItemObject = Instantiate(itemPreFab, dropPosition, Quaternion.identity, null);
                    }
                    Itemstate = ItemObject.GetComponent<ItemState>();
                    Itemstate.itemCode = itemCode;                                                          //itemCode 지정하면 원하는 아이템 드롭(디버깅용도)
                    interact = ItemObject.GetComponent<InteractScript>();
                    interact.interactEvent.AddListener(() => ItemAdd(ItemObject));
                }else
                {
                    if (dropPosition == Vector3.zero)
                    {                                                                                                                               //itemCode 지정하지 않으면 선택된 아이템 버리기(보통의 상황)                    
                        RaycastHit2D dropray = Physics2D.Raycast(P.transform.position, Vector2.down, Mathf.Infinity, ground);
                        itemList[selectSlot].transform.position = dropray.point + Vector2.up * 0.05f;
                        itemList[selectSlot].SetActive(true);
                    }
                    else
                    {
                        itemList[selectSlot].transform.position = dropPosition;
                        itemList[selectSlot].SetActive(true);
                    }
                    //interact = itemList[selectSlot].GetComponent<InteractScript>();
                    //interact.interatEvent.AddListener(() => ItemAdd(itemList[selectSlot]));
                }
                itemList[selectSlot] = null;
                SlotChange(0);
            }
        }        
    }

    public void ItemDropCancel()
    {
        UIManager.instance.KeyPress(itemDropPress, false);
    }

    public void GetHerb(GameObject Herb)
    {

    }

    public void UnlockEquipment(int slotNum)
    {
        switch(slotNum)
        {
            case 0:
                equipmentList[0] = 1;
                break;
        }
    }

    public void RewnewEquipment()
    {
        SlotChange(0);
    }
}

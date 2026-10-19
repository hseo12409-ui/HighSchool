using NUnit.Framework.Constraints;
using UnityEngine;

public class DebugManager : MonoBehaviour
{
    [SerializeField] Transform player;
    [SerializeField] GameObject debugTool;
    
    [Header("텔레포트")]
    [SerializeField] Transform[] teleportSpots;
    [SerializeField] Transform teleportSpot;

    [Header("아이템 소환")]
    public int itemcode;    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        debugTool.SetActive(false);
    }

    public void ActiveDebugTools()
    {
        debugTool.SetActive(!debugTool.activeSelf);
    }

    public void TeleportChanged(int index)
    {
        Debug.Log(index);
        teleportSpot = teleportSpots[index];
    }

    public void Teleport()
    {
        player.position = teleportSpot.position;
    }

    public void ItemcodeChange(string a)
    {
        if (a != "")
        {
            itemcode = int.Parse(a);
        }
    }

    public void SummonItem()
    {
        ItemManager.instance.ItemDrop(false, itemcode);
    }
}

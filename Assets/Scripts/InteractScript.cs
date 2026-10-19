using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class InteractScript : MonoBehaviour
{
    public int priority;

    [SerializeField] Vector3 UIexPos;

    public UnityEvent interactEvent;
    public UnityEvent enterEvent;
    public UnityEvent exitEvent;

    public bool isLock;
    public bool caninteract;
    public bool offOnPress;
    public bool interactOnStay = false;
    public bool actOneTime;
    [SerializeField] bool destoryOnEnter = false;
    
    int playerLayerIndex;
    [SerializeField] LayerMask interactionLayer;

    Player p;
    SpriteRenderer spr;

    private void Reset()
    {        
        interactionLayer = 1 << LayerMask.NameToLayer("Player");
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        spr = GetComponent<SpriteRenderer>();
        playerLayerIndex = LayerMask.NameToLayer("Player");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == playerLayerIndex && (interactionLayer.value & (1 << collision.gameObject.layer)) != 0)                               //접촉한게 플레이어일때
        {
            enterEvent.Invoke();
            if (!interactOnStay)
            {
                if (p == null) p = collision.GetComponent<Player>();
                if (caninteract)
                {
                    UIManager.instance.InterationSet(true, priority, UIexPos, transform);
                    if (p.interationpriority <= priority)
                    {
                        p.interationpriority = priority;
                        p.interat = this;
                    }
                }
            }
        }
        else
        {
            if ((interactionLayer.value & (1 << collision.gameObject.layer)) != 0)
            {
                enterEvent.Invoke();
                if (destoryOnEnter && !interactOnStay)
                {
                    Destroy(collision.gameObject);
                }
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (interactOnStay)
        {
            if (collision.gameObject.layer == playerLayerIndex && (interactionLayer.value & (1 << collision.gameObject.layer)) != 0)
            {
                if (p == null) p = collision.GetComponent<Player>();
                if (caninteract)
                {
                    enterEvent.Invoke();
                    UIManager.instance.InterationSet(true, priority, UIexPos, transform);                    
                    if (p.interationpriority <= priority)
                    {
                        p.interationpriority = priority;
                        p.interat = this;
                    }
                }
            }
            else
            {
                if ((interactionLayer.value & (1 << collision.gameObject.layer)) != 0) enterEvent.Invoke();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == playerLayerIndex && (interactionLayer.value & (1 << collision.gameObject.layer)) != 0)
        {
            exitEvent.Invoke();
            if (p.interat != null)
            {                
                if (p.interat == this)
                {
                    UIManager.instance.InterationSet(false, priority, UIexPos, transform);
                    if (p != null)
                    {
                        p = collision.GetComponent<Player>();
                        p.interationpriority = 0;
                        p.interat = null;
                        p = null;
                    }
                }                                
            }
        }
        else
        {
            if ((interactionLayer.value & (1 << collision.gameObject.layer)) != 0) exitEvent.Invoke();
        }
    }

    public void interatfeedback()
    {
        if (caninteract)
        {
            if (!isLock)
            {
                interactEvent.Invoke();
                if (actOneTime)
                {
                    caninteract = false;
                }
                if (offOnPress)
                {
                    if (p.interat == this)
                    {
                        UIManager.instance.InterationSet(false, priority, UIexPos, transform);
                        p.interationpriority = 0;
                        p.interat = null;                        
                    }
                }
            }else
            {
                UIManager.instance.InteractionText(0);
            }
        }
    }

    public void Lock()
    {
        isLock = true;
    }

    public void Unlock()
    {
        isLock = false;
    }
}

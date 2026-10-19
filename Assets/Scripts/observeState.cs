using UnityEngine;

public class observeState : MonoBehaviour
{
    public bool isObserve;

    public Vector3 observePos;

    public string infor1;
    public string infor2;
    public string infor3;

    [SerializeField] float[] effectWait;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RewnewObserveUI(bool onOff)
    {
        if (isObserve) UIManager.instance.OnObserveUI(onOff, infor1, infor2, infor3, gameObject, effectWait);
        Debug.Log("Rewnew");
    }

    public void RewnewObserveText()
    {
        if (isObserve) UIManager.instance.ObserveText(infor1, infor2 , infor3);
    }
}

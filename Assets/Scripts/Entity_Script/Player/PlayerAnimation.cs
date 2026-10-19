using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] GameObject animatorObject;

    Animator anim;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        anim = animatorObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetTrigger(string triggerName)
    {
        anim.SetTrigger(triggerName);
    }

    public void ResetTrigger(string triggerName)
    {
        anim.ResetTrigger(triggerName);
    }

    public void SetBool(string boolName, bool value)
    {
        anim.SetBool(boolName, value);
    }

    public void SetFloat(string floatName, float value)
    {
        anim.SetFloat(floatName, value);
    }

    public void SetInt(string intName, int value)
    {
        anim.SetInteger(intName, value);
    }

    public void SetBoolIsrun(bool value)
    {
        anim.SetBool("isrun", value);
    }
}

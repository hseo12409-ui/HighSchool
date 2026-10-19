using System.Collections;
using UnityEngine;

public class Fire : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] GameObject hose;    

    public int fireStrength;                                        //0약불 1중불 2강불
    [SerializeField] float temperature;

    Animator hoseAnim;
    FireLight fireLight;
    observeState observe;
    Player p;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        hoseAnim = hose.GetComponent<Animator>();
        fireLight = GetComponent<FireLight>();
        observe = GetComponent<observeState>();
        p = player.GetComponent<Player>();

        StartCoroutine(DownTemperature());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ActiveFlameGun()
    {
        Debug.Log("FlameGun");
        hoseAnim.SetTrigger("air");
        if (temperature < 820) StartCoroutine(UpTemperature());
    }

    IEnumerator UpTemperature()
    {
        yield return new WaitForSeconds(0.4f);
        temperature += 40;
        fireLight.canChangeValue = false;
        float before = fireLight.random;
        fireLight.random = before + 1.3f;
        yield return new WaitForSeconds(0.7f);
        fireLight.random = before;
        fireLight.canChangeValue = true;
    }

    IEnumerator DownTemperature()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            if (p.currentState != PlayerState.MAKEPOTION)
            {
                if (temperature > 80) temperature -= 1;
                string fireStage = "";
                if (temperature >= 600)
                {
                    fireStrength = 2;
                    fireStage = "강불";
                }
                else if (temperature >= 200)
                {
                    fireStrength = 1;
                    fireStage = "중불";
                }
                else if (temperature >= 80)
                {
                    fireStrength = 0;
                    fireStage = "약불";
                }
                observe.infor2 = $"<color=#C63C00>{fireStage}</color>\n<color=red><size=25>{temperature}℃</size></color>";
                observe.RewnewObserveText();
            }            
        }
    }
}

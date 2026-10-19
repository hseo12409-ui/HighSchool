using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering.Universal;

public class alphacontroller : MonoBehaviour
{
    [SerializeField] float duration;
    [SerializeField] float endWait;                                         //아이템은 1.2

    [SerializeField] bool startOnEnable = true;
    [SerializeField] bool autoAlphaDown = true;
    [SerializeField] bool a = false;

    [SerializeField] AnimationCurve alphaCurve;    

    [SerializeField] GameObject[] setAlphaObject;

    [SerializeField] UnityEvent endEvent;

    List<Image> image = new List<Image>();
    List<TextMeshProUGUI> txt = new List<TextMeshProUGUI>();
    List<SpriteRenderer> sprite = new List<SpriteRenderer>();
    List<Light2D> light = new List<Light2D>();

    Coroutine AlphaUpI;
    Coroutine AlphaUpT;
    Coroutine AlphaUpS;
    Coroutine AlphaUpL;

    Coroutine AlphaDownI;
    Coroutine AlphaDownT;
    Coroutine AlphaDownS;
    Coroutine AlphaDownL;

    Coroutine AlphaDownHead;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        
    }

    private void OnEnable()
    {
        for (int i = 0; i < setAlphaObject.Length; i++)
        {
            switch (setAlphaObject[i].name[setAlphaObject[i].name.Length - 1])
            {
                case 'i':
                    image.Add(setAlphaObject[i].GetComponent<Image>());
                    break;

                case 't':
                    txt.Add(setAlphaObject[i].GetComponent<TextMeshProUGUI>());
                    break;

                case 's':
                    sprite.Add(setAlphaObject[i].GetComponent<SpriteRenderer>());
                    break;

                case 'l':
                    light.Add(setAlphaObject[i].GetComponent<Light2D>());
                    break;
            }
        }

        if (startOnEnable)
        {
            AlphaUp();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }    

    public void AlphaUp()
    {
        {
            if (image.Count != 0)
            {
                for (int i = 0; i < image.Count; i++)
                {
                    image[i].color = new Vector4(image[i].color.r, image[i].color.g, image[i].color.b, 0);
                }
            }

            if (txt.Count != 0)
            {
                for (int i = 0; i < txt.Count; i++)
                {
                    txt[i].color = new Vector4(txt[i].color.r, txt[i].color.g, txt[i].color.b, 0);
                }
            }

            if (sprite.Count != 0)
            {
                for (int i = 0; i < sprite.Count; i++)
                {
                    sprite[i].color = new Vector4(sprite[i].color.r, sprite[i].color.g, sprite[i].color.b, 0);
                }
            }

            if (light.Count != 0)
            {
                for (int i = 0; i < light.Count; i++)
                {
                    light[i].intensity = 0;
                }
            }
        }

        //StopCoroutine
        {
            //Up
        if (AlphaUpI != null)
        {
            StopCoroutine(AlphaUpI);
            AlphaUpI = null;
        }

        if (AlphaUpT != null)
        {
            StopCoroutine(AlphaUpT);
            AlphaUpT = null;
        }

        if (AlphaUpS != null)
        {
            StopCoroutine(AlphaUpS);
            AlphaUpS = null;
        }

            if (AlphaUpL != null)
            {
                StopCoroutine(AlphaUpL);
                AlphaUpL = null;
            }

            //Down
            if (AlphaDownI != null)
        {
            StopCoroutine(AlphaDownI);
            AlphaDownI = null;
        }

        if (AlphaDownT != null)
        {
            StopCoroutine(AlphaDownT);
            AlphaDownT = null;
        }
            if (AlphaDownS != null)
            {
                StopCoroutine(AlphaDownS);
                AlphaDownS = null;
            }

            if (AlphaDownL != null)
            {
                StopCoroutine(AlphaDownL);
                AlphaDownL = null;
            }
        }

        AlphaUpI = StartCoroutine(SetAlphaUpImage());
        AlphaUpT = StartCoroutine(SetAlphaUpTxt());
        AlphaUpS = StartCoroutine(SetAlphaUpSprite());
        AlphaUpL = StartCoroutine(SetAlphaUpLight());
        if (autoAlphaDown)
        {
            AlphaDown();
        }        
    }

    
    public IEnumerator SetAlphaUpImage()
    {        
        if (image.Count != 0)
        {
            float i = 0;
            for (int t = 0; t < 10; t++)
            {
                if (image[0].color.a < 1)
                {
                    i += 0.1f;
                    if (i < 0)
                    {
                        i = 0;
                    }
                    else if (i > 1)
                    {
                        i = 1;
                    }
                    for (int j = 0; j < image.Count; j++)
                    {
                        image[j].color = new Vector4(image[j].color.r, image[j].color.g, image[j].color.b, alphaCurve.Evaluate(i));
                        //if (a) Debug.Log(alphaCurve.Evaluate(image[j].color.a + 0.1f));
                    }
                    yield return new WaitForSeconds(0.02f);                    
                }
                else
                {
                    break;
                }
            }
        }
        AlphaUpI = null;        
    }

    public IEnumerator SetAlphaUpTxt()
    {        
        if (txt.Count != 0)
        {
            float i = 0;
            for (int t = 0; t < 10; t++)
            {
                if (txt[0].color.a < 1)
                {
                    i += 0.1f;
                    if (i < 0)
                    {
                        i = 0;
                    }
                    else if (i > 1)
                    {
                        i = 1;
                    }
                    for (int j = 0; j < txt.Count; j++)
                    {
                        txt[j].color = new Vector4(txt[j].color.r, txt[j].color.g, txt[j].color.b, alphaCurve.Evaluate(i));
                    }
                    yield return new WaitForSeconds(0.02f);                    
                }
                else
                {
                    break;
                }
            }
        }
        AlphaUpT = null;        
    }

    public IEnumerator SetAlphaUpSprite()
    {        
        if (sprite.Count != 0)
        {
            float i = 0;
            for (int t = 0; t < 10; t++)
            {
                if (sprite[0].color.a < 1)
                {
                    i += 0.1f;
                    if (i < 0)
                    {
                        i = 0;
                    }
                    else if (i > 1)
                    {
                        i = 1;
                    }
                    for (int j = 0; j < sprite.Count; j++)
                    {
                        sprite[j].color = new Vector4(sprite[j].color.r, sprite[j].color.g, sprite[j].color.b, alphaCurve.Evaluate(i));
                    }
                    yield return new WaitForSeconds(0.02f);                    
                }
                else
                {
                    break;
                }
            }
        }
        AlphaUpS = null;        
    }

    public IEnumerator SetAlphaUpLight()
    {
        if (light.Count != 0)
        {
            float i = 0;
            for (int t = 0; t < 10; t++)
            {
                if (light[0].color.a < 1)
                {
                    i += 0.1f;
                    if (i < 0)
                    {
                        i = 0;
                    }
                    else if (i > 1)
                    {
                        i = 1;
                    }
                    for (int j = 0; j < light.Count; j++)
                    {
                        light[j].intensity = alphaCurve.Evaluate(i);
                    }
                    yield return new WaitForSeconds(0.02f);
                }
                else
                {
                    break;
                }
            }
        }
        AlphaUpL = null;
    }

    public void AlphaDown()
    {
        if (AlphaDownHead == null)
        {
            AlphaDownHead = StartCoroutine(AlphaDownC());
        }
    }

    public IEnumerator AlphaDownC()
    {
        yield return new WaitForSeconds(endWait);

        //StopCoroutine
        {
            //Up
            if (AlphaUpI != null)
            {
                StopCoroutine(AlphaUpI);
                AlphaUpI = null;
            }

            if (AlphaUpT != null)
            {
                StopCoroutine(AlphaUpT);
                AlphaUpT = null;
            }

            if (AlphaUpS != null)
            {
                StopCoroutine(AlphaUpS);
                AlphaUpS = null;
            }

            if (AlphaUpL != null)
            {
                StopCoroutine(AlphaUpL);
                AlphaUpL = null;
            }

            //Down
            if (AlphaDownI != null)
            {
                StopCoroutine(AlphaDownI);
                AlphaDownI = null;
            }

            if (AlphaDownT != null)
            {
                StopCoroutine(AlphaDownT);
                AlphaDownT = null;
            }

            if (AlphaDownS != null)
            {
                StopCoroutine(AlphaDownS);
                AlphaDownS = null;
            }

            if (AlphaDownL != null)
            {
                StopCoroutine(AlphaDownL);
                AlphaDownL = null;
            }
        }

        //Debug.Log($"Down {gameObject.name}");
        AlphaDownI = StartCoroutine(SetAlphaDownImage());
        AlphaDownT = StartCoroutine(SetAlphaDownTxt());
        AlphaDownS = StartCoroutine(SetAlphaDownSprite());
        AlphaDownL = StartCoroutine(SetAlphaDownLight());
    }

    public IEnumerator SetAlphaDownImage()
    {
        if (image.Count != 0)
        {
            float i = 1;
            for (int t = 0; t < 10; t++)
            {
                if (image[0].color.a > 0)
                {
                    i -= 0.1f;
                    if (i < 0)
                    {
                        i = 0;
                    }
                    else if (i > 1)
                    {
                        i = 1;
                    }
                    for (int j = 0; j < image.Count; j++)
                    {
                        image[j].color = new Vector4(image[j].color.r, image[j].color.g, image[j].color.b, alphaCurve.Evaluate(i));
                        //if (a) Debug.Log(alphaCurve.Evaluate(image[j].color.a - 0.1f));
                    }
                    yield return new WaitForSeconds(0.02f);                    
                }
                else
                {
                    break;
                }
            }
        }
        AlphaDownI = null;
        AlphaDownHead = null;
    }

    public IEnumerator SetAlphaDownTxt()
    {
        if (txt.Count != 0)
        {
            float i = 1;
            for (int t = 0; t < 10;  t++)
            {
                if (txt[0].color.a > 0)
                {
                    i -= 0.1f;
                    if (i < 0)
                    {
                        i = 0;
                    }
                    else if (i > 1)
                    {
                        i = 1;
                    }
                    for (int j = 0; j < txt.Count; j++)
                    {
                        txt[j].color = new Vector4(txt[j].color.r, txt[j].color.g, txt[j].color.b, alphaCurve.Evaluate(i));
                    }
                    yield return new WaitForSeconds(0.02f);                    
                }
                else
                {
                    break;
                }
            }
        }
        AlphaDownT = null;
        AlphaDownHead = null;
    }

    public IEnumerator SetAlphaDownSprite()
    {
        if (sprite.Count != 0)
        {
            Debug.Log("Down");
            float i = 1;
            for (int t = 0; t < 10; t++)
            {
                if (sprite[0].color.a > 0)
                {
                    i -= 0.1f;
                    if (i < 0)
                    {
                        i = 0;
                    }
                    else if (i > 1)
                    {
                        i = 1;
                    }
                    for (int j = 0; j < sprite.Count; j++)
                    {                        
                        sprite[j].color = new Vector4(sprite[j].color.r, sprite[j].color.g, sprite[j].color.b, alphaCurve.Evaluate(i));
                    }
                    yield return new WaitForSeconds(0.02f);                    
                }
                else
                {
                    break;
                }
            }
        }
        AlphaDownS = null;
        AlphaDownHead = null;
    }

    public IEnumerator SetAlphaDownLight()
    {
        if (light.Count != 0)
        {            
            float i = 1;
            for (int t = 0; t < 10; t++)
            {
                if (light[0].color.a > 0)
                {
                    i -= 0.1f;
                    if (i < 0)
                    {
                        i = 0;
                    }
                    else if (i > 1)
                    {
                        i = 1;
                    }
                    for (int j = 0; j < light.Count; j++)
                    {
                        light[j].intensity = alphaCurve.Evaluate(i);
                    }
                    yield return new WaitForSeconds(0.02f);
                }
                else
                {
                    break;
                }
            }
        }
        AlphaDownL = null;
        AlphaDownHead = null;
    }
}

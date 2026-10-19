using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.AdaptivePerformance;
using UnityEngine.Rendering.Universal;

public class FireLight : MonoBehaviour
{
    [SerializeField] bool setRandomAnimSpeed;
    [SerializeField] bool setLightStrengthLerp;
    public bool canChangeValue = true;
    bool velocityCheck;
    bool isShaking;    

    public float random;

    [MinMaxRangeSlider(0f, 100f)]
    [SerializeField] Vector2 randomRange;

    Rigidbody2D rb;
    Light2D light;
    Animator anim;

    Coroutine VelocityCheckVari;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        light = GetComponent<Light2D>();
        if (setRandomAnimSpeed) anim = GetComponent<Animator>();

        StartCoroutine(SetRandom());
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (setLightStrengthLerp) light.intensity = Mathf.Lerp(light.intensity, random, 0.1f);

        if (rb != null)
        {
            if (Mathf.Abs(rb.linearVelocityX) < 1f)
            {
                isShaking = false;
                velocityCheck = false;
            }
            else
            {
                isShaking = true;
            }
        }
    }    

    IEnumerator SetRandom()
    {
        while (true)
        {
            if (canChangeValue)
            {
                if (!isShaking) random = Random.Range(randomRange.x, randomRange.y);
                else random = Random.Range(0.25f, randomRange.y - randomRange.y / 2f);

                if (!setLightStrengthLerp) light.intensity = random;
                if (setRandomAnimSpeed) anim.SetFloat("Speed", random);                
            }
            if (!isShaking) yield return new WaitForSeconds(0.2f);
            else yield return new WaitForSeconds(0.1f);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        velocityCheck = true;
    }
}

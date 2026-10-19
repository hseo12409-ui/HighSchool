using System.Collections;
using UnityEngine;

public class CamShake : MonoBehaviour
{
    public static CamShake instance;

    [SerializeField] Transform timeLineCam;
    Vector3 timeLIneCamPos;

    [SerializeField] int shakePriority;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        instance = this;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        timeLineCam.localPosition = Vector3.Lerp(timeLineCam.localPosition, timeLIneCamPos, 0.1f);
    }

    public IEnumerator Shake(float XStrength, float YStrength, float shakeDuration, int shakeTime, int priority)
    {
        if (shakePriority < priority)
        {
            shakePriority = priority;
            for (int i = 0; i < shakeTime; i++)
            {
                transform.localPosition = new Vector3(Random.Range(-XStrength, XStrength), Random.Range(-YStrength, YStrength), 0);
                yield return new WaitForSeconds(shakeDuration);
            }
            shakePriority = 0;
            transform.localPosition = Vector3.zero;
        }
    }

    public IEnumerator TimeLineShake(float XStrength, float YStrength, float shakeDuration, int shakeTime, int priority)
    {
        if (shakePriority < priority)
        {
            shakePriority = priority;
            for (int i = 0; i < shakeTime; i++)
            {
                timeLIneCamPos = new Vector3(Random.Range(-XStrength, XStrength), Random.Range(-YStrength, YStrength), 0);
                yield return new WaitForSeconds(shakeDuration);
            }
            shakePriority = 0;
            timeLineCam.localPosition = Vector3.zero;
        }
    }
}

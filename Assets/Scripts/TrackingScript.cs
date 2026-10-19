using UnityEngine;
using UnityEngine.UI;

enum TrackingTypeEnum
{
    WORLDTOSCREEN,
    SCREENTOSCREEN
}

public class TrackingScript : MonoBehaviour
{
    public Transform trackingobject;

    [SerializeField] TrackingTypeEnum trackingType = TrackingTypeEnum.WORLDTOSCREEN;

    [SerializeField] float lerpSpeed;

    public Vector3 exPos;
    Vector2 smoothVel;

    [SerializeField] bool isEnable;
    [SerializeField] bool lerpMoving;

    RectTransform rect;
    RectTransform rect1;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        if (trackingType == TrackingTypeEnum.SCREENTOSCREEN && trackingobject != null) rect1 = trackingobject.gameObject.GetComponent<RectTransform>();

        if (trackingType == TrackingTypeEnum.WORLDTOSCREEN) transform.position = Camera.main.WorldToScreenPoint(trackingobject.position + exPos);
        else rect.anchoredPosition = rect1.anchoredPosition;
        
    }

    private void Update()
    {
        if (isEnable)
        {
            if (!lerpMoving)
            {
                if (trackingType == TrackingTypeEnum.WORLDTOSCREEN) transform.position = Camera.main.WorldToScreenPoint(trackingobject.position + exPos);
                else rect.anchoredPosition = rect1.anchoredPosition;
            }
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isEnable)
        {
            if (lerpMoving)
            {
                Vector3 movepos = Vector3.zero;
                if (trackingType == TrackingTypeEnum.WORLDTOSCREEN)
                {
                    movepos = Camera.main.WorldToScreenPoint(trackingobject.position + exPos);
                    transform.position = Vector2.SmoothDamp(transform.position, movepos, ref smoothVel, lerpSpeed);
                }
                else
                {
                    movepos = rect1.anchoredPosition;
                    rect.anchoredPosition = Vector2.SmoothDamp(rect.anchoredPosition, movepos, ref smoothVel, lerpSpeed);
                }                
            }
        }
    }

    private void OnEnable()
    {
        isEnable = true;
        if (trackingType == TrackingTypeEnum.WORLDTOSCREEN) transform.position = Camera.main.WorldToScreenPoint(trackingobject.position + exPos);
        else rect.anchoredPosition = rect1.anchoredPosition;
    }

    private void OnDisable()
    {
        isEnable = false;
    }

    public void ChangeTrackingTarget(GameObject target)
    {        
        if (trackingType == TrackingTypeEnum.SCREENTOSCREEN) rect1 = target.GetComponent<RectTransform>();
        trackingobject = target.transform;
    }
}

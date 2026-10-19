using System.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class TravelManager : MonoBehaviour
{
    public static TravelManager instance;

    [SerializeField] GameObject player;
    
    [SerializeField] bool isControl;

    [SerializeField] UnityEvent travel;

    [Header("TravelStop")]
    [SerializeField] GameObject selectTravelStop;
    [SerializeField] int selectTravelStopNum;                                                   //선택한 정거장 번호
    [SerializeField] int currentTravelStopNum;                                                   //지금 있는 정거장 번호
    [SerializeField] GameObject[] travelStopList;

    [Header("TravelTimeLine")]
    [SerializeField] TimelineAsset Travel;

    Player P;
    PlayableDirector timeLine;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        P = player.GetComponent<Player>();
        timeLine = GetComponent<PlayableDirector>();
    }

    public void TravelStopSelectStart(int currentStopNum)
    {
        currentTravelStopNum = currentStopNum;
        selectTravelStopNum = currentTravelStopNum;
        isControl = true;
        selectTravelStop = travelStopList[selectTravelStopNum];
        P.ChangePlayerState(PlayerState.SELECTTRAVELSTOP);
        UIManager.instance.TutorialOn(1);
        UIManager.instance.TravelMapUIOnOff(true);
        UIManager.instance.ChangeTravelStopUI(selectTravelStopNum);
    }

    public void TravelStopSelectEnd()
    {
        currentTravelStopNum = 0;
        selectTravelStopNum = 0;        
        isControl = false;
        selectTravelStop = null;
        UIManager.instance.TutorialOff(1);
        UIManager.instance.TravelMapUIOnOff(false);
        P.ChangePlayerState(PlayerState.NORMAL);
    }

    public void ChangeTravelStop(int value)
    {
        Debug.Log("Change");
        if (isControl)
        {
            if (value < 0)                                                      //value가 -일때(전페이지)
            {
                if (selectTravelStopNum != 0)
                {                    
                    selectTravelStopNum -= 1;
                }
            }
            else                                                                //value가 +일때(다음페이지)
            {
                if (selectTravelStopNum < travelStopList.Length - 1)
                {                    
                    selectTravelStopNum += 1;
                }
            }
            selectTravelStop = travelStopList[selectTravelStopNum];
            UIManager.instance.ChangeTravelStopUI(selectTravelStopNum);
        }
    }

    public void TravelStart()
    {        
        if (selectTravelStopNum != currentTravelStopNum)
        {
            isControl = false;
            travel.Invoke();
            StartCoroutine(TravelStartCoroutine());
            UIManager.instance.TutorialOff(1);            
        }
    }

    IEnumerator TravelStartCoroutine()
    {
        yield return new WaitForSeconds(0.5f);
        P.ChangePlayerState(PlayerState.PLAYTIMELINE);
        timeLine.playableAsset = Travel;
        timeLine.Play();
        StartCoroutine(CamShake.instance.TimeLineShake(0.1f, 0.3f, 0.05f, 100, 5));
        yield return new WaitForSeconds(1);
        P.Teleport(selectTravelStop.transform);
        yield return new WaitForSeconds(4.5f);        
        TravelStopSelectEnd();
    }
}

using UnityEngine;

public class SetCampos : MonoBehaviour
{
    public Transform Pos;
    public float zoomValue;
    public Vector3 camPos;                                              //이 값을 고정
    public Vector3 exPos;                                               //이 값을 추가
    public bool isLockPos;                                              //true camPos로 카메라가 고정됨(특정 좌표만 고정), false Pos로 카메라가 고정됨                                          
    public bool onlyZoom;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

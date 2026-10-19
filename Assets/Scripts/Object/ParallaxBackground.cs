using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    Transform camTransform;
    Vector3 lastCamPos;

    [Header("움직일거")]
    [SerializeField] GameObject[] moveObject;

    public (GameObject, float) a;

    [SerializeField] float[] parallaxEffect; //0: 고정, 1이하 : 카메라보다 느리게 이동(플레이어보다 멀리 있음), 1: 카메랑 같이 이동(플레이어랑 같은 거리에 있음), 1이상:카메라보다 더 빨리 이동(가까이 있는거)

    void Awake()
    {
        camTransform = Camera.main.transform;
        lastCamPos = camTransform.position;
    }

    void LateUpdate()
    {
        for (int i = 0; i < moveObject.Length; i++)
        {
            Vector3 moveValue = camTransform.position - lastCamPos;                 //이번 프레임에 이동한 거리

            moveObject[i].transform.position += new Vector3(moveValue.x * parallaxEffect[i], 0, 0);            
        }
        lastCamPos = camTransform.position;
    }
}

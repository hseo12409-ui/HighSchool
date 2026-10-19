using UnityEngine;
using System.Collections;
using Steamworks;
using TMPro;

public class HapticManager : MonoBehaviour
{
    // 어디서든 HapticManager.Instance로 접근할 수 있게 만드는 싱글톤 패턴
    public static HapticManager Instance { get; private set; }

    private InputHandle_t padHandle;
    [SerializeField] bool isControllerFound = false;

    [SerializeField] TextMeshProUGUI checktxt;

    [Header("Normal")]
    [SerializeField] AnimationCurve lowcurve;
    [SerializeField] AnimationCurve highcurve;
    [SerializeField] float dur;

    [Header("Gun")]
    [SerializeField] AnimationCurve lowcurve1;
    [SerializeField] AnimationCurve highcurve1;
    [SerializeField] float dur1;

    private void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); }
    }

    void Start()
    {
        StartCoroutine(InitializeSteamInputRoutine());
    }

    private void Update()
    {
        checktxt.text = padHandle.ToString();
    }

    IEnumerator InitializeSteamInputRoutine()
    {
        yield return new WaitUntil(() => SteamManager.Initialized);
        SteamInput.Init(false);
        SteamInput.RunFrame();

        yield return new WaitForSeconds(0.5f);

        InputHandle_t[] allPadHandles = new InputHandle_t[Constants.STEAM_INPUT_MAX_COUNT];
        int controllerCount = SteamInput.GetConnectedControllers(allPadHandles);

        if (controllerCount > 0)
        {
            //  [0]을 붙여서 배열의 첫 번째(0번째) 패드 주소를 꺼내오도록 수정합니다.
            padHandle = allPadHandles[0];

            isControllerFound = true;
            Debug.Log($"[햅틱 매니저] 듀얼센스 연동 완료 (주소: {padHandle})");
            float elapsed = 0f;
            while (elapsed < dur1)
            {
                // 현재 진행 비율 계산 (0.0 ~ 1.0)
                float normalizedTime = elapsed / dur1;

                //  커브 그래프에서 현재 시간 시점의 높이 값(Evaluate)을 추출 (0f ~ 1f)
                float leftValue = lowcurve1.Evaluate(normalizedTime);
                float rightValue = highcurve1.Evaluate(normalizedTime);

                // 추출한 0~1 사이의 비율 값을 햅틱 byte 범위(0~255)로 변환
                byte leftIntensity = (byte)(Mathf.Clamp01(leftValue) * 255);
                byte rightIntensity = (byte)(Mathf.Clamp01(rightValue) * 255);

                // 스팀 드라이버로 전송
                PlayHaptic(leftIntensity, rightIntensity);

                // 0.02초(50Hz) 간격으로 아주 촘촘하게 갱신하여 아날로그 사운드 파형처럼 부드럽게 연출
                yield return new WaitForSeconds(0.02f);
                elapsed += 0.02f;
            }
            PlayHaptic(0, 0);

        }

    }

    //  외부 스크립트들이 호출할 공용 진동 배달 함수
    public void PlayHaptic(byte leftIntensity, byte rightIntensity)
    {
        if (!isControllerFound) return;

        //듀얼센스, 엑박패드 등 모든 패드를 무선/이어폰 없이 지원하는 표준 함수입니다.
        // 0 ~ 65535 사이의 ushort 정수 값을 받으므로, byte(0~255) 값을 큰 숫자로 변환해 줍니다.
        ushort leftSpeed = (ushort)(leftIntensity * 257);   // 255 대입 시 최대치인 65535가 됨
        ushort rightSpeed = (ushort)(rightIntensity * 257);

        // 복잡한 char 타입 없이 주소표와 좌우 진동 세기만 깔끔하게 던집니다.
        SteamInput.TriggerVibration(padHandle, leftSpeed, rightSpeed);        
    }
}

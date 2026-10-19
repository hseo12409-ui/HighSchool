using UnityEngine;
using UnityEngine.InputSystem;


//듀얼센스, 사이버코모도
using CyberKomodo;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.XInput;
using System.Collections;
using UnityEngine.Audio;
using Steamworks;
using System;
using Unity.IO.LowLevel.Unsafe;
using NUnit.Framework.Constraints;

public class DualSense : MonoBehaviour
{
    public static DualSense instance;

    public string controltype;

    [Header("진동")]
    public bool isleftuse;
    public bool isrightuse;
    
    public float leftpriority;
    public float rightpriority;

    public float leftstrength;
    public float rightstrength;

    //[Range(1f, 10f)] public float hapticIntensity = 3.0f;

    //private float[] audioSamples = new float[256];

    //[SerializeField] HapticEffectSO test1;

    public CyberKomodo.DualSenseGamepadHID dualSense;
    public XInputController Xbox;

    public GameObject AudioSource;

    Coroutine Hapticvari;
    AudioSource AS;

    private InputHandle_t padHandle;
    private bool isControllerFound = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        instance = this;

        AS = GetComponent<AudioSource>();
        // 스팀 인풋 공식 햅틱 펄스 함수 형식
        // 스팀 인풋 공식 햅틱 펄스 함수 형식
        //SteamInput.TriggerSimpleHapticEvent(padHandle, EControllerHapticEvent.k_EControllerHapticEvent_LightTick);
        // padHandle은 이전에 구한 컨트롤러 핸들 변수입니다.
        StartCoroutine(Check());


        //AS.gamepadSpeakerOutputType = GamepadSpeakerOutputType.Vibration;
        //var config = AudioSettings.GetConfiguration();
        //config.speakerMode = AudioSpeakerMode.Quad; // 3, 4채널 재생용 설정
        ////AudioSettings.Reset(config);
        //Debug.Log("현재 스피커 모드: " + config.speakerMode);

        //// 스피커 모드를 기반으로 실제 채널 수 확인하기
        //int channelCount = GetChannelCount(config.speakerMode);
        //Debug.Log("현재 출력 채널 수: " + channelCount + "개");
        //AudioSettings.speakerMode = AudioSpeakerMode.Mono;
        //#if UNITY_PS4 || UNITY_PS5 || UNITY_EDITOR
        //        AS.PlayOnGamepad(3);
        //#endif


        if (dualSense == null || Xbox == null)
        {
            dualSense = Gamepad.current as CyberKomodo.DualSenseGamepadHID;
            Xbox = Gamepad.current as XInputController;
            if (dualSense != null)
            {
                controltype = "D";
            }else if (Xbox != null)
            {
                controltype = "X";
            }else
            {
                controltype = "keym";
            }
            return;
        }

        ////Debug.Log("D");
        //AudioSpeakerMode projectSpeakerMode = AudioSettings.driverCapabilities;

        //// 2. 현재 런타임 환경에서 작동 중인 실제 출력 스피커 모드 확인
        //AudioSpeakerMode currentSpeakerMode = AudioSettings.speakerMode;

        //Debug.Log($"[유니티 설정] 드라이버 지원 모드: {projectSpeakerMode}");
        //Debug.Log($"[현재 출력] 실제 작동 모드: {currentSpeakerMode}");

        //// 채널 수 직접 계산 및 출력
        ////int channelCount = GetChannelCount(currentSpeakerMode);
        //Debug.Log($"[채널 수] 현재 활성화된 오디오 채널 수: {channelCount}개");

        ////HapticManager.PlayEffect(test1, transform.position);
    }

    IEnumerator Check()
    {
        // 스팀 매니저가 완전히 켜질 때까지 잠깐 대기 (안전장치)
        yield return new WaitUntil(() => SteamManager.Initialized);

        // 스팀이 하드웨어를 탐색할 수 있도록 1.5초 정도 충분히 시간을 줍니다.
        yield return new WaitForSeconds(1.5f);

        InputHandle_t[] allPadHandles = new InputHandle_t[Constants.STEAM_INPUT_MAX_COUNT];
        int controllerCount = SteamInput.GetConnectedControllers(allPadHandles);

        if (controllerCount > 0)
        {
            // 찾은 주소표를 전역 변수에 소중히 저장해 둡니다.
            padHandle = allPadHandles[0];
            isControllerFound = true;
            Debug.Log($"[성공] 듀얼센스 주소표({padHandle})를 미리 확보했습니다!");
        }
        else
        {
            Debug.LogWarning("게임 시작 시점에 듀얼센스를 찾지 못했습니다. 패드를 켜고 다시 시도하세요.");
        }
        yield return new WaitForSeconds(0.5f);
        if (isControllerFound)
        {
            SteamInput.TriggerSimpleHapticEvent(
                padHandle,
                EControllerHapticLocation.k_EControllerHapticLocation_Left,
                255, (char)0, 255, (char)0
            );
        }
    }

    //void OnAudioFilterRead(float[] data, int channels)
    //{
    //    Debug.Log(channels);
    //    // Project Settings에서 Quad로 설정했다면 channels는 4가 됩니다.
    //    if (channels == 4)
    //    {
    //        for (int i = 0; i < data.Length; i += 4)
    //        {                
    //            // data[i]   = 1번 채널 (Front Left - 게임 소리)
    //            // data[i+1] = 2번 채널 (Front Right - 게임 소리)
    //            // data[i+2] = 3번 채널 (Rear Left - 듀얼센스 왼쪽 햅틱 진동)
    //            // data[i+3] = 4번 채널 (Rear Right - 듀얼센스 오른쪽 햅틱 진동)

    //            // 기본 소리 채널(1,2번)로 가는 진동음을 음소거하여 햅틱 모터로만 소리가 가게 만듭니다.
    //            float leftWave = data[i];
    //            float rightWave = data[i + 1];
    //            Debug.Log(leftWave + "" + rightWave);

    //            data[i] = 0f;      // 스피커 소리 끔
    //            data[i + 1] = 0f;  // 스피커 소리 끔

    //            data[i + 2] = leftWave;  // 왼쪽 모터로 파형 전달
    //            data[i + 3] = rightWave; // 오른쪽 모터로 파형 전달
    //        }
    //    }
    //}

    //private int GetChannelCount(AudioSpeakerMode mode)
    //{
    //    switch (mode)
    //    {
    //        case AudioSpeakerMode.Mono: return 1;
    //        case AudioSpeakerMode.Stereo: return 2;
    //        case AudioSpeakerMode.Quad: return 4;         // 듀얼센스 햅틱용 (Ch 3, 4)
    //        case AudioSpeakerMode.Surround: return 5;
    //        case AudioSpeakerMode.Mode5point1: return 6;
    //        case AudioSpeakerMode.Mode7point1: return 8;
    //        case AudioSpeakerMode.Prologic: return 2;
    //        default: return 0;
    //    }
    //}

    // Update is called once per frame
    void Update()
    {        
        //if (Input.GetKeyDown(KeyCode.Alpha1))
        //{
        //    Debug.Log("LED 빨간색 변경");
        //    dualSense.SetLightBarColor(Color.red);
        //    //dualSense.SetLightBarColor(Color.green);
        //    //CyberKomodo.LightBrightness = 1;            
        //}

        //// 4. [효과 초기화] 3번 키를 누르면 모든 트리거 효과 리셋
        //if (Input.GetKeyDown(KeyCode.Alpha2))
        //{            
        //    dualSense.SetTriggerEffect(TriggerSide.Right, TriggerEffect.Feedback(1, 1));
        //    Debug.Log("2");
        //}

        //if (Input.GetKeyDown(KeyCode.Alpha3))
        //{            
        //    dualSense.SetTriggerEffect(TriggerSide.Right, TriggerEffect.Feedback(5, 5));
        //    Debug.Log("3");
        //}

        //if (Input.GetKeyDown(KeyCode.Alpha4))
        //{
        //    //dualSense.SetTriggerEffectVibration(TriggerSide.Both, 5, 5, 5);
        //    StartCoroutine(Haptic(0, 0.05f, 0.1f));
        //    Debug.Log("4");
        //}
    }

    public void Hapticvoid(float low, float high, float vibtime, float priorityL, float priorityR, bool smooth = false ,float smoothtime = 0)
    {
        //Debug.Log("호출됨");
        if (controltype == "D")
        {
            if (high != 0 && rightpriority < priorityR && low != 0 && leftpriority < priorityL)         //both
            {
                rightpriority = priorityR;
                rightstrength = high;
                isrightuse = true;

                if (Hapticvari != null)
                {
                    //dualSense.SetMotorSpeeds(0, 0);
                    StopCoroutine(Hapticvari);
                }
                Hapticvari = StartCoroutine(Haptic(low, high, vibtime, priorityL, priorityR, smooth, smoothtime));
            }
            else if (low != 0 && leftpriority < priorityL)                                             //left
            {
                leftpriority = priorityL;
                leftstrength = low;
                isleftuse = true;

                if (Hapticvari != null)
                {
                    //dualSense.SetMotorSpeeds(0, 0);
                    StopCoroutine(Hapticvari);
                }
                Hapticvari = StartCoroutine(Haptic(low, high, vibtime, priorityL, priorityR, smooth, smoothtime));
            }
            else if (high != 0 && rightpriority < priorityR)                                            //right
            {
                rightpriority = priorityR;
                rightstrength = high;
                isrightuse = true;

                if (Hapticvari != null)
                {
                    //dualSense.SetMotorSpeeds(0, 0);
                    StopCoroutine(Hapticvari);
                }
                Hapticvari = StartCoroutine(Haptic(low, high, vibtime, priorityL, priorityR, smooth, smoothtime));
            }
        }else if (controltype == "X")
        {
            if (high != 0 && rightpriority < priorityR && low != 0 && leftpriority < priorityL)         //both
            {
                rightpriority = priorityR;
                rightstrength = high;
                isrightuse = true;

                if (Hapticvari != null)
                {
                    //Xbox.SetMotorSpeeds(0, 0);
                    StopCoroutine(Hapticvari);
                }
                Hapticvari = StartCoroutine(Haptic(low, high, vibtime, priorityL, priorityR, smooth, smoothtime));
            }
            else if (low != 0 && leftpriority < priorityL)                                             //left
            {
                leftpriority = priorityL;
                leftstrength = low;
                isleftuse = true;

                if (Hapticvari != null)
                {
                    //Xbox.SetMotorSpeeds(0, 0);
                    StopCoroutine(Hapticvari);
                }
                Hapticvari = StartCoroutine(Haptic(low, high, vibtime, priorityL, priorityR, smooth, smoothtime));
            }
            else if (high != 0 && rightpriority < priorityR)                                            //right
            {
                rightpriority = priorityR;
                rightstrength = high;
                isrightuse = true;

                if (Hapticvari != null)
                {
                    //Xbox.SetMotorSpeeds(0, 0);
                    StopCoroutine(Hapticvari);
                }
                Hapticvari = StartCoroutine(Haptic(low, high, vibtime, priorityL, priorityR, smooth, smoothtime));
            }
        }
    }

    public IEnumerator Haptic(float low, float high, float vibtime, float priorityL, float priorityR, bool smooth, float smoothtime)                     //low(왼쪽 모터) 최저값 0.004, high(오른쪽 모터) 최저값 0.004
    {
        yield return null;
        if (low > 1)
        {
            low = 1;
            //Debug.Log("over");
        }
        if (high > 1)
        {
            high = 1;
            //Debug.Log("over");
        }

        if (low != 0 && leftpriority < priorityL)
        {
            leftpriority = priorityL;
            leftstrength = low;
            isleftuse = true;
        }
        if (high != 0 && rightpriority < priorityR)
        {
            rightpriority = priorityR;
            rightstrength = high;
            isrightuse = true;
        }

        if (controltype == "D")
        {
            dualSense.SetMotorSpeeds(leftstrength, rightstrength);
            yield return new WaitForSeconds(vibtime);            
            dualSense.SetMotorSpeeds(0, 0);
            dualSense.ResetHaptics();

        }
        else if (controltype == "X")
        {
            leftstrength *= 2;
            rightstrength *= 2;
            Xbox.SetMotorSpeeds(leftstrength, rightstrength);
            yield return new WaitForSeconds(vibtime);
            Xbox.SetMotorSpeeds(0, 0);
            //Xbox.ResetHaptics();
        }

        leftpriority = 0;
        leftstrength = 0;
        isleftuse = false;
        rightpriority = 0;
        rightstrength = 0;
        isrightuse = false;
        Hapticvari = null;

        if (low != 0 && leftpriority < priorityL)
        {
            leftpriority = 0;
            leftstrength = 0;
            isleftuse = false;
        }
        if (high != 0 && rightpriority < priorityR)
        {
            rightpriority = 0;
            rightstrength = 0;
            isrightuse = false;
        }
    }

    public void SetLightBarvoid(string offsetcolor, float duration, bool eternal = false, Vector4 manualcolor = default)
    {
        StartCoroutine(SetLightBar(offsetcolor, duration, eternal, manualcolor));
    }

    IEnumerator SetLightBar(string offsetcolor, float duration, bool eternal = false, Vector4 manualcolor = default)
    {
        if (controltype == "D")
        {
            if (offsetcolor == "")
            {
                dualSense.SetLightBarColor(manualcolor);
            }else
            {
                switch (offsetcolor)
                {
                    case "Red":
                        //Debug.Log("Red");
                        dualSense.SetLightBarColor(Color.red);
                        break;

                    case "Orange":
                        //Debug.Log("Orange");
                        dualSense.SetLightBarColor(Color.orange);
                        break;

                    case "Yellow":
                        //Debug.Log("Yellow");
                        dualSense.SetLightBarColor(Color.yellow);
                        break;

                    case "Green":
                        //Debug.Log("Green");
                        dualSense.SetLightBarColor(Color.green);
                        break;

                    case "Blue":
                        //Debug.Log("Blue");
                        dualSense.SetLightBarColor(Color.blue);
                        break;

                    case "Purple":
                        //Debug.Log("Purple");
                        dualSense.SetLightBarColor(Color.purple);
                        break;
                }
            }
            if (!eternal)
            {
                yield return new WaitForSeconds(duration);
                dualSense.SetLightBarColor(Vector4.zero);
            }
        }
    }

    public void WeaponTrigger(bool left, bool right, byte start, byte end, byte Strength)
    {
        if (controltype == "D")
        {
            if (left && right)              //both
            {
                dualSense.SetTriggerEffectWeapon(TriggerSide.Both, start, end, Strength);
            }
            else if (left)                 //left
            {
                dualSense.SetTriggerEffectWeapon(TriggerSide.Left, start, end, Strength);
            }
            else                            //right
            {
                dualSense.SetTriggerEffectWeapon(TriggerSide.Right, start, end, Strength);
            }
        }
    }

    public void TriggerFeedback(bool left, bool right, byte start, byte Strength, bool onlythis = false)                                   //onlythis체크하면 다른 적응형 트리거 취소함
    {
        //Debug.Log("Feedback");
        if (controltype == "D")
        {
            if (left && right)              //both
            {
                //if (onlythis) dualSense.SetTriggerEffectOff(TriggerSide.Both);
                dualSense.SetTriggerEffect(TriggerSide.Both, TriggerEffect.Feedback(start, Strength));
            }
            else if (left)                 //left
            {
                //if (onlythis) dualSense.SetTriggerEffectOff(TriggerSide.Right);
                dualSense.SetTriggerEffect(TriggerSide.Left, TriggerEffect.Feedback(start, Strength));
            }
            else                            //right
            {
                //if (onlythis) dualSense.SetTriggerEffectOff(TriggerSide.Left);
                dualSense.SetTriggerEffect(TriggerSide.Right, TriggerEffect.Feedback(start, Strength));
            }
        }
    }

    public void ResetTrigger(bool left, bool right)
    {
        //Debug.Log("Reset");
        if (controltype == "D")
        {
            if (left && right)
            {
                dualSense.SetTriggerEffectOff(TriggerSide.Both);
            }
            else if (left)
            {
                dualSense.SetTriggerEffectOff(TriggerSide.Left);
            }
            else
            {
                dualSense.SetTriggerEffectOff(TriggerSide.Right);
            }
        }
    }

    public void StopVib()
    {
        if (controltype != "keym")
        {
            if (controltype == "D")
            {
                dualSense.ResetHaptics();
            }else
            {
                Xbox.ResetHaptics();
            }
        }        
    }

    public void PauseVib()
    {
        if (controltype != "keym")
        {
            if (controltype == "D")
            {
                dualSense.PauseHaptics();
            }
            else
            {
                Xbox.PauseHaptics();
            }            
        }            
    }

    public void ResumeVib()
    {
        if (controltype == "keym")
        {
            if (controltype == "D")
            {
                dualSense.ResumeHaptics();
            }
            else
            {
                Xbox.ResumeHaptics();
            }
        }        
    }
}

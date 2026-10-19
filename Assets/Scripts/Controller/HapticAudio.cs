using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(AudioSource))]
public class HapticAudio : MonoBehaviour
{
    [Header("햅틱 진동용 사운드 파일 (.wav)")]
    public AudioClip weaponVibrationClip;

    [Header("햅틱 진동 세기")]
    [Range(0f, 2f)] public float hapticVolume = 1.2f;

    private AudioSource hapticAudioSource;

    void Awake()
    {
        hapticAudioSource = GetComponent<AudioSource>();

        // [수정] 0f가 완전한 2D 세팅입니다. 
        // 3D(1f) 상태에서는 OnAudioFilterRead가 오디오 파이프라인에서 제외됩니다.
        hapticAudioSource.spatialBlend = 0f;

        hapticAudioSource.playOnAwake = false;
        hapticAudioSource.loop = false; // 일회성이므로 루프는 끕니다.
        hapticAudioSource.bypassEffects = true;
        hapticAudioSource.bypassListenerEffects = true;
    }

    void Update()
    {
        // [수정] 마우스 왼쪽 클릭 시 진동 발생 테스트
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            TriggerHaptic();
        }
    }

    public void TriggerHaptic()
    {
        if (weaponVibrationClip != null && hapticAudioSource != null)
        {
            // PlayOneShot 대신 Play()를 사용하는 것이 오디오 버퍼 라우팅에 더 안정적입니다.
            hapticAudioSource.clip = weaponVibrationClip;
            hapticAudioSource.Play();
            Debug.Log("TriggerHaptic: 오디오 재생 시작");
        }
    }

    // 오디오 소스가 실제로 데이터를 '재생 중일 때만' 이 함수가 유니티 엔진에 의해 호출됩니다.
    void OnAudioFilterRead(float[] data, int channels)
    {
        // 채널 수 확인용 로그 (유니티 콘솔 우측 하단에서 Collapse를 끄면 실시간으로 찍히는 것을 볼 수 있습니다)
        Debug.Log($"OnAudioFilterRead 작동 중 - 채널 수: {channels}");

        // 유니티 프로젝트 설정(Project Settings -> Audio -> Speaker Mode)이 
        // 최소 Quad(4채널) 이상이거나 Surround/5.1 등이어야 합니다.
        if (channels < 4) return;

        for (int i = 0; i < data.Length; i += channels)
        {
            // 원본 사운드의 1, 2번 채널(좌/우) 샘플 복사
            float leftSample = data[i];
            float rightSample = data[i + 1];

            // 1. 유저 헤드셋/스피커(1, 2번 채널) 소리 차단 (귀에는 안 들리게)
            data[i] = 0f;
            data[i + 1] = 0f;

            // 2. 듀얼센스 패드 진동판(3, 4번 채널)으로 데이터 전송
            data[i + 2] = leftSample * hapticVolume;
            data[i + 3] = rightSample * hapticVolume;
        }
    }
}

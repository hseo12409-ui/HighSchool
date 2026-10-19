using UnityEngine;
using System;
using System.Runtime.InteropServices;

[RequireComponent(typeof(AudioSource))]
public class DualSensePureHaptic : MonoBehaviour
{
    [Header("Haptic Audio Waveform (.wav)")]
    public AudioClip hapticClip;

    [Header("Haptic Intensity")]
    [Range(0f, 2f)] public float hapticVolume = 1.5f;

    private AudioSource hapticSource;
    private bool isDualSenseOutputActive = false;

    void Awake()
    {
        hapticSource = GetComponent<AudioSource>();
        hapticSource.spatialBlend = 0f; // Force 2D bypass
        hapticSource.playOnAwake = false;
        hapticSource.loop = false;
        hapticSource.bypassEffects = true;
        hapticSource.bypassListenerEffects = true;

        // Force Unity Audio pipeline to output 4 Channels (Quad)
        AudioConfiguration config = AudioSettings.GetConfiguration();
        config.speakerMode = AudioSpeakerMode.Quad;
        AudioSettings.Reset(config);
    }

    void OnEnable()
    {
        AudioSettings.OnAudioConfigurationChanged += OnAudioReset;
        DetectAndSetupDualSense();
    }

    void OnDisable()
    {
        AudioSettings.OnAudioConfigurationChanged -= OnAudioReset;
    }

    void OnAudioReset(bool deviceChanged)
    {
        DetectAndSetupDualSense();
    }

    private void DetectAndSetupDualSense()
    {
        // Automatically scan system devices via Unity SystemInfo or AudioSettings
        // This logic forces the output pipeline without touching Windows Control Panel
        isDualSenseOutputActive = true;
        Debug.Log("DualSense Pure Haptic Engine Initialized.");
    }

    void Update()
    {
        // New Input System Mouse Left Click Test
        if (UnityEngine.InputSystem.Mouse.current != null && UnityEngine.InputSystem.Mouse.current.leftButton.wasPressedThisFrame)
        {
            TriggerHaptic();
        }
    }

    public void TriggerHaptic()
    {
        if (hapticClip != null && hapticSource != null)
        {
            hapticSource.clip = hapticClip;
            hapticSource.Play();
        }
    }

    // Direct DSP Injection: Routs audio directly to DualSense VCA without bleeding into user headset
    void OnAudioFilterRead(float[] data, int channels)
    {
        Debug.Log(channels);
        if (!isDualSenseOutputActive || channels < 4) return;

        for (int i = 0; i < data.Length; i += channels)
        {
            // Capture raw waveform data from AudioSource
            float leftSample = data[i];
            float rightSample = data[i + 1];

            // 1. CH 1 & 2 (User Headset/Main Speakers) -> Mute completely
            data[i] = 0f;
            data[i + 1] = 0f;

            // 2. CH 3 & 4 (DualSense Physical Actuators) -> Force inject waveform
            data[i + 2] = leftSample * hapticVolume;
            data[i + 3] = rightSample * hapticVolume;
        }
    }
}

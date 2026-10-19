using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;

namespace CyberKomodo
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct DualSenseHIDState : IInputStateTypeInfo
    {
        public static FourCC Format = new('D', 'S', '5', 'S'); // DS5 State
        public readonly FourCC format => Format;

        [InputControl(name = "leftStick", layout = "Stick", format = "VC2B")]
        [InputControl(name = "leftStick/x", offset = 0, format = "BYTE", parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5")]
        [InputControl(name = "leftStick/left", offset = 0, format = "BYTE", parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5,clamp=1,clampMin=0,clampMax=0.5,invert")]
        [InputControl(name = "leftStick/right", offset = 0, format = "BYTE", parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5,clamp=1,clampMin=0.5,clampMax=1")]
        [InputControl(name = "leftStick/y", offset = 1, format = "BYTE", parameters = "invert,normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5")]
        [InputControl(name = "leftStick/up", offset = 1, format = "BYTE", parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5,clamp=1,clampMin=0,clampMax=0.5,invert")]
        [InputControl(name = "leftStick/down", offset = 1, format = "BYTE", parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5,clamp=1,clampMin=0.5,clampMax=1,invert=false")]
        public byte leftStickX;
        public byte leftStickY;

        [InputControl(name = "rightStick", layout = "Stick", format = "VC2B")]
        [InputControl(name = "rightStick/x", offset = 0, format = "BYTE", parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5")]
        [InputControl(name = "rightStick/left", offset = 0, format = "BYTE", parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5,clamp=1,clampMin=0,clampMax=0.5,invert")]
        [InputControl(name = "rightStick/right", offset = 0, format = "BYTE", parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5,clamp=1,clampMin=0.5,clampMax=1")]
        [InputControl(name = "rightStick/y", offset = 1, format = "BYTE", parameters = "invert,normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5")]
        [InputControl(name = "rightStick/up", offset = 1, format = "BYTE", parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5,clamp=1,clampMin=0,clampMax=0.5,invert")]
        [InputControl(name = "rightStick/down", offset = 1, format = "BYTE", parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5,clamp=1,clampMin=0.5,clampMax=1,invert=false")]
        public byte rightStickX;
        public byte rightStickY;

        [InputControl(name = "leftTrigger", format = "BYTE")]
        public byte leftTrigger;
        [InputControl(name = "rightTrigger", format = "BYTE")]
        public byte rightTrigger;

        [InputControl(name = "dpad", format = "BIT", layout = "Dpad", sizeInBits = 4, defaultState = 8)]
        [InputControl(name = "dpad/up", format = "BIT", layout = "DiscreteButton", parameters = "minValue=7,maxValue=1,nullValue=8,wrapAtValue=7", bit = 0, sizeInBits = 4)]
        [InputControl(name = "dpad/right", format = "BIT", layout = "DiscreteButton", parameters = "minValue=1,maxValue=3", bit = 0, sizeInBits = 4)]
        [InputControl(name = "dpad/down", format = "BIT", layout = "DiscreteButton", parameters = "minValue=3,maxValue=5", bit = 0, sizeInBits = 4)]
        [InputControl(name = "dpad/left", format = "BIT", layout = "DiscreteButton", parameters = "minValue=5, maxValue=7", bit = 0, sizeInBits = 4)]
        [InputControl(name = "buttonWest", displayName = "Square", bit = 4)]
        [InputControl(name = "buttonSouth", displayName = "Cross", bit = 5)]
        [InputControl(name = "buttonEast", displayName = "Circle", bit = 6)]
        [InputControl(name = "buttonNorth", displayName = "Triangle", bit = 7)]
        public byte buttonsVector0;
        [InputControl(name = "leftShoulder", bit = 0)]
        [InputControl(name = "rightShoulder", bit = 1)]
        [InputControl(name = "leftTriggerButton", layout = "Button", bit = 2)]
        [InputControl(name = "rightTriggerButton", layout = "Button", bit = 3)]
        [InputControl(name = "select", displayName = "Share", bit = 4)]
        [InputControl(name = "start", displayName = "Options", bit = 5)]
        [InputControl(name = "leftStickPress", bit = 6)]
        [InputControl(name = "rightStickPress", bit = 7)]
        public byte buttonsVector1;
        [InputControl(name = "systemButton", layout = "Button", displayName = "System", bit = 0)]
        [InputControl(name = "touchpadButton", layout = "Button", displayName = "Touchpad Press", bit = 1)]
        [InputControl(name = "micButton", layout = "Button", displayName = "Mic Mute", bit = 2)]
        public byte buttonsVector2;

        [InputControl(name = "gyro", layout = "Vector3", sizeInBits = 48, noisy = true)]
        [InputControl(name = "gyro/x", layout = "Axis", format = "SHRT")]
        [InputControl(name = "gyro/y", offset = 2, layout = "Axis", format = "SHRT")]
        [InputControl(name = "gyro/z", offset = 4, layout = "Axis", format = "SHRT")]
        public Vector3I16 gyro;
        [InputControl(name = "accel", layout = "Vector3", sizeInBits = 48, noisy = true)]
        [InputControl(name = "accel/x", layout = "Axis", format = "SHRT")]
        [InputControl(name = "accel/y", offset = 2, layout = "Axis", format = "SHRT")]
        [InputControl(name = "accel/z", offset = 4, layout = "Axis", format = "SHRT")]
        public Vector3I16 accel;
        public DualSenseHIDTouchpad touch0;
        public DualSenseHIDTouchpad touch1;

        public byte rightTriggerFeedback;
        public byte leftTriggerFeedback;

    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Vector3I16
    {
        public short x;
        public short y;
        public short z;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 4)]
    public readonly struct DualSenseHIDTouchpad
    {
        public byte Id => (byte)(inner & 0x7F);
        public bool IsActive => (inner & 0x80) == 0;
        public int X => (int)((inner >> 0x08) & 0xFFF);
        public int Y => (int)((inner >> 0x14) & 0xFFF);

        private readonly uint inner;
    }

    public enum BatteryState : byte
    {
        None = 0,
        Charging = 1,
        Full = 2,
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 1)]
    public readonly struct DualSenseHIDBatteryInfo
    {
        public float BatteryLevelPercent => BatteryLevelRaw * 100f / 8f;
        public int BatteryLevelRaw => inner & 0x0F;
        public BatteryState State => (BatteryState)((inner >> 4) & 0x0F);

        private readonly byte inner;
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 63)]
    public struct DualSenseHIDInputPacket
    {
        [FieldOffset(0)] public byte leftStickX;
        [FieldOffset(1)] public byte leftStickY;

        [FieldOffset(2)] public byte rightStickX;
        [FieldOffset(3)] public byte rightStickY;

        [FieldOffset(4)] public byte leftTrigger;
        [FieldOffset(5)] public byte rightTrigger;

        [FieldOffset(7)] public byte buttonsVector0;
        [FieldOffset(8)] public byte buttonsVector1;
        [FieldOffset(9)] public byte buttonsVector2;

        [FieldOffset(15)] public Vector3I16 gyro;
        [FieldOffset(21)] public Vector3I16 accel;

        [FieldOffset(32)] public DualSenseHIDTouchpad touch0;
        [FieldOffset(36)] public DualSenseHIDTouchpad touch1;

        [FieldOffset(41)] public byte rightTriggerFeedback;
        [FieldOffset(42)] public byte leftTriggerFeedback;

        [FieldOffset(52)] public DualSenseHIDBatteryInfo batteryInfo;
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 10)]
    public struct DualSenseHIDMinimalInput
    {
        [FieldOffset(0)] public byte reportId;
        [FieldOffset(1)] public byte leftStickX;
        [FieldOffset(2)] public byte leftStickY;
        [FieldOffset(3)] public byte rightStickX;
        [FieldOffset(4)] public byte rightStickY;
        [FieldOffset(5)] public byte buttons0;
        [FieldOffset(6)] public byte buttons1;
        [FieldOffset(7)] public byte buttons2;
        [FieldOffset(8)] public byte leftTrigger;
        [FieldOffset(9)] public byte rightTrigger;
    }
    [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 64)]
    public struct DualSenseHIDUsbInput
    {
        [FieldOffset(0)] public byte reportId;
        [FieldOffset(1)] public DualSenseHIDInputPacket packet;
    }
    [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 78)]
    public struct DualSenseHIDBluetoothInput
    {
        [FieldOffset(0)] public byte reportId;
        [FieldOffset(2)] public DualSenseHIDInputPacket packet;

        [FieldOffset(74)] public uint crc;
    }

    // Taken from https://gist.github.com/Nielk1/6d54cc2c00d2201ccb8c2720ad7538db
    public enum TriggerEffectMode : byte
    {
        Off = 0x05,
        Feedback = 0x21,
        Weapon = 0x25,
        Vibration = 0x26
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 11)]
    public struct TriggerEffect
    {
        public TriggerEffectMode mode;
        public unsafe fixed byte params_[10];

        /// <summary>
        /// Trigger will return to default off state
        /// </summary>
        public static TriggerEffect Off()
        {
            TriggerEffect effect = default;
            effect.mode = TriggerEffectMode.Off;
            for (int i = 0; i < 10; i++)
            {
                unsafe
                {
                    effect.params_[i] = 0;
                }
            }
            return effect;
        }

        /// <summary>
        /// Trigger will resist movement beyond the start position.
        /// </summary>
        /// <param name="position">The starting zone of the trigger effect. Must be between 0 and 9 inclusive.</param>
        /// <param name="strength">The force of the resistance. Must be between 0 and 8 inclusive.</param>
        /// <remarks>Parameters that are out of the specified range will be clamped and produce a warning (silent in player builds)</remarks>
        public static TriggerEffect Feedback(byte position, byte strength)
        {
            TriggerEffect effect = default;
            if (position > 9)
            {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                Debug.LogWarning($"[CyberKomodo.DualSense] {nameof(TriggerEffect)}.{nameof(Feedback)} {nameof(position)} out of range. Clamping");
#endif
                position = 9;
            }

            if (strength > 8)
            {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                Debug.LogWarning($"[CyberKomodo.DualSense] {nameof(TriggerEffect)}.{nameof(Feedback)} {nameof(strength)} out of range. Clamping");
#endif
                strength = 8;
            }

            if (strength == 0)
            {
                return Off();
            }

            byte forceValue = (byte)((strength - 1) & 0x07);
            uint forceZones = 0;
            ushort activeZones = 0;

            for (int i = position; i < 10; i++)
            {
                forceZones |= (uint)(forceValue << (3 * i));
                activeZones |= (ushort)(1 << i);
            }

            effect.mode = TriggerEffectMode.Feedback;

            unsafe
            {
                *(ushort*)effect.params_ = activeZones;
                *(uint*)(effect.params_ + 2) = forceZones;
                *(uint*)(effect.params_ + 6) = 0x00;
            }
            return effect;
        }

        /// <summary>
        /// Trigger will resist movement beyond the start position until the end position
        /// </summary>
        /// <param name="startPosition">The starting zone of the trigger effect. Must be between 2 and 7 inclusive.</param>
        /// <param name="endPosition">The ending zone of the trigger effect. Must be between <paramref name="startPosition"/>+1 and 8 inclusive.</param>
        /// <param name="strength">The force of the resistance. Must be between 0 and 8 inclusive.</param>
        /// <remarks>Parameters that are out of the specified range will be clamped and produce a warning (silent in player builds)</remarks>
        public static TriggerEffect Weapon(byte startPosition, byte endPosition, byte strength)
        {
            TriggerEffect effect = default;
            if (startPosition > 7 || startPosition < 2)
            {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                Debug.LogWarning($"[CyberKomodo.DualSense] {nameof(TriggerEffect)}.{nameof(Weapon)} {nameof(startPosition)} out of range. Clamping");
#endif
                startPosition = Math.Clamp(startPosition, (byte)2, (byte)7);
            }
            if (endPosition > 8 || endPosition <= startPosition)
            {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                Debug.LogWarning($"[CyberKomodo.DualSense] {nameof(TriggerEffect)}.{nameof(Weapon)} {nameof(endPosition)} out of range. Clamping");
#endif
                endPosition = Math.Clamp(endPosition, (byte)(startPosition + 1), (byte)8);
            }
            if (strength > 8)
            {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                Debug.LogWarning($"[CyberKomodo.DualSense] {nameof(TriggerEffect)}.{nameof(Weapon)} {nameof(strength)} out of range. Clamping");
#endif
                strength = 8;
            }

            if (strength == 0)
            {
                return Off();
            }

            ushort startAndStopZones = (ushort)((1 << startPosition) | (1 << endPosition));

            effect.mode = TriggerEffectMode.Weapon;

            unsafe
            {
                *(ushort*)effect.params_ = startAndStopZones;
                *(effect.params_ + 2) = (byte)((strength - 1) & 0x07);
            }
            for (int i = 3; i < 10; i++)
            {
                unsafe
                {
                    effect.params_[i] = 0;
                }
            }

            return effect;
        }

        /// <summary>
        /// Trigger will vibrate with the input amplitude and frequency beyond the start position
        /// </summary>
        /// <param name="position">The starting zone of the trigger effect. Must be between 0 and 9 inclusive.</param>
        /// <param name="strength">Strength of the automatic cycling action. Must be between 0 and 8 inclusive.</param>
        /// <param name="frequency">Frequency of the automatic cycling action in hertz.</param>
        /// <remarks>Parameters that are out of the specified range will be clamped and produce a warning (silent in player builds)</remarks>
        public static TriggerEffect Vibration(byte position, byte strength, byte frequency)
        {
            TriggerEffect effect = default;
            if (position > 9)
            {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                Debug.LogWarning($"[CyberKomodo.DualSense] {nameof(TriggerEffect)}.{nameof(Vibration)} {nameof(position)} out of range. Clamping");
#endif
                position = 9;
            }

            if (strength > 8)
            {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                Debug.LogWarning($"[CyberKomodo.DualSense] {nameof(TriggerEffect)}.{nameof(Vibration)} {nameof(strength)} out of range. Clamping");
#endif
                strength = 8;
            }

            if (strength == 0 || frequency == 0)
            {
                return Off();
            }
            byte forceValue = (byte)((strength - 1) & 0x07);
            uint forceZones = 0;
            ushort activeZones = 0;

            for (int i = position; i < 10; i++)
            {
                forceZones |= (uint)(forceValue << (3 * i));
                activeZones |= (ushort)(1 << i);
            }

            effect.mode = TriggerEffectMode.Vibration;

            unsafe
            {
                *(ushort*)effect.params_ = activeZones;
                *(uint*)(effect.params_ + 2) = forceZones;
                *(uint*)(effect.params_ + 6) = 0x00;

                // already zeroed by previous write
                *(effect.params_ + 8) = frequency;
            }

            return effect;
        }
    }
    public enum LightBrightness : byte
    {
        High = 0,
        Medium = 1,
        Low = 2,
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 47)]
    public struct DualSenseHIDOutputPacket
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static byte SetBit(byte full, int bit, bool value)
            => value ? (byte)(full | (1 << bit)) : (byte)(full & ~(1 << bit));
        public bool EnableRumbleEmulation
        {
            get => (flagVector0 & (1 << 0)) != 0;
            set => flagVector0 = SetBit(flagVector0, 0, value);
        }
        public bool UseRumbleNotHaptics
        {
            get => (flagVector0 & (1 << 1)) != 0;
            set => flagVector0 = SetBit(flagVector0, 1, value);
        }
        public bool AllowRightTriggerFFB
        {
            get => (flagVector0 & (1 << 2)) != 0;
            set => flagVector0 = SetBit(flagVector0, 2, value);
        }
        public bool AllowLeftTriggerFFB
        {
            get => (flagVector0 & (1 << 3)) != 0;
            set => flagVector0 = SetBit(flagVector0, 3, value);
        }
        public bool AllowHeadphoneVolume
        {
            get => (flagVector0 & (1 << 4)) != 0;
            set => flagVector0 = SetBit(flagVector0, 4, value);
        }
        public bool AllowSpeakerVolume
        {
            get => (flagVector0 & (1 << 5)) != 0;
            set => flagVector0 = SetBit(flagVector0, 5, value);
        }
        public bool AllowMicVolume
        {
            get => (flagVector0 & (1 << 6)) != 0;
            set => flagVector0 = SetBit(flagVector0, 6, value);
        }
        public bool AllowAudioControl
        {
            get => (flagVector0 & (1 << 7)) != 0;
            set => flagVector0 = SetBit(flagVector0, 7, value);
        }
        public bool AllowMuteLight
        {
            get => (flagVector1 & (1 << 0)) != 0;
            set => flagVector1 = SetBit(flagVector1, 0, value);
        }
        public bool AllowAudioMute
        {
            get => (flagVector1 & (1 << 1)) != 0;
            set => flagVector1 = SetBit(flagVector1, 1, value);
        }
        public bool AllowLedColor
        {
            get => (flagVector1 & (1 << 2)) != 0;
            set => flagVector1 = SetBit(flagVector1, 2, value);
        }
        public bool ResetLights
        {
            get => (flagVector1 & (1 << 3)) != 0;
            set => flagVector1 = SetBit(flagVector1, 3, value);
        }
        public bool AllowPlayerIndicators
        {
            get => (flagVector1 & (1 << 4)) != 0;
            set => flagVector1 = SetBit(flagVector1, 4, value);
        }
        public bool AllowHapticLowPassFilter
        {
            get => (flagVector1 & (1 << 5)) != 0;
            set => flagVector1 = SetBit(flagVector1, 5, value);
        }
        public bool AllowMotorPowerLevel
        {
            get => (flagVector1 & (1 << 6)) != 0;
            set => flagVector1 = SetBit(flagVector1, 6, value);
        }
        public bool AllowAudioControl2
        {
            get => (flagVector1 & (1 << 7)) != 0;
            set => flagVector1 = SetBit(flagVector1, 7, value);
        }
        public bool AllowLightBrightnessChange
        {
            get => (flagVector2 & (1 << 0)) != 0;
            set => flagVector2 = SetBit(flagVector2, 0, value);
        }
        public bool AllowColorLightFadeAnimation
        {
            get => (flagVector2 & (1 << 1)) != 0;
            set => flagVector2 = SetBit(flagVector2, 1, value);
        }
        public bool EnableImprovedRumbleEmulation
        {
            get => (flagVector2 & (1 << 2)) != 0;
            set => flagVector2 = SetBit(flagVector2, 2, value);
        }

        public bool PlayerLight1
        {
            get => (playerLightVector & (1 << 0)) != 0;
            set => playerLightVector = SetBit(playerLightVector, 0, value);
        }
        public bool PlayerLight2
        {
            get => (playerLightVector & (1 << 1)) != 0;
            set => playerLightVector = SetBit(playerLightVector, 1, value);
        }
        public bool PlayerLight3
        {
            get => (playerLightVector & (1 << 2)) != 0;
            set => playerLightVector = SetBit(playerLightVector, 2, value);
        }
        public bool PlayerLight4
        {
            get => (playerLightVector & (1 << 3)) != 0;
            set => playerLightVector = SetBit(playerLightVector, 3, value);
        }
        public bool PlayerLight5
        {
            get => (playerLightVector & (1 << 4)) != 0;
            set => playerLightVector = SetBit(playerLightVector, 4, value);
        }
        public bool PlayerLightDisableFade
        {
            get => (playerLightVector & (1 << 5)) != 0;
            set => playerLightVector = SetBit(playerLightVector, 5, value);
        }

        [FieldOffset(0)] public byte flagVector0;
        [FieldOffset(1)] public byte flagVector1;

        [FieldOffset(2)] public byte rumbleLight;
        [FieldOffset(3)] public byte rumbleHeavy;

        [FieldOffset(10)] public TriggerEffect rightTrigger;
        [FieldOffset(21)] public TriggerEffect leftTrigger;

        [FieldOffset(38)] public byte flagVector2;

        [FieldOffset(42)] public LightBrightness lightBrightness;
        [FieldOffset(43)] public byte playerLightVector;

        [FieldOffset(44)] public byte ledR;
        [FieldOffset(45)] public byte ledG;
        [FieldOffset(46)] public byte ledB;
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 48)]
    public struct DualSenseHIDUsbOutput
    {
        [FieldOffset(0)] public byte reportId;
        [FieldOffset(1)] public DualSenseHIDOutputPacket packet;
        public static unsafe DualSenseHIDUsbOutput Create(DualSenseHIDOutputPacket packet)
        {
            DualSenseHIDUsbOutput output = default;

            output.reportId = 0x02;
            output.packet = packet;

            return output;
        }
    }
    [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 78)]
    public struct DualSenseHIDBluetoothOutput
    {
        [FieldOffset(0)] public byte reportId;
        [FieldOffset(1)] public byte seq_tag;
        [FieldOffset(2)] public byte tag;
        [FieldOffset(3)] public DualSenseHIDOutputPacket packet;
        [FieldOffset(74)] public uint crc;

        public static unsafe DualSenseHIDBluetoothOutput Create(DualSenseHIDOutputPacket packet, ref byte seq)
        {
            DualSenseHIDBluetoothOutput output = default;

            output.reportId = 0x31;
            output.seq_tag = (byte)(seq << 4);
            if (++seq >= 16) seq = 0;
            output.tag = 0x10;

            output.packet = packet;
            output.crc = ComputeCrc((byte*)UnsafeUtility.AddressOf(ref output), 74);

            return output;
        }

        private static readonly uint[] s_CrcTable = BuildTable();

        private static uint[] BuildTable()
        {
            const uint poly = 0xEDB88320;
            uint[] table = new uint[256];
            for (uint i = 0; i < 256; ++i)
            {
                uint c = i;
                for (int k = 0; k < 8; ++k)
                    c = (c & 1) != 0 ? (poly ^ (c >> 1)) : (c >> 1);
                table[i] = c;
            }
            return table;
        }
        private static unsafe uint ComputeCrc(byte* data, int length)
        {
            uint crc = 0xFFFFFFFFu;
            // seeded crc32 with 0xA2 per sony spec 
            crc = (crc >> 8) ^ s_CrcTable[(crc ^ 0xA2) & 0xFF];
            for (int i = 0; i < length; ++i)
                crc = (crc >> 8) ^ s_CrcTable[(crc ^ data[i]) & 0xFF];
            return ~crc;
        }
    }

}
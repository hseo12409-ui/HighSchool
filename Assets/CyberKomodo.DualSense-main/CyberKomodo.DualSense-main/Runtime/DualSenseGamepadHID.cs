using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.HID;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.Scripting;
using UnityEngine.InputSystem.Utilities;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.InputSystem.Controls;



#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CyberKomodo
{
    [Flags]
    public enum TriggerSide
    {
        Left = 1 << 0,
        Right = 1 << 1,

        Both = Left | Right
    }

#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    [Preserve]
    [InputControlLayout(stateType = typeof(DualSenseHIDState), displayName = "DualSense HID")]
    public class DualSenseGamepadHID : DualShockGamepad, IInputStateCallbackReceiver
    {
        public ButtonControl leftTriggerButton { get; protected set; }
        public ButtonControl rightTriggerButton { get; protected set; }
        public ButtonControl playStationButton { get; protected set; }
#if UNITY_EDITOR
        static DualSenseGamepadHID()
        {
            Register();
        }
#endif
#if UNITY_EDITOR
        [InitializeOnLoadMethod]
#endif
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        private static void Register()
        {
            // NOTE(Kas): Multiple paths to Register in Editor because there's some odd stuff happening
            // where the controller will not get recognized at all until a domain reload.
#if UNITY_EDITOR
            if (s_Registered) return;
            s_Registered = true;
#endif
            if (s_HidDescriptorProperty == null)
                throw new InvalidOperationException($"{nameof(DualShockGamepad)}.hidDescriptor not found. The version of the unity input system you are using may be incompatible.");

            InputSystem.RegisterLayout<DualSenseGamepadHID>(
                name: "CyberKomodo.DualSenseGamepadHID", matches:
                new InputDeviceMatcher().WithInterface("HID")
                    .WithManufacturer("Sony.*Entertainment")
                    .WithCapability("vendorId", 0x54C)
                    .WithCapability("productId", 0xCE6)
            );

            // NOTE(Kas): Some weird stuff was happening with unity cold-starts where it would
            // refuse to send the HID output if we removed the old layout, this seems to fix it
            InputSystem.onFindLayoutForDevice -= FindLayoutForDevice;
            InputSystem.onFindLayoutForDevice += FindLayoutForDevice;
        }

        private static string FindLayoutForDevice(ref InputDeviceDescription description, string matchedLayout, InputDeviceExecuteCommandDelegate executeDeviceCommand)
        {
            if (matchedLayout == "DualSenseGamepadHID")
                return "CyberKomodo.DualSenseGamepadHID";
            return null;
        }

        // Implementation of IEventPreProcessor.PreProcessEvent
        // the ILPostProcessor will apply the real unity internal interface
        private unsafe bool PreProcessEvent(InputEventPtr eventPtr)
        {
            if (!eventPtr.IsA<StateEvent>()) return eventPtr.type != DeltaStateEvent.Type;
            StateEvent* stateEvent = (StateEvent*)eventPtr.data;

            // If someone queued DS5S, use as-is
            if (stateEvent->stateFormat == DualSenseHIDState.Format)
                return true;
            uint size = stateEvent->stateSizeInBytes;
            byte reportId = *(byte*)stateEvent->state;

            DualSenseHIDState newState = default;
            bool handled = false;
            switch (reportId)
            {
                case 0x01:
                    {
                        if (size == sizeof(DualSenseHIDBluetoothInput))
                        {
                            // This is not the bluetooth struct, unity allocates the largest input for some reason,
                            // we're receiving a minimal input here
                            HandleMinimalEvent((DualSenseHIDMinimalInput*)stateEvent->state, ref newState);
                            handled = true;
                        }
                        else if (size == sizeof(DualSenseHIDUsbInput))
                        {
                            HandleUSBEvent((DualSenseHIDUsbInput*)stateEvent->state, ref newState);
                            handled = true;
                        }

                        break;
                    }
                case 0x31:
                    {
                        if (size == sizeof(DualSenseHIDBluetoothInput))
                        {
                            HandleBluetoothEvent((DualSenseHIDBluetoothInput*)stateEvent->state, ref newState);
                            handled = true;
                        }

                        break;
                    }
            }
            if (handled)
            {
                // Overriding the state format is important otherwise there will be confusion downstream
                stateEvent->stateFormat = DualSenseHIDState.Format;
                UnsafeUtility.MemCpy(stateEvent->state, UnsafeUtility.AddressOf(ref newState), Math.Min(UnsafeUtility.SizeOf<DualSenseHIDState>(), size));

                return true;
            }
            else
            {
                return false;
            }
        }

        public unsafe void OnStateEvent(InputEventPtr eventPtr)
        {
            InputState.Change(this, eventPtr);
        }
        public void OnNextUpdate()
        {
        }
        public bool GetStateOffsetForEvent(InputControl control, InputEventPtr eventPtr, ref uint offset) => false;

        public override void ResetHaptics() => SetMotorSpeeds(0f, 0f);

        protected override void FinishSetup()
        {
            leftTriggerButton = GetChildControl<ButtonControl>("leftTriggerButton");
            rightTriggerButton = GetChildControl<ButtonControl>("rightTriggerButton");
            playStationButton = GetChildControl<ButtonControl>("systemButton");
            selectButton = GetChildControl<ButtonControl>("select");
            startButton = GetChildControl<ButtonControl>("start");

            base.FinishSetup();

            // NOTE(Kas): This is once again unity internal. Maybe we could parse the JSON ourselves but this will do for now
            HID.HIDDeviceDescriptor desc = (HID.HIDDeviceDescriptor)s_HidDescriptorProperty.GetValue(this);

            m_outputReportSize = desc.outputReportSize;
            m_OutputState = default;

            // Early setup of bluetooth flag
            m_UsingBluetooth = desc.inputReportSize > 64;

            if (!s_HookedEvents)
            {
                Application.quitting += ResetAllControllers;
                InputSystem.onDeviceChange += (device, change) =>
                {
                    if (change == InputDeviceChange.Removed && device is DualSenseGamepadHID ds)
                        ds.ResetState();
                };
#if UNITY_EDITOR
                EditorApplication.playModeStateChanged += (state) =>
                {
                    if (state == PlayModeStateChange.ExitingPlayMode)
                        ResetAllControllers();
                };
#endif
                s_HookedEvents = true;
            }
        }

        private unsafe void ConsumeInputPacket(DualSenseHIDInputPacket* packet, ref DualSenseHIDState state)
        {
            state.leftStickX = packet->leftStickX;
            state.leftStickY = packet->leftStickY;
            state.rightStickX = packet->rightStickX;
            state.rightStickY = packet->rightStickY;
            state.leftTrigger = packet->leftTrigger;
            state.rightTrigger = packet->rightTrigger;
            state.buttonsVector0 = packet->buttonsVector0;
            state.buttonsVector1 = packet->buttonsVector1;
            state.buttonsVector2 = (byte)(packet->buttonsVector2 & 0x07);
            state.gyro = packet->gyro;
            state.accel = packet->accel;
            state.touch0 = packet->touch0;
            state.touch1 = packet->touch1;
            state.rightTriggerFeedback = packet->rightTriggerFeedback;
            state.leftTriggerFeedback = packet->leftTriggerFeedback;
        }
        private unsafe void HandleUSBEvent(DualSenseHIDUsbInput* usbdata, ref DualSenseHIDState state)
        {
            ConsumeInputPacket(&usbdata->packet, ref state);
            m_UsingBluetooth = false;
        }
        private unsafe void HandleMinimalEvent(DualSenseHIDMinimalInput* mindata, ref DualSenseHIDState state)
        {
            state.leftStickX = mindata->leftStickX;
            state.leftStickY = mindata->leftStickY;
            state.rightStickX = mindata->rightStickX;
            state.rightStickY = mindata->rightStickY;
            state.leftTrigger = mindata->leftTrigger;
            state.rightTrigger = mindata->rightTrigger;
            state.buttonsVector0 = mindata->buttons0;
            state.buttonsVector1 = mindata->buttons1;
            state.buttonsVector2 = (byte)(mindata->buttons2 & 0x03);

            m_UsingBluetooth = true;
        }
        private unsafe void HandleBluetoothEvent(DualSenseHIDBluetoothInput* btdata, ref DualSenseHIDState state)
        {
            ConsumeInputPacket(&btdata->packet, ref state);
            m_UsingBluetooth = true;
        }

        /// <summary>
        /// Sets the player lights on the controller to the player number
        /// </summary>
        /// <param name="player">Player number. Set to 0 to turn off</param>
        public void SetPlayerLight(byte player, LightBrightness brightness = LightBrightness.High, bool fade = true)
        {
            m_OutputState.AllowPlayerIndicators = true;
            m_OutputState.AllowLightBrightnessChange = true;
            m_OutputState.playerLightVector = 0;

            switch (player)
            {
                case 0x01:
                    m_OutputState.PlayerLight3 = true;
                    break;
                case 0x02:
                    m_OutputState.PlayerLight2 = true;
                    m_OutputState.PlayerLight4 = true;
                    break;
                case 0x03:
                    m_OutputState.PlayerLight2 = true;
                    m_OutputState.PlayerLight3 = true;
                    m_OutputState.PlayerLight4 = true;
                    break;
                case 0x04:
                    m_OutputState.PlayerLight1 = true;
                    m_OutputState.PlayerLight2 = true;
                    m_OutputState.PlayerLight4 = true;
                    m_OutputState.PlayerLight5 = true;
                    break;
                default: break;
            }
            m_OutputState.lightBrightness = brightness;
            m_OutputState.PlayerLightDisableFade = !fade;

            SendState();
        }

        public override void SetLightBarColor(Color color)
        {
            m_OutputState.AllowLedColor = true;
            m_OutputState.ledR = (byte)NormalizedFloatToUInt(color.r, byte.MinValue, byte.MaxValue);
            m_OutputState.ledG = (byte)NormalizedFloatToUInt(color.g, byte.MinValue, byte.MaxValue);
            m_OutputState.ledB = (byte)NormalizedFloatToUInt(color.b, byte.MinValue, byte.MaxValue);
            _ = SendState();
        }

        public override void SetMotorSpeeds(float lowFrequency, float highFrequency)
        {
            m_OutputState.EnableRumbleEmulation = true;
            m_OutputState.UseRumbleNotHaptics = true;
            m_OutputState.EnableImprovedRumbleEmulation = true;
            m_OutputState.rumbleLight = (byte)NormalizedFloatToUInt(highFrequency, byte.MinValue, byte.MaxValue);
            m_OutputState.rumbleHeavy = (byte)NormalizedFloatToUInt(lowFrequency, byte.MinValue, byte.MaxValue);
            _ = SendState();
        }

        /// <param name="sides">The sides of the triggers the effect should be applied to</param>
        /// <inheritdoc cref="TriggerEffect.Off()"/>
        public bool SetTriggerEffectOff(TriggerSide sides)
            => SetTriggerEffect(sides, TriggerEffect.Off());

        /// <param name="sides">The sides of the triggers the effect should be applied to</param>
        /// <inheritdoc cref="TriggerEffect.Feedback(byte, byte)"/>
        public bool SetTriggerEffectFeedback(TriggerSide sides, byte position, byte strength)
            => SetTriggerEffect(sides, TriggerEffect.Feedback(position, strength));

        /// <param name="sides">The sides of the triggers the effect should be applied to</param>
        /// <inheritdoc cref="TriggerEffect.Weapon(byte, byte, byte)"/>
        public bool SetTriggerEffectWeapon(TriggerSide sides, byte startPosition, byte endPosition, byte strength)
            => SetTriggerEffect(sides, TriggerEffect.Weapon(startPosition, endPosition, strength));

        /// <param name="sides">The sides of the triggers the effect should be applied to</param>
        /// <inheritdoc cref="TriggerEffect.Vibration(byte, byte, byte)"/>
        public bool SetTriggerEffectVibration(TriggerSide sides, byte position, byte strength, byte frequency)
            => SetTriggerEffect(sides, TriggerEffect.Vibration(position, strength, frequency));

        public bool SetTriggerEffect(TriggerSide sides, in TriggerEffect effect)
        {
            if ((sides & TriggerSide.Left) != 0)
            {
                m_OutputState.AllowLeftTriggerFFB = true;
                m_OutputState.leftTrigger = effect;
            }
            if ((sides & TriggerSide.Right) != 0)
            {
                m_OutputState.AllowRightTriggerFFB = true;
                m_OutputState.rightTrigger = effect;
            }
            return SendState();
        }
        private static void ResetAllControllers()
        {
            foreach (var device in InputSystem.devices)
            {
                if (device is DualSenseGamepadHID ds)
                    ds.ResetState();
            }
        }

        private void ResetState()
        {
            DualSenseHIDOutputPacket packet = default;
            packet.AllowRightTriggerFFB = true;
            packet.AllowLeftTriggerFFB = true;
            packet.AllowLedColor = true;
            packet.AllowPlayerIndicators = true;
            packet.AllowLightBrightnessChange = true;
            packet.EnableRumbleEmulation = true;
            packet.UseRumbleNotHaptics = true;
            packet.EnableImprovedRumbleEmulation = true;

            // NOTE(Kas): Should we preserve the old color?

            packet.leftTrigger = TriggerEffect.Off();
            packet.rightTrigger = TriggerEffect.Off();

            m_OutputState = packet;
            SendState();
        }

        private unsafe bool SendState()
        {
            if (m_UsingBluetooth)
            {
                return SendPacket(DualSenseHIDBluetoothOutput.Create(m_OutputState, ref m_seqId)) >= 0;
            }
            else
            {
                return SendPacket(DualSenseHIDUsbOutput.Create(m_OutputState)) >= 0;
            }
        }

        private unsafe long SendPacket<TPacket>(in TPacket packet) where TPacket : unmanaged
        {
            // NOTE(Kas): This entire dance is required because HIDO requires the command to be the size of outputReportSize
            // from the descriptor so we must allocate a buffer for it then throw our (usually) smaller command into it
            int size = InputDeviceCommand.BaseCommandSize + m_outputReportSize;

#if UNITY_EDITOR
            Debug.Assert(sizeof(TPacket) <= size, "sizeof(TPacket) is bigger than the maximum output size allowed for 'HIDO'");
#endif
            byte* buf = stackalloc byte[size];
            UnsafeUtility.MemClear(buf, size);
            InputDeviceCommand* cmd = (InputDeviceCommand*)buf;
            *cmd = new(s_HidOutputFourCC, size);
            *(TPacket*)(buf + InputDeviceCommand.BaseCommandSize) = packet;
            return ExecuteCommand((InputDeviceCommand*)buf);
        }

        // another unity internal method
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint NormalizedFloatToUInt(float value, uint uintMinValue, uint uintMaxValue)
        {
            if (value <= 0.0f)
                return uintMinValue;
            if (value >= 1.0f)
                return uintMaxValue;
            return (uint)(value * ((double)uintMaxValue - uintMinValue) + uintMinValue);
        }

        private bool m_UsingBluetooth = false;
        private DualSenseHIDOutputPacket m_OutputState = default;
        private byte m_seqId;
        private int m_outputReportSize;
        private static readonly FourCC s_HidOutputFourCC = new('H', 'I', 'D', 'O');
        private static readonly PropertyInfo s_HidDescriptorProperty = typeof(DualShockGamepad).GetProperty("hidDescriptor", BindingFlags.NonPublic | BindingFlags.Instance);
        private static bool s_HookedEvents = false;
#if UNITY_EDITOR
        private static bool s_Registered = false;
#endif
    }

}
# DualSense integration for Unity's InputSystem

A drop-in replacement for the existing DualSense implementation for Unity's Input System with Bluetooth support.

This project is by no means complete and does not support other cool things you can do with a DualSense controller.

We have tested (and are actively testing) this project during the development of [Acrolyte](https://store.steampowered.com/app/3459050/Acrolyte/)

If you wish to implement a feature or contribute anything, submit a PR, all contributions are welcome.

## Features
`CyberKomodo.DualSense` supports all the inputs from the DualSense controller and is backwards compatible with Unity's default implementation.

The key additions are **Bluetooth** support which Unity's implementation only supports for input, limiting the possibilities of what you can do.

#### Supported outputs:
 - Adaptive triggers
 - RGB Lightbar
 - Player light customization
 - Controller rumble (DualShock-style motors)

## Compatibility
At the moment we have only tested this on Windows with Unity 6000.0.48f1 with v1.14.0 of Unity's Input System.

There are multiple Unity internal APIs used that may be prone to breaking in future versions (or are not present in older ones).

## Installation
You can install this package via the Unity Package Manager
1. In the Unity Editor, head over to Window -> Package Manager
2. Click the `+` and select `Add package from git URL...`
3. Enter `https://github.com/CyberKomodo/CyberKomodo.DualSense.git`

## Usage / Quick Start
```csharp
using CyberKomodo;
using UnityEngine.InputSystem;

if (Gamepad.current is CyberKomodo.DualSenseGamepadHID dualsense) {
    // NOTE: The output state is sticky. Any effects you apply will be active until you modify them yourself.

    // Set the lightbar color
    dualsense.SetLightBarColor(Color.yellow);

    // Various trigger effects
    dualsense.SetTriggerEffectWeapon(TriggerSide.Both, startPosition: 2, endPosition: 8, strength: 8);
    dualsense.SetTriggerEffectVibration(TriggerSide.Both, position: 2, strength: 8, frequency: 11);
    dualsense.SetTriggerEffectFeedback(TriggerSide.Both, position: 0, strength: 8);
    dualsense.SetTriggerEffectOff(TriggerSide.Both);

    // Player light controls
    dualsense.SetPlayerLight(1, LightBrightness.High);
    dualsense.SetPlayerLight(2, LightBrightness.Low);
    dualsense.SetPlayerLight(3, LightBrightness.Low, fade: true);
    
    // Classic Unity compatible DualShock rumble
    // NOTE: Due to the fact the state is sticky, you must update the motor rumble frequently to keep it active
    // as the controller will stop on its own if it doesn't get a state update. This also has a side effect of
    // retriggering the motor rumble if you never clear the haptics either through SetMotorSpeeds(0f, 0f) or ResetHaptics.
    dualsense.SetMotorSpeeds(1f, 1f);
}
```
## Limitations
 - Touchpad is parsed but no unity control is exposed
 - Battery state not exposed
 - Trigger feedback readback not exposed

## Credits
- https://github.com/nondebug/dualsense
- https://gist.github.com/dogtopus/894da226d73afb3bdd195df41b3a26aa
- https://github.com/nullkal/UniSense
- Existing DualSense implementation in Unity's Input System
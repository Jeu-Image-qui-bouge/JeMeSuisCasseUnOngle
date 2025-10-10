using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public static class HapticPulseVR
{
    static InputDevice left, right;
    static bool devicesSearched;

    static void EnsureDevices()
    {
        if (devicesSearched && left.isValid && right.isValid) return;

        var list = new List<InputDevice>();
        InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.HeldInHand | InputDeviceCharacteristics.Controller, list);

        left = default;
        right = default;

        foreach (var d in list)
        {
            if (!d.isValid) continue;
            var ch = d.characteristics;
            if ((ch & InputDeviceCharacteristics.Left) != 0) left = d;
            if ((ch & InputDeviceCharacteristics.Right) != 0) right = d;
        }
        devicesSearched = true;
    }

    /// <summary>Impulse sur la main demandée.</summary>
    public static void Pulse(bool leftHand, float amplitude = 0.6f, float duration = 0.08f)
    {
        EnsureDevices();
        var dev = leftHand ? left : right;
        if (dev.isValid)
        {
            // OpenXR / XRIT: canal 0
            dev.SendHapticImpulse(0u, Mathf.Clamp01(amplitude), Mathf.Max(0f, duration));
        }

        // Fallback Oculus (si Oculus Integration est importée)
#if OCULUS_INTEGRATION
        var ctrl = leftHand ? OVRInput.Controller.LTouch : OVRInput.Controller.RTouch;
        OVRInput.SetControllerVibration(1f, Mathf.Clamp01(amplitude), ctrl);
        // Stop programmé
        StopAfter(duration, ctrl);
#endif
    }

    /// <summary>Impulse sur les deux mains.</summary>
    public static void PulseBoth(float amplitude = 0.6f, float duration = 0.08f)
    {
        EnsureDevices();
        if (left.isValid) left.SendHapticImpulse(0u, Mathf.Clamp01(amplitude), Mathf.Max(0f, duration));
        if (right.isValid) right.SendHapticImpulse(0u, Mathf.Clamp01(amplitude), Mathf.Max(0f, duration));

#if OCULUS_INTEGRATION
        OVRInput.SetControllerVibration(1f, Mathf.Clamp01(amplitude), OVRInput.Controller.LTouch);
        OVRInput.SetControllerVibration(1f, Mathf.Clamp01(amplitude), OVRInput.Controller.RTouch);
        StopAfter(duration, OVRInput.Controller.LTouch | OVRInput.Controller.RTouch);
#endif
    }

#if OCULUS_INTEGRATION
    static async void StopAfter(float d, OVRInput.Controller c)
    {
        if (d <= 0f) return;
        await System.Threading.Tasks.Task.Delay((int)(d * 1000f));
        OVRInput.SetControllerVibration(0f, 0f, c);
    }
#endif
}

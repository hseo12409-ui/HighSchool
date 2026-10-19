using System;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.HableCurve;

[Serializable]
public class GuideUIform : MonoBehaviour
{
    [SerializeField, Tooltip("¼ø¼­ u, d, l, r / y, a, x, b / L1, L2, R1, R2 / L, R")] int guideName;
        
    public Sprite keyboard;
    public Sprite controller;

    [ContextMenu("SetSprite")]
    public void SetSprite()
    {
        keyboard = UIManager.instance.keyboardMouseGuiedUI[guideName];
        controller = UIManager.instance.controllerGuiedUI[guideName];
    }
}

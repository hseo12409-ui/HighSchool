using UnityEngine;

public enum ControlTypeEnum
{
    KEYBOARD,
    GAMEPAD
}

public class PlayerInputManager : MonoBehaviour
{
    public static PlayerInputManager instance;

    public ControlTypeEnum controlType = ControlTypeEnum.KEYBOARD;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    public void VisibleCursor(bool isVisible)
    {
        Cursor.visible = isVisible;
    }

    public void LockCursor(bool isLocked)
    {
        if (isLocked) Cursor.lockState = CursorLockMode.Locked;
        else Cursor.lockState = CursorLockMode.None;
    }

    public void KeyboardAnyKey()
    {
        if (controlType != ControlTypeEnum.KEYBOARD)
        {
            controlType = ControlTypeEnum.KEYBOARD;
            if (Cursor.visible) Cursor.lockState = CursorLockMode.None;
            else
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = false;
            }
            UIManager.instance.ChangeGuideUI(false);
        }
    }

    public void GamepdAnyKey()
    {
        if (controlType != ControlTypeEnum.GAMEPAD)
        {
            controlType = ControlTypeEnum.GAMEPAD;            
            if (Cursor.visible) Cursor.lockState = CursorLockMode.Locked;
            else
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = false;
            }
            UIManager.instance.ChangeGuideUI(true);
        }
    }
}

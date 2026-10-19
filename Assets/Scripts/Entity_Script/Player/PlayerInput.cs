using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    [SerializeField, Tooltip("네모 버튼 누르기 시작한 시점")] float squareStart;                                       //짧게 누르기 : 아이템 사용, 길게 누르기 : 아이템 버리기
    [SerializeField, Tooltip("버리기 UI보이기 시작")] bool isDropStart;
    [SerializeField, Tooltip("버리기 시작")] bool isDropKeyPress;

    [SerializeField, Tooltip("o 버튼 누르기 시작한 시점")] float circleStart;                                       //짧게 누르기 : 포션 제작, 길게 누르기 : 포션 제작 취소
    [SerializeField, Tooltip("o 버튼 누른 길이")] float circlePressTime;                                       //짧게 누르기 : 포션 제작, 길게 누르기 : 포션 제작 취소
    [SerializeField, Tooltip("버리기 시작")] bool isCancelStart;
    [SerializeField, Tooltip("버리기 시작")] bool isCancelKeyPress;
    [SerializeField] GameObject circlePressUI;

    Player P;
    Climb Pclimb;
    PlayerMoveStuff Pstuff;
    PlayerObserve Pobserve;
    PlayerParticle Pparticle;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        P = GetComponent<Player>();
        Pclimb = GetComponent<Climb>();
        Pstuff = GetComponent<PlayerMoveStuff>();
        Pobserve = GetComponent<PlayerObserve>();
        Pparticle = GetComponent<PlayerParticle>();        
    }

    private void FixedUpdate()
    {
        if (isDropKeyPress && !isDropStart && Time.time - squareStart > 0.5f)
        {
            isDropStart = true;
            ItemManager.instance.RenewItemCanUse(true, 0);
            ItemManager.instance.ItemDrop(true, 0, P.itemDropPosition);
        }

        if (isCancelKeyPress && !isCancelStart && Time.time - circleStart > 0.5f)
        {
            isCancelStart = true;
            UIManager.instance.KeyPress(circlePressUI, true);
        }
    }

    public void KeyboardAnyKey(InputAction.CallbackContext context)
    {
        PlayerInputManager.instance.KeyboardAnyKey();
    }

    public void GamepdAnyKey(InputAction.CallbackContext context)
    {
        PlayerInputManager.instance.GamepdAnyKey();
    }

    public void OnMove(InputAction.CallbackContext context)
    {        
        P.OnMove(context.ReadValue<Vector2>());
    }

    public void OnAim(InputAction.CallbackContext context)
    {
        Vector2 aimInput = context.ReadValue<Vector2>();        

        Debug.Log($"aimInput : {aimInput}");
        P.OnAim(aimInput);
    }

    public void OnMouseAim(InputAction.CallbackContext context)
    {
        PlayerInputManager.instance.KeyboardAnyKey();
        Vector2 aimInput = context.ReadValue<Vector2>() / 10;
        //Debug.Log(aimInput);

        if (Mathf.Abs(aimInput.x) > 1)
        {
            aimInput.x = Mathf.Sign(aimInput.x);
        }

        if (Mathf.Abs(aimInput.y) > 1)
        {
            aimInput.y = Mathf.Sign(aimInput.y);
        }

        //aimInput = aimInput.normalized;        
        P.OnMouseAim(aimInput);
    }

    public void OnMousePosition(InputAction.CallbackContext context)
    {
        P.SetMousePosition(context.ReadValue<Vector2>());
    }

    public void Scroll(InputAction.CallbackContext context)
    {
        float scrollInput = context.ReadValue<float>();
        if (context.performed && scrollInput != 0)
        {
            if (!P.isaim && P.currentState == PlayerState.NORMAL) ItemManager.instance.SlotChange((int)Mathf.Sign(scrollInput));
            if (!P.isaim && P.currentState == PlayerState.READGUIDE) UIManager.instance.ChangePage((int)Mathf.Sign(scrollInput));
            if (!P.isaim && P.currentState == PlayerState.SELECTTRAVELSTOP) TravelManager.instance.ChangeTravelStop((int)Mathf.Sign(scrollInput));
        }
    }

    public void OnX(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            P.Jump();            
            PotionManager.instance.AddHerbEnd();
            if (!P.isaim && P.currentState == PlayerState.SELECTTRAVELSTOP) TravelManager.instance.TravelStart();
        }
    }

    public void OnTriangle(InputAction.CallbackContext context)             //세모
    {
        if (context.performed)
        {
            P.OnInteraction();
        }
    }

    public void OnSquare(InputAction.CallbackContext context)             //네모
    {
        if (context.performed)
        {
            squareStart = Time.time;
            isDropStart = false;
            isDropKeyPress = true;            
        }
        else if (context.canceled)
        {
            isDropKeyPress = false;
            P.OnSquareEnd(Time.time - squareStart);
        }
    }

    public void OnCircle(InputAction.CallbackContext context)             //원(뒤로가기, 취소)
    {
        if (context.performed)
        {
            circleStart = Time.time;
            isCancelKeyPress = true;
            isCancelStart = false;

            if (P.currentState == PlayerState.READGUIDE) UIManager.instance.GuideOff();
            if (!P.isaim && P.currentState == PlayerState.SELECTTRAVELSTOP) TravelManager.instance.TravelStopSelectEnd();
        }
        else if (context.canceled)
        {
            circlePressTime = Time.time - circleStart;
            isCancelKeyPress = false;
            isCancelStart = false;

            UIManager.instance.KeyPress(circlePressUI, false);
            if (circlePressTime > 1.5f)
            {
                PotionManager.instance.StopMakePotion();
            }            
        }        
    }

    public void OnUp(InputAction.CallbackContext context)
    {
        if (context.performed)
        {            
            if (!P.isaim && P.currentState == PlayerState.NORMAL)
            {                
                ItemManager.instance.SlotTypeChange();
            }
        }
    }

    public void OnDown(InputAction.CallbackContext context)
    {
        if (context.performed)
        {            
            if (!P.isaim && P.currentState == PlayerState.NORMAL)
            {                
                ItemManager.instance.SlotTypeChange();
            }
        }
    }

    public void OnLeft(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            
        }
    }

    public void OnRight(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            
        }
    }

    public void L1(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (!P.isaim && (P.currentState == PlayerState.NORMAL || P.currentState == PlayerState.MAKEPOTION)) ItemManager.instance.SlotChange(-1);
            if (!P.isaim && P.currentState == PlayerState.READGUIDE) UIManager.instance.ChangePage(-1);
            if (!P.isaim && P.currentState == PlayerState.SELECTTRAVELSTOP) TravelManager.instance.ChangeTravelStop(-1);
        }
    }

    public void L2(InputAction.CallbackContext context)                     //조준
    {
        if (P.currentState == PlayerState.CLIMB)
        {
            if (context.performed)
            {
                Pclimb.ClimbUp(-1);
            }
            else if (context.canceled)
            {
                Pclimb.ClimbDown(-1);
            }
        }
        else
        {
            if (ItemManager.instance.selectInventoryType == 1)                           //장비 인벤이 선택됐는지, 선택된 아이템이 지팡이인지 확인
            {
                int equipmentCode = ItemManager.instance.equipmentList[ItemManager.instance.selectEquipmentSlot];
                if (!Pstuff.isstuffcharge || Pstuff.canmovestuff)
                {
                    if (context.performed)
                    {
                        P.AimEquipmentStart(equipmentCode);
                    }
                    else if (context.canceled)
                    {
                        Debug.Log("canceled");
                        P.AimEquipmentEnd(equipmentCode);
                    }
                }                                    
            }
        }
    }

    public void L3(InputAction.CallbackContext context)
    {

    }

    public void R1(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (!P.isaim && (P.currentState == PlayerState.NORMAL || P.currentState == PlayerState.MAKEPOTION)) ItemManager.instance.SlotChange(1);
            if (!P.isaim && P.currentState == PlayerState.READGUIDE) UIManager.instance.ChangePage(1);
            if (!P.isaim && P.currentState == PlayerState.SELECTTRAVELSTOP) TravelManager.instance.ChangeTravelStop(1);
        }
    }

    public void R2(InputAction.CallbackContext context)
    {
        if (P.currentState == PlayerState.CLIMB)
        {
            if (context.performed)
            {
                Pclimb.ClimbUp(1);
            }
            else if (context.canceled)
            {
                Pclimb.ClimbDown(1);
            }
        }
        else
        {
            if (ItemManager.instance.selectInventoryType == 1 && P.currentState != PlayerState.CLIMB)                           //장비 인벤이 선택됐는지, 선택된 아이템이 관찰인지 확인
            {
                int equipmentCode = ItemManager.instance.equipmentList[ItemManager.instance.selectEquipmentSlot];
                if (context.performed)
                {
                    P.UseEquipmentStart(equipmentCode);
                }
                else if (context.canceled)
                {
                    P.UseEquipmentEnd(equipmentCode);
                }
            }            
        }
    }    

    public void R3(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Pstuff.StuffTurn();
        }
    }

    //---------------------아래로 potion키---------------------------------

    public void OnPotionX(InputAction.CallbackContext context)
    {
        if (context.performed && !PotionManager.instance.addHerb)
        {
            PotionManager.instance.Makepotion('a');
        }
    }

    public void OnPotionSquare(InputAction.CallbackContext context)
    {
        if (context.performed && !PotionManager.instance.addHerb)
        {
            PotionManager.instance.Makepotion('x');
        }
    }

    public void OnPotionTriangle(InputAction.CallbackContext context)
    {
        if (context.performed && !PotionManager.instance.addHerb)
        {
            PotionManager.instance.Makepotion('y');
        }
    }

    public void OnPotionCircle(InputAction.CallbackContext context)
    {
        if (context.canceled && !PotionManager.instance.addHerb)
        {
            if (circlePressTime < 1.5f)
            {
                PotionManager.instance.Makepotion('b');
            }            
        }
    }

    public void OnPotionUp(InputAction.CallbackContext context)
    {
        if (context.performed && !PotionManager.instance.addHerb)
        {
            PotionManager.instance.Makepotion('u');
        }
    }

    public void OnPotionDown(InputAction.CallbackContext context)
    {
        if (context.performed && !PotionManager.instance.addHerb)
        {
            PotionManager.instance.Makepotion('d');
        }
    }

    public void OnPotionLeft(InputAction.CallbackContext context)
    {
        if (context.performed && !PotionManager.instance.addHerb)
        {
            PotionManager.instance.Makepotion('l');
        }
    }

    public void OnPotionRight(InputAction.CallbackContext context)
    {
        if (context.performed && !PotionManager.instance.addHerb)
        {
            PotionManager.instance.Makepotion('r');
        }
    }
}

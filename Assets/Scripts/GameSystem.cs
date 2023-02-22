using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EventParameters;
using EventEnums;
using UnityEngine.InputSystem;

public class GameSystem : MonoBehaviour, PlayerControls.IGameSystemActions
{
    public static GameSystem Instance { get; private set; }
    private PlayerControls playerControls;
    EventParam evt;

    private bool buildMode = false;
    private int currency = 0;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There is more than one GameObject instance!");
            return;
        }
        Instance = this;
        playerControls = new PlayerControls();
        playerControls.GameSystem.SetCallbacks(this);
    }
    private void OnEnable() { playerControls.Enable(); }
    private void OnDisable() { playerControls.Disable(); }

    public int GetCurrency()
    {
        return currency;
    }

    public void SetCurrency(int value)
    {
        currency = value; 
        EventManager.TriggerEvent(GameEventType.CurrencyChange, evt);
    }

    public void AddCurrency(int value)
    {
        currency += value;
        EventManager.TriggerEvent(GameEventType.CurrencyChange, evt);
    }

    public void OnBuildMode(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            buildMode = !buildMode;
            EventParam param = new EventParam(buildMode);
            EventManager.TriggerEvent(GameEventType.ToggleBuildMode, param);
        }
    }
}

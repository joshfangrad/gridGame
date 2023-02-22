using System;
using UnityEngine;
using EventParameters;
using UnityEngine.UI;
using EventEnums;

public class UIController : MonoBehaviour
{
    public static UIController instance { get; private set; }
    private Action<EventParam> toggleBuildModeListener;
    private Action<EventParam> currencyChangeListener;
    [SerializeField]
    private GameObject buildModeText, currencyText;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("There is already a UIController instance!");
            return;
        }
        instance = this;

    }
    void Start()
    {
        buildModeText.SetActive(false);
        //call CurrencyChange once to set the initial balance
        EventParam evt = new EventParam();
        CurrencyChange(evt);
    }
    private void OnEnable()
    {
        toggleBuildModeListener = new Action<EventParam>(ToggleBuildMode);
        EventManager.StartListening(GameEventType.ToggleBuildMode, toggleBuildModeListener);

        currencyChangeListener = new Action<EventParam>(CurrencyChange);
        EventManager.StartListening(GameEventType.CurrencyChange, currencyChangeListener);
    }
    private void OnDestroy()
    {
        EventManager.StopListening(GameEventType.ToggleBuildMode, toggleBuildModeListener);

        EventManager.StopListening(GameEventType.CurrencyChange, currencyChangeListener);
    }

    private void ToggleBuildMode(EventParam param)
    {
        buildModeText.SetActive(param.state);
    }
    private void CurrencyChange(EventParam param)
    {
        int currency = GameSystem.Instance.GetCurrency();
        Text currencyTextComponent = currencyText.GetComponent<Text>();
        currencyTextComponent.text = $"Currency: {currency}";
    }
}

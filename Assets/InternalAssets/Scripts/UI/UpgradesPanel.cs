using EventBusSystem;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

public class UpgradesPanel : MonoBehaviour, IGameStateChangedSubscriber
{
    [SerializeField] private RectTransform _panelRect;

    [SerializeField] private Button _damageUpgradeButton;
    [SerializeField] private TextMeshProUGUI _damageUpgradeText;

    [SerializeField] private Button _speedUpgradeButton;
    [SerializeField] private TextMeshProUGUI _speedUpgradeText;

    [SerializeField] private Button _rangeUpgradeButton;
    [SerializeField] private TextMeshProUGUI _rangeUpgradeText;

    private int _maxDamageUpgr = 2;
    private int _currentDamageUpgr = 0;

    private int _maxSpeedUpgr = 2;
    private int _currentSpeedUpgr = 0;

    private int _maxRangeUpgr = 2;
    private int _currentRangeUpgr = 0;

    private void Awake()
    {
        _damageUpgradeButton.onClick.AddListener(OnDamageUpgradeButtonPressed);
        _speedUpgradeButton.onClick.AddListener(OnSpeedUpgradeButtonPressed);
        _rangeUpgradeButton.onClick.AddListener(OnRangeUpgradeButtonPressed);

        _damageUpgradeText.text = 1.ToString();
        _speedUpgradeText.text = 1.ToString();
        _rangeUpgradeText.text = 1.ToString();
    }

    private void OnEnable()
    {
        EventBus.Subscribe(this);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe(this);
    }

    private void OnDamageUpgradeButtonPressed()
    {
        if(_currentDamageUpgr < _maxDamageUpgr)
        {
            _currentDamageUpgr ++;
            _damageUpgradeText.text = (_currentDamageUpgr + 1).ToString();
            EventBus.RaiseEvent<IUpgradeSubscriber>(h => h.HandleDamageUpgrade(_currentDamageUpgr));
        }
    }

    private void OnSpeedUpgradeButtonPressed()
    {
        if (_currentSpeedUpgr < _maxSpeedUpgr)
        {
            _currentSpeedUpgr++;
            _speedUpgradeText.text = (_currentSpeedUpgr + 1).ToString();
            EventBus.RaiseEvent<IUpgradeSubscriber>(h => h.HandleSpeedUpgrade(_currentSpeedUpgr));
        }
    }

    private void OnRangeUpgradeButtonPressed()
    {
        if (_currentRangeUpgr < _maxRangeUpgr)
        {
            _currentRangeUpgr++;
            _rangeUpgradeText.text = (_currentRangeUpgr + 1).ToString();
            EventBus.RaiseEvent<IUpgradeSubscriber>(h => h.HandleRangeUpgrade(_currentRangeUpgr));
        }
    }

    public void HandleGameOver()
    {
        _panelRect.transform.localScale = Vector3.zero;
    }

    public void HandleGameRestart()
    {
        _damageUpgradeText.text = 1.ToString();
        _speedUpgradeText.text = 1.ToString();
        _rangeUpgradeText.text = 1.ToString();

        _panelRect.transform.localScale = Vector3.one;
    }
}
using EventBusSystem;

using UnityEngine;
using UnityEngine.UI;

public class GameOverPanel : MonoBehaviour, IGameStateChangedSubscriber
{
    [SerializeField] private RectTransform _rect;
    [SerializeField] private Button _restartButton;

    private void Awake()
    {
        _restartButton.onClick.AddListener(OnRestartButtonPressed);
    }

    private void OnEnable()
    {
        EventBus.Subscribe(this);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe(this);
    }

    private void OnRestartButtonPressed()
    {
        EventBus.RaiseEvent<IGameStateChangedSubscriber>(h => h.HandleGameRestart());
    }

    public void HandleGameOver()
    {
        _rect.transform.localScale = Vector3.one;
    }

    public void HandleGameRestart()
    {
        _rect.transform.localScale = Vector3.zero;
    }
}
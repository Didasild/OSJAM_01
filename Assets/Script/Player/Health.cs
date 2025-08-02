using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    private static readonly int MaxHealth = Shader.PropertyToID("_MaxHealth");
    private static readonly int CurrentHealth = Shader.PropertyToID("_CurrentHealth");

    #region FIELDS
    [Header("HEALTH")]
    [ReadOnly][SerializeField] private int _currentHealth;
    [SerializeField] private int _baseHealth = 1;
    [SerializeField] private int maxHealthPoints = 2;
    
    [Header("VISUAL")]
    [SerializeField] private Image healthBar;
    #endregion

    private Material _healthBarMaterial;
    private Player _player;
    private VisualManager _visualManager;

    public void Init(Player player, VisualManager visualManager)
    {
        _player = player;
        _currentHealth = _baseHealth;
        _visualManager = visualManager;
        _healthBarMaterial = healthBar.material;
        UpdateMaxHealthVisual(maxHealthPoints);
        UpdateHealthPointVisual(_currentHealth);
    }
    
    public void ResetHealthPoint()
    {
        _currentHealth = _baseHealth;
        UpdateHealthPointVisual(_currentHealth);
        CheckCurrentLifeActions();
    }

    public void SetCurrentHealth(int healthPoints)
    {
        _currentHealth = healthPoints;
        UpdateHealthPointVisual(_currentHealth);
        CheckCurrentLifeActions();
    }

    public void DecreaseHealth(int damage)
    {
        _currentHealth -= damage;
        UpdateHealthPointVisual(_currentHealth, false);
        
        _visualManager.fullScreenFeedbackController.HitFeedback();
        _visualManager.shakeCamController.LittleShakeCamera();
        
        CheckCurrentLifeActions();
    }

    public void IncreaseHealth(int heal)
    {
        if (_currentHealth >= maxHealthPoints)
        {
            return;
        }
        _currentHealth += heal;
        
        CheckCurrentLifeActions();
        UpdateHealthPointVisual(_currentHealth);
    }

    public void IncreaseMaxHealth(int newPoints)
    {
        maxHealthPoints += newPoints;
        IncreaseHealth(newPoints);
        UpdateHealthPointVisual(_currentHealth);
        UpdateMaxHealthVisual(maxHealthPoints);
    }

    private void UpdateHealthPointVisual(int currentHealth, bool increase = true)
    {
        Ease ease = increase ? Ease.OutExpo : Ease.OutBounce;
        
        if (_healthBarMaterial.HasProperty(CurrentHealth))
        {
            _visualManager.FadeProperty(_healthBarMaterial, "_CurrentHealth", currentHealth, 0.4f, 0f, ease);
        }
    }

    private void UpdateMaxHealthVisual(int newMaxHealth)
    {
        if (_healthBarMaterial.HasProperty(MaxHealth))
        {
            _healthBarMaterial.SetFloat(MaxHealth, newMaxHealth);
        }
    }

    private void CheckCurrentLifeActions()
    {
        switch (_currentHealth)
        {
            case > 1:
                _visualManager.fullScreenFeedbackController.LowLifeFeedback(false);
                break;
            case 1:
                _visualManager.fullScreenFeedbackController.LowLifeFeedback(true);
                break;
            case <= 0:
                DeathSequence();
                break;
        }
    }

    private void DeathSequence()
    {
        GameManager.Instance.FloorManager.eventsController.RtcManager.StopRtc();
        _visualManager.fullScreenFeedbackController.LowLifeFeedback(false);
        _visualManager.shakeCamController.MidShakeCamera(0.8f);
        DOVirtual.DelayedCall(0.5f, () =>
        {
            _visualManager.fullScreenFeedbackController.DeathCloseScreenFeedback();
        }).OnComplete(() =>
        {
            DOVirtual.DelayedCall(1f,()=>
            {
                GameManager.Instance.ChangeGameState(GameState.Lose);
            });
        });
    }
}

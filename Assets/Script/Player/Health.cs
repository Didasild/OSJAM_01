using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    private static readonly int MaxHealth = Shader.PropertyToID("_MaxHealth");
    private static readonly int CurrentHealth = Shader.PropertyToID("_CurrentHealth");

    [Header("HEALTH")]
    [ReadOnly][SerializeField] private int _currentHealth;
    [ReadOnly][SerializeField] private int maxHealthPoints = 3;
    
    [Header("VISUAL")]
    [SerializeField] private Image healthBar;

    private Material _healthBarMaterial;
    private Player _player;
    private VisualManager _visualManager;

    public void Init(Player player, VisualManager visualManager)
    {
        _player = player;
        _currentHealth = maxHealthPoints;
        _visualManager = visualManager;
        _healthBarMaterial = healthBar.material;
        UpdateMaxHealthVisual(maxHealthPoints);
        UpdateHealthPointVisual(maxHealthPoints);
    }
    
    public void ResetHealthPoint()
    {
        _currentHealth = maxHealthPoints;
        UpdateHealthPointVisual(_currentHealth);
    }

    public void DecreaseHealth(int damage)
    {
        _currentHealth -= damage;
        UpdateHealthPointVisual(_currentHealth, false);
        
        _visualManager.PlayHitFeedbacks();
        
        if (_currentHealth <= 0)
        {
            GameManager.Instance.ChangeGameState(GameState.Lose);
        }
    }

    public void IncreaseHealth(int heal)
    {
        if (_currentHealth >= maxHealthPoints)
        {
            return;
        }
        _currentHealth += heal;
        UpdateHealthPointVisual(_currentHealth);
    }

    public void IncreaseMaxHealth(int newPoints)
    {
        maxHealthPoints += newPoints;
        _currentHealth += newPoints;
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
}

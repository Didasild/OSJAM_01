using TMPro;
using UnityEngine;

public class Health : MonoBehaviour
{
    [Header("HEALTH")]
    public int initialHealthPoints = 3;
    public int maxHealthPoints = 3;
    
    [Header("VISUAL")]
    public TMP_Text healthPointText;
    private int _healthPoints;
    private Player _player;
    private VisualManager _visualManager;

    public void Init(Player player, VisualManager visualManager)
    {
        _player = player;
        _healthPoints = initialHealthPoints;
        _visualManager = visualManager;
    }
    
    public void ResetHealthPoint()
    {
        _healthPoints = maxHealthPoints;
        UpdateHealthPointVisual(_healthPoints);
    }

    public void DecreaseHealth(int damage)
    {
        _healthPoints -= damage;
        UpdateHealthPointVisual(_healthPoints);
        
        _visualManager.PlayHitFeedbacks();
        
        if (_healthPoints <= 0)
        {
            GameManager.Instance.ChangeGameState(GameState.Lose);
        }
    }

    public void IncreaseHealth(int heal)
    {
        if (_healthPoints >= maxHealthPoints)
        {
            return;
        }
        _healthPoints += heal;
        UpdateHealthPointVisual(_healthPoints);
    }

    public void IncreaseMaxHealth(int newPoints)
    {
        maxHealthPoints += newPoints;
        _healthPoints += newPoints;
        UpdateHealthPointVisual(_healthPoints);
    }

    private void UpdateHealthPointVisual(int currentHealth)
    {
        healthPointText.text = _healthPoints.ToString();
    }
}

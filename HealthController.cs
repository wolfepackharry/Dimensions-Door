using UnityEngine;

public class HealthController : MonoBehaviour
{
    [SerializeField] private float _maxHealth = 100f;
    private float _currentHealth;
    public float CurrentHealth{get{return _currentHealth;}set{_currentHealth = value;}}

    public void TakeDamage(float amount)
    {
        _currentHealth -= amount;
    }
}
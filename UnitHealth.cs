public class UnitHealth
{
    float _currentHealth;
    float _currentMaxHealth;
    public bool _isImmune = false;

    public float Health
    {
        get { return _currentHealth; }
        set { _currentHealth = value; }
    }

    public float MaxHealth
    {
        get { return _currentMaxHealth; }
        set { _currentMaxHealth = value; }
    }

    public UnitHealth(float health, float maxHealth)
    {
        _currentHealth = health;
        _currentMaxHealth = maxHealth;
    }

    public void DamageUnit(float damageAmount)
    {
        if (_currentHealth > 0 && !_isImmune)
        {
            _currentHealth -= damageAmount;
        }
    }

    public void HealUnit(float healAmount)
    {
        if (_currentHealth < _currentMaxHealth)
        {
            _currentHealth += healAmount;
        }

        if (_currentHealth > _currentMaxHealth)
        {
            _currentHealth = _currentMaxHealth;
        }
    }
}

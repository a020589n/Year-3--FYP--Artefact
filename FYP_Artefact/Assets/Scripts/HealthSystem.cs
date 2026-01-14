using System;
using UnityEngine;

public class HealthSystem
{
    public event EventHandler OnHealthChanged;
    public event EventHandler OnDead;
    
    private int healthMax = 100;
    private int healthCurrent;

    public HealthSystem(int healthMax)
    {
        this.healthMax = healthMax;
        healthCurrent = healthMax;
    }

    public void SetHealthAmount(int health)
    {
        this.healthCurrent = health;

        if (OnHealthChanged != null)
        {
            OnHealthChanged(this, EventArgs.Empty);
        }
    }

    public float GetHealthPercentage()
    {
        return (float)healthCurrent / healthMax;
    }

    public int GetHealthAmount()
    {
        return healthCurrent;
    }

    public void Damage(int amount)
    {
        healthCurrent -= amount;

        if (healthCurrent < 0)
        {
            healthCurrent = 0;
        }
        
        if (OnHealthChanged != null)
        {
            OnHealthChanged(this, EventArgs.Empty);
        }

        if (healthCurrent <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        if (OnDead != null)
        {
            OnDead(this, EventArgs.Empty);
        }
    }

    public bool IsDead()
    {
        return healthCurrent <= 0;
    }
    
    public void Heal(int amount)
    {
        healthCurrent += amount;

        if (healthCurrent > healthMax)
        {
            healthCurrent = healthMax;
        }
        
        if (OnHealthChanged != null)
        {
            OnHealthChanged(this, EventArgs.Empty);
        }
        
    }
}



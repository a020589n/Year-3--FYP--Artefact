using System;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(CharacterBase))]
[RequireComponent(typeof(Animator))]
public class CharacterBattle : MonoBehaviour
{
    private CharacterBase characterBase;
    private Animator animator;
    
    private HealthSystem healthSystem;
    private HealthBar healthBar;

    [Header("Damage")]
    [SerializeField] private int minDamage = 5;
    [SerializeField] private int maxDamage = 10;


    [Header("Animators")]
    [SerializeField] private RuntimeAnimatorController playerAnimator;
    [SerializeField] private RuntimeAnimatorController enemyAnimator;

    public bool IsPlayer { get; private set; }

    private void Awake()
    {
        characterBase = GetComponent<CharacterBase>();
        animator = GetComponent<Animator>();
    }

    public void Setup(bool isPlayerTeam)
    {
        IsPlayer = isPlayerTeam;

        animator.runtimeAnimatorController =
            isPlayerTeam ? playerAnimator : enemyAnimator;

        // Player faces right → enemy faces left (or vice versa)
        characterBase.FaceDirection(isPlayerTeam); 
        // Animator will already default to Idle

        healthSystem = new HealthSystem(100);
        
        healthBar = GetComponentInChildren<HealthBar>();
        if (healthBar != null)
        {
            healthBar.Bind(healthSystem);
        }
    }

    #region ---------- OLD BATTLE COMMANDS ----------
    
        // public void Guard()
        // {
        //     characterBase.Guard();
        // }
        //
        // public void Attack(CharacterBattle target, Action onAttackComplete)
        // { 
        //     int damageAmount = Random.Range(minDamage, maxDamage + 1);
        //     characterBase.AttackEnemy(target.transform, () => { target.Damage(damageAmount); }, onAttackComplete);
        // }
        
    #endregion
    
    #region ---------- BATTLE COMMANDS ----------

    /// <summary>
    /// Execute a full RPS turn against a target, using CombatIntents
    /// </summary>
    public void ExecuteTurn(CharacterBattle target, CombatIntent myIntent, CombatIntent targetIntent, Action onTurnComplete)
    {
        // Determine attack effect
        var attackOutcome = RPSRulesResolver.ResolveOutcome(myIntent.AttackChoice, targetIntent.DefendChoice);
        var attackEffect = RPSOutcomeEffectMap.GetEffect(attackOutcome);

        // Apply attack effect
        ApplyEffect(target, attackEffect);

        // Determine defend effect (other way round)
        var defendOutcome = RPSRulesResolver.ResolveOutcome(targetIntent.AttackChoice, myIntent.DefendChoice);
        var defendEffect = RPSOutcomeEffectMap.GetEffect(defendOutcome);

        // Apply defend effect
        ApplyEffect(this, defendEffect, targetIntent.AttackChoice);

        // Callbacks for completion
        onTurnComplete?.Invoke();
    }

    #endregion
    
    #region ---------- EFFECT APPLICATION ----------

    /// <summary>
    /// Applies a CombatEffect to a character
    /// </summary>
    private void ApplyEffect(CharacterBattle target, CombatEnums.CombatEffect effect, CombatEnums.RPSChoice attackChoice = CombatEnums.RPSChoice.Rock)
    {
        int damageAmount = Random.Range(minDamage, maxDamage + 1);

        switch (effect)
        {
            case CombatEnums.CombatEffect.DealDamage:
                target.TakeDamage(damageAmount);
                break;

            case CombatEnums.CombatEffect.HealDefender:
                // Heal amount = would-be damage
                target.Heal(damageAmount);
                break;

            case CombatEnums.CombatEffect.Nothing:
                // No action
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(effect), effect, null);
        }
    }

    /// <summary>
    /// Deals damage and shows popup
    /// </summary>
    private void TakeDamage(int damageAmount)
    {
        // Spawn damage popup above the character
        Vector3 popupPosition = transform.position + Vector3.up * 2f;
        DamagePopup.Create(popupPosition, damageAmount, damageAmount == maxDamage, false);

        //Deal Damage and Check for Death
        healthSystem.Damage(damageAmount);
        
        if (healthSystem.IsDead())
        {
            //trigger death events. Play victory screen or defeat screen
        }
    }

    /// <summary>
    /// Heals the character by the given amount
    /// </summary>
    private void Heal(int amount)
    {
        // Spawn damage popup above the character
        Vector3 popupPosition = transform.position + Vector3.up * 2f;
        DamagePopup.Create(popupPosition, amount, false, true);
        healthSystem.Heal(amount);
    }

    #endregion


    // private void Damage(int damageAmount)
    // { 
    //     // Spawn damage popup above the character
    //     if (damageAmount == maxDamage)
    //     {
    //         _isCriticalHit = true;
    //     }
    //     else
    //     {
    //         _isCriticalHit = false;
    //     }
    //     
    //     Vector3 popupPosition = transform.position + Vector3.up * 2f;
    //     DamagePopup.Create(popupPosition, damageAmount, _isCriticalHit);
    //     
    //     //Deal Damage and Check for Death
    //     healthSystem.Damage(damageAmount);
    //
    //     if (healthSystem.IsDead())
    //     {
    //         //trigger death events. Play victory screen or defeat screen
    //     }
    // }

    public bool IsDead()
    {
        return healthSystem.IsDead();
    }
}



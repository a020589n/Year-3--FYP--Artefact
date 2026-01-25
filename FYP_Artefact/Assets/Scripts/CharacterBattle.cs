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
    private int damageRoll = 0;


    [Header("Animators")]
    [SerializeField] private RuntimeAnimatorController playerAnimator;
    [SerializeField] private RuntimeAnimatorController enemyAnimator;
    
    [Header("RPS Icon")]
    private RPSIntentDisplay intentIcon;

    public bool IsPlayer { get; private set; }

    private void Awake()
    {
        characterBase = GetComponent<CharacterBase>();
        animator = GetComponent<Animator>();
        
        intentIcon = GetComponent<RPSIntentDisplay>();
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
        
        //Tint enemy characters red (player remains unchanged)
        if (!isPlayerTeam)
        {
            characterBase.GetComponent<SpriteRenderer>().color = Color.red;
        }
    }

    #region ---------- RPS ICON COMMANDS ----------

    public void ShowAttackIcon(CombatEnums.RPSChoice choice)
    {
        intentIcon?.ShowAttack(choice);
    }

    public void ShowDefendIcon(CombatEnums.RPSChoice choice)
    {
        intentIcon?.ShowDefend(choice);
    }

    public void ClearIntentIcon()
    {
        intentIcon?.Clear();
    }

    #endregion
    
    #region ---------- BATTLE COMMANDS ----------

    /// <summary>
    /// Execute a full RPS turn against a target, using CombatIntents
    /// </summary>
    public void ExecuteTurn(CharacterBattle target, CombatIntent myIntent, CombatIntent targetIntent, Action onTurnComplete)
    {
        // SHOW INTENT ICONS
        intentIcon.ShowAttack(myIntent.AttackChoice);
        target.intentIcon.ShowDefend(targetIntent.DefendChoice);
        
        // Determine attack effect
        var attackOutcome = RPSRulesResolver.ResolveOutcome(myIntent.AttackChoice, targetIntent.DefendChoice);
        var attackEffect = RPSOutcomeEffectMap.GetEffect(attackOutcome);

        // Determine defend effect (other way round)
        var defendOutcome = RPSRulesResolver.ResolveOutcome(targetIntent.AttackChoice, myIntent.DefendChoice);
        var defendEffect = RPSOutcomeEffectMap.GetEffect(defendOutcome);
        
        damageRoll = Random.Range(minDamage, maxDamage + 1);
        
        // Perform animated attack
        characterBase.AttackEnemy(
            target.transform,

            // ON HIT (mid-animation)
            () =>
            {
                // Apply attack effect
                ApplyEffect(target, attackEffect, damageRoll);
                
                // Only heal if defending character
                if (defendEffect == CombatEnums.CombatEffect.HealDefender)
                {
                    characterBase.Guard();
                    
                    //ApplyEffect(this, defendEffect, damageRoll);
                }
            },

            // ON ATTACK COMPLETE (after slide back)
            //This is passing a function, not calling a function. Ask Luke because it seems to work???
            onTurnComplete
        );
    }

    #endregion
    
    #region ---------- EFFECT APPLICATION ----------

    /// <summary>
    /// Applies a CombatEffect to a character
    /// </summary>
    private void ApplyEffect(CharacterBattle target, CombatEnums.CombatEffect effect, int damageAmount)
    {

        switch (effect)
        {
            case CombatEnums.CombatEffect.DealDamage:
                target.TakeDamage(damageAmount);
                break;

            case CombatEnums.CombatEffect.HealDefender:
                //Heal amount = 1/3 of would-be damage, rounded down
                target.Heal(Mathf.FloorToInt(damageAmount / 3f));
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


    public bool IsDead()
    {
        return healthSystem.IsDead();
    }
}



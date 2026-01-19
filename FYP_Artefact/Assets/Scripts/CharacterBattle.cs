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
    private bool _isCriticalHit = false;


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

    #region ---------- BATTLE COMMANDS ----------
    
        public void Guard()
        {
            characterBase.Guard();
        }

        public void Attack(CharacterBattle target, Action onAttackComplete)
        { 
            int damageAmount = Random.Range(minDamage, maxDamage + 1);
            characterBase.AttackEnemy(target.transform, () => { target.Damage(damageAmount); }, onAttackComplete);
        }
        
    #endregion


    private void Damage(int damageAmount)
    { 
        // Spawn damage popup above the character
        if (damageAmount == maxDamage)
        {
            _isCriticalHit = true;
        }
        else
        {
            _isCriticalHit = false;
        }
        
        Vector3 popupPosition = transform.position + Vector3.up * 2f;
        DamagePopup.Create(popupPosition, damageAmount, _isCriticalHit);
        
        //Deal Damage and Check for Death
        healthSystem.Damage(damageAmount);

        if (healthSystem.IsDead())
        {
            //trigger death events. Play victory screen or defeat screen
        }
    }

    public bool IsDead()
    {
        return healthSystem.IsDead();
    }
}



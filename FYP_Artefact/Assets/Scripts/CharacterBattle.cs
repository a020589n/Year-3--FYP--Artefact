using System;
using UnityEngine;

[RequireComponent(typeof(CharacterBase))]
[RequireComponent(typeof(Animator))]
public class CharacterBattle : MonoBehaviour
{
    private CharacterBase characterBase;
    private Animator animator;
    
    private HealthSystem healthSystem;
    private HealthBar healthBar;


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
            characterBase.AttackEnemy(target.transform, () => { target.Damage(25); }, onAttackComplete);
        }
        
    #endregion

    
    public void Damage(int damageAmount)
    {
        healthSystem.Damage(damageAmount);
        
        // Spawn damage popup above the character
        Vector3 popupPosition = transform.position + Vector3.up * 2f;
        DamagePopup.Create(popupPosition, damageAmount, false);
    }

    public bool IsDead()
    {
        return healthSystem.IsDead();
    }
}



using UnityEngine;

[RequireComponent(typeof(CharacterBase))]
[RequireComponent(typeof(Animator))]
public class CharacterBattle : MonoBehaviour
{
    private CharacterBase characterBase;
    private Animator animator;

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
    }

    // ---------- BATTLE COMMANDS ----------

    public void Guard()
    {
        characterBase.Guard();
    }

    public void Attack(CharacterBattle target)
    {
        characterBase.AttackEnemy(target.transform);
    }
}



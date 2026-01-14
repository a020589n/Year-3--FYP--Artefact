using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
public class CharacterBase : MonoBehaviour
{
    // Animator hashes
    private static readonly int Run = Animator.StringToHash("Run");
    private static readonly int Attack = Animator.StringToHash("Attack");
    private static readonly int GuardTrigger = Animator.StringToHash("Guard");

    [Header("Components")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator animator;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float attackDuration = 0.6f;

    private Vector3 startPos;

    protected virtual void Awake()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        if (animator == null)
            animator = GetComponent<Animator>();
        
        Debug.Log($"SpriteRenderer found: {spriteRenderer != null}");
        Debug.Log($"Animator found: {animator != null}");


        startPos = transform.position;
    }

    // ---------- ANIMATION ----------
    public void SetRun(bool value)
    {
        animator.SetBool(Run, value);
    }

    public void Guard()
    {
        animator.SetTrigger(GuardTrigger);
    }

    // ---------- ATTACK ----------
    public void AttackEnemy(Transform enemy)
    {
        StopAllCoroutines();
        StartCoroutine(AttackSequence(enemy));
    }

    private IEnumerator AttackSequence(Transform enemy)
    {
        // Run toward enemy
        SetRun(true);

        while (Vector3.Distance(transform.position, enemy.position) > 0.5f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                enemy.position,
                moveSpeed * Time.deltaTime
            );
            yield return null;
        }

        // Stop & attack
        SetRun(false);
        animator.SetTrigger(Attack);

        // Wait for attack animation
        yield return new WaitForSeconds(attackDuration);

        // Run back
        SetRun(true);

        while (Vector3.Distance(transform.position, startPos) > 0.05f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                startPos,
                moveSpeed * Time.deltaTime
            );
            yield return null;
        }

        SetRun(false);
    }

    // ---------- FACING ----------
    public void FaceDirection(bool faceRight)
    {
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * (faceRight ? 1 : -1);
        transform.localScale = scale;
    }
}




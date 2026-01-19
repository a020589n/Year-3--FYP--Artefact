using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
public class CharacterBase : MonoBehaviour
{
    #region Animator hashes

        private static readonly int Run = Animator.StringToHash("Run");
        private static readonly int Attack = Animator.StringToHash("Attack");
        private static readonly int GuardTrigger = Animator.StringToHash("Guard");
    
    #endregion

    [Header("Components")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator animator;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float attackDuration = 0.6f;

    private Vector3 startPos;

    protected virtual void Awake()
    {
        if (spriteRenderer == null)
        {spriteRenderer = GetComponent<SpriteRenderer>();}

        if (animator == null)
        {animator = GetComponent<Animator>();}

        Debug.Log($"SpriteRenderer found: {spriteRenderer != null}");
        Debug.Log($"Animator found: {animator != null}");


        startPos = transform.position;
    }

    #region ---------- ANIMATION ----------
    
        public void SetRun(bool value)
        {
            animator.SetBool(Run, value);
        }

        public void Guard()
        {
            animator.SetTrigger(GuardTrigger);
        }
    
    #endregion

    #region---------- ATTACK ----------
    
    public void AttackEnemy(Transform target, Action onHit = null, Action onAttackComplete = null)
    {
        StopAllCoroutines();
        StartCoroutine(AttackSequence(target,onHit, onAttackComplete));
    }

    private IEnumerator AttackSequence(Transform target, Action onHit, Action onAttackComplete)
    {
        // Run toward enemy
        SetRun(true);

        while (Vector3.Distance(transform.position, target.position) > 2.0f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                target.position,
                moveSpeed * Time.deltaTime
            );
            yield return null;
        }

        // Stop & attack
        SetRun(false);
        animator.SetTrigger(Attack);
        onHit?.Invoke();

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
        
        onAttackComplete?.Invoke();
    }

    #endregion


    #region ---------- FACING ----------
        
        public void FaceDirection(bool faceRight)
        {
            HealthBar healthBar = GetComponentInChildren<HealthBar>();

            Transform hbTransform = null;
            if (healthBar != null)
            {
                hbTransform = healthBar.transform;
                hbTransform.SetParent(null, true); // detach, keep world transform
            }

            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x) * (faceRight ? -1 : 1);
            transform.localScale = scale;

            if (hbTransform != null)
            {
                hbTransform.SetParent(transform, true); // reattach, keep world transform
            }
        }
        
    #endregion
}




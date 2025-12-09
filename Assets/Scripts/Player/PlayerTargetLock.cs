using UnityEngine;

public class PlayerTargetLock : MonoBehaviour
{
    [Header("Target Lock Settings")]
    [SerializeField] private float searchRadius = 12f;
    [SerializeField] private LayerMask targetLayerMask;

    private bool isTargeting;
    private Transform currentTarget;

    public Transform CurrentTarget => currentTarget;
    public bool IsTargeting => isTargeting;

    private void Update()
    {
        if (isTargeting && currentTarget == null)
        {
            AcquireTarget();
        }
    }

    public void SetTargeting(bool active)
    {
        isTargeting = active;
        
        if (active)
        {
            AcquireTarget();
        }
        else
        {
            currentTarget = null;
        }
    }

    private void AcquireTarget()
    {
        float bestDistSq = Mathf.Infinity;
        Transform best = null;

        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position,
            searchRadius,
            targetLayerMask
        );

        foreach (var hit in hits)
        {
            if (hit == null) continue;
            if (hit.transform == transform) continue;

            float distSq = (hit.transform.position - transform.position).sqrMagnitude;
            if (distSq < bestDistSq)
            {
                bestDistSq = distSq;
                best = hit.transform;
            }
        }

        currentTarget = best;

        if (currentTarget == null)
            isTargeting = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, searchRadius);
    }
}
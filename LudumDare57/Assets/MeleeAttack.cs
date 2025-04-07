using UnityEngine;
using Unity.FPS.Game; // For Damageable

public class MeleeAttack : MonoBehaviour
{
    [Header("Melee Settings")]
    [Tooltip("Distance in front of the player where the melee hit is centered")]
    public float meleeRange = 1.5f;

    [Tooltip("Radius of the melee hit detection")]
    public float meleeRadius = 0.5f;

    [Tooltip("Damage dealt by the melee attack")]
    public float meleeDamage = 40f;

    [Tooltip("Number of attacks allowed per second")]
    public float attackRate = 1f;

    [Tooltip("Layers to consider as hittable (e.g. enemy layers)")]
    public LayerMask hittableLayers;

    [Header("Knockback Settings")]
    [Tooltip("Force to apply to enemies for knockback effect")]
    public float knockbackForce = 10f;

    [Header("Impact Effects")]
    public GameObject impactVfx;
    public float impactVfxLifetime = 2f;
    public AudioClip impactSfxClip;
    public Animator punchAnimator;

    private float nextAttackTime = 0f;
    private bool isAttacking = false;

    private GameObject owner;

    void Start()
    {
        owner = gameObject;
    }

    void Update()
    {
        // Use left mouse button as an example attack trigger.
        if (Input.GetMouseButtonDown(0) && !isAttacking && Time.time >= nextAttackTime)
        {
            Debug.Log("Punch Triggered");
            punchAnimator.SetTrigger("Punch");
            isAttacking = true;
            nextAttackTime = Time.time + 1f / attackRate;
        }
    }

    // Called by animation event when the punch hits
    public void Punched()
    {
        Debug.Log("Punched() event fired");

        PerformMeleeAttack();
        isAttacking = false;
    }

    void PerformMeleeAttack()
    {
        Vector3 attackPoint = transform.position + transform.forward * meleeRange;
        Debug.Log($"Performing melee at {attackPoint}");

        Collider[] hitColliders = Physics.OverlapSphere(attackPoint, meleeRadius);
        Debug.Log($"Hit {hitColliders.Length} objects");

        foreach (Collider col in hitColliders)
        {
            // If the target has a Damageable component, apply damage.
            Damageable damageable = col.GetComponent<Damageable>();
            if (damageable != null)
            {
                damageable.InflictDamage(meleeDamage, false, owner);
            }

            // Apply knockback if the target has a Rigidbody.
            Rigidbody rb = col.attachedRigidbody;
            if (rb != null)
            {
                Vector3 knockbackDirection = (col.transform.position - transform.position).normalized;
                rb.AddForce(knockbackDirection * knockbackForce, ForceMode.Impulse);
            }
        }

        // Optionally spawn a visual effect at the attack point.
        if (impactVfx != null)
        {
            GameObject vfxInstance = Instantiate(impactVfx, attackPoint, Quaternion.identity);
            if (impactVfxLifetime > 0)
            {
                Destroy(vfxInstance, impactVfxLifetime);
            }
        }

        // Optionally play a sound effect at the attack point.
        if (impactSfxClip != null)
        {
            AudioSource.PlayClipAtPoint(impactSfxClip, attackPoint);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 attackPoint = transform.position + transform.forward * meleeRange;
        Gizmos.DrawWireSphere(attackPoint, meleeRadius);
    }
}

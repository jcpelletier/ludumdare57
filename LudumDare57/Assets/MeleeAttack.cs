using UnityEngine;
using Unity.FPS.Game; // For Damageable
using System.Collections.Generic;

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
        if (Input.GetMouseButtonDown(0) && !isAttacking && Time.time >= nextAttackTime)
        {
            Debug.Log("Punch Triggered");
            punchAnimator.SetTrigger("Punch");
            isAttacking = true;
            nextAttackTime = Time.time + 1f / attackRate;
        }
    }

    // Called by animation event
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

        Collider[] hitColliders = Physics.OverlapSphere(attackPoint, meleeRadius, hittableLayers);
        Debug.Log($"Hit {hitColliders.Length} objects");

        HashSet<GameObject> damagedObjects = new HashSet<GameObject>();

        foreach (Collider col in hitColliders)
        {
            GameObject target = col.gameObject;

            if (damagedObjects.Contains(target))
                continue;

            if (target == owner)
                continue;

            Damageable damageable = target.GetComponent<Damageable>();
            if (damageable != null)
            {
                damageable.InflictDamage(meleeDamage, false, owner);
                damagedObjects.Add(target);
            }

            Rigidbody rb = col.attachedRigidbody;
            if (rb != null)
            {
                Vector3 knockbackDirection = (target.transform.position - transform.position).normalized;
                rb.AddForce(knockbackDirection * knockbackForce, ForceMode.Impulse);
            }
        }

        if (impactVfx != null)
        {
            GameObject vfxInstance = Instantiate(impactVfx, attackPoint, Quaternion.identity);
            if (impactVfxLifetime > 0)
            {
                Destroy(vfxInstance, impactVfxLifetime);
            }
        }

        if (impactSfxClip != null)
        {
            AudioSource.PlayClipAtPoint(impactSfxClip, attackPoint);
        }

        // Spawn a visible red debug sphere at the attack point
        /*
        GameObject debugSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        debugSphere.transform.position = attackPoint;
        debugSphere.transform.localScale = Vector3.one * meleeRadius * 2f;
        debugSphere.GetComponent<Collider>().enabled = false;
        debugSphere.GetComponent<Renderer>().material.color = Color.red;
        Destroy(debugSphere, 1.0f);
        */
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 attackPoint = transform.position + transform.forward * meleeRange;
        Gizmos.DrawWireSphere(attackPoint, meleeRadius);
    }
}

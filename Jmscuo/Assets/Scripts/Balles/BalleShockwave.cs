using UnityEngine;

public class ShockwaveOnCollision : MonoBehaviour
{
    [Header("Paramètres de l'impulsion")]
    public float radius = 1f;
    public float force = 3f;
    public ForceMode forceMode = ForceMode.Impulse;

    [Header("Filtrage")]
    public string groundTag = "Ground";
    public string targetTag = "Balle";
    public bool affectAll = false;

    [Header("VFX")]
    [Tooltip("Prefab de VFX instancié à l'impact")]
    public GameObject shockwaveVfxPrefab;
    [Tooltip("Placer le VFX au point de contact (sinon au centre de la boule)")]
    public bool spawnAtContactPoint = true;
    [Tooltip("Aligner le VFX sur la normale de l'impact")]
    public bool alignToNormal = true;
    [Tooltip("Mise à l’échelle du VFX = radius * 2")]
    public bool scaleVfxWithRadius = true;
    [Tooltip("Auto-destruction du VFX (0 = laisser le prefab gérer)")]
    public float vfxLifetime = 0f;

    [Header("XR (URP Single Pass)")]
    [Tooltip("Matériau URP/Particles/Unlit compatible XR, utilisé par VFXOnBothOptics")]
    public Material xrSafeParticleMaterial;
    [Tooltip("Forcer le GPU instancing sur les ParticleSystemRenderer")]
    public bool forceGpuInstancing = true;

    [Header("Haptique")]
    public bool hapticsOnImpact = true;
    [Range(0f, 1f)] public float hapticsAmplitude = 0.7f;
    [Range(0f, 1f)] public float hapticsDuration = 0.12f;
    public bool hapticsBothHands = true; // sinon, vibre uniquement la main droite

    [Header("Destruction")]
    public float destroyDelay = 0.1f;

    [Header("Debug")]
    public bool showGizmos = true;

    private bool hasExploded = false;
    private Vector3 lastExplosionPos;

    private void OnCollisionEnter(Collision collision)
    {
        if (hasExploded) return;
        if (!collision.gameObject.CompareTag(groundTag)) return;

        hasExploded = true;

        // --- Haptique ---
        if (hapticsOnImpact)
        {
            if (hapticsBothHands)
                HapticPulseVR.PulseBoth(hapticsAmplitude, hapticsDuration);
            else
                HapticPulseVR.Pulse(false, hapticsAmplitude, hapticsDuration); // false = main droite
        }

        // --- Position / normale d'impact ---
        Vector3 explosionPosition = (spawnAtContactPoint && collision.contactCount > 0)
            ? collision.GetContact(0).point
            : transform.position;

        Vector3 normal = (collision.contactCount > 0)
            ? collision.GetContact(0).normal
            : Vector3.up;

        lastExplosionPos = explosionPosition;

        // --- VFX ---
        SpawnShockwaveVfx(explosionPosition, normal);

        // --- Onde de choc physique ---
        Collider[] colliders = Physics.OverlapSphere(explosionPosition, radius);
        foreach (Collider nearby in colliders)
        {
            Rigidbody rb = nearby.attachedRigidbody;
            if (!rb) continue;
            if (rb.gameObject == gameObject) continue;
            if (!affectAll && !nearby.CompareTag(targetTag)) continue;

            Vector3 direction = (nearby.transform.position - explosionPosition).normalized;
            rb.AddForce(direction * force, forceMode);
        }

        // --- Fin de vie de la boule ---
        if (destroyDelay <= 0f)
            Destroy(gameObject);
        else
            Destroy(gameObject, destroyDelay);
    }

    private void SpawnShockwaveVfx(Vector3 pos, Vector3 normal)
    {
        if (!shockwaveVfxPrefab) return;

        Quaternion rot = alignToNormal ? Quaternion.LookRotation(normal) : Quaternion.identity;
        var vfx = Instantiate(shockwaveVfxPrefab, pos, rot);

        if (scaleVfxWithRadius)
            vfx.transform.localScale = Vector3.one * (radius * 2f);

        // --- Correction XR (affichage deux yeux) ---
        var fixer = vfx.GetComponent<VFXOnBothOptics>();
        if (!fixer) fixer = vfx.AddComponent<VFXOnBothOptics>();

        fixer.xrSafeParticleMaterial = xrSafeParticleMaterial;
        fixer.forceGpuInstancing = forceGpuInstancing;
        fixer.applyOnStart = false;
        fixer.debugLog = false;
        fixer.ApplyXRFix();

        if (vfxLifetime > 0f)
            Destroy(vfx, vfxLifetime);
    }

    private void OnDrawGizmosSelected()
    {
        if (!showGizmos) return;
        Gizmos.color = new Color(1f, 0.5f, 0f, 0.3f);
        Vector3 c = Application.isPlaying ? lastExplosionPos : transform.position;
        Gizmos.DrawSphere(c, radius);
    }
}

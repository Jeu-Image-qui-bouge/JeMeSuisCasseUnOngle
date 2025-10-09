using UnityEngine;
using System.Collections;

public class ShockwaveOnCollision : MonoBehaviour
{
    [Header("Impulsion")]
    public float radius = 1f;
    public float force = 3f;
    public ForceMode forceMode = ForceMode.Impulse;

    [Header("Filtrage")]
    public string groundTag = "Ground";
    public string targetTag = "Balle";
    public bool affectAll = false;

    [Header("FX visuels / audio")]
    [Tooltip("Prefab de particules / VFX pour l'onde de choc")]
    public GameObject shockwaveVfxPrefab;
    [Tooltip("Jouer un son à l'impact")]
    public AudioClip shockwaveSfx;
    [Range(0f, 1f)] public float sfxVolume = 0.9f;
    [Tooltip("Placer le VFX au point de contact (sinon au centre de l'objet)")]
    public bool spawnAtContactPoint = true;
    [Tooltip("Aligne le VFX sur la normale de l'impact (utile pour des anneaux)")]
    public bool alignToNormal = true;
    [Tooltip("Durée avant auto-destruction du VFX si le prefab n'a pas sa propre gestion")]
    public float vfxLifetime = 4f;

    [Header("Flash de lumière (optionnel)")]
    public bool lightFlash = true;
    public float lightRange = 6f;
    public float lightIntensity = 8f;    // en Built-in ; en URP/HDRP, adapte selon ton pipeline
    public float lightFade = 0.08f;

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

        Debug.Log("Coucou");

        hasExploded = true;

        // Position / normale pour FX
        Vector3 explosionPosition = spawnAtContactPoint && collision.contactCount > 0
            ? collision.GetContact(0).point
            : transform.position;

        Vector3 normal = collision.contactCount > 0
            ? collision.GetContact(0).normal
            : Vector3.up;

        lastExplosionPos = explosionPosition;

        // --- FX ---
        SpawnFX(explosionPosition, normal);

        // --- Impulsion physique ---
        Collider[] colliders = Physics.OverlapSphere(explosionPosition, radius);
        foreach (Collider nearby in colliders)
        {
            Rigidbody rb = nearby.attachedRigidbody;
            if (rb == null) continue;
            if (rb.gameObject == gameObject) continue;
            if (!affectAll && !nearby.CompareTag(targetTag)) continue;

            Vector3 direction = (nearby.transform.position - explosionPosition).normalized;
            rb.AddForce(direction * force, forceMode);
        }

        Destroy(gameObject, destroyDelay);
    }

    private void SpawnFX(Vector3 pos, Vector3 normal)
    {
        // VFX
        if (shockwaveVfxPrefab)
        {
            Quaternion rot = alignToNormal ? Quaternion.LookRotation(normal) : Quaternion.identity;
            var vfx = Instantiate(shockwaveVfxPrefab, pos, rot);

            // Si le prefab n'a pas d'auto-kill, on le détruit au bout de vfxLifetime
            if (vfxLifetime > 0f) Destroy(vfx, vfxLifetime);

            // Si ton VFX doit refléter le rayon, on met à l'échelle (si le prefab est unité = diamètre 1)
            // Ajuste si besoin : beaucoup de VFX Graph utilisent l'échelle locale comme multiplicateur.
            vfx.transform.localScale = Vector3.one * (radius * 2f);
        }

        // SFX
        if (shockwaveSfx)
            AudioSource.PlayClipAtPoint(shockwaveSfx, pos, sfxVolume);

        // Flash de lumière
        if (lightFlash)
            StartCoroutine(FlashLight(pos));
    }

    private IEnumerator FlashLight(Vector3 pos)
    {
        var go = new GameObject("ShockwaveFlashLight");
        var l = go.AddComponent<Light>();
        l.range = lightRange;
        l.intensity = lightIntensity;
        l.color = Color.white;
        go.transform.position = pos;

        float t = 0f;
        float start = l.intensity;
        while (t < lightFade)
        {
            t += Time.deltaTime;
            l.intensity = Mathf.Lerp(start, 0f, t / lightFade);
            yield return null;
        }
        Destroy(go);
    }

    private void OnDrawGizmosSelected()
    {
        if (!showGizmos) return;
        Gizmos.color = new Color(1f, 0.5f, 0f, 0.25f);
        Vector3 c = Application.isPlaying ? lastExplosionPos : transform.position;
        Gizmos.DrawSphere(c, radius);
    }
}

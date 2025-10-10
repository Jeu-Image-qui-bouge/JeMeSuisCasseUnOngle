using UnityEngine;

public class SpawnVfxOnCollision : MonoBehaviour
{
    [Header("VFX")]
    [Tooltip("Prefab VFX avec VFXOnBothOptics (URP Particles Unlit + Instancing)")]
    public GameObject vfxPrefab;

    [Tooltip("Placer le VFX au point de contact (sinon au centre de cet objet)")]
    public bool spawnAtContactPoint = true;

    [Tooltip("Aligner le VFX sur la normale de l'impact")]
    public bool alignToNormal = true;

    [Tooltip("Mise à l’échelle du VFX en fonction d’un rayon externe (0 = pas de scale automatique)")]
    public float scaleFromRadius = 0f; // ex: mets le même radius que ta shockwave pour matcher visuellement

    [Tooltip("Auto-destruction du VFX (0 = laisser le prefab gérer)")]
    public float vfxLifetime = 0f;

    [Header("Filtrage (optionnel)")]
    [Tooltip("Ne spawner que si on touche un objet avec ce tag (laisser vide pour ignorer)")]
    public string requiredOtherTag = "Ground";

    [Tooltip("Une seule fois")]
    public bool oneShot = true;

    bool triggered;

    void OnCollisionEnter(Collision c)
    {
        if (!enabled) return;
        if (oneShot && triggered) return;

        if (!string.IsNullOrEmpty(requiredOtherTag) && !c.gameObject.CompareTag(requiredOtherTag))
            return;

        TriggerVfx(c);
        if (oneShot) triggered = true;
    }

    public void TriggerVfx(Collision c)
    {
        if (!vfxPrefab) return;

        Vector3 pos = spawnAtContactPoint && c.contactCount > 0 ? c.GetContact(0).point : transform.position;
        Vector3 nrm = c.contactCount > 0 ? c.GetContact(0).normal : Vector3.up;

        Quaternion rot = alignToNormal ? Quaternion.LookRotation(nrm) : Quaternion.identity;
        var vfx = Instantiate(vfxPrefab, pos, rot);

        if (scaleFromRadius > 0f)
            vfx.transform.localScale = Vector3.one * (scaleFromRadius * 2f);

        // Le fix XR est géré PAR le prefab via VFXOnBothOptics (ApplyOnStart = true)
        // → rien à faire ici.

        if (vfxLifetime > 0f)
            Destroy(vfx, vfxLifetime);
    }
}

using UnityEngine;

[ExecuteAlways]
public class VFXOnBothOptics : MonoBehaviour
{
    [Header("Matériau XR Safe")]
    [Tooltip("Matériau compatible XR (ex : shader URP/Particles/Unlit avec instancing activé)")]
    public Material xrSafeParticleMaterial;

    [Header("Options")]
    [Tooltip("Forcer le GPU Instancing sur les ParticleSystemRenderer")]
    public bool forceGpuInstancing = true;

    [Tooltip("Appliquer le correctif automatiquement au Start (sinon, appeler ApplyXRFix() manuellement)")]
    public bool applyOnStart = true;

    [Tooltip("Afficher un log dans la console lors de l’application du correctif")]
    public bool debugLog = true;

    private bool _applied;

    private void Start()
    {
        if (applyOnStart) ApplyXRFix();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (Application.isPlaying) return;
        if (applyOnStart && !_applied)
        {
            ApplyXRFix();
        }
    }
#endif

    [ContextMenu("Apply XR Fix Now")]
    public void ApplyXRFix()
    {
        if (!xrSafeParticleMaterial && !forceGpuInstancing)
        {
            if (debugLog)
                Debug.LogWarning($"[VFXOnBothOptics] Aucun matériau XR assigné sur {name}. Rien à appliquer.");
            return;
        }

        int fixedCount = 0;
        var psrs = GetComponentsInChildren<ParticleSystemRenderer>(true);
        foreach (var psr in psrs)
        {
#if UNITY_2020_3_OR_NEWER
            if (forceGpuInstancing)
                psr.enableGPUInstancing = true;
#endif

            if (xrSafeParticleMaterial)
            {
                var mats = psr.sharedMaterials;
                for (int i = 0; i < mats.Length; i++)
                    mats[i] = xrSafeParticleMaterial;
                psr.sharedMaterials = mats;
            }

            fixedCount++;
        }

        _applied = true;

        if (debugLog)
            Debug.Log($"[VFXOnBothOptics] Correctif XR appliqué à {fixedCount} systèmes de particules dans {name}.");
    }
}

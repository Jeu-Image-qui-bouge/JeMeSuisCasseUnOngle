using UnityEngine;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine.SceneManagement;

public class ResetGame : MonoBehaviour
{
    [Header("Cibles à détruire")]
    [Tooltip("Détruire par Tag (ex: \"Balle\"). Vide = ignoré.")]
    public string ballTag = "Balle";

    [Tooltip("Détruire par Layer (ex: layer 'Boule'). -1 = ignoré.")]
    public int ballLayer = -1;

    [Tooltip("Détruire les objets qui possèdent ce composant (ex: Boule, BouleGlouton). Laisser vide pour ignorer.")]
    public string componentTypeName = ""; // ex: "Boule" ou "BouleGlouton"

    [Header("Options supplémentaires")]
    [Tooltip("Détruire aussi le cochonnet (si présent)")]
    public bool destroyCochonnet = false;
    public string cochonnetTag = "Cochonnet";

    [Tooltip("Si tu as un parent 'container' qui reçoit toutes les instances (ex: 'BoulesRoot')")]
    public Transform containerParent;

    [Tooltip("Recharger la scène au reset (plus radical)")]
    public bool reloadSceneInstead = false;

    [Header("Sécurité")]
    [Tooltip("Utiliser DestroyImmediate en Éditeur si pas en Play Mode (utile pour tester dans l'éditeur)")]
    public bool useImmediateInEditor = true;

    // Appelle ça depuis un bouton UI ou un événement
    public void DoReset()
    {
        if (reloadSceneInstead)
        {
            var idx = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(idx);
            return;
        }

        // 1) Si un conteneur est défini, on peut tout purger dedans.
        if (containerParent != null)
        {
            // On ne supprime pas le parent, seulement ses enfants
            for (int i = containerParent.childCount - 1; i >= 0; i--)
                SafeDestroy(containerParent.GetChild(i).gameObject);
        }

        // 2) Par Tag
        if (!string.IsNullOrWhiteSpace(ballTag))
            foreach (var go in GameObject.FindGameObjectsWithTag(ballTag))
                SafeDestroy(go);

        // 3) Par Layer
        if (ballLayer >= 0 && ballLayer < 32)
        {
            // Trouve tous les root objects, puis descend
            foreach (var root in SceneManager.GetActiveScene().GetRootGameObjects())
            {
                foreach (var t in root.GetComponentsInChildren<Transform>(true))
                {
                    if (t.gameObject.layer == ballLayer)
                        SafeDestroy(t.gameObject);
                }
            }
        }

        // 4) Par Type de composant (nom)
        if (!string.IsNullOrWhiteSpace(componentTypeName))
        {
            var type = System.Type.GetType(componentTypeName);
            if (type == null)
            {
                // Essaie de le trouver parmi tous les types chargés
                type = System.AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(a => a.GetTypes())
                    .FirstOrDefault(t => t.Name == componentTypeName);
            }

            if (type != null && typeof(Component).IsAssignableFrom(type))
            {
                var comps = FindObjectsOfType(type, includeInactive: true);
                foreach (var c in comps)
                    SafeDestroy(((Component)c).gameObject);
            }
            else
            {
                Debug.LogWarning($"[ResetGame] Type introuvable ou invalide: '{componentTypeName}'.");
            }
        }

        // 5) Cochonnet
        if (destroyCochonnet && !string.IsNullOrWhiteSpace(cochonnetTag))
        {
            foreach (var go in GameObject.FindGameObjectsWithTag(cochonnetTag))
                SafeDestroy(go);
        }

        Debug.Log("[ResetGame] Reset effectué : boules (et objets sélectionnés) supprimés.");
    }

    // Helper pour lier à un bouton UI
    public void DoResetFromUIButton() => DoReset();

    // Destruction sûre en Play/Editor
    private void SafeDestroy(GameObject go)
    {
        if (go == null) return;

#if UNITY_EDITOR
        if (!Application.isPlaying && useImmediateInEditor)
        {
            // En mode Éditeur (pas Play), DestroyImmediate évite d'accumuler des objets
            Undo.RegisterFullObjectHierarchyUndo(go, "ResetGame Destroy");
            Object.DestroyImmediate(go);
            return;
        }
#endif
        Object.Destroy(go);
    }
}

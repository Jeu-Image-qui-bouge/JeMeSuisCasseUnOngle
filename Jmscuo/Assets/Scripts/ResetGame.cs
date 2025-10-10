using UnityEngine;

public class ClearChildrenOnClick : MonoBehaviour
{
    [Header("Parent contenant les balles")]
    public GameObject parentObject;

    [Header("Debug")]
    public bool showLogs = true;

    public void ClearChildren()
    {
        if (parentObject == null)
        {
            Debug.LogWarning("[ClearChildrenOnClick] Aucun parent assigné !");
            return;
        }

        int childCount = parentObject.transform.childCount;

        if (childCount == 0)
        {
            if (showLogs)
                Debug.Log($"{parentObject.name} ne contient aucun enfant à supprimer.");
            return;
        }

        // Boucle de suppression des enfants
        for (int i = parentObject.transform.childCount - 1; i >= 0; i--)
        {
            Transform child = parentObject.transform.GetChild(i);
            Destroy(child.gameObject);
        }

        if (showLogs)
            Debug.Log($"Tous les {childCount} enfants de {parentObject.name} ont été supprimés !");
    }
}

using UnityEngine;

public class ReplaceChildOnNewSpawn : MonoBehaviour
{
    [Header("Debug")]
    public bool showLogs = false;

    [Header("Tag à filtrer pour remplacement")]
    public string targetTag = "NewBalle";

    private Transform lastChild;

    private void Update()
    {
        if (transform.childCount > 0)
        {
            Transform currentChild = transform.GetChild(transform.childCount - 1);

            if (!currentChild.CompareTag(targetTag))
                return;

            if (lastChild != null && lastChild.CompareTag(targetTag))
            {
                if (currentChild != lastChild)
                {
                    if (showLogs)
                        Debug.Log($"Ancien enfant détruit : {lastChild.name}");

                    Destroy(lastChild.gameObject);
                }
            }

            lastChild = currentChild;

            if (showLogs)
                Debug.Log($"Nouveau enfant détecté : {lastChild.name}");
        }
    }
}

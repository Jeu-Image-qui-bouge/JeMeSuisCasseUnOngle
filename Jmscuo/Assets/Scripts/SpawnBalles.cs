using UnityEngine;

public class PrefabSpawner : MonoBehaviour
{
    [Header("Liste des prefabs à instancier")]
    public GameObject[] prefabs;

    [Header("Position de spawn")]
    public Transform spawnPoint;

    [Header("Parent GameObject pour stocker les objets spawné (facultatif)")]
    public GameObject parentObject;

    public void SpawnPrefab(int index)
    {
        if (index < 0 || index >= prefabs.Length)
        {
            Debug.LogWarning($"Index {index} invalide pour le spawn de prefab !");
            return;
        }

        if (prefabs[index] == null)
        {
            Debug.LogWarning($"Le prefab à l'index {index} est vide !");
            return;
        }

        if (spawnPoint == null)
        {
            Debug.LogWarning("Aucun spawnPoint défini !");
            return;
        }

        Transform parentTransform = parentObject != null ? parentObject.transform : null;

        GameObject instance = Instantiate(prefabs[index], spawnPoint.position, spawnPoint.rotation, parentTransform);
    }

    public void SpawnRandomPrefab()
    {
        if (prefabs == null || prefabs.Length == 0)
        {
            Debug.LogWarning("Aucun prefab disponible pour le spawn aléatoire !");
            return;
        }

        if (spawnPoint == null)
        {
            Debug.LogWarning("Aucun spawnPoint défini !");
            return;
        }

        int randomIndex = Random.Range(0, prefabs.Length);
        GameObject prefabToSpawn = prefabs[randomIndex];

        if (prefabToSpawn == null)
        {
            Debug.LogWarning($"Le prefab aléatoire à l'index {randomIndex} est vide !");
            return;
        }

        Transform parentTransform = parentObject != null ? parentObject.transform : null;

        GameObject instance = Instantiate(prefabToSpawn, spawnPoint.position, spawnPoint.rotation, parentTransform);
    }
}

using UnityEngine;

public class PrefabSpawner : MonoBehaviour
{
    [Header("Liste des prefabs � instancier")]
    public GameObject[] prefabs;

    [Header("Position de spawn")]
    public Transform spawnPoint;

    public void SpawnPrefab(int index)
    {
        if (index < 0 || index >= prefabs.Length)
        {
            Debug.LogWarning($"Index {index} invalide pour le spawn de prefab !");
            return;
        }

        if (prefabs[index] == null)
        {
            Debug.LogWarning($"Le prefab � l'index {index} est vide !");
            return;
        }

        if (spawnPoint == null)
        {
            Debug.LogWarning("Aucun spawnPoint d�fini !");
            return;
        }

        Instantiate(prefabs[index], spawnPoint.position, spawnPoint.rotation);
    }
    public void SpawnRandomPrefab()
    {
        if (prefabs == null || prefabs.Length == 0)
        {
            Debug.LogWarning("Aucun prefab disponible pour le spawn al�atoire !");
            return;
        }

        if (spawnPoint == null)
        {
            Debug.LogWarning("Aucun spawnPoint d�fini !");
            return;
        }

        int randomIndex = Random.Range(0, prefabs.Length);
        GameObject prefabToSpawn = prefabs[randomIndex];

        if (prefabToSpawn == null)
        {
            Debug.LogWarning($"Le prefab al�atoire � l'index {randomIndex} est vide !");
            return;
        }

        Instantiate(prefabToSpawn, spawnPoint.position, spawnPoint.rotation);
        Debug.Log($"Prefab al�atoire spawn� : {prefabToSpawn.name}");
    }
}

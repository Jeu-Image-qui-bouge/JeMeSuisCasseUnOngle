using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BouleTchernobyl : MonoBehaviour
{
    [Header("Param�tres al�atoires")]
    [Range(0f, 1f)]
    public float chanceExplosion = 0.25f; // 25% de chance d'�tre pi�g�e
    private bool estExplosive;

    [Header("Explosion")]
    public float rayonExplosion = 5f;
    public float forceExplosion = 700f;
    public float delaiAvantExplosion = 2f;
    public GameObject effetExplosion; // Prefab de particules
    public AudioClip sonExplosion; // Son d'explosion
    private AudioSource audioSource;

    private bool aExplose = false;

    void Start()
    {
        // Tirage al�atoire au lancement
        estExplosive = Random.value < chanceExplosion;

        // R�cup�re ou ajoute un AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    void OnCollisionEnter(Collision collision)
    {
        // Si la boule est pi�g�e et qu�elle n�a pas encore explos�
        if (estExplosive && !aExplose)
        {
            aExplose = true;
            Invoke(nameof(Exploser), delaiAvantExplosion);
        }
    }

    void Exploser()
    {
        // Effet visuel
        if (effetExplosion != null)
            Instantiate(effetExplosion, transform.position, Quaternion.identity);

        // Son
        if (sonExplosion != null)
            audioSource.PlayOneShot(sonExplosion);

        // Appliquer la force aux objets autour
        Collider[] colliders = Physics.OverlapSphere(transform.position, rayonExplosion);
        foreach (Collider nearby in colliders)
        {
            Rigidbody rb = nearby.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(forceExplosion, transform.position, rayonExplosion);
            }
        }

        // D�truire la boule apr�s 0.2s pour laisser jouer le son
        Destroy(gameObject, 0.2f);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, rayonExplosion);
    }
}

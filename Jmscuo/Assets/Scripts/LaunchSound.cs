using UnityEngine;

public class LaunchSound : MonoBehaviour
{
    private static LaunchSound instance;

    [Header("Musique")]
    [SerializeField] private AudioClip music;
    [SerializeField, Range(0f, 1f)] private float volume = 0.6f;
    [SerializeField] private bool loop = true;

    private AudioSource source;

    void Awake()
    {
        // Empêche les doublons si la scène se recharge
        if (instance != null && instance != this) { Destroy(gameObject); return; }
        instance = this;

        DontDestroyOnLoad(gameObject);

        source = gameObject.AddComponent<AudioSource>();
        source.clip = music;
        source.loop = loop;
        source.playOnAwake = false;
        source.volume = volume;
        source.spatialize = false;      // musique 2D
        source.spatialBlend = 0f;       // 0 = 2D, 1 = 3D
    }

    void Start()
    {
        if (music != null) source.Play();
        else Debug.LogWarning("[LaunchSound] Assigne un AudioClip dans l’inspecteur.");
    }
}

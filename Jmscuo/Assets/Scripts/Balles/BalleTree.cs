using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class GrowAfterLaunch : MonoBehaviour
{
    [Header("Paramètres de croissance")]
    public float targetScaleMultiplier = 3f;
    public float growDelay = 1f; 
    public float growDuration = 2f; 

    private Vector3 initialScale;
    private Vector3 targetScale;
    private float growTimer = 0f;
    private bool hasLaunched = false;
    private bool isGrowing = false;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        initialScale = transform.localScale;
        targetScale = initialScale * targetScaleMultiplier;
    }

    private void Update()
    {
        if (!hasLaunched)
        { 
            if (rb.linearVelocity.magnitude > 0.1f)
            {
                hasLaunched = true;
                growTimer = 0f;
            }
        }
        else
        {
            growTimer += Time.deltaTime;

            if (growTimer >= growDelay && growTimer <= growDelay + growDuration)
            {
                isGrowing = true;
                float t = (growTimer - growDelay) / growDuration;
                transform.localScale = Vector3.Lerp(initialScale, targetScale, t);
            }
            else if (growTimer > growDelay + growDuration && isGrowing)
            {
                transform.localScale = targetScale;
                isGrowing = false;
            }
        }
    }
}

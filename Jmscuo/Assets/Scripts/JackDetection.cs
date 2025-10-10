using UnityEngine;

public class FollowClosestBallCanvas : MonoBehaviour
{
    [Header("Paramètres de détection")]
    public string ballTag = "Balle";
    public float detectionRange = 10f;
    public float uiHeightOffset = 0.3f;
    public float updateInterval = 0.2f;

    [Header("Canvas à déplacer (World Space)")]
    public GameObject canvasUI;

    [Header("Caméra du joueur (pour orienter le canvas)")]
    public Camera playerCamera;

    private GameObject currentClosestBall;
    private float nextUpdateTime;

    private void Start()
    {
        if (canvasUI == null)
        {
            return;
        }

        canvasUI.SetActive(false);
    }

    private void Update()
    {
        if (Time.time >= nextUpdateTime)
        {
            nextUpdateTime = Time.time + updateInterval;
            UpdateClosestBall();
        }

        if (canvasUI == null)
            return; 

        if (currentClosestBall != null)
        {
            Vector3 targetPosition = currentClosestBall.transform.position + Vector3.up * uiHeightOffset;
            canvasUI.transform.position = targetPosition;

            if (!canvasUI.activeSelf)
                canvasUI.SetActive(true);

            if (playerCamera != null)
            {
                canvasUI.transform.LookAt(playerCamera.transform);
                canvasUI.transform.Rotate(0, 180f, 0); 
            }
        }
        else
        {
            if (canvasUI.activeSelf)
                canvasUI.SetActive(false);
        }
    }

    private void UpdateClosestBall()
    {
        GameObject[] balls = GameObject.FindGameObjectsWithTag(ballTag);

        GameObject closest = null;
        float minDist = Mathf.Infinity;

        foreach (GameObject ball in balls)
        {
            float dist = Vector3.Distance(transform.position, ball.transform.position);
            if (dist < minDist && dist <= detectionRange)
            {
                minDist = dist;
                closest = ball;
            }
        }

        currentClosestBall = closest;
    }
}

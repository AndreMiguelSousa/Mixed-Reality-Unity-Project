using System.Collections;
using UnityEngine;

public class Flood : MonoBehaviour
{
    public float floodHeight = 1f;
    public float duration = 2f;
    public float delayBeforeFlood = 30f;

    public Transform waterPlane;
    public Transform cameraTransform;

    private bool hasFlooded = false;
    private bool hasDrained = false;

    // Mouse shake detection
    private float shakeThreshold = 500f;
    private Vector3 lastMousePosition;
    private float shakeTimer = 0f;
    private int shakeCount = 0;

    private void Start()
    {
        // Automatically start flood after delay
        Invoke(nameof(TriggerFlood), delayBeforeFlood);
        lastMousePosition = Input.mousePosition;
    }
    void Update()
    {
        if (!hasFlooded) return;

        DetectMouseShake();
    }

    public void TriggerFlood()
    {
        LeanTween.moveY(waterPlane.gameObject, floodHeight, duration).setEaseInOutSine();
        StartCoroutine(FloodEvent());
    }

    private IEnumerator FloodEvent()
    {
        LeanTween.moveY(waterPlane.gameObject, floodHeight, duration).setEaseInOutSine();

        // Wait for the flood animation to finish to prevent accidentally clearing it before it appears
        yield return new WaitForSeconds(duration);

        hasFlooded = true;
    }

    public void DrainFlood()
    {
        Debug.Log("Drained");
        LeanTween.moveY(waterPlane.gameObject, -1f, duration).setEaseInOutSine();
        hasDrained = true;
    }

    void DetectMouseShake()
    {
        Vector3 currentMousePosition = Input.mousePosition;
        float movement = (currentMousePosition - lastMousePosition).magnitude;
        float speed = movement / Time.deltaTime;

        if (speed > shakeThreshold && movement > 50f)
        {
            shakeCount++;
            shakeTimer = 0.3f;
        }

        lastMousePosition = currentMousePosition;

        if (shakeTimer > 0)
        {
            shakeTimer -= Time.deltaTime;

            if (shakeCount >= 3 && !hasDrained)
            {
                LeanTween.rotateZ(cameraTransform.gameObject, 10f, 1f).setLoopPingPong(1);
                DrainFlood();
            }
        }
        else
        {
            shakeCount = 0; // reset if time runs out
        }
    }
}

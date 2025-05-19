using UnityEngine;

public class Flood : MonoBehaviour
{
    public Transform waterPlane;
    public float floodHeight = 1f;
    public float duration = 2f;
    public float delayBeforeFlood = 30f;

    public TiltCity tiltCity;

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
        hasFlooded = true;
    }

    public void DrainFlood()
    {
        LeanTween.moveY(waterPlane.gameObject, -1f, duration).setEaseInOutSine();
        hasDrained = true;
    }

    void DetectMouseShake()
    {
        Vector3 currentMousePosition = Input.mousePosition;
        float speed = (currentMousePosition - lastMousePosition).magnitude / Time.deltaTime;

        if (speed > shakeThreshold)
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
                DrainFlood();
                tiltCity.Tilt();
            }
        }
        else
        {
            shakeCount = 0; // reset if time runs out
        }
    }
}

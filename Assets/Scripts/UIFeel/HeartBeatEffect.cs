using UnityEngine;

public class HeartBeatEffect : MonoBehaviour
{
    public float heartbeatSpeed = 1f;
    public float heartbeatAmount = 0.2f;
    public AnimationCurve scaleCurve;

    private Vector3 originalScale;
    private float initialTime;

    private void Start()
    {
        originalScale = transform.localScale;
        initialTime = Time.time;
    }

    private void Update()
    {
        float time = (Time.time - initialTime) * heartbeatSpeed;
        float scale = originalScale.x + (scaleCurve.Evaluate(time) * heartbeatAmount);

        transform.localScale = new Vector3(scale, scale, scale);

        if (time > scaleCurve[scaleCurve.length - 1].time)
        {
            initialTime = Time.time;
        }
    }
}

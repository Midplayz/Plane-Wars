using UnityEngine;
using UnityEngine.UI;

public class WobbleEffect : MonoBehaviour
{
    public float wobbleSpeed = 1f;
    public float wobbleAmount = 10f;
    public float rotationAmount = 15f;
    public float maxRotationAngle = 30f;

    private RectTransform rectTransform;
    private Vector3 originalPosition;

    private void Start()
    {

        originalPosition = rectTransform.anchoredPosition;

    }
    private void OnEnable()
    {
        rectTransform = GetComponent<RectTransform>();
        StartCoroutine(WobbleEffectEnum());
    }
    private void OnDisable()
    {
        StopCoroutine(WobbleEffectEnum());
    }
    private System.Collections.IEnumerator WobbleEffectEnum()
    {
        while (true)
        {
            float offsetX = Mathf.Sin(Time.time * wobbleSpeed) * wobbleAmount;
            float offsetY = Mathf.Cos(Time.time * wobbleSpeed) * wobbleAmount;

            rectTransform.anchoredPosition = originalPosition + new Vector3(offsetX, offsetY, 0f);

            // Calculate rotation based on movement direction
            Vector3 direction = new Vector3(offsetX, offsetY, 0f).normalized;
            float angle = Mathf.Clamp(Vector3.Angle(Vector3.up, direction) * Mathf.Sign(offsetX), -maxRotationAngle, maxRotationAngle);
            Quaternion rotation = Quaternion.Euler(0f, 0f, angle);
            rectTransform.rotation = Quaternion.Slerp(rectTransform.rotation, rotation, Time.deltaTime * rotationAmount);

            yield return null;
        }
    }
}

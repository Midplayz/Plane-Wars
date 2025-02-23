using UnityEngine;

public class UIScaleAnimation : MonoBehaviour
{
    public float scaleDuration = 1f;
    public float scaleAmount = 1.2f;

    private Vector3 originalScale;

    private void OnEnable()
    {
        originalScale = Vector3.one;
        StartCoroutine(ScaleAnimation());
    }

    private void OnDisable()
    {
        StopCoroutine(ScaleAnimation());
    }

    private System.Collections.IEnumerator ScaleAnimation()
    {
        while (true)
        {
            yield return ScaleTo(scaleAmount, scaleDuration);

            yield return ScaleTo(1f, scaleDuration);
        }
    }

    private System.Collections.IEnumerator ScaleTo(float targetScale, float duration)
    {
        Vector3 startScale = transform.localScale;
        Vector3 targetScaleVector = originalScale * targetScale;

        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            transform.localScale = Vector3.Lerp(startScale, targetScaleVector, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localScale = targetScaleVector;
    }
}

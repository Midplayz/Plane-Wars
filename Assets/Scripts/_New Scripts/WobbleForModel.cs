using UnityEngine;
using System.Collections;

public class WobbleForModel : MonoBehaviour
{
    public float wobbleSpeed = 1f;
    public float wobbleAmount = 10f;
    public float rotationAmount = 15f;
    public float maxRotationAngle = 30f;
    public float maxPositionOffset = 2f; // Maximum offset in X and Y directions

    private Transform transformOfObj;
    private Vector3 originalPosition;
    private Vector3 originalRotation;

    bool gotTheValues = false;
    private void OnEnable()
    {
        if (!gotTheValues)
        {
            transformOfObj = GetComponent<Transform>();
            originalPosition = transformOfObj.localPosition;
            originalRotation = transformOfObj.localRotation.eulerAngles;
            gotTheValues = true;
        }
        StartCoroutine(WobbleEffectEnum());
    }

    private void OnDisable()
    {
        StopCoroutine(WobbleEffectEnum());
    }

    private IEnumerator WobbleEffectEnum()
    {
        while (true)
        {
            float offsetX = Mathf.Sin(Time.time * wobbleSpeed) * wobbleAmount;
            float offsetY = Mathf.Cos(Time.time * wobbleSpeed) * wobbleAmount;

            offsetX = Mathf.Clamp(offsetX, -maxPositionOffset, maxPositionOffset);
            offsetY = Mathf.Clamp(offsetY, -maxPositionOffset, maxPositionOffset);

            Vector3 wobbleOffset = new Vector3(offsetX, offsetY, 0f);
            transformOfObj.localPosition = originalPosition + wobbleOffset;

            Vector3 currentRotation = originalRotation + new Vector3(0f, 0f, Mathf.Sin(Time.time * wobbleSpeed) * rotationAmount);
            transformOfObj.localRotation = Quaternion.Euler(currentRotation);

            yield return null;
        }
    }

}

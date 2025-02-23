using UnityEngine;

public class RotateUIElement : MonoBehaviour
{
    public float rotationSpeed = 180f;

    private RectTransform rectTransform;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        rectTransform.Rotate(Vector3.forward, rotationSpeed * Time.unscaledDeltaTime);
    }
}

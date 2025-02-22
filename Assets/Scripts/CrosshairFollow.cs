using UnityEngine;
using UnityEngine.EventSystems;

public class CrosshairFollow : MonoBehaviour
{
    [field: Header("------ On-Screen Crosshair Following ------")]
    [field: SerializeField] Joystick joystick;
    [field: SerializeField] RectTransform canvasRect;
    [field: SerializeField] float movementSpeed = 5f;
    [field: SerializeField] float returnSpeed = 10f;
    public bool enableRecentre = true;
    public bool enableControl = true;

    [field: Header("------ Lock On Mechanics ------")]
    [field: SerializeField] private RectTransform uiElement;
    [field: SerializeField] private Camera mainCamera;
    [field: SerializeField] private float boxLength = 10f;
    [field: SerializeField] private float boxSize = 1f;
    [field: SerializeField] private LayerMask layerMask;

    private Vector3 initialPosition;

    private void Start()
    {
        initialPosition = transform.position;
    }

    private void Update()
    {
        //if (enableControl && Mathf.Abs(joystick.Horizontal) > 0.1f || enableControl && Mathf.Abs(joystick.Vertical) > 0.1f)
        //{
        //    Vector3 moveDirection = new Vector3(joystick.Horizontal, joystick.Vertical, 0f);

        //    Vector3 newPosition = transform.position + moveDirection * movementSpeed * Time.deltaTime;

        //    Vector3 clampedPosition = ClampPositionWithinCanvas(newPosition);

        //    transform.position = clampedPosition;
        //}
        //else
        //{
            if (enableRecentre)
            {
                transform.position = Vector3.Lerp(transform.position, initialPosition, returnSpeed * Time.deltaTime);
                if(transform.position == initialPosition)
                {
                    enableRecentre = false;
                }
            }
        //}
    }

    private Vector3 ClampPositionWithinCanvas(Vector3 position)
    {
        Vector3 localPosition = canvasRect.InverseTransformPoint(position);

        Vector2 canvasSize = canvasRect.rect.size;

        Vector2 imageHalfSize = GetComponent<RectTransform>().rect.size * 0.5f;

        Vector2 minPosition = imageHalfSize - canvasSize * 0.5f;
        Vector2 maxPosition = canvasSize * 0.5f - imageHalfSize;

        Vector2 clampedLocalPosition = new Vector2(
            Mathf.Clamp(localPosition.x, minPosition.x, maxPosition.x),
            Mathf.Clamp(localPosition.y, minPosition.y, maxPosition.y)
        );

        Vector3 clampedPosition = canvasRect.TransformPoint(clampedLocalPosition);

        return clampedPosition;
    }

    
}

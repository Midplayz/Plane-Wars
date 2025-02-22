using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class LockIntoObject : MonoBehaviour
{
    [field: Header("------ Lock Crosshair Onto Opponents ------")]
    [field: SerializeField] float lockTime = 1f;  
    [field: SerializeField] PlaneMovement PlaneMovement;  
    [field: SerializeField] RectTransform canvas;  
    public GameObject targetObject;
    public bool hasJustLocked = false;

    private UnityEngine.UI.Image uiImage;  
    private Vector3 offset; 
    private bool isLocked = false;  
    private float timer = 0f;  

    private void Start()
    {
        uiImage = GetComponent<UnityEngine.UI.Image>();
    }

    private void Update()
    {
        if (isLocked)
        {
            if (!IsElementInsideCanvas())
            {
                UnlockImage();
                GetComponent<CrosshairFollow>().enableRecentre = true;
                GetComponent<CrosshairFollow>().enableControl = true;
                targetObject = null;
                timer = 0f;
                isLocked = false;
            }
            timer += Time.deltaTime;
            if (targetObject != null)
            {
                LockImage();
            }
            if (targetObject == null)
            {
                UnlockImage();
                GetComponent<CrosshairFollow>().enableRecentre = true;
                GetComponent<CrosshairFollow>().enableControl = true;
                timer = 0f;
                isLocked = false;
            }
            if (targetObject != null && timer >= lockTime)
            {
                UnlockImage();
                GetComponent<CrosshairFollow>().enableRecentre = true;
                GetComponent<CrosshairFollow>().enableControl = true;
                targetObject = null;
                timer = 0f;
                isLocked = false;
            }
        }
        else if(targetObject != null && !isLocked)
        {
            GetComponent<CrosshairFollow>().enableControl = false;
            GetComponent<CrosshairFollow>().enableRecentre = false;
            LockImage();
        }
    }

    private void LockImage()
    {
        offset = targetObject.transform.position - Camera.main.WorldToScreenPoint(transform.position);

        uiImage.rectTransform.position = Camera.main.WorldToScreenPoint(targetObject.transform.position);

        isLocked = true;
        hasJustLocked = true;
    }

    private void UnlockImage()
    {
        timer = 0f;

        isLocked = false;
        PlaneMovement.followTarget = PlaneMovement.cube;
        Invoke("ChangeBoolValue", 1.5f);
    }

    private void ChangeBoolValue()
    {
        hasJustLocked = false; 
    }

    private bool IsElementInsideCanvas()
    {
        Vector2 canvasSize = canvas.rect.size;

        Vector2 imageHalfSize = GetComponent<RectTransform>().rect.size * 0.5f;

        Vector2 minPosition = imageHalfSize - canvasSize * 0.5f;
        Vector2 maxPosition = canvasSize * 0.5f - imageHalfSize;

        Vector2 uiImagePosition = new Vector2(uiImage.rectTransform.position.x, uiImage.rectTransform.position.y);
        if(uiImagePosition.x > minPosition.x && uiImagePosition.y > minPosition.y && uiImagePosition.x < maxPosition.x && uiImagePosition.y < maxPosition.y) 
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}

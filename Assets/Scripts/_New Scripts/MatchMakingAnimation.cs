using UnityEngine;
using UnityEngine.UI;

public class MatchMakingAnimation : MonoBehaviour
{
    public RectTransform boundsRectTransform; // Assign the RectTransform within which the UI element should stay.
    public float moveSpeed = 50f; // Adjust this to control the movement speed.

    private RectTransform myRectTransform;
    private Vector2 minPosition;
    private Vector2 maxPosition;
    private Vector3 targetPosition;

    void Start()
    {
        // Get a reference to the RectTransform component of the UI element.
        myRectTransform = GetComponent<RectTransform>();

        // Calculate the min and max positions within the assigned bounds.
        CalculateMinMaxPositions();

        // Start moving the UI element.
        SetRandomTargetPosition();
    }

    void Update()
    {
        // Move towards the target position smoothly.
        myRectTransform.anchoredPosition = Vector3.MoveTowards(myRectTransform.anchoredPosition, targetPosition, moveSpeed * Time.deltaTime);

        // Check if we've reached the target position, then set a new target.
        if (Vector3.Distance(myRectTransform.anchoredPosition, targetPosition) < 0.1f)
        {
            SetRandomTargetPosition();
        }
    }

    void CalculateMinMaxPositions()
    {
        if (boundsRectTransform != null)
        {
            Vector2 parentSize = boundsRectTransform.rect.size;
            Vector2 mySize = myRectTransform.rect.size;

            // Calculate the minimum and maximum positions within the assigned bounds.
            minPosition = new Vector2(-parentSize.x / 2 + mySize.x / 2, -parentSize.y / 2 + mySize.y / 2);
            maxPosition = new Vector2(parentSize.x / 2 - mySize.x / 2, parentSize.y / 2 - mySize.y / 2);
        }
    }

    void SetRandomTargetPosition()
    {
        // Generate a random target position within the bounds.
        float randomX = Random.Range(minPosition.x, maxPosition.x);
        float randomY = Random.Range(minPosition.y, maxPosition.y);
        targetPosition = new Vector3(randomX, randomY, 0);
    }
}
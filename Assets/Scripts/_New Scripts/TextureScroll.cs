using Google.Protobuf.WellKnownTypes;
using UnityEngine;
using UnityEngine.UI;

public class TextureScroll : MonoBehaviour
{
    [field: SerializeField] private float scrollSpeed = 0.5f;
    [field: SerializeField] private bool  isForMatchMaking = false;
    [field: SerializeField] private float lerpingTime;
    [field: SerializeField] private float timeForChanging;

    private Image image;
    private Material customMaterial;
    private float offset = 0f;

    private float startValue;
    private float endValue;

    float timer = 0f;

    private void Start()
    {
        image = GetComponent<Image>();

        // Create a new instance of the material so it doesn't affect other images.
        customMaterial = new Material(image.material);
        image.material = customMaterial;
    }

    private void Update()
    {
        offset += Time.deltaTime * scrollSpeed;
        if (offset > 1.0f)
        {
            offset -= 1.0f;
        }
        customMaterial.SetTextureOffset("_MainTex", new Vector2(offset, 0f));
        if(isForMatchMaking) 
        { 
            timer += Time.deltaTime;
            if(timer > timeForChanging)
            {
                startValue = scrollSpeed;
                endValue = scrollSpeed * -1;
                StartCoroutine(LerpFloat());
                timer = 0f;
            }
        }

    }

    System.Collections.IEnumerator LerpFloat()
    {
        float startTime = Time.time;
        float endTime = startTime + lerpingTime;

        while (Time.time < endTime)
        {
            float t = (Time.time - startTime) / lerpingTime;
            scrollSpeed = Mathf.Lerp(startValue, endValue, t);
            yield return null; // Wait for the next frame.
        }

        scrollSpeed = endValue;
    }

}

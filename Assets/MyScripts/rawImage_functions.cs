using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class rawImage_functions : MonoBehaviour
{
    private RawImage rawImage;
    float minScale = 25f;
    float maxScale = 300f;

    void Start()
    {
        rawImage = GetComponent<RawImage>();
    }

    public void ChangeColor()
    {
        float r = Random.Range(0f, 1f);
        float g = Random.Range(0f, 1f);
        float b = Random.Range(0f, 1f);
        rawImage.color = new Color(r, g, b, 1f);
    }

    public void ChangeScale()
    {
        rawImage.rectTransform.sizeDelta = new Vector2(Random.Range(minScale, maxScale), Random.Range(minScale, maxScale));
    }

}

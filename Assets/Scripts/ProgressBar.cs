using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    [SerializeField] private Color startColor = Color.red, endColor = Color.green;

    private GameObject canvas;
    private Image fillImage;

    private float fillPercent = 0f, lastFillPercent = 0f;

    private void Awake()
    {
        canvas = transform.GetChild(0).gameObject;
        fillImage = canvas.transform.GetChild(1).GetComponent<Image>();
    }

    void Update()
    {
        // point canvas at camera
        transform.rotation = Camera.main.transform.rotation; 

        // change how full the bar is if the value has changed
        if (fillPercent != lastFillPercent)
        {
            fillImage.fillAmount = fillPercent;
            fillImage.color = Color.Lerp(startColor, endColor, Mathf.PingPong(fillPercent, 1));

            lastFillPercent = fillPercent;
        }
    }

    public void SetFillLevel(float fill)
    {
        fillPercent = fill;
    }
}

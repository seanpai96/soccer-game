using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderHighlightController : MonoBehaviour
{
    public Renderer objectRenderer;
    public Color highlightShader;
    private Color originalShader; 

    void Start()
    {
        originalShader = objectRenderer.material.color; 
    }

    public void HoverSliderColorChanged()
    {
        objectRenderer.material.color = highlightShader;
    }

    public void LeaveSliderColorChanged()
    {
        objectRenderer.material.color = originalShader;
    }
}

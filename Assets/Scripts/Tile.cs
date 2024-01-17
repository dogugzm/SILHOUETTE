using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum COLOR_TYPES
{
    SOFT_SHADOW,
    HARD_SHADOW,
    NEAR_COLOR,
    WRONG_COLOR,
    DEFAULT
}


public class Tile : MonoBehaviour
{

    public MeshRenderer[] renderers;
    public bool isSuitable = false;

    [SerializeField] Color softShadowColor;
    [SerializeField] Color hardShadowColor;
    [SerializeField] Color nearColor;
    [SerializeField] Color wrongColor;
    [SerializeField] Color defaultColor;


    private void Start()
    {
        renderers = GetComponentsInChildren<MeshRenderer>();

    }

    Color GetColor(COLOR_TYPES color)
    {
        switch (color)
        {
            case COLOR_TYPES.SOFT_SHADOW:
                return softShadowColor;
            case COLOR_TYPES.HARD_SHADOW:
                return hardShadowColor;
            case COLOR_TYPES.DEFAULT:
                return defaultColor;
            case COLOR_TYPES.NEAR_COLOR:
                return nearColor;
            case COLOR_TYPES.WRONG_COLOR:
                return wrongColor;
        }
        return defaultColor;
    }

    public void ChangeColor(COLOR_TYPES color)
    {
        foreach (var renderer in renderers)
        {
            renderer.material.color = GetColor(color);
        }
    }
}

using UnityEngine;

public enum COLOR_TYPES
{
    SOFT_SHADOW,
    WALL_SHADOW,
    NEAR_COLOR,
    WRONG_COLOR,
    DEFAULT
}

public class Tile : MonoBehaviour
{
    //private int tileGrade = 2;

    public MeshRenderer tileRenderer;
    public bool isNear = false;
    
    [SerializeField] Color nearColor;
    [SerializeField] Color wrongColor;
    [SerializeField] Color defaultColor;
    [SerializeField] Color wallShadowColor;
    [SerializeField] Color hardShadowColor;


    Color GetColor(COLOR_TYPES color)
    {
        switch (color)
        {
            case COLOR_TYPES.DEFAULT:
                return defaultColor;
            case COLOR_TYPES.NEAR_COLOR:
                return nearColor;
            case COLOR_TYPES.WRONG_COLOR:
                return wrongColor;
            case COLOR_TYPES.WALL_SHADOW:
                return wallShadowColor;
            case COLOR_TYPES.SOFT_SHADOW:
                return hardShadowColor;
        }
        return defaultColor;
    }

   
    public void ChangeColor(COLOR_TYPES color)
    {
        tileRenderer.material.color = GetColor(color);
    }
}

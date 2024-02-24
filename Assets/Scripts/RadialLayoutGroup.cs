using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class RadialLayoutGroup : LayoutGroup
{
    [Range(0f, 360f)]
    public float startAngle = 0f;
    public float angleStep = 30f;
    public float radius = 100f;

    public override void CalculateLayoutInputHorizontal()
    {
        base.CalculateLayoutInputHorizontal();
        SetLayout();
    }

    public override void CalculateLayoutInputVertical()
    {
        SetLayout();
    }

    private void SetLayout()
    {
        if (transform.childCount == 0)
            return;

        float currentAngle = startAngle;
        float stepAngle = angleStep;

        foreach (RectTransform rect in rectChildren)
        {
            SetChildAlongAxis(rect, 0, Mathf.Cos(currentAngle * Mathf.Deg2Rad) * radius, rect.rect.width);
            SetChildAlongAxis(rect, 1, Mathf.Sin(currentAngle * Mathf.Deg2Rad) * radius, rect.rect.height);

            currentAngle += stepAngle;
        }
    }

    public override void SetLayoutHorizontal()
    {
    }

    public override void SetLayoutVertical()
    {
    }
}

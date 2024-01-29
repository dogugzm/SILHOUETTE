using DG.Tweening;
using UnityEngine;

public class Cube : MonoBehaviour
{
    private MeshRenderer cubeRenderer;
    [HideInInspector] public Color defaultColor;
    public Color deleteColor;

    private void Awake()
    {
        cubeRenderer = GetComponent<MeshRenderer>();
        defaultColor = cubeRenderer.material.color;
    }

    public void ChangeColor(Color color, float time)
    {
        cubeRenderer.material.DOColor(color, time);
    }

}

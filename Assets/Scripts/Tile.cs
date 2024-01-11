using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public MeshRenderer[] renderers;
    public bool isSuitable = false;

    private void Start()
    {
        renderers = GetComponentsInChildren<MeshRenderer>();

    }

    public void ChangeColor(Color color)
    {
        foreach (var renderer in renderers)
        {
            renderer.material.color = color;
        }
    }
}

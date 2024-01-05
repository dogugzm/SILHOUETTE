using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public MeshRenderer[] renderers;

    private void Start()
    {
        renderers = GetComponentsInChildren<MeshRenderer>();

    }

    public void ChangeColor()
    {
        foreach (var renderer in renderers)
        {
            renderer.material.color = Color.gray;
        }
    }
}

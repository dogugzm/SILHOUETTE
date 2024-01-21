using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : MonoBehaviour
{

    MeshRenderer renderer;

    private void Start()
    {
        renderer = GetComponent<MeshRenderer>();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="color"></param>
    /// <param name="time"></param>
    /// <param name="boolCheck">bool parameter to make it !bool after completed</param>
    public void ChangeColor(Color color,float time,bool boolCheck)
    {
        renderer.material.DOColor(color, time).OnComplete(() => boolCheck = !boolCheck);
    }



}

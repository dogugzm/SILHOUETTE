using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SwipeControl : MonoBehaviour
{
    public Scrollbar scrollbar;
    float currentScrollPos = 0.5f;
    public float[] pos;
    public float distance;

    // Start is called before the first frame update
    void Start()
    {
        pos = new float[transform.childCount];
        distance = 1f / (pos.Length - 1f);
        for (int i = 0; i < pos.Length; i++)
        {
            pos[i] = distance * i;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            currentScrollPos = scrollbar.value;
        }
        else
        {
            for (int i = 0;i < pos.Length;i++)
            {
                if (currentScrollPos < pos[i] + (distance / 2) && currentScrollPos > pos[i] - (distance / 2))
                {
                    scrollbar.value = Mathf.Lerp(scrollbar.value, pos[i], 0.1f);
                }
            }
        }
    }



}

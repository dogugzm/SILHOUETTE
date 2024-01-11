using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{

    public static Action<int> OnGenerateLevelCalled;
    [SerializeField] int size;

    void GenerateLevel()
    {
        OnGenerateLevelCalled?.Invoke(size);
        // random wall shadow generate(sayý,compx)
        // clickable center
        // check finished
    }

    private void Start()
    {
        GenerateLevel();
    }




}

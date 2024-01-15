using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{

    //public static Action<int> OnGenerateLevelCalled;
    [SerializeField] int size;

    [SerializeField] WallGrid wallX;
    [SerializeField] WallGrid wallZ;

    async void GenerateLevel()
    {
        wallZ.CreateWallAsync();
        await wallX.CreateWallAsync();

        //for (int i = 0; i < size; i++)
        //{
        //    await wallX.SetShadowTile(wallX.GetRandomTupleFromSuitable());
        //    await UniTask.DelayFrame(1);
        //}
        //// wallX.ShowProceduralShadow();
        //Debug.Log("x shadow length : " + wallX.shadowTuples.Count);
        //wallZ.SetSuitableFromShadow(wallX.shadowTuples);
    }

    private void Start()
    {
        GenerateLevel();
    }




}

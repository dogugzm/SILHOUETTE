using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

enum ProjectionAxis
{
    XBased,
    ZBased
}

public class WallGrid : MonoBehaviour
{

    [SerializeField] int size;
    [SerializeField] Tile TilePrefab;
    Dictionary<Tuple<int, int>, Tile> grid = new();
    List<Tuple<int, int>> shadowGrid = new();

    [SerializeField] Vector3 startingPositionOffset;
    [SerializeField] Vector3 startingRotationOffset;
    [SerializeField] ProjectionAxis projectionAxis;

    List<Tuple<int, int>> suitableTuples = new List<Tuple<int, int>>();

    private void OnEnable()
    {
        Instantiator.OnCubeCreatedTriggered += ChangeTileShadow;
        LevelGenerator.OnGenerateLevelCalled += GenerateRandomShadow;
    }
    private void OnDisable()
    {
        Instantiator.OnCubeCreatedTriggered -= ChangeTileShadow;
        LevelGenerator.OnGenerateLevelCalled -= GenerateRandomShadow;
    }
    private void Start()
    {
        transform.SetPositionAndRotation(startingPositionOffset, Quaternion.Euler(startingRotationOffset));
    }

    private async void GenerateRandomShadow(int pSize)
    {
        await CreateWallAsync();

        for (int i = 0; i < pSize; i++)
        {
            Tuple<int, int> randomPos = GenerateUniqueRandomPosition();
            ChangeTileShadow(randomPos);
        }
    }

  

    private void UpdateSuitableTiles(Dictionary<Tuple<int, int>, Tile> grid)
    {

        foreach (var kvp in grid)
        {
            // Her bir Tile'ýn isSuitable özelliðini kontrol etme
            if (kvp.Value.isSuitable)
            {
                suitableTuples.Add(kvp.Key);
            }
        }
    }

    private Tuple<int, int> GenerateUniqueRandomPosition()
    {
        // get random tuple from suitable tile

        Tuple<int, int> randomPos = suitableTuples[UnityEngine.Random.Range(0, suitableTuples.Count)];

        if (shadowGrid.Contains(randomPos))
        {
            // If the position is already in the shadowGrid, recursively call the method again to generate a new position.
            return GenerateUniqueRandomPosition();
        }

        // If the position is unique, return it.
        return randomPos;
    }

    private async UniTask CreateWallAsync()
    {
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                Tile instantiatedTile = Instantiate(TilePrefab, transform);
                instantiatedTile.transform.localPosition = new Vector3(i, 0, j);
                instantiatedTile.name = "Tile" + i + "_" + j;
                grid.Add(new Tuple<int, int>(i, j), instantiatedTile);
                if (i is 0)
                {
                    instantiatedTile.isSuitable = true;
                }

                // Wait for the next frame
                await UniTask.DelayFrame(5);
            }
        }
        UpdateSuitableTiles(grid);

    }

    

    void ChangeTileShadow(Vector3 position)
    {

        Tuple<int, int> gridPos;
        if (projectionAxis is ProjectionAxis.XBased)
        {
            gridPos = new Tuple<int, int>((int)position.y, (int)position.z);
        }
        else
        {
            gridPos = new Tuple<int, int>((int)position.x, (int)position.y);

        }

        if (grid.TryGetValue(gridPos, out Tile tile))
        {
            tile.ChangeColor(Color.grey);
            shadowGrid.Add(gridPos);

        }

        
    }

    void CalculateSuitableTiles(List<Tuple<int,int>> suitableCenters)
    {
        // +1 lerinde tile yoksa o +1 ler benim için suitabledýr.
        foreach (var item in suitableCenters)
        {
            Tuple<int, int> tupleXNeg = new(item.Item1 + 1, item.Item2);
            Tuple<int, int> tupleYNeg = new(item.Item1 , item.Item2 + 1);
            Tuple<int, int> tupleXYNeg = new(item.Item1 + 1, item.Item2 + 1);

            if (grid.TryGetValue(tupleXYNeg, out Tile tileX))
            {
                if (!shadowGrid.Contains(tupleXNeg))
                {
                    tileX.isSuitable = true;
                    tileX.ChangeColor(Color.cyan);
                }
            }
            if (grid.TryGetValue(tupleXYNeg, out Tile tileY))
            {
                if (!shadowGrid.Contains(tupleYNeg))
                {
                    tileY.isSuitable = true;
                    tileX.ChangeColor(Color.cyan);

                }
            }
            if (grid.TryGetValue(tupleXYNeg, out Tile tileXY))
            {
                if (!shadowGrid.Contains(tupleXYNeg))
                {
                    tileXY.isSuitable = true;
                    tileX.ChangeColor(Color.cyan);

                }
            }

        }

    }



    void ChangeTileShadow(Tuple<int, int> gridPos)
    {

        if (grid.TryGetValue(gridPos, out Tile tile))
        {
            tile.ChangeColor(Color.grey);
            shadowGrid.Add(gridPos);


        }

        CalculateSuitableTiles(shadowGrid);
        UpdateSuitableTiles(grid);
    }

}




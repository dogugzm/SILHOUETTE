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
    public List<Tuple<int, int>> shadowGrid = new();

    [SerializeField] Vector3 startingPositionOffset;
    [SerializeField] Vector3 startingRotationOffset;
    [SerializeField] ProjectionAxis projectionAxis;

    public List<Tuple<int, int>> suitableTuples = new List<Tuple<int, int>>();

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
    private async UniTask CreateWallAsync()
    {
        int randomJ = UnityEngine.Random.Range(0, size);
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                Tile instantiatedTile = Instantiate(TilePrefab, transform);
                instantiatedTile.transform.localPosition = new Vector3(i, 0, j);
                instantiatedTile.name = "Tile" + i + "_" + j;
                Tuple<int, int> instantiatedTuple = new(i, j);
                grid.Add(instantiatedTuple, instantiatedTile);
                if ((i == 0) && (j == randomJ))
                {
                    instantiatedTile.isSuitable = true;
                    suitableTuples.Add(instantiatedTuple);
                    Debug.Log("suitable setted");
                }

                // Wait for the next frame
                await UniTask.DelayFrame(5);
            }
        }
    }


    private Tuple<int, int> GenerateUniqueRandomPosition()
    {
        // get random tuple from suitable tile
        Debug.Log(suitableTuples.Count);
        Tuple<int, int> randomPos = suitableTuples[UnityEngine.Random.Range(0, suitableTuples.Count)];
        


        if (shadowGrid.Contains(randomPos))
        {
            // If the position is already in the shadowGrid, recursively call the method again to generate a new position.
            return GenerateUniqueRandomPosition();
        }

        // If the position is unique, return it.
        return randomPos;
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

    void ChangeTileShadow(Tuple<int, int> gridPos)
    {

        if (grid.TryGetValue(gridPos, out Tile tile))
        {
            tile.ChangeColor(Color.grey);
            shadowGrid.Add(gridPos);
            suitableTuples.Remove(gridPos);
        }
        CalculateSuitableTiles(shadowGrid);
    }

    void CalculateSuitableTiles(List<Tuple<int, int>> suitableCenters)
    {
        // +1 lerinde tile yoksa o +1 ler benim için suitabledýr.
        foreach (var item in suitableCenters)
        {
            Tuple<int, int> tupleXNeg = new(item.Item1 + 1, item.Item2);
            Tuple<int, int> tupleYNeg = new(item.Item1, item.Item2 + 1);
            Tuple<int, int> tupleXYNeg = new(item.Item1 , item.Item2 - 1);


            if (grid.TryGetValue(tupleXNeg, out Tile tileX))
            {
                if (!shadowGrid.Contains(tupleXNeg))
                {
                    tileX.isSuitable = true;
                    suitableTuples.Add(tupleXNeg);
                    tileX.ChangeColor(Color.cyan);
                }
            }
            if (grid.TryGetValue(tupleYNeg, out Tile tileY))
            {
                if (!shadowGrid.Contains(tupleYNeg))
                {
                    tileY.isSuitable = true;
                    suitableTuples.Add(tupleYNeg);
                    tileY.ChangeColor(Color.cyan);

                }
            }
            if (grid.TryGetValue(tupleXYNeg, out Tile tileXY))
            {
                if (!shadowGrid.Contains(tupleXYNeg))
                {
                    tileXY.isSuitable = true;
                    suitableTuples.Add(tupleXYNeg); 
                    tileXY.ChangeColor(Color.cyan);

                }
            }

        }

    }

}




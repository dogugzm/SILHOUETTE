using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;

enum ProjectionAxis
{
    XBased,
    ZBased
}

public class WallGrid : MonoBehaviour
{
    [SerializeField] Tile TilePrefab;
    readonly int size = 5;

    Dictionary<Tuple<int, int>, Tile> grid = new();
    public List<Tuple<int, int>> shadowTuples = new();
    private readonly List<Tuple<int, int>> correctTuples = new();
    private readonly List<Tuple<int, int>> wrongTuples = new();
    private readonly List<Tuple<int, int>> suitableTuples = new List<Tuple<int, int>>();

    public bool isCompleted = false;

    public Action WallCompleted;
    int maxXValue = 0;
    int maxYValue = 0;

    [SerializeField] Vector3 startingPositionOffset;
    [SerializeField] Vector3 startingRotationOffset;
    [SerializeField] ProjectionAxis projectionAxis;


    private void OnEnable()
    {
        CubeInstantiator.OnCubeCreatedTriggered += SetShadowTile;
        CubeInstantiator.OnCubeDeletedTriggered += RemoveShadowTile;
        LevelGenerator.LevelFinished += LevelCompleted;
    }
    private void OnDisable()
    {
        CubeInstantiator.OnCubeCreatedTriggered -= SetShadowTile;
        CubeInstantiator.OnCubeDeletedTriggered -= RemoveShadowTile;
        LevelGenerator.LevelFinished -= LevelCompleted;
    }
    private void Start()
    {
        transform.SetPositionAndRotation(startingPositionOffset, Quaternion.Euler(startingRotationOffset));
    }

    private void LevelCompleted()
    {
        ClearWall();
    }

    public async UniTask CreateWallAsync()
    {
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                Tile instantiatedTile = Instantiate(TilePrefab, transform);
                instantiatedTile.transform.localPosition = new Vector3(i, 0, j);
                instantiatedTile.name = "Tile" + i + "_" + j;
                Tuple<int, int> instantiatedTuple = new(i, j);
                grid.Add(instantiatedTuple, instantiatedTile);
                await UniTask.Delay(50);
            }
        }
    }

    public Tuple<int, int> GetTupleFromNears()
    {
        if (suitableTuples.Count == 0)
        {
            Tuple<int, int> randomFirstTuple = new(0, UnityEngine.Random.Range(0, size));
            suitableTuples.Add(randomFirstTuple);
            if (grid.TryGetValue(randomFirstTuple, out Tile value))
            {
                //value.ChangeColor(COLOR_TYPES.NEAR_COLOR);
            }
        }

        Tuple<int, int> randomPos = suitableTuples[UnityEngine.Random.Range(0, suitableTuples.Count)];

        if (shadowTuples.Contains(randomPos))
        {
            return GetTupleFromNears();
        }

        // If the position is unique, return it.
        return randomPos;
    }

    private void ClearWall()
    {
        foreach (var item in shadowTuples)
        {
            if (grid.TryGetValue(item, out Tile tile))
            {
                tile.ChangeColor(COLOR_TYPES.DEFAULT);
            }
        }
        shadowTuples.Clear();
        correctTuples.Clear();
        suitableTuples.Clear();
        wrongTuples.Clear();
        isCompleted = false;    
    }

    void SetShadowTile(Vector3 position)
    {
        Tuple<int, int> gridPos = GetTupleFromVector3(position);

        if (grid.TryGetValue(gridPos, out Tile tile))
        {
            if (shadowTuples.Contains(gridPos))
            {
                tile.ChangeColor(COLOR_TYPES.HARD_SHADOW);
                if (!correctTuples.Contains(gridPos))
                {
                    correctTuples.Add(gridPos);
                    Debug.Log(correctTuples.Count,gameObject);
                }
            }
            else
            {
                if (!wrongTuples.Contains(gridPos))
                {
                    wrongTuples.Add(gridPos);
                }
                tile.ChangeColor(COLOR_TYPES.WRONG_COLOR);
            }

        }
        CheckLevelCompleted();
    }

    private void CheckLevelCompleted()
    {
        if (CompareTuples(shadowTuples, correctTuples))
        {
            if (isCompleted)
            {
                return;
            }
            isCompleted = true;
            WallCompleted?.Invoke();
        }
    }

    private void RemoveShadowTile(Vector3 position,bool alreadyHaveShadowX, bool alreadyHaveShadowY)
    {
        Tuple<int, int> gridPos = GetTupleFromVector3(position);

        bool anyOtherShadow = projectionAxis == ProjectionAxis.XBased ? alreadyHaveShadowY : alreadyHaveShadowX; 

        if (grid.TryGetValue(gridPos, out Tile tile))
        {
            if (shadowTuples.Contains(gridPos))
            {
                if (anyOtherShadow)
                {
                    tile.ChangeColor(COLOR_TYPES.HARD_SHADOW);
                }
                else
                {
                    if (correctTuples.Contains(gridPos))
                    {
                        correctTuples.Remove(gridPos);
                    }                         
                    tile.ChangeColor(COLOR_TYPES.WALL_SHADOW);
                }
            }
            else if (wrongTuples.Contains(gridPos))
            {
                if (wrongTuples.Contains(gridPos))
                {
                    wrongTuples.Remove(gridPos);
                }
                tile.ChangeColor(COLOR_TYPES.DEFAULT);
            }
            else
            {
                tile.ChangeColor(COLOR_TYPES.DEFAULT);
            }

        }

        CheckLevelCompleted();

    }

    public async UniTask SetShadowTile(Tuple<int, int> gridPos)
    {
        if (grid.TryGetValue(gridPos, out Tile tile))
        {
            shadowTuples.Add(gridPos);
            if (gridPos.Item1 > maxXValue)
            {
                maxXValue = gridPos.Item1;
            }
            if (gridPos.Item2 > maxYValue)
            {
                maxYValue = gridPos.Item2;
            }
            suitableTuples.Remove(gridPos);
            tile.ChangeColor(COLOR_TYPES.WALL_SHADOW);
        }
        SetNearTiles(shadowTuples);
        await UniTask.DelayFrame(10);
    }

    void SetNearTiles(List<Tuple<int, int>> suitableCenters)
    {
        // +1 lerinde tile yoksa o +1 ler benim için suitabledýr.
        foreach (var item in suitableCenters)
        {
            Tuple<int, int> tupleXNeg = new(item.Item1 + 1, item.Item2);
            Tuple<int, int> tupleYNeg = new(item.Item1, item.Item2 + 1);
            Tuple<int, int> tupleXYNeg = new(item.Item1, item.Item2 - 1);

            if (grid.TryGetValue(tupleXNeg, out Tile tileX))
            {
                if (!shadowTuples.Contains(tupleXNeg))
                {
                    tileX.isNear = true;
                    suitableTuples.Add(tupleXNeg);
                    //tileX.ChangeColor(COLOR_TYPES.NEAR_COLOR);
                }
            }
            if (grid.TryGetValue(tupleYNeg, out Tile tileY))
            {
                if (!shadowTuples.Contains(tupleYNeg))
                {
                    tileY.isNear = true;
                    suitableTuples.Add(tupleYNeg);
                    //tileY.ChangeColor(COLOR_TYPES.NEAR_COLOR);

                }
            }
            if (grid.TryGetValue(tupleXYNeg, out Tile tileXY))
            {
                if (!shadowTuples.Contains(tupleXYNeg))
                {
                    tileXY.isNear = true;
                    suitableTuples.Add(tupleXYNeg);
                    //tileXY.ChangeColor(COLOR_TYPES.NEAR_COLOR);

                }
            }

        }

    }

    bool CompareTuples(List<Tuple<int, int>> list1, List<Tuple<int, int>> list2)
    {

        if (wrongTuples.Count > 0)
        {
            return false;
        }

        if (list1.Count != list2.Count)
            return false;

        //unordered
        HashSet<Tuple<int, int>> set1 = new(list1);
        HashSet<Tuple<int, int>> set2 = new(list2);

        return set1.SetEquals(set2);
    }

    public async void SetShadowFromOtherWall(List<Tuple<int, int>> shadows,float percantage)
    {
        bool firstTime = true;

        foreach (var item in shadows)
        {
            Debug.Log(item);

            if (firstTime)
            {
                int x = item.Item1;
                int y = UnityEngine.Random.Range(0, size - 1);

                //it can be 2 times also.
                Tuple<int, int> firstShadowTuple = new(x, y);
                SetShadowTile(firstShadowTuple);
                firstTime = false;
                continue;
            }

            if (shadowTuples.Any(tuple => tuple.Item1 == item.Item1))
            {
                //ayný sýrada max 1 shadow(hardest)
                int rndm = UnityEngine.Random.Range(0, 100);
                if (rndm <= percantage)
                {
                    continue;
                }
            }

            if (suitableTuples.Any(tuple => tuple.Item1 == item.Item1))
            {
                var suitableOnRow = suitableTuples.Where(tuple => tuple.Item1 == item.Item1);
                //it can be 2 times as well
                Tuple<int, int> tupleRandom = suitableOnRow.ElementAt(UnityEngine.Random.Range(0, suitableOnRow.Count()));
                SetShadowTile(tupleRandom);
                await UniTask.DelayFrame(10);

                continue;
            }

            await UniTask.DelayFrame(10);

        }

    }

    public Tuple<int, int> GetTupleFromVector3(Vector3 position)
    {
        if (projectionAxis is ProjectionAxis.XBased)
        {
            return new Tuple<int, int>((int)position.y, (int)position.z);
        }
        else
        {
            return new Tuple<int, int>((int)position.y, (int)position.x);
        }
    }


}




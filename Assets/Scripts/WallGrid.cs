using System;
using System.Collections;
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

    [SerializeField] Vector3 startingPositionOffset;
    [SerializeField] Vector3 startingRotationOffset;
    [SerializeField] ProjectionAxis projectionAxis;

    private void OnEnable()
    {
        Instantiator.OnCubeCreatedTriggered += ChangeTileShadow;
    }


    private void Start()
    {
        CreateWall();
        transform.SetPositionAndRotation(startingPositionOffset, Quaternion.Euler(startingRotationOffset));
    }


    void CreateWall()
    {
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                Tile instantiatedTile = Instantiate(TilePrefab, new Vector3(i, 0, j), Quaternion.identity);
                instantiatedTile.transform.parent = transform;
                instantiatedTile.name = "Tile" + i + "_" + j;
                grid.Add(new Tuple<int, int>(i, j), instantiatedTile);
            }

        }
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
            tile.ChangeColor();
        }


    }

}

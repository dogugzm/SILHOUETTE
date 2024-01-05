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
    Dictionary<Tuple<int, int>, Tile>  grid = new();
    [SerializeField] Vector3 startingPositionOffset;
    [SerializeField] Vector3 startingRotationOffset;
    [SerializeField] ProjectionAxis projectionAxis;


    private void Start()
    {
        CreateWall();
        transform.SetPositionAndRotation(startingPositionOffset, Quaternion.Euler(startingRotationOffset));
    }


    void CreateWall()
    {
        for (int i = 0; i < size; i++)
        {
            for(int j = 0; j < size; j++)
            {
                Tile instantiatedTile = Instantiate(TilePrefab, new Vector3(i, 0, j), Quaternion.identity);
                instantiatedTile.transform.parent = transform;
                instantiatedTile.name = "Tile" + i + "_" + j;
                grid.Add(new Tuple<int, int>(i, j), instantiatedTile);
            }

        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (grid.ContainsKey(new Tuple<int, int>(0, 0)))
            {
                if (projectionAxis is ProjectionAxis.XBased)
                {
                    ChangeTileShadow(grid[new Tuple<int, int>(5,5)]);

                }

            }
            else
            {
                Debug.Log("No Such Tile");
            }
        }
    }

    void ChangeTileShadow(Tile tile)
    {
        tile.ChangeColor();
    }

}

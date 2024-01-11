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

    private Tuple<int, int> GenerateUniqueRandomPosition()
    {
        Tuple<int, int> randomPos = new(UnityEngine.Random.Range(0, size), UnityEngine.Random.Range(0, size));

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
                Tile instantiatedTile = Instantiate(TilePrefab,transform);
                instantiatedTile.transform.localPosition = new Vector3(i, 0, j);
                instantiatedTile.name = "Tile" + i + "_" + j;
                grid.Add(new Tuple<int, int>(i, j), instantiatedTile);

                // Wait for the next frame
                await UniTask.DelayFrame(5);
            }
        }

    }

    void CreatePlane(int width, int length)
    {
        // Create a new GameObject to hold the plane
        GameObject planeObject = new GameObject("RuntimePlane");
        planeObject.transform.parent = transform;


        // Add a MeshFilter component
        MeshFilter meshFilter = planeObject.AddComponent<MeshFilter>();

        // Create a new mesh
        Mesh mesh = new();

        // Define vertices for the plane
        Vector3[] vertices = new Vector3[4];
        vertices[0] = new Vector3(0f, 0f, 0f);
        vertices[1] = new Vector3(-width, 0f, 0f);
        vertices[2] = new Vector3(-width, 0f, length);
        vertices[3] = new Vector3(0f, 0f, length);

        // Define triangles
        int[] triangles = { 0, 1, 2, 0, 2, 3 };

        // Set the mesh data
        mesh.vertices = vertices;
        mesh.triangles = triangles;

        // Automatically calculate normals
        //mesh.RecalculateNormals();

        // Assign the mesh to the MeshFilter component
        meshFilter.mesh = mesh;

        // Add a MeshRenderer component
        MeshRenderer meshRenderer = planeObject.AddComponent<MeshRenderer>();

        // Optional: Set material for the plane
        meshRenderer.material = TilePrefab.GetComponentInChildren<MeshRenderer>().sharedMaterial;

        transform.position = new Vector3(length, 0, width);


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
            shadowGrid.Add(gridPos);
        }
    }

    void ChangeTileShadow(Tuple<int, int> gridPos)
    {

        if (grid.TryGetValue(gridPos, out Tile tile))
        {
            tile.ChangeColor();
            shadowGrid.Add(gridPos);

        }
    }

}




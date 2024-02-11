using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class CubeInstantiator : MonoBehaviour
{
    [SerializeField] GameObject CubePrefab;
    private GameObject clickedCube;

    public static Action<Vector3> OnCubeCreatedTriggered;
    public static Action<Vector3, bool, bool> OnCubeDeletedTriggered;

    List<Vector3> InstantiatedCubePositions = new();
    List<GameObject> InstantiatedCubes = new();

    public float destroyTimer = 0;
    [SerializeField] private float destroyMaxTimer;
    private bool scalingStarted = false;
    bool canInstantiate = false;

    public bool duplicateOnX { get; private set; }
    public bool duplicateOnY { get; private set; }

    [SerializeField] private Camera mainCam;

    private void OnEnable()
    {
        LevelGenerator.LevelFinished += OnLevelFinished;
    }

    private void OnDisable()
    {
        LevelGenerator.LevelFinished -= OnLevelFinished;
    }

    private void OnLevelFinished()
    {
        foreach (var item in InstantiatedCubes)
        {
            Destroy(item);
        }

        InstantiatedCubePositions.Clear();
        InstantiatedCubes.Clear();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                clickedCube = hit.transform.gameObject;
                canInstantiate = true;
            }
        }

        if (Input.GetMouseButton(0))
        {
            destroyTimer += Time.deltaTime;
            if (!scalingStarted)
            {
                scalingStarted = true;
                if (clickedCube.TryGetComponent(out Cube cube))
                {
                    cube.ChangeColor(cube.deleteColor, destroyMaxTimer);
                }
            }
            if (destroyTimer >= destroyMaxTimer)
            {
                if (clickedCube != null && clickedCube.TryGetComponent(out Cube cube))
                {
                    DeleteCube(clickedCube);
                    clickedCube = null;
                }
                destroyTimer = 0;
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (clickedCube is null)
            {
                return;
            }
            if (destroyTimer > 0.3f && destroyTimer < destroyMaxTimer)
            {
                canInstantiate = false;
                destroyTimer = 0;
            }
            if (scalingStarted)
            {
                scalingStarted = false;
                if (clickedCube.TryGetComponent(out Cube cube))
                {
                    cube.ChangeColor(cube.defaultColor, destroyMaxTimer);
                }
            }
            if (!canInstantiate)
            {
                return;
            }
            Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.transform.gameObject != clickedCube)
                {
                    return;
                }
                Vector3 intersectionNormal = hit.normal;
                Vector3 intersectionPoint = hit.transform.position;
                if (hit.transform.TryGetComponent(out Cube cube))
                {
                    CreateCube(intersectionPoint + intersectionNormal);
                }
                else
                {
                    CreateCube(intersectionPoint);
                }
            }

        }

    }

    void CreateCube(Vector3 position)
    {
        if (InstantiatedCubePositions.Contains(position))
        {
            Debug.Log("Not On Same Position");
            return;
        }
        GameObject instantiatedGO = Instantiate(CubePrefab, position, Quaternion.identity);
        InstantiatedCubes.Add(instantiatedGO);
        duplicateOnX = InstantiatedCubePositions.Any(pos => pos.x == position.x);
        duplicateOnY = InstantiatedCubePositions.Any(pos => pos.y == position.y);
        InstantiatedCubePositions.Add(position);
        Debug.Log("Instantiated with pos: " + position);
        OnCubeCreatedTriggered.Invoke(instantiatedGO.transform.position);
    }

    void DeleteCube(GameObject cube)
    {
        Destroy(cube);
        InstantiatedCubes.Remove(cube);
        InstantiatedCubePositions.Remove(cube.transform.position);
        duplicateOnX = InstantiatedCubePositions.Any(pos => pos.x == cube.transform.position.x);
        duplicateOnY = InstantiatedCubePositions.Any(pos => pos.y == cube.transform.position.y);

        OnCubeDeletedTriggered.Invoke(cube.transform.position, duplicateOnX, duplicateOnY);
    }

}

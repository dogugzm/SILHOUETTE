using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;


public class Instantiator : MonoBehaviour
{
    [SerializeField] GameObject CubePrefab;
    private GameObject clickedCube;

    public static Action<Vector3> OnCubeCreatedTriggered;
    public static Action<Vector3> OnCubeDeletedTriggered;

    List<Vector3> InstantiatedCubePositions = new();
    List<GameObject> InstantiatedCubes = new();

    public float destroyTimer = 0;
    [SerializeField] private float destroyMaxTimer;
    private bool scalingStarted = false;
    bool canInstantiate = false;

    private void OnEnable()
    {
        LevelGenerator.LevelFinished += OnLevelFinished;
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

    private void OnDisable()
    {
        LevelGenerator.LevelFinished -= OnLevelFinished;

    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

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
                    cube.ChangeColor(Color.red, destroyMaxTimer);
                }
            }
            if (destroyTimer >= destroyMaxTimer)
            {
                if (clickedCube != null)
                {
                    DeleteCube(clickedCube);
                    clickedCube = null;
                }
                destroyTimer = 0;
            }
           
        }

        if (Input.GetMouseButtonUp(0))
        {
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
                    cube.ChangeColor(Color.white, destroyMaxTimer);
                }
            }
            if (!canInstantiate)
            {
                return;
            }
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.transform.gameObject != clickedCube)
                {
                    return;
                }
                Vector3 intersectionNormal = hit.normal;
                Vector3 intersectionPoint = hit.transform.position;
                if (hit.transform.TryGetComponent<Cube>(out Cube cube))
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
        InstantiatedCubePositions.Add(position);
        Debug.Log("Instantiated with pos: " + position);
        OnCubeCreatedTriggered.Invoke(instantiatedGO.transform.position);
    }

    void DeleteCube(GameObject cube)
    {
        Destroy(cube);
        InstantiatedCubes.Remove(cube);
        InstantiatedCubePositions.Remove(cube.transform.position);
        OnCubeDeletedTriggered.Invoke(cube.transform.position);
    }

}

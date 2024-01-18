using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Instantiator : MonoBehaviour
{
    [SerializeField] GameObject CubePrefab;

    public static Action<Vector3> OnCubeCreatedTriggered;

    List<Vector3> InstantiatedCubePositions = new();
    List<GameObject> InstantiatedCubes = new();


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
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Vector3 intersectionNormal = hit.normal;
                Vector3 intersectionPoint = hit.transform.position;

                CreateCube(intersectionPoint + intersectionNormal);
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
        Debug.Log("Instantiated with pos: " +  position);
        OnCubeCreatedTriggered.Invoke(instantiatedGO.transform.position);
    }

}

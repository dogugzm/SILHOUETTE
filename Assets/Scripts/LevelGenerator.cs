using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{

    public static Action LevelFinished;
    [SerializeField] int size;

    [SerializeField] WallGrid wallX;
    [SerializeField] WallGrid wallZ;
    [Range(0, 100)]

    public GameObject groundGO;
    public GameObject ground1;
    public GameObject ground2;

    private int maxSize = 25;
    private int percantageOfmultipleShadow = 100;
    bool firstLevel = true;
    [SerializeField] private Ease ease;
    [SerializeField] private Ease ease2;


    private void OnEnable()
    {
        wallX.WallCompleted += CheckIfGameFinished;
        wallZ.WallCompleted += CheckIfGameFinished;
    }

    private void OnDisable()
    {
        wallX.WallCompleted -= CheckIfGameFinished;
        wallZ.WallCompleted -= CheckIfGameFinished;
    }

    private void Start()
    {
        GenerateLevel();
    }

    async void GenerateLevel()
    {
        StartFancyMovement();

        if (firstLevel)
        {
            wallZ.CreateWallAsync();

            await wallX.CreateWallAsync();
        }

        if (percantageOfmultipleShadow == 100)
        {
            size++;
        }
        if (size >= 4)
        {
            percantageOfmultipleShadow -= 25;
        }

        for (int i = 0; i < size; i++)
        {
            await wallX.SetShadowTile(wallX.GetTupleFromNears());
            await UniTask.DelayFrame(1);
        }

        wallZ.SetShadowFromOtherWall(wallX.shadowTuples, percantageOfmultipleShadow);
        if (percantageOfmultipleShadow <= 25)
        {
            percantageOfmultipleShadow = 100;
        }
        firstLevel = false;
    }

    private void StartFancyMovement()
    {
        groundGO.transform.DOMove(new Vector3(4, 0, 4), 2f).SetEase(ease);
        ground1.transform.DOMoveY(-7f, 1f).SetEase(ease2);

        ground2.transform.DOMoveY(-7f, 1f).SetEase(ease2);
    }

    async void CheckIfGameFinished()
    {
        if (wallX.isCompleted && wallZ.isCompleted)
        {
            Debug.Log("LEVEL FINISHED");
            await UniTask.Delay(1000);
            LevelFinished?.Invoke();
            await UniTask.Delay(500);
            GenerateLevel();
        }
    }



}

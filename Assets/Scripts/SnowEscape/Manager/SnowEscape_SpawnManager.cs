using System;
using System.Collections.Generic;
using UnityEngine;

public class SnowEscape_SpawnManager : MonoBehaviour
{
    [Header("소환 오브젝트 참조")]
    [SerializeField] Transform snowballSpawnPoint;
    [SerializeField] GameObject snowball;
    [SerializeField] Transform[] treeSpawnPoints;
    [SerializeField] GameObject tree;

    [Header("소환 관련 변수")]
    private float currentIntervalTime = 0f;
    [SerializeField] float spawnIntervalTime = 10f;

    [SerializeField] List<Transform> availableSpawnPoints = new List<Transform>();

    void Awake()
    {
        InitialTreeSpawnPoints();
    }

    void Update()
    {
        currentIntervalTime += Time.deltaTime;
        if (currentIntervalTime >= spawnIntervalTime)
        {
            // 시간 초기화 
            currentIntervalTime = 0f;

            // 나무 소환 
            // SpawnTree();
        }
    }

    public void SpawnSnowball()
    {
        if (snowballSpawnPoint != null && snowball != null)
        {
            Instantiate(snowball, snowballSpawnPoint.position, snowballSpawnPoint.rotation);
        }
    }

    #region 나무 
    private void InitialTreeSpawnPoints()
    {
        availableSpawnPoints.Clear();
        foreach (Transform spawnPoint in treeSpawnPoints)
        {
            availableSpawnPoints.Add(spawnPoint);
        }
    }

    // public void SpawnTree()
    // {
    //     // 생성할 오브젝트 개수 
    //     int spawnIndex = Random.Range(1, 3);

    //     if(spawnIndex > treeSpawnPoints.Length)
    //     {
    //         spawnIndex = treeSpawnPoints.Length;
    //     }

    //     List<Transform> currentAvailbleSpawnPoints = new List<Transform>();
    //     foreach(Transform spawnPoint in treeSpawnPoints)
    //     {
    //         // bool isOccupied = Physics.CheckBox(spawnPoint.position, OverlapBoxCommand)
    //     }

    // }
    #endregion

}

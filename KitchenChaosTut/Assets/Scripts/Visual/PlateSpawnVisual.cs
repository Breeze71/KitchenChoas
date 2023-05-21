using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateSpawnVisual : MonoBehaviour
{
    [SerializeField] private PlateCounter platesCounter;
    [SerializeField] private Transform counterTopPoint;
    [SerializeField] private Transform plateVisualPrefab;

    private List<GameObject> plateVisualGameObjList;

    private void Awake() 
    {
        plateVisualGameObjList = new List<GameObject>();    
    }
    private void Start() 
    {
        platesCounter.OnPlateSpawned += PlateCounter_OnPlateSpawned;
        platesCounter.OnplateRemoved += PlateCounter_OnPlateRemoved;
    }

    private void PlateCounter_OnPlateSpawned(object sender, System.EventArgs e)
    {
        Transform plateVisualTransform = Instantiate(plateVisualPrefab, counterTopPoint);

        // 每次生成 visual ，由於每個桌上只能一個 obj
        float plateOffestY = 0.1f;
        plateVisualTransform.localPosition = new Vector3(0, plateOffestY * plateVisualGameObjList.Count, 0);
        plateVisualGameObjList.Add(plateVisualTransform.gameObject);
    }

    // 拿走最上層 plate
    private void PlateCounter_OnPlateRemoved(object sender, EventArgs e)
    {
        GameObject pickedUpPlate = plateVisualGameObjList[plateVisualGameObjList.Count - 1];
        plateVisualGameObjList.Remove(pickedUpPlate);

        Destroy(pickedUpPlate);
    }
}

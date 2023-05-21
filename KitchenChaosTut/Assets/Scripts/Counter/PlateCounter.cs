using UnityEngine;
using System;

public class PlateCounter : BaseCounter
{
    public event EventHandler OnPlateSpawned;
    public event EventHandler OnplateRemoved;

    [SerializeField] private KitchenObjSO plateKitchenObjSO;

    private float spawnPlateTimer;
    private float spawnPlateTimerMax = 4f;
    private int platesSpawnAmount;
    private int platesSpawnAmountMax = 4;

    private void Update() 
    {
        spawnPlateTimer += Time.deltaTime;
        if(spawnPlateTimer > spawnPlateTimerMax)
        {
            spawnPlateTimer = 0f;

            if(platesSpawnAmount < platesSpawnAmountMax)
            {
                platesSpawnAmount++;

                OnPlateSpawned?.Invoke(this, EventArgs.Empty);
            }
        }

    }

    public override void Interact(PlayerMovement player)
    {
        if(!player.HasKitchenObj())
        {   
            // take the plate
            if(platesSpawnAmount > 0)
            {
                platesSpawnAmount--;

                KitchenObj.SpawnKitchObj(plateKitchenObjSO, player);
                OnplateRemoved?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}

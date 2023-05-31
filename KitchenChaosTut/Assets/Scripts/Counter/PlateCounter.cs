using UnityEngine;
using System;
using Unity.Netcode;

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
        if(!IsServer)
        {
            return;
        }

        spawnPlateTimer += Time.deltaTime;
        if(spawnPlateTimer > spawnPlateTimerMax)
        {
            spawnPlateTimer = 0f;

            if(GameManager.Instance.IsGamePlaying() && platesSpawnAmount < platesSpawnAmountMax)
            {
                spawnPlateServerRpc();
            }
        }

    }

    /* Netcode */
    [ServerRpc]
    private void spawnPlateServerRpc()
    {
        // 由 Server Broadcast。也只需 Server 計算盤子生成
        spawnPlateClientRpc();
    }
    [ClientRpc]
    private void spawnPlateClientRpc()
    {
        platesSpawnAmount++;
        OnPlateSpawned?.Invoke(this, EventArgs.Empty); // Spawned Visual
    }

    public override void Interact(PlayerMovement player)
    {
        if(!player.HasKitchenObj())
        {   
            // take the plate
            if(platesSpawnAmount > 0)
            {
                KitchenObj.SpawnKitchObj(plateKitchenObjSO, player);    // already sync

                InteractLogicServerRpc();
            }
        }
    }

    /* Netcode */
    [ServerRpc(RequireOwnership = false)]
    private void InteractLogicServerRpc()
    {
        InteractLogicClientRpc();
    }
    [ClientRpc]
    private void InteractLogicClientRpc()
    {
        platesSpawnAmount--;

        OnplateRemoved?.Invoke(this, EventArgs.Empty);  // Removed Visual
    }

}

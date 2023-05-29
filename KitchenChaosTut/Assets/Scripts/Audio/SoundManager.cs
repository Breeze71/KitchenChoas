using System;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private const string PlayerRefsSoundEfxVolume = "SoundEffectsVolume";

    public static SoundManager Instance
    {
        get;
        private set;
    }
    
    [SerializeField] private AudioClipSO audioClipSO;


    private float volume = .3f;

    private void Awake() 
    {
        Instance = this;

        volume = PlayerPrefs.GetFloat(PlayerRefsSoundEfxVolume, .3f);
    }
    private void Start() 
    {
        DeliveryManager.Instance.OnRecipeSuccess += DeliveryManager_OnRecipeSuccessSound;
        DeliveryManager.Instance.OnRecipeFailed += DeliveryManager_OnRecipeFailedSound;

        CuttingCounter.OnAnyCutSound += CuttingCounter_OnAnyCutSound;    

        //PlayerMovement.Instance.OnPickupSound += Player_OnPickupSound;
        
        BaseCounter.OnAnyObjDropSound += BaseCounter_OnAnyObjDropSound;

        TrashCounter.OnAnyObjTrashedSound += TrashCounter_OnAnyObjTrashedSound;
    }

    private void TrashCounter_OnAnyObjTrashedSound(object sender, EventArgs e)
    {
        TrashCounter trashCounter = sender as TrashCounter;
        PlaySound(audioClipSO.trash, trashCounter.transform.position);        
    }

    private void BaseCounter_OnAnyObjDropSound(object sender, EventArgs e)
    {
        BaseCounter baseCounter = sender as BaseCounter;
        PlaySound(audioClipSO.objectDrop, baseCounter.transform.position);
    }

    private void Player_OnPickupSound(object sender, EventArgs e)
    {
        //PlaySound(audioClipSO.objectPickup, PlayerMovement.Instance.transform.position);
    }

    private void CuttingCounter_OnAnyCutSound(object sender, EventArgs e)
    {
        // 由於有多個 cutting counter / sender as CuttingCounter
        CuttingCounter cuttingCounter = sender as CuttingCounter;
        PlaySound(audioClipSO.chop, cuttingCounter.transform.position);
    }

    private void DeliveryManager_OnRecipeFailedSound(object sender, EventArgs e)
    {
        DeliveryCounter deliveryCounter = DeliveryCounter.Instance;

        PlaySound(audioClipSO.deliveryFailed, deliveryCounter.transform.position);
    }
    
    private void DeliveryManager_OnRecipeSuccessSound(object sender, EventArgs e)
    {
        DeliveryCounter deliveryCounter = DeliveryCounter.Instance;

        PlaySound(audioClipSO.deliverySuccess, deliveryCounter.transform.position);
    }

    // play sound
    private void PlaySound(AudioClip[] audioClipArray, Vector3 position, float volumeMultiplier = 1f)
    {
        PlaySound(audioClipArray[UnityEngine.Random.Range(0, audioClipArray.Length)], position, volume * volumeMultiplier);
    }
    private void PlaySound(AudioClip audioClip, Vector3 position, float volumeMultiplier = 1f)
    {
        AudioSource.PlayClipAtPoint(audioClip, position, volume * volumeMultiplier);
    }

    public void PlayFootstepSound(Vector3 position, float volume)
    {
        PlaySound(audioClipSO.footstep, position, volume);
    }
    public void PlayCountDownSound()
    {
        PlaySound(audioClipSO.warning, Vector3.zero);
    }
    public void PlayWarningSound(Vector3 position)
    {
        PlaySound(audioClipSO.warning, position);
    }

    public void ChangeVolume()
    {
        volume +=.1f;

        if(volume > 1f)
            volume = 0f;

        // 儲存音效大小
        PlayerPrefs.SetFloat(PlayerRefsSoundEfxVolume, volume);
        PlayerPrefs.Save();
    }
    public float GetVolume()
    {
        return volume;
    }
}

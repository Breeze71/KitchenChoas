using UnityEngine;

public class PlayerSound : MonoBehaviour
{
    private PlayerMovement player;
    private float footstepTimer;
    private float footstepTimerMax = 0.1f;

    private void Awake() {
        player = GetComponent<PlayerMovement>();
    }

    private void Update() {
        footstepTimer -= Time.deltaTime;

        if(footstepTimer < 0f)
        {
            footstepTimer = footstepTimerMax;

            if(player.IsWalking())
            {
                float volume = 1f;
                SoundManager.Instance.PlayFootstepSound(player.transform.position, volume);
            }
        }
    }
}

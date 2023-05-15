using UnityEngine;

public class BaseCounter : MonoBehaviour
{
    // Containter , ClearCounter
    // Override Interact
    public virtual void Interact(PlayerMovement player)
    {
        Debug.Log("BaseCounter.Interact");
    }
}

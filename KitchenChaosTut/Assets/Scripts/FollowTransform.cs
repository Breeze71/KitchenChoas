using UnityEngine;

// 由於 NetworkObj 不能有父物件，所以不能直接 SetParent
public class FollowTransform : MonoBehaviour
{
    private Transform targetTransform;

    // Set the target to follow
    public void SetTargetTransform(Transform targetTransform)
    {
        this.targetTransform = targetTransform;
    }

    private void LateUpdate() 
    {
        if(targetTransform == null)
        {
            return;
        }

        transform.position = targetTransform.position;
        transform.rotation = targetTransform.rotation;
    }
}

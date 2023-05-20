using UnityEngine;

public class ContainerCounterVisual : MonoBehaviour
{
    private const string openClose = "OpenClose";
    [SerializeField] private ContainerCounter containerCounter;

    private Animator anim;

    private void Awake() {
        anim = GetComponent<Animator>();
    }

    private void Start() 
    {
        containerCounter.OnPlayerGrabbedObj += ContainCounter_OnPlayerGrabbedObj;
    }

    private void ContainCounter_OnPlayerGrabbedObj(object sender, System.EventArgs e)
    {
        anim.SetTrigger(openClose);
    }
}

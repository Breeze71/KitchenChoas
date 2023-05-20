using UnityEngine;

public class CuttingCounterVisual : MonoBehaviour
{
    private const string cut = "Cut";
    [SerializeField] private CuttingCounter cuttingCounter;

    private Animator anim;

    private void Awake() {
        anim = GetComponent<Animator>();
    }

    private void Start() 
    {
        cuttingCounter.OnCut += CuttingCounter_OnCut;
    }

    private void CuttingCounter_OnCut(object sender, System.EventArgs e)
    {
        anim.SetTrigger(cut);
    }
}

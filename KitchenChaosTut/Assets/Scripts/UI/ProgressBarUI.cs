using UnityEngine;
using UnityEngine.UI;

public class ProgressBarUI : MonoBehaviour
{
    [SerializeField] private GameObject hasProgressGameObj;
    [SerializeField] private Image barImg;

    private IHasProgress hasProgress;

    private void Start() 
    {
        hasProgress = hasProgressGameObj.GetComponent<IHasProgress>();
        if(hasProgress == null)
        {
            Debug.LogError("does not have ihasProgress");
        }

        // 只需開局訂閱
        hasProgress.OnProgressBarChanged += HasProgress_OnProgressBarChanged;

        barImg.fillAmount = 0f;

        Hide();
    }

    private void HasProgress_OnProgressBarChanged(object sender, IHasProgress.OnProgressBarChangeEventArgs e)
    {
        barImg.fillAmount = e.progressBarNormalized;

        if(e.progressBarNormalized == 0f || e.progressBarNormalized == 1f)
            Hide();
        else
            Show();
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);        
    }
}

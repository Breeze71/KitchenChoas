using UnityEngine;
using UnityEngine.UI;

public class LoadingProgressBar : MonoBehaviour
{
    [SerializeField] private Image bar;
    private float startValue = 0;
    private float loadingValue;

    // Update is called once per frame
    void Update()
    {
        loadingValue = Loader.GetLoadingProgress();

        bar.fillAmount = Mathf.Lerp(startValue, loadingValue, 0.1f);

        startValue = loadingValue;

    }
}

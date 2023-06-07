using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class TestingNetcodeUI : MonoBehaviour
{
    [SerializeField] private Button startHostBtn;
    [SerializeField] private Button startClientBtn;

    private void Awake() 
    {
        startHostBtn.onClick.AddListener(() =>
        {
            Debug.Log("host");
            KitchenGameMultiplayer.Instance.StartHost();
            Hide();
        });

        startClientBtn.onClick.AddListener(() =>
        {
            Debug.Log("Client");
            KitchenGameMultiplayer.Instance.StartClient();
            Hide();
        });        
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}

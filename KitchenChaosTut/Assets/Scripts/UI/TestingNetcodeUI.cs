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
            NetworkManager.Singleton.StartHost();
            Hide();
        });

        startClientBtn.onClick.AddListener(() =>
        {
            Debug.Log("Client");
            NetworkManager.Singleton.StartClient();
            Hide();
        });        
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}

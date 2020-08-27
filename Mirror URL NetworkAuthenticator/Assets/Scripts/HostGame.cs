using UnityEngine;
using UnityEngine.UI;

public class HostGame : MonoBehaviour
{
    Button HostButton;

    void Start()
    {
        HostButton = GetComponent<Button>();
        HostButton.onClick.AddListener(Host);
    }

    void Host()
	{
        Connector.Instance.HostGame();
	}        
}

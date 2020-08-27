using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class Connector : MonoBehaviour
{
    //Remember - this script has to be attached to same GameObject with NetworkManager!
    public static Connector Instance;

    //Attack in Inspector GameObject with Authenticator. Remember to do this in NetworkManager too!
    public GameObject AuthenticatorGameObject;

    //Attach Text in your scene to see Authenticator results.
    public Text ResultText;

    NetworkManager NetManager;
    Authenticator Authenticator;

	private void Awake()
	{
        Instance = this;
	}

	private void Start()
    {
        NetManager = GetComponent<NetworkManager>();
        Authenticator = AuthenticatorGameObject.GetComponent<Authenticator>();
    }

    /*
     * You can create Host and Server connections on your own (Server does 
     * not require Authenticator), with using template below:
     */
    public void ConnectAsClient(string AccountName, string Password)
	{
        //THIS IS REQUIRED - SET AUTHENTICATOR'S VALUE HERE.
        Authenticator.AccountName = AccountName;
        Authenticator.Password = Password;
        Authenticator.Manager = this.NetManager;

        NetManager.StartClient();
	}

    public void HostGame()
	{
        NetManager.StartHost();
	}

    public void DisplayResults(string MessageToDisplay)
	{
        ResultText.text = MessageToDisplay;
	}
}

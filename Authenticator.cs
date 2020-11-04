using System.Collections;
using UnityEngine;
using Mirror;
using UnityEngine.Networking;
using System;

public class Authenticator : NetworkAuthenticator
{
	//Your API URL
	private const string URL = "https://example.com/login.php";

	//Input data for your auth system
	public class AuthRequestMessage : NetworkMessage
	{
		public string AccountName;
		public string Password;
	}

	//Output data from server
	public class AuthResponseMessage : NetworkMessage
	{
		public int AccountId;
		public bool Result;
		public string Message;
	}

	public NetworkManager Manager;
	public string AccountName;
	public string Password;

	public override void OnStartServer()
	{
		base.OnStartServer();
		NetworkServer.RegisterHandler<AuthRequestMessage>(OnAuthRequestMessage, false);
	}

	public override void OnStartClient()
	{
		base.OnStartClient();
		NetworkClient.RegisterHandler<AuthResponseMessage>(OnAuthResponseMessage, false);
	}

	/*
	 * When client wants to authenticate, following logic is being made.
	 * It creates Request Message with data from this class (data
	 * received from Input Fields).
	 */
	public override void OnClientAuthenticate(NetworkConnection Connection)
	{
		AuthRequestMessage Request = new AuthRequestMessage
		{
			AccountName = this.AccountName,
			Password = this.Password,
		};
		Connection.Send(Request);
	}

	public override void OnServerAuthenticate(NetworkConnection Connection)
	{
		//Function useless for this. This is called on SERVER when CLIENT wants to authenticate.
	}

	/*
	 * Wait for request message from client
	 */
	public void OnAuthRequestMessage(NetworkConnection Connection, AuthRequestMessage RequestMessage)
	{
		StartCoroutine(ValidateData(Connection, RequestMessage.AccountName, RequestMessage.Password));
	}

	/*
	 * Wait for response message from ValidateData() courtine
	 */
	public void OnAuthResponseMessage(NetworkConnection Connection, AuthResponseMessage ResponseMessage)
	{
		//You will see your auth results on login scene.
		Connector.Instance.DisplayResults("Account ID: " + ResponseMessage.AccountId + "; is success: " + ResponseMessage.Result + "; Message: " + ResponseMessage.Message);

		if(ResponseMessage.Result)
		{
			//here do success logic, like change scene
		}
		else
		{
			//here do failure logic, like display error

			//REMEMBER TO DISCONNECT CONNECTION & SET AUTHENTICATION AS FAILED IF AUTHENTICATION WILL FAIL
			Connection.isAuthenticated = false;
			StartCoroutine(DelayDisconnect(Connection, 1f));
		}
	}

	IEnumerator DelayDisconnect(NetworkConnection Connection, float Delay)
	{
		yield return new WaitForSeconds(Delay);
		Connection.Disconnect();
	}

	IEnumerator ValidateData(NetworkConnection Connection, string AccountName, string Password)
	{
		/*
		 * Here make request to your web server request,
		 * include data you need. This example uses account name and password,
		 * but you can do email or whatever.
		 */

		WWWForm Form = new WWWForm();
		Form.AddField("accountName", AccountName);
		Form.AddField("password", Password);

		UnityWebRequest Request = UnityWebRequest.Post(URL, Form);

		yield return Request.SendWebRequest();

		//Check for web errors:
		if(Request.isNetworkError || Request.isHttpError)
		{

			//Create response message
			AuthResponseMessage ResponseMessage = new AuthResponseMessage
			{
				AccountId = 0,
				Result = false,
				Message = "There was an error when handling request"
			};

			//Send response message
			Connection.Send(ResponseMessage);
			yield break;
		}

		/*
		 * Example server response: <account ID>:<success>:<message>, like:
		 * 12:true:"Logged in successfully!" or:
		 * 0:false:"Invalid password!"
		 * 
		 * HERE PARSE YOUR SERVER RESPONSE, FOR EXAMPLE JSON DATA.
		 */

		string ServerResponse = Request.downloadHandler.text;
		string[] ServerResponseParsed = ServerResponse.Split(':');

		int AccountId = Convert.ToInt32(ServerResponseParsed[0]);
		bool IsSuccess = Convert.ToBoolean(ServerResponseParsed[1]);
		string Message = ServerResponseParsed[2];

		//Create response message
		AuthResponseMessage FinalResponseMessage = new AuthResponseMessage
		{
			AccountId = AccountId,
			Result = IsSuccess,
			Message = Message
		};

		//Send message
		Connection.Send(FinalResponseMessage);
	}
}

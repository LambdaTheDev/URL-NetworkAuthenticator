using UnityEngine;
using UnityEngine.UI;

public class LoginHandler : MonoBehaviour
{
	public InputField AccountNameField;
	public InputField PasswordField;
	public Button Button;

	private void Start()
	{
		Button.onClick.AddListener(SubmitButton);
	}

	void SubmitButton()
	{
		string AccountName = AccountNameField.text;
		string Password = PasswordField.text;

		Connector.Instance.ConnectAsClient(AccountName, Password);
	}
}

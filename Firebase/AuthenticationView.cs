using UnityEngine;
using UnityEngine.UI;

public class AuthenticationView : MonoBehaviour {

    //----------------------------------------------------------------------------------------------------------------------------------------------//
    // This script calls the sign in, sign out, and register functions on the UserAuthentication script.

    // InputField variables:
    [SerializeField]
    private InputField registerUsername;
    [SerializeField]
    private InputField registerEmail;
    [SerializeField]
    private InputField registerPassword;
    [SerializeField]
    private InputField registerConfirm;
    [SerializeField]
    private InputField registerPin;
    [SerializeField]
    private InputField registerPinConfirm;
    [SerializeField]
    private InputField loginEmail;
    [SerializeField]
    private InputField loginPassword;

    // Script variables:
    private UserAuthentication auth;
    private AuthenticationEvents authEvents;

    //----------------------------------------------------------------------------------------------------------------------------------------------//
    // Assigns the relevant script to auth.

    private void Start () {

        auth = this.gameObject.transform.GetComponent<UserAuthentication>();
        authEvents = this.gameObject.transform.GetComponent<AuthenticationEvents> ();
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//
    // Runs the CreateUser() function on the auth script when a button is pressed. Inputs the register email and password.

    public void CreateUser () {

        authEvents.AlertBoxOn (true);

        if (registerUsername.text.Length >= 6 && registerEmail.text != "" && registerPassword.text.Length >= 6 && registerConfirm.text != "" && registerPassword.text == registerConfirm.text && registerPin.text.Length >= 6 && registerPinConfirm.text != "" && registerPin.text == registerPinConfirm.text) {
            string email = registerEmail.text;
            string password = registerPassword.text;
            auth.CreateUser (email, password);

            authEvents.AlertBoxState ("Success!");
        } else {
            authEvents.AlertBoxState ("Warning!");

            if (registerUsername.text == "") {
                authEvents.Message ("Username field is empty...");
            } else {
                if (registerEmail.text == "") {
                    authEvents.Message ("Email field is empty...");
                } else {
                    if (registerPassword.text == "") {
                        authEvents.Message ("Password field is empty...");
                    } else {
                        if (registerConfirm.text == "") {
                            authEvents.Message ("Confirm password field is empty...");
                        } else {
                            if (registerPassword.text != registerConfirm.text) {
                                authEvents.Message ("Confirm password does not match...");
                            } else {
                                if (registerPin.text == "") {
                                    authEvents.Message ("PIN Number field is empty...");
                                } else {
                                    if (registerPinConfirm.text == "") {
                                        authEvents.Message ("Confirm PIN Number field is empty...");
                                    } else {
                                        if (registerPin.text != registerPinConfirm.text) {
                                            authEvents.Message ("Confirm PIN Number does not match...");
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//
    // Runs the SignInUser() function on the auth script when a button is pressed. Inputs the login email and password.

    public void SignInUser () {

        if (loginEmail.text != "" && loginPassword.text != "") {
            string email = loginEmail.text;
            string password = loginPassword.text;
            auth.SignInUser (email, password);
        }
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//
    // Runs the SignOutUser() function on the auth script when a button is pressed.

    public void SignOutUser () {

        auth.SignOutUser();
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//
}
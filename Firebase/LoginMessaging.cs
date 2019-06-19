using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class LoginMessaging : MonoBehaviour {

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    [SerializeField]
    private UserAuthentication auth;

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
    
    [SerializeField]
    private GameObject errorBox;
    [SerializeField]
    private Text headerText;
    [SerializeField]
    private Text messageText;

    [SerializeField]
    private GameObject alertBox;
    [SerializeField]
    private Text alertText;

    [SerializeField]
    private Color validColor;
    [SerializeField]
    private Color invalidColor;

    [SerializeField]
    private Button signUpButton;
    [SerializeField]
    private Button logInButton;

    //----------------------------------------------------------------------------------------------------------------------------------------------////----------------------------------------------------------------------------------------------------------------------------------------------//

    public void Update () {

        #region // Set Alert

        if (registerUsername.text.Length >= 6 && registerEmail.text.Length >= 6 && registerPassword.text.Length >= 6 && registerConfirm.text.Length >= 6 && registerPassword.text == registerConfirm.text && registerPin.text.Length == 4 && registerPinConfirm.text.Length == 4 && registerPin.text == registerPinConfirm.text) {
            ToggleAlertBox (false, "");
            signUpButton.interactable = true;
        } else {
            signUpButton.interactable = false;
            if (registerUsername.text.Length < 6) {
                if (registerUsername.text.Length == 0) {
                    ToggleAlertBox (true, "Please enter a username.");
                } else {
                    ToggleAlertBox (true, "The username you have entered is too short, it must be at least 6 characters long.");
                }
            } else {
                if (!registerEmail.text.Contains ("@") || !registerEmail.text.Contains (".")) {
                    if (registerEmail.text.Length == 0) {
                        ToggleAlertBox (true, "Please enter an email.");
                    } else {
                        ToggleAlertBox (true, "The email you have entered is invalid, please make sure you use a valid email format.");
                    }
                } else {
                    if (registerPassword.text.Length < 6) {
                        if (registerPassword.text.Length == 0) {
                            ToggleAlertBox (true, "Please enter a password.");
                        } else {
                            ToggleAlertBox (true, "The password you have entered is too short, it must be at least 6 characters long.");
                        }
                    } else {
                        if (registerConfirm.text.Length == 0) {
                            ToggleAlertBox (true, "Please confirm your password. These two fields need to match.");
                        } else {
                            if (registerPassword.text != registerConfirm.text) {
                                ToggleAlertBox (true, "The Confirmation Password does not match the first Pasword.");
                            } else {
                                if (registerPin.text.Length < 4) {
                                    if (registerPin.text.Length == 0) {
                                        ToggleAlertBox (true, "Please enter a 4-digit PIN number.");
                                    } else {
                                        ToggleAlertBox (true, "The PIN number you have entered is too short, it must be 4 digits long.");
                                    }
                                } else {
                                    if (registerPinConfirm.text.Length < 4) {
                                        if (registerPinConfirm.text.Length == 0) {
                                            ToggleAlertBox (true, "Please confirm your PIN number.");
                                        } else {
                                            ToggleAlertBox (true, "The Confirmation PIN number does not match the first PIN number.");
                                        }
                                    } else {
                                        if (registerPin.text != registerPinConfirm.text) {
                                            ToggleAlertBox (true, "The Confirmation PIN number does not match the first PIN number.");
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        if (loginEmail.text != "" && loginPassword.text != "") {
            logInButton.interactable = true;
        } else {
            logInButton.interactable = false;
        }

        #endregion

        #region // Check Fields

        // Login Fields

        if (loginEmail.text == "") {
            InputFieldAlert (loginEmail, invalidColor);
        } else {
            InputFieldAlert (loginEmail, validColor);
        }

        if (loginPassword.text == "") {
            InputFieldAlert (loginPassword, invalidColor);
        } else {
            InputFieldAlert (loginPassword, validColor);
        }

        // Register Fields

        if (registerUsername.text.Length < 6) {
            InputFieldAlert (registerUsername, invalidColor);
        } else {
            InputFieldAlert (registerUsername, validColor);
        }

        if (!registerEmail.text.Contains ("@") || !registerEmail.text.Contains (".")) {
            InputFieldAlert (registerEmail, invalidColor);
        } else {
            InputFieldAlert (registerEmail, validColor);
        }

        if (registerPassword.text.Length < 6) {
            InputFieldAlert (registerPassword, invalidColor);
        } else {
            InputFieldAlert (registerPassword, validColor);
        }

        if (registerConfirm.text.Length == 0 || registerPassword.text != registerConfirm.text) {
            InputFieldAlert (registerConfirm, invalidColor);
        } else {
            InputFieldAlert (registerConfirm, validColor);
        }

        if (registerPin.text.Length < 4) {
            InputFieldAlert (registerPin, invalidColor);
        } else {
            InputFieldAlert (registerPin, validColor);
        }

        if (registerPinConfirm.text.Length == 0 || registerPin.text != registerPinConfirm.text) {
            InputFieldAlert (registerPinConfirm, invalidColor);
        } else {
            InputFieldAlert (registerPinConfirm, validColor);
        }

        #endregion
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void ToggleErrorScreen (bool val) {

        errorBox.SetActive (val);
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void SetMessage (string header, string message, bool val) {

        headerText.text = header;
        messageText.text = message;
        ToggleErrorScreen (val);
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void ToggleAlertBox (bool val, string message) {

        alertBox.SetActive (val);
        alertText.text = message;
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void CheckLogin () {

        if (loginEmail.text != "" && loginPassword.text != "") {
            string email = loginEmail.text;
            string password = loginPassword.text;
            auth.SignInUser (email, password);
        }
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void CheckRegister () {

        if (registerUsername.text.Length >= 6 && registerEmail.text.Length >= 6 && registerPassword.text.Length >= 6 && registerConfirm.text.Length >= 6 && registerPassword.text == registerConfirm.text && registerPin.text.Length == 4 && registerPinConfirm.text.Length == 4 && registerPin.text == registerPinConfirm.text) {
            string email = registerEmail.text;
            string password = registerPassword.text;
            auth.CreateUser (email, password);
        }
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    private void InputFieldAlert (InputField field, Color color) {

        field.transform.GetComponent<ColorFade> ().SetAlertColor (color);
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//
}
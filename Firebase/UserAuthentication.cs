using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using Firebase;
using Firebase.Unity.Editor;
using Firebase.Auth;
using Firebase.Database;

public class UserAuthentication : MonoBehaviour {

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    [SerializeField]
    private CirclesManager circlesManager;
    [SerializeField]
    private DebugConsole debug;
    [SerializeField]
    private LoginMessaging messaging;
    [SerializeField]
    private LoginRegisterUI sceneManager;
    [SerializeField]
    private Image loadingScreen;
    public bool loading = true;
    private float speed = 5.0f;

    [SerializeField]
    private InputField newPassword;
    private string currentPassword;
    [SerializeField]
    private InputField newUsername;
    [SerializeField]
    private InputField newPin;

    [SerializeField]
    private Button confirmButton;
    [SerializeField]
    private Button deleteButton;
    [SerializeField]
    private InputField confirmInput;
    [SerializeField]
    private InputField deleteInput;

    private bool newUsernameValid = true;
    private bool newPasswordValid = true;
    private bool newPinValid = true;

    //---------------------------------------------------------------//
    public FirebaseAuth auth;
    public FirebaseUser currentUser;
    private string databaseURL = "https://bucket-nz.firebaseio.com/";
    private static UserAuthentication instance = null;
    //---------------------------------------------------------------//

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    private void Start () {

        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        } else {
            Destroy(this.gameObject);
        }
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl(databaseURL);
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        auth.StateChanged += AuthStateChanged;
        AuthStateChanged(this, null);
        
        LoadUser ();
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    private void Update () {

        if (loading) {
            SetLoadingScreen (loadingScreen, 1.0f, true);
            SetLoadingScreen (loadingScreen.transform.GetChild (0).GetComponent<Image>(), 1.0f, true);
            SetLoadingScreen (loadingScreen.transform.GetChild (0).GetChild (0).GetComponent<Image> (), 1.0f, true);
        } else {
            SetLoadingScreen (loadingScreen, 0.0f, false);
            SetLoadingScreen (loadingScreen.transform.GetChild (0).GetComponent<Image> (), 0.0f, false);
            SetLoadingScreen (loadingScreen.transform.GetChild (0).GetChild (0).GetComponent<Image> (), 0.0f, false);
        }

        if (newUsernameValid && newPasswordValid && newPinValid) {
            if (confirmInput.text == currentPassword) {
                confirmButton.interactable = true;
            } else {
                confirmButton.interactable = false;
            }
        } else {
            confirmButton.interactable = false;
        }

        if (deleteInput.text == currentPassword) {
            deleteButton.interactable = true;
        } else {
            deleteButton.interactable = false;
        }
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    private void SetLoadingScreen (Image img, float alpha, bool val) {

        img.color = Color.Lerp (img.color, new Color (img.color.r, img.color.g, img.color.b, alpha), Time.deltaTime * speed);
        img.raycastTarget = val;
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    private void CheckInputField (InputField input, int min, string field, bool valid) {
        
        Text message = input.transform.GetChild (input.transform.childCount - 1).GetComponent<Text> ();

        if (input.text.Length > 0) {
            if (input.text.Length < min) {
                message.text = field + " is too short";
                valid = false;
            } else {
                message.text = "";
                valid = true;
            }
        } else {
            message.text = "";
            valid = true;
        }
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void ValidateInputs () {

        CheckInputField (newUsername, 6, "New username", newUsernameValid);
        CheckInputField (newPassword, 6, "New password", newPasswordValid);
        CheckInputField (newPin, 4, "New PIN", newPinValid);
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    private void OnDestroy () {

        auth.StateChanged -= AuthStateChanged;
        auth = null;
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void CreateUser (string email, string password) {

        loading = true;

        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith (async (task) => {
            if (task.IsCanceled) {
                await new WaitForUpdate ();
                loading = false;
                messaging.SetMessage ("Process Cancelled.", "Your account was not created due to the process being cancelled part way through.", true);
                return;
            }
            if (task.IsFaulted) {
                await new WaitForUpdate ();
                loading = false;
                messaging.SetMessage ("An error has been encountered.", "Your account has not been created due to the following error: " + task.Exception.GetBaseException (), true);
                return;
            }

            await new WaitForUpdate ();

            Firebase.Auth.FirebaseUser newUser = task.Result;

            transform.GetComponent<DataGetSet> ().currentUserID = currentUser.UserId;
            transform.GetComponent<DataGetSet> ().currentUserEmail = currentUser.Email;

            transform.GetComponent<DataCreateNewUser> ().SaveNewUserData (currentUser.UserId);

            SaveUser (email, password);

            loading = false;

            sceneManager.OpenMenu (2);
        });
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void SignInUser (string email, string password) {

        loading = true;

        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWith (async (task) => {
            if (task.IsCanceled) {
                await new WaitForUpdate ();
                loading = false;
                //debug.UpdateReport ((DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond).ToString (), "Your password was not changed due to the process being cancelled part way through.", debug.red);
                messaging.SetMessage ("Process Cancelled.", "Signing in has failed due to the process being cancelled part way through.", true);
                return;
            }
            if (task.IsFaulted) {
                await new WaitForUpdate ();
                loading = false;
                messaging.SetMessage ("An error has been encountered.", "Signing in has failed due to the following error: " + task.Exception.GetBaseException (), true);
                return;
            }

            await new WaitForUpdate ();

            Firebase.Auth.FirebaseUser newUser = task.Result;

            SaveUser (email, password);

            loading = false;
            
            transform.GetComponent<DataGetSet> ().GetUserInfo ();
            transform.GetComponent<DataGetSet> ().currentUserID = currentUser.UserId;

            sceneManager.OpenMenu (2);
        });
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void SignOutUser () {

        auth.SignOut();

        SaveUser ("", "");
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void ChangePassword () {

        if (newPassword.text != "") {
            auth.CurrentUser.UpdatePasswordAsync (newPassword.text).ContinueWith (async (task) => {
                if (task.IsCanceled) {
                    await new WaitForUpdate ();
                    //debug.UpdateReport ((DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond).ToString (), "Your password was not changed due to the process being cancelled part way through.", debug.red);
                    messaging.SetMessage ("Process Cancelled.", "Your password was not changed due to the process being cancelled part way through.", true);
                    return;
                }
                if (task.IsFaulted) {
                    await new WaitForUpdate ();
                    //debug.UpdateReport ((DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond).ToString (), "Your password has not been changed due to the following error: " + task.Exception.GetBaseException (), debug.red);
                    messaging.SetMessage ("An error has been encountered.", "Your password has not been changed due to the following error: " + task.Exception.GetBaseException (), true);
                    return;
                }
                await new WaitForUpdate ();
                SaveUser (auth.CurrentUser.Email, newPassword.text);
                currentPassword = newPassword.text;
                newPassword.text = "";
                confirmInput.text = "";
            });
        } else {
            confirmInput.text = "";
        }
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void DeleteAccount () {

        string uid = auth.CurrentUser.UserId;

        auth.CurrentUser.DeleteAsync ().ContinueWith (async (task) => {
            if (task.IsCanceled) {
                await new WaitForUpdate ();
                //debug.UpdateReport ((DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond).ToString (), "Your account was not deleted due to the process being cancelled part way through.", debug.red);
                messaging.SetMessage ("Process Cancelled.", "Your account was not deleted due to the process being cancelled part way through.", true);
                return;
            }
            if (task.IsFaulted) {
                await new WaitForUpdate ();
                //debug.UpdateReport ((DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond).ToString (), "Your account has not been deleted due to the following error: " + task.Exception.GetBaseException (), debug.red);
                messaging.SetMessage ("An error has been encountered.", "Your account has not been deleted due to the following error: " + task.Exception.GetBaseException (), true);
                return;
            }
            await new WaitForUpdate ();
            circlesManager.RemoveFromAllCircles (this.transform.GetComponent<DataGetSet>().currentUserID);
            //debug.message = "Account Deleted";
            deleteInput.text = "";
            SaveUser ("", "");
            //ClearData (uid);
            sceneManager.OpenMenu (1);
            RemoveFromTotal ();
        });
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    private void RemoveFromTotal () {

        DataRef.General ().GetValueAsync ().ContinueWith (async (task) => {
            if (task.IsCanceled || task.IsFaulted) {
                await new WaitForUpdate ();
                return;
            }
            await new WaitForUpdate ();
            DataSnapshot snapshot = task.Result;
            int totalusers = int.Parse (snapshot.Child ("TotalDonors").Value.ToString ());
            int newUsers = totalusers - 1;

            RemoveFromTotalSet ("TotalDonors", newUsers);
        });
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    private void RemoveFromTotalSet (string child, int value) {

        DataRef.General ().Child (child).SetValueAsync (value.ToString ()).ContinueWith (async (task) => {
            if (task.IsFaulted || task.IsCanceled) {
                await new WaitForUpdate ();
                print (task.Exception);
                return;
            }
            await new WaitForUpdate ();
        });
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void ClearData (string uid) {

        DataRef.User (uid).RemoveValueAsync ().ContinueWith (async (task) => {
            if (task.IsFaulted) {
                await new WaitForUpdate ();
                //debug.message = "Process Faulted (Clearing Data)";
                messaging.SetMessage ("Process Cancelled.", "Your data was not cleared due to the process being cancelled part way through.", true);
                return;
            }
            if (task.IsFaulted) {
                await new WaitForUpdate ();
                //debug.message = "Process Canceled (Clearing Data)";
                messaging.SetMessage ("An error has been encountered.", "Your data has not been cleared due to the following error: " + task.Exception.GetBaseException (), true);
                return;
            }
            await new WaitForUpdate ();
            //debug.message = "Data Cleared";
        });
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    private void AuthStateChanged (object sender, System.EventArgs e) {

        if (auth.CurrentUser != currentUser) {
            bool signedIn = currentUser != auth.CurrentUser && auth.CurrentUser != null;
            if (!signedIn && currentUser != null) {
                Debug.Log ("Signed out " + currentUser.UserId);
            }
            currentUser = auth.CurrentUser;
            if (signedIn) {
                Debug.Log ("Signed in " + currentUser.UserId);
                loading = true;
            }
        }
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    private void SaveUser (string e, string p) {

        BinaryFormatter bf = new BinaryFormatter ();
        FileStream file = File.Create (Application.persistentDataPath + "/userInfo.dat");

        UserStoredInfo info = new UserStoredInfo ();
        info.storedEmail = e;
        info.storedPassword = p;
        currentPassword = p;

        bf.Serialize (file, info);
        file.Close ();
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    private void LoadUser () {

        if (File.Exists (Application.persistentDataPath + "/userInfo.dat")) {

            BinaryFormatter bf = new BinaryFormatter ();
            FileStream file = File.Open (Application.persistentDataPath + "/userInfo.dat", FileMode.Open);

            UserStoredInfo info = (UserStoredInfo)bf.Deserialize (file);
            file.Close ();

            if (info.storedEmail != "") {
                SignInUser (info.storedEmail, info.storedPassword);
                loading = true;
                currentPassword = info.storedPassword;
            } else {
                loading = false;
            }
        } else {
            loading = false;
        }
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//
}
[Serializable]
class UserStoredInfo {

    public string storedEmail;
    public string storedPassword;
}
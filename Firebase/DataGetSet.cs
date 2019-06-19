using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Firebase;
using Firebase.Database;

public class DataGetSet : MonoBehaviour {

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    [SerializeField]
    private DebugManager debug;

    [SerializeField]
    private GroupManager groupManager;
    [SerializeField]
    private ProfilePictureManager pictureManager;
    [SerializeField]
    private MenuNavigation menuNav;
    [SerializeField]
    private ActivityManager activityManager;
    [SerializeField]
    private CirclesManager circlesManager;

    [SerializeField]
    private Text username;
    [SerializeField]
    private Text createdDate;
    [SerializeField]
    private TopUpManager topUp;

    [SerializeField]
    private InputField newUsername;
    [SerializeField]
    private InputField newPIN;

    public string currentUsername;
    public string currentUserID;
    public string currentUserEmail;
    public string userPermissions = "Default";

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void GetUserInfo () {

        print (DateTime.Now.ToString ("MMMM") + " " + DateTime.Now.Day.ToString () + " at " + DateTime.Now.ToString ("hh") + ":" + DateTime.Now.Minute.ToString () + " " + DateTime.Now.ToString ("tt"));

        //debug.message = "Process Starting (Get User Info)";
        debug.message = FirebaseDatabase.DefaultInstance.RootReference.ToString ();
        DataRef.CurrentUser ().GetValueAsync ().ContinueWith (async (task) => {
            await new WaitForUpdate ();
            if (task.IsCanceled || task.IsFaulted) {
                return;
            }
            DataSnapshot snapshot = task.Result;
            username.text = snapshot.Child ("Username").Value.ToString ();
            currentUsername = snapshot.Child ("Username").Value.ToString ();
            DateTime tempDate = Convert.ToDateTime (snapshot.Child ("CreatedDate").Value.ToString ());
            string month = tempDate.ToString ("MMMM");
            string year = tempDate.Year.ToString();
            createdDate.text = "Member Since " + month + " " + year;
            userPermissions = snapshot.Child ("Permissions").Value.ToString ();
            menuNav.SetComingSoon (userPermissions);
            topUp.target = (int.Parse (snapshot.Child ("Balance").Value.ToString ())) / 100;
            topUp.balance = (int.Parse (snapshot.Child ("Balance").Value.ToString ())) / 100;
            pictureManager.DisplayPicture (snapshot.Child ("PictureID").Value.ToString ());
            pictureManager.RevealProfileInfo ();
            
            transform.GetComponent<UserAuthentication> ().loading = false;

            GetGroupInfo (snapshot);

            activityManager.StartActivityProcess ();
            circlesManager.StartCircleProcess ();
        });
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void GetGroupInfo (DataSnapshot snap) {

        if (snap.Child ("Groups").HasChildren) {
            groupManager.ClearList ();

            for (int i = 1; i <= snap.Child ("Groups").ChildrenCount; i++) {
                groupManager.InstantiateGroup (snap.Child ("Groups").Child ("Group" + i.ToString ()).Value.ToString ());
            }
        }
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void SetInfoSingle (string target, string value) {

        debug.message = "Process Starting (Set Info Single)";
        DataRef.CurrentUser ().Child (target).SetValueAsync (value).ContinueWith (async (task) => {
            await new WaitForUpdate ();
            if (task.IsCanceled || task.IsFaulted) {
                debug.message = "Process Error (Set Info Single)";
                return;
            }
            debug.message = "Process Completed (Set Info Single)";
        });
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void SetDoerBalance (string target, string value) {

        DataRef.Projects (target).Child ("Funds").SetValueAsync (value).ContinueWith (async (task) => { await new WaitForUpdate (); });
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void ConfirmChanges () {

        pictureManager.SetNewPictureID ();
        
        if (newUsername.text != "") {
            SetInfoSingle ("Username", newUsername.text);
            username.text = newUsername.text;
            newUsername.text = "";
            if (newPIN.text.Length == 4) {
                
            } else {

            }
            GetComponent<UserAuthentication> ().ChangePassword ();
        } else {
            GetComponent<UserAuthentication> ().ChangePassword ();
        }

        menuNav.SetSubMenu (0);
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//
}
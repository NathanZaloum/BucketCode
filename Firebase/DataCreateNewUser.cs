using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Firebase;
using Firebase.Database;

public class DataCreateNewUser : MonoBehaviour {

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    [SerializeField]
    private DebugManager debug;
    [SerializeField]
    private ActivityManager activityManager;
    [SerializeField]
    private CirclesManager circlesManager;

    [SerializeField]
    private InputField username;
    [SerializeField]
    private InputField pinNumber;

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void SaveNewUserData (string userID) {
        
        DataRef.CurrentUser ().Child ("Username").SetValueAsync (username.text).ContinueWith (async (task) => {
            await new WaitForUpdate ();
            if (task.IsCanceled || task.IsFaulted) {
                return;
            }
        });
        DataRef.CurrentUser ().Child ("CreatedDate").SetValueAsync (DateTime.Now.ToString()).ContinueWith (async (task) => {
            await new WaitForUpdate ();
            if (task.IsCanceled || task.IsFaulted) {
                return;
            }
        });
        DataRef.CurrentUser ().Child ("Balance").SetValueAsync ("0").ContinueWith (async (task) => {
            await new WaitForUpdate ();
            if (task.IsCanceled || task.IsFaulted) {
                return;
            }
        });
        DataRef.CurrentUser ().Child ("stripe").Child ("pin").SetValueAsync (pinNumber.text).ContinueWith (async (task) => {
            await new WaitForUpdate ();
            if (task.IsCanceled || task.IsFaulted) {
                return;
            }
        });
        DataRef.CurrentUser ().Child ("PictureID").SetValueAsync ("Animal_Kiwi").ContinueWith (async (task) => {
            await new WaitForUpdate ();
            if (task.IsCanceled || task.IsFaulted) {
                return;
            }
        });
        DataRef.CurrentUser ().Child ("Permissions").SetValueAsync ("Default").ContinueWith (async (task) => {
            await new WaitForUpdate ();
            if (task.IsCanceled || task.IsFaulted) {
                return;
            }
        });
        DataRef.CurrentUser ().Child ("DonationTotal").SetValueAsync ("0").ContinueWith (async (task) => {
            await new WaitForUpdate ();
            if (task.IsCanceled || task.IsFaulted) {
                return;
            }
        });
        DataRef.CurrentUser ().Child ("Circles").Child ("Bucket Circle New Zealand").SetValueAsync ("Bucket Circle New Zealand").ContinueWith (async (task) => {
            await new WaitForUpdate ();
            if (task.IsCanceled || task.IsFaulted) {
                return;
            }
        });
        DataRef.CurrentUser ().Child ("Onboarded").Child ("Bucket Circle New Zealand").SetValueAsync ("false").ContinueWith (async (task) => {
            await new WaitForUpdate ();
            if (task.IsCanceled || task.IsFaulted) {
                return;
            }
        });
        DataRef.Circles ("Bucket Circle New Zealand").Child ("Members").Child (userID.ToString ()).SetValueAsync (userID.ToString ()).ContinueWith (async (task) => {
            await new WaitForUpdate ();
            if (task.IsCanceled || task.IsFaulted) {
                return;
            }
        });

        DataRef.Circles ("Bucket Circle New Zealand").Child ("TotalMembers").GetValueAsync ().ContinueWith (async (task) => {
            await new WaitForUpdate ();
            if (task.IsCanceled || task.IsFaulted) {
                return;
            }
            DataSnapshot snapshot = task.Result;
            int totalMembers = int.Parse (snapshot.Value.ToString ());
            int newTotal = totalMembers + 1;

            AddToCircleTotal (newTotal);
        });

        DataRef.General ().Child ("TotalDonors").GetValueAsync ().ContinueWith (async (task) => {
            await new WaitForUpdate ();
            if (task.IsCanceled || task.IsFaulted) {
                return;
            }
            DataSnapshot snapshot = task.Result;
            int totalusers = int.Parse (snapshot.Value.ToString ());
            int newUsers = totalusers + 1;

            AddToTotalSet ("TotalDonors", newUsers);
        });

        GetComponent<DataGetSet> ().GetUserInfo ();
        activityManager.SetAlertNewDonor (username.text, userID);

        activityManager.StartActivityProcess ();
        circlesManager.StartCircleProcess ();
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    private void AddToTotalSet (string child, int value) {

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

    private void AddToCircleTotal (int value) {

        DataRef.Circles ("Bucket Circle New Zealand").Child ("TotalMembers").SetValueAsync (value.ToString ()).ContinueWith (async (task) => {
            await new WaitForUpdate ();
            if (task.IsCanceled || task.IsFaulted) {
                return;
            }
        });
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//
}
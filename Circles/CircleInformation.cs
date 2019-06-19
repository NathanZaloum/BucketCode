using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Firebase;
using Firebase.Database;

public class CircleInformation : MonoBehaviour {

    //----------------------------------------------------------------------------------------------------------------------------------------------//
    
    [SerializeField]
    private Button settingsButton;
    [SerializeField]
    private Text titleField;
    [SerializeField]
    private Text aboutField;
    [SerializeField]
    private RectTransform projectsContent;

    private bool isMember;

    public string circleRef;
    
    private string title;
    private string about;
    private string totalMembers;
    private string totalDonated;
    private string accessType;

    private List<string> members = new List<string> ();
    private List<string> projects = new List<string> ();

    private bool load = true;

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    private void Start () {

        //GetInformation ();
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    private void GetInformation () {

        DataRef.Circles (circleRef).GetValueAsync ().ContinueWith (async (task) => {
            await new WaitForUpdate ();
            if (task.IsFaulted || task.IsCanceled) {
                print (task.Exception);
                return;
            }
            DataSnapshot snapshot = task.Result;

            title = snapshot.Child ("Name").Value.ToString ();
            about = snapshot.Child ("About").Value.ToString ();
            accessType = snapshot.Child ("Access").Value.ToString ();
            int tempMembers = int.Parse (snapshot.Child ("TotalMembers").Value.ToString ());
            int tempDonated = int.Parse (snapshot.Child ("TotalDonated").Value.ToString ());
            
            if (tempMembers < 100) {
                totalMembers = (tempMembers).ToString ();
            } else {
                totalMembers = String.Format ("{0:0,0}", (tempMembers));
            }
            if (tempDonated < 10000) {
                totalDonated = (tempDonated / 100).ToString ();
            } else {
                totalDonated = String.Format ("{0:0,0}", (tempDonated / 100));
            }

            members.Clear ();
            projects.Clear ();

            foreach (DataSnapshot snap in snapshot.Child ("Members").Children) {
                members.Add (snap.Value.ToString ());
            }
            if (snapshot.Child ("Projects").HasChildren) {
                foreach (DataSnapshot snap in snapshot.Child ("Projects").Children) {
                    projects.Add (snap.Child ("Reference").Value.ToString ());
                }
            }

            SetHudDisplay ();

            if (circleRef == "Bucket Circle New Zealand") {
                if (load) {
                    SetMainDisplay ();
                    load = false;
                }
            } else {
                load = false;
            }
            
        });
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    private void SetHudDisplay () {

        Transform info = this.gameObject.transform.GetChild (1).GetComponent<Transform> ();
        if (int.Parse (totalMembers) == 1) {
            info.GetChild (0).GetComponent<Text> ().text = "1 Member";
        } else {
            info.GetChild (0).GetComponent<Text> ().text = totalMembers + " Members";
        }
        info.GetChild (1).GetComponent<Text> ().text = "$" + totalDonated + " Donated";
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void SetMainDisplay () {
        
        aboutField.text = about;
        titleField.text = title;

        if (accessType == "All") {

        } else {

        }
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//
}
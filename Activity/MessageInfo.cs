using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Firebase;
using Firebase.Database;

public class MessageInfo : MonoBehaviour {

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public string userID;
    public string userName;
    public string referenceID;
    public string referenceName;
    public string messageType;

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void GetUser (string id) {

        userID = id;

        DataRef.User (id).Child ("Username").GetValueAsync ().ContinueWith (async (task) => {
            await new WaitForUpdate ();
            DataSnapshot snapshot = task.Result;
            
            userName = snapshot.Child ("Username").Value.ToString ();
        });
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void GetLink (string type, string reference) {

        messageType = type;
        referenceID = reference;

        if (type == "Project") {
            DataRef.Projects (reference).GetValueAsync ().ContinueWith (async (task) => {
                await new WaitForUpdate ();
                DataSnapshot snapshot = task.Result;

                referenceName = snapshot.Child ("ProjectName").Value.ToString ();

                SetMessageInfo ();
            });
        } else if (type == "Circle") {
            DataRef.Circles (reference).GetValueAsync ().ContinueWith (async (task) => {
                await new WaitForUpdate ();
                DataSnapshot snapshot = task.Result;

                referenceName = snapshot.Child ("Name").Value.ToString ();

                SetMessageInfo ();
            });
        }
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    private void SetMessageInfo () {

        if (messageType == "Project") {
            transform.GetChild (0).GetComponent<Text> ().text = userName + " recommends checking out this project: " + referenceName;
            transform.GetChild (1).GetChild (1).GetChild (0).GetComponent<Text> ().text = "View Project";
        } else if (messageType == "Circle") {
            transform.GetChild (0).GetComponent<Text> ().text = userName + " has invited you to join this circle: " + referenceName;
            transform.GetChild (1).GetChild (1).GetChild (0).GetComponent<Text> ().text = "View Circle";
        }
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void GoToLink () {

        if (messageType == "Project") {
            
        } else if (messageType == "Circle") {
            
        }
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void DismissMessage () {

        DataRef.CurrentUser ().Child ("Inbox").Child (transform.gameObject.name).RemoveValueAsync ().ContinueWith (async (task) => { await new WaitForUpdate (); });
        Destroy (this.gameObject);
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//
}
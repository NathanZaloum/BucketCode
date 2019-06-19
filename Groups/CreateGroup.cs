using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Firebase;
using Firebase.Database;
using Firebase.Auth;

public class CreateGroup : MonoBehaviour {

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    private FirebaseAuth auth;

    [SerializeField]
    private DataGetSet dataGetSet;
    [SerializeField]
    private MenuNavigation canvas;

    [SerializeField]
    private Color customRed, customGreen;
    
    [SerializeField]
    private InputField groupName, groupDescription, contact;

    [SerializeField]
    private Text nameMessage, descriptionMessage, contactMessage;

    [SerializeField]
    private Text nameText, descriptionText, contactText;

    [SerializeField]
    private Button createButton;
    [SerializeField]
    private Text buttonText;

    private bool taken = false;
    private bool nameValid = false;
    private bool descriptionValid = false;
    private bool contactValid = false;
    private bool creating = false;

    private DataSnapshot snap;

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    private void Start () {

        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    private void Update () {

        if (groupName.text.Length < 6) {
            nameMessage.text = "Name is too short";
            nameText.color = customRed;
            nameValid = false;
        } else {
            if (taken) {
                nameMessage.text = "Name is taken";
                nameText.color = customRed;
                nameValid = false;
            } else {
                nameMessage.text = "";
                nameText.color = customGreen;
                nameValid = true;
            }
        }

        if (groupDescription.text.Length < 6) {
            descriptionMessage.text = "Description is too short";
            descriptionText.color = customRed;
            descriptionValid = false;
        } else {
            descriptionMessage.text = "";
            descriptionText.color = customGreen;
            descriptionValid = true;
        }

        if (contact.text.Length < 6) {
            contactMessage.text = "Contact is too short";
            contactText.color = customRed;
            contactValid = false;
        } else {
            contactMessage.text = "";
            contactText.color = customGreen;
            contactValid = true;
        }

        if (nameValid && descriptionValid && contactValid) {
            if (creating) {
                createButton.interactable = false;
                buttonText.text = "Creating Group...";
            } else {
                createButton.interactable = true;
                buttonText.text = "Create Group";
            }
        } else {
            createButton.interactable = false;
            buttonText.text = "Complete Form";
        }
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void CheckForName () {

        if (groupName.text.Length > 5) {
            DataRef.Groups (groupName.text).GetValueAsync ().ContinueWith (async (task) => {
                await new WaitForUpdate ();
                DataSnapshot snapshot = task.Result;
                if (snapshot.Exists) {
                    taken = true;
                } else {
                    taken = false;
                }
            });
        }
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void Create () {

        creating = true;

        string uid = auth.CurrentUser.UserId.ToString();

        DataRef.CurrentUser ().GetValueAsync ().ContinueWith (async (task) => {
            await new WaitForUpdate ();
            DataSnapshot snapshot = task.Result;
            SetReference ("Group" + (snapshot.Child ("Groups").ChildrenCount + 1).ToString(), groupName.text);
            snap = snapshot;
        });

        DataRef.Groups (groupName.text).Child ("GroupName").SetValueAsync (groupName.text).ContinueWith (async (task) => { await new WaitForUpdate (); });
        DataRef.Groups (groupName.text).Child ("GroupDescription").SetValueAsync (groupDescription.text).ContinueWith (async (task) => { await new WaitForUpdate (); });
        DataRef.Groups (groupName.text).Child ("Contact").SetValueAsync (contact.text).ContinueWith (async (task) => { await new WaitForUpdate (); });
        DataRef.Groups (groupName.text).Child ("Administrator").SetValueAsync (uid).ContinueWith (async (task) => {
            await new WaitForUpdate ();
            ResetFields ();
            dataGetSet.GetGroupInfo (snap);
            canvas.ToggleDoerSub (false);
            creating = false;
        });
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    private void SetReference (string refTitle, string refValue) {

        DataRef.CurrentUser ().Child ("Groups").Child (refTitle).SetValueAsync (refValue).ContinueWith (async (task) => { await new WaitForUpdate (); });
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    private void ResetFields () {

        groupName.text = "";
        groupDescription.text = "";
        contact.text = "";
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//
}
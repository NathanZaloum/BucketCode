using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Firebase;
using Firebase.Database;

public class FeedbackManager : MonoBehaviour {

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    [SerializeField]
    private DataGetSet userData;
    
    [SerializeField]
    private Text displayText;
    [SerializeField]
    private InputField inputField;
    [SerializeField]
    private Button submitButton;

    [SerializeField]
    private RectTransform content;
    [SerializeField]
    private RectTransform message;
    [SerializeField]
    private RectTransform signature;
    [SerializeField]
    private RectTransform comment;

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    private void Update () {

        if (inputField.isFocused == true) {
            inputField.transform.GetChild (1).gameObject.SetActive (false);
        } else {
            inputField.transform.GetChild (1).gameObject.SetActive (true);
        }

        content.sizeDelta = new Vector2 (content.sizeDelta.x, 500 + message.sizeDelta.y + signature.sizeDelta.y + comment.sizeDelta.y);
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void AdjustText () {

        displayText.text = inputField.text;

        if (displayText.text.Length > 0) {
            submitButton.interactable = true;
        } else {
            submitButton.interactable = false;
            comment.sizeDelta = new Vector2 (comment.sizeDelta.x, 58.2f);
        }
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void SaveFeedback () {

        string date = DateTime.Now.ToString ("dd MMMM yyyy");
        string time = DateTime.Now.ToString ("HH:mm:ss");
        string userID = userData.currentUserID;
        string userEmail = userData.currentUserEmail;
        string feedbackTitle = (DateTime.Now.ToString ("yyyy-MM-dd (HH:mm:ss) ") + userEmail);
        print (feedbackTitle);

        DataRef.Feedback (feedbackTitle).Child ("Date").SetValueAsync (date).ContinueWith (async (task) => { await new WaitForUpdate (); });
        DataRef.Feedback (feedbackTitle).Child ("Time").SetValueAsync (time).ContinueWith (async (task) => { await new WaitForUpdate (); });
        DataRef.Feedback (feedbackTitle).Child ("UserID").SetValueAsync (userID).ContinueWith (async (task) => { await new WaitForUpdate (); });
        DataRef.Feedback (feedbackTitle).Child ("UserEmail").SetValueAsync (userEmail).ContinueWith (async (task) => { await new WaitForUpdate (); });
        DataRef.Feedback (feedbackTitle).Child ("Feedback").SetValueAsync (displayText.text).ContinueWith (async (task) => { await new WaitForUpdate (); });

        inputField.text = "";
        AdjustText ();
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//
}
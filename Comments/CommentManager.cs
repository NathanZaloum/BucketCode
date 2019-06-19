using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Firebase;
using Firebase.Database;

public class CommentManager : MonoBehaviour {

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    [SerializeField]
    private CirclesManager circlesManager;

    [SerializeField]
    private GameObject commentBox;

    [SerializeField]
    private Button commentButton;

    [SerializeField]
    private Text displayText;
    [SerializeField]
    private InputField inputField;

    [SerializeField]
    private RectTransform commentMainBox;
    [SerializeField]
    private RectTransform commentInputBox;
    [SerializeField]
    private RectTransform commentsContent;

    [SerializeField]
    private GameObject commentPrefab;

    [SerializeField]
    private Text commentNumber;

    private string pushKey;
    private string date;
    private string timestamp;
    private string comment;
    private string user;
    
    private bool commentsOpen;

    public bool breakCycle = false;

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    private void Update () {

        if (inputField.isFocused == true) {
            inputField.transform.GetChild (1).gameObject.SetActive (false);
        } else {
            inputField.transform.GetChild (1).gameObject.SetActive (true);
        }

        commentNumber.text = commentsContent.childCount.ToString ();
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void AdjustText () {

        displayText.text = inputField.text;

        if (displayText.text.Length > 0) {
            commentButton.interactable = true;
            commentButton.transform.GetChild (0).GetComponent<Text> ().color = new Color (0.0f, 0.686f, 0.541f);
        } else {
            commentButton.interactable = false;
            commentButton.transform.GetChild (0).GetComponent<Text> ().color = new Color (0.855f, 0.855f, 0.855f);
        }
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void CreateComment () {
        
        GetInstance ();
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    private void GetInstance () {

        pushKey = DataRef.Circles (circlesManager.activeCircleRef).Child ("Comments").Push ().Key;
        date = DateTime.Now.ToString ("MMMM") + " " + DateTime.Now.Day.ToString () + " at " + DateTime.Now.ToString ("hh") + ":" + DateTime.Now.Minute.ToString () + " " + DateTime.Now.ToString ("tt");
        timestamp = (DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond).ToString ();
        comment = displayText.text;
        user = DataRef.CurrentUser ().Key.ToString ();

        SetValue ("User", user);
        SetValue ("Date", date);
        SetValue ("Timestamp", timestamp);
        SetValue ("Comment", comment);

        inputField.text = "";
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    private void SetValue (string child, string value) {

        DataRef.Circles (circlesManager.activeCircleRef).Child ("Comments").Child (pushKey).Child (child).SetValueAsync (value).ContinueWith (async (taskSet) => { await new WaitForUpdate (); });
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void PopulateList () {

        if (commentsContent.childCount > 0) {
            foreach (Transform child in commentsContent) {
                Destroy (child.gameObject);
            }
        }

        DataRef.Circles (circlesManager.activeCircleRef).Child ("Comments").GetValueAsync ().ContinueWith (async (task) => {
            await new WaitForUpdate ();
            DataSnapshot snapshot = task.Result;

            foreach (DataSnapshot comment in snapshot.Children) {
                string ts = comment.Child ("Timestamp").Value.ToString ();
                string userRef = comment.Child ("User").Value.ToString ();
                string d = comment.Child ("Date").Value.ToString ();
                string c = comment.Child ("Comment").Value.ToString ();
                InstantiateComment (ts, userRef, d, c);
            }

            OrganiseList ();
        });
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void OrganiseList () {

        List<Transform> comments = new List<Transform> ();
        foreach (Transform com in commentsContent) {
            comments.Add (com);
        }

        comments = comments.OrderByDescending (x => x.name).ToList ();

        for (int i = 0; i < commentsContent.transform.childCount; i++) {
            comments[i].SetSiblingIndex (i);
        }
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    private void InstantiateComment (string ts, string userReference, string d, string c) {
        
        GameObject com = Instantiate (commentPrefab, Vector3.zero, Quaternion.identity) as GameObject;
        com.name = ts;
        com.transform.SetParent (commentsContent);
        com.transform.localScale = new Vector3 (1, 1, 1);
        com.transform.GetComponent<CommentInfo> ().AssignInfo (userReference, d, c);
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void StartCommentCheck () {

        if (breakCycle == true) {
            breakCycle = false;
            PopulateReCheck ();
        } else {
            DataRef.Circles (circlesManager.activeCircleRef).Child ("Comments").GetValueAsync ().ContinueWith (async (task) => {
                await new WaitForUpdate ();
                DataSnapshot snapshot = task.Result;

                if (int.Parse (snapshot.ChildrenCount.ToString ()) != commentsContent.childCount) {
                    PopulateReCheck ();
                } else {
                    StartCoroutine (CheckForNewComments (0.1f));
                }
            });
        }
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    private IEnumerator CheckForNewComments (float delay) {

        yield return new WaitForSeconds (delay);

        StartCommentCheck ();
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void PopulateReCheck () {

        if (commentsContent.childCount > 0) {
            foreach (Transform child in commentsContent) {
                Destroy (child.gameObject);
            }
        }

        DataRef.Circles (circlesManager.activeCircleRef).Child ("Comments").GetValueAsync ().ContinueWith (async (task) => {
            await new WaitForUpdate ();
            DataSnapshot snapshot = task.Result;

            foreach (DataSnapshot comment in snapshot.Children) {
                string ts = comment.Child ("Timestamp").Value.ToString ();
                string userRef = comment.Child ("User").Value.ToString ();
                string d = comment.Child ("Date").Value.ToString ();
                string c = comment.Child ("Comment").Value.ToString ();
                InstantiateComment (ts, userRef, d, c);
            }

            OrganiseList ();

            StartCoroutine (CheckForNewComments (0.1f));
        });
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void ToggleCommentsBox () {

        commentsOpen = !commentsOpen;

        if (commentsOpen == true) {
            commentBox.transform.GetComponent<RectTransform> ().localScale = new Vector3 (1.0f, 1.0f, 1.0f);
        } else if (commentsOpen == false) {
            commentBox.transform.GetComponent<RectTransform> ().localScale = new Vector3 (0.0f, 0.0f, 0.0f);
        }
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//
}
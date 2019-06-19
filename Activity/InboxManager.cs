using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Firebase;
using Firebase.Database;

public class InboxManager : MonoBehaviour {

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    [SerializeField]
    private GameObject inbox;
    [SerializeField]
    private Transform content;
    [SerializeField]
    private Transform viewport;
    [SerializeField]
    private Transform inboxNumber;

    [SerializeField]
    private GameObject messagePrefab;

    private string pushKey;
    private string date;
    private string timestamp;

    private bool openState = false;

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    private void Update () {

        if (content.parent.GetComponent<RectTransform>().sizeDelta.y < 500) {
            viewport.GetComponent<LayoutElement> ().preferredHeight = content.parent.GetComponent<RectTransform> ().sizeDelta.y;
        } else {
            viewport.GetComponent<LayoutElement> ().preferredHeight = 500.0f;
        }

        inboxNumber.GetComponent<Text> ().text = content.childCount.ToString ();
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void StartInboxCheck () {
        
        DataRef.CurrentUser ().Child ("Inbox").GetValueAsync ().ContinueWith (async (task) => {
            await new WaitForUpdate ();
            DataSnapshot snapshot = task.Result;

            if (int.Parse (snapshot.ChildrenCount.ToString ()) != (content.childCount)) {
                PopulateReCheck ();
            } else {
                StartCoroutine (CheckForNewMessage (0.1f));
            }
        });
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    private IEnumerator CheckForNewMessage (float delay) {

        yield return new WaitForSeconds (delay);

        StartInboxCheck ();
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void PopulateReCheck () {

        if (content.childCount > 0) {
            foreach (Transform child in content) {
                Destroy (child.gameObject);
            }
        }

        DataRef.Activity ().GetValueAsync ().ContinueWith (async (task) => {
            await new WaitForUpdate ();
            DataSnapshot snapshot = task.Result;

            foreach (DataSnapshot message in snapshot.Children) {

                InstantiateMessage (message);
            }

            Descending ();

            StartCoroutine (CheckForNewMessage (0.1f));
        });
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    private void Descending () {

        List<Transform> messages = new List<Transform> ();
        foreach (Transform message in content) {
            messages.Add (message);
        }

        messages = messages.OrderByDescending (x => x.name).ToList ();

        for (int i = 0; i < content.transform.childCount; i++) {
            messages[i].SetSiblingIndex (i);
        }
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    private void GetInstance () {

        pushKey = DataRef.Activity ().Push ().Key;
        date = DateTime.Now.ToString ("MMMM") + " " + DateTime.Now.Day.ToString () + " at " + DateTime.Now.ToString ("hh") + ":" + DateTime.Now.Minute.ToString () + " " + DateTime.Now.ToString ("tt");
        timestamp = (DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond).ToString ();
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void SetAlertDonation (string type, string reference, string userId) {

        GetInstance ();

        SetValue ("Type", type);
        SetValue ("Date", date);
        SetValue ("Timestamp", timestamp);
        SetValue ("UserID", userId);
        SetValue ("Reference", reference);
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    private void SetValue (string child, string value) {

        DataRef.CurrentUser ().Child ("Inbox").Child (pushKey).Child (child).SetValueAsync (value).ContinueWith (async (taskSet) => { await new WaitForUpdate (); });
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    private void InstantiateMessage (DataSnapshot snap) {

        GameObject message = Instantiate (messagePrefab, Vector3.zero, Quaternion.identity) as GameObject;
        message.transform.SetParent (content);
        message.transform.GetComponent<RectTransform> ().anchoredPosition = Vector3.zero;
        message.transform.localScale = new Vector3 (1, 1, 1);
        message.name = snap.Child ("Timestamp").Value.ToString ();
        message.transform.GetComponent<MessageInfo> ().GetUser (snap.Child ("UserID").Value.ToString ());
        message.transform.GetComponent<MessageInfo> ().GetLink (snap.Child ("Type").Value.ToString (), snap.Child ("Reference").Value.ToString ());

        Descending ();
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void ToggleInbox () {

        openState = !openState;

        inbox.SetActive (openState);
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//
}
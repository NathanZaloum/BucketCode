using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Firebase;
using Firebase.Database;

public class ActivityManager : MonoBehaviour {

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    [SerializeField]
    private GameObject donationPrefab;
    [SerializeField]
    private GameObject newDonorPrefab;
    [SerializeField]
    private GameObject newProjectPrefab;
    [SerializeField]
    private GameObject projectFundedPrefab;

    [SerializeField]
    private RectTransform content;

    private string pushKey;
    private string date;
    private string timestamp;

    [SerializeField]
    private int donationS, newDonorS, newProjectS, projectFundedS;

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void StartActivityProcess () {

        StartActivityCheck ();
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void BackToTop () {

        StartCoroutine (MoveToPosition (content, Vector3.zero, 0.2f));
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    private IEnumerator MoveToPosition (Transform transform, Vector3 targetPosition, float timeToMove) {

        Vector3 currentPos = transform.GetComponent<RectTransform> ().anchoredPosition;
        float t = 0.0f;

        content.parent.parent.GetComponent<ScrollRect> ().inertia = false;

        while (t < 1) {
            t += Time.deltaTime / timeToMove;
            transform.GetComponent<RectTransform> ().anchoredPosition = Vector3.Lerp (currentPos, targetPosition, t);
            yield return null;
        }

        content.parent.parent.GetComponent<ScrollRect> ().inertia = true;
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    private void InstantiateAlert (GameObject prefab, string type, int y, DataSnapshot snap) {

        GameObject alert = Instantiate (prefab, Vector3.zero, Quaternion.identity) as GameObject;
        alert.transform.SetParent (content);
        alert.name = snap.Child ("Timestamp").Value.ToString ();
        alert.transform.GetComponent<RectTransform> ().sizeDelta = new Vector2 (0, y);
        alert.transform.GetComponent<RectTransform> ().anchoredPosition = Vector3.zero;
        alert.transform.localScale = new Vector3 (1, 1, 1);

        if (type == "Donation") {
            alert.transform.GetComponent<AlertInfo> ().AssignDonation (snap);
        } else if (type == "New Donor") {
            alert.transform.GetComponent<AlertInfo> ().AssignNewDonor (snap);
        } else if (type == "New Project") {
            alert.transform.GetComponent<AlertInfo> ().AssignNewProject (snap);
        } else if (type == "Project Funded") {
            alert.transform.GetComponent<AlertInfo> ().AssignProjectFunded (snap);
        }
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    private void GetInstance () {
        
        pushKey = DataRef.Activity ().Push ().Key;
        date = DateTime.Now.ToString ("MMMM") + " " + DateTime.Now.Day.ToString () + " at " + DateTime.Now.ToString ("hh") + ":" + DateTime.Now.Minute.ToString () + " " + DateTime.Now.ToString ("tt");
        timestamp = (DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond).ToString ();
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void SetAlertDonation (string amount, string donor, string reference, string userId) {

        GetInstance ();
        
        SetValue ("Type", "Donation");
        SetValue ("Date", date);
        SetValue ("Timestamp", timestamp);
        SetValue ("UserID", userId);
        SetValue ("Amount", amount);
        SetValue ("Donor", donor);
        SetValue ("Reference", reference);
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void SetAlertNewDonor (string name, string userID) {

        GetInstance ();
        
        SetValue ("Type", "New Donor");
        SetValue ("Date", date);
        SetValue ("Timestamp", timestamp);
        SetValue ("UserID", userID);
        SetValue ("Name", name);
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void SetAlertNewProject (string reference, string pictureID) {
        
        GetInstance ();
        
        SetValue ("Type", "New Project");
        SetValue ("Date", date);
        SetValue ("Timestamp", timestamp);
        SetValue ("PictureID", pictureID);
        SetValue ("Reference", reference);
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void SetAlertProjectFunded (string reference, string pictureID) {

        GetInstance ();
        
        SetValue ("Type", "Project Funded");
        SetValue ("Date", date);
        SetValue ("Timestamp", timestamp);
        SetValue ("PictureID", pictureID);
        SetValue ("Reference", reference);
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    private void SetValue (string child, string value) {

        DataRef.Activity ().Child (pushKey).Child (child).SetValueAsync (value).ContinueWith (async (taskSet) => { await new WaitForUpdate (); });
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    private void Descending () {

        List<Transform> events = new List<Transform> ();
        foreach (Transform user in content) {
            events.Add (user);
        }

        events = events.OrderByDescending (x => x.name).ToList ();

        for (int i = 0; i < content.transform.childCount; i++) {
            events[i].SetSiblingIndex (i);
        }
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void StartActivityCheck () {

        DataRef.Activity ().GetValueAsync ().ContinueWith (async (task) => {
            await new WaitForUpdate ();
            DataSnapshot snapshot = task.Result;

            if (int.Parse (snapshot.ChildrenCount.ToString ()) != content.childCount) {
                PopulateReCheck ();
            } else {
                StartCoroutine (CheckForNewActivity (0.1f));
            }
        });
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    private IEnumerator CheckForNewActivity (float delay) {

        yield return new WaitForSeconds (delay);

        StartActivityCheck ();
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

            foreach (DataSnapshot alert in snapshot.Children) {
                string type = alert.Child ("Type").Value.ToString ();

                if (type == "Donation") {
                    InstantiateAlert (donationPrefab, type, donationS, alert);
                } else if (type == "New Donor") {
                    InstantiateAlert (newDonorPrefab, type, newDonorS, alert);
                } else if (type == "New Project") {
                    InstantiateAlert (newProjectPrefab, type, newProjectS, alert);
                } else if (type == "Project Funded") {
                    InstantiateAlert (projectFundedPrefab, type, projectFundedS, alert);
                }
            }

            Descending ();

            StartCoroutine (CheckForNewActivity (0.1f));
        });
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//
}
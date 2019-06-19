using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Firebase;
using Firebase.Database;

public class DebugConsole : MonoBehaviour {

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    [SerializeField]
    private UserAuthentication auth;

    [SerializeField]
    private RectTransform content;
    [SerializeField]
    private GameObject reportPrefab;

    public Color green;
    public Color orange;
    public Color red;

    private bool state = false;

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    private void Update () {

        if (Input.GetKeyDown (KeyCode.RightAlt)) {
            ToggleDebug ();
        }
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void UpdateReport (string label, string message, Color status) {
        
        if (content.Find (label) != null) {
            content.Find (label).GetChild (0).GetComponent<Text> ().text = message;
            content.Find (label).GetChild (1).GetComponent<Image> ().color = status;
        } else {
            AddNew (label, message, status);
        }
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    private void AddNew (string label, string message, Color status) {

        GameObject newReport = Instantiate (reportPrefab, Vector3.zero, Quaternion.identity) as GameObject;
        newReport.transform.SetParent (content);
        newReport.name = label;
        newReport.transform.localScale = new Vector3 (1.0f, 1.0f, 1.0f);
        newReport.transform.GetChild (0).GetComponent<Text> ().text = message;
        newReport.transform.GetChild (1).GetComponent<Image> ().color = status;
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void ToggleDebug () {

        state = !state;

        if (state == true) {
            transform.localScale = new Vector3 (1.0f, 1.0f, 1.0f);
        } else {
            transform.localScale = new Vector3 (0.0f, 0.0f, 0.0f);
        }
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//
}
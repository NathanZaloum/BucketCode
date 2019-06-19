using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Firebase;
using Firebase.Database;

public class ProjectManager : MonoBehaviour {

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    [SerializeField]
    private GameObject projectPrefab;
    
    [SerializeField]
    private RectTransform projectList, anchor, content;
    [SerializeField]
    private GameObject message;
    private int listSize;

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void ClearList () {

        foreach (Transform item in projectList.transform) {
            Destroy (item.gameObject);
        }
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void InstantiateProject (string target) {

        DataRef.Projects (target).GetValueAsync ().ContinueWith (async (task) => {
            await new WaitForUpdate ();
            DataSnapshot snapshot = task.Result;

            string pName = snapshot.Child ("ProjectName").Value.ToString ();
            string gName = snapshot.Child ("Group").Value.ToString ();
            string pRef = "(" + gName + ") " + pName;
            string description = snapshot.Child ("ProjectDescription").Value.ToString ();
            string fundingGoal = snapshot.Child ("FundingGoal").Value.ToString ();
            string fundingAmount = snapshot.Child ("FundingAmount").Value.ToString ();
            string location = snapshot.Child ("Tags").Child ("Location").Value.ToString ();
            string pictureID = snapshot.Child ("PictureID").Value.ToString ();
            List<string> categories = new List<string> ();
            foreach (DataSnapshot snap in snapshot.Child ("Tags").Child ("Categories").Children) {
                categories.Add (snap.Value.ToString ());
            }
            List<string> goalTitles = new List<string> ();
            foreach (DataSnapshot s2 in snapshot.Child ("Measurements").Children) {
                goalTitles.Add (s2.Child ("Title").Value.ToString ());
            }
            List<string> goalValues = new List<string> ();
            foreach (DataSnapshot s3 in snapshot.Child ("Measurements").Children) {
                goalValues.Add (s3.Child ("Value").Value.ToString ());
            }

            GameObject project = Instantiate (projectPrefab, Vector3.zero, Quaternion.identity) as GameObject;
            project.transform.SetParent (projectList);
            project.transform.localScale = new Vector3 (1, 1, 1);
            project.transform.GetComponent<RectTransform> ().sizeDelta = new Vector2 (0, 275);
            project.GetComponent<ProjectInformation> ().SetInformation (pRef, gName, pName, description, fundingGoal, fundingAmount, location, categories, goalTitles, goalValues, pictureID);
        });
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void PopulateList (string gName) {

        ClearList ();

        DataRef.Groups (gName).GetValueAsync ().ContinueWith (async (task) => {
            await new WaitForUpdate ();
            DataSnapshot snapshot = task.Result;

            for (int i = 1; i <= snapshot.Child ("Projects").ChildrenCount; i++) {
                InstantiateProject (/*"(" + snapshot.Child ("GroupName").Value.ToString () + ") " + */snapshot.Child ("Projects").Child ("Project" + i.ToString ()).Value.ToString ());
            }
        });
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    private void Update () {

        listSize = projectList.childCount;

        if (listSize == 0) {
            message.SetActive (true);
        } else {
            message.SetActive (false);
        }

        if (listSize < 2) {
            anchor.sizeDelta = new Vector2 (0, 275);
            content.sizeDelta = new Vector2 (0, 455);
        } else {
            anchor.sizeDelta = new Vector2 (0, (275 * listSize) + (25 * (listSize)));
            content.sizeDelta = new Vector2 (0, (275 * listSize) + (25 * (listSize)) + 180);
        }

        if (listSize == 1) {
            projectList.GetChild (0).GetComponent<RectTransform> ().anchoredPosition = new Vector3 (0, 0, 0);
        } else if (listSize > 1) {
            projectList.GetChild (0).GetComponent<RectTransform> ().anchoredPosition = new Vector3 (0, 0, 0);
            for (int i = 1; i < projectList.childCount; i++) {
                projectList.GetChild (i).GetComponent<RectTransform> ().anchoredPosition = new Vector3 (0, -(300 * i), 0);
            }
        }
	}

    //----------------------------------------------------------------------------------------------------------------------------------------------//
}
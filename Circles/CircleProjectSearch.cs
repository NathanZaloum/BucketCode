using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Firebase;
using Firebase.Database;

public class CircleProjectSearch : MonoBehaviour {

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    [SerializeField]
    private RectTransform projectsContent;
    [SerializeField]
    private GameObject circlesProjectPrefab;

    [SerializeField]
    private CirclesManager circlesManager;

    [SerializeField]
    private Transform filterBar;
    [SerializeField]
    private Image cover;

    public bool breakCycle = false;

    private bool filterStatus = false;
    private string filterType = "Alphabetical";

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void SwitchFilter (string type) {

        filterType = type;
        ToggleFilterMenu ();
        OrganiseList ();
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void ToggleButtons (Button active) {

        filterBar.GetChild (1).GetChild (0).GetComponent<Button> ().interactable = true;
        filterBar.GetChild (1).GetChild (1).GetComponent<Button> ().interactable = true;
        filterBar.GetChild (1).GetChild (2).GetComponent<Button> ().interactable = true;
        filterBar.GetChild (1).GetChild (3).GetComponent<Button> ().interactable = true;
        filterBar.GetChild (1).GetChild (4).GetComponent<Button> ().interactable = true;
        filterBar.GetChild (1).GetChild (5).GetComponent<Button> ().interactable = true;
        filterBar.GetChild (1).GetChild (6).GetComponent<Button> ().interactable = true;
        active.interactable = false;
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void ToggleFilterMenu () {

        filterStatus = !filterStatus;

        if (filterStatus == true) {
            filterBar.GetChild (0).GetChild (0).GetChild (0).GetComponent<RectTransform> ().localRotation = Quaternion.Euler (0.0f, 0.0f, 90.0f);
            cover.raycastTarget = true;
            StartCoroutine (MoveFilterBar (new Vector3 (0.0f, -1175.0f, 0.0f), new Color (0.0f, 0.0f, 0.0f, 0.784f), 0.2f));
            StartCoroutine (MoveTopPadding (660 + 525, 0.2f));
        } else if (filterStatus == false) {
            filterBar.GetChild (0).GetChild (0).GetChild (0).GetComponent<RectTransform> ().localRotation = Quaternion.Euler (0.0f, 0.0f, -90.0f);
            cover.raycastTarget = false;
            StartCoroutine (MoveFilterBar (new Vector3 (0.0f, -650.0f, 0.0f), new Color (0.0f, 0.0f, 0.0f, 0.0f), 0.2f));
            StartCoroutine (MoveTopPadding (660, 0.2f));
            StartCoroutine (MoveProjectListContent (new Vector3 (0.0f, 0.0f, 0.0f), 0.2f));
        }
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void CloseFilterMenu () {

        filterStatus = false;

        filterBar.GetChild (0).GetChild (0).GetChild (0).GetComponent<RectTransform> ().localRotation = Quaternion.Euler (0.0f, 0.0f, -90.0f);
        cover.raycastTarget = false;
        filterBar.GetComponent<RectTransform> ().anchoredPosition = new Vector3 (0.0f, -100.0f, 0.0f);
        cover.color = new Color (0.0f, 0.0f, 0.0f, 0.0f);
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    private IEnumerator MoveFilterBar (Vector3 targetPosition, Color targetAlpha, float timeToMove) {

        Vector3 currentPos = filterBar.GetComponent<RectTransform> ().anchoredPosition;
        Color currentAlpha = cover.color;
        float t = 0.0f;

        while (t < 1) {
            t += Time.deltaTime / timeToMove;
            filterBar.GetComponent<RectTransform> ().anchoredPosition = Vector3.Lerp (currentPos, targetPosition, t);
            cover.color = Color.Lerp (currentAlpha, targetAlpha, t);
            yield return null;
        }
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    private IEnumerator MoveTopPadding (int targetPadding, float timeToMove) {

        int currentPadding = projectsContent.parent.GetComponent<VerticalLayoutGroup> ().padding.top;
        float t = 0.0f;

        while (t < 1) {
            t += Time.deltaTime / timeToMove;
            projectsContent.parent.GetComponent<VerticalLayoutGroup> ().padding.top = Mathf.RoundToInt (Mathf.Lerp (currentPadding, targetPadding, t));
            yield return null;
        }
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    private IEnumerator MoveProjectListContent (Vector3 targetPosition, float timeToMove) {

        Vector3 currentPos = projectsContent.parent.GetComponent<RectTransform> ().anchoredPosition;
        float t = 0.0f;

        while (t < 1) {
            t += Time.deltaTime / timeToMove;
            projectsContent.parent.GetComponent<RectTransform> ().anchoredPosition = Vector3.Lerp (currentPos, targetPosition, t);
            yield return null;
        }
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void PopulateList () {

        foreach (Transform child in projectsContent) {
            Destroy (child.gameObject);
        }

        DataRef.Circles (circlesManager.activeCircleRef).GetValueAsync ().ContinueWith (async (task) => {
            await new WaitForUpdate ();
            DataSnapshot snapshot = task.Result;

            foreach (DataSnapshot project in snapshot.Child ("Projects").Children) {
                int donationsMoney = int.Parse (project.Child ("DonationTotal").Value.ToString ());
                int donationsMembers = int.Parse (project.Child ("Members").ChildrenCount.ToString ());
                string projectRef = project.Child ("Reference").Value.ToString ();
                InstantiateProject (donationsMembers, donationsMoney, snapshot, projectRef);
            }

            OrganiseList ();
        });
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    private void InstantiateProject (int members, int money, DataSnapshot snapshot, string projectRef) {

        GameObject project = Instantiate (circlesProjectPrefab, Vector3.zero, Quaternion.identity) as GameObject;
        project.transform.SetParent (projectsContent);
        project.transform.localScale = new Vector3 (1, 1, 1);

        DataRef.Projects (projectRef).GetValueAsync ().ContinueWith (async (task) => {
            await new WaitForUpdate ();
            DataSnapshot projectSnap = task.Result;

            string pName = projectSnap.Child ("ProjectName").Value.ToString ();
            project.name = pName;
            string gName = projectSnap.Child ("Group").Value.ToString ();
            //string pRef = "(" + gName + ") " + pName;
            string pRef = projectRef;
            string description = projectSnap.Child ("ProjectDescription").Value.ToString ();
            string fundingGoal = projectSnap.Child ("FundingGoal").Value.ToString ();
            string fundingAmount = projectSnap.Child ("FundingAmount").Value.ToString ();
            string location = projectSnap.Child ("Tags").Child ("Location").Value.ToString ();
            string pictureID = projectSnap.Child ("PictureID").Value.ToString ();
            List<string> categories = new List<string> ();
            foreach (DataSnapshot snap in projectSnap.Child ("Tags").Child ("Categories").Children) {
                categories.Add (snap.Value.ToString ());
            }
            List<string> goalTitles = new List<string> ();
            foreach (DataSnapshot s2 in projectSnap.Child ("Measurements").Children) {
                goalTitles.Add (s2.Child ("Title").Value.ToString ());
            }
            List<string> goalValues = new List<string> ();
            foreach (DataSnapshot s3 in projectSnap.Child ("Measurements").Children) {
                goalValues.Add (s3.Child ("Value").Value.ToString ());
            }

            project.transform.GetComponent<ProjectInformation> ().SetCircleRelevantInfo (pRef, gName, pName, description, fundingGoal, fundingAmount, location, categories, goalTitles, goalValues, pictureID, money.ToString (), members.ToString ());
            //project.transform.GetComponent<ProjectInformation> ().SetInformation (pRef, gName, pName, description, fundingGoal, fundingAmount, location, categories, goalTitles, goalValues, pictureID);
        });
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    private void OrganiseList () {

        List<Transform> projects = new List<Transform> ();
        foreach (Transform project in projectsContent) {
            projects.Add (project);
        }

        if (filterType == "Alphabetical") {
            projects = projects.OrderBy (x => x.name).ToList ();
        } else if (filterType == "Dollar Amount Funded") {
            projects = projects.OrderByDescending (x => int.Parse (x.GetComponent<ProjectInformation> ().projectFundAmount)).ToList ();
        } else if (filterType == "Percentage Funded") {
            projects = projects.OrderByDescending (x => int.Parse (x.GetComponent<ProjectInformation> ().fundPercentage)).ToList ();
        } else if (filterType == "Funding Goal") {
            projects = projects.OrderBy (x => int.Parse (x.GetComponent<ProjectInformation> ().projectFundGoal)).ToList ();
        } else if (filterType == "Funds Needed") {
            projects = projects.OrderBy (x => int.Parse (x.GetComponent<ProjectInformation> ().projectFundNeeded)).ToList ();
        } else if (filterType == "Funds From Our Circle") {
            projects = projects.OrderByDescending (x => int.Parse (x.GetComponent<ProjectInformation> ().circleFunded)).ToList ();
        } else if (filterType == "Number of Members") {
            projects = projects.OrderByDescending (x => int.Parse (x.GetComponent<ProjectInformation> ().circleMembers)).ToList ();
        }

        for (int i = 0; i < projectsContent.transform.childCount; i++) {
            projects[i].SetSiblingIndex (i);
        }
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void StartProjectCheck () {

        if (breakCycle == true) {
            breakCycle = false;
            PopulateReCheck ();
        } else {
            DataRef.Circles (circlesManager.activeCircleRef).Child ("Projects").GetValueAsync ().ContinueWith (async (task) => {
                await new WaitForUpdate ();
                DataSnapshot snapshot = task.Result;

                if (int.Parse (snapshot.ChildrenCount.ToString ()) != projectsContent.childCount) {
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

        StartProjectCheck ();
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void PopulateReCheck () {

        if (projectsContent.childCount > 0) {
            foreach (Transform child in projectsContent) {
                Destroy (child.gameObject);
            }
        }

        DataRef.Circles (circlesManager.activeCircleRef).GetValueAsync ().ContinueWith (async (task) => {
            await new WaitForUpdate ();
            DataSnapshot snapshot = task.Result;

            foreach (DataSnapshot project in snapshot.Child ("Projects").Children) {
                int donationsMoney = int.Parse (project.Child ("DonationTotal").Value.ToString ());
                int donationsMembers = int.Parse (project.Child ("Members").ChildrenCount.ToString ());
                string projectRef = project.Child ("Reference").Value.ToString ();
                InstantiateProject (donationsMembers, donationsMoney, snapshot, projectRef);
            }

            OrganiseList ();

            StartCoroutine (CheckForNewComments (0.1f));
        });
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//
}
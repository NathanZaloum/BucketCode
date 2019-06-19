using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Firebase;
using Firebase.Database;

public class DiscoverManager : MonoBehaviour {

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    [SerializeField]
    private GameObject projectPrefab;
    
    [SerializeField]
    private RectTransform resultsBox, content;

    [SerializeField]
    private GameObject message;

    [SerializeField]
    private Transform location, category;

    private List<string> locationProjects = new List<string> ();
    private List<string> categoryProjects = new List<string> ();
    private List<string> mergedProjectsList = new List<string> ();

    [SerializeField]
    private List<Button> subButtons = new List<Button> ();
    [SerializeField]
    private List<GameObject> subMenus = new List<GameObject> ();

    private string filterType = "Alphabetical";

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    private void Update () {

        if (resultsBox.childCount == 0) {
            message.SetActive (true);
        } else {
            message.SetActive (false);
        }
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    private void Start () {

        Search ();
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void Search () {

        ClearList ();
        locationProjects.Clear ();
        categoryProjects.Clear ();
        mergedProjectsList.Clear ();

        List<string> categoriesList = new List<string> ();

        for (int i = 0; i < category.childCount - 1; i++) {
            if (category.GetChild (i).GetComponent<FilterButton> ().selected) {
                categoriesList.Add (category.GetChild (i).GetChild (1).GetComponent<Text> ().text);
            }
        }
        categoriesList = categoriesList.Distinct ().ToList ();

        DataRef.Filters ("Category").GetValueAsync ().ContinueWith (async (task) => {
            await new WaitForUpdate ();
            DataSnapshot snapshot = task.Result;

            foreach (string category in categoriesList) {
                foreach (DataSnapshot snap in snapshot.Child (category).Children) {
                    if (!categoryProjects.Contains (snap.Value.ToString ())) {
                        categoryProjects.Add (snap.Value.ToString ());
                    }
                }
            }
        });

        List<string> locationsList = new List<string> ();

        for (int i = 0; i < location.childCount - 1; i++) {
            if (location.GetChild (i).GetComponent<FilterButton> ().selected) {
                locationsList.Add (location.GetChild (i).GetChild (1).GetComponent<Text> ().text);
            }
        }
        locationsList = locationsList.Distinct ().ToList ();

        DataRef.Filters ("Location").GetValueAsync ().ContinueWith (async (task) => {
            await new WaitForUpdate ();
            DataSnapshot snapshot = task.Result;

            foreach (string location in locationsList) {
                foreach (DataSnapshot snap in snapshot.Child (location).Children) {
                    if (!locationProjects.Contains (snap.Value.ToString ())) {
                        locationProjects.Add (snap.Value.ToString ());
                    }
                }
            }

            foreach (string project in categoryProjects) {
                if (locationProjects.Contains (project)) {
                    mergedProjectsList.Add (project);
                }
            }

            for (int i = 0; i < mergedProjectsList.Count; i++) {
                InstantiateProject (mergedProjectsList[i]);
            }
        });
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    private void InstantiateProject (string target) {

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
            project.transform.SetParent (resultsBox);
            if (pRef == "(Bucket NZ) Bucket Team") {
                project.name = "zzzzz - " + pName;
            } else {
                project.name = pName;
            }
            project.transform.localScale = new Vector3 (1, 1, 1);
            project.GetComponent<ProjectInformation> ().SetInformation (pRef, gName, pName, description, fundingGoal, fundingAmount, location, categories, goalTitles, goalValues, pictureID);

            OrganiseResults (filterType);
        });
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void OrganiseResults (string type) {

        List<Transform> projects = new List<Transform> ();
        foreach (Transform project in resultsBox) {
            projects.Add (project);
        }

        filterType = type;

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
        }

        for (int i = 0; i < resultsBox.transform.childCount; i++) {
            projects[i].SetSiblingIndex (i);
        }
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    private void ClearList () {

        foreach (Transform item in resultsBox) {
            Destroy (item.gameObject);
        }
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void SetSubMenu(GameObject subMenu) {

        this.transform.GetComponent<FilterUI> ().CloseFilterMenu ();

        if (this.transform.GetChild (0).GetChild (2).gameObject.activeSelf == true) {
            this.transform.GetChild (0).GetChild (2).GetComponent<CirclesSearch> ().CloseFilterMenu ();
        }
        if (this.transform.GetChild (0).GetChild (3).gameObject.activeSelf == true) {
            this.transform.GetChild (0).GetChild (3).GetComponent<PeopleSearch> ().CloseFilterMenu ();
        }

        foreach (GameObject menu in subMenus) {
            menu.SetActive (false);
        }

        subMenu.SetActive (true);
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void SetSubMenuButtons(Button subButton) {

        foreach (Button button in subButtons) {
            button.interactable = true;
        }

        subButton.interactable = false;
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//
}
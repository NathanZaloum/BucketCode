using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Firebase;
using Firebase.Database;

public class CreateProject : MonoBehaviour {

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    [SerializeField]
    private GroupManager groupManager;
    [SerializeField]
    private MenuNavigation canvasScript;
    [SerializeField]
    private ActivityManager activityManager;

    [SerializeField]
    private Color customRed, customGreen;

    [SerializeField]
    private RectTransform doerPanel;
    [SerializeField]
    private RectTransform doerSubPanel;

    [SerializeField]
    private Text categoriesText;
    [SerializeField]
    private List<FilterButton> categoriesList = new List<FilterButton> ();

    private List<string> categoryTags = new List<string> ();

    [SerializeField]
    private RectTransform categoryBox;
    private bool categoryOpen = false;

    [SerializeField]
    private RectTransform canvas;
    private float canvasX;
    private float canvasY;

    [SerializeField]
    private InputField projectName, projectDescription;

    [SerializeField]
    private Text nameMessage, descriptionMessage, categoryMessage;

    [SerializeField]
    private Text nameText, descriptionText, categoryText, locationText;

    [SerializeField]
    private Button createButton;
    [SerializeField]
    private Text buttonText;

    [SerializeField]
    private Slider slider;
    [SerializeField]
    private Text fundingGoal;
    private string fund;

    private bool taken = false;
    private bool nameValid = false;
    private bool descriptionValid = false;
    private bool categoryValid = false;
    private bool creating = false;

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    private void Start () {

        canvasX = canvas.sizeDelta.x;
        canvasY = canvas.sizeDelta.y;

        doerSubPanel.sizeDelta = new Vector2 (canvasX, canvasY);
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    private void Update () {

        if (categoryOpen) {
            categoryBox.sizeDelta = Vector2.MoveTowards (categoryBox.sizeDelta, new Vector2 (0, 150), 50.0f);
        } else {
            categoryBox.sizeDelta = Vector2.MoveTowards (categoryBox.sizeDelta, new Vector2 (0, 0), 50.0f);
        }

        if (projectName.text.Length < 6) {
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

        if (projectDescription.text.Length < 6) {
            descriptionMessage.text = "Description is too short";
            descriptionText.color = customRed;
            descriptionValid = false;
        } else {
            descriptionMessage.text = "";
            descriptionText.color = customGreen;
            descriptionValid = true;
        }

        if (categoryText.text.Length == 0) {
            categoryMessage.text = "Pick at least one category";
            categoryValid = false;
        } else {
            categoryMessage.text = "";
            categoryValid = true;
        }

        if (nameValid && descriptionValid && categoryValid) {
            if (creating) {
                createButton.interactable = false;
                buttonText.text = "Creating Project...";
            } else {
                createButton.interactable = true;
                buttonText.text = "Create Project";
            }
        } else {
            createButton.interactable = false;
            buttonText.text = "Complete Form";
        }

        ManageSlider ();
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void CheckForName () {

        if (projectName.text.Length > 5) {
            DataRef.Projects ("(" + groupManager.activeGroup.name + ") " + projectName.text).GetValueAsync ().ContinueWith (async (task) => {
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

    public void ToggleCategoryBox (bool off) {

        if (off) {
            categoryOpen = false;
        } else {
            categoryOpen = !categoryOpen;
        }
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void UpdateCategoriesText () {

        categoryTags.Clear ();

        List<string> categories = new List<string> ();

        foreach (FilterButton item in categoriesList) {
            if (item.selected) {
                categories.Add (item.transform.GetChild (1).GetComponent<Text> ().text);
                categoryTags.Add (item.transform.GetChild (1).GetComponent<Text> ().text);
            }
        }

        string newText = "";

        for (int i = 0; i < categories.Count; i++) {
            if (i == categories.Count - 1) {
                newText += categories[i];
            } else {
                newText += (categories[i] + ", ");
            }
        }

        categoriesText.text = newText;
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void Create () {

        creating = true;

        string groupName = groupManager.activeGroup.groupName;
        string projectRefName = "(" + groupName + ") " + projectName.text;

        DataRef.Groups (groupName).GetValueAsync ().ContinueWith (async (task) => {
            await new WaitForUpdate ();
            DataSnapshot snapshot = task.Result;
            SetReference (groupName, "Project" + (snapshot.Child ("Projects").ChildrenCount + 1).ToString (), projectRefName);
        });

        DataRef.Projects (projectRefName).Child ("ProjectName").SetValueAsync (projectName.text).ContinueWith (async (task) => { await new WaitForUpdate (); });
        DataRef.Projects (projectRefName).Child ("ProjectDescription").SetValueAsync (projectDescription.text).ContinueWith (async (task) => { await new WaitForUpdate (); });
        DataRef.Projects (projectRefName).Child ("FundingGoal").SetValueAsync ((int.Parse (fund) * 100).ToString()).ContinueWith (async (task) => { await new WaitForUpdate (); });
        DataRef.Projects (projectRefName).Child ("FundingAmount").SetValueAsync ("0").ContinueWith (async (task) => { await new WaitForUpdate (); });
        DataRef.Projects (projectRefName).Child ("Group").SetValueAsync (groupName).ContinueWith (async (task) => { await new WaitForUpdate (); });
        foreach (string tag in categoryTags) {
            DataRef.Projects (projectRefName).Child ("Tags").Child ("Categories"). Child (tag).SetValueAsync (tag).ContinueWith (async (task) => { await new WaitForUpdate (); });
            DataRef.Filters ("Category").Child (tag).Child (projectRefName).SetValueAsync (projectRefName).ContinueWith (async (task) => { await new WaitForUpdate (); });
        }
        DataRef.Filters ("Location").Child (locationText.text).Child (projectRefName).SetValueAsync (projectRefName).ContinueWith (async (task) => { await new WaitForUpdate (); });
        DataRef.Projects (projectRefName).Child ("Tags").Child ("Location").SetValueAsync (locationText.text).ContinueWith (async (task) => {
            await new WaitForUpdate ();
            ResetFields ();
            canvasScript.ToggleDoerSub (false);
            creating = false;
            groupManager.activeGroup.Populate ();
        });

        activityManager.SetAlertNewProject (projectRefName, "Group_Default");
        //activityManager.Resubscribe ();
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    private void SetReference (string group, string refTitle, string refValue) {

        DataRef.Groups (group).Child ("Projects").Child (refTitle).SetValueAsync (refValue).ContinueWith (async (task) => { await new WaitForUpdate (); });
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    private void ResetFields () {

        projectName.text = "";
        projectDescription.text = "";
        categoryText.text = "";
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    private void ManageSlider () {

        int moneyVal = Mathf.RoundToInt (200 + (slider.value * 50));
        fund = moneyVal.ToString ();
        if (moneyVal == 1000) {
            fundingGoal.text = "S1,000";
        } else {
            fundingGoal.text = "S" + moneyVal.ToString ();
        }
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//
}
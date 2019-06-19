using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProjectInformation : MonoBehaviour {

    //----------------------------------------------------------------------------------------------------------------------------------------------//
    
    [SerializeField]
    private bool discover;
    
    [SerializeField]
    private GameObject information;
    [SerializeField]
    private GameObject placeholder;

    public string projectRef;
    public string groupName;
    public string projectName;
    public string projectDescription;
    public string projectFundGoal;
    public string projectFundAmount;
    public string projectFundNeeded;
    public string fundPercentage;
    public string location;
    public string projectPicture;
    public List<string> categories = new List<string> ();
    public List<string> goalTitles = new List<string> ();
    public List<string> goalValues = new List<string> ();
    private float sliderval1;

    public string circleFunded;
    public string circleMembers;
    private float sliderval2;

    public ProjectPageDisplay projectPage;
    public MenuNavigation canvas;
    public DebugConsole debug;

    private string timestamp;

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    private void Awake () {

        projectPage = GameObject.Find ("SubPanels").transform.GetChild (1).GetComponent<ProjectPageDisplay> ();
        canvas = GameObject.Find ("Canvas").transform.GetComponent<MenuNavigation> ();
        debug = GameObject.Find ("DebugDisplay").transform.GetComponent<DebugConsole> ();
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    private void RevealInfo () {

        placeholder.SetActive (false);
        information.SetActive (true);
	}

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void SetInformation (string pRef, string gName, string pName, string description, string fundGoal, string fundAmount, string loc, List<string> cats, List<string> gT, List<string> gV, string pictureID) {

        timestamp = (DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond).ToString ();

        projectRef = pRef;
        groupName = gName;
        projectName = pName;
        projectDescription = description;
        projectFundGoal = fundGoal;
        projectFundAmount = fundAmount;
        location = loc;
        projectPicture = pictureID;
        foreach (string s1 in cats) {
            categories.Add (s1);
        }
        foreach (string s2 in gT) {
            goalTitles.Add (s2);
        }
        foreach (string s3 in gV) {
            goalValues.Add (s3);
        }
        float goal = int.Parse (fundGoal);
        float current = int.Parse (fundAmount);
        float percentage = 100 / (goal / current);
        if (fundGoal == "0") {
            fundPercentage = "0";
            sliderval1 = 0;
        } else {
            fundPercentage = Mathf.RoundToInt (percentage).ToString ();
            sliderval1 = percentage / 100;
        }

        int fundNeeded = Mathf.RoundToInt ((goal - current) / 100);
        projectFundNeeded = fundNeeded.ToString ();

        SetDisplay (fundNeeded.ToString ());
        RevealInfo ();
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    private void SetDisplay (string need) {

        Text title = information.transform.GetChild (0).GetComponent<Text> ();
        Text group = information.transform.GetChild (1).GetComponent<Text> ();
        Text percentage = information.transform.GetChild (4).GetComponent<Text> ();
        Text needed = information.transform.GetChild (5).GetChild (0).GetChild (0).GetComponent<Text> ();
        Text funded = information.transform.GetChild (4).GetChild (0).GetChild (0).GetComponent<Text> ();
        Text goal = information.transform.GetChild (5).GetComponent<Text> ();
        Slider slider = information.transform.GetChild (3).GetComponent<Slider> ();
        
        title.text = projectName;
        group.text = groupName;
        percentage.text = fundPercentage + "%";
        funded.text = "($" + (int.Parse (projectFundAmount) / 100).ToString () + ")";
        needed.text = "$" + need;
        goal.text = "(Goal: $" + (int.Parse (projectFundGoal) / 100).ToString () + ")";
        slider.value = sliderval1;

        Image picture = information.transform.GetChild (6).GetChild (0).GetComponent<Image> ();
        Sprite img = Resources.Load<Sprite> ("2D/Projects/" + projectPicture);
        picture.sprite = img;
        picture.GetComponent<ScaleImageToCircle> ().Scale ();

        debug.UpdateReport (projectRef, projectRef + " UI sizes are - " + "\n" +
            "Percentage (SizeDelta: " + percentage.transform.GetComponent<RectTransform>().sizeDelta + "), " + "\n" +
            "Percentage (AnchoredPosition: " + percentage.transform.GetComponent<RectTransform>().anchoredPosition + "), " + "\n" +
            "Percentage (LocalScale: " + percentage.transform.GetComponent<RectTransform> ().localScale + "), " + "\n" +
            "Percentage (TextValue: " + percentage.transform.GetComponent<Text> ().text + "), " + "\n" +
            "Needed (SizeDelta: " + needed.transform.GetComponent<RectTransform> ().sizeDelta + "), " + "\n" +
            "Needed (AnchoredPosition: " + needed.transform.GetComponent<RectTransform> ().anchoredPosition + "), " + "\n" +
            "Needed (LocalScale: " + needed.transform.GetComponent<RectTransform> ().localScale + "), " + "\n" +
            "Needed (TextValue: " + needed.transform.GetComponent<Text> ().text + "), " + "\n" +
            "Funded (SizeDelta: " + funded.transform.GetComponent<RectTransform> ().sizeDelta + "), " + "\n" +
            "Funded (AnchoredPosition: " + funded.transform.GetComponent<RectTransform> ().anchoredPosition + "), " + "\n" +
            "Funded (LocalScale: " + funded.transform.GetComponent<RectTransform> ().localScale + "), " + "\n" +
            "Funded (TextValue: " + funded.transform.GetComponent<Text> ().text + "), " + "\n" +
            "Goal (SizeDelta: " + goal.transform.GetComponent<RectTransform> ().sizeDelta + "), " + "\n" +
            "Goal (AnchoredPosition: " + goal.transform.GetComponent<RectTransform> ().anchoredPosition + "), " + "\n" +
            "Goal (LocalScale: " + goal.transform.GetComponent<RectTransform> ().localScale + "), " + "\n" +
            "Goal (TextValue: " + goal.transform.GetComponent<Text> ().text + ").", debug.green);
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void SendInformation () {

        projectPage.RecieveInformation (projectName, groupName, projectDescription, projectFundAmount, projectFundGoal, goalTitles, goalValues, projectPicture);
        canvas.SwitchSubPanels (projectPage.gameObject);
        canvas.SetSubMenu (2);
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void SendToDonate () {

        projectPage.RecieveInformation (projectName, groupName, projectDescription, projectFundAmount, projectFundGoal, goalTitles, goalValues, projectPicture);
        canvas.SnapSubMenu (3);
        canvas.SwitchSubPanels (projectPage.gameObject);
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void SetCircleRelevantInfo (string pRef, string gName, string pName, string description, string fundGoal, string fundAmount, string loc, List<string> cats, List<string> gT, List<string> gV, string pictureID, string cFund, string cMember) {
        
        circleFunded = cFund;
        circleMembers = cMember;

        float goal = int.Parse (fundGoal);
        float current = int.Parse (cFund);
        float percentage = 100 / (goal / current);
        sliderval2 = percentage / 100;

        Text funded = information.transform.GetChild (7).GetChild (0).GetComponent<Text> ();
        Text members = information.transform.GetChild (7).GetChild (1).GetChild (0).GetComponent<Text> ();
        Text membersText = information.transform.GetChild (7).GetChild (1).GetComponent<Text> ();
        Slider circleSlider = information.transform.GetChild (8).GetComponent<Slider> ();

        funded.text = "$" + (int.Parse (cFund) / 100).ToString ();
        members.text = circleMembers;
        if (int.Parse (cMember) == 1) {
            membersText.text = "Member Donated";
        } else {
            membersText.text = "Members Donated";
        }
        circleSlider.value = sliderval2;

        SetInformation (pRef, gName, pName, description, fundGoal, fundAmount, loc, cats, gT, gV, pictureID);
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//
}
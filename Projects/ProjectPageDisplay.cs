using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProjectPageDisplay : MonoBehaviour {

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    [SerializeField]
    private RectTransform canvas;
    
    [SerializeField]
    private DiscoverManager discover;

    [SerializeField]
    private RectTransform content;
    [SerializeField]
    private RectTransform aboutBox;
    [SerializeField]
    private RectTransform goalsBox;

    [SerializeField]
    private GameObject goalPrefab;

    public Text title;
    public Text group;
    public Text about;
    public Text fundingNeeded;
    public Text fundingPercentage;
    public Slider slider;
    public Button donateButton;
    public Image image;

    public int fundGoal;

    public string projectReference;
    public int currentBalance;

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    private void Start () {

        image.GetComponent<RectTransform> ().sizeDelta = new Vector2 (canvas.sizeDelta.x, (canvas.sizeDelta.x / 2.618f));
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    private void Update () {
        
        if (currentBalance == fundGoal) {
            donateButton.interactable = false;
            donateButton.transform.GetChild (0).GetComponent<Text> ().text = "This project is fully funded!";
        } else {
            donateButton.interactable = true;
            donateButton.transform.GetChild (0).GetComponent<Text> ().text = "Donate to this project";
        }

        content.sizeDelta = new Vector2 (content.sizeDelta.x, (855 + aboutBox.sizeDelta.y + goalsBox.transform.parent.transform.GetComponent<RectTransform> ().sizeDelta.y));
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void RecieveInformation (string _title, string _group, string _about, string _fundAmount, string _fundGoal, List<string> gT, List<string> gV, string _picture) {

        title.text = _title;
        group.text = _group;
        about.text = _about;
        print (_picture);
        Sprite img = Resources.Load<Sprite> ("2D/Projects/" + _picture);
        image.sprite = img;
        image.transform.GetComponent<ScaleImageToCircle> ().Scale ();
        fundGoal = int.Parse (_fundGoal);
        int fAmount = int.Parse (_fundAmount);
        float percentage;
        if (fAmount == 0) {
            percentage = 0;
        } else {
            percentage = 100 / (fundGoal / fAmount);
        }
        currentBalance = fAmount;
        fundingPercentage.text = (Mathf.RoundToInt (percentage)).ToString () + "%";
        fundingNeeded.text = "$" + ((fundGoal - fAmount) / 100).ToString ();
        if (percentage == 0) {
            slider.value = 0;
        } else {
            slider.value = percentage / 100;
        }

        foreach (Transform goal in goalsBox) {
            Destroy (goal.gameObject);
        }

        float goalsSize = 0;

        for (int i = 0; i < gT.Count; i++) {
            GameObject goal = Instantiate (goalPrefab, Vector3.zero, Quaternion.identity) as GameObject;
            goal.transform.SetParent (goalsBox.transform);
            goal.transform.SetAsLastSibling ();
            goal.transform.localScale = new Vector3 (1, 1, 1);
            goal.transform.GetComponent<RectTransform> ().sizeDelta = new Vector2 (goalsBox.sizeDelta.x, 75);
            goal.transform.GetComponent<RectTransform> ().anchoredPosition = new Vector3 (0, -(75 * i), 0);
            goal.transform.GetChild (0).GetChild (0).GetComponent<Text> ().text = gV[i];
            goal.transform.GetChild (0).GetChild (0).GetChild (0).GetComponent<Text> ().text = gT[i];
            goalsSize += 75;
            goalsBox.sizeDelta = new Vector2 (goalsBox.sizeDelta.x, goalsSize + 15);
            goalsBox.transform.parent.transform.GetComponent<RectTransform>().sizeDelta = new Vector2 (goalsBox.transform.parent.transform.GetComponent<RectTransform> ().sizeDelta.x, goalsSize + 95);
        }

        projectReference = "(" + _group + ") " + _title;
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void UpdateDisplay (int newBalance) {

        float percentage;
        if (newBalance == 0) {
            percentage = 0;
        } else {
            percentage = 100 / (fundGoal / newBalance);
        }
        currentBalance = newBalance;
        fundingPercentage.text = (Mathf.RoundToInt (percentage)).ToString () + "%";
        fundingNeeded.text = "$" + ((fundGoal - newBalance) / 100).ToString ();
        if (percentage == 0) {
            slider.value = 0;
        } else {
            slider.value = percentage / 100;
        }

        // Refresh discover list
        // discover.Search ();
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//
}
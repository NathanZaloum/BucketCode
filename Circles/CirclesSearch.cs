using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Firebase;
using Firebase.Database;

public class CirclesSearch : MonoBehaviour {

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    [SerializeField]
    private CirclesManager circlesManager;
    [SerializeField]
    private RectTransform content;
    [SerializeField]
    private GameObject circlePrefab;
    [SerializeField]
    private GameObject message;
    [SerializeField]
    private Transform filterBar;
    [SerializeField]
    private Image cover;
    [SerializeField]
    private GameObject circlePreview;

    private string filterType = "Alphabetical";
    private bool filterStatus = false;
    private bool circleOpen = false;

    private string activeCircle;

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    private void Update () {

        if (content.childCount == 0) {
            message.SetActive (true);
        } else {
            message.SetActive (false);
        }
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void PopulateList () {

        foreach (Transform child in content) {
            Destroy (child.gameObject);
        }

        DataRef.AllCirlces ().GetValueAsync ().ContinueWith (async (task) => {
            await new WaitForUpdate ();
            DataSnapshot snapshot = task.Result;

            foreach (DataSnapshot circle in snapshot.Children) {
                InstantiateCircle (circle.Child ("Name").Value.ToString (), circle.Child ("PictureID").Value.ToString (), int.Parse (circle.Child ("TotalMembers").Value.ToString ()));
            }

            OrganiseList ();
        });
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    private void InstantiateCircle (string cirlceName, string pictureID, int members) {

        GameObject circle = Instantiate (circlePrefab, Vector3.zero, Quaternion.identity) as GameObject;
        circle.transform.SetParent (content);
        circle.transform.localScale = new Vector3 (1, 1, 1);
        circle.name = cirlceName;
        circle.transform.GetChild (0).GetComponent<Text> ().text = cirlceName;
        Sprite img = Resources.Load<Sprite> ("2D/Groups/" + pictureID);
        circle.transform.GetChild (1).GetChild (0).GetChild (0).GetComponent<Image> ().sprite = img;
        circle.transform.GetComponent<UserScore> ().totalMembers = members;
        circle.transform.GetChild (2).GetComponent<Text> ().text = members.ToString ();
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    private void OrganiseList () {

        List<Transform> circles = new List<Transform> ();
        foreach (Transform circle in content) {
            circles.Add (circle);
        }

        if (filterType == "Alphabetical") {
            circles = circles.OrderBy (x => x.name).ToList ();
        } else if (filterType == "Most Members") {
            circles = circles.OrderByDescending (x => x.GetComponent<UserScore> ().totalMembers).ToList ();
        } else if (filterType == "Fewest Members") {
            circles = circles.OrderBy (x => x.GetComponent<UserScore> ().totalMembers).ToList ();
        }

        for (int i = 0; i < content.transform.childCount; i++) {
            circles[i].SetSiblingIndex (i);
        }
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void SwitchFilter (string type) {

        filterType = type;
        ToggleFilterMenu ();
        OrganiseList ();
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void ToggleButtons (Button active) {

        foreach (Transform button in filterBar.GetChild (1)) {
            button.GetComponent<Button>().interactable = true;
        }
        active.interactable = false;
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void ToggleFilterMenu () {

        filterStatus = !filterStatus;

        if (filterStatus == true) {
            filterBar.GetChild (0).GetChild (0).GetChild (0).GetComponent<RectTransform> ().localRotation = Quaternion.Euler (0.0f, 0.0f, 90.0f);
            cover.raycastTarget = true;
            StartCoroutine (MoveToPosition (new Vector3 (0.0f, -325.0f, 0.0f), new Color (0.0f, 0.0f, 0.0f, 0.784f), 0.2f));
        } else if (filterStatus == false) {
            filterBar.GetChild (0).GetChild (0).GetChild (0).GetComponent<RectTransform> ().localRotation = Quaternion.Euler (0.0f, 0.0f, -90.0f);
            cover.raycastTarget = false;
            StartCoroutine (MoveToPosition (new Vector3 (0.0f, -100.0f, 0.0f), new Color (0.0f, 0.0f, 0.0f, 0.0f), 0.2f));
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

    private IEnumerator MoveToPosition (Vector3 targetPosition, Color targetAlpha, float timeToMove) {

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

    public void ToggleCirclePreview () {

        circleOpen = !circleOpen;

        if (circleOpen == true) {
            CheckMembership ();
            SetPreviewInformation ();
        } else if (circleOpen == false) {
            circlePreview.SetActive (false);
            activeCircle = "";
        }
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void LinkCircleReference (string reference) {

        activeCircle = reference;
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void JoinCircle () {

        circlesManager.JoinCircle (activeCircle);

        PopulateList ();
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    private void SetPreviewInformation () {

        DataRef.Circles (activeCircle).GetValueAsync ().ContinueWith (async (task) => {
            await new WaitForUpdate ();
            DataSnapshot snapshot = task.Result;

            circlePreview.transform.GetChild (1).GetChild (0).GetChild (0).GetComponent<Text> ().text = snapshot.Child ("Name").Value.ToString ();
            Sprite img = Resources.Load<Sprite> ("2D/Groups/" + snapshot.Child ("SecondaryPictureID").Value.ToString ());
            circlePreview.transform.GetChild (1).GetChild (1).GetChild (0).GetChild (0).GetComponent<Image> ().sprite = img;
            circlePreview.transform.GetChild (1).GetChild (1).GetChild (0).GetChild (0).GetComponent<ScaleImageToCircle> ().Scale ();
            circlePreview.transform.GetChild (1).GetChild (2).GetChild (1).GetComponent<Text> ().text = snapshot.Child ("About").Value.ToString ();

            circlePreview.SetActive (true);
        });
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    private void CheckMembership () {

        DataRef.CurrentUser ().GetValueAsync ().ContinueWith (async (task) => {
            await new WaitForUpdate ();
            DataSnapshot snapshot = task.Result;

            if (snapshot.Child ("Circles").HasChild (activeCircle)) {
                circlePreview.transform.GetChild (1).GetChild (3).gameObject.SetActive (false);
                circlePreview.transform.GetChild (1).GetChild (4).gameObject.SetActive (true);
            } else {

                circlePreview.transform.GetChild (1).GetChild (3).gameObject.SetActive (true);
                circlePreview.transform.GetChild (1).GetChild (4).gameObject.SetActive (false);
            }
        });
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//
}
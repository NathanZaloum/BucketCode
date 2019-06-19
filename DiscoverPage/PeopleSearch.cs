using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Firebase;
using Firebase.Database;

public class PeopleSearch : MonoBehaviour {

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    [SerializeField]
    private RectTransform content;
    [SerializeField]
    private GameObject userPrefab;
    [SerializeField]
    private GameObject message;
    [SerializeField]
    private Transform filterBar;
    [SerializeField]
    private Image cover;

    private string filterType = "Alphabetical";
    private bool filterStatus = false;

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

        DataRef.AllUsers ().GetValueAsync ().ContinueWith (async (task) => {
            await new WaitForUpdate ();
            DataSnapshot snapshot = task.Result;

            foreach (DataSnapshot user in snapshot.Children) {
                InstantiateUser (user.Child ("Username").Value.ToString(), user.Child ("PictureID").Value.ToString (), int.Parse (user.Child ("DonationTotal").Value.ToString ()));
            }

            OrganiseList ();
        });
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    private void InstantiateUser (string userName, string pictureID, int donations) {

        GameObject user = Instantiate (userPrefab, Vector3.zero, Quaternion.identity) as GameObject;
        user.transform.SetParent (content);
        user.transform.localScale = new Vector3 (1, 1, 1);
        user.name = userName;
        user.transform.GetChild (0).GetComponent<Text> ().text = userName;
        Sprite img = Resources.Load<Sprite> ("2D/Animals/" + pictureID);
        user.transform.GetChild (1).GetChild (0).GetChild (0).GetComponent<Image> ().sprite = img;
        user.transform.GetComponent<UserScore> ().totalDonated = donations / 100;
        user.transform.GetChild (2).GetComponent<Text> ().text = "$" + (donations / 100).ToString ();
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    private void OrganiseList () {

        List<Transform> users = new List<Transform> ();
        foreach (Transform user in content) {
            users.Add (user);
        }

        if (filterType == "Alphabetical") {
            users = users.OrderBy (x => x.name).ToList ();
        } else if (filterType == "Donations") {
            users = users.OrderByDescending (x => x.GetComponent<UserScore> ().totalDonated).ToList ();
        }

        for (int i = 0; i < content.transform.childCount; i++) {
            users[i].SetSiblingIndex (i);
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
            button.GetComponent<Button> ().interactable = true;
        }
        active.interactable = false;
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void ToggleFilterMenu () {

        filterStatus = !filterStatus;

        if (filterStatus == true) {
            filterBar.GetChild (0).GetChild (0).GetChild (0).GetComponent<RectTransform> ().localRotation = Quaternion.Euler (0.0f, 0.0f, 90.0f);
            cover.raycastTarget = true;
            StartCoroutine (MoveToPosition (new Vector3 (0.0f, -250.0f, 0.0f), new Color (0.0f, 0.0f, 0.0f, 0.784f), 0.2f));
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
}
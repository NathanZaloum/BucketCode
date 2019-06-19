using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Firebase;
using Firebase.Database;

public class ProfilePictureManager : MonoBehaviour {

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    [SerializeField]
    private Image profilePicture;

    [SerializeField]
    private Transform buttons;

    [SerializeField]
    private Button defaultPicture;

    private Button selectedPicture;

    [SerializeField]
    private GameObject cover;

    [SerializeField]
    private ProjectInformation bucketTeam;

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    private void Start () {

        selectedPicture = defaultPicture;
        GetBucketTeamProject ();
	}

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void ToggleSelectedPicture (Button active) {
		
        foreach (Transform button in buttons) {
            button.GetComponent<Button>().interactable = true;
            button.transform.GetChild (1).gameObject.SetActive (false);
        }
        active.interactable = false;
        active.transform.GetChild (1).gameObject.SetActive (true);
        selectedPicture = active;
	}

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void DisplayPicture (string pictureID) {

        Sprite image = Resources.Load <Sprite>("2D/Animals/" + pictureID);
        profilePicture.sprite = image;
        ToggleSelectedPicture (buttons.Find (pictureID).GetComponent<Button> ());
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void SetNewPictureID () {

        DataRef.CurrentUser ().Child ("PictureID").SetValueAsync (selectedPicture.gameObject.name).ContinueWith (async (task) => {
            await new WaitForUpdate ();
            if (task.IsCanceled || task.IsFaulted) {
                return;
            }
            Sprite image = Resources.Load<Sprite> ("2D/Animals/" + selectedPicture.gameObject.name);
            profilePicture.sprite = image;
        });
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void RevealProfileInfo () {

        cover.SetActive (false);
        profilePicture.color = Color.white;
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    private void GetBucketTeamProject () {

        DataRef.Projects ("(Bucket NZ) Bucket Team").GetValueAsync ().ContinueWith (async (task) => {
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

            bucketTeam.SetInformation (pRef, gName, pName, description, fundingGoal, fundingAmount, location, categories, goalTitles, goalValues, pictureID);
        });
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//
}
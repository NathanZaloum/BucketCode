using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Firebase;
using Firebase.Database;

public class AlertInfo : MonoBehaviour {

    //----------------------------------------------------------------------------------------------------------------------------------------------//
    
    // 0 = top message
    // 1 = type message
    // 2 = timestamp
    // 3 = picture
    // 4 = project link

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void AssignDonation (DataSnapshot snap) {

        DataRef.User (snap.Child ("UserID").Value.ToString ()).GetValueAsync ().ContinueWith (async (task) => {
            await new WaitForUpdate ();
            if (task.IsFaulted || task.IsCanceled) {
                return;
            }
            DataSnapshot snapshot = task.Result;

            Sprite image = Resources.Load<Sprite> ("2D/Animals/" + snapshot.Child ("PictureID").Value.ToString ());
            this.transform.GetChild (3).GetChild (0).GetChild (0).GetComponent<Image> ().sprite = image;
        });

        DataRef.Projects (snap.Child ("Reference").Value.ToString ()).GetValueAsync ().ContinueWith (async (task) => {
            await new WaitForUpdate ();
            if (task.IsFaulted || task.IsCanceled) {
                return;
            }
            DataSnapshot snapshot = task.Result;

            this.transform.GetChild (0).GetComponent<Text> ().text = snap.Child ("Donor").Value.ToString ();
            this.transform.GetChild (1).GetComponent<Text> ().text = "Donated $" + snap.Child ("Amount").Value.ToString () + " to " + snapshot.Child ("ProjectName").Value.ToString ();
            this.transform.GetChild (2).GetComponent<Text> ().text = snap.Child ("Date").Value.ToString ();

            string pName = snapshot.Child ("ProjectName").Value.ToString ();
            string gName = snapshot.Child ("Group").Value.ToString ();
            string pRef = "(" + gName + ") " + pName;
            string description = snapshot.Child ("ProjectDescription").Value.ToString ();
            string fundingGoal = snapshot.Child ("FundingGoal").Value.ToString ();
            string fundingAmount = snapshot.Child ("FundingAmount").Value.ToString ();
            string location = snapshot.Child ("Tags").Child ("Location").Value.ToString ();
            string pictureID = snapshot.Child ("PictureID").Value.ToString ();
            List<string> categories = new List<string> ();
            foreach (DataSnapshot s1 in snapshot.Child ("Tags").Child ("Categories").Children) {
                categories.Add (s1.Value.ToString ());
            }
            List<string> goalTitles = new List<string> ();
            foreach (DataSnapshot s2 in snapshot.Child ("Measurements").Children) {
                goalTitles.Add (s2.Child ("Title").Value.ToString ());
            }
            List<string> goalValues = new List<string> ();
            foreach (DataSnapshot s3 in snapshot.Child ("Measurements").Children) {
                goalValues.Add (s3.Child ("Value").Value.ToString ());
            }

            this.transform.GetChild (4).GetComponent<ProjectInformation> ().SetInformation (pRef, gName, pName, description, fundingGoal, fundingAmount, location, categories, goalTitles, goalValues, pictureID);
        });
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void AssignNewDonor (DataSnapshot snap) {

        DataRef.User (snap.Child ("UserID").Value.ToString ()).GetValueAsync ().ContinueWith (async (task) => {
            await new WaitForUpdate ();
            if (task.IsFaulted || task.IsCanceled) {
                return;
            }
            DataSnapshot snapshot = task.Result;

            Sprite image = Resources.Load<Sprite> ("2D/Animals/" + snapshot.Child ("PictureID").Value.ToString ());
            this.transform.GetChild (3).GetChild (0).GetChild (0).GetComponent<Image> ().sprite = image;
        });

        this.transform.GetChild (0).GetComponent<Text> ().text = snap.Child ("Name").Value.ToString ();
        this.transform.GetChild (2).GetComponent<Text> ().text = snap.Child ("Date").Value.ToString ();
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void AssignNewProject (DataSnapshot snap) {

        DataRef.Projects (snap.Child ("Reference").Value.ToString ()).GetValueAsync ().ContinueWith (async (task) => {
            await new WaitForUpdate ();
            if (task.IsFaulted || task.IsCanceled) {
                return;
            }
            DataSnapshot snapshot = task.Result;

            this.transform.GetChild (0).GetComponent<Text> ().text = snapshot.Child ("Group").Value.ToString ();
            this.transform.GetChild (2).GetComponent<Text> ().text = snap.Child ("Date").Value.ToString ();
            Sprite image = Resources.Load<Sprite> ("2D/Groups/Group_Default");
            this.transform.GetChild (3).GetChild (0).GetChild (0).GetComponent<Image> ().sprite = image;

            string pName = snapshot.Child ("ProjectName").Value.ToString ();
            string gName = snapshot.Child ("Group").Value.ToString ();
            string pRef = "(" + gName + ") " + pName;
            string description = snapshot.Child ("ProjectDescription").Value.ToString ();
            string fundingGoal = snapshot.Child ("FundingGoal").Value.ToString ();
            string fundingAmount = snapshot.Child ("FundingAmount").Value.ToString ();
            string location = snapshot.Child ("Tags").Child ("Location").Value.ToString ();
            string pictureID = snapshot.Child ("PictureID").Value.ToString ();
            List<string> categories = new List<string> ();
            foreach (DataSnapshot s in snapshot.Child ("Tags").Child ("Categories").Children) {
                categories.Add (s.Value.ToString ());
            }
            List<string> goalTitles = new List<string> ();
            foreach (DataSnapshot s2 in snapshot.Child ("Measurements").Children) {
                goalTitles.Add (s2.Child ("Title").Value.ToString ());
            }
            List<string> goalValues = new List<string> ();
            foreach (DataSnapshot s3 in snapshot.Child ("Measurements").Children) {
                goalValues.Add (s3.Child ("Value").Value.ToString ());
            }

            this.transform.GetChild (4).GetComponent<ProjectInformation> ().SetInformation (pRef, gName, pName, description, fundingGoal, fundingAmount, location, categories, goalTitles, goalValues, pictureID);
        });
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void AssignProjectFunded (DataSnapshot snap) {

        DataRef.Projects (snap.Child ("Reference").Value.ToString ()).GetValueAsync ().ContinueWith (async (task) => {
            await new WaitForUpdate ();
            if (task.IsFaulted || task.IsCanceled) {
                return;
            }
            DataSnapshot snapshot = task.Result;

            this.transform.GetChild (0).GetComponent<Text> ().text = snapshot.Child ("Group").Value.ToString ();
            this.transform.GetChild (2).GetComponent<Text> ().text = snap.Child ("Date").Value.ToString ();
            Sprite image = Resources.Load<Sprite> ("2D/Groups/Group_Default");
            this.transform.GetChild (3).GetChild (0).GetChild (0).GetComponent<Image> ().sprite = image;

            string pName = snapshot.Child ("ProjectName").Value.ToString ();
            string gName = snapshot.Child ("Group").Value.ToString ();
            string pRef = "(" + gName + ") " + pName;
            string description = snapshot.Child ("ProjectDescription").Value.ToString ();
            string fundingGoal = snapshot.Child ("FundingGoal").Value.ToString ();
            string fundingAmount = snapshot.Child ("FundingAmount").Value.ToString ();
            string location = snapshot.Child ("Tags").Child ("Location").Value.ToString ();
            string pictureID = snapshot.Child ("PictureID").Value.ToString ();
            List<string> categories = new List<string> ();
            foreach (DataSnapshot s in snapshot.Child ("Tags").Child ("Categories").Children) {
                categories.Add (s.Value.ToString ());
            }
            List<string> goalTitles = new List<string> ();
            foreach (DataSnapshot s2 in snapshot.Child ("Measurements").Children) {
                goalTitles.Add (s2.Child ("Title").Value.ToString ());
            }
            List<string> goalValues = new List<string> ();
            foreach (DataSnapshot s3 in snapshot.Child ("Measurements").Children) {
                goalValues.Add (s3.Child ("Value").Value.ToString ());
            }

            this.transform.GetChild (4).GetComponent<ProjectInformation> ().SetInformation (pRef, gName, pName, description, fundingGoal, fundingAmount, location, categories, goalTitles, goalValues, pictureID);
        });
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//
}
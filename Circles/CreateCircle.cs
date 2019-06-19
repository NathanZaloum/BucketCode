using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Firebase;
using Firebase.Database;

public class CreateCircle : MonoBehaviour {

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    [SerializeField]
    private UserAuthentication firebase;
    [SerializeField]
    private CirclesManager circlesManager;

    [SerializeField]
    private InputField nameInput;
    [SerializeField]
    private InputField aboutInput;
    [SerializeField]
    private Image firstImage;
    [SerializeField]
    private Image secondImage;
    [SerializeField]
    private Button button;

    [SerializeField]
    private List<Sprite> firstImages = new List<Sprite> ();
    [SerializeField]
    private List<Sprite> secondImages = new List<Sprite> ();

    private int firstCount = 0;
    private int secondCount = 0;

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    private void OnEnable () {
        
        firstImage.GetComponent<ScaleImageToCircle> ().Scale ();
        secondImage.GetComponent<ScaleImageToCircle> ().Scale ();
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    private void Update () {

        if (nameInput.transform.parent.GetComponent<TextLimitCheck>().validInput == true && aboutInput.transform.parent.GetComponent<TextLimitCheck> ().validInput == true) {
            button.interactable = true;
        } else {
            button.interactable = false;
        }
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void CreateNewCircle () {

        SetData (nameInput.text, "Name", nameInput.text);
        SetData (nameInput.text, "About", aboutInput.text);
        SetData (nameInput.text, "PictureID", firstImage.sprite.name);
        SetData (nameInput.text, "SecondaryPictureID", secondImage.sprite.name);
        SetData (nameInput.text, "TotalDonated", "0");
        SetData (nameInput.text, "TotalMembers", "1");
        AddReferenceForCreator (nameInput.text, firebase.currentUser.UserId);
        OpenCirclePage (nameInput.text);
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void ChangeFirstPicture (string direction) {

        if (direction == "Left") {
            if (firstCount == 0) {
                firstCount = firstImages.Count - 1;
            } else {
                firstCount--;
            }
        } else if (direction == "Right") {
            if (firstCount == firstImages.Count - 1) {
                firstCount = 0;
            } else {
                firstCount++;
            }
        }
        
        firstImage.sprite = firstImages[firstCount];
        firstImage.GetComponent<ScaleImageToCircle> ().Scale ();
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void ChangeSecondPicture (string direction) {

        if (direction == "Left") {
            if (secondCount == 0) {
                secondCount = secondImages.Count - 1;
            } else {
                secondCount--;
            }
        } else if (direction == "Right") {
            if (secondCount == secondImages.Count - 1) {
                secondCount = 0;
            } else {
                secondCount++;
            }
        }

        secondImage.sprite = secondImages[secondCount];
        secondImage.GetComponent<ScaleImageToCircle> ().Scale ();
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    private void AddReferenceForCreator (string circleRef, string userRef) {

        DataRef.Circles (circleRef).Child ("Members").Child (userRef).SetValueAsync (userRef).ContinueWith (async (task) => {
            await new WaitForUpdate ();
            if (task.IsFaulted || task.IsCanceled) {
                print (task.Exception);
                return;
            }
        });

        DataRef.CurrentUser ().Child ("Circles").Child (circleRef).SetValueAsync (circleRef).ContinueWith (async (task) => {
            await new WaitForUpdate ();
            if (task.IsFaulted || task.IsCanceled) {
                print (task.Exception);
                return;
            }
        });
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    private void SetData (string circleRef, string child, string value) {

        DataRef.Circles (circleRef).Child (child).SetValueAsync (value).ContinueWith (async (task) => {
            await new WaitForUpdate ();
            if (task.IsFaulted || task.IsCanceled) {
                print (task.Exception);
                return;
            }
        });
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    private void OpenCirclePage (string circleRef) {

        DataRef.CurrentUser ().Child ("Circles").GetValueAsync ().ContinueWith (async (task) => {
            await new WaitForUpdate ();
            DataSnapshot snapshot = task.Result;

            List<string> circles = new List<string> ();
            foreach (DataSnapshot child in snapshot.Children) {
                circles.Add (child.Value.ToString ());
            }

            int index = circles.IndexOf (circleRef);
            circlesManager.PopulateCircleList (index);
        });
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//
}
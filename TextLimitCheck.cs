using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Firebase;
using Firebase.Database;

public class TextLimitCheck : MonoBehaviour {

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    [SerializeField]
    private Text display;
    [SerializeField]
    private InputField input;
    [SerializeField]
    private Text alert;

    [SerializeField]
    private Color customRed, customGrey;

    private bool nameTaken = false;
    public bool validInput = false;

    private List<string> takenCircleNames = new List<string> ();

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    private void Start () {

        CheckAbout ();
        GetListOfCircles ();
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void CheckName () {

        GetListOfCircles ();

        display.text = input.text.Length.ToString () + "/" + input.characterLimit;

        if (input.text.Length == input.characterLimit || input.text.Length < 6) {
            display.color = customRed;
        } else {
            display.color = customGrey;
        }

        if (input.text.Length < 6) {
            alert.text = "Your name is too short, please make it at least 6 characters long.";
            alert.color = customRed;
            validInput = false;
        } else {
            if (takenCircleNames.Contains (input.text)) {
                alert.text = "This name is already in use by another Circle. Please choose another.";
                alert.color = customRed;
                validInput = false;
            } else {
                if (input.text.Length == input.characterLimit) {
                    alert.text = "Character limit has been reached.";
                    alert.color = customGrey;
                    validInput = true;
                } else {
                    alert.text = "";
                    validInput = true;
                }
            }
        }
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void CheckAbout () {

        display.text = input.text.Length.ToString () + "/" + input.characterLimit;

        if (input.text.Length == input.characterLimit || input.text.Length < 6) {
            display.color = customRed;
        } else {
            display.color = customGrey;
        }

        if (input.text.Length < 6) {
            alert.text = "Your description is too short, please make it at least 6 characters long.";
            alert.color = customRed;
            validInput = false;
        } else {
            if (input.text.Length == input.characterLimit) {
                alert.text = "Character limit has been reached.";
                alert.color = customGrey;
                validInput = true;
            } else {
                alert.text = "";
                validInput = true;
            }
        }
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    private void GetListOfCircles () {

        DataRef.AllCirlces ().GetValueAsync ().ContinueWith (async (task) => {
            await new WaitForUpdate ();
            DataSnapshot snapshot = task.Result;

            foreach (DataSnapshot child in snapshot.Children) {
                if (!takenCircleNames.Contains (child.Child("Name").Value.ToString ())) {
                    takenCircleNames.Add (child.Child ("Name").Value.ToString ());
                }
            }
        });
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//
}
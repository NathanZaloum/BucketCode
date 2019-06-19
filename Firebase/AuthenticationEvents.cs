using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AuthenticationEvents : MonoBehaviour {

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    // Sprite variables:
    [SerializeField]
    private Sprite tick;
    [SerializeField]
    private Sprite exclamation;

    // Color variables:
    [SerializeField]
    private Color green;
    [SerializeField]
    private Color red;

    // GameObject variables:
    [SerializeField]
    private GameObject alertBox;

    // Text variables:
    [SerializeField]
    private Text title;
    [SerializeField]
    private Text body;

    // Image variables:
    [SerializeField]
    private Image image;

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void UserCreated () {

        // When a user is created they are also signed in which means both UserCreated and UserSignedIn events occur, keep this in mind.
        Debug.Log ("User Created Event!!!");
        Message ("User Created");
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void RegisterFailed () {
        
        Debug.Log ("Register Failed Event!!!");
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void UserSignedIn () {

        Debug.Log ("User Signed In Event!!!");
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void UserSignedOut () {

        Debug.Log ("User Signed Out Event!!!");
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void Message (string message) {
        
        body.text = message;
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void AlertBoxOn (bool val) {

        alertBox.SetActive (val);
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void AlertBoxState (string state) {

        if (state == "Warning!") {
            Set (exclamation, red, state);
        } else if (state == "Success!") {
            Set (tick, green, state);
        }
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    private void Set (Sprite s, Color c, string m) {
        
        image.sprite = s;
        image.color = c;
        title.text = m;
        title.color = c;
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//
}
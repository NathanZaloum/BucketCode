using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugManager : MonoBehaviour {

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    [SerializeField]
    private UserAuthentication auth;

    [SerializeField]
    private Text userField;
    [SerializeField]
    private Text firebaseField;

    public string message = "Waiting...";

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    private void Update () {

        userField.text = "Current User: " + auth.currentUser.UserId;
        firebaseField.text = "Firebase Status: " + message;
	}

    //----------------------------------------------------------------------------------------------------------------------------------------------//
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserScore : MonoBehaviour {

    //----------------------------------------------------------------------------------------------------------------------------------------------//
    
    private CirclesSearch circlesMenu;
    
    public int totalDonated;
    public int totalMembers;

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    private void Awake () {

        if (totalMembers > 0) {
            circlesMenu = GameObject.Find ("CirclesMenu").transform.GetComponent<CirclesSearch> ();
        }
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void OpenPreview () {

        GameObject.Find ("CirclesMenu").transform.GetComponent<CirclesSearch> ().LinkCircleReference (this.gameObject.name);
        GameObject.Find ("CirclesMenu").transform.GetComponent<CirclesSearch> ().ToggleCirclePreview ();
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//
}
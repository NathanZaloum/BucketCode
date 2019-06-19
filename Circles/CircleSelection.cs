using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CircleSelection : MonoBehaviour {

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    private CirclesManager circlesManager;

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void SendTarget () {

        foreach (Transform child in transform.parent) {
            child.GetComponent<Button> ().interactable = true;
        }
        transform.GetComponent<Button> ().interactable = false;

        circlesManager = GameObject.Find ("CirclesPanel").transform.GetComponent<CirclesManager> ();
        circlesManager.PickCircle (this.gameObject.name);
        circlesManager.ToggleCircleBox ();
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//
}
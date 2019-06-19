using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Exclamation : MonoBehaviour {

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    void Update () {
		
        if (transform.parent.GetComponent<InputField>().text == "") {
            GetComponent<Image> ().enabled = true;
        } else {
            GetComponent<Image> ().enabled = false;
        }
	}

    //----------------------------------------------------------------------------------------------------------------------------------------------//
}
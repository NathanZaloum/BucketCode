using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorFade : MonoBehaviour {

    //----------------------------------------------------------------------------------------------------------------------------------------------//
    
    //private float speed = 1.5f;

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void SetAlertColor (Color color) {

        transform.GetComponent<Image> ().color = color;

        foreach (Transform t in transform) {
            if (t.GetComponent<Image> () != null) {
                t.GetComponent<Image> ().color = color;
            }
            if (t.GetComponent<Text> () != null) {
                t.GetComponent<Text> ().color = color;
            }
        }
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    private void Update () {

        /*foreach (Transform t in transform) {
            if (t.GetComponent<Image> () != null) {
                t.GetComponent<Image> ().color = Color.Lerp (t.GetComponent<Image> ().color, fadeToColor, Time.deltaTime * speed);
            }
            if (t.GetComponent<Text> () != null) {
                t.GetComponent<Text> ().color = Color.Lerp (t.GetComponent<Text> ().color, fadeToColor, Time.deltaTime * speed);
            }
        }*/
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//
}
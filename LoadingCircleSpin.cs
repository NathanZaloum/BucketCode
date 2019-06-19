using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingCircleSpin : MonoBehaviour {

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    private float speed = 300.0f;
    private bool fast = false;

    private RectTransform rect;
    private Image img;

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    private void Start () {

        rect = this.gameObject.transform.GetChild (0).GetComponent<RectTransform> ();
        img = this.gameObject.transform.GetChild (0).GetComponent<Image> ();

        StartCoroutine (SpeedAdjust ());
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//
    
    private void Update () {

        if (fast) {
            speed = Mathf.Lerp (speed, 500.0f, Time.deltaTime * 25.0f);
            img.fillAmount = Mathf.Lerp (img.fillAmount, 0.1f, 25.0f * Time.deltaTime);
        } else {
            speed = Mathf.Lerp (speed, 250.0f, Time.deltaTime * 25.0f);
            img.fillAmount = Mathf.Lerp (img.fillAmount, 0.15f, 25.0f * Time.deltaTime);
        }

        rect.Rotate (0, 0, speed * Time.deltaTime);
	}

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    private IEnumerator SpeedAdjust () {
        
        fast = false;
        yield return new WaitForSeconds (1.0f);
        fast = true;
        yield return new WaitForSeconds (0.3f);
        StartCoroutine (SpeedAdjust ());
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//
}
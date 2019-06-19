using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Firebase;
using Firebase.Database;

public class Onboarder : MonoBehaviour {

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    [SerializeField]
    private RectTransform canvas;
    private float canvasX;
    private float canvasY;

    [SerializeField]
    private RectTransform panel;
    
    private int currentPage = 1;

    private float speed = 5.0f;

    [SerializeField]
    private Image dot1, dot2, dot3;

    [SerializeField]
    private Color grey, green;

    [SerializeField]
    private Transform page1, page2, page3;

    [SerializeField]
    private GameObject nextButton, continueButton;

    private bool closed = false;

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    private void Awake () {
        
        canvasX = canvas.sizeDelta.x;
        canvasY = canvas.sizeDelta.y;
        this.gameObject.transform.GetComponent<RectTransform> ().sizeDelta = new Vector2 (canvasX, canvasY);
        
        if (this.gameObject.name == "OnboardingFirstOpen") {
            LoadOnboard ();
        } else if (this.gameObject.name == "OnboardingFirstUser") {
            GetOnboardStatus ();
        }
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    private void Update () {

        if (currentPage == 1) {
            Fade (page1, 1);
            Fade (page2, 0);
            Fade (page3, 0);
        } else if (currentPage == 2) {
            Fade (page1, 0);
            Fade (page2, 1);
            Fade (page3, 0);
        } else if (currentPage == 3) {
            Fade (page1, 0);
            Fade (page2, 0);
            Fade (page3, 1);
        }

        if (closed) {
            RectTransform rect = this.gameObject.transform.GetComponent<RectTransform> ();
            rect.anchoredPosition = Vector3.MoveTowards (rect.anchoredPosition, new Vector3 (0, canvasY * 1.5f, 0), 50.0f);
            panel.gameObject.SetActive (true);
        } else {
            panel.gameObject.SetActive (false);
        }
	}

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void NextPage (string type) {
        
        if (type == "First Device") {
            if (currentPage == 1) {
                currentPage = 2;
                SetDot (dot2);
            } else if (currentPage == 2) {
                currentPage = 3;
                SetDot (dot3);
            } else if (currentPage == 3) {
                currentPage = 1;
                SetDot (dot1);
            }
        } else if (type == "New User") {
            if (currentPage == 1) {
                currentPage = 2;
                SetDot (dot2);
            } else if (currentPage == 2) {
                currentPage = 3;
                SetDot (dot3);
                // hide next button and reveal continue button
                //nextButton.gameObject.SetActive (false);
                //continueButton.gameObject.SetActive (true);
            }
        }
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    private void SetDot (Image dot) {

        dot1.color = grey;
        dot2.color = grey;
        dot3.color = grey;

        dot.color = green;
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    private void Fade (Transform page, int alpha) {

        for (int i = 0; i < page.childCount; i++) {
            if (page.GetChild (i).GetComponent<Image>() != null) {
                Image img = page.GetChild (i).GetComponent<Image> ();
                img.color = Color.Lerp (img.color, new Color (img.color.r, img.color.g, img.color.b, alpha), Time.deltaTime * speed);
            } else if (page.GetChild (i).GetComponent<Text> () != null) {
                Text txt = page.GetChild (i).GetComponent<Text> ();
                txt.color = Color.Lerp (txt.color, new Color (txt.color.r, txt.color.g, txt.color.b, alpha), Time.deltaTime * speed);
            }
        }
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void CloseOnboarder (string menu) {

        if (menu == "Sign Up") {
            panel.anchoredPosition = new Vector3 (canvasX, 0, 0);
            canvas.GetComponent<LoginRegisterUI> ().SwitchMenu (1);
        } else if (menu == "Log In") {
            panel.anchoredPosition = new Vector3 (0, 0, 0);
        }
        closed = true;
        SaveOnboard ();
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void GetOnboardStatus () {
        
        DataRef.CurrentUser ().Child ("Onboarded").Child ("Bucket Circle New Zealand").GetValueAsync ().ContinueWith (async (task) => {
            await new WaitForUpdate ();
            if (task.IsCanceled || task.IsFaulted) {
                return;
            }
            DataSnapshot snapshot = task.Result;
            if (snapshot.Value.ToString () == "true") {
                this.gameObject.SetActive (false);
            } else {
                this.gameObject.SetActive (true);
            }
        });
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void SetOnboardStatus() {

        DataRef.CurrentUser ().Child ("Onboarded").Child ("Bucket Circle New Zealand").SetValueAsync ("true").ContinueWith (async (task) => {
            await new WaitForUpdate ();
            if (task.IsCanceled || task.IsFaulted) {
                return;
            }
            this.gameObject.SetActive (false);
        });
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    private void SaveOnboard () {

        BinaryFormatter bf = new BinaryFormatter ();
        FileStream file = File.Create (Application.persistentDataPath + "/" + this.gameObject.name + ".dat");

        OnboardConfirmation info = new OnboardConfirmation ();
        info.onboarded = true;

        bf.Serialize (file, info);
        file.Close ();
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    private void LoadOnboard () {

        if (File.Exists (Application.persistentDataPath + "/" + this.gameObject.name + ".dat")) {

            BinaryFormatter bf = new BinaryFormatter ();
            FileStream file = File.Open (Application.persistentDataPath + "/" + this.gameObject.name + ".dat", FileMode.Open);

            OnboardConfirmation info = (OnboardConfirmation)bf.Deserialize (file);
            file.Close ();

            if (info.onboarded == true) {
                // Close Onboard Screen
                this.gameObject.SetActive (false);
            } else {
                // Open Onboard Screen
                this.gameObject.SetActive (true);
            }
        } else {
            // Open Onboard Screen
            this.gameObject.SetActive (true);
        }
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//
}
[Serializable]
class OnboardConfirmation {

    public bool onboarded;
}
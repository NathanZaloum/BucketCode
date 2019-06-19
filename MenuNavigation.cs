using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuNavigation : MonoBehaviour {

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    [SerializeField]
    private Color green;
    [SerializeField]
    private Color grey;
    [SerializeField]
    private Color pink;

    private Color transitionColor;

    [SerializeField]
    private RectTransform panels;
    [SerializeField]
    private RectTransform subPanels;
    [SerializeField]
    private RectTransform buttons;

    [SerializeField]
    private RectTransform doerPanels;
    [SerializeField]
    private RectTransform doerSubPanels;
    [SerializeField]
    private RectTransform doerBottom;

    [SerializeField]
    private GameObject projectPanel;
    [SerializeField]
    private GameObject editPanel;
    [SerializeField]
    private GameObject aboutPanel;
    [SerializeField]
    private GameObject autoPaymentPanel;
    [SerializeField]
    private GameObject feedbackFormPanel;
    [SerializeField]
    private GameObject createCirclePanel;

    [SerializeField]
    private GameObject comingSoon;

    private float canvasX;
    private float canvasY;

    private int currentMenu = 3;
    private int currentSub = 0;
    private int currentDoer = 3;

    private bool subOn = false;
    private float subX;
    private float subY;

    private float duration = 0.2f;

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    private void Start () {

        SetScreenSize ();
        SetButtons ();

        panels.GetComponent<RectTransform> ().anchoredPosition = Vector3.zero;

        SetCurrentMenu (3);
        SetDoerMenu (3);
        SetSubMenu (0);
        SwitchDoer (false);
        ToggleDoerSub (false);

        panels.GetComponent<RectTransform> ().sizeDelta = new Vector2 ((canvasX * 5), canvasY);
        subPanels.GetComponent<RectTransform> ().sizeDelta = new Vector2 ((canvasX * 3), canvasY);
        doerPanels.GetComponent<RectTransform> ().sizeDelta = new Vector2 (canvasX, canvasY);
        doerSubPanels.GetComponent<RectTransform> ().sizeDelta = new Vector2 (canvasX, canvasY);
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    private void Update () {

        if (doerPanels.anchoredPosition.y == 0) {
            doerSubPanels.gameObject.SetActive (true);
        } else {
            doerSubPanels.gameObject.SetActive (false);
        }
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    private IEnumerator MoveToPosition(Transform target, Vector3 targetPosition, float timeToMove) {

        Vector3 currentPos = target.GetComponent<RectTransform> ().anchoredPosition;
        float t = 0.0f;

        while (t < 1) {
            t += Time.deltaTime / duration;
            target.GetComponent<RectTransform> ().anchoredPosition = Vector3.Lerp (currentPos, targetPosition, t);
            yield return null;
        }
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    private void SetScreenSize () {

        canvasX = gameObject.transform.GetComponent<RectTransform> ().sizeDelta.x;
        canvasY = gameObject.transform.GetComponent<RectTransform> ().sizeDelta.y;

        for (int i = 0; i < panels.childCount; i++) {
            panels.GetChild(i).GetComponent<RectTransform>().sizeDelta = new Vector2 (canvasX, canvasY);
            panels.GetChild (i).GetComponent<RectTransform> ().anchoredPosition = new Vector2 (-(canvasX * 2) + (canvasX * i), 0);
        }

        for (int i = 0; i < 3; i++) {
            subPanels.GetChild (i).GetComponent<RectTransform> ().sizeDelta = new Vector2 (canvasX, canvasY);
            subPanels.GetChild (i).GetComponent<RectTransform> ().anchoredPosition = new Vector2 (-canvasX + (canvasX * i), 0);
        }

        for (int i = 3; i < subPanels.childCount; i++) {
            subPanels.GetChild (i).GetComponent<RectTransform> ().sizeDelta = new Vector2 (canvasX, canvasY);
            subPanels.GetChild (i).GetComponent<RectTransform> ().anchoredPosition = new Vector2 (0, 0);
        }

        for (int i = 0; i < doerPanels.childCount; i++) {
            doerPanels.GetChild (i).GetComponent<RectTransform> ().sizeDelta = new Vector2 (canvasX, canvasY);
            doerPanels.GetChild (i).GetComponent<RectTransform> ().anchoredPosition = new Vector2 (-(canvasX * 2) + (canvasX * i), 0);
        }

        comingSoon.GetComponent<RectTransform> ().sizeDelta = new Vector2 (canvasX, canvasY);
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void SetCurrentMenu (int position) {
        
        currentMenu = position;
        SetButtons ();
        if (position == 1) {
            StartCoroutine (MoveToPosition (panels, new Vector3 (canvasX * 2.0f, 0.0f, 0.0f), 1.0f));
        } else if (position == 2) {
            StartCoroutine (MoveToPosition (panels, new Vector3 (canvasX, 0.0f, 0.0f), 1.0f));
        } else if (position == 3) {
            StartCoroutine (MoveToPosition (panels, new Vector3 (0.0f, 0.0f, 0.0f), 1.0f));
        } else if (position == 4) {
            StartCoroutine (MoveToPosition (panels, new Vector3 (-canvasX, 0.0f, 0.0f), 1.0f));
        } else if (position == 5) {
            StartCoroutine (MoveToPosition (panels, new Vector3 (-(canvasX * 2.0f), 0.0f, 0.0f), 1.0f));
        }
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void SetDoerMenu (int position) {
        
        currentDoer = position;
        SetDoerButtons ();
        if (position == 1) {
            StartCoroutine (MoveToPosition (doerPanels, new Vector3 ((canvasX * 2.0f), 0.0f, 0.0f), 1.0f));
        } else if (position == 2) {
            StartCoroutine (MoveToPosition (doerPanels, new Vector3 (canvasX, 0.0f, 0.0f), 1.0f));
        } else if (position == 3) {
            StartCoroutine (MoveToPosition (doerPanels, new Vector3 (0.0f, 0.0f, 0.0f), 1.0f));
        }
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void SetSubMenu (int position) {

        if (position != 0) {
            if (position == 1) {
                StartCoroutine (MoveToPosition (subPanels, new Vector3 (canvasX, 0.0f, 0.0f), 1.0f));
            } else if (position == 2) {
                StartCoroutine (MoveToPosition (subPanels, new Vector3 (0.0f, 0.0f, 0.0f), 1.0f));
            } else if (position == 3) {
                StartCoroutine (MoveToPosition (subPanels, new Vector3 (-canvasX, 0.0f, 0.0f), 1.0f));
            }
        } else {
            StartCoroutine (MoveToPosition (subPanels, new Vector3 (0.0f, canvasY, 0.0f), 1.0f));
        }
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void SnapSubMenu (int position) {

        if (position == 1) {
            subPanels.anchoredPosition = new Vector3 (canvasX, 0.0f, 0.0f);
        } else if (position == 2) {
            subPanels.anchoredPosition = new Vector3 (0.0f, 0.0f, 0.0f);
        } else if (position == 3) {
            subPanels.anchoredPosition = new Vector3 (-canvasX, 0.0f, 0.0f);
        }
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    private void SetButtons () {

        foreach (Transform button in buttons) {
            button.GetComponent<Button> ().interactable = true;
            button.GetChild (0).GetComponent<Image> ().color = grey;
            button.GetChild (1).GetComponent<Text> ().color = grey;
        }
        
        buttons.GetChild (currentMenu - 1).GetComponent<Button> ().interactable = false;
        buttons.GetChild (currentMenu - 1).GetChild (0).GetComponent<Image> ().color = green;
        buttons.GetChild (currentMenu - 1).GetChild (1).GetComponent<Text> ().color = green;
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    private void SetDoerButtons () {

        foreach (Transform button in doerBottom) {
            button.GetComponent<Button> ().interactable = true;
            button.GetChild (0).GetComponent<Image> ().color = grey;
            button.GetChild (1).GetComponent<Text> ().color = grey;
        }

        doerBottom.GetChild (currentDoer - 1).GetComponent<Button> ().interactable = false;
        doerBottom.GetChild (currentDoer - 1).GetChild (0).GetComponent<Image> ().color = green;
        doerBottom.GetChild (currentDoer - 1).GetChild (1).GetComponent<Text> ().color = green;
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void SwitchSubPanels (GameObject panel) {

        projectPanel.SetActive (false);
        editPanel.SetActive (false);
        aboutPanel.SetActive (false);
        autoPaymentPanel.SetActive (false);
        feedbackFormPanel.SetActive (false);
        createCirclePanel.SetActive (false);

        panel.SetActive (true);
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void SwitchDoer (bool on) {

        if (on) {
            StartCoroutine (MoveToPosition (doerBottom, new Vector3 (0.0f, 0.0f, 0.0f), 1.0f));
            StartCoroutine (MoveToPosition (doerPanels, new Vector3 (0.0f, 0.0f, 0.0f), 1.0f));
            StartCoroutine (MoveToPosition (comingSoon.transform.GetComponent<RectTransform>(), new Vector3 (0.0f, 0.0f, 0.0f), 1.0f));
        } else {
            StartCoroutine (MoveToPosition (doerBottom, new Vector3 (0.0f, -canvasY, 0.0f), 1.0f));
            StartCoroutine (MoveToPosition (doerPanels, new Vector3 (0.0f, -canvasY, 0.0f), 1.0f));
            StartCoroutine (MoveToPosition (comingSoon.transform.GetComponent<RectTransform> (), new Vector3 (0.0f, -canvasY, 0.0f), 1.0f));
        }
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void SetComingSoon (string permission) {

        if (permission == "Default") {
            comingSoon.SetActive (true);
        } else if (permission == "Admin") {
            comingSoon.SetActive (false);
        }
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void ToggleDoerSub (bool on) {

        if (on) {
            StartCoroutine (MoveToPosition (doerSubPanels, new Vector3 (0.0f, 0.0f, 0.0f), 1.0f));
        } else {
            StartCoroutine (MoveToPosition (doerSubPanels, new Vector3 (0.0f, -canvasY, 0.0f), 1.0f));
        }
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void SwitchDoerSubPanels (GameObject panel) {

        foreach (Transform p in doerSubPanels.gameObject.transform) {
            p.gameObject.SetActive (false);
        }

        panel.SetActive (true);
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//
}
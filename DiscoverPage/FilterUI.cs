using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FilterUI : MonoBehaviour {

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    [SerializeField]
    private DiscoverManager discoverManager;
    
    [SerializeField]
    private Transform filterBar;
    [SerializeField]
    private Image cover;

    private bool filterStatus = false;
    private string filterType = "Alphabetical";

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void SwitchFilter (string type) {

        filterType = type;
        ToggleFilterMenu ();
        discoverManager.OrganiseResults (filterType);
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void ToggleButtons (Button active) {

        filterBar.GetChild (1).GetChild (0).GetComponent<Button> ().interactable = true;
        filterBar.GetChild (1).GetChild (1).GetComponent<Button> ().interactable = true;
        filterBar.GetChild (1).GetChild (2).GetComponent<Button> ().interactable = true;
        filterBar.GetChild (1).GetChild (3).GetComponent<Button> ().interactable = true;
        filterBar.GetChild (1).GetChild (4).GetComponent<Button> ().interactable = true;
        active.interactable = false;
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void ToggleFilterMenu () {

        filterStatus = !filterStatus;

        if (filterStatus == true) {
            filterBar.GetChild (0).GetChild (0).GetChild (0).GetComponent<RectTransform> ().localRotation = Quaternion.Euler (0.0f, 0.0f, 90.0f);
            cover.raycastTarget = true;
            StartCoroutine (MoveToPosition (new Vector3 (0.0f, -635.0f, 0.0f), new Color (0.0f, 0.0f, 0.0f, 0.784f), 0.2f));
        } else if (filterStatus == false) {
            filterBar.GetChild (0).GetChild (0).GetChild (0).GetComponent<RectTransform> ().localRotation = Quaternion.Euler (0.0f, 0.0f, -90.0f);
            cover.raycastTarget = false;
            StartCoroutine (MoveToPosition (new Vector3 (0.0f, -100.0f, 0.0f), new Color (0.0f, 0.0f, 0.0f, 0.0f), 0.2f));
        }
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void CloseFilterMenu () {

        filterStatus = false;
        
        filterBar.GetChild (0).GetChild (0).GetChild (0).GetComponent<RectTransform> ().localRotation = Quaternion.Euler (0.0f, 0.0f, -90.0f);
        cover.raycastTarget = false;
        filterBar.GetComponent<RectTransform> ().anchoredPosition = new Vector3 (0.0f, -100.0f, 0.0f);
        cover.color = new Color (0.0f, 0.0f, 0.0f, 0.0f);
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    private IEnumerator MoveToPosition (Vector3 targetPosition, Color targetAlpha, float timeToMove) {

        Vector3 currentPos = filterBar.GetComponent<RectTransform> ().anchoredPosition;
        Color currentAlpha = cover.color;
        float t = 0.0f;

        while (t < 1) {
            t += Time.deltaTime / timeToMove;
            filterBar.GetComponent<RectTransform> ().anchoredPosition = Vector3.Lerp (currentPos, targetPosition, t);
            cover.color = Color.Lerp (currentAlpha, targetAlpha, t);
            yield return null;
        }
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void OpenList (GameObject list) {

        list.SetActive (true);
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void CloseList (GameObject list) {

        list.SetActive (false);
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//
}
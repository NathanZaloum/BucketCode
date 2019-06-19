using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CircleDragUI : MonoBehaviour {

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    [SerializeField]
    private RectTransform projectListContent;
    [SerializeField]
    private RectTransform circleInfo;
    [SerializeField]
    private Image coverMain, coverSub;

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    private void Update () {
		
        if (projectListContent.anchoredPosition.y <= 500.0f) {
            circleInfo.anchoredPosition = projectListContent.anchoredPosition;
        } else {
            circleInfo.anchoredPosition = new Vector3 (0.0f, 500.0f, 0.0f);
            ;
        }

        if (projectListContent.anchoredPosition.y <= 400.0f) {
            coverMain.color = new Color (1.0f, 1.0f, 1.0f, projectListContent.anchoredPosition.y / 400.0f);
        } else {
            coverMain.color = new Color (1.0f, 1.0f, 1.0f, 1.0f);
        }

        if (projectListContent.anchoredPosition.y >= 400.0f && projectListContent.anchoredPosition.y <= 500.0f) {
            coverSub.color = new Color (1.0f, 1.0f, 1.0f, 1.0f - ((projectListContent.anchoredPosition.y - 400.0f) / 100.0f));
        } else {
            coverSub.color = new Color (1.0f, 1.0f, 1.0f, 0.0f);
        }

        if (projectListContent.anchoredPosition.y >= 400.0f) {
            coverSub.transform.parent.SetSiblingIndex (2);
        } else {
            coverSub.transform.parent.SetSiblingIndex (1);
        }

        if (projectListContent.anchoredPosition.y >= 200.0f) {
            coverMain.raycastTarget = true;
        } else {
            coverMain.raycastTarget = false;
        }
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void FilterAction () {

        if (projectListContent.anchoredPosition.y <= 500.0f) {
            StartCoroutine (MoveToPosition (500.0f));
        }
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    private IEnumerator MoveToPosition (float targetY) {

        Vector3 currentPosition = projectListContent.anchoredPosition;

        float t = 0.0f;

        while (t < 1) {
            t += Time.deltaTime / 0.2f;
            projectListContent.anchoredPosition = Vector3.Lerp (currentPosition, new Vector3 (0.0f, targetY, 0.0f), t);
            yield return null;
        }
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//
}
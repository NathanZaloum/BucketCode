using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CountdownTimer : MonoBehaviour {

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    [SerializeField]
    private RectTransform timerBox;
    [SerializeField]
    private Image progressBar;
    [SerializeField]
    private Text daysText;
    [SerializeField]
    private Text hoursText;
    [SerializeField]
    private Text minutesText;
    [SerializeField]
    private Text secondsText;

    private bool state = false;

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    private void Update () {

        DateTime nowDate = DateTime.Now;
        int nextMonth = DateTime.Now.AddMonths (1).Month;
        DateTime endDate = new DateTime (DateTime.Now.Year, nextMonth, 1);
        DateTime startDate = new DateTime (DateTime.Now.Year, DateTime.Now.Month, 1);

        double secondsTotal = (endDate - startDate).TotalSeconds;
        double secondsElapsed = (nowDate - startDate).TotalSeconds;
        progressBar.fillAmount = (float) (secondsElapsed / secondsTotal);

        if (state == true) {
            int currentDay = DateTime.Now.Day;
            int currentHour = DateTime.Now.Hour;
            int currentMinute = DateTime.Now.Minute;
            int currentSecond = DateTime.Now.Second;
            int daysInMonth = DateTime.DaysInMonth (DateTime.Now.Year, DateTime.Now.Month);

            if (currentSecond > 0) {
                currentMinute += 1;
            } else {
                currentSecond = 60;
            }
            if (currentMinute > 0) {
                currentHour += 1;
            } else {
                currentMinute = 60;
            }
            if (currentHour > 0) {
                currentDay += 1;
            } else {
                currentHour = 24;
            }

            daysText.text = (daysInMonth - currentDay).ToString ();
            hoursText.text = (24 - currentHour).ToString ("D2");
            minutesText.text = (60 - currentMinute).ToString ("D2");
            secondsText.text = (60 - currentSecond).ToString ("D2");
        }
	}

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void ToggleTimer () {

        state = !state;

        if (state == true) {
            StartCoroutine (ScaleToSize (1.0f));
        } else {
            StartCoroutine (ScaleToSize (0.0f));
        }
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    private IEnumerator ScaleToSize (float targetScale) {

        Vector3 currentScale = timerBox.localScale;
        float t = 0.0f;

        while (t < 1) {
            t += Time.deltaTime / 0.2f;
            timerBox.localScale = Vector3.Lerp (currentScale, new Vector3 (targetScale, targetScale, targetScale), t);
            yield return null;
        }
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//
}
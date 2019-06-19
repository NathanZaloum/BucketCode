using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class RadialSlider : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler {

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    [SerializeField]
    private TopUpManager topUpManager;
    [SerializeField]
    private Text balance;

    public bool topUp = true;

    public Image fill;
    public Text amount;
    public Text target;

    public bool overThis = false;
    public bool beingHeld = false;

    //private int step = 5;

    public float rotationValue = 72;

    private float tempAmount = 20;

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    private void Start () {

        //target.text = "Your new bucket balance will be $" + (topUpManager.target + tempAmount).ToString ();
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    private void Update () {

        if (topUp) {
            target.text = "Your new bucket balance will be $" + (int.Parse (balance.text.Substring (balance.text.IndexOf ('$') + 1)) + int.Parse (amount.text.Substring (amount.text.IndexOf ('$') + 1))).ToString ();
        } else {
            target.text = "Your new bucket balance will be $" + Mathf.Clamp ((int.Parse (balance.text.Substring (balance.text.IndexOf ('$') + 1)) - int.Parse (amount.text.Substring (amount.text.IndexOf ('$') + 1))), 0, (int.Parse (balance.text.Substring (balance.text.IndexOf ('$') + 1)) - int.Parse (amount.text.Substring (amount.text.IndexOf ('$') + 1)))).ToString ();
        }

        

        CheckForInput ();

        if (beingHeld) {

            Vector3 touch;

            if (Input.touchCount > 0) {
                touch = Input.GetTouch (0).position;
            } else {
                touch = Input.mousePosition;
            }

            Vector3 targetDir = touch - transform.parent.transform.position;
            float angle = Mathf.Atan2 (targetDir.y, targetDir.x) * Mathf.Rad2Deg;

            if (angle <= 90 && angle >= 0) {
                rotationValue = -(angle - 90);
            } else if (angle >= -180 && angle < 0) {
                rotationValue = -angle + 90;
            } else if (angle <= 180 && angle > 90) {
                rotationValue = 360 - (angle - 90);
            }

            transform.parent.transform.rotation = Quaternion.AngleAxis (Mathf.Floor ((-rotationValue + 90) / 18) * 18, Vector3.forward);
            fill.fillAmount = (Mathf.Ceil (rotationValue / 18) * 18) / 360;
            tempAmount = Mathf.RoundToInt (fill.fillAmount * 100);
            topUpManager.MatchSliders (tempAmount);
            amount.text = "$" + tempAmount.ToString ();
            //if (topUp) {
            //    target.text = "Your new bucket balance will be $" + (topUpManager.target + tempAmount).ToString ();
            //} else {
            //    target.text = "Your new bucket balance will be $" + Mathf.Clamp ((topUpManager.target - tempAmount), 0, (topUpManager.target - tempAmount)).ToString ();
            //}
        }
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    #region Input
    private void CheckForInput () {

        if (Input.GetMouseButtonDown (0) || (Input.touchCount > 0 && Input.GetTouch (0).phase == TouchPhase.Began)) {
            if (overThis) {
                beingHeld = true;
            }
        }

        if (Input.GetMouseButtonUp (0) && Input.touchCount == 0) {
            beingHeld = false;
        }
    }

    public void OnPointerEnter (PointerEventData eventData) {
        
        //overThis = true;
    }

    public void OnPointerExit (PointerEventData eventData) {
        
        //overThis = false;
    }

    public void OnPointerDown (PointerEventData eventData) {

        beingHeld = true;
    }

    public void OnPointerUp (PointerEventData eventData) {

        beingHeld = false;
    }
    #endregion

    //----------------------------------------------------------------------------------------------------------------------------------------------//
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Firebase;
using Firebase.Database;

public class AutoPaymentManager : MonoBehaviour {

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    private string paymentAmount;
    private string paymentStatus;
    private string paymentNext;
    private string paymentDay;
    private string paymentFrequency;

    private int amount = 20;
    private int increment = 5;

    [SerializeField]
    private Text amountDisplay;
    [SerializeField]
    private List<Button> statusButtons = new List<Button> ();
    [SerializeField]
    private List<Button> incrementButtons = new List<Button> ();
    [SerializeField]
    private List<Button> dayButtons = new List<Button> ();
    [SerializeField]
    private List<Button> frequencyButtons = new List<Button> ();

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    private void Start () {
		

	}

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void SwitchStatus (Button active) {
		
        foreach (Button b in statusButtons) {
            b.interactable = true;
            b.transform.GetChild (1).gameObject.SetActive (false);
        }
        active.interactable = false;
        active.transform.GetChild (1).gameObject.SetActive (true);
        paymentStatus = active.gameObject.name;
	}

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void SwitchIncrement (Button active) {

        foreach (Button b in incrementButtons) {
            b.interactable = true;
            b.transform.GetChild (1).gameObject.SetActive (false);
        }
        active.interactable = false;
        active.transform.GetChild (1).gameObject.SetActive (true);
        increment = int.Parse (active.gameObject.name);
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void SwitchDay (Button active) {

        foreach (Button b in dayButtons) {
            b.interactable = true;
            b.transform.GetChild (1).gameObject.SetActive (false);
        }
        active.interactable = false;
        active.transform.GetChild (1).gameObject.SetActive (true);
        paymentDay = active.gameObject.name;
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void SwitchFrequency (Button active) {

        foreach (Button b in frequencyButtons) {
            b.interactable = true;
            b.transform.GetChild (1).gameObject.SetActive (false);
        }
        active.interactable = false;
        active.transform.GetChild (1).gameObject.SetActive (true);
        paymentFrequency = active.gameObject.name;
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void PlusAmount () {
        
        amount += increment;
        amountDisplay.text = "$" + amount.ToString ();
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void MinusAmount () {

        if (amount > 0) {
            if (amount > increment) {
                amount -= increment;
            } else {
                amount = 0;
            }
            amountDisplay.text = "$" + amount.ToString ();
        }
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//
}
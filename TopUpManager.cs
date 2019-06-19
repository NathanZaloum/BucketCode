using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Firebase;
using Firebase.Database;

public class TopUpManager : MonoBehaviour {

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    [SerializeField]
    private DataGetSet dataGetSet;
    [SerializeField]
    private ActivityManager activityManager;
    [SerializeField]
    private CirclesManager circlesManager;

    [SerializeField]
    private RadialSlider sliderT1;
    [SerializeField]
    private Text displayT1;
    [SerializeField]
    private Button buttonT1;
    [SerializeField]
    private RadialSlider sliderT2;
    [SerializeField]
    private Text displayT2;
    [SerializeField]
    private Button buttonT2;
    [SerializeField]
    private RadialSlider sliderD;
    [SerializeField]
    private Text displayD;
    [SerializeField]
    private Button buttonD;

    [SerializeField]
    private GameObject confirmation;

    [SerializeField]
    private List<Text> balances = new List<Text> ();

    private int increment = 5;
    private int topup;
    private int donate;
    public int balance = 0;
    public int target = 0;
    public float sliderVal = 20.0f;

    private float t;
    private float duration = 1.0f;
    private int start = 0;

    private bool wait = false;

    private string activeCardID = "";
    private string activePushID = "";

    private string userPIN = "";

    [SerializeField]
    private ProjectPageDisplay projectInfo;

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    private void Start () {

        StartCoroutine (Wait ());
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    private IEnumerator Wait () {

        wait = false;
        topup = Mathf.RoundToInt (increment * sliderVal);
        donate = Mathf.RoundToInt (increment * sliderVal);
        yield return new WaitForSeconds (0.5f);
        wait = true;
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    private void Update () {
        
        //topup = Mathf.RoundToInt (increment * sliderVal);
        if (wait) {
            topup = Mathf.RoundToInt (sliderVal);
            donate = Mathf.RoundToInt (sliderVal);
        }
        displayT1.text = "$" + topup.ToString ();
        displayT2.text = "$" + topup.ToString ();
        displayD.text = "$" + donate.ToString ();
        
        foreach (Text b in balances) {
            b.text = "$" + balance.ToString ();
        }

        if (balance != target) {
            if (target > balance) { // Value is increasing
                SetButton (buttonT1, false, "Filling bucket...");
                SetButton (buttonT2, false, "Filling bucket...");
                SetButton (buttonD, false, "Donate");
            } else if (target < balance) { // Value is decreasing
                SetButton (buttonT1, false, "Top-Up");
                SetButton (buttonT2, false, "TopUp");
                SetButton (buttonD, false, "Donating...");
            }
        } else {
            SetButton (buttonT1, true, "Top-Up");
            SetButton (buttonT2, true, "Top-Up");
            SetButton (buttonD, true, "Donate");
        }

        if (balance != target) {
            t += Time.deltaTime / duration;
            balance = Mathf.CeilToInt (Mathf.Lerp (start, target, t));
        }
	}

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    private void SetButton (Button b, bool val, string t) {

        b.interactable = val;
        b.transform.GetChild (1).GetComponent<Text> ().text = t;
    }
    
    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void TopUp () {

        DataRef.CurrentUser ().Child ("stripe").Child ("customerID").GetValueAsync ().ContinueWith (async (task) => {
            if (task.IsFaulted || task.IsCanceled) {
                await new WaitForUpdate ();
                return;
            }
            await new WaitForUpdate ();
            DataSnapshot snapshot = task.Result;
            string stripeID;
            stripeID = snapshot.Value.ToString ();
            StartStripe (stripeID, this.gameObject.name, "Confirm");
        });

        /*if (balance == target) {
            t = 0;
            start = balance;
            target = balance + topup;
            dataGetSet.SetInfoSingle ("Balance", (target * 100).ToString());
        }*/
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void Donate () {

        int fGoal = projectInfo.fundGoal / 100;
        int fAmount = projectInfo.currentBalance / 100;
        int fNeeded = fGoal - fAmount;

        if (balance == target) {
            if (balance >= donate) {
                if (fNeeded > donate) {
                    t = 0;
                    start = balance;
                    target = balance - donate;
                    dataGetSet.SetInfoSingle ("Balance", (target * 100).ToString ());
                    UpdateProjectBalance (donate);
                    activityManager.SetAlertDonation (donate.ToString (), dataGetSet.currentUsername, projectInfo.projectReference, dataGetSet.currentUserID);
                    //activityManager.Resubscribe ();
                    AddToTotalGet (donate * 100);
                    circlesManager.AddReferenceToAllUserCircles (dataGetSet.currentUserID, donate * 100, projectInfo.projectReference);
                } else {
                    t = 0;
                    start = balance;
                    target = balance - fNeeded;
                    dataGetSet.SetInfoSingle ("Balance", (target * 100).ToString ());
                    UpdateProjectBalance (fNeeded);
                    activityManager.SetAlertDonation (fNeeded.ToString (), dataGetSet.currentUsername, projectInfo.projectReference, dataGetSet.currentUserID);
                    //activityManager.Resubscribe ();
                    StartCoroutine (WaitSetFunded ());
                    AddToTotalGet (fNeeded * 100);
                    circlesManager.AddReferenceToAllUserCircles (dataGetSet.currentUserID, fNeeded * 100, projectInfo.projectReference);
                }
            } else {
                if (balance > 0) {
                    int giveAmount = balance;
                    if (fNeeded > giveAmount) {
                        t = 0;
                        start = balance;
                        target = 0;
                        dataGetSet.SetInfoSingle ("Balance", (0).ToString ());
                        UpdateProjectBalance (giveAmount);
                        activityManager.SetAlertDonation (giveAmount.ToString (), dataGetSet.currentUsername, projectInfo.projectReference, dataGetSet.currentUserID);
                        //activityManager.Resubscribe ();
                        AddToTotalGet (giveAmount * 100);
                        circlesManager.AddReferenceToAllUserCircles (dataGetSet.currentUserID, giveAmount * 100, projectInfo.projectReference);
                    } else {
                        t = 0;
                        start = balance;
                        target = balance - fNeeded;
                        dataGetSet.SetInfoSingle ("Balance", (target * 100).ToString ());
                        UpdateProjectBalance (fNeeded);
                        activityManager.SetAlertDonation (fNeeded.ToString (), dataGetSet.currentUsername, projectInfo.projectReference, dataGetSet.currentUserID);
                        //activityManager.Resubscribe ();
                        StartCoroutine (WaitSetFunded ());
                        AddToTotalGet (fNeeded * 100);
                        circlesManager.AddReferenceToAllUserCircles (dataGetSet.currentUserID, fNeeded * 100, projectInfo.projectReference);
                    }
                } else {
                    // Do nothing, you have no money to give
                    // Let the user know
                }
            }
        }
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void MatchSliders (float active) {

        sliderVal = Mathf.RoundToInt (active);

        sliderT1.rotationValue = active * 3.6f;
        sliderT2.rotationValue = active * 3.6f;
        sliderD.rotationValue = active * 3.6f;
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    private void StartStripe (string stripeCustomerID, string returnObject, string returnMethod) {
        
        AndroidJavaClass activityClass, callClass;
        AndroidJavaObject activity;
        activityClass = new AndroidJavaClass ("com.unity3d.player.UnityPlayer");
        activity = activityClass.GetStatic<AndroidJavaObject> ("currentActivity");
        callClass = new AndroidJavaClass ("com.scimitar.bucket.UnityCalls");
        callClass.CallStatic ("StartStripeActivity", activity, stripeCustomerID, returnObject, returnMethod);

    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    private void Confirm (string cardID) {

        if (cardID != null) {
            // charge or do something else with card

            // Confirm action
            activeCardID = cardID;
            ConfirmationBox (true);
            confirmation.transform.GetChild (1).GetComponent<Text> ().text = cardID;
            //Text cardNum = confirmation.transform.GetChild (0).GetComponent<Text> ();
            //cardNum.text = "**** **** **** " + "";
        } else {
            // card select was canceled
            activeCardID = "";
        }

    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void ConfirmationBox (bool val) {

        confirmation.SetActive (val);
        if (val == true) {
            confirmation.transform.GetChild (1).GetChild (1).GetComponent<Text> ().text = "Accessing PIN";
            DataRef.CurrentUser ().Child ("stripe").Child ("pin").GetValueAsync ().ContinueWith (async (task) => {
                if (task.IsFaulted || task.IsCanceled) {
                    await new WaitForUpdate ();
                    confirmation.transform.GetChild (1).GetChild (1).GetComponent<Text> ().text = "Failed to get PIN";
                    return;
                }
                await new WaitForUpdate ();
                DataSnapshot snapshot = task.Result;
                userPIN = snapshot.Value.ToString ();
                confirmation.transform.GetChild (1).GetChild (1).GetComponent<Text> ().text = "PIN accessed";
            });
        } else {
            userPIN = "";
        }
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    class StripeCharge {

        public string amount;
        public string source;

        public StripeCharge (string a, string s) {

            this.amount = a;
            this.source = s;
        }
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void ChargeCard () {

        InputField inputPin = confirmation.transform.GetChild (1).GetChild (3).GetComponent<InputField> ();

        if (inputPin.text == userPIN) {
            // Charge the card
            confirmation.transform.GetChild (1).GetChild (1).GetComponent<Text> ().text = "Making Charge";
            inputPin.text = "";
            MakeCharge ();
        } else {
            // wrong pin
            confirmation.transform.GetChild (1).GetChild (1).GetComponent<Text> ().text = "Wrong PIN";
            inputPin.text = "";
            return;
        }
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    private void MakeCharge () {

        confirmation.transform.GetChild (2).gameObject.SetActive (true);

        int tempTarget = balance + topup;
        string balanceCents = (tempTarget * 100).ToString ();

        activePushID = DataRef.CurrentUser ().Child ("stripe").Child ("charges").Push ().Key;

        StripeCharge charge = new StripeCharge (balanceCents, activeCardID);
        string json = JsonUtility.ToJson (charge);

        DataRef.CurrentUser ().Child ("stripe").Child ("charges").Child (activePushID).SetRawJsonValueAsync (json).ContinueWith (async (task) => {
            if (task.IsFaulted || task.IsCanceled) {
                await new WaitForUpdate ();
                confirmation.transform.GetChild (2).gameObject.SetActive (false);
                return;
            }
            await new WaitForUpdate ();
            ListenForCharge ();
        });
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    private void ListenForCharge () {

        DataRef.CurrentUser ().Child ("stripe").Child ("charges").Child (activePushID).ValueChanged += HandleValueChanged;
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    private void HandleValueChanged (object sender, ValueChangedEventArgs args) {

        if (args.DatabaseError != null) {
            confirmation.transform.GetChild (1).GetChild (1).GetComponent<Text> ().text = "An error has been encountered (#1).";
            confirmation.transform.GetChild (2).gameObject.SetActive (false);
            return;
        }
        // Do stuff

        string status = args.Snapshot.Child ("status").Value.ToString ();

        if (status == "inProgress" || status == null) {
            return;
        } else {
            DataRef.CurrentUser ().Child ("stripe").Child ("charges").Child (activePushID).ValueChanged -= HandleValueChanged;
        }

        if (status == "error") {
            confirmation.transform.GetChild (1).GetChild (1).GetComponent<Text> ().text = "An error has been encountered (#2).";
            confirmation.transform.GetChild (2).gameObject.SetActive (false);
        } else if (status == "succeeded") {
            confirmation.transform.GetChild (2).gameObject.SetActive (false);
            ConfirmationBox (false);
            t = 0;
            start = balance;
            target = balance + topup;
        }
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    private void UpdateProjectBalance (int donation) {

        int newBalance = projectInfo.currentBalance + (donation * 100);
        string newAmount = newBalance.ToString ();

        DataRef.Projects (projectInfo.projectReference).Child ("FundingAmount").SetValueAsync (newAmount).ContinueWith (async (task) => {
            if (task.IsFaulted || task.IsCanceled) {
                await new WaitForUpdate ();
                print (task.Exception);
                return;
            }
            await new WaitForUpdate ();
            projectInfo.UpdateDisplay (newBalance);
        });
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    private IEnumerator WaitSetFunded () {

        yield return new WaitForSeconds (1.0f);
        activityManager.SetAlertProjectFunded (projectInfo.projectReference, "Group_Default");
        //activityManager.Resubscribe ();
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    private void AddToTotalGet (int donation) {

        DataRef.CurrentUser ().Child ("DonationTotal").GetValueAsync ().ContinueWith (async (task) => {
            if (task.IsFaulted || task.IsCanceled) {
                await new WaitForUpdate ();
                print (task.Exception);
                return;
            }
            await new WaitForUpdate ();
            DataSnapshot snapshot = task.Result;
            int totalAmount = int.Parse (snapshot.Value.ToString ());
            int newTotal = totalAmount + donation;

            AddToUserTotal (newTotal);
        });

        DataRef.General ().GetValueAsync ().ContinueWith (async (task) => {
            if (task.IsFaulted || task.IsCanceled) {
                await new WaitForUpdate ();
                print (task.Exception);
                return;
            }
            await new WaitForUpdate ();
            DataSnapshot snapshot = task.Result;
            int totalAmount = int.Parse (snapshot.Child ("TotalDonationsAmount").Value.ToString ());
            int totalMade = int.Parse (snapshot.Child ("TotalDonationsMade").Value.ToString ());

            int newAmount = totalAmount + donation;
            int newMade = totalMade + 1;

            AddToTotalSet ("TotalDonationsAmount", newAmount);
            AddToTotalSet ("TotalDonationsMade", newMade);
        });
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    private void AddToTotalSet (string child, int value) {

        DataRef.General ().Child (child).SetValueAsync (value.ToString ()).ContinueWith (async (task) => {
            if (task.IsFaulted || task.IsCanceled) {
                await new WaitForUpdate ();
                print (task.Exception);
                return;
            }
            await new WaitForUpdate ();
        });
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    private void AddToUserTotal (int value) {

        DataRef.CurrentUser ().Child ("DonationTotal").SetValueAsync (value.ToString ()).ContinueWith (async (task) => {
            if (task.IsFaulted || task.IsCanceled) {
                await new WaitForUpdate ();
                print (task.Exception);
                return;
            }
            await new WaitForUpdate ();
        });
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//
}
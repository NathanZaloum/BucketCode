using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Firebase;
using Firebase.Database;

public class CirclesManager : MonoBehaviour {

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    [SerializeField]
    private Color green, grey;
    
    [SerializeField]
    private UserAuthentication userAuth;
    [SerializeField]
    private CommentManager commentManager;
    [SerializeField]
    private DebugConsole debug;

    [SerializeField]
    private Text mainTitle;
    [SerializeField]
    private Text aboutTitle;
    [SerializeField]
    private Text about;
    [SerializeField]
    private Text membersCount;
    [SerializeField]
    private Image donatedSlider;
    [SerializeField]
    private Text donated;
    [SerializeField]
    private Text availableMain;
    [SerializeField]
    private Text availableSub;
    [SerializeField]
    private Image pictureOne;
    [SerializeField]
    private Image pictureTwo;

    private float donatedFunds;
    private float availableFunds;

    [SerializeField]
    private Button leaveButton;
    [SerializeField]
    private GameObject projectHolder;

    public string activeCircleRef;

    private bool aboutOpen;
    private bool membersOpen;
    private bool circlesOpen;

    [SerializeField]
    private GameObject aboutBox;
    [SerializeField]
    private GameObject memberBox;
    [SerializeField]
    private Transform membersContent;
    [SerializeField]
    private GameObject memberPrefab;
    [SerializeField]
    private GameObject circlesBox;
    [SerializeField]
    private Transform circlesContent;
    [SerializeField]
    private GameObject circlePrefab;
    [SerializeField]
    private GameObject leftArrow;
    [SerializeField]
    private GameObject rightArrow;
    [SerializeField]
    private GameObject noCirclesMessage;
    [SerializeField]
    private GameObject loadingPanel;

    private bool begunChecks = false;
    private bool breakCycle = false;

    private bool loading = true;

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void StartCircleProcess () {

        DataRef.CurrentUser ().Child ("Circles").GetValueAsync ().ContinueWith (async (task) => {
            await new WaitForUpdate ();
            DataSnapshot snapshot = task.Result;

            if (snapshot.HasChild ("Bucket Circle New Zealand")) {

                List<string> circles = new List<string> ();
                foreach (DataSnapshot circle in snapshot.Children) {
                    circles.Add (circle.Value.ToString ());
                }
                circles.Sort ();
                PopulateCircleList (circles.IndexOf ("Bucket Circle New Zealand"));
            } else {
                PopulateCircleList (0);
            }
        });
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    private void Update () {

        if (begunChecks) {
            if (loading == false) {
                if (circlesContent.childCount == 0) {
                    noCirclesMessage.SetActive (true);
                } else {
                    noCirclesMessage.SetActive (false);
                }
            }
        }

        if (activeCircleRef != "") {
            if (circlesContent.Find (activeCircleRef) != null) {
                int index = circlesContent.Find (activeCircleRef).GetSiblingIndex ();
                if (index == 0) {
                    leftArrow.SetActive (false);
                } else {
                    leftArrow.SetActive (true);
                }
                if (index == circlesContent.childCount - 1) {
                    rightArrow.SetActive (false);
                } else {
                    rightArrow.SetActive (true);
                }
            }
        }

        GetMemberBalances ();
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void NextLeft () {

        int currentIndex = circlesContent.Find (activeCircleRef).GetSiblingIndex ();

        if (currentIndex != 0) {
            GetCircleInformation (circlesContent.GetChild (currentIndex - 1).gameObject.name);
        }

        foreach (Transform child in circlesContent) {
            child.GetComponent<Button> ().interactable = true;
        }
        circlesContent.GetChild (currentIndex - 1).GetComponent<Button> ().interactable = false;
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void NextRight () {

        int currentIndex = circlesContent.Find (activeCircleRef).GetSiblingIndex ();

        if (currentIndex != circlesContent.childCount - 1) {
            GetCircleInformation (circlesContent.GetChild (currentIndex + 1).gameObject.name);
        }

        foreach (Transform child in circlesContent) {
            child.GetComponent<Button> ().interactable = true;
        }
        circlesContent.GetChild (currentIndex + 1).GetComponent<Button> ().interactable = false;
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void PickCircle (string circle) {
        
        GetCircleInformation (circlesContent.GetChild (circlesContent.Find (circle).GetSiblingIndex ()).gameObject.name);
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    private IEnumerator FadeLoad (float targetAlpha, bool state, float timeToMove) {

        float currentAlpha = loadingPanel.GetComponent<Image>().color.a;
        
        float t = 0.0f;

        while (t < 1) {
            t += Time.deltaTime / timeToMove;
            foreach (Image img in loadingPanel.GetComponentsInChildren<Image> ()) {
                img.raycastTarget = state;
                img.color = Color.Lerp (new Color (img.color.r, img.color.g, img.color.b, currentAlpha), new Color (img.color.r, img.color.g, img.color.b, targetAlpha), t);
            }
            yield return null;
        }
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void JoinCircle (string circleRef) {

        string userRef = userAuth.transform.GetComponent<DataGetSet> ().currentUserID;

        DataRef.CurrentUser ().Child ("Circles").Child (circleRef). SetValueAsync (circleRef).ContinueWith (async (task) => {
            await new WaitForUpdate ();
            if (task.IsFaulted || task.IsCanceled) {
                print (task.Exception);
                return;
            }
        });

        DataRef.Circles (circleRef).Child ("Members").Child (userRef).SetValueAsync (userRef).ContinueWith (async (task) => {
            await new WaitForUpdate ();
            if (task.IsFaulted || task.IsCanceled) {
                print (task.Exception);
                return;
            }
        });

        DataRef.Circles (circleRef).Child ("TotalMembers").GetValueAsync ().ContinueWith (async (task) => {
            await new WaitForUpdate ();
            if (task.IsFaulted || task.IsCanceled) {
                print (task.Exception);
                return;
            }
            DataSnapshot snapshot = task.Result;

            int totalMembers = int.Parse (snapshot.Value.ToString ());
            int newTotal = totalMembers + 1;

            SetTotalMembers (circleRef, newTotal);

            List<string> circles = new List<string> ();
            foreach (Transform circle in circlesContent) {
                circles.Add (circle.gameObject.name);
            }
            circles.Add (circleRef);
            circles.Sort ();

            PopulateCircleList (circles.IndexOf (circleRef));
        });
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void LeaveCircle () {

        string userRef = userAuth.transform.GetComponent<DataGetSet> ().currentUserID;
        string circleRef = activeCircleRef;

        RemoveUserFromCircle (circleRef, userRef);

        int currentIndex = circlesContent.Find (activeCircleRef).GetSiblingIndex ();
        int maxIndex = circlesContent.childCount - 1;
        Destroy (circlesContent.GetChild (currentIndex).gameObject);
        if (currentIndex < maxIndex) {
            PopulateCircleList (0);
        } else {
            PopulateCircleList (maxIndex - 1);
        }
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void AddReferenceToAllUserCircles (string uid, int donation, string projectRef) {

        DataRef.CurrentUser ().Child ("Circles").GetValueAsync ().ContinueWith (async (task) => {
            await new WaitForUpdate ();
            if (task.IsFaulted || task.IsCanceled) {
                print (task.Exception);
                return;
            }
            DataSnapshot snapshot = task.Result;

            foreach (DataSnapshot snap in snapshot.Children) {
                string circleRef = snap.Value.ToString ();
                GetCircle (circleRef, projectRef, donation, uid);
            }
        });
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void RemoveFromAllCircles (string uid) {

        DataRef.User (uid).Child ("Circles").GetValueAsync ().ContinueWith (async (task) => {
            await new WaitForUpdate ();
            if (task.IsFaulted || task.IsCanceled) {
                print (task.Exception);
                return;
            }
            DataSnapshot snapshot = task.Result;

            foreach (DataSnapshot snap in snapshot.Children) {
                string circleRef = snap.Value.ToString ();
                RemoveUser (circleRef, uid);
            }
        });
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    private void GetCircle (string circleRef, string projectRef, int donation, string uid) {

        DataRef.Circles (circleRef).GetValueAsync ().ContinueWith (async (task) => {
            await new WaitForUpdate ();
            if (task.IsFaulted || task.IsCanceled) {
                print (task.Exception);
                return;
            }
            DataSnapshot snapshot = task.Result;

            int totalDonated = int.Parse (snapshot.Child ("TotalDonated").Value.ToString ());
            int newTotal = totalDonated + donation;
            SetCircleTotal (circleRef, newTotal.ToString ());

            SetDonationReferences (circleRef, projectRef, uid);

            if (snapshot.Child ("Projects").Child (projectRef).Child ("DonationTotal").Exists) {
                int donationTotal = int.Parse (snapshot.Child ("Projects").Child (projectRef).Child ("DonationTotal").Value.ToString ());
                int newDonation = donationTotal + donation;
                SetProjectTotal (circleRef, projectRef, newDonation.ToString ());
            } else {
                SetProjectTotal (circleRef, projectRef, donation.ToString ());
            }
        });
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    private void SetDonationReferences (string circleRef, string projectRef, string userRef) {

        DataRef.Circles (circleRef).Child ("Projects").Child (projectRef).Child ("Members").Child (userRef).SetValueAsync (userRef).ContinueWith (async (task) => {
            await new WaitForUpdate ();
            if (task.IsFaulted || task.IsCanceled) {
                print (task.Exception);
                return;
            }
        });
        DataRef.Circles (circleRef).Child ("Projects").Child (projectRef).Child ("Reference").SetValueAsync (projectRef).ContinueWith (async (task) => {
            await new WaitForUpdate ();
            if (task.IsFaulted || task.IsCanceled) {
                print (task.Exception);
                return;
            }
        });
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    private void SetCircleTotal (string circleRef, string value) {

        DataRef.Circles (circleRef).Child ("TotalDonated").SetValueAsync (value).ContinueWith (async (task) => {
            await new WaitForUpdate ();
            if (task.IsFaulted || task.IsCanceled) {
                print (task.Exception);
                return;
            }
        });
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    private void SetProjectTotal (string circleRef, string projectRef, string value) {

        DataRef.Circles (circleRef).Child ("Projects").Child (projectRef).Child ("DonationTotal").SetValueAsync (value).ContinueWith (async (task) => {
            await new WaitForUpdate ();
            if (task.IsFaulted || task.IsCanceled) {
                print (task.Exception);
                return;
            }
        });
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    private void RemoveUser (string circleRef, string userRef) {

        DataRef.Circles (circleRef).Child ("Members").Child (userRef).RemoveValueAsync ().ContinueWith (async (task) => {
            await new WaitForUpdate ();
            if (task.IsFaulted || task.IsCanceled) {
                print (task.Exception);
                return;
            }
        });

        DataRef.Circles (circleRef).Child ("TotalMembers").GetValueAsync ().ContinueWith (async (task) => {
            await new WaitForUpdate ();
            if (task.IsFaulted || task.IsCanceled) {
                print (task.Exception);
                return;
            }
            DataSnapshot snapshot = task.Result;

            int currentMembers = int.Parse (snapshot.Value.ToString ());
            int newMembers = currentMembers - 1;

            SetTotalMembers (circleRef, newMembers);
            userAuth.ClearData (userRef);
        });
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    private void RemoveUserFromCircle (string circleRef, string userRef) {

        DataRef.User (userRef).Child ("Circles").Child (circleRef).RemoveValueAsync ().ContinueWith (async (task) => {
            await new WaitForUpdate ();
            if (task.IsFaulted || task.IsCanceled) {
                print (task.Exception);
                return;
            }
        });

        DataRef.Circles (circleRef).Child ("Members").Child (userRef).RemoveValueAsync ().ContinueWith (async (task) => {
            await new WaitForUpdate ();
            if (task.IsFaulted || task.IsCanceled) {
                print (task.Exception);
                return;
            }
        });

        DataRef.Circles (circleRef).Child ("TotalMembers").GetValueAsync ().ContinueWith (async (task) => {
            await new WaitForUpdate ();
            if (task.IsFaulted || task.IsCanceled) {
                print (task.Exception);
                return;
            }
            DataSnapshot snapshot = task.Result;

            int currentMembers = int.Parse (snapshot.Value.ToString ());
            int newMembers = currentMembers - 1;

            SetTotalMembers (circleRef, newMembers);
        });
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    private void RemoveReferences (string circleRef, string projectRef, string userRef) {
        
        DataRef.Circles (circleRef).Child ("Projects").Child (projectRef).Child ("Members").Child (userRef).RemoveValueAsync ().ContinueWith (async (task) => {
            await new WaitForUpdate ();
            if (task.IsFaulted || task.IsCanceled) {
                print (task.Exception);
                return;
            }
        });
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    private void SetTotalMembers (string circleRef, int value) {

        DataRef.Circles (circleRef).Child ("TotalMembers").SetValueAsync (value.ToString ()).ContinueWith (async (task) => {
            await new WaitForUpdate ();
            if (task.IsFaulted || task.IsCanceled) {
                print (task.Exception);
                return;
            }
        });
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    #region // About Handler

    public void ToggleAboutBox () {

        aboutOpen = !aboutOpen;

        if (aboutOpen == true) {
            aboutBox.transform.GetComponent<RectTransform> ().localScale = new Vector3 (1.0f, 1.0f, 1.0f);
        } else if (aboutOpen == false) {
            aboutBox.transform.GetComponent<RectTransform> ().localScale = new Vector3 (0.0f, 0.0f, 0.0f);
        }
    }

    #endregion

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    #region // Members Handler

    public void ToggleMemberBox () {

        membersOpen = !membersOpen;

        if (membersOpen == true) {
            memberBox.transform.GetComponent<RectTransform> ().localScale = new Vector3 (1.0f, 1.0f, 1.0f);
            membersContent.GetComponent<RectTransform> ().anchoredPosition = Vector3.zero;
        } else if (membersOpen == false) {
            memberBox.transform.GetComponent<RectTransform> ().localScale = new Vector3 (0.0f, 0.0f, 0.0f);
        }
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void PopulateMembers () {

        if (membersContent.childCount > 0) {
            foreach (Transform child in membersContent) {
                Destroy (child.gameObject);
            }
        }
        
        DataRef.Circles (activeCircleRef).Child ("Members").GetValueAsync ().ContinueWith (async (task) => {
            await new WaitForUpdate ();
            DataSnapshot snapshot = task.Result;

            foreach (DataSnapshot member in snapshot.Children) {
                string userRef = member.Value.ToString ();
                InstantiateMember (userRef);
            }
            
            membersCount.text = membersContent.childCount.ToString () + " Members";
            GetMemberBalances ();
            OrganiseMembersList ();
        });
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void OrganiseMembersList () {

        List<Transform> members = new List<Transform> ();
        foreach (Transform member in membersContent) {
            members.Add (member);
        }

        members = members.OrderByDescending (x => x.name).ToList ();

        for (int i = 0; i < membersContent.transform.childCount; i++) {
            members[i].SetSiblingIndex (i);
        }
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    private void InstantiateMember (string userReference) {

        GameObject com = Instantiate (memberPrefab, Vector3.zero, Quaternion.identity) as GameObject;
        com.transform.SetParent (membersContent);
        com.transform.localScale = new Vector3 (1, 1, 1);

        DataRef.User (userReference).GetValueAsync ().ContinueWith (async (task) => {
            await new WaitForUpdate ();
            DataSnapshot snapshot = task.Result;

            string username = snapshot.Child ("Username").Value.ToString ();
            string balance = snapshot.Child ("Balance").Value.ToString ();
            string pictureID = snapshot.Child ("PictureID").Value.ToString ();

            com.name = username;
            com.transform.GetChild (1).GetComponent<Text> ().text = username;
            com.transform.GetChild (2).GetComponent<Text> ().text = userReference;
            com.transform.GetChild (3).GetComponent<Text> ().text = balance;
            Sprite img = Resources.Load<Sprite> ("2D/Animals/" + pictureID);
            com.transform.GetChild (0).GetChild (0).GetComponent<Image> ().sprite = img;
        });
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void StartMemberCheck () {

        if (breakCycle == true) {
            breakCycle = false;
            PopulateMemberReCheck ();
        } else {
            //debug.UpdateReport ("Member Check", "Member Check: Starting search for members...", debug.orange);
            DataRef.Circles (activeCircleRef).Child ("Members").GetValueAsync ().ContinueWith (async (task) => {
                if (task.IsFaulted || task.IsCanceled) {
                    await new WaitForUpdate ();
                    //debug.UpdateReport ("Member Check", "Member Check: Failed to access members.", debug.red);
                }
                await new WaitForUpdate ();
                DataSnapshot snapshot = task.Result;

                //debug.UpdateReport ("Member Check", "Member Check: Found members...", debug.orange);

                if (int.Parse (snapshot.ChildrenCount.ToString ()) != membersContent.childCount) {
                    PopulateMemberReCheck ();
                } else {
                    StartCoroutine (CheckForMemberChanges (0.1f));
                }
            });
        }
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    private IEnumerator CheckForMemberChanges (float delay) {

        yield return new WaitForSeconds (delay);
        
        StartMemberCheck ();
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void PopulateMemberReCheck () {

        if (membersContent.childCount > 0) {
            foreach (Transform child in membersContent) {
                Destroy (child.gameObject);
            }
        }

        DataRef.Circles (activeCircleRef).Child ("Members").GetValueAsync ().ContinueWith (async (task) => {
            if (task.IsFaulted || task.IsCanceled) {
                await new WaitForUpdate ();
                //debug.UpdateReport ("Member Check", "Member Check: Failed to access members.", debug.red);
            }
            await new WaitForUpdate ();
            DataSnapshot snapshot = task.Result;

            foreach (DataSnapshot member in snapshot.Children) {
                string userRef = member.Value.ToString ();
                InstantiateMember (userRef);
            }
            
            membersCount.text = membersContent.childCount.ToString () + " Members";
            OrganiseMembersList ();

            //debug.UpdateReport ("Member Check", "Member Check: Populated members list", debug.green);

            StartCoroutine (FadeLoad (0.0f, false, 1.0f));

            StartCoroutine (CheckForMemberChanges (0.1f));
        });
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    private void GetMemberBalances () {

        availableFunds = 0;
        
        if (membersContent.childCount > 0) {
            foreach (Transform child in membersContent) {
                if (child.GetChild (3).GetComponent<Text> ().text != "0" && child.GetChild (3).GetComponent<Text> ().text != "") {
                    availableFunds += int.Parse (child.GetChild (3).GetComponent<Text> ().text);
                }
            }
        }

        if (availableFunds == 0) {
            donatedSlider.fillAmount = 0;
            availableMain.text = "$0";
            availableSub.text = "$0";
        } else {
            float percentage = (donatedFunds / availableFunds);
            //print (percentage + " - " + donatedFunds + " - " + availableFunds);
            donatedSlider.fillAmount = percentage;
            availableMain.text = "$" + (availableFunds / 100).ToString ();
            availableSub.text = "$" + (availableFunds / 100).ToString ();
        }
    }

    #endregion

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    #region // Circles Handler

    public void ToggleCircleBox () {

        circlesOpen = !circlesOpen;

        if (circlesOpen == true) {
            circlesBox.transform.GetComponent<RectTransform> ().localScale = new Vector3 (1.0f, 1.0f, 1.0f);
        } else if (circlesOpen == false) {
            circlesBox.transform.GetComponent<RectTransform> ().localScale = new Vector3 (0.0f, 0.0f, 0.0f);
        }
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void PopulateCircleList (int index) {

        StartCoroutine (FadeLoad (1.0f, true, 1.0f));
        loading = true;

        if (circlesContent.childCount > 0) {
            foreach (Transform child in circlesContent) {
                Destroy (child.gameObject);
            }
        }

        DataRef.CurrentUser ().Child ("Circles").GetValueAsync ().ContinueWith (async (task) => {
            await new WaitForUpdate ();
            DataSnapshot snapshot = task.Result;

            foreach (DataSnapshot circle in snapshot.Children) {
                string circleRef = circle.Value.ToString ();
                InstantiateCircle (circleRef);
            }

            OrganiseCirclesList ();

            foreach (Transform child in circlesContent) {
                child.GetComponent<Button> ().interactable = true;
            }
            circlesContent.GetChild (index).GetComponent<Button> ().interactable = false;
            
            GetCircleInformation (circlesContent.GetChild (index).gameObject.name);
        });
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void OrganiseCirclesList () {

        List<Transform> circles = new List<Transform> ();
        foreach (Transform circle in circlesContent) {
            circles.Add (circle);
        }

        circles = circles.OrderBy (x => x.name).ToList ();

        for (int i = 0; i < circlesContent.transform.childCount; i++) {
            circles[i].SetSiblingIndex (i);
        }
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    private void InstantiateCircle (string circleReference) {

        GameObject circ = Instantiate (circlePrefab, Vector3.zero, Quaternion.identity) as GameObject;
        circ.transform.SetParent (circlesContent);
        circ.transform.localScale = new Vector3 (1, 1, 1);
        circ.name = circleReference;
        circ.transform.GetChild (0).GetComponent<Text> ().text = circleReference;
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    private void GetCircleInformation (string circleReference) {

        StartCoroutine (FadeLoad (1.0f, true, 1.0f));
        loading = true;
        activeCircleRef = circleReference;

        //debug.UpdateReport (circleReference, circleReference + ": Getting Circle information...", debug.orange);

        DataRef.Circles (circleReference).GetValueAsync ().ContinueWith (async (task) => {
            if (task.IsCanceled || task.IsFaulted) {
                await new WaitForUpdate ();
                //debug.UpdateReport (circleReference, circleReference + ": Failed to get Circle information.", debug.red);
            }
            await new WaitForUpdate ();
            DataSnapshot snapshot = task.Result;

            //debug.UpdateReport (circleReference, circleReference + ": Accessed Circle information. Loading information to screen...", debug.orange);

            string name = snapshot.Child ("Name").Value.ToString ();
            string description = snapshot.Child ("About").Value.ToString ();
            string pictureID = snapshot.Child ("PictureID").Value.ToString ();
            string secondPictureID = snapshot.Child ("SecondaryPictureID").Value.ToString ();
            string totalDonated = snapshot.Child ("TotalDonated").Value.ToString ();

            mainTitle.text = name;
            aboutTitle.text = name;
            about.text = description;
            Sprite img1 = Resources.Load<Sprite> ("2D/Groups/" + pictureID);
            pictureOne.sprite = img1;
            Sprite img2 = Resources.Load<Sprite> ("2D/Groups/" + secondPictureID);
            pictureTwo.sprite = img2;
            pictureTwo.transform.GetComponent<ScaleImageToCircle> ().Scale ();
            if (circleReference == "Bucket Circle New Zealand") {
                pictureTwo.transform.GetComponent<RectTransform> ().anchoredPosition = new Vector3 (0.0f, -95.0f, 0.0f);
            } else if (circleReference == "DOC Supporters") {
                pictureTwo.transform.GetComponent<RectTransform> ().anchoredPosition = new Vector3 (0.0f, 40.0f, 0.0f);
            } else if (circleReference == "Predator Free Brooklyn") {
                pictureTwo.transform.GetComponent<RectTransform> ().anchoredPosition = new Vector3 (0.0f, 0.0f, 0.0f);
            } else {
                pictureTwo.transform.GetComponent<RectTransform> ().anchoredPosition = new Vector3 (0.0f, 0.0f, 0.0f);
            }
            donated.text = "$" + (int.Parse (totalDonated) / 100).ToString ();
            donatedFunds = int.Parse (totalDonated);

            //debug.UpdateReport (circleReference, circleReference + ": Loaded information to screen.", debug.green);

            if (begunChecks == false) {
                begunChecks = true;
                transform.GetComponent<CommentManager> ().StartCommentCheck ();
                StartMemberCheck ();
                projectHolder.transform.GetComponent<CircleProjectSearch> ().PopulateReCheck ();
                loading = false;
            } else {
                transform.GetComponent<CommentManager> ().breakCycle = true;
                breakCycle = true;
                projectHolder.transform.GetComponent<CircleProjectSearch> ().breakCycle = true;
                loading = false;
            }
        });
    }

    #endregion

    //----------------------------------------------------------------------------------------------------------------------------------------------//
}
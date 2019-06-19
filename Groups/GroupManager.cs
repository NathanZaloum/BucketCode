using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Firebase;
using Firebase.Database;

public class GroupManager : MonoBehaviour {

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    [SerializeField]
    private RectTransform canvas;
    
    [SerializeField]
    private Text detailsButton, membersButton;

    [SerializeField]
    private RectTransform groupScrollView;

    [SerializeField]
    private RectTransform addGroup;
    [SerializeField]
    private GameObject groupPrefab;

    // Group Information
    public GroupInformation activeGroup;

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void ClearList () {

        for (int i = 0; i < groupScrollView.transform.childCount; i++) {
            if (groupScrollView.GetChild (i).gameObject.name == "GroupHolderPrefab(Clone)") {
                Destroy (groupScrollView.GetChild (i).gameObject);
            }
        }
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void InstantiateGroup (string target) {

        DataRef.Groups (target).GetValueAsync ().ContinueWith (async (task) => {
            await new WaitForUpdate ();
            DataSnapshot snapshot = task.Result;

            string name = snapshot.Child ("GroupName").Value.ToString ();
            string description = snapshot.Child ("GroupDescription").Value.ToString ();
            string contact = snapshot.Child ("Contact").Value.ToString ();
            //string access = snapshot.Child ("AccessType").Value.ToString ();

            GameObject group = Instantiate (groupPrefab, Vector3.zero, Quaternion.identity) as GameObject;
            group.transform.SetParent (groupScrollView);
            group.transform.localScale = new Vector3 (1, 1, 1);
            SortGroupScrollView ();
            group.GetComponent<GroupInformation> ().SetInformation (name, description, contact, "Administrator");
        });
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void SortGroupScrollView () {

        addGroup.SetAsLastSibling ();
        
        float halfX = canvas.sizeDelta.x / 2;
        int count = groupScrollView.transform.childCount;

        groupScrollView.sizeDelta = new Vector2 (halfX + (halfX * count), 350);

        for (int i = 0; i < count; i++) {
            groupScrollView.GetChild (i).GetComponent<RectTransform> ().anchoredPosition = new Vector3 (halfX + (halfX * i), 0, 0);
        }

        groupScrollView.GetChild (0).GetComponent<GroupInformation> ().ToggleOn ();
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GroupInformation : MonoBehaviour {

    //----------------------------------------------------------------------------------------------------------------------------------------------//
    
    private GroupManager groupManager;
    private ProjectManager projectManager;

    public bool selected = false;

    public string groupName;
    public string groupDescription;
    public string groupContact;
    public string accessType;

    private Text nameField;
    private Text accessField;

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    private void Awake () {
        
        groupManager = GameObject.Find ("GroupPanel").GetComponent<GroupManager> ();
        projectManager = GameObject.Find ("ProjectsPanel").GetComponent<ProjectManager> ();

        //ToggleOn ();
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void SetInformation (string name, string description, string contact, string access) {

        groupName = name;
        groupDescription = description;
        groupContact = contact;
        accessType = access;

        SetDisplay ();
        Populate ();
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    private void SetDisplay () {

        nameField = this.gameObject.transform.GetChild (1).GetComponent<Text> ();
        accessField = this.gameObject.transform.GetChild (2).GetComponent<Text> ();

        nameField.text = groupName;
        accessField.text = accessType;
	}

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void ToggleOn () {

        Transform parent = this.transform.parent;

        for (int i = 0; i < parent.childCount - 1; i++) {
            parent.GetChild (i).GetComponent<GroupInformation> ().SetButton ();
        }
        this.transform.GetChild (0).GetComponent<Button> ().interactable = false;
        selected = true;
        groupManager.activeGroup = this;
        Populate ();
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void SetButton () {

        this.transform.GetChild (0).GetComponent<Button> ().interactable = true;
        selected = false;
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void Populate () {

        projectManager.PopulateList (groupName);
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//
}
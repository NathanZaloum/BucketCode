using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FilterButton : MonoBehaviour {

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public bool selected = true;
    public bool allSelected = true;
    
    [SerializeField]
    private Color colorOff, colorOn;
    [SerializeField]
    private Sprite plus, minus;
    [SerializeField]
    private Transform all;

    private Image img;

    private Transform parent;

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    private void Start () {

        img = transform.GetChild (0).GetChild (0).GetComponent<Image> ();
        parent = this.gameObject.transform.parent;

        SetItem ();
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void SetItem () {

        if (selected) {
            transform.GetComponent<Image> ().color = colorOn;
            img.sprite = minus;
        } else {
            transform.GetComponent<Image> ().color = colorOff;
            img.sprite = plus;
        }
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void SwitchSelected () {

        selected = !selected;
        if (selected == false) {
            if (all.GetComponent<FilterButton> ().allSelected == true) {
                all.GetComponent<FilterButton> ().selected = false;
                all.GetComponent<FilterButton> ().allSelected = false;
                all.GetComponent<FilterButton> ().SetItem ();
            }
        } else {
            // Check if every other task is also selected
            bool finalSelected = true;
            foreach (Transform child in parent) {
                if (child.gameObject.name != "All") {
                    if (child.GetComponent<FilterButton> ().selected == false) {
                        finalSelected = false;
                    }
                }
            }
            if (finalSelected == true) {
                all.GetComponent<FilterButton> ().selected = true;
                all.GetComponent<FilterButton> ().allSelected = true;
                all.GetComponent<FilterButton> ().SetItem ();
            }
        }
        SetItem ();
	}

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void SwitchAll () {

        if (allSelected) {
            // Deselect all
            foreach (Transform child in parent) {
                child.GetComponent<FilterButton> ().selected = false;
                child.GetComponent<FilterButton> ().SetItem ();
            }
            allSelected = false;
        } else {
            // Select all
            foreach (Transform child in parent) {
                child.GetComponent<FilterButton> ().selected = true;
                child.GetComponent<FilterButton> ().SetItem ();
            }
            allSelected = true;
        }
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//
}
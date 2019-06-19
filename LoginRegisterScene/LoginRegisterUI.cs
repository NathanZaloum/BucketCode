using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ColorConverter;

public class LoginRegisterUI : MonoBehaviour {

    //----------------------------------------------------------------------------------------------------------------------------------------------//
    // LoginRegisterUI handles all of the LoginRegisterScene UI elements, inlcuding setting the panel positions, widths, heights, and transitions.

    [SerializeField]
    private GameObject loginRegisterMenu;
    [SerializeField]
    private GameObject mainMenu;

    // Int variables:
    private int currentMenu = 2;

    // Float variables:
    private float targetX = 0.0f;
    private float canvasX;
    private float canvasY;
    private float logoScale;
    private float moveSpeed = 35.0f;
    private float scaleSpeed = 0.15f;

    // RectTransform variables:
    [SerializeField]
    private RectTransform panel; // Set in inspector

    // List variables:
    [SerializeField]
    private List<GameObject> fadeTargets = new List<GameObject> (); // Set in inspector

    // Color variables:
    private Color lightGrey = RGBColor.RGBtoFloat(218, 218, 218, 255);
    private Color pink = RGBColor.RGBtoFloat (238, 193, 210, 255);

    //----------------------------------------------------------------------------------------------------------------------------------------------//
    // SetPanels() only needs to be run once when the app loads up since the devide screen, and therefore the Canvas variables, won't change until the
    // app is closed and opened up on a new device, at which point SetPanels() will run again then. Also sets starting values.

    private void Awake () {
        
        SetPanels ();
        SetMoveVariables ();
        //PrepareForFade ();
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//
    // MovePanel() is constantly run as the function to smoothly move the Panel's RectTransform needs to be in Update(). FadeIn() also needs to be in
    // Update() as it uses the Lerp() function.
    
    private void Update () {
        
        MovePanel ();
        //FadeIn (0.1f);
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//
    // This function sets targetX, depending on the value of currentMenu (which is set via the SwitchMenu() function), to its respective variation
    // of the canvasX variable. targetX is used in the MovePanel() function to set the position of the Panel so that the relevant UI is shown.
    // logoScale is also used to hide/reveal the logo for each relevant menu. Runs SetCircleColors() to change their colors.

    private void SetMoveVariables () {

        if (currentMenu == 1) {
            targetX = canvasX;
            //logoScale = 0.0f;
        } else if (currentMenu == 2) {
            targetX = 0.0f;
            //logoScale = 1.0f;
        }
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//
    // This function sets the canvasX variable to the width of the Canvas RectTransform and the cavasY variable to the height. Next, the three
    // sub-panels under Panel are assigned to temporary variables and then their width and height are set to match the Canvas. Next, their positions
    // are set to the correct spacing relative to the Canvas.

    private void SetPanels () {

        canvasX = GetComponent<RectTransform> ().sizeDelta.x;
        canvasY = GetComponent<RectTransform> ().sizeDelta.y;

        RectTransform registerPanel = panel.gameObject.transform.GetChild (0).gameObject.transform.GetComponent<RectTransform> ();
        RectTransform mainPanel = panel.gameObject.transform.GetChild (1).gameObject.transform.GetComponent<RectTransform> ();

        // Set widths
        registerPanel.sizeDelta = new Vector2 (canvasX, canvasY);
        mainPanel.sizeDelta = new Vector2 (canvasX, canvasY);

        // Set positions
        registerPanel.anchoredPosition = new Vector2 (-canvasX, 0.0f);
        mainPanel.anchoredPosition = new Vector2 (0.0f, 0.0f);
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//
    // This functions uses the MoveTowards() function to constantly move Panel towards the targetX position. It does the same for the localScale of
    // the logo.

    private void MovePanel () {

        panel.anchoredPosition = Vector2.MoveTowards (panel.anchoredPosition, new Vector2 (targetX, panel.anchoredPosition.y), moveSpeed);
        //logo.localScale = Vector3.MoveTowards (logo.localScale, new Vector3 (logoScale, logoScale, logoScale), scaleSpeed);
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//
    // Each button that changes the menu will use this function and input their target menu number. When a button is pressed the currentMenu variable
    // will be set to the inputed menu number. SetMoveVariables() is then run to adjust the targetX variable.
    // 1 = Register Menu.
    // 2 = Main Menu.
    // 3 = Login Menu.

    public void SwitchMenu (int menu) {

        currentMenu = menu;
        SetMoveVariables ();
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//
    // Sets the color components of fadeTargets to have an alpha of 0.0f. This is so they will start transparent when the splash screen finishes but
    // will stay visible while I work in the editor.

    private void PrepareForFade () {
        
        foreach (GameObject g in fadeTargets) {
            if (g.transform.GetComponent<Image>() != null) {
                Image image = g.transform.GetComponent<Image> ();
                image.color = new Color (image.color.r, image.color.g, image.color.b, 0.0f);
            }
            if (g.transform.GetComponent<Text> () != null) {
                Text text = g.transform.GetComponent<Text> ();
                text.color = new Color (text.color.r, text.color.g, text.color.b, 0.0f);
            }
        }
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//
    // Fades the alpha value of the color components of fadeTargets from 0.0f to 1.0f.

    private void FadeIn (float speed) {

        foreach (GameObject g in fadeTargets) {
            if (g.transform.GetComponent<Image> () != null) {
                Image image = g.transform.GetComponent<Image> ();
                image.color = Color.Lerp (image.color, new Color (image.color.r, image.color.g, image.color.b, 1.0f), Time.time * speed);
            }
            if (g.transform.GetComponent<Text> () != null) {
                Text text = g.transform.GetComponent<Text> ();
                text.color = Color.Lerp (text.color, new Color (text.color.r, text.color.g, text.color.b, 1.0f), Time.time * speed);
            }
        }
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void OpenMenu (int menu) {

        if (menu == 1) {
            SwitchMenu (2);
            mainMenu.SetActive (false);
            loginRegisterMenu.SetActive (true);
        } else if (menu == 2) {
            SwitchMenu (2);
            loginRegisterMenu.SetActive (false);
            mainMenu.SetActive (true);
        }
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//
}
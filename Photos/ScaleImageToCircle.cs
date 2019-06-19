using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScaleImageToCircle : MonoBehaviour {

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public bool matchToWidth;
    public bool matchToHeight;
    
    private float imageHeight;
    private float imageWidth;
    private float aspectRatio;
    private float parentHeight;
    private float parentWidth;

    public RectTransform parent;

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    private void Start () {

        Scale ();
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void Scale () {

        imageHeight = transform.GetComponent<Image> ().sprite.textureRect.height;
        imageWidth = transform.GetComponent<Image> ().sprite.textureRect.width;
        
        parentWidth = parent.sizeDelta.x;
        parentHeight = parent.sizeDelta.y;

        if (imageWidth > imageHeight) {
            aspectRatio = imageWidth / imageHeight;
        } else {
            aspectRatio = imageHeight / imageWidth;
        }

        if (matchToWidth) {
            float widthMultiplier = imageWidth / parentWidth;
            transform.GetComponent<RectTransform> ().sizeDelta = new Vector2 (parentWidth, (imageHeight / widthMultiplier));
        }

        if (matchToHeight) {
            transform.GetComponent<RectTransform> ().sizeDelta = new Vector2 ((parentHeight * aspectRatio), parentHeight);
        }
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Firebase;
using Firebase.Database;

public class CommentInfo : MonoBehaviour {

    //----------------------------------------------------------------------------------------------------------------------------------------------//
    
    [SerializeField]
    private Text dateField;
    [SerializeField]
    private Text commentField;
    [SerializeField]
    private Text usernameField;
    [SerializeField]
    private Image pictureIDField;
    
    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public void AssignInfo (string userReference, string date, string comment) {
        
        DataRef.User (userReference).GetValueAsync ().ContinueWith (async (task) => {
            await new WaitForUpdate ();
            DataSnapshot snapshot = task.Result;

            usernameField.text = snapshot.Child ("Username").Value.ToString ();
            Sprite image = Resources.Load<Sprite> ("2D/Animals/" + snapshot.Child ("PictureID").Value.ToString ());
            pictureIDField.sprite = image;
            dateField.text = date;
            commentField.text = comment;
        });
	}

    //----------------------------------------------------------------------------------------------------------------------------------------------//
}
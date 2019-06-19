using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Firebase.Auth;
using Firebase.Database;

public class DataRef : MonoBehaviour {

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    private static DatabaseReference _users = FirebaseDatabase.DefaultInstance.RootReference.Child ("USERS");
    private static DatabaseReference _groups = FirebaseDatabase.DefaultInstance.RootReference.Child ("GROUPS");
    private static DatabaseReference _projects = FirebaseDatabase.DefaultInstance.RootReference.Child ("PROJECTS");
    private static DatabaseReference _circles = FirebaseDatabase.DefaultInstance.RootReference.Child ("CIRCLES");
    private static DatabaseReference _filters = FirebaseDatabase.DefaultInstance.RootReference.Child ("FILTERS");
    private static DatabaseReference _activity = FirebaseDatabase.DefaultInstance.RootReference.Child ("ACTIVITY");
    private static DatabaseReference _general = FirebaseDatabase.DefaultInstance.RootReference.Child ("GENERAL");
    private static DatabaseReference _feedback = FirebaseDatabase.DefaultInstance.RootReference.Child ("FEEDBACK");

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public static DatabaseReference AllUsers () {

        return _users;
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public static DatabaseReference User (string user) {

        return _users.Child (user);
	}

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public static DatabaseReference CurrentUser () {

        return _users.Child (FirebaseAuth.DefaultInstance.CurrentUser.UserId);
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public static DatabaseReference Projects (string project) {

        return _projects.Child (project);
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public static DatabaseReference Groups (string group) {

        return _groups.Child (group);
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public static DatabaseReference Filters (string type) {

        return _filters.Child (type);
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public static DatabaseReference AllCirlces () {

        return _circles;
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public static DatabaseReference Circles (string circle) {

        return _circles.Child (circle);
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public static DatabaseReference Activity () {

        return _activity;
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public static DatabaseReference General () {

        return _general;
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//

    public static DatabaseReference Feedback (string feed) {

        return _feedback.Child (feed);
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------//
}
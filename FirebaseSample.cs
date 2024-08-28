using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class FirebaseSample : MonoBehaviour
{
    public TextMeshProUGUI tmpObject; // Assign this in the editor or find it dynamically
    private DatabaseReference databaseReference;

    private void Start ()
    {
        tmpObject = GameObject.FindObjectOfType<TextMeshProUGUI>(); // Find the TMP object in the scene
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            FirebaseApp app = FirebaseApp.DefaultInstance;
            databaseReference = FirebaseDatabase.DefaultInstance.GetReference("data/sensor/co2");
            FetchData();
        });
    }

    void FetchData ()
    {
        databaseReference.GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                // Handle the error...
                Debug.LogError("Error getting data",this);
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                // If the expected value is a floating-point number, change the parsing method
                float value = float.Parse(snapshot.Value.ToString());
                UpdateTMP(value);
            }
        });
    }

    void UpdateTMP (float value)
    {
        if (tmpObject != null)
        {
            tmpObject.text = value.ToString();
        }
    }


}

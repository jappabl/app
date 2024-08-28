using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class SensorDataFetcher : MonoBehaviour
{
    private DatabaseReference databaseReference;
    public TextMeshProUGUI temperatureCText;
    public TextMeshProUGUI temperatureFText;
    public TextMeshProUGUI co2Text;
    public TextMeshProUGUI humidityText;

    public delegate void SensorDataUpdatedHandler (Dictionary<string,object> sensorData);
    public event SensorDataUpdatedHandler OnSensorDataUpdated;

    private void Start ()
    {

        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.Exception != null)
            {
                // Handle errors here
                Debug.LogError("Could not resolve all Firebase dependencies: " + task.Exception);
                return;
            }
            // Proceed only if dependencies are correctly resolved
            if (task.IsCompleted && task.Exception == null)
            {
                FirebaseApp app = FirebaseApp.DefaultInstance;
                databaseReference = FirebaseDatabase.DefaultInstance.GetReference("data/sensor");
                FetchSensorData();
            }
        });

    }

    protected virtual void RaiseOnSensorDataUpdated (Dictionary<string,object> data)
    {
        OnSensorDataUpdated?.Invoke(data);
    }

    void FetchSensorData ()
    {
        databaseReference.GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                // Log the error
                Debug.LogError("Error getting sensor data",this);
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                UpdateSensorUI((IDictionary<string,object>)snapshot.Value);
            }
        });
    }

    private void UpdateSensorUI (IDictionary<string,object> sensorData)
    {
        if (sensorData.TryGetValue("temp",out object tempData))
        {
            var tempDict = tempData as Dictionary<string,object>;
            if (tempDict != null)
            {
                if (tempDict.TryGetValue("c",out object temperatureC))
                {
                    temperatureCText.text = temperatureC.ToString() + "�C";
                }
                if (tempDict.TryGetValue("f",out object temperatureF))
                {
                    temperatureFText.text = temperatureF.ToString() + "�F";
                }
            }
        }
        if (sensorData.TryGetValue("co2",out object co2))
        {
            co2Text.text = co2.ToString() + " ppm";
        }
        if (sensorData.TryGetValue("humidity",out object humidity))
        {
            humidityText.text = humidity.ToString() + "%";
        }

        OnSensorDataUpdated?.Invoke(new Dictionary<string,object>(sensorData));
    }
}

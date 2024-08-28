using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;

public class ScenarioDetector : MonoBehaviour
{
    [SerializeField] private float CO2;
    [SerializeField] private float humidity;
    [SerializeField] private float temperatureC;
    [SerializeField] private float temperatureF;
    [Space]
    [SerializeField] private Sprite[] scenarioImages;
    [SerializeField] private Image scenarioImagePreview;
    [Space]
    public Button launchScenarioButton;
    public TextMeshProUGUI scenarioText;
    [Space]
    [SerializeField] private bool debugMode = false;
    [SerializeField] private int sceneToLoad;

    private DatabaseReference databaseReference;

    // Start is called before the first frame update
    void Start ()
    {
        if (!debugMode)
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
        launchScenarioButton.interactable = false;

        if (debugMode == true)
        {
            CheckForScenario();
        }

        
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
                UpdateScenarioData((IDictionary<string,object>)snapshot.Value);
            }
        });
    }

    private void UpdateScenarioData (IDictionary<string,object> sensorData)
    {
        if (sensorData.TryGetValue("co2",out object co2))
        {
            CO2 = float.Parse(co2.ToString());
        }
        if (sensorData.TryGetValue("humidity",out object humidity))
        {
            this.humidity = float.Parse(humidity.ToString());
        }
        if (sensorData.TryGetValue("temp",out object tempData))
        {
            var tempDict = tempData as Dictionary<string,object>;
            if (tempDict != null)
            {
                if (tempDict.TryGetValue("c",out object temperatureC))
                {
                    this.temperatureC = float.Parse(temperatureC.ToString());
                }
                if (tempDict.TryGetValue("f",out object temperatureF))
                {
                    this.temperatureF = float.Parse(temperatureF.ToString());
                }
            }
        }
        CheckForScenario();
    }

    private void CheckForScenario()
    {
        // Conditions needed for a fire
        if (CO2 >= 2000.0f && humidity <= 30.0f && temperatureF >= 977.0f)
        {
            scenarioText.text = ("Learn To Survive A Fire");
            scenarioImagePreview.sprite = scenarioImages[0];
            launchScenarioButton.interactable = true;
            sceneToLoad = 4;
            Debug.Log("fire detected");
        }

        else if (temperatureF >= 100.0f)
        {
            scenarioText.text = ("High heat detected");
            Debug.Log("High heat");
        }

        // Conditions checking for high co2 emissions
        else if (CO2 >= 4000.0f)
        {
            Debug.Log("high co2 exposure detected");
        }

        else
        {
            scenarioText.text = ("Normal");
            Debug.Log("conditions normal");
        }
    }

    public void LaunchScenario()
    {
        SceneManager.LoadScene(sceneToLoad);
    }
}

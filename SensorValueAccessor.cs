using UnityEngine;

public class SensorValueAccessor : MonoBehaviour
{
    private SensorDataFetcher dataFetcher;

    public float TemperatureC { get; private set; }
    public float TemperatureF { get; private set; }
    public float CO2 { get; private set; }
    public float Humidity { get; private set; }

    private void Start ()
    {
        dataFetcher = GetComponent<SensorDataFetcher>();
        if (dataFetcher == null)
        {
            Debug.LogError("SensorDataFetcher component is not found on the object.");
        }
    }

    private void Update ()
    {
        if (dataFetcher != null)
        {
            TemperatureC = ParseFloatFromTMP(dataFetcher.temperatureCText.text);
            TemperatureF = ParseFloatFromTMP(dataFetcher.temperatureFText.text);
            CO2 = ParseFloatFromTMP(dataFetcher.co2Text.text);
            Humidity = ParseFloatFromTMP(dataFetcher.humidityText.text);
        }
    }

    private float ParseFloatFromTMP (string text)
    {
        if (string.IsNullOrEmpty(text))
            return 0f;
        string[] splitText = text.Split(' ');
        return float.TryParse(splitText[0],out float value) ? value : 0f;
    }
}

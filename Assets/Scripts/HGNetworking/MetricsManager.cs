using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MetricsManager : MonoBehaviour
{
    #region Singleton Design
    private static MetricsManager _instance;

    public static MetricsManager Instance { get { return _instance; } }


    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }
    #endregion
    [SerializeField]
    [Header("Metrics")]
    private List<DataVisualization> metricsList = new List<DataVisualization>();
    private Dictionary<string, DataVisualization> metricsDictionary = new Dictionary<string, DataVisualization>();

    [Header("Text Canvas Components")]
    [SerializeField]
    private Text metricsText;
    [SerializeField]
    private Text debugText;

    private bool isUIActive = false;
    
    public static void AddDatapointToMetric(string metricName, float value, bool cumulative = false)
    {
        Debug.Log("Metrics are disabled... sorry for your loss");
        return;
        Datapoint datapoint = new Datapoint(Time.time, value);

        //If metric is cumulative, increment the value by the previous value in the animation curve
        if (cumulative && Instance.metricsDictionary.ContainsKey(metricName) && datapoint.Time != Instance.metricsDictionary[metricName].lastVal)
        {
            datapoint.Value += Instance.metricsDictionary[metricName].GetLastValue();
        }

        //If metric is not present add it to the dictionary
        AddMetricIfNotPresent(metricName);

        //Store the value in the dictionary
        Instance.metricsDictionary[metricName].Add(datapoint);
    }

    public static void AddMetricIfNotPresent(string metricName)
    {
        if (!Instance.metricsDictionary.ContainsKey(metricName))
        {
            DataVisualization dataVisualization = new DataVisualization(metricName);
            Instance.metricsDictionary.Add(metricName, dataVisualization);
            Instance.metricsList.Add(dataVisualization);
        }
    }

    #region Render UI
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.BackQuote)) {
            isUIActive = !isUIActive;
            metricsText.transform.parent.gameObject.SetActive(isUIActive);
            debugText.transform.parent.gameObject.SetActive(isUIActive);
        }

        if (isUIActive)
        {
            string tempMetricsText = "";
            foreach (DataVisualization dataVisualization in metricsDictionary.Values)
            {
                tempMetricsText += $"<color=#{dataVisualization.GetColor()}>{dataVisualization.GetName()} : {dataVisualization.GetLastValue()}</color>\n";
            }
            metricsText.text = tempMetricsText;

            string tempDebugText = "";
            foreach (Connection connection in HG.Networking.ConnectionManager.connections.Values)
            {
                tempDebugText += $"\nConnection ID: {connection.connectionId} | Ping: {(int) (connection.GetAveragePing() * 1000)}";
            }
            debugText.text = tempDebugText;
        }
    }
    #endregion

    #region DataVisualization Class 
    [System.Serializable]
    public class DataVisualization
    {
        [SerializeField]
        private string name;
        [SerializeField]
        private Dictionary<float, float> animationCurve;
        public float lastVal = 0;

        private string color;

        public DataVisualization(string name)
        {
            this.name = name;
            color = ColorUtility.ToHtmlStringRGBA(Random.ColorHSV(0f, 1f, 1f, 1f, 0.8f, 1f));
            animationCurve = new Dictionary<float, float>();
        }

        public void Add(Datapoint datapoint)    
        {
            lastVal = Mathf.Max(lastVal, datapoint.Time);
            if (animationCurve.ContainsKey(datapoint.Time))
            {
                animationCurve[datapoint.Time] += datapoint.Value;
            }
            else
            {
                animationCurve.Add(datapoint.Time, datapoint.Value);
            }
        }

        public float GetLastValue()
        {
            if(animationCurve.ContainsKey(lastVal))
            {
                return animationCurve[lastVal];
            }
            return 0f;
        }

        #region Accessors
        public string GetName()
        {
            return name;
        }

        public string GetColor()
        {
            return color;
        }
        #endregion
    }
    #endregion
}


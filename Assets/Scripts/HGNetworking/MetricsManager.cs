using System.Collections.Generic;
using UnityEngine;
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
    
    public static void AddDatapointToMetric(string metricName, float value, bool cumulative = false)
    {
        //If metric is cumulative, increment the value by the previous value in the animation curve
        if(cumulative && Instance.metricsDictionary.ContainsKey(metricName))
        {
            value += Instance.metricsDictionary[metricName].GetLastValue();
        }

        //Store the value in the dictionary
        Datapoint datapoint = new Datapoint(Time.time, value);
        if (!Instance.metricsDictionary.ContainsKey(metricName))
        {
            DataVisualization dataVisualization = new DataVisualization(metricName);
            Instance.metricsDictionary.Add(metricName, dataVisualization);
            Instance.metricsList.Add(dataVisualization);
        }
        Instance.metricsDictionary[metricName].Add(datapoint);
    }

    #region DataVisualization Class 
    [System.Serializable]
    public class DataVisualization
    {
        [SerializeField]
        private string name;

        [SerializeField]
        private AnimationCurve animationCurve;
        
        public DataVisualization(string name)
        {
            this.name = name;
            animationCurve = new AnimationCurve();
        }

        public void Add(Datapoint datapoint)
        {
            animationCurve.AddKey(datapoint.Time, datapoint.Value);
        }

        public float GetLastValue()
        {
            if(animationCurve.length == 0)
            {
                return 0;
            }
            return animationCurve.keys[animationCurve.length-1].value;
        }
    }
    #endregion
}


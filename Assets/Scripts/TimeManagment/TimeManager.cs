using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class TimeManager : MonoBehaviour
{
    [SerializeField]
    public float timeSlowDownRate = .5f;
    [SerializeField]
    public float timeSpeedUpRate = .5f;
    [SerializeField]
    public float timeStopLength = 4;

    public float timeTillLength = 0; // this is used in EaseTimeToDefault() to run the while statement until it reaches timeStopLength;
    private float defaultTimeScale;
    private float defaultFixedDeltaTime;
    [SerializeField] private bool isTimeStopped = false;

    /*
     * singleton to ensure we only have 1 time manager
     */
    private static TimeManager _instance;
    public static TimeManager Instance { get { return _instance; } }

    private void Awake()
    {
        /*
         * deletes this game object of one instance of time manager exists already
         */
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        defaultTimeScale = Time.timeScale;
        defaultFixedDeltaTime = Time.fixedDeltaTime;
        
    }

    // Update is called once per frame
    void Update()
    {
        TimeLeft();
        FreezeTime();
        Debug.Log(Time.timeScale);
        Debug.Log(isTimeStopped);
    }

    /*
     * this is the method used for the time stop mechanic
     */
    void TimeStop()
    {
        Debug.Log("Time has been stopped");
        Time.timeScale = .1f;
        
    }

    void FreezeTime()
    {
        if (Input.GetKeyDown(KeyCode.L) && timeTillLength > 0)
        {
            TimeStop();
            isTimeStopped = true;
        }

        // code to undo the time pause if activated
        if (Input.GetKeyDown(KeyCode.E) && isTimeStopped == true)
        {
            isTimeStopped = false;
            EaseTimeToDefault();
        }
    }

    void TimeLeft()
    {
        if (timeTillLength <= timeStopLength && isTimeStopped == true)
        {
            timeTillLength--;
        }

        if (timeTillLength <= 0)
        {
            EaseTimeToDefault();
        }
    }

    /*
     * Method to gradually reset time to 1. 
     * may need to rename
     */
    void EaseTimeToDefault()
    {
        while (Time.timeScale < 1f)
        {
            Debug.Log("Time has started to revert back");
            Time.timeScale += (1f / timeStopLength) * Time.unscaledDeltaTime;
            Time.fixedDeltaTime = Time.timeScale * .02f;
        }

        // original code do not delete!
        /*if (Time.timeScale < 1f)
        {
            Debug.Log("Time has started to revert back");
            Time.timeScale += (1f / timeStopLength) * Time.unscaledDeltaTime;
            Time.fixedDeltaTime = Time.timeScale * .02f;
        }*/

        // testing code
        /*for (int i = 0; i < 1f; i++)
        {
            if (Time.timeScale < 1f)
            {
            Debug.Log("Time has started to revert back");
            Time.timeScale += (1f / timeStopLength) * Time.unscaledDeltaTime;
            Time.fixedDeltaTime = Time.timeScale * .02f;
            }

            Debug.Log("Time is back to normal");
        }*/

        
    }
}

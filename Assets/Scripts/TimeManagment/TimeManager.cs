using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeManager : MonoBehaviour
{
    [SerializeField]
    public float timeSlowDownRate = .5f;
    [SerializeField]
    public float timeSpeedUpRate = .5f;
    [SerializeField]
    public float timeStopLength = 4;

    //REMOVE LATER - Just for showing off new Timestop
    [SerializeField]
    public Image timeStopReadyIndicator;
     
    //The timer variable for the time stop
    float timeValue;

    //The cooldown timer variable
    float coolDownValue;

    //Keeps track of when the timer times out, turns true when the timer runs out and false after
    //the cooldown
    bool outtaTime;

   

    public float timeTillLength = 0; // this is used in EaseTimeToDefault() to run the while statement until it reaches timeStopLength;
    private float defaultTimeScale;
    private float defaultFixedDeltaTime;
    [SerializeField] private bool isTimeStopped = false;

    /*
     * singleton to ensure we only have 1 time manager -A
     */
    private static TimeManager _instance;
    public static TimeManager Instance { get { return _instance; } }

    private void Awake()
    {
        /*
         * deletes this game object of one instance of time manager exists already -A
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
        timeValue = 4f; //This will (hopefully) give the player 4 seconds total of meter
        coolDownValue = 4f;

        outtaTime = false;
        
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(timeValue);
        Debug.Log(coolDownValue);
        //Debug.Log(isTimeStopped);
        //TimeLeft();
        FreezeTime();

        Cooldown();

        EaseTimeToDefault();

        //Cooldown();
        
        Timer();
        AmIOuttaTime();
        //Debug.Log(Time.timeScale);
        //Debug.Log(isTimeStopped);

        //REMOVE LATER -just for demonstration purposes -Ryan
        if(outtaTime)
        {
            timeStopReadyIndicator.gameObject.SetActive(false);
        }
        else
        {
            timeStopReadyIndicator.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// Checks on update whether or not the player has time meter left
    /// </summary>
    void AmIOuttaTime()
    {
        if (timeValue <= 0)
        {
            if (!outtaTime)
            {
                coolDownValue = 0;
            }
            outtaTime = true;

        }
        if (coolDownValue >= 4f)
        {
            if (outtaTime)
            {
                timeValue = 4f;
            }
            outtaTime = false;
        }
    }

    /*
     * this is the method used for the time stop mechanic -A
     */
    void TimeStop()
    {
        //Debug.Log("Time has been stopped");
        if (!outtaTime && Time.timeScale > .1f)
        {
            //Debug.Log("AYEAH");
            isTimeStopped = true;
            Time.timeScale -= timeSlowDownRate;
        }
        
    }

    /// <summary>
    /// Adds to the cool down
    /// </summary>
    void Cooldown()
    {
        if (coolDownValue < 4f && outtaTime)
        {
            coolDownValue += Time.unscaledDeltaTime;
        }
    }

    void FreezeTime()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            TimeStop();
            
        }
        if (!Input.GetKey(KeyCode.LeftShift))
        {
            //TimeStop();
            isTimeStopped = false;
        }
        //if (!Input.GetKeyDown(KeyCode.LeftShift) && !outtaTime)
        //{
        //    isTimeStopped = false;
        //}

        //// code to undo the time pause if activated
        //if (Input.GetKeyDown(KeyCode.E) && isTimeStopped == true)
        //{
        //    isTimeStopped = false;
        //    EaseTimeToDefault();
        //}
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

    /// <summary>
    /// Increments the timer for the time stop, decreasing while time stop is active and
    /// refilling it when it's not
    /// </summary>
    void Timer()
    {
        if (isTimeStopped && !outtaTime)
        {
            timeValue -= Time.unscaledDeltaTime;
            
        }
        //else
        //{
        //    if (timeValue <= 0 && timeValue < .4f)
        //    {
        //        timeValue += Time.deltaTime;
        //    }
        //}
    }

    /*
     * Method to gradually reset time to 1. may need to rename -A
     */
    void EaseTimeToDefault()
    {
        //while (Time.timeScale < 1f)
        //{
        //    Debug.Log("Time has started to revert back");
        //    Time.timeScale += (1f / timeStopLength) * Time.unscaledDeltaTime;
        //    Time.fixedDeltaTime = Time.timeScale * .02f;
        //}

        // original code do not delete!
        if (Time.timeScale < 1f)
        {
            //Debug.Log("Time has started to revert back");
            Time.timeScale += ((1f / timeStopLength) * Time.unscaledDeltaTime) * timeSpeedUpRate;
            Time.fixedDeltaTime = Time.timeScale * .02f;
        }

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

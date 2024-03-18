using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class StopWatch : MonoBehaviour
{
    bool initialized = false;
    private int minutos, segundos, cents;
    //the current time set in update
    float currentTime;

    //the time that this started at
    float initialTime;

    //used to track the time before pauses
    float stackedTime;

    //if the timer has started yet
    bool started;

    //invokes this method to send the time
    private UnityAction<float> getFloatTime;

    [Header("Si no es nulo, este texto se actualizará en FixedUpdate.")]
    public Text timerText;

    [Header("Esto mostrará el tiempo transcurrido desde el inicio")]
    public float timeElapsed;

    [SerializeField] bool hasFirstClick = true;

    private void Update()
    {
        if (!hasFirstClick && Input.GetMouseButtonDown(0))
        {
            hasFirstClick = true;
            ResumeStopWatch();
        }
    }

    private void FixedUpdate()
    {
        if (started)
        {
            currentTime = Time.time;
            timeElapsed = currentTime - initialTime + stackedTime;

            getFloatTime?.Invoke(timeElapsed);

            if (timerText != null)
            {
                timerText.text = minutos.ToString("00") +":" + segundos.ToString("00") +":" + cents.ToString("00");
                minutos = (int)(currentTime / 60f);
                segundos = (int)(currentTime - minutos * 60F);
                cents = (int)((currentTime - (int)currentTime) * 100f);
                //timeLabel.text = string.Format("{0}:{1}:{2}", minutos, segundos, cents);
                //string.Format("{0:0}:{1:0}:{2:0}", minutos, segundos, cents);// + timeElapsed;

            }
        }
    }

    /**
     * A basic timer, does not set text or call actions
     */
    public void StartStopWatch()
    {
        initialTime = Time.time;
        timeElapsed = 0f;
        stackedTime = 0f;
        currentTime = 0f;
        started = true;
        initialized = true;
        hasFirstClick = false;

        PauseStopWatch();
    }
    
    /**
     * A basic timer that also calls a function during FixedUpdate
     */
    public void StartStopWatch(UnityAction<float> getFloatTime)
    {
        StartStopWatch();
        this.getFloatTime = getFloatTime;
    }

    /**
     * A basic timer that sets text on every update
     */
    public void StartStopWatch(Text timerText)
    {
        StartStopWatch();
        this.timerText = timerText;
    }

    /**
     * A multifunctional timer 
     * in FixedUpdate:
     *      calls function with float
     *      sets text to current time
     */
    public void StartStopWatch(UnityAction<float> getFloatTime, Text timerText)
    {
        StartStopWatch();
        this.getFloatTime = getFloatTime;
        this.timerText = timerText;
    }

    /**
     * Stops the counting, 
     * sets the time to 0,
     * and if there is text or a function attached
     *      sets those to 0 as well
     *  
     * This does not destroy the existing UnityAction or Text
     * a basic StartStopWatch() call will restart the timer with no issues.
     */
    public void StopStopWatch()
    {
        hasFirstClick = true;
        started = false;
        currentTime = 0;
        initialTime = 0;
        stackedTime = 0;
        timeElapsed = 0;
        getFloatTime?.Invoke(0);

        if (timerText != null)
        {
            timerText.text = "0";
        }
    }

    /**
     * If stopWatch is started, pause it
     * 
     * If stopWatch is paused, resume it
     */
    public void StopWatchToggle()
    {
        if (!initialized)
        {
            Debug.LogError("StopWatch is not initialized");
            return;
        }

        started = !started;

        //just turned back on
        if (started)
        {
            ResumeStopWatch();
        }

        //just turned off
        else
        {
            PauseStopWatch();
        }
    }

    /**
     * Pauses the stopWatch
     */
    public void PauseStopWatch()
    {
        if (!initialized)
        {
            Debug.LogError("StopWatch is not initialized");
            return;
        }

        if (!started) return;

        stackedTime = timeElapsed;
        started = false;
        timeElapsed = 0;
    }

    /**
     * Resumes the stopWatch
     */
    public void ResumeStopWatch()
    {
        if (!initialized)
        {
            Debug.LogError("StopWatch is not initialized");
            return;
        }

        if (started) return;

        started = true;
        initialTime = Time.time;
    }

    /**
     * Stops and starts the stopWatch
     */
    public void RestartStopWatch()
    {
        StopStopWatch();
        StartStopWatch();
    }
}

using UnityEngine;
using System.IO;
using System.IO.Pipes;
using System.Threading;
using System;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class NamedPipeServer : MonoBehaviour
{
    public string pipeName = "Driving";
    private static NamedPipeServerStream pipeServer;
    private bool isRunning = true;
    public bool isConnected = false;    
    private static StreamReader reader;
    public int currentSceneIndex;
    public string prevLine;

    [SerializeField]
    public PipeValue[] pipeValues;

    //[SerializeField]
    //public Channel[] eegChannels;

    private void Awake()
    {
        //eegChannels = new Channel[5];


        //QualitySettings.vSyncCount = 1;
        Application.targetFrameRate = 120;

        DontDestroyOnLoad(this);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        

    }

    private void Start()
    {
        //Application.quitting += HandleApplicationQuit;

        NamedPipeServer[] namePipeServers = GameObject.FindObjectsByType<NamedPipeServer>(FindObjectsSortMode.None);

        foreach (NamedPipeServer namePipeServer in namePipeServers)
        {
            if (namePipeServer != this) { Destroy(namePipeServer.gameObject); Time.timeScale = 1; return; }
        }

        // Register the Application.quitting event to handle application exit
        //Application.quitting += HandleApplicationQuit;

       
        
        if (pipeServer == null)
            StartNamedPipeServer();



    }

    private void HandleApplicationQuit()
    {
        // Set isRunning to false to exit the server loop gracefully
        isRunning = false;
        if (reader != null)
        {
            reader.Close();
            reader.Dispose();
            reader = null;
        }
        
        // Close the pipe server if it was created
        if (pipeServer != null)
        {
            try
            {
                //pipeServer.EndWaitForConnection(result);
                pipeServer.Close();
                pipeServer.Dispose();
                Debug.Log("Pipe server closed.");
            }
            catch (IOException ex)
            {
                Debug.LogError("An error occurred while closing the pipe server: " + ex.Message);
            }
        }
    }

    private void StartNamedPipeServer()
    {
        // Start a coroutine to handle the named pipe server asynchronously
        StartCoroutine(NamedPipeServerCoroutine());
    }
    //int i = 0;
    IAsyncResult result;

    private IEnumerator NamedPipeServerCoroutine()
    {
        while (isRunning)
        {
            if (pipeServer == null || !pipeServer.IsConnected)
            {
                // Replace the named pipe server creation with async version for Unity
                pipeServer = new NamedPipeServerStream(pipeName,
                    PipeDirection.InOut, 1, PipeTransmissionMode.Byte,
                    PipeOptions.Asynchronous);

                // Start the asynchronous operation to wait for the client connection
                result = pipeServer.BeginWaitForConnection(OnClientConnected, null);
            }
            // Wait for the connection or timeout
            float startTime = Time.time;
            while (!result.IsCompleted)
            {
                yield return null; // Yield until the next frame
            }

            if (pipeServer == null || !pipeServer.IsConnected)
            {
                // Complete the connection process
                pipeServer.EndWaitForConnection(result);
            }


            isRunning = false;
        }

    }

    public void OnApplicationQuit()
    {
        HandleApplicationQuit();    

    }

    bool endSceneStart = false;
    static bool IsPipeBrokenException(IOException ex)
    {
        const int ERROR_PIPE_NOT_CONNECTED = 233;
        const int ERROR_NO_DATA = 232;

        var errorCode = ex.HResult & 0xFFFF;
        return errorCode == ERROR_PIPE_NOT_CONNECTED || errorCode == ERROR_NO_DATA;
    }
    public void LateUpdate()
    {
        currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        if(reader == null) return;

        ReadMessage();

    }




    private void OnClientConnected(IAsyncResult result)
    {
        reader = new StreamReader(pipeServer);
        isConnected = true;
        try
        {
            pipeServer.ReadByte();
            Debug.Log("The pipe stream is not broken.");
        }
        catch (IOException ex) when (IsPipeBrokenException(ex))
        {

            Debug.Log("The pipe stream is broken.");
        }

        print("OnClientConnected()");
    }




    public string lastestLine;
    public string test;
    public void ReadMessage()
    {
        if(reader == null) return;
        lastestLine = reader.ReadLine();
        if (lastestLine == null) { return; }
        if (lastestLine.Length == 0) { return; }
        //string test = "{\"EEG\":\"false\",\"Events\":{\"Interval\":\"\",\"Pause\":\"\"},\"Keys\":{\"Left\":\"false\",\"Right\":\"false\"}}";
        //lastestLine = "{\"EEG\":\"false\",\"Events\":{\"Interval\":\"\",\"Pause\":\"\"},\"Keys\":{\"Left\":\"false\",\"Right\":\"false\"}}";

        JObject json = new JObject();

        try
        {
            json = JObject.Parse(lastestLine);

            print(json["Keys"]);
            print(json["Keys"]["Left"]);
        }
        catch
        {
            return;
        }


        for (int i = 0; i < pipeValues.Length; i++)
        {
            if (pipeValues[i].valueType == pipeValueType.Keyboard)
            {
                print("2 " + json["Keys"][pipeValues[i].valueName].ToString());

                pipeValues[i].value = json["Keys"][pipeValues[i].valueName].ToString();

                if (pipeValues[i].value == "true")
                {
                    pipeValues[i].unityEvent.Invoke();
                }
            }
            if (pipeValues[i].valueType == pipeValueType.Event)
            {
                pipeValues[i].value = json["Events"][pipeValues[i].valueName].ToString();

                if (pipeValues[i].value == "true")
                {
                    pipeValues[i].unityEvent_TrueState.Invoke();
                }
                else if (pipeValues[i].value == "false")
                {
                    pipeValues[i].unityEvent_FalseState.Invoke();
                }
            }
            else
            {
                if (!json.ContainsKey(pipeValues[i].valueName)) continue;
                pipeValues[i].value = json[pipeValues[i].valueName].ToString();
            }

        }






        prevLine = lastestLine;





    }




    //[ContextMenu("Force Interval End")]
    //public void ForceIntervalEnd()
    //{
    //    endSceneRefScript.IntervalEnd();
    //    if (currentSceneIndex > 1)
    //    {
    //        soundControl.AdjustAll();
    //        Pause intervalPause = GameObject.FindObjectOfType<CanvasScript>().intervalPause;
    //        if (intervalPause.pauseMenu.activeInHierarchy)
    //            intervalPause.pauseMenu.SetActive(false);
    //    }
    //}


    //[ContextMenu("Force Interval Start")]
    //public void ForceIntervalStart()
    //{
    //    if (!gameTimerScript.gameEnded)
    //    {
    //        soundControl.AdjustAmbient(0.4f);
    //        soundControl.AdjustBGVolume(0.4f);

    //        gameTimerScript.Interval();
    //    }
    //}
}




public enum pipeValueType { Bool, Int, String, Float, Event, Keyboard}
[Serializable]
public struct PipeValue
{
    public string valueName;
    public string value;
    public pipeValueType valueType;
    public UnityEvent unityEvent;
    public UnityEvent unityEvent_TrueState;
    public UnityEvent unityEvent_FalseState;
}

public enum ChannelId { R, A, B, C, D }
[Serializable]
public struct Channel
{
    public ChannelId Id;
    public float value;
    public float threshold;
    public bool ignore;
}



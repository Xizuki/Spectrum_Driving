using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XizukiMethods
{
    /* Documnetation 




    */


    public class Xi_Addon_Timer_Advanced : MonoBehaviour       // Not Finished
    {
        [SerializeField]
        public List<Timers_Advanced> timers = new List<Timers_Advanced> { };

        public void Update()
        {
            if (timers.Count <= 0) { return; }
            for (int i = 0; i < timers.Count; i++)
            {
                timers[i].curTime += Time.deltaTime;
            }
        }

        public void Set(string _id, float _curTime, float _goalTime, bool _isLoop, bool _destroyWhenDone)
        {
            Timers_Advanced newTimer = new Timers_Advanced(_id, _curTime, _goalTime, _isLoop, _destroyWhenDone);
            timers.Add(newTimer);
        }
        public void Set(string _id, float _curTime, float _goalTime, bool _isLoop, bool _destroyWhenDone, List<float> _checkpoints)
        {
            Timers_Advanced newTimer = new Timers_Advanced(_id, _curTime, _goalTime, _isLoop, _destroyWhenDone, _checkpoints);
            timers.Add(newTimer);
        }

        public bool Check(string _id)
        {
            for (int i = 0; i < timers.Count; i++)
            {
                if (timers[i].id == _id)
                {
                    if (timers[i].curTime >= timers[i].goalTime)
                    {
                        if (timers[i].isLoop) { timers[i].curTime = 0; }
                        if (timers[i].destroyWhenDone) { timers.Remove(timers[i]); }
                        return true;
                    }
                }
            }
            return false;
        }
        public int CheckPoint(string _id)
        {
            for (int i = 0; i < timers.Count; i++)
            {
                if (timers[i].id != _id) { continue; }             
                if (timers[i].checkpoints == null) { return -1; }   

                /*
                if (timers[i].curTime >= timers[i].checkpoints[timers[i].checkpoints.Count])
                {
                    if (timers[i].destroyWhenDone) { timers.Remove(timers[i]); continue; }
                    if (timers[i].isLoop) { timers[i].curTime = 0; }
                    return timers[i].checkpoints.Count - 1;
                }
                */

                for (int o = 1; o < timers[i].checkpoints.Count; o++)
                {
                    if (timers[i].curTime > timers[i].checkpoints[o - 1] && timers[i].curTime <= timers[i].checkpoints[o])
                    {
                        return o-1;
                    }
                }
            }
            return -1;
        }

        /* public int CheckPoint(string _id)
         {
             for (int i = 0; i < timers.Count; i++)
             {
                 if(timers[i].id != _id) { continue; }
                 if(timers[i].checkpoints == null) { continue; }

                 if(timers[i].curTime >= timers[i].checkpoints[timers[i].checkpoints.Count - 1]) 
                 {
                     if (timers[i].destroyWhenDone) { timers.Remove(timers[i]); continue; }
                     if (timers[i].isLoop) { timers[i].curTime = 0; }
                     return timers[i].checkpoints.Count - 1; 
                 }

                 for (int o = 1; o < timers[i].checkpoints.Count; o++)
                 {
                     if (timers[i].curTime > timers[i].checkpoints[o-1] && timers[i].curTime < timers[i].checkpoints[o])
                     {
                         return o;
                     }
                 }
             }
             return -1;
         }
        */
         public bool Running(string _id)
         {
             foreach(Timers_Advanced timer in timers)
             {
                 if (timer.id == _id) return true;
             }
             return false;
         }
        

        /*
        public bool CheckPoint(int checkpoint_id)
        {
            return null;
        }

        */

        [System.Serializable]
        public class Timers_Advanced
        {
            public Timers_Advanced(string _id, float _curTime, float _goalTime, bool _isLoop, bool _destroyWhenDone, List<float> _checkpoints)
            {
                id = _id; curTime = _curTime; goalTime = _goalTime; isLoop = _isLoop; destroyWhenDone=_destroyWhenDone;

                checkpoints = new List<float> { 0 };
                for (int i = 0; i < _checkpoints.Count; i++)
                {
                    checkpoints.Add(_checkpoints[i]);
                }
                checkpoints.Add(_goalTime);
            }
            public Timers_Advanced(string _id, float _curTime, float _goalTime, bool _isLoop, bool _destroyWhenDone)
            {
                id = _id; curTime = _curTime; goalTime = _goalTime; isLoop = _isLoop; destroyWhenDone = _destroyWhenDone;
            }
            public string id;
            public float curTime;
            public float goalTime;
            public bool isLoop;
            public bool destroyWhenDone;

            public List<float> checkpoints;
        }

        [ContextMenu("TestAddTimer")]
        public void Test()
        {
            Set("Test Id2", 0, 5f, true, false, new List<float> { 0, 1, 2, 3, 4, 4.99f});
        }
    }

}


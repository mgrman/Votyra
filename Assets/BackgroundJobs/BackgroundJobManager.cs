//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading;
//using UnityEngine;

//public class BackgroundJobManager : IDisposable
//{
//    private Thread _thread;
//    private bool _stop;

//    private int _idCounter = 0;
//    private object _idLock = new object();

//    private object _jobsLock = new object();
//    private Dictionary<int, Job> _jobs;

//    private Job _activeJob;

//    private static object _instanceLock = new object();

//    private static BackgroundJobManager _instance;

//    public static BackgroundJobManager Instance
//    {
//        get
//        {
//            lock (_instanceLock)
//            {
//                _instance = _instance ?? new BackgroundJobManager();
//            }
//            return _instance;
//        }
//    }

//    public int SleepFor = 100;

//    private BackgroundJobManager()
//    {
//        _thread = new Thread(ExecuteJobs);
//        _thread.Start();
//        _jobs = new Dictionary<int, Job>();
//    }

//    private void ExecuteJobs()
//    {
//        while (true)
//        {
//            try
//            {
//                if (_stop)
//                {
//                    return;
//                }

//                var jobs = PopJobs();
//                if (jobs.Any())
//                {
//                    foreach(var job in jobs)
//                    {
//                        job.Compute();
//                    }
                    
//                }
//                else
//                {
//                    Thread.Sleep(SleepFor);
//                }
//            }
//            catch(Exception ex) {
//                Debug.Log(string.Format("Exceptions during backgroundJob {0}", ex.Message));
//            }
//        }
//    }
    

//    public void PushJob(Action job, int id)
//    {
//        lock (_jobsLock)
//        {
//            if (_jobs[id] == null)
//            {
//                _jobs[id] = new Job(id,  job);
//            }
//        }
//    }

//    public int GetId()
//    {
//        int id;
//        lock (_idLock)
//        {
//            id = _idCounter;
//            _idCounter++;
//            _jobs[id] = null;
//        }
//        return id;
//    }

//    public void RevokeId(int id)
//    {
//        lock (_idLock)
//        {
//            if (_jobs.ContainsKey(id))
//            {
//                _jobs.Remove(id);
//            }
//        }
//    }
    

//    private Job[] PopJobs()
//    {
//        Job[] jobs;
//        lock (_jobsLock)
//        {
//            jobs = _jobs.Values.Where(o => o != null).ToArray();

//            foreach (var key in _jobs.Keys.ToArray())
//            {
//                _jobs[key] = null;
//            }
//        }
//        return jobs;
//    }

//    public void Dispose()
//    {
//        if (_thread != null)
//        {
//            _stop = true;
//            _thread = null;
//        }
//    }

//    private class Job
//    {
//        public readonly int Id;
//        public readonly Action Compute;
//        public Job(int id, Action job)
//        {
//            Id = id;
//            Compute = job;
//        }
//    }
//}



using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System;
using System.Collections.Concurrent;

public class ThreadPool : IDisposable
{
    private ConcurrentQueue<Action> threadTask;
    public ConcurrentQueue<Action> returnTask;
    private List<Thread> workingThread;
    private AutoResetEvent workSignal;
    private bool isWorking = true;

    public ThreadPool(int threadCount)
    {
        threadTask = new ConcurrentQueue<Action>();
        returnTask = new ConcurrentQueue<Action>();
        workingThread = new List<Thread>();
        workSignal = new AutoResetEvent(false);

        for (int i = 0; i < threadCount; i++)
        {
            Thread newThread = new Thread(ThreadWork);
            newThread.IsBackground = true;
            workingThread.Add(newThread);
            newThread.Start();
        }
    }

    private void ThreadWork()
    {
        while (true)
        {
            workSignal.WaitOne();
            if (isWorking == false) break;

            while (threadTask.TryDequeue(out var task))
            {
                try
                {
                    task();
                }
                catch (Exception exception)
                {
                    Debug.LogError("Thread Manager Error : " + exception);
                }
            }
        }
    }

    public void EnqueueWorkQueue(Action action)
    {
        if (action == null) return;

        threadTask.Enqueue(action);
        workSignal.Set();
    }

    public void EnqueueReturnQueue(Action action)
    {
        if (action == null) return;

        returnTask.Enqueue(action);
    }

    public void Dispose()
    {
        isWorking = false;

        for (int i = 0; i < workingThread.Count; i++)
        {
            workSignal.Set();
        }

        foreach (var thread in workingThread)
        {
            thread.Join();
        }
    }
}

public class ThreadManager : Singleton<ThreadManager>, IInitializeInter
{
    private ThreadPool threadPool;

    public void Initialize()
    {
        // Environment.ProcessorCount 로 CPU 코어 수에 대응하여 사용 가능
        threadPool = new ThreadPool(4);
        testTask();
    }
    
    private void Update()
    {
        while (threadPool.returnTask.TryDequeue(out var task))
        {
            task();
        }
    }

    public void testTask()
    {
        threadPool.EnqueueWorkQueue(() =>
        {
            threadPool.EnqueueReturnQueue(() =>
            {
                transform.position = new Vector3(0, 0, 0);
            });
        });
    }

    public void AddTaskOnQueue(Action taskAction, Action returnAction)
    {
        taskAction += () => threadPool.EnqueueReturnQueue(returnAction);
        threadPool.EnqueueWorkQueue(taskAction);
    }

    void OnDestroy()
    {
        threadPool.Dispose();
    }
}

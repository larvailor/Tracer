﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TracerLib
{
    public class Tracer : ITracer
    {
        TracerResult tracerResult { get; set; }
        private ConcurrentDictionary<int, ThreadTracer> cdThreadTracers;
        static private object locker = new object();

        public Tracer()
        {
            cdThreadTracers = new ConcurrentDictionary<int, ThreadTracer>();
        }

        public void StartTrace()
        {
            ThreadTracer curThreadTracer = AddOrGetThreadTracer(Thread.CurrentThread.ManagedThreadId);
            curThreadTracer.StartTrace();
        }

        public void StopTrace()
        {
            ThreadTracer currThreadTracer = AddOrGetThreadTracer(Thread.CurrentThread.ManagedThreadId);
            currThreadTracer.StopTrace();
        }

        public TracerResult GetTraceResult()
        {
            tracerResult = new TracerResult(cdThreadTracers);
            return tracerResult;
        }

        private ThreadTracer AddOrGetThreadTracer(int id)
        {
            lock(locker)
            {
                if (!cdThreadTracers.TryGetValue(id, out ThreadTracer threadTracer))
                {
                    threadTracer = new ThreadTracer(id);
                    cdThreadTracers[id] = threadTracer;
                }
                return threadTracer;
            }
        }
    }
}

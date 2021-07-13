using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Schd
{
    class Result
    {
        public int processID;
        public int startP;
        public int burstTime;
        public int waitingTime;
        public int responsetime;
        public int turnaroundtime;
        public int totalwaitingTime;

        public Result(int processID, int startP, int burstTime, int waitingTime, int responsetime,int turnaroundtime,int totalwaitingTime)
        {
            this.processID = processID;
            this.startP = startP;
            this.burstTime = burstTime;
            this.waitingTime = waitingTime;
            this.responsetime = responsetime;
            this.turnaroundtime = turnaroundtime;
            this.totalwaitingTime = totalwaitingTime;
        }
    }
}

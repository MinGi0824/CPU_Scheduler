using System.Collections.Generic;
using System.Linq;
namespace Schd
{
    class ReadyQueueElement
    {
        public int processID;       
        public int burstTime;
        public int waitingTime;
        public double priority;          //HRRN 스케쥴링 계산을 위해 double형
        public int arriveTIme;
        public bool check=false;         //responseTime계산을 위해 프로세스가 처음시작되는건지 확인하는 변수
        public int responseTime; 
        public int firstburstTime=0;    //선점형 스케쥴링에서 사용되는 변수
        public int totalwaitingTime;    //프로세스별 누적대기시간

        public ReadyQueueElement(int processID, int burstTime, int waitingTime, double priority, int arriveTIme,bool check,int firstburstTime,int totalwaitingTime,int responseTime)
        {
            this.processID = processID;
            this.burstTime = burstTime;
            this.waitingTime = waitingTime;
            this.priority = priority;
            this.arriveTIme = arriveTIme;
            this.responseTime = 0;
            this.check=check;
            this.firstburstTime = firstburstTime;
            this.totalwaitingTime = totalwaitingTime;
            this.responseTime = responseTime;
        }
    }

    class FCFS
    {
        public static List<Result> Run(List<Process> jobList, List<Result> resultList)
        {
            int currentProcess = 0;     //현재 실행중인 프로세스ID
            int cpuTime = 0;            //현재 실행중인 프로세스 동작한 시간
            int cpuDone = 0;            //현재 실행중인 프로세스 끝나는 시간
            int runTime = 0;            //전체 실행시간

            List<ReadyQueueElement> readyQueue = new List<ReadyQueueElement>();     //readtQueue 생성
            do
            {
                while (jobList.Count != 0)           //jobList가 비어있지 않을 때
                {
                    Process frontJob = jobList.ElementAt(0);    //jobList에 0번째를 frontJob으로 지정
                    if (frontJob.arriveTime == runTime)         //frontJob의 도착시간이랑 실행시간이 같으면 실행
                    {
                        readyQueue.Add(new ReadyQueueElement(frontJob.processID, frontJob.burstTime, 0, frontJob.priority, frontJob.arriveTime, false, frontJob.burstTime, 0, 0));   //readyQueue에 frontJob추가
                        jobList.RemoveAt(0);                    //jobList에서 frontJob 삭제
                    }
                    else break;
                }

                if (currentProcess == 0)            //현재 실행중인 프로세스가 없을 때
                {
                    if (readyQueue.Count != 0)       //readyQueue가 비어있지 않을 때
                    {
                        ReadyQueueElement rq = readyQueue.ElementAt(0);      //readyQueue에 0번째를 rq로 지정

                        resultList.Add(new Result(rq.processID, runTime, rq.burstTime, rq.waitingTime, runTime - rq.arriveTIme, rq.burstTime + rq.waitingTime, rq.waitingTime));    //resultList에 rq 추가
                        cpuDone = rq.burstTime;
                        cpuTime = 0;
                        currentProcess = rq.processID;
                        readyQueue.RemoveAt(0);             //readyQueue에서 rq 삭제
                    }
                }
                else                                //현재 실행중인 프로세스가 존재할때
                {
                    if (cpuTime == cpuDone)         //프로세스 실행 완료 시
                    {
                        currentProcess = 0;         //현재 실행중 프로세스 없음으로 변경
                        continue;
                    }
                }

                cpuTime++;
                runTime++;

                for (int i = 0; i < readyQueue.Count; i++)
                {
                    readyQueue.ElementAt(i).waitingTime++;      //readyQueue에 있는 프로세스들 대기시간 증가
                }

            } while (jobList.Count != 0 || readyQueue.Count != 0 || currentProcess != 0);

            return resultList;
        }
    }
    class SJF
    {
        public static List<Result> Run(List<Process> jobList, List<Result> resultList)
        {
            int currentProcess = 0;
            int cpuTime = 0;
            int cpuDone = 0;
            int runTime = 0;

            int min = 0;        //burstTime이 가장 짧은 프로세스ID

            List<ReadyQueueElement> readyQueue = new List<ReadyQueueElement>();

            do
            {
                while (jobList.Count != 0)
                {
                    Process frontJob = jobList.ElementAt(0);
                    if (frontJob.arriveTime == runTime)
                    {
                        readyQueue.Add(new ReadyQueueElement(frontJob.processID, frontJob.burstTime, 0, frontJob.priority, frontJob.arriveTime, false, frontJob.burstTime, 0, 0));
                        jobList.RemoveAt(0);
                    }
                    else break;
                }

                ReadyQueueElement tmp = null;                  //오름차순 정렬에 사용할 임시저장 변수

                if (currentProcess == 0)
                {
                    if (readyQueue.Count != 0)
                    {
                        for (int i = 0; i < readyQueue.Count - 1; i++)      //readyQueue에 있는 프로세스들을 bursttime을 기준으로 오름차순 정렬
                        {
                            min = i;
                            for (int j = i + 1; j < readyQueue.Count; j++)
                            {
                                if (readyQueue[i].burstTime > readyQueue[j].burstTime)
                                    min = j;
                            }
                            tmp = readyQueue[min];
                            readyQueue[min] = readyQueue[i];
                            readyQueue[i] = tmp;

                        }
                        ReadyQueueElement rq = readyQueue.ElementAt(0);
                        resultList.Add(new Result(rq.processID, runTime, rq.burstTime, rq.waitingTime, runTime - rq.arriveTIme, rq.burstTime + rq.waitingTime, rq.waitingTime));
                        cpuDone = rq.burstTime;
                        cpuTime = 0;
                        currentProcess = rq.processID;
                        readyQueue.RemoveAt(0);
                    }
                }
                else
                {
                    if (cpuTime == cpuDone)
                    {
                        currentProcess = 0;
                        continue;
                    }
                }

                cpuTime++;
                runTime++;

                for (int i = 0; i < readyQueue.Count; i++)
                {
                    readyQueue.ElementAt(i).waitingTime++;
                }

            } while (jobList.Count != 0 || readyQueue.Count != 0 || currentProcess != 0);

            return resultList;
        }

    }
    class NonPreemptive_Priority
    {
        public static List<Result> Run(List<Process> jobList, List<Result> resultList)
        {
            int currentProcess = 0;
            int cpuTime = 0;
            int cpuDone = 0;
            int runTime = 0;

            int max = 0;            //우선순위가 가장 높은 프로세스ID

            List<ReadyQueueElement> readyQueue = new List<ReadyQueueElement>();
            do
            {
                while (jobList.Count != 0)
                {
                    Process frontJob = jobList.ElementAt(0);
                    if (frontJob.arriveTime == runTime)
                    {
                        readyQueue.Add(new ReadyQueueElement(frontJob.processID, frontJob.burstTime, 0, frontJob.priority, frontJob.arriveTime, false, frontJob.burstTime, 0, 0));
                        jobList.RemoveAt(0);
                    }
                    else break;
                }

                ReadyQueueElement tmp = null;                          //내림차순 정렬에 사용할 임시저장 변수

                for (int i = 0; i < readyQueue.Count - 1; i++)      //readyQueue에 있는 프로세스들을 priority를 기준으로 내림차순 정렬
                {
                    max = i;
                    for (int j = i + 1; j < readyQueue.Count; j++)
                    {
                        if (readyQueue[i].priority < readyQueue[j].priority)
                            max = j;
                    }
                    tmp = readyQueue[max];
                    readyQueue[max] = readyQueue[i];
                    readyQueue[i] = tmp;

                }
                if (currentProcess == 0)
                {
                    if (readyQueue.Count != 0)
                    {
                        ReadyQueueElement rq = readyQueue.ElementAt(0);
                        resultList.Add(new Result(rq.processID, runTime, rq.burstTime, rq.waitingTime, runTime - rq.arriveTIme, rq.burstTime + rq.waitingTime, rq.waitingTime));
                        cpuDone = rq.burstTime;
                        cpuTime = 0;
                        currentProcess = rq.processID;
                        readyQueue.RemoveAt(0);
                    }
                }
                else
                {
                    if (cpuTime == cpuDone)
                    {
                        currentProcess = 0;
                        continue;
                    }
                }

                cpuTime++;
                runTime++;

                for (int i = 0; i < readyQueue.Count; i++)
                {
                    readyQueue.ElementAt(i).waitingTime++;
                }

            } while (jobList.Count != 0 || readyQueue.Count != 0 || currentProcess != 0);
            return resultList;
        }
    }
    class Preemptive_Priority
    {
        public static List<Result> Run(List<Process> jobList, List<Result> resultList)
        {
            int currentProcess = 0;
            int cpuTime = 0;
            int cpuDone = 0;
            int runTime = 0;

            int max = 0;
            double prioritytmp = 0;               //현재 프로세스 우선순위 임시저장 변수

            ReadyQueueElement tmp2 = null;       //현재 프로세스정보 임시저장

            List<ReadyQueueElement> readyQueue = new List<ReadyQueueElement>();
            do
            {
                while (jobList.Count != 0)
                {
                    Process frontJob = jobList.ElementAt(0);
                    if (frontJob.arriveTime == runTime)
                    {
                        readyQueue.Add(new ReadyQueueElement(frontJob.processID, frontJob.burstTime, 0, frontJob.priority, frontJob.arriveTime, false, frontJob.burstTime, 0, 0));
                        jobList.RemoveAt(0);
                    }
                    else break;
                }

                ReadyQueueElement tmp = null;

                for (int i = 0; i < readyQueue.Count - 1; i++)
                {
                    max = i;
                    for (int j = i + 1; j < readyQueue.Count; j++)
                    {
                        if (readyQueue[i].priority < readyQueue[j].priority)
                            max = j;
                    }
                    tmp = readyQueue[max];
                    readyQueue[max] = readyQueue[i];
                    readyQueue[i] = tmp;

                }

                if (currentProcess == 0)
                {
                    if (readyQueue.Count != 0)
                    {
                        ReadyQueueElement rq = readyQueue.ElementAt(0);

                        if (!rq.check)                                  //프로세스가 처음시작 됬을 때 실행
                        {
                            rq.responseTime = runTime - rq.arriveTIme;  //응답시간 계산
                            resultList.Add(new Result(rq.processID, runTime, rq.burstTime, rq.waitingTime, rq.responseTime, rq.burstTime + rq.waitingTime, rq.waitingTime));
                        }
                        else                                            //프로세스가 실행된 적이 있는 경우
                        {
                            resultList.Add(new Result(rq.processID, runTime, rq.burstTime, rq.waitingTime, rq.responseTime, rq.firstburstTime + rq.waitingTime, rq.waitingTime));
                        }
                        cpuDone = rq.burstTime;
                        cpuTime = 0;
                        currentProcess = rq.processID;
                        prioritytmp = rq.priority;
                        tmp2 = rq;
                        readyQueue.RemoveAt(0);
                    }
                }
                else
                {
                    if (cpuTime == cpuDone)
                    {
                        currentProcess = 0;
                        continue;
                    }
                    else if (readyQueue.Count != 0)
                    {
                        if (prioritytmp < readyQueue.ElementAt(0).priority)         //readyQueue에 있는 프로세스의 우선순위가 현재 프로세스의 우선순위보다 높은 경우 실행
                        {
                            resultList.RemoveAt(resultList.Count - 1);              //resultList에 추가해던 현재 프로세스 정보를 삭제
                            resultList.Add(new Result(tmp2.processID, runTime - cpuTime, cpuTime, tmp2.waitingTime, tmp2.responseTime, tmp2.burstTime + tmp2.waitingTime, tmp2.waitingTime));    //resultList에 현재 프로세스정보(startp, burstTime)를 수정하여 다시 추가
                            readyQueue.Add(new ReadyQueueElement(tmp2.processID, tmp2.burstTime - cpuTime, tmp2.waitingTime, tmp2.priority, tmp2.arriveTIme, true, tmp2.firstburstTime, 0, tmp2.responseTime));  //현재 프로세스를 다시 readyQueue로 복귀시킴
                            currentProcess = 0;
                            continue;
                        }
                    }
                }

                cpuTime++;
                runTime++;

                for (int i = 0; i < readyQueue.Count; i++)
                {
                    readyQueue.ElementAt(i).waitingTime++;
                }

            } while (jobList.Count != 0 || readyQueue.Count != 0 || currentProcess != 0);
            return resultList;
        }
    }
    class RoundRobin
    {
        public static List<Result> Run(List<Process> jobList, List<Result> resultList, int TimeSlice)
        {
            int currentProcess = 0;
            int cpuTime = 0;
            int cpuDone = 0;
            int runTime = 0;
            int burstTimetmp = 0;    //기존 burstTime

            List<ReadyQueueElement> readyQueue = new List<ReadyQueueElement>();
            ReadyQueueElement tmp = null;

            do
            {
                while (jobList.Count != 0)
                {
                    Process frontJob = jobList.ElementAt(0);
                    if (frontJob.arriveTime == runTime)
                    {
                        readyQueue.Add(new ReadyQueueElement(frontJob.processID, frontJob.burstTime, 0, frontJob.priority, frontJob.arriveTime, false, frontJob.burstTime, 0, 0));
                        jobList.RemoveAt(0);
                    }
                    else
                        break;
                }

                if (currentProcess == 0)
                {
                    if (readyQueue.Count != 0)
                    {
                        ReadyQueueElement rq = readyQueue.ElementAt(0);
                        cpuTime = 0;
                        currentProcess = rq.processID;
                        burstTimetmp = rq.burstTime;

                        if (rq.check == false)
                        {
                            rq.responseTime = runTime - rq.arriveTIme;
                            if (TimeSlice < rq.burstTime)                   //TimeSlice가 현재 프로세스 실행시간보다 작을 경우 실행
                            {
                                resultList.Add(new Result(rq.processID, runTime, TimeSlice, rq.waitingTime, rq.responseTime, rq.burstTime + rq.totalwaitingTime, rq.totalwaitingTime));
                                cpuDone = TimeSlice;
                                rq.burstTime = rq.burstTime - TimeSlice;
                                tmp = rq;
                            }
                            else                                            //TimeSlice가 현재 프로세스 실행시간보다 클 경우 실행
                            {
                                resultList.Add(new Result(rq.processID, runTime, rq.burstTime, rq.waitingTime, rq.responseTime, rq.burstTime + rq.totalwaitingTime, rq.totalwaitingTime));
                                cpuDone = rq.burstTime;
                            }
                            rq.responseTime = runTime - rq.arriveTIme;
                        }
                        else
                        {
                            if (TimeSlice < rq.burstTime)
                            {
                                resultList.Add(new Result(rq.processID, runTime, TimeSlice, rq.waitingTime, rq.responseTime, rq.firstburstTime + rq.totalwaitingTime, rq.totalwaitingTime));
                                cpuDone = TimeSlice;
                                rq.burstTime = rq.burstTime - TimeSlice;
                                tmp = rq;
                            }
                            else
                            {
                                resultList.Add(new Result(rq.processID, runTime, rq.burstTime, rq.waitingTime, rq.responseTime, rq.firstburstTime + rq.totalwaitingTime, rq.totalwaitingTime));
                                cpuDone = rq.burstTime;
                            }
                        }
                        readyQueue.RemoveAt(0);
                    }
                }
                else
                {
                    if (cpuTime == cpuDone)
                    {
                        currentProcess = 0;
                        if (burstTimetmp > TimeSlice)          //TimeSlice가 현재 프로세스 실행시간보다 작을 경우 실행
                        {
                            readyQueue.Add(new ReadyQueueElement(tmp.processID, tmp.burstTime, 0, tmp.priority, tmp.arriveTIme, true, tmp.firstburstTime, tmp.totalwaitingTime, tmp.responseTime));      //현재 프로세스를 다시 readyQueue로 복귀시킴
                        }
                        continue;
                    }
                }

                cpuTime++;
                runTime++;

                for (int i = 0; i < readyQueue.Count; i++)
                {
                    readyQueue.ElementAt(i).waitingTime++;
                    readyQueue.ElementAt(i).totalwaitingTime++;
                }
            } while (jobList.Count != 0 || readyQueue.Count != 0 || currentProcess != 0);

            return resultList;
        }
    }
    class HRRN
    {
        public static List<Result> Run(List<Process> jobList, List<Result> resultList)
        {
            int currentProcess = 0;
            int cpuTime = 0;
            int cpuDone = 0;
            int runTime = 0;

            int max = 0;            //우선순위가 가장 높은 프로세스ID

            List<ReadyQueueElement> readyQueue = new List<ReadyQueueElement>();
            do
            {
                while (jobList.Count != 0)
                {
                    Process frontJob = jobList.ElementAt(0);
                    if (frontJob.arriveTime == runTime)
                    {
                        readyQueue.Add(new ReadyQueueElement(frontJob.processID, frontJob.burstTime, 0, frontJob.priority, frontJob.arriveTime, false, frontJob.burstTime, 0, 0));
                        jobList.RemoveAt(0);
                    }
                    else break;
                }

                ReadyQueueElement tmp = null;                          //내림차순 정렬에 사용할 임시저장 변수

                for (int i = 0; i < readyQueue.Count - 1; i++)      //readyQueue에 있는 프로세스들을 priority를 기준으로 내림차순 정렬
                {
                    max = i;
                    for (int j = i + 1; j < readyQueue.Count; j++)
                    {
                        if (readyQueue[i].priority < readyQueue[j].priority)
                            max = j;
                    }
                    tmp = readyQueue[max];
                    readyQueue[max] = readyQueue[i];
                    readyQueue[i] = tmp;

                }
                if (currentProcess == 0)
                {
                    if (readyQueue.Count != 0)
                    {
                        ReadyQueueElement rq = readyQueue.ElementAt(0);
                        resultList.Add(new Result(rq.processID, runTime, rq.burstTime, rq.waitingTime, runTime - rq.arriveTIme, rq.burstTime + rq.waitingTime, rq.waitingTime));
                        cpuDone = rq.burstTime;
                        cpuTime = 0;
                        currentProcess = rq.processID;
                        readyQueue.RemoveAt(0);
                    }
                }
                else
                {
                    if (cpuTime == cpuDone)
                    {
                        currentProcess = 0;
                        continue;
                    }
                }

                cpuTime++;
                runTime++;

                for (int i = 0; i < readyQueue.Count; i++)
                {
                    readyQueue.ElementAt(i).waitingTime++;
                    readyQueue.ElementAt(i).priority = (double)(((double)readyQueue.ElementAt(i).waitingTime + (double)readyQueue.ElementAt(i).burstTime) / (double)readyQueue.ElementAt(i).burstTime);    //readyQueue 프로세스들의 우선순위 수정
                }

            } while (jobList.Count != 0 || readyQueue.Count != 0 || currentProcess != 0);
            return resultList;
        }
    }

}

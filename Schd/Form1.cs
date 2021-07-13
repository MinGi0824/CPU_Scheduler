using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace Schd
{
    public partial class Scheduling : Form
    {
        string[] readText;
        private bool readFile = false;
        List<Process> pList, pView;
        List<Result> resultList;
        int Processnum = 0;             //선점형 스케쥴링에서 waiting time을 구하기 위한 변수

        public Scheduling()
        {
            InitializeComponent();
        }

        private void OpenFile_Click(object sender, EventArgs e)
        {
            pView.Clear();
            pList.Clear();

            //파일 오픈
            string path =  SelectFilePath();
            if (path == null) return;
    
            readText = File.ReadAllLines(path);
            
            //토큰 분리
            for (int i = 0; i < readText.Length; i++)
            {
                string[] token = readText[i].Split(' ');
                Process p = new Process(int.Parse(token[1]), int.Parse(token[2]), int.Parse(token[3]), int.Parse(token[4]));
                pList.Add(p);
            }

            //Grid에 process 출력
            dataGridView1.Rows.Clear();
            string[] row = { "", "", "", "" };
            foreach (Process p in pList)
            {
                row[0] = p.processID.ToString();
                row[1] = p.arriveTime.ToString();
                row[2] = p.burstTime.ToString();
                row[3] = p.priority.ToString();

                dataGridView1.Rows.Add(row);
                Processnum++;
            }

            //arriveTime으로 정렬
            pList.Sort(delegate(Process x, Process y)
            {
                if (x.arriveTime > y.arriveTime) return 1;
                else if (x.arriveTime < y.arriveTime) return -1;
                else
                {
                    return x.processID.CompareTo(y.processID);
                }
                //return x.arriveTime.CompareTo(y.arriveTime);
            });

            readFile = true;
        }

        private string SelectFilePath()
        {
            openFileDialog1.Filter = "텍스트파일|*.txt";
            return (openFileDialog1.ShowDialog() == DialogResult.OK) ? openFileDialog1.FileName : null;
        }

        private void Run_Click(object sender, EventArgs e)
        {
            if (!readFile) return;

            //스케쥴러 실행

            if (comboBox.Text == "FCFS")
                resultList = FCFS.Run(pList, resultList);
            else if (comboBox.Text == "SJF")
                resultList = SJF.Run(pList, resultList);
            else if (comboBox.Text == "NonPreemptive_Priority")
                resultList = NonPreemptive_Priority.Run(pList, resultList);
            else if (comboBox.Text == "Preemptive_Priority")
                resultList = Preemptive_Priority.Run(pList, resultList);
            else if (comboBox.Text == "HRRN")
                resultList = HRRN.Run(pList, resultList);
            else if (comboBox.Text == "Round Robin")
            {
                int i;
                i = int.Parse(textBox.Text);
                resultList = RoundRobin.Run(pList, resultList, i);
            }

            else
                MessageBox.Show("스케줄링 방식을 선택하세요.");


            //결과출력
            dataGridView2.Rows.Clear();
            string[] row = { "", "", ""};

            double watingTime = 0.0;
            foreach (Result r in resultList)
            {
                row[0] = r.processID.ToString();
                row[1] = r.burstTime.ToString();
                row[2] = r.waitingTime.ToString();

                //watingTime = r.totalwaitingTime;

                dataGridView2.Rows.Add(row);
            }


            //프로세스 별 출력
            dataGridView3.Rows.Clear();
            string[] row2 = { "", "", "",""};

            double watingTime2 = 0.0;

            for (int i = 1; i <= Processnum; i++)
            {
                foreach (Result r in resultList)
                {
                    if (r.processID == i)
                    {
                        row2[0] = r.processID.ToString();
                        row2[1] = r.waitingTime.ToString();
                        row2[2] = r.responsetime.ToString();
                        row2[3] = r.turnaroundtime.ToString();
                        watingTime2 = r.totalwaitingTime;
                    }

                }
                watingTime += watingTime2;
                dataGridView3.Rows.Add(row2);
            }
            TRTime.Text = "Total Execution Time : " + (resultList[resultList.Count - 1].startP + resultList[resultList.Count - 1].burstTime).ToString();
            avgRT.Text = "Averge Waiting TIme : " + (watingTime / Processnum).ToString();
            Throughput.Text = "Throughput : "+(((double)resultList[resultList.Count - 1].startP + (double)resultList[resultList.Count - 1].burstTime)/ (double)Processnum).ToString();
            panel1.Invalidate();
        }


        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            int startPosition = 10;
            double waitingTime = 0.0;

            int resultListPosition = 0;
            foreach (Result r in resultList)
            {
                e.Graphics.DrawString("p" + r.processID.ToString(), Font, Brushes.Black, startPosition + (r.startP * 10), resultListPosition);
                e.Graphics.DrawRectangle(Pens.Red, startPosition + (r.startP * 10), resultListPosition + 20, r.burstTime * 10, 30);
                e.Graphics.DrawString(r.burstTime.ToString(), Font, Brushes.Black, startPosition + (r.startP * 10), resultListPosition + 60);
                e.Graphics.DrawString(r.waitingTime.ToString(), Font, Brushes.Black, startPosition + (r.startP * 10), resultListPosition + 80);
                waitingTime += (double)r.waitingTime;
            }
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

        }
        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void textBox_TextChanged(object sender, EventArgs e)
        {

        }
        private void comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            pList = new List<Process>();
            pView = new List<Process>();
            resultList = new List<Result>();

            //입력창
            DataGridViewTextBoxColumn processColumn = new DataGridViewTextBoxColumn();
            DataGridViewTextBoxColumn arriveTimeColumn = new DataGridViewTextBoxColumn();
            DataGridViewTextBoxColumn burstTimeColumn = new DataGridViewTextBoxColumn();
            DataGridViewTextBoxColumn priorityColumn = new DataGridViewTextBoxColumn();

            processColumn.HeaderText = "#Process";
            processColumn.Name = "#Process";
            arriveTimeColumn.HeaderText = "Arrive TIme";
            arriveTimeColumn.Name = "Arrive TIm";
            burstTimeColumn.HeaderText = "Burst Time";
            burstTimeColumn.Name = "Burst Time";
            priorityColumn.HeaderText = "Priority";
            priorityColumn.Name = "Priority";

            dataGridView1.Columns.Add(processColumn);
            dataGridView1.Columns.Add(arriveTimeColumn);
            dataGridView1.Columns.Add(burstTimeColumn);
            dataGridView1.Columns.Add(priorityColumn);



            //결과창
            DataGridViewTextBoxColumn resultProcessColumn = new DataGridViewTextBoxColumn();
            DataGridViewTextBoxColumn resultBurstTimeColumn = new DataGridViewTextBoxColumn();
            DataGridViewTextBoxColumn resultWaitingTimeColumn = new DataGridViewTextBoxColumn();


            resultProcessColumn.HeaderText = "#Process";
            resultProcessColumn.Name = "#Process";
            resultBurstTimeColumn.HeaderText = "Execution Time";
            resultBurstTimeColumn.Name = "Execution Time";
            resultWaitingTimeColumn.HeaderText = "Waiting Time";
            resultWaitingTimeColumn.Name = "Waiting Time";

            dataGridView2.Columns.Add(resultProcessColumn);
            dataGridView2.Columns.Add(resultBurstTimeColumn);
            dataGridView2.Columns.Add(resultWaitingTimeColumn);

            //프로세스 별 출력 창
            DataGridViewTextBoxColumn Process= new DataGridViewTextBoxColumn();
            DataGridViewTextBoxColumn waitingTime = new DataGridViewTextBoxColumn();
            DataGridViewTextBoxColumn responseTime = new DataGridViewTextBoxColumn();
            DataGridViewTextBoxColumn turnaroundTime = new DataGridViewTextBoxColumn();


            Process.HeaderText = "#Process";
            Process.Name = "#Process";
            waitingTime.HeaderText = "Waiting Time";
            waitingTime.Name = "Waiting Time";
            responseTime.HeaderText = "Response Time";
            responseTime.Name = "Response Time";
            turnaroundTime.HeaderText = "Turnaround Time";
            turnaroundTime.Name = "Turnaround Time";

            dataGridView3.Columns.Add(Process);
            dataGridView3.Columns.Add(waitingTime);
            dataGridView3.Columns.Add(responseTime);
            dataGridView3.Columns.Add(turnaroundTime);

        }
    }
}

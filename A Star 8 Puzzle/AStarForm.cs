
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;

namespace A_Star_8_Puzzle
{
	public partial class MainForm : Form
    {
        #region Properties

        private const string FINISH = "123804765"; // end status
        private const int SPEED = 800; // movement speed of cell number
        private List<Status_Node> OpenList; // list containing generated states
        private List<Status_Node> CloseList; // list containing found states
        private List<Status_Node> TraceList; // trace list
        private int StepCount; // number of steps
        private Status_Node CurNode; // current node
        private Status_Node Game; // topic
        private bool first = true; // flag marks node as first position
        private bool reset = false; // reset the Puzzle matrix

        #endregion

        #region Methods

        /// <summary>
        /// tính toán xem trạng thái có thể trở về trạng thái đích được không
        /// </summary>
        private bool IsCanSolve(Status_Node n)
        {
            int length = n.Code.Length;
            int value = 0;
            int[] k = new int[length];

            for (int i = 0; i < length; i++)
                int.TryParse(n.Code[i].ToString(), out k[i]);

            for (int i = 0; i < length; i++)
            {
                int t = k[i];
                if (t > 0)
                {
                    for (int j = i + 1; j < length; j++)
                    {
                        if (k[j] < t && k[j] > 0)
                        {
                            value++;
                        }
                    }
                }
            }

            if (value % 2 != 0)
                return true;
            else
                return false;
        }


        /// <summary>
        /// add a new state to the open list (containing the generated child states)
        /// so that the list is sorted in ascending order by F value
        /// - input variable is node n
        /// </summary>
        private void AddNodeToOpenList(Status_Node n)
        {
            int i = 1;

            if (OpenList.Count == 0) //if list is empty
            {
                OpenList.Add(n);
                return;
            }

            // If the list is not empty, search to see if it is already there
            bool found = false;
            bool canadd = false;

            for (i = 0; i < OpenList.Count; i++)
            {
                // find
                if (n.Code.Equals(OpenList[i].Code))
                {
                    found = true;
                    if (n.G < OpenList[i].G) // compare G values, get the smaller Node
                    {
                        canadd = true;
                        OpenList.RemoveAt(i); // delete element try i
                    }
                    return;
                }
            }
            // not found or addable
            if (!found || canadd)
            {
                // browse the list and insert in ascending order of F
                for (i = 0; i < OpenList.Count; i++)
                {
                    if (n.F < OpenList[i].F)
                    {
                        break;
                    }
                }

                if (i == OpenList.Count)
                    OpenList.Add(n);
                else
                    OpenList.Insert(i, n);
            }
        }

        /// <summary>
        /// Get a status from the OPen List and move it to the Close list
        /// because the list is sorted in ascending order of F, take the first element (OpenList[0])
        /// - returns the retrieved node
        /// </summary>
        private Status_Node GetNodeFromOpenList()
        {
            if (OpenList.Count > 0)
            {
                Status_Node n = OpenList[0];

                OpenList.RemoveAt(0); // delete the first node
                CloseList.Add(n); // switch to CloseList
                return n;
            }
            return null;
        }
        // <summary>
        /// Check the existence of a node in the Close list
        /// if yes -> return true, otherwise false
        /// </summary>
        private bool IsInCloseList(Status_Node n)
        {
            for (int i = 0; i < CloseList.Count; i++)
            {
                if (CloseList[i].Code.Equals(n.Code))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Evaluation function
        /// Returns the value H which is the number of cells in the wrong position of the current state compared to the final result
        /// </summary>
        private int CalculateH(string current)
        {
            int count = 0;
            for (int i = 0; i < current.Length; i++)
            {
                if (!current[i].Equals(FINISH[i]))
                {
                    count++;
                }
            }
            return count;
        }

        /// <summary>
        /// Calculate the child states of node n
        /// </summary>
        private void ExpandNode(Status_Node n)
        {
            int index = n.Code.IndexOf('0'); // find position '0' in string

            switch (index)
            {
                case 0: // If cell 0 is in the position of cell 1, it can go right or down
                    {
                        // child state 1 - to the right
                        char[] child1 = n.Code.ToCharArray();
                        // turn right. For example: 083124765 becomes 803124765
                        child1[0] = child1[1];
                        child1[1] = '0';
                        string map = new string(child1);

                        // if node n has no parent or the child node generated is different from n
                        if (n.Parent == null || !n.Code.Equals(map))
                        {
                            // generate child node
                            Status_Node child = new Status_Node(map, n, n.G + 1, CalculateH(map));
                            // if the child node is not in the list close
                            if (!IsInCloseList(child))
                            {
                                AddNodeToOpenList(child); // insert into list open
                            }
                        }

                        char[] child2 = n.Code.ToCharArray();
                        child2[0] = child2[3];
                        child2[3] = '0';
                        map = new string(child2);
                        if (n.Parent == null || !n.Code.Equals(map))
                        {
                            Status_Node child = new Status_Node(map, n, n.G + 1, CalculateH(map));
                            if (!IsInCloseList(child))
                            {
                                AddNodeToOpenList(child);
                            }
                        }
                        break;
                    }
                case 1:
                    {
                        char[] child1 = n.Code.ToCharArray();
                        child1[1] = child1[0];
                        child1[0] = '0';
                        string map = new string(child1);
                        if (n.Parent == null || !n.Code.Equals(map))
                        {
                            Status_Node child = new Status_Node(map, n, n.G + 1, CalculateH(map));
                            if (!IsInCloseList(child))
                                AddNodeToOpenList(child);
                        }
                        // turn right
                        char[] child2 = n.Code.ToCharArray();
                        child2[1] = child2[2];
                        child2[2] = '0';
                        map = new string(child2);
                        if (n.Parent == null || !n.Code.Equals(map))
                        {
                            Status_Node child = new Status_Node(map, n, n.G + 1, CalculateH(map));
                            if (!IsInCloseList(child))
                                AddNodeToOpenList(child);
                        }

                        char[] child3 = n.Code.ToCharArray();
                        child3[1] = child3[4];
                        child3[4] = '0';
                        map = new string(child3);
                        if (n.Parent == null || !n.Code.Equals(map))
                        {
                            Status_Node child = new Status_Node(map, n, n.G + 1, CalculateH(map));
                            if (!IsInCloseList(child))
                                AddNodeToOpenList(child);
                        }
                        break;
                    }
                case 2:
                    {
                        char[] child1 = n.Code.ToCharArray();
                        child1[2] = child1[1];
                        child1[1] = '0';
                        string map = new string(child1);
                        if (n.Parent == null || !n.Code.Equals(map))
                        {
                            Status_Node child = new Status_Node(map, n, n.G + 1, CalculateH(map));
                            if (!IsInCloseList(child))
                                AddNodeToOpenList(child);
                        }

                        char[] child2 = n.Code.ToCharArray();
                        child2[2] = child2[5];
                        child2[5] = '0';
                        map = new string(child2);
                        if (n.Parent == null || !n.Code.Equals(map))
                        {
                            Status_Node child = new Status_Node(map, n, n.G + 1, CalculateH(map));
                            if (!IsInCloseList(child))
                                AddNodeToOpenList(child);
                        }
                        break;
                    }
                case 3:
                    {
                        char[] child1 = n.Code.ToCharArray();
                        child1[3] = child1[0];
                        child1[0] = '0';
                        string map = new string(child1);
                        if (n.Parent == null || !n.Code.Equals(map))
                        {
                            Status_Node child = new Status_Node(map, n, n.G + 1, CalculateH(map));
                            if (!IsInCloseList(child))
                                AddNodeToOpenList(child);
                        }

                        char[] child2 = n.Code.ToCharArray();
                        child2[3] = child2[4];
                        child2[4] = '0';
                        map = new string(child2);
                        if (n.Parent == null || !n.Code.Equals(map))
                        {
                            Status_Node child = new Status_Node(map, n, n.G + 1, CalculateH(map));
                            if (!IsInCloseList(child))
                                AddNodeToOpenList(child);
                        }

                        char[] child3 = n.Code.ToCharArray();
                        child3[3] = child3[6];
                        child3[6] = '0';
                        map = new string(child3);
                        if (n.Parent == null || !n.Code.Equals(map))
                        {
                            Status_Node child = new Status_Node(map, n, n.G + 1, CalculateH(map));
                            if (!IsInCloseList(child))
                                AddNodeToOpenList(child);
                        }
                        break;
                    }
                case 4:
                    {
                        char[] child1 = n.Code.ToCharArray();
                        child1[4] = child1[1];
                        child1[1] = '0';
                        string map = new string(child1);
                        if (n.Parent == null || !n.Code.Equals(map))
                        {
                            Status_Node child = new Status_Node(map, n, n.G + 1, CalculateH(map));
                            if (!IsInCloseList(child))
                                AddNodeToOpenList(child);
                        }

                        char[] child2 = n.Code.ToCharArray();
                        child2[4] = child2[3];
                        child2[3] = '0';
                        map = new string(child2);
                        if (n.Parent == null || !n.Code.Equals(map))
                        {
                            Status_Node child = new Status_Node(map, n, n.G + 1, CalculateH(map));
                            if (!IsInCloseList(child))
                                AddNodeToOpenList(child);
                        }

                        char[] child3 = n.Code.ToCharArray();
                        child3[4] = child3[5];
                        child3[5] = '0';
                        map = new string(child3);
                        if (n.Parent == null || !n.Code.Equals(map))
                        {
                            Status_Node child = new Status_Node(map, n, n.G + 1, CalculateH(map));
                            if (!IsInCloseList(child))
                                AddNodeToOpenList(child);
                        }

                        char[] child4 = n.Code.ToCharArray();
                        child4[4] = child4[7];
                        child4[7] = '0';
                        map = new string(child4);
                        if (n.Parent == null || !n.Code.Equals(map))
                        {
                            Status_Node child = new Status_Node(map, n, n.G + 1, CalculateH(map));
                            if (!IsInCloseList(child))
                                AddNodeToOpenList(child);
                        }
                        break;
                    }
                case 5:
                    {
                        char[] child1 = n.Code.ToCharArray();
                        child1[5] = child1[2];
                        child1[2] = '0';
                        string map = new string(child1);
                        if (n.Parent == null || !n.Code.Equals(map))
                        {
                            Status_Node child = new Status_Node(map, n, n.G + 1, CalculateH(map));
                            if (!IsInCloseList(child))
                                AddNodeToOpenList(child);
                        }
                        // to the left
                        char[] child2 = n.Code.ToCharArray();
                        child2[5] = child2[4];
                        child2[4] = '0';
                        map = new string(child2);
                        if (n.Parent == null || !n.Code.Equals(map))
                        {
                            Status_Node child = new Status_Node(map, n, n.G + 1, CalculateH(map));
                            if (!IsInCloseList(child))
                                AddNodeToOpenList(child);
                        }
                        // down
                        char[] child3 = n.Code.ToCharArray();
                        child3[5] = child3[8];
                        child3[8] = '0';
                        map = new string(child3);
                        if (n.Parent == null || !n.Code.Equals(map))
                        {
                            Status_Node child = new Status_Node(map, n, n.G + 1, CalculateH(map));
                            if (!IsInCloseList(child))
                                AddNodeToOpenList(child);
                        }
                        break;
                    }
                case 6:
                    {
                        // go up
                        char[] child1 = n.Code.ToCharArray();
                        child1[6] = child1[3];
                        child1[3] = '0';
                        string map = new string(child1);
                        if (n.Parent == null || !n.Code.Equals(map))
                        {
                            Status_Node child = new Status_Node(map, n, n.G + 1, CalculateH(map));
                            if (!IsInCloseList(child))
                                AddNodeToOpenList(child);
                        }
                        // turn right
                        char[] child2 = n.Code.ToCharArray();
                        child2[6] = child2[7];
                        child2[7] = '0';
                        map = new string(child2);
                        if (n.Parent == null || !n.Code.Equals(map))
                        {
                            Status_Node child = new Status_Node(map, n, n.G + 1, CalculateH(map));
                            if (!IsInCloseList(child))
                                AddNodeToOpenList(child);
                        }
                        break;
                    }
                case 7:
                    {
                        // go up
                        char[] child1 = n.Code.ToCharArray();
                        child1[7] = child1[4];
                        child1[4] = '0';
                        string map = new string(child1);
                        if (n.Parent == null || !n.Code.Equals(map))
                        {
                            Status_Node child = new Status_Node(map, n, n.G + 1, CalculateH(map));
                            if (!IsInCloseList(child))
                                AddNodeToOpenList(child);
                        }

                        char[] child2 = n.Code.ToCharArray();
                        child2[7] = child2[6];
                        child2[6] = '0';
                        map = new string(child2);
                        if (n.Parent == null || !n.Code.Equals(map))
                        {
                            Status_Node child = new Status_Node(map, n, n.G + 1, CalculateH(map));
                            if (!IsInCloseList(child))
                                AddNodeToOpenList(child);
                        }

                        char[] child3 = n.Code.ToCharArray();
                        child3[7] = child3[8];
                        child3[8] = '0';
                        map = new string(child3);
                        if (n.Parent == null || !n.Code.Equals(map))
                        {
                            Status_Node child = new Status_Node(map, n, n.G + 1, CalculateH(map));
                            if (!IsInCloseList(child))
                                AddNodeToOpenList(child);
                        }
                        break;
                    }
                case 8:
                    {
                        char[] child1 = n.Code.ToCharArray();
                        child1[8] = child1[5];
                        child1[5] = '0';
                        string map = new string(child1);
                        if (n.Parent == null || !n.Code.Equals(map))
                        {
                            Status_Node child = new Status_Node(map, n, n.G + 1, CalculateH(map));
                            if (!IsInCloseList(child))
                                AddNodeToOpenList(child);
                        }

                        char[] child2 = n.Code.ToCharArray();
                        child2[8] = child2[7];
                        child2[7] = '0';
                        map = new string(child2);
                        if (n.Parent == null || !n.Code.Equals(map))
                        {
                            Status_Node child = new Status_Node(map, n, n.G + 1, CalculateH(map));
                            if (!IsInCloseList(child))
                                AddNodeToOpenList(child);
                        }
                        break;
                    }
            }
        }

        #region A* Algorithm
        // A* Algorithm
        private Status_Node Start(string start, string end)
        {
            Stopwatch sw = new Stopwatch();

            OpenList.Clear();
            CloseList.Clear();
            StepCount = 0;                        

            sw.Start();
            // The first node is the problem matrix
            Status_Node firstNode = new Status_Node(start, null, 0, CalculateH(start));

            AddNodeToOpenList(firstNode);

            Status_Node currentNode = null;
            // calculate steps, for list with ptu number greater than 50000 -> not found
            while (OpenList.Count > 0 && OpenList.Count < 50000)
            {
                currentNode = GetNodeFromOpenList();
                // get the first node in the list, move it to close list

                // check if target state is there?
                if (currentNode.Code.Equals(FINISH)) // nếu đúng thì trả về trạng thái này
                {                                       
                    sw.Stop();
                    lblFinish.Text = "Time: " + sw.ElapsedMilliseconds.ToString() + " milliseconds";
                    lblCountStatistic.Text = "Number of states found: " + OpenList.Count.ToString();
                    return currentNode;
                }

                ExpandNode(currentNode);// find child states if not found
            }
            return null;
        }
        #endregion

        private void Stop()
        {
            if (this.CurNode.Code.Equals(FINISH))
            {
                MessageBox.Show("Solved", "Completed", MessageBoxButtons.OK, MessageBoxIcon.Information);
                if (MessageBox.Show("Do you want to continue?", "?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    lblFinish.Text = "";
                    lblCountStatistic.Text = "";
                }
                else
                {
                    Application.Exit();
                }
            }
        }

        // move number cells
        private bool MoveStep(Direction dir)
        {
            int index = -1;
            if (this.CurNode != null)
                index = this.CurNode.Code.IndexOf("0"); // tìm vị trí ô trống

            switch (dir)
            {
                case Direction.LEFT:
                    {
                        if (index % 3 == 2) return false;

                        char[,] data = this.CurNode.Map;
                        char tmp = this.CurNode.Map[index / 3, index % 3 + 1];
                        data[index / 3, index % 3] = tmp;
                        data[index / 3, index % 3 + 1] = '0';
                        this.CurNode.Map = data;
                        break;
                    }
                case Direction.RIGHT:
                    {
                        if (index % 3 == 0) return false;

                        char[,] data = this.CurNode.Map;
                        char tmp = this.CurNode.Map[index / 3, index % 3 - 1];
                        data[index / 3, index % 3] = tmp;
                        data[index / 3, index % 3 - 1] = '0';
                        this.CurNode.Map = data;
                        break;
                    }
                case Direction.UP:
                    {
                        if (index / 3 == 2) return false;

                        char[,] data = this.CurNode.Map;
                        char tmp = this.CurNode.Map[index / 3 + 1, index % 3];
                        data[index / 3, index % 3] = tmp;
                        data[index / 3 + 1, index % 3] = '0';
                        this.CurNode.Map = data;
                        break;
                    }
                case Direction.DOWN:
                    {
                        if (index / 3 == 0) return false;

                        char[,] data = this.CurNode.Map;
                        char tmp = this.CurNode.Map[index / 3 - 1, index % 3];
                        data[index / 3, index % 3] = tmp;
                        data[index / 3 - 1, index % 3] = '0';
                        this.CurNode.Map = data;
                        break;
                    }
                default:
                    break;
            }
            return true;
        }

        #endregion

        public MainForm()
		{
			InitializeComponent();
            OpenList = new List<Status_Node>();
			CloseList = new List<Status_Node>();
            TraceList = new List<Status_Node>();
            this.timerPlay.Interval = SPEED;
		}

        private void PaintBackGround(Graphics g)
        {
            g.Clear(Color.LightCyan);
            Pen p = new Pen(Color.Cyan, 1.0f);

            // draw the mesh
            g.DrawLine(p, 0f, 96f, 288f, 96f);
            g.DrawLine(p, 0f, 192f, 288f, 192f);
            g.DrawLine(p, 96f, 0f, 96f, 288f);
            g.DrawLine(p, 192f, 0f, 192f, 288f);
        }

        // vẽ 8 puzzle
        private void pbGame_Paint(object sender, PaintEventArgs e)
        {
            PaintBackGround(e.Graphics);
            if (this.CurNode != null)
                this.CurNode.Paint(e.Graphics);
        }

        // Move cells on mouse click
        private void pbGame_MouseClick(object sender, MouseEventArgs e)
        {
            if(this.timerPlay.Enabled)
                return;
            Point p = new Point(e.Y / 96, e.X / 96); 
            bool isok = false;

            if (!isok)
            {
                Point p1 = p;
                p1.Offset(-1, 0);
                if (p1.X >= 0 && p1.Y >= 0 && p1.X < 3 && p1.Y < 3)
                {
                    if (CurNode.Map[p1.X, p1.Y] == '0')
                    {
                        isok = MoveStep(Direction.UP);
                    }
                }
            }
            if (!isok)
            {
                Point p2 = p;
                p2.Offset(1, 0);
                if (p2.X >= 0 && p2.Y >= 0 && p2.X < 3 && p2.Y < 3)
                {
                    if (CurNode.Map[p2.X, p2.Y] == '0')
                    {
                        isok = MoveStep(Direction.DOWN);// downward
                    }
                }
            }
            if (!isok)
            {
                Point p3 = p;
                p3.Offset(0, -1);
                if (p3.X >= 0 && p3.Y >= 0 && p3.X < 3 && p3.Y < 3)
                {
                    if (CurNode.Map[p3.X, p3.Y] == '0')
                    {
                        isok = MoveStep(Direction.LEFT);// sang trái
                    }
                }
            }
            if (!isok)
            {
                Point p4 = p;
                p4.Offset(0, 1);
                if (p4.X >= 0 && p4.Y >= 0 && p4.X < 3 && p4.Y < 3)
                {
                    if (CurNode.Map[p4.X, p4.Y] == '0')
                    {
                        isok = MoveStep(Direction.RIGHT);
                    }
                }
            }

            if (isok)
            {
                this.btnResolve.Text = "Prize";
                this.pbGame.Refresh();
                this.StepCount++;
                this.lblCount.Text = this.StepCount.ToString();
                Stop();
            }
        }

        private void bntNew_Click(object sender, EventArgs e)
        {
            this.timerPlay.Enabled = false;
            this.btnResolve.Enabled = true;
            // khởi tạo mặc định
            if (this.first)
            {
                this.CurNode = new Status_Node("876042531", null, 0, CalculateH("876042531"));
                //this.CurNode = new Status_Node("684031275", null, 0, CalculateH("684031275"));
                this.first = false;
            }
            else
            {
                if (this.reset)
                {
                    this.CurNode = this.Game;
                    this.reset = false;
                }
                else
                {
                    // random initialization
                    this.CurNode = new Status_Node(FINISH, null, 0, 0); // CurNode is the target state
                    Random rd = new Random();

                    for (int i = 0; i < 100; i++)
                    {
                        int j = rd.Next(1000) % 4;
                        MoveStep((Direction)j);// shuffle 8 puzzles in 4 directions
                    }
                }
            }

            this.Game = new Status_Node(this.CurNode.Code, null, 0, this.CurNode.H);// save the current state for the reset button
            this.btnResolve.Text = "Prize";
            this.StepCount = 0;
            this.lblCount.Text = this.StepCount.ToString();
            this.pbGame.Refresh();
        }

        private void btnResolve_Click(object sender, EventArgs e)
        {
            if (this.btnResolve.Text == "Prize" || this.btnResolve.Text == "Continue")
            {
                if (this.btnResolve.Text == "Prize")
                {
                    if (IsCanSolve(this.CurNode))
                    {
                        progressBar1.Style = ProgressBarStyle.Marquee;
                        progressBar1.MarqueeAnimationSpeed = 20;                       
                        Status_Node n = Start(this.CurNode.Code, FINISH);                        

                        if (n != null)// If solved, insert into trace array
                        {
                            TraceList.Clear();
                            while (n != null)
                            {
                                TraceList.Insert(0, n); // insert into TraceList
                                n = n.Parent;
                            }

                            this.StepCount = 0;
                            MessageBox.Show("Number of steps: " + (TraceList.Count - 1), "Number of steps", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            
                            this.btnResolve.Enabled = true;
                            this.btnResolve.Text = "Stop";
                            this.timerPlay.Enabled = true;
                        }
                        else
                        {
                            this.btnResolve.Enabled = false;
                            MessageBox.Show("Can't find a solution, the number of states is too large");
                        }
                    }
                    else
                    {
                        this.btnResolve.Enabled = false;
                        MessageBox.Show("Couldn't find the solution");
                        return;
                    }
                }
                else
                {
                    this.timerPlay.Enabled = true;
                    this.btnResolve.Text = "Continue";
                }
            }
            else
            {
                this.btnResolve.Text = "Continue";
                this.timerPlay.Enabled = false;
            }
        }

        // reset
        private void btnReset_Click(object sender, EventArgs e)
        {
            this.reset = true;
            bntNew_Click(null, null);
        }

        // trace, show steps
        private void timerPlay_Tick(object sender, EventArgs e)
        {
            progressBar1.Style = ProgressBarStyle.Continuous;
            progressBar1.Maximum = this.TraceList.Count;
            progressBar1.Minimum = 0;
            progressBar1.Step = 1;

            if (this.StepCount < this.TraceList.Count)
            {
                this.CurNode = this.TraceList[this.StepCount];
                pbGame.Refresh();

                this.lblCount.Text = "" + this.StepCount++;
                progressBar1.PerformStep();

                if (this.StepCount == this.TraceList.Count)
                {
                    this.timerPlay.Enabled = false;
                    lblFinish.Text = "SOLVED";
                    MessageBox.Show("Solved", "Completed", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    if (MessageBox.Show("Do you want to continue?", "?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        lblFinish.Text = "";
                        lblCountStatistic.Text = "";
                        progressBar1.Value = 0;
                        bntNew_Click(null, null);
                    }
                    else
                    {
                        Application.Exit();
                    }
                    GC.Collect();
                    this.btnResolve.Text = "Prize";
                }
            }
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            bntNew_Click(null, null);
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}
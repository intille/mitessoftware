using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using System.Collections; //ArrayList
using System.IO;

using ZedGraph;
using MobiRnD_RDT.Utilities;//FileReadWrite
using MobiRnD_RDT.Logging; //Logger


namespace NESPDataViewer
{
    public partial class Form1 : Form
    {
        #region FIELDS
        string _pathDataset = "";
        DateTime _firstDate = DateTime.MinValue;
        DateTime _lastDate = DateTime.MinValue;

        Hashtable _htPanes = new Hashtable();
        ArrayList _alCheckBoxes = new ArrayList();
        ArrayList _alLinesWithSymbols = new ArrayList();
        Hashtable _htBoxes = new Hashtable();

        bool _isFirstTime = true; //used to determine whether graphs need to be cleared on Reset
        bool _doesShowHover = true; 

        int _minutesPage = 60;

        //try setting these to false if graphing too slowly
        bool _isUsingLabels = true; //on mouse over, shows label for data point
        bool _isAdaptingPointSize = true; //as graph gets larger/smaller, changes point size to match

        #endregion

        #region INITIALIZE
        public Form1()
        {
            InitializeComponent();
            
        }
        public Form1(string pathStartDS)
        {
            Logger.LogDebug("Form1", "arguments " + pathStartDS);
            _pathDataset = pathStartDS;
            InitializeComponent();
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
            SetGraphPanels();

            if (_isUsingLabels)
                zedGraphControl1.PointValueEvent += new ZedGraphControl.PointValueHandler(zedGraphControl1_PointValueEvent);

            if (_pathDataset.Length > 0) OpenDataset(_pathDataset);
        }
        #endregion

        #region LAYOUT and FORMATTING
        private void Form1_Resize(object sender, EventArgs e)
        {
            SetLayout();
        }

        private void SetLayout()
        {   
            int graphwidth = ClientRectangle.Width-groupBox1.Width;
            int graphheight = ClientRectangle.Height - 100;

            groupBox1.Location = new Point(graphwidth, MainMenuStrip.Bottom + 5);
            groupBox1.Size = new Size(groupBox1.Width, graphheight);           
                       
            zedGraphControl1.Location = new Point(0, MainMenuStrip.Bottom);
            zedGraphControl1.Size = new Size(graphwidth,graphheight);

            hScrollBar1.Width = graphwidth-10;
            hScrollBar1.Location = new Point(5, zedGraphControl1.Bottom + 20);
            lbFirstDate.Location = new Point(5, hScrollBar1.Bottom);
            lbSecondDate.Location = new Point(hScrollBar1.Right - lbSecondDate.Width, hScrollBar1.Bottom);
            lbScrollTime.Location = new Point(hScrollBar1.Left, hScrollBar1.Top - lbScrollTime.Height);
            buttonZoomOut.Location = new Point(hScrollBar1.Right + 5, hScrollBar1.Top);
        }

        private void SetGraphPanels()
        {
            MasterPane myMaster = zedGraphControl1.MasterPane;

            _firstDate = DateTime.Now;
            _lastDate = DateTime.Now.AddYears(-3);

            myMaster.PaneList.Clear();

            // Set the master pane title
            myMaster.Title.IsVisible = false;

            // Fill the pane background with a color gradient
            myMaster.Fill = new Fill(Color.White, Color.MediumSlateBlue, 45.0F);

            // Set the margins and the space between panes to 10 points
            myMaster.Margin.All = 0;
            myMaster.InnerPaneGap = 0;

            // Enable the master pane legend
            myMaster.Legend.IsVisible = false;
            //myMaster.Legend.Position = LegendPos.TopCenter;

            // Vertical pan and zoom not allowed
            zedGraphControl1.IsEnableVPan = false;
            zedGraphControl1.IsEnableVZoom = false;

            zedGraphControl1.IsShowPointValues = _isUsingLabels;
            zedGraphControl1.IsSynchronizeXAxes = true;        


        }
        private GraphPane AddPane(string name, string ytitle)
        {
            GraphPane pane = new GraphPane();
            pane.Margin.All = 5;
            pane.Legend.IsVisible = false;
            pane.Title.Text = name;
            pane.Title.IsVisible = false;
            pane.YAxis.Title.Text = ytitle;
            pane.XAxis.Title.IsVisible = false;
            pane.XAxis.Type = AxisType.Date;
            pane.XAxis.Scale.MajorUnit = DateUnit.Second;
            pane.XAxis.MajorGrid.IsVisible = true;
            //pane.YAxis.Scale.Min = 0;
            pane.Fill.Color = Color.Empty;
            zedGraphControl1.MasterPane.Add(pane);

            _htBoxes.Add(name, new BoxObj());

            #region ADD CHECKBOX
            _htPanes.Add(name, pane);
            CheckBox cBox = new CheckBox();
            cBox.Parent = groupBox1;
            if (_alCheckBoxes.Count > 0)
            {
                int y = ((CheckBox)_alCheckBoxes[_alCheckBoxes.Count - 1]).Bottom + 20;
                cBox.Location = new Point(5, y);
            }
            else cBox.Location = new Point(5, 15);
            cBox.Text = name;
            cBox.Checked = true;
            cBox.CheckedChanged += new EventHandler(checkBox_CheckedChanged);
            _alCheckBoxes.Add(cBox);
            #endregion

            //RefreshMasterPaneLayout();

            return pane;

        }
        private void RefreshMasterPaneLayout()
        {
            // Tell ZedGraph to auto layout all the panes
            using (Graphics g = CreateGraphics())
            {
                zedGraphControl1.MasterPane.SetLayout(g, PaneLayout.SingleColumn);                
            }
            if (_isAdaptingPointSize) SetPointSize();
            zedGraphControl1.AxisChange();
            zedGraphControl1.Refresh();

            #region ARRANGE CHECKBOXES
            if (_alCheckBoxes.Count > 1)
            {
                int lastcheckbox = 10;
                bool isFirstBox = true;
                for (int i = 0; i < _alCheckBoxes.Count; i++)
                {
                    string name = ((CheckBox)_alCheckBoxes[i]).Text;
                    int y = 0;
                    //Is Pane Showing? 
                    if (zedGraphControl1.MasterPane.PaneList.Contains((GraphPane)_htPanes[name]))
                    {
                        y = Convert.ToInt32(zedGraphControl1.MasterPane.PaneList[name].Rect.Y);
                        if ((y == 0) && (isFirstBox)) y = lastcheckbox;
                        else if (y < lastcheckbox) y = lastcheckbox + 2 + ((CheckBox)_alCheckBoxes[i]).Height;
                    }
                    else if (isFirstBox) y = lastcheckbox;
                    else y = lastcheckbox + 2 + ((CheckBox)_alCheckBoxes[i]).Height;
                    ((CheckBox)_alCheckBoxes[i]).Location = new Point(5, y);
                    isFirstBox = false;
                    lastcheckbox = y;
                }
                groupBox1.Visible = true;
            }
            else groupBox1.Visible = false;
            #endregion

        }

        private void SetPointSize()
        {
            if (zedGraphControl1.MasterPane.PaneList.Count > 0)
            {
                double minutes = ((TimeSpan)(_lastDate - _firstDate)).TotalMinutes;
                double ticks = (zedGraphControl1.MasterPane.PaneList[0].XAxis.Scale.Max - zedGraphControl1.MasterPane.PaneList[0].XAxis.Scale.Min)*1000;
                int charts = zedGraphControl1.MasterPane.PaneList.Count;
                float point = (float)((10 * 7) / (ticks * charts));
                if (point < 1) point = 1;
                else if (point > 10) point = 10;
                for (int i = 0; i < _alLinesWithSymbols.Count; i++)
                {
                    ((LineItem)_alLinesWithSymbols[i]).Symbol.Size = point;
                }
            }
        }
        private void SetTimes()
        {
            lbFirstDate.Text = _firstDate.ToString();
            lbSecondDate.Text = _lastDate.ToString();
            lbScrollTime.Text = _firstDate.ToString();
            TimeSpan ts = _lastDate - _firstDate;

            #region DETERMINE PAGING SIZE BASED ON TOTAL TIMESPAN OF DATA
            if (ts.TotalHours > 3)//4 or more hours of data
                _minutesPage = 60;
            else if (ts.TotalMinutes > 60)//between 1-4 hours of data
                _minutesPage = 10;
            else if (ts.TotalMinutes > 15) _minutesPage = 5; //between 15-60 minutes of data
            else _minutesPage = 1;
            #endregion

            hScrollBar1.LargeChange = 1;
            hScrollBar1.SmallChange = 1;
            hScrollBar1.Maximum = (int)Math.Ceiling(ts.TotalMinutes / _minutesPage);


            //XDate startx = new XDate(_firstDate.AddMinutes(-padding));
            XDate startx = new XDate(_firstDate);

            //XDate endx = new XDate(_lastDate.AddMinutes(padding));
            XDate endx = new XDate(_lastDate);
            for (int i = 0; i < zedGraphControl1.MasterPane.PaneList.Count; i++)
            {
                zedGraphControl1.MasterPane[i].XAxis.Scale.Min = (double)startx;
                zedGraphControl1.MasterPane[i].XAxis.Scale.Max = (double)endx;
            }

            zedGraphControl1.AxisChange();
            zedGraphControl1.Refresh();
        }

        private void WidenDatesIfNeeded(PointPairList pl)
        {
            if (pl.Count > 0)
                WidenDatesIfNeeded(new XDate(pl[0].X), new XDate(pl[pl.Count - 1].X));
        }
        private void WidenDatesIfNeeded(XDate fDate, XDate lDate)
        {
            if (fDate.DateTime < _firstDate) _firstDate = fDate.DateTime;
            if (lDate.DateTime > _lastDate) _lastDate = lDate.DateTime;
        }

        private void SetEnable(bool isEnabled)
        {
            zedGraphControl1.Enabled = isEnabled;
            groupBox1.Enabled = isEnabled;
            hScrollBar1.Enabled = isEnabled;
            buttonZoomOut.Enabled = isEnabled;

            zedGraphControl1.Visible = isEnabled;
        }
        private void Reset()
        {
            if (!_isFirstTime)
            {
                _firstDate = DateTime.MinValue;
                _lastDate = DateTime.MinValue;

                zedGraphControl1.MasterPane.PaneList.Clear();
                zedGraphControl1.MasterPane.GraphObjList.Clear();
                _htBoxes.Clear();
                _htPanes.Clear();
                _alLinesWithSymbols.Clear();
                groupBox1.Controls.Clear();
                _alCheckBoxes.Clear();
            }
            else _isFirstTime = false;

        }
        #endregion

        #region CHART CONTENT
        #region HEART RATE
        private void AddHeartCurve(GraphPane gp, string name, PointPairList ppl,Color lineColor, PointPairList pplInvalid)
        {
            LineItem pointsCurve = gp.AddCurve("Heart rate " + name, ppl, lineColor, SymbolType.Circle);
            if (!_isAdaptingPointSize) pointsCurve.Symbol.Size = 1F;
            pointsCurve.Symbol.Fill = new Fill(lineColor);
            pointsCurve.Line.IsVisible = false;
            _alLinesWithSymbols.Add(pointsCurve);

            LineItem pointsCurveInvalid = gp.AddCurve("Heart rate " + name +", Out of Range", pplInvalid, lineColor, SymbolType.XCross);
            pointsCurveInvalid.Line.IsVisible = false;
            _alLinesWithSymbols.Add(pointsCurveInvalid);

        }
        
        private void CreateHeartRateGraph(GraphPane gp, string[] files)
        {
            string[] coloroptions = new string[] { "red", "darkred", "tomato", "crimson" };
            for (int f = 0; f < files.Length; f++)
            {
                string[] values = FileReadWrite.ReadLinesFromFile(files[f]);
                string name = Path.GetFileNameWithoutExtension(files[f]).Replace("HeartRate_", "");

                PointPairList listValid = new PointPairList();
                PointPairList listInvalid = new PointPairList();
                DateTime lastDate = new DateTime(1900, 1, 1);
                for (int i = 0; i < values.Length; i++)
                {
                    try
                    {
                        string[] split = values[i].Split(',');
                        if ((split.Length > 2) && (split[1].Length > 0) && (split[2].Length > 0))
                        {
                            DateTime dt = DateTime.Parse(split[1]);

                            double x = (double)new XDate(dt);
                            double y = Convert.ToDouble(split[2]);

                            string label = String.Format("Heart rate from {0}\n{1} {2}", name, dt.ToShortTimeString(), y);

                            if ((y >= 35) && (y <= 190))
                            {
                                if (_isUsingLabels) listValid.Add(x, y, label);
                                else listValid.Add(x, y);
                            }
                            else
                            {
                                if (_isUsingLabels) listInvalid.Add(x, y, label.Replace("Heart", "Invalid heart"));
                                else listInvalid.Add(x, y);
                            }
                        }
                    }
                    catch { }
                }

                AddHeartCurve(gp, name, listValid,Color.FromName(coloroptions[f]),listInvalid);

                WidenDatesIfNeeded(listValid);
            }

        }
        #endregion

        #region MITES
        private static DateTime ConvertUNIXDatTime(double timestamp)
        {
            // First make a System.DateTime equivalent to the UNIX Epoch.
            DateTime dateTime = new System.DateTime(1970, 1, 1, 0, 0, 0, 0);

            // Add the number of seconds in UNIX timestamp to be converted.
            dateTime = dateTime.AddSeconds(timestamp/1000);

            return dateTime;

        }
        

        private void AddAccelerationCurve(string name, string title, PointPairList pplX, PointPairList pplY, PointPairList pplZ, PointPairList pplActivityCount, PointPairList pplSamplingRate)
        {
            GraphPane pane = AddPane(name,"Acceleration - " + title);           

            LineItem pointsCurveX = pane.AddCurve("X", pplX, Color.LightBlue, SymbolType.Circle);
            LineItem pointsCurveY = pane.AddCurve("Y", pplY, Color.Blue, SymbolType.Circle);
            LineItem pointsCurveZ = pane.AddCurve("Z", pplZ, Color.DarkBlue, SymbolType.Circle);
            pointsCurveX.Symbol.Fill = new Fill(Color.LightBlue);
            pointsCurveY.Symbol.Fill = new Fill(Color.Blue);
            pointsCurveZ.Symbol.Fill = new Fill(Color.DarkBlue);
            if (!_isAdaptingPointSize)
            {
                pointsCurveX.Symbol.Size = 1F;
                pointsCurveY.Symbol.Size = 1F;
                pointsCurveZ.Symbol.Size = 1F;
            }
            _alLinesWithSymbols.Add(pointsCurveX);
            _alLinesWithSymbols.Add(pointsCurveY);
            _alLinesWithSymbols.Add(pointsCurveZ);

            pointsCurveX.Line.IsVisible = false;
            pointsCurveY.Line.IsVisible = false;
            pointsCurveZ.Line.IsVisible = false;

            LineItem pointsCurveSampling = pane.AddCurve("Sampling Rate",pplSamplingRate,Color.DarkGray,SymbolType.None);            
            LineItem pointsCurveActivity = pane.AddCurve("Activity Count", pplActivityCount, Color.Violet, SymbolType.None);
            pointsCurveActivity.IsY2Axis = true;
            

            

        }
        private void CreateAccelerationGraph(int paneOrder, string filepath, string channel, string location)
        {
            #region ACCELERATION X Y Z
            string[] accel = FileReadWrite.ReadLinesFromFile(filepath);
                 
            PointPairList listX = new PointPairList();
            PointPairList listY = new PointPairList();
            PointPairList listZ = new PointPairList();                      

            for (int i = 1; i < accel.Length; i++)
            {
                try
                {
                    string[] split = accel[i].Split(',');
                    if ((split.Length > 4) && (split[1].Length > 0) && (split[2].Length > 0))
                    {
                        DateTime dt = DateTime.Parse(split[1]);

                        double x = (double)new XDate(dt);
                        double value = Convert.ToDouble(split[2]);
                        string label = String.Format("Channel {0}, X-axis, at {1}\n{2} {3}", channel, location, dt.ToShortTimeString(), value);

                        if (_isUsingLabels) listX.Add(x, value, label);
                        else listX.Add(x, value);

                        value = Convert.ToDouble(split[3]);
                        label = String.Format("Channel {0}, Y-axis, at {1}\n{2} {3}", channel, location, dt.ToShortTimeString(), value);
                        if (_isUsingLabels) listY.Add(x, value, label);
                        else listY.Add(x, value);

                        value = Convert.ToDouble(split[4]);
                        label = label = String.Format("Channel {0}, Z-axis, at {1}\n{2} {3}", channel, location, dt.ToShortTimeString(), value);
                        if (_isUsingLabels) listZ.Add(x, value, label);
                        else listZ.Add(x, value);
                    }
                }
                catch { }
            }
            #endregion

            #region SAMPLE RATES
            string[] samp = new string[0];
            string[] matches = Directory.GetFiles(Path.GetDirectoryName(filepath), String.Format("MITES_{0}_SampleRate*", channel));
            if (matches.Length == 1)
                samp = FileReadWrite.ReadLinesFromFile(matches[0]);

            PointPairList listSampleRates = new PointPairList();
            for (int i = 1; i < samp.Length; i++)
            {
                try
                {
                    string[] split = samp[i].Split(',');
                    if ((split.Length > 2) && (split[1].Length > 0) && (split[2].Length > 0))
                    {
                        DateTime dt = DateTime.Parse(split[1]);
                        double x = (double)new XDate(dt);
                        double value = Convert.ToDouble(split[2]);
                        string label = String.Format("{0} samples per second at {1}", value, dt.ToShortTimeString());
                        if (_isUsingLabels) listSampleRates.Add(x, value, label);
                        else listSampleRates.Add(x, value);
                    }
                }
                catch { }
            }
            #endregion

            #region ACTIVITY COUNTS
            string[] counts = new string[0];            
            matches = Directory.GetFiles(Path.GetDirectoryName(filepath),String.Format("MITES_{0}_ActivityCount*",channel));
            if (matches.Length == 1)
                counts = FileReadWrite.ReadLinesFromFile(matches[0]);
            PointPairList listActivityCounts = new PointPairList();
            for (int i = 1; i < counts.Length; i++)
            {
                try
                {
                    string[] split = counts[i].Split(',');
                    if ((split.Length > 4) && (split[1].Length > 0) && (split[2].Length > 0))
                    {
                        DateTime dt = DateTime.Parse(split[1]);
                        double x = (double)new XDate(dt);
                        double value = Convert.ToDouble(split[2]) + Convert.ToDouble(split[3]) + Convert.ToDouble(split[4]);
                        string label = String.Format("Activity count {0} at {1}", value, dt.ToShortTimeString());
                        if (_isUsingLabels) listActivityCounts.Add(x, value, label);
                        else listActivityCounts.Add(x, value);
                    }
                }
                catch { }
            }
            #endregion

            AddAccelerationCurve(paneOrder + " MITes " + channel + " " + location, location,listX, listY, listZ,listActivityCounts,listSampleRates);

            WidenDatesIfNeeded(listX);
        }

        #endregion

  
        //ADD_GRAPH STEP 2
        #region Oxycon
        private void CreateOxyconGraph(GraphPane gp, string filePath)
        {
            string[] values = FileReadWrite.ReadLinesFromFile(filePath);

            //One PointPairList for each data column to graph
            //Each of these represents a separate line
            PointPairList listHR = new PointPairList();
            PointPairList listBF = new PointPairList();
            PointPairList listVE = new PointPairList();
            PointPairList listVO2kg = new PointPairList();
            PointPairList listRER = new PointPairList();

            //for each row, add values to PointPairLists
            for (int i = 0; i < values.Length; i++)
            {
                try
                {
                    //expecting values in format: UnixTimeStamp,TimeStamp,OxyconHR,OxyconBF,OxyconVE,OxyconVO2kg,OxyconRER
                    string[] split = values[i].Split(',');

                    if (split.Length > 2) //TimeStamp + at least one data value
                    {
                        #region TIMESTAMP - X VALUE
                        DateTime dt = ConvertUNIXDatTime(Convert.ToDouble(split[0]));//UnixTimeStamp, Column 1/A
                        //DateTime dt = DateTime.Parse(split[1]);//TimeStamp, Column 2/B
                        double x = (double)new XDate(dt);//x value is numeric representation of TimeStamp
                        #endregion

                        #region DATA VALUE - Y VALUE
                        double y = 0; string label = "";

                        #region OxyconHR
                        if ((split.Length > 2) && (split[2].Length > 0))
                        {
                            y = Convert.ToDouble(split[2]);//Column 3/C
                            if (_isUsingLabels)
                            {
                                label = String.Format("HR\n{0} {1}", dt.ToShortTimeString(), y);
                                listHR.Add(x, y, label);
                            }
                            else listHR.Add(x, y);
                        }
                        #endregion

                        #region OxyconBF
                        if ((split.Length > 3) && (split[3].Length > 0))
                        {
                            y = Convert.ToDouble(split[3]);//Column 4/D
                            if (_isUsingLabels)
                            {
                                label = String.Format("BF\n{0} {1}", dt.ToShortTimeString(), y);
                                listBF.Add(x, y, label);
                            }
                            else listBF.Add(x, y);
                        }
                        #endregion

                        #region OxyconVE
                        if ((split.Length > 4) && (split[4].Length > 0))
                        {
                            y = Convert.ToDouble(split[4]);//Column 5/E
                            if (_isUsingLabels)
                            {
                                label = String.Format("VE\n{0} {1}", dt.ToShortTimeString(), y);
                                listVE.Add(x, y, label);
                            }
                            else listVE.Add(x, y);
                        }
                        #endregion

                        #region OxyconV02kg
                        if ((split.Length > 5) && (split[5].Length > 0))
                        {
                            y = Convert.ToDouble(split[5]);//Column 6/F
                            if (_isUsingLabels)
                            {
                                label = String.Format("VO2kg\n{0} {1}", dt.ToShortTimeString(), y);
                                listVO2kg.Add(x, y, label);
                            }
                            else listVO2kg.Add(x, y);
                        }
                        #endregion

                        #region OxyconRER
                        if ((split.Length > 6) && (split[6].Length > 0))
                        {
                            y = Convert.ToDouble(split[6]);//Column 7/G
                            if (_isUsingLabels)
                            {
                                label = String.Format("RER\n{0} {1}", dt.ToShortTimeString(), y);
                                listRER.Add(x, y, label);
                            }
                            else listRER.Add(x, y);
                        }
                        #endregion
                        #endregion

                    }

                }
                catch { }
            }

            #region SET DISPLAY PROPERTIES FOR LINES
            LineItem pointsCurve;

            #region ON Y-AXIS 1 (left-side)
            #region HR
            pointsCurve = gp.AddCurve("Oxycon HR", listHR, Color.Red, SymbolType.Circle);
            pointsCurve.Symbol.Fill = new Fill(Color.Red);
            if (!_isAdaptingPointSize) pointsCurve.Symbol.Size = 1F;
            pointsCurve.Line.IsVisible = false;
            pointsCurve.Tag = "HR";
            _alLinesWithSymbols.Add(pointsCurve);
            #endregion

            #region BF
            pointsCurve = gp.AddCurve("Oxycon BF", listBF, Color.GreenYellow, SymbolType.Square);
            pointsCurve.Symbol.Fill = new Fill(Color.GreenYellow);
            if (!_isAdaptingPointSize) pointsCurve.Symbol.Size = 1F;
            pointsCurve.Line.IsVisible = false;
            pointsCurve.Tag = "BF";
            _alLinesWithSymbols.Add(pointsCurve);
            #endregion
            #endregion

            #region ON Y-AXIS 2 - right-side
            #region VE
            pointsCurve = gp.AddCurve("Oxycon VE", listVE, Color.Orange, SymbolType.Diamond);
            pointsCurve.Symbol.Fill = new Fill(Color.Orange);
            if (!_isAdaptingPointSize) pointsCurve.Symbol.Size = 1F;
            pointsCurve.Line.IsVisible = false;
            pointsCurve.Tag = "VE";
            pointsCurve.IsY2Axis = true;
            _alLinesWithSymbols.Add(pointsCurve);
            #endregion

            #region V02kg
            pointsCurve = gp.AddCurve("Oxycon VO2kg", listVO2kg, Color.Orchid, SymbolType.TriangleDown);
            pointsCurve.Symbol.Fill = new Fill(Color.Orchid);
            if (!_isAdaptingPointSize) pointsCurve.Symbol.Size = 1F;
            pointsCurve.Line.IsVisible = false;
            pointsCurve.Tag = "V02kg";
            pointsCurve.IsY2Axis = true;
            _alLinesWithSymbols.Add(pointsCurve);
            #endregion

            #region RER
            pointsCurve = gp.AddCurve("Oxycon RER", listRER, Color.Navy, SymbolType.Triangle);
            pointsCurve.Symbol.Fill = new Fill(Color.Navy);
            if (!_isAdaptingPointSize) pointsCurve.Symbol.Size = 1F;
            pointsCurve.Line.IsVisible = false;
            pointsCurve.Tag = "RER";
            pointsCurve.IsY2Axis = true;
            _alLinesWithSymbols.Add(pointsCurve);
            #endregion
            #endregion

            #endregion

            //if time-dates for lines include dates not previously graphed, widen range
            WidenDatesIfNeeded(listHR);            

        }
        #endregion

        #region GPS
        private void CreateGPSGraph(GraphPane gp, string filepath)
        {
            string[] values = FileReadWrite.ReadLinesFromFile(filepath);
            PointPairList list_NO = new PointPairList();
            PointPairList list_HAS = new PointPairList();

            for (int i = 0; i < values.Length; i++)
            {
                try
                {
                    string[] split = values[i].Split(',');
                    DateTime dt = DateTime.Parse(split[0]);
                    double x = (double)new XDate(dt);
                    if (split[1].Equals("NOT_AVAILABLE"))
                        list_NO.Add(x, 200, dt.ToShortTimeString() + " GPS NOT AVAILABLE");
                    else list_HAS.Add(x, 205, dt.ToShortTimeString() + " " + split[1] + ", " + split[2]);
                }
                catch { }
            }


            LineItem pointsCurveNo = gp.AddCurve("GPS", list_NO, Color.Gray, SymbolType.Square);
            pointsCurveNo.Line.IsVisible = false;
            if (!_isAdaptingPointSize) pointsCurveNo.Symbol.Size = 1F;
            _alLinesWithSymbols.Add(pointsCurveNo);
            LineItem pointsCurveHas = gp.AddCurve("GPS", list_HAS, Color.Magenta, SymbolType.Triangle);
            pointsCurveHas.Line.IsVisible = false;
            if (!_isAdaptingPointSize) pointsCurveHas.Symbol.Size = 1F;
            pointsCurveHas.Symbol.Fill = new Fill(Color.Magenta);
            _alLinesWithSymbols.Add(pointsCurveHas);
            WidenDatesIfNeeded(list_NO);
        }
        private void CreatePOIGraph(GraphPane gp, string filepath)
        {
            PointPairList list = new PointPairList();
            string[] values = FileReadWrite.ReadLinesFromFile(filepath);
            for (int i = 0; i < values.Length; i++)
            {
                try
                {
                    string[] split = values[i].Split(',');
                    double x = (double)new XDate(DateTime.Parse(split[0]));
                    string tag = "";
                    split = values[i].Split(':', '(');
                    for (int j = 5; j < split.Length; j += 2)
                    {
                        tag += split[j] + "\n";
                    }                   

                    list.Add(x, 200, tag);
                }
                catch { }
            }

            LineItem pointsCurve = gp.AddCurve("POI", list, Color.Black, SymbolType.Star);
            pointsCurve.Line.IsVisible = false;
            pointsCurve.Symbol.Fill = new Fill(Color.Black);
            pointsCurve.Symbol.Size = 6F;
            if (!_isAdaptingPointSize) pointsCurve.Symbol.Size = 1F;
            _alLinesWithSymbols.Add(pointsCurve);
            WidenDatesIfNeeded(list);
        }
        #endregion

        #region LABELS
        private void CreateDiaryGraph(GraphPane gp, string filepath, string title, int y)
        {
            gp.BarSettings.Base = BarBase.Y;
            gp.BarSettings.Type = BarType.Overlay;

            PointPairList labelList = new PointPairList();

            string[] values = FileReadWrite.ReadLinesFromFile(filepath);
            for (int i = 0; i < values.Length; i++)
            {
                try
                {
                    string[] split = values[i].Split(',');
                    DateTime dtStart = DateTime.Parse(split[0]);
                    double startx = (double)new XDate(dtStart);
                    if (split[1].Length > 0)
                    {
                        #region END DATE
                        DateTime dtEnd = DateTime.Parse(split[1]);
                        double endx = (double)new XDate(dtEnd);
                        #endregion

                        #region COLOR OF BAR
                        string color = "black";
                        bool isSolid = false;
                        if ((split.Length > 2) && (split[2].Length > 0))
                        {
                            color = split[2];
                            isSolid = true;
                        }
                        #endregion
                        

                        #region LABEL AND POINT
                        string label = "";
                        for (int c = 3; c < split.Length; c++)
                        {
                            label += split[c].Replace("_", ", ").Replace("-", " ").Trim(',',' ') + "\n ";
                        }
                        labelList = new PointPairList();
                        labelList.Add(endx, y, startx, String.Format("{3} {0}-{1}\n,{2}",dtStart.ToShortTimeString(),dtEnd.ToShortTimeString(),label,title));
                        #endregion

                        #region ADD BAR
                        HiLowBarItem myBar = gp.AddHiLowBar(title, labelList, Color.FromName(color));
                        if (isSolid) myBar.Bar.Fill.Type = FillType.Solid;
                        else myBar.Bar.Fill.Type = FillType.None;
                        #endregion
                    }
                }
                catch { }
            }




        }
        #endregion

        #region PHOTOS and SURVEYS
        private void CreatePhotoGraph(GraphPane gp, string filepath, string imagedir)
        {
            PointPairList list_Photo = new PointPairList();
            string[] values = FileReadWrite.ReadLinesFromFile(filepath);
            for (int i = 0; i < values.Length; i++)
            {
                try
                {
                    string[] split = values[i].Split(',');
                    double x = (double)new XDate(DateTime.Parse(split[0]));
                    list_Photo.Add(x, 25, split[2]+","+Path.Combine(imagedir,split[1]));
                }
                catch { }
            }

            LineItem pointsCurve = gp.AddCurve("photos", list_Photo, Color.Black, SymbolType.Square);
            pointsCurve.Line.IsVisible = false;
            pointsCurve.Symbol.Fill = new Fill(Color.LightGray);
            pointsCurve.Symbol.Size = 10F;

            WidenDatesIfNeeded(list_Photo);
        }

        private void CreateSurveyGraph(GraphPane gp, string filepath)
        {
            PointPairList list = new PointPairList();
            string[] values = FileReadWrite.ReadLinesFromFile(filepath);
            for (int i = 0; i < values.Length; i++)
            {
                try
                {
                    string[] split = values[i].Split(',');
                    double x = (double)new XDate(DateTime.Parse(split[0]));
                    list.Add(x, 20, split[1].Replace(";","\n"));
                }
                catch { }
            }

            LineItem pointsCurve = gp.AddCurve("surveys", list, Color.Purple, SymbolType.Diamond);
            pointsCurve.Line.IsVisible = false;
            pointsCurve.Symbol.Fill = new Fill(Color.Plum);
            pointsCurve.Symbol.Size = 10F;

            WidenDatesIfNeeded(list);
        }

        #endregion

        #region ANNOTATOR-CLASSIFIER
        private void CreateAgreementGraph(GraphPane gp, string[] files)
        {
            DateTime dtLastDate = DateTime.Now;
            for (int f = 0; f < files.Length; f++)
            {
                DateTime dt = dtLastDate.AddMinutes(5);

                string[] values = FileReadWrite.ReadLinesFromFile(files[f]);
                string name = Path.GetFileNameWithoutExtension(files[f]).Replace("average-a_", "");

                PointPairList listAnnotator = new PointPairList();
                PointPairList listClassifier = new PointPairList();
                PointPairList listDifference = new PointPairList();
                ArrayList alAgree_lists = new ArrayList();
                ArrayList alAgree_values = new ArrayList();
                double lastDifference = 0;

                //PointPairList listAgree = new PointPairList();
                //PointPairList listDisagree = new PointPairList();
                //PointPairList listConfusion = new PointPairList();
                double yClass = 2.3, yAnnotate = 1.1;
                for (int i = 0; i < values.Length; i++)
                {
                    try
                    {
                        string[] split = values[i].Split(',');
                        if (split.Length > 2)
                        {
                            
                            int msec = Convert.ToInt32(split[0]);
                            dtLastDate = dt.AddMilliseconds(msec * 400);
                            double x = (double)new XDate(dtLastDate);

                            #region BLOCK GRAPH ANNOTATOR
                            double yA = Convert.ToDouble(split[1]) + 1.1;
                            if (yA != yAnnotate)
                            {
                                listAnnotator.Add(x, yAnnotate);
                                yAnnotate = yA;
                            }
                            listAnnotator.Add(x, yA);
                            yA -= 1.1;
                            #endregion

                            #region BLOCK GRAPH CLASSIFIER
                            double yC = Convert.ToDouble(split[2]) + 2.3;
                            if (yC != yClass)
                            {
                                listClassifier.Add(x, yClass);
                                yClass = yC;
                            }
                            listClassifier.Add(x, yC);
                            yC -= 2.3;
                            #endregion

                            double difference = yC - yA;
                            if (!alAgree_values.Contains(difference))
                            {
                                alAgree_values.Add(difference);
                                alAgree_lists.Add(new PointPairList());
                            }
                            int index = alAgree_values.IndexOf(difference);

                            if (difference != lastDifference)
                            {
                                listDifference.Add(x, lastDifference);
                                int lastindex = alAgree_values.IndexOf(lastDifference);
                                ((PointPairList)alAgree_lists[lastindex]).Add(x, 1);
                                ((PointPairList)alAgree_lists[index]).Add(x, 0);
                                lastDifference = difference;
                            }

                            
                            for (int a = 0; a < alAgree_lists.Count; a++)
                            {
                                if (a != index)
                                    ((PointPairList)alAgree_lists[a]).Add(x, 0);
                            }
                            ((PointPairList)alAgree_lists[index]).Add(x, 1);

                            listDifference.Add(x, difference);

                            //double nyA = 0, nyD = 0, nyC = 0;
                            //if (split[1] == split[2])
                            //{
                            //    if (split[2] == split[3])
                            //        nyA = 3;
                            //    else
                            //        nyD = 3;

                            //}
                            //else nyC = 3;

                            //if (yA != nyA) listAgree.Add(x, yA);
                            //if (yC != nyC) listConfusion.Add(x, yC);
                            //if (yD != nyD) listDisagree.Add(x, yD);
                            //listAgree.Add(x, nyA);
                            //listDisagree.Add(x, nyD);
                            //listConfusion.Add(x, nyC);
                            //yA = nyA; yC = nyC; yD = nyD;

                        }

                    }
                    catch { }
                }
                AddHorizontalText(gp, name, (double)new XDate(dt), Color.Black);
                LineItem lineCurve1 = gp.AddCurve("Annotator 1 " + name, listAnnotator, Color.Green, SymbolType.Default);
                lineCurve1.Symbol.IsVisible = false;
                lineCurve1.Line.IsVisible = true;
                _alLinesWithSymbols.Add(lineCurve1);

                AddVerticalText(gp, "Annotators", 1.1F, Color.Green);


                LineItem lineCurveC = gp.AddCurve("Classifier " + name, listClassifier, Color.Blue, SymbolType.Default);
                lineCurveC.Line.IsVisible = true;
                lineCurveC.Symbol.IsVisible = false;

                AddVerticalText(gp, "Classifier", 2.3F, Color.Blue);

                LineItem lineCurveD = gp.AddCurve("Difference " + name, listDifference, Color.Tomato, SymbolType.Default);
                lineCurveD.Line.IsVisible = true;
                lineCurveD.Line.Width = 0.5F;
                lineCurveD.Symbol.IsVisible = false;
                lineCurveD.Line.Fill = new Fill(Color.Tomato);

                AddVerticalText(gp, "Over\nestimate", 0, Color.Tomato);
                AddVerticalText(gp, "Under\nestimate", -1.0F, Color.Tomato);


                //for (int a = 0; a < alAgree_lists.Count; a++)
                //{
                //    Color fillColor = Color.Yellow;
                //    double diff = ((double)alAgree_values[a]);
                //    if (diff != 0)
                //    {
                //        int percent = Convert.ToInt32(Math.Round((1.1 - Math.Abs(diff)) * 255));
                //        if (diff < 0) fillColor = Color.FromArgb(percent, percent, 255);
                //        else fillColor = Color.FromArgb(255, percent, percent);
                //    }
                //    LineItem lineCurveAgree = gp.AddCurve("Agreement " + name + " " + alAgree_values[a].ToString(), ((PointPairList)alAgree_lists[a]), fillColor, SymbolType.Default);
                //    lineCurveAgree.Line.IsVisible = true;
                //    lineCurveAgree.Symbol.IsVisible = false;
                //    // Fill the area under the curve with a white-red gradient at 45 degrees
                //    lineCurveAgree.Line.Fill = new Fill(fillColor);
                //}

                //LineItem lineCurveAgree = gp.AddCurve("Agreement " + name, listAgree, Color.Green, SymbolType.Default);
                //lineCurveAgree.Line.IsVisible = true;
                //lineCurveAgree.Symbol.IsVisible = false;
                //// Fill the area under the curve with a white-red gradient at 45 degrees
                //lineCurveAgree.Line.Fill = new Fill(Color.White, Color.Green, 45F);

                //LineItem lineCurveDisagree = gp.AddCurve("Disgreement " + name, listDisagree, Color.Red, SymbolType.Default);
                //lineCurveDisagree.Line.IsVisible = true;
                //lineCurveDisagree.Symbol.IsVisible = false;
                //// Fill the area under the curve with a white-red gradient at 45 degrees
                //lineCurveDisagree.Line.Fill = new Fill(Color.White, Color.Red, 45F);

                //LineItem lineCurveConfusion = gp.AddCurve("Confusion " + name, listConfusion, Color.Yellow, SymbolType.Default);
                //lineCurveConfusion.Line.IsVisible = true;
                //lineCurveConfusion.Symbol.IsVisible = false;
                //// Fill the area under the curve with a white-red gradient at 45 degrees
                //lineCurveConfusion.Line.Fill = new Fill(Color.White, Color.Yellow, 45F);



                WidenDatesIfNeeded(listAnnotator);




            }

        }
        private void AddVerticalText(GraphPane gp, string label, double y, Color color)
        {
            TextObj text = new TextObj(label, 0, y);
            // use ChartFraction coordinates so the text is placed relative to the Chart.Rect
            text.Location.CoordinateFrame = CoordType.XChartFractionYScale;
            // rotate the text 90 degrees
            text.FontSpec.Angle = 90.0F;
            text.FontSpec.FontColor = color;
            text.FontSpec.IsBold = true;
            text.FontSpec.Size = 10;
            // Disable the border and background fill options for the text
            text.FontSpec.Border.IsVisible = false;
            text.FontSpec.Fill.IsVisible = false;
            // Align the text such the the Left-Bottom corner is at the specified coordinates
            text.Location.AlignH = AlignH.Left;
            text.Location.AlignV = AlignV.Bottom;
            gp.GraphObjList.Add(text);
        }
        private void AddHorizontalText(GraphPane gp, string label, double x, Color color)
        {
            TextObj text = new TextObj(label, x, 0.05);
            // use ChartFraction coordinates so the text is placed relative to the Chart.Rect
            text.Location.CoordinateFrame = CoordType.XScaleYChartFraction;
            // rotate the text 90 degrees
            text.FontSpec.Angle = 0.0F;
            text.FontSpec.FontColor = color;
            text.FontSpec.IsBold = true;
            text.FontSpec.Size = 10;
            // Disable the border and background fill options for the text
            text.FontSpec.Border.IsVisible = false;
            text.FontSpec.Fill.IsVisible = false;
            // Align the text such the the Left-Bottom corner is at the specified coordinates
            text.Location.AlignH = AlignH.Left;
            text.Location.AlignV = AlignV.Bottom;
            gp.GraphObjList.Add(text);
        }

        #endregion

        // Build the Chart
        private void BuildCharts(string path)
        {
            SetGraphPanels();
            string[] files;

            int paneOrdering = 1;

            #region DETERMINE WHICH GRAPHS TO DISPLAY BASED ON AVAILABLE FILES
            #region ACCELEROMETER GRAPHS
            files = Directory.GetFiles(path, "MITes*Raw*");
            for (int i = 0; i < files.Length; i++)
            {
                string channel = "", location = "";
                string[] sensorinfo = Path.GetFileNameWithoutExtension(files[i]).Split('_');
                if (sensorinfo.Length >= 4)
                {
                    channel = sensorinfo[1];
                    location = sensorinfo[3];
                }
                CreateAccelerationGraph(paneOrdering, files[i], channel, location);
                paneOrdering++;
            }
            #endregion

            //ADD_GRAPH STEP 1
            #region OXYCON
            string oxyFile = Path.Combine(path, "Oxycon-S1.csv"); 
            if (File.Exists(oxyFile))
            {
                string title = paneOrdering + " Oxycon";
                GraphPane ePane = AddPane(title, "Oxycon");
                CreateOxyconGraph(ePane, oxyFile);
                paneOrdering++;
            }
            #endregion

            #region COMBINED DATATYPE GRAPH - USUALLY HAS HEART RATE + 1 or more of GPS data, Annotation labels, or Survey responses
            GraphPane hPane = null;
            string filepath = "";

            files = Directory.GetFiles(path, "HeartRate*");
            if (files.Length > 0)
            {
                string title = paneOrdering + " Heart Rate";
                hPane = AddPane(title, "Beats Per Minute");
                CreateHeartRateGraph(hPane, files);
            }
            else if (AnyMatches(path, "GPS*,POI*"))
            {
                string title = paneOrdering + " Location";
                hPane = AddPane(title, "");
            }
            else if (AnyMatches(path, "annotat*,photos*,surveys*"))
            {
                string title = paneOrdering + " Labels";
                hPane = AddPane(title, "");
            }
            else if (AnyMatches(path, "average-*"))
            {
                string title = paneOrdering + " Annotation";
                hPane = AddPane(title, "");
                hPane.YAxis.IsVisible = false;
                _doesShowHover = false;
            }
            

            if (hPane != null)
            {
                paneOrdering++;
                filepath = Path.Combine(path, "GPSlog.csv");
                if (File.Exists(filepath))
                    CreateGPSGraph(hPane, filepath);
                filepath = Path.Combine(path, "POIlog.csv");
                if (File.Exists(filepath))
                    CreatePOIGraph(hPane, filepath);

                files = Directory.GetFiles(path, "Annotat*");
                for (int i = 0; i < files.Length; i++)
                {
                    string name = Path.GetFileNameWithoutExtension(files[i]);
                    CreateDiaryGraph(hPane, files[i], name, 10 + 20 * i);
                }

                filepath = Path.Combine(path, "photos.csv");
                if (File.Exists(filepath))
                    CreatePhotoGraph(hPane, filepath, path);

                filepath = Path.Combine(path, "surveys.csv");
                if (File.Exists(filepath))
                    CreateSurveyGraph(hPane, filepath);

                files = Directory.GetFiles(path, "average-*");
                if (files.Length > 0)
                    CreateAgreementGraph(hPane, files);

            }
            #endregion





            #endregion

            hScrollBar1.Value = 0;            
            SetTimes();
            RefreshMasterPaneLayout();
        }

        /// <summary>
        /// Determines whether there is at least one file matching one of the supplied filename patterns
        /// </summary>
        /// <param name="pathSearchDirectory">absolute path of the directory to search</param>
        /// <param name="filePatterns">comma separated list of file patterns (using wild cards such as asterisk) to match against directory file list</param>
        /// <returns>true if any files matched for one or more of the supplied patterns within the directory specified by the path</returns>
        private bool AnyMatches(string pathSearchDirectory, string filePatterns)
        {
            bool isMatch = false;
            string[] patternsToMatch = filePatterns.Split(',');
            int i = 0;
            while (!isMatch && (i < patternsToMatch.Length))
            {
                if (Directory.GetFiles(pathSearchDirectory, patternsToMatch[i]).Length > 0) isMatch = true;
                i++;
            }

            return isMatch;
            
        }
        #endregion

        #region INTERACTION

        #region OPEN DATASET
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                _pathDataset = folderBrowserDialog1.SelectedPath;
                OpenDataset(_pathDataset);
            }
        }
        private void OpenDataset(string path)
        {
            if (Directory.Exists(path))
            {
                this.Cursor = Cursors.WaitCursor;
                Reset();
                SetEnable(false);
                this.Refresh();

                BuildCharts(path);
                SetEnable(true);
                this.Cursor = Cursors.Default;
            }
            else Logger.LogError("OpenDataset", path + " does not exist");
        }
        #endregion

        #region ZOOM
        private void hScrollBar1_ValueChanged(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            XDate startx = new XDate(_firstDate.Year, _firstDate.Month, _firstDate.Day, _firstDate.Hour, _firstDate.Minute, _firstDate.Second);
            
            XDate endx = new XDate(startx);
            startx.AddMinutes(hScrollBar1.Value*_minutesPage);
            endx.AddMinutes((hScrollBar1.Value + 1)*_minutesPage);
            for (int i = 0; i < zedGraphControl1.MasterPane.PaneList.Count; i++)
            {
                zedGraphControl1.MasterPane[i].XAxis.Scale.Min = (double)startx;
                zedGraphControl1.MasterPane[i].XAxis.Scale.Max = (double)endx;
            }
            int pixelunits = (int)Math.Ceiling((double)(hScrollBar1.Width - 130) / hScrollBar1.Maximum);
            lbScrollTime.Location = new Point(hScrollBar1.Left + pixelunits* hScrollBar1.Value,lbScrollTime.Location.Y);
            lbScrollTime.Text = String.Format("{0}-{1}", startx.DateTime.ToShortTimeString(), endx.DateTime.ToShortTimeString());

            if (_isAdaptingPointSize) SetPointSize();

            zedGraphControl1.AxisChange();
            zedGraphControl1.Refresh();
            
            this.Cursor = Cursors.Default;
            
        }

        private void buttonZoomOut_Click(object sender, EventArgs e)
        {
            XDate startx = new XDate(_firstDate.Year, _firstDate.Month, _firstDate.Day, _firstDate.Hour, _firstDate.Minute, _firstDate.Second);

            XDate endx = new XDate(_lastDate.Year, _lastDate.Month, _lastDate.Day, _lastDate.Hour, _lastDate.Minute, _lastDate.Second);
            for (int i = 0; i < zedGraphControl1.MasterPane.PaneList.Count; i++)
            {
                zedGraphControl1.MasterPane[i].XAxis.Scale.Min = (double)startx;
                zedGraphControl1.MasterPane[i].XAxis.Scale.Max = (double)endx;
            }
            lbScrollTime.Text = "VIEWING ALL";

            zedGraphControl1.AxisChange();
            zedGraphControl1.Refresh();

            if (_isAdaptingPointSize) SetPointSize(); 
        }
        #endregion

        #region SHOW/HIDE PANES
        private void checkBox_CheckedChanged(object sender, EventArgs e)
        {
            string item = ((CheckBox)sender).Text;
            bool show = ((CheckBox)sender).Checked;
            bool isPane = _htPanes.Contains(item);
            if (!show)
            {
                if (isPane) RemovePane(item);
            }
            else
            {
                if (isPane) ShowPane(item);
            }
            RefreshMasterPaneLayout();
        }

        private void ShowPane(string pane)
        {
            GraphPane gp = (GraphPane)_htPanes[pane];
            int index = 0;
            //determine placement of pane
            bool isFound = false;
            for (int i = 0; i < zedGraphControl1.MasterPane.PaneList.Count; i++)
            {
                if (!isFound)
                {
                    string panetitle = zedGraphControl1.MasterPane.PaneList[i].Title.Text;
                    if (panetitle.CompareTo(pane) > 0)
                    {
                        index = i;
                        isFound = true;
                    }

                }
            }
            if (!isFound) index = zedGraphControl1.MasterPane.PaneList.Count;

            if (gp != null)
                zedGraphControl1.MasterPane.PaneList.Insert(index, gp);

        }

        private void RemovePane(string pane)
        {
            GraphPane gp = zedGraphControl1.MasterPane.PaneList[pane];
            if (gp != null)
                zedGraphControl1.MasterPane.PaneList.Remove(gp);

        }
        #endregion       

        #region HOVER
        PointPair ppHover = null;
        private void HighlightGraphs(double x, double z)
        {
            if (_doesShowHover)
            {
                for (int i = 0; i < zedGraphControl1.MasterPane.PaneList.Count; i++)
                {
                    GraphPane gp = zedGraphControl1.MasterPane.PaneList[i];
                    if (_htBoxes.Contains(gp.Title.Text))
                    {
                        BoxObj boxForLabel = (BoxObj)_htBoxes[gp.Title];
                        gp.GraphObjList.Clear();

                        boxForLabel = new BoxObj(x, gp.YAxis.Scale.Max, z, gp.YAxis.Scale.Max - gp.YAxis.Scale.Min, Color.Black, Color.PapayaWhip);
                        boxForLabel.Location.CoordinateFrame = CoordType.AxisXYScale;
                        boxForLabel.Border.Style = System.Drawing.Drawing2D.DashStyle.DashDot;

                        //// place the box behind the axis items, so the grid is drawn on top of it
                        boxForLabel.ZOrder = ZOrder.F_BehindGrid;
                        _htBoxes[gp.Title] = boxForLabel;
                        gp.GraphObjList.Add(boxForLabel);
                    }
                }
                zedGraphControl1.AxisChange();
                this.Refresh();
            }
        }
        string zedGraphControl1_PointValueEvent(ZedGraphControl sender, GraphPane pane, CurveItem curve, int iPt)
        {

            if (curve[iPt] != ppHover)
            {
                ppHover = curve[iPt];
                if (pictureBox1.Visible)
                {
                    pictureBox1.Visible = false;
                }
                if (curve.Label.Text == "photos")
                {
                    PointF pf = pane.GeneralTransform(curve[iPt].X, curve[iPt].Y, CoordType.AxisXYScale);
                    string[] split = curve[iPt].Tag.ToString().Split(',');
                    pictureBox1.Image = Image.FromFile(split[1]);
                    pictureBox1.Location = new Point(Convert.ToInt32(pf.X), Convert.ToInt32(pf.Y) - pictureBox1.Height);
                    pictureBox1.BringToFront();
                    pictureBox1.Visible = true;
                    return split[0];
                }
                else if (curve.Label.Text.StartsWith("Anno"))
                {
                    HighlightGraphs(curve[iPt].Z, curve[iPt].X - curve[iPt].Z);
                }

            }
            else if (curve.Label.Text == "photos")
            {
                string[] split = curve[iPt].Tag.ToString().Split(',');
                return split[0];
            }

            if (curve[iPt].Tag != null)
                return curve[iPt].Tag.ToString();
            else return curve[iPt].ToString();
        }

        #endregion

        #endregion


    }
}
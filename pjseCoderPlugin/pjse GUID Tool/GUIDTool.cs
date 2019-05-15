/***************************************************************************
 *   Copyright (C) 2005-2008 by Peter L Jones                              *
 *   pljones@users.sf.net                                                  *
 *                                                                         *
 *   This program is free software; you can redistribute it and/or modify  *
 *   it under the terms of the GNU General Public License as published by  *
 *   the Free Software Foundation; either version 2 of the License, or     *
 *   (at your option) any later version.                                   *
 *                                                                         *
 *   This program is distributed in the hope that it will be useful,       *
 *   but WITHOUT ANY WARRANTY; without even the implied warranty of        *
 *   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the         *
 *   GNU General Public License for more details.                          *
 *                                                                         *
 *   You should have received a copy of the GNU General Public License     *
 *   along with this program; if not, write to the                         *
 *   Free Software Foundation, Inc.,                                       *
 *   59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.             *
 ***************************************************************************/
using System;
using System.Drawing;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Threading;
using SimPe.Interfaces;
using SimPe.Interfaces.Plugin;
using SimPe.Interfaces.Scenegraph;
using SimPe.PackedFiles.Wrapper;

namespace pjse.guidtool
{
	/// <summary>
	/// Summary description for GUIDTool.
	/// </summary>
	public class GUIDTool : System.Windows.Forms.Form, ITool
    {
        #region Form variables

        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label lbStatus;
        private RichTextBox rtbReport;
        private TextBox tbNumber;
        private Label lbName;
        private TextBox tbName;
        private Label lbNumber;
        private Button btnSearch;
        private Button btnClose;
        private GroupBox groupBox1;
        private FlowLayoutPanel flpSearchFor;
        private CheckBox ckbObjdGUID;
        private CheckBox ckbObjdName;
        private CheckBox ckbNrefName;
        private CheckBox ckbBhavName;
        private CheckBox ckbBconName;
        private GroupBox groupBox2;
        private FlowLayoutPanel flpSearchIn;
        private RadioButton rb1default;
        private RadioButton rb1CPOnly;
        private Button btnHelp;
        private CheckBox ckbCallsToBHAV;
        private FlowLayoutPanel flpFilter;
        private SimPe.Plugin.GUIDChooser gcGroup;
        private Button btnClearFilter;
        private CheckBox ckbSGSearch;
        private Label label1;
        private CheckBox ckbFromBHAV;
        private CheckBox ckbFromObjf;
        private CheckBox ckbFromTtab;
        private FlowLayoutPanel flpCallsFrom;
        private FlowLayoutPanel flpNames;
        private TableLayoutPanel tlpName;
        private CheckBox ckbGLOB;
        private TableLayoutPanel tableLayoutPanel1;
        private FlowLayoutPanel flpSTR;
        private Label label2;
        private CheckBox ckbSTR;
        private CheckBox ckbCTSS;
        private CheckBox ckbTTAs;
        private CheckBox ckbDefLang;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

        #endregion

        public GUIDTool()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

            this.Height = Math.Max(PersistantHeight, this.MinimumSize.Height);
            this.Width = Math.Max(PersistantWidth, this.MinimumSize.Width);
            if (this.Height < PersistantHeight) PersistantHeight = this.Height;
            if (this.Width < PersistantWidth) PersistantWidth = this.Width;

            lHex32 = new List<TextBox>(new TextBox[] { tbNumber, });
            rbGroup = new List<RadioButton>(new RadioButton[] {rb1default, rb1CPOnly });

            this.oldText = this.btnSearch.Text;
            this.prompt = this.lbStatus.Text;

            SearchComplete += new EventHandler(Complete);

            #region Group filter
            sgNames = new List<string>();
            sgGroups = new List<uint>();
            sgNames.Add("Globals");
            sgGroups.Add(0x7FD46CD0);
            sgNames.Add("Behaviour");
            sgGroups.Add(0x7FE59FD0);
            foreach (SimPe.Data.SemiGlobalAlias sga in SimPe.Data.MetaData.SemiGlobals)
                if (sga.Known)
                {
                    sgNames.Add(sga.Name);
                    sgGroups.Add(sga.Id);
                }

            gcGroup.KnownObjects = new object[] { sgNames, sgGroups, };
            #endregion
        }

        private static string BASENAME = @"PJSE\Bhav\GUIDTool";
        private static SimPe.XmlRegistryKey xrk = SimPe.Helper.WindowsRegistry.PluginRegistryKey;
        private int PersistantHeight
        {
            get
            {
                SimPe.XmlRegistryKey rkf = SimPe.Helper.WindowsRegistry.PluginRegistryKey.CreateSubKey(BASENAME);
                object o = rkf.GetValue("PersistentHeight", this.Height);
                return Convert.ToInt32(o);
            }

            set
            {
                SimPe.XmlRegistryKey rkf = SimPe.Helper.WindowsRegistry.PluginRegistryKey.CreateSubKey(BASENAME);
                rkf.SetValue("PersistentHeight", value);
            }
        }

        private int PersistantWidth
        {
            get
            {
                SimPe.XmlRegistryKey rkf = SimPe.Helper.WindowsRegistry.PluginRegistryKey.CreateSubKey(BASENAME);
                object o = rkf.GetValue("PersistentWidth", this.Width);
                return Convert.ToInt32(o);
            }

            set
            {
                SimPe.XmlRegistryKey rkf = SimPe.Helper.WindowsRegistry.PluginRegistryKey.CreateSubKey(BASENAME);
                rkf.SetValue("PersistentWidth", value);
            }
        }


		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}


        private bool searching = false;
        private int matches = 0;
        private string oldText = null;
        private string prompt = null;
        private Thread searchThread = null;

        private List<String> sgNames = null;
        private List<UInt32> sgGroups = null;

        private List<RadioButton> rbGroup = null;
        private static bool Selected(RadioButton rb) { return rb.Checked; }

        private static int byPackageGroupTypeInstance(pjse.FileTable.Entry x, pjse.FileTable.Entry y)
        {
            int result = x.Package.FileName.CompareTo(y.Package.FileName);
            if (result == 0)
                result = x.Group.CompareTo(y.Group);
            if (result == 0)
                result = x.Type.CompareTo(y.Type);
            if (result == 0)
                result = x.Instance.CompareTo(y.Instance);
            return result;
        }

        /*
         * 0: ckbObjdGUID.Checked, 1: ckbObjdName.Checked,
         * 2: ckbNrefName.Checked,
         * 3: ckbBhavName.Checked,
         * 4: ckbBconName.Checked,
         * 5: ckbCallsToBHAV.Checked,
         * 6: ckbSGSearch.Checked,
         * 7: ckbFromBHAV.Checked, 8: ckbFromObjf.Checked, 9: ckbFromTtab.Checked,
         * 10: ckbGLOB.Checked,
         * 11: ckbSTR.Checked, 12: ckbCTSS.Checked, 13: ckbTTAs.Checked, 14: ckbDefLang.Checked,
         */
        private void Search(object o)
        {
            bool[] type = (bool[])((object[])o)[0];
            FileTable.Source where = (FileTable.Source)((object[])o)[1];
            uint searchNumber = (uint)((object[])o)[2];
            string searchText = (string)((object[])o)[3];
            uint group = (uint)((object[])o)[4];

            SetProgressCallback setProgress = new SetProgressCallback(SetProgress);
            AddResultCallback addResult = new AddResultCallback(AddResult);
            StopSearchCallback stopSearch = new StopSearchCallback(StopSearch);
            EventHandler onSearchComplete = new EventHandler(OnSearchComplete);

            try
            {
                List<pjse.FileTable.Entry> results = new List<FileTable.Entry>();
                if (group != 0)
                {
                    if (type[6])
                    #region Focus on SemiGlobal group
                    {
                        List<pjse.FileTable.Entry> globs = new List<FileTable.Entry>(pjse.FileTable.GFT[SimPe.Data.MetaData.GLOB_FILE, where]);
                        foreach (pjse.FileTable.Entry fte in globs)
                        {
                            SimPe.Plugin.Glob glob = ((SimPe.Plugin.Glob)fte.Wrapper);
                            if (glob == null) continue;
                            if (group != glob.SemiGlobalGroup) continue;

                            List<pjse.FileTable.Entry> temp = new List<FileTable.Entry>();
                            if (type[7]) temp.AddRange(pjse.FileTable.GFT[Bhav.Bhavtype, fte.Group, where]);
                            if (type[8]) temp.AddRange(pjse.FileTable.GFT[Objf.Objftype, fte.Group, where]);
                            if (type[9]) temp.AddRange(pjse.FileTable.GFT[Ttab.Ttabtype, fte.Group, where]);

                            if (fte.Group == 0xffffffff)
                            {
                                foreach (pjse.FileTable.Entry entry in temp)
                                    if (entry.Package == fte.Package) results.Add(entry);
                            }
                            else results.AddRange(temp);
                        }
                        if (type[7]) results.AddRange(pjse.FileTable.GFT[Bhav.Bhavtype, group, where]);
                        if (type[8]) results.AddRange(pjse.FileTable.GFT[Objf.Objftype, group, where]);
                        if (type[9]) results.AddRange(pjse.FileTable.GFT[Ttab.Ttabtype, group, where]);
                    }
                    #endregion
                    else if (type[10])
                    #region References to GLOB
                    {
                        List<pjse.FileTable.Entry> globs = new List<FileTable.Entry>(pjse.FileTable.GFT[SimPe.Data.MetaData.GLOB_FILE, where]);
                        foreach (pjse.FileTable.Entry fte in globs)
                        {
                            SimPe.Plugin.Glob glob = ((SimPe.Plugin.Glob)fte.Wrapper);
                            if (glob == null) continue;
                            if (group != glob.SemiGlobalGroup) continue;

                            pjse.FileTable.Entry[] objds = pjse.FileTable.GFT[SimPe.Data.MetaData.OBJD_FILE, fte.Group, where];

                            if (objds.Length == 0)
                                results.Add(fte);
                            else
                            {
                                if (fte.Group == 0xffffffff)
                                {
                                    foreach(pjse.FileTable.Entry entry in objds)
                                        if (entry.Package == fte.Package)
                                        {
                                            results.Add(entry);
                                            break;
                                        }
                                }
                                else
                                    results.Add(objds[0]);
                            }
                        }
                    }
                    #endregion
                    else
                    #region Search within group
                    {
                        if (type[0] || type[1])
                            results.AddRange(pjse.FileTable.GFT[SimPe.Data.MetaData.OBJD_FILE, group, where]);
                        if (type[2])
                            results.AddRange(pjse.FileTable.GFT[0x4E524546, group, where]); // NREF
                        if (type[3])
                            results.AddRange(pjse.FileTable.GFT[Bhav.Bhavtype, group, where]);
                        if (type[4])
                            results.AddRange(pjse.FileTable.GFT[Bcon.Bcontype, group, where]);
                        if (type[5])
                        {
                            if (type[7]) results.AddRange(pjse.FileTable.GFT[Bhav.Bhavtype, group, where]);
                            if (type[8]) results.AddRange(pjse.FileTable.GFT[Objf.Objftype, group, where]);
                            if (type[9]) results.AddRange(pjse.FileTable.GFT[Ttab.Ttabtype, group, where]);
                        }
                        if (type[11])
                            results.AddRange(pjse.FileTable.GFT[StrWrapper.Strtype, group, where]);
                        if (type[12])
                            results.AddRange(pjse.FileTable.GFT[StrWrapper.CTSStype, group, where]);
                        if (type[13])
                            results.AddRange(pjse.FileTable.GFT[StrWrapper.TTAstype, group, where]);
                    }
                    #endregion
                }
                else // group == 0
                {
                    if (type[6] || type[10]) { } // no results for group == 0
                    else
                    #region Search without group
                    {
                        if (type[0] || type[1])
                            results.AddRange(pjse.FileTable.GFT[SimPe.Data.MetaData.OBJD_FILE, where]);
                        if (type[2])
                            results.AddRange(pjse.FileTable.GFT[0x4E524546, where]); // NREF
                        if (type[3])
                            results.AddRange(pjse.FileTable.GFT[Bhav.Bhavtype, where]);
                        if (type[4])
                            results.AddRange(pjse.FileTable.GFT[Bcon.Bcontype, where]);
                        if (type[5])
                        {
                            if (type[7]) results.AddRange(pjse.FileTable.GFT[Bhav.Bhavtype, where]);
                            if (type[8]) results.AddRange(pjse.FileTable.GFT[Objf.Objftype, where]);
                            if (type[9]) results.AddRange(pjse.FileTable.GFT[Ttab.Ttabtype, where]);
                        }
                        if (type[11])
                            results.AddRange(pjse.FileTable.GFT[StrWrapper.Strtype, where]);
                        if (type[12])
                            results.AddRange(pjse.FileTable.GFT[StrWrapper.CTSStype, where]);
                        if (type[13])
                            results.AddRange(pjse.FileTable.GFT[StrWrapper.TTAstype, where]);
                    }
                    #endregion
                }

                results.Sort(byPackageGroupTypeInstance);

                Invoke(setProgress, new object[] { false, results.Count });

                int j = 0;
                pjse.FileTable.Entry previtem = null;
                foreach (pjse.FileTable.Entry item in results)
                {
                    if (item != previtem)
                    {
                        previtem = item;

                        uint itemguid = 0;

                        System.IO.BinaryReader reader = item.Wrapper.StoredData;
                        if (item.Type == SimPe.Data.MetaData.OBJD_FILE)
                            if (reader.BaseStream.Length > 0x5c + 4) // sizeof(uint)
                            {
                                reader.BaseStream.Seek(0x5c, System.IO.SeekOrigin.Begin);
                                itemguid = reader.ReadUInt32();
                            }

                        if ((type[0] && itemguid == searchNumber) ||
                            ((type[1] || type[2] || type[3]) && item.ToString().ToLower().Contains(searchText)) ||
                            type[10])
                            Invoke(addResult, new object[] { itemguid, item, });

                        else if (type[5]) switch (item.Type)
                            {
                                case Bhav.Bhavtype:
                                    foreach (Instruction i in (Bhav)item.Wrapper)
                                        if (i.OpCode == searchNumber)
                                            Invoke(addResult, new object[] { itemguid, item, });
                                    break;
                                case Objf.Objftype:
                                    foreach (ObjfItem i in (Objf)item.Wrapper)
                                        if (i.Action == searchNumber || i.Guardian == searchNumber)
                                            Invoke(addResult, new object[] { itemguid, item, });
                                    break;
                                case Ttab.Ttabtype:
                                    foreach (TtabItem i in (Ttab)item.Wrapper)
                                        if (i.Action == searchNumber || i.Guardian == searchNumber)
                                            Invoke(addResult, new object[] { itemguid, item, });
                                    break;
                            }

                        else if (((type[11] && item.Type == StrWrapper.Strtype) ||
                          (type[12] && item.Type == StrWrapper.CTSStype) ||
                          (type[13] && item.Type == StrWrapper.TTAstype)))
                        {
                            if (type[14])
                                foreach (StrItem si in ((StrWrapper)item.Wrapper)[(byte)1])
                                {
                                    if (si.Title.ToString().ToLower().Contains(searchText))
                                    {
                                        Invoke(addResult, new object[] { itemguid, item, });
                                        break;
                                    }
                                }
                            else
                                foreach (StrItem si in (StrWrapper)item.Wrapper)
                                {
                                    if (si.Title.ToString().ToLower().Contains(searchText))
                                    {
                                        Invoke(addResult, new object[] { itemguid, item, });
                                        break;
                                    }
                                }
                        }
                    }
                //DealtWith:
                    Invoke(setProgress, new object[] { true, ++j });
                    Thread.Sleep(0);
                    if ((bool)Invoke(stopSearch))
                        break;
                }
            }
            catch (ThreadInterruptedException) { }
            finally
            {
                Thread.Sleep(0);
                BeginInvoke(onSearchComplete, new object[] { this, EventArgs.Empty });
            }
        }

        private delegate void SetProgressCallback(bool maxOrValue, int progress);
        private void SetProgress(bool maxOrValue, int progress)
        {
            if (maxOrValue == false)
            {
                SimPe.WaitingScreen.Stop();
                this.progressBar1.Maximum = progress;
            }
            else
                this.progressBar1.Value = progress;
        }

        private delegate void AddResultCallback(uint itemguid, pjse.FileTable.Entry item);
        private void AddResult(uint itemguid, pjse.FileTable.Entry item)
        {
            //string report_line = "Group {0}: [{1} guid: {2}] {3} ({4})";
            if (item.Type == SimPe.Data.MetaData.OBJD_FILE)
            {
                this.rtbReport.AppendText(Localization.GetString("gt_reportOBJD",
                    SimPe.Helper.HexString(item.PFD.Group),
                    item.PFD.TypeName.Name,
                    "0x" + SimPe.Helper.HexString(itemguid),
                    item.ToString(),
                    item.Package.FileName) + "\r\n");
            }
            else
            //string report_line = "Group {0}: [{1} {2}] {3} ({4})";
            {
                this.rtbReport.AppendText(Localization.GetString("gt_report",
                    SimPe.Helper.HexString(item.PFD.Group),
                    item.PFD.TypeName.Name,
                    item.ToString(),
                    item.Package.FileName)+"\r\n");
            }

            this.rtbReport.ScrollToCaret();
            matches++;
        }

        private delegate bool StopSearchCallback();
        private bool StopSearch()
        {
            return !searching;
        }

        private event EventHandler SearchComplete;
        private void OnSearchComplete(object sender, EventArgs e)
        {
            if (SearchComplete != null) { SearchComplete(sender, e); }
        }



        private void Start()
        {
            bool[] type = new bool[] {
                /*0*/ckbObjdGUID.Checked, ckbObjdName.Checked, ckbNrefName.Checked, ckbBhavName.Checked, ckbBconName.Checked,
                /*5*/ckbCallsToBHAV.Checked, ckbSGSearch.Checked, ckbFromBHAV.Checked, ckbFromObjf.Checked, ckbFromTtab.Checked,
                /*10*/ckbGLOB.Checked, ckbSTR.Checked, ckbCTSS.Checked, ckbTTAs.Checked, ckbDefLang.Checked,
            };
            uint number = 0;
            try { number = Convert.ToUInt32(this.tbNumber.Text.Trim(), 16); }
            catch(System.FormatException) { number = 0; }
            this.tbNumber.Text = "0x" + SimPe.Helper.HexString(number);
            if (number == 0) { type[0] = type[5] = false; } // don't search for 0 GUID...
            if (number < 0x2000 || number > 0x2fff) { type[6] = false; } // don't do SG search except for SG BHAVs...

            if (gcGroup.Value == 0) { type[6] = type[10] = false; } // don't search with no Group filter

            this.tbName.Text = this.tbName.Text.Trim().ToLower();
            if (this.tbName.Text.Length == 0) { type[1] = type[2] = type[3] = type[4] = type[11] = type[12] = type[13] = false; } // don't search for empty string

            SimPe.WaitingScreen.Wait();
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;
            // this.rtbReport.UseWaitCursor = true; // Methods missing from Mono
            this.btnSearch.Cursor = System.Windows.Forms.Cursors.Default;
            this.flpSearchFor.Enabled = this.flpSearchIn.Enabled = this.flpFilter.Enabled = this.tlpName.Enabled =
                this.btnClose.Enabled = false;
            this.btnSearch.Text = pjse.Localization.GetString("gt_Stop");
            this.lbStatus.Visible = false;
            this.progressBar1.Value = 0;
            this.progressBar1.Visible = true;
            this.rtbReport.Text = "";

            searching = true;
            matches = 0;

            FileTable.Source[] aS = new FileTable.Source[] { FileTable.Source.Any, FileTable.Source.Local };
            FileTable.Source s;
            int rbS = rbGroup.FindIndex(Selected);

            s = (rbS >= 0 && rbS < aS.Length) ? aS[rbS] : FileTable.Source.Any;

            searchThread = new Thread(new ParameterizedThreadStart(Search));
            searchThread.Start(new object[] { type, s, number, this.tbName.Text, gcGroup.Value });
        }

        private void Stop()
        {
            if (!searching) Complete(null, null);
            else
            {
                this.btnSearch.Enabled = false;
                this.btnSearch.Cursor = System.Windows.Forms.Cursors.WaitCursor;
                searching = false;
            }
        }

        private void Complete(object sender, EventArgs e)
        {
            searching = false;
            while (searchThread != null && searchThread.IsAlive)
                searchThread.Join(10);
            searchThread = null;
            this.Cursor = this.btnSearch.Cursor = System.Windows.Forms.Cursors.Default;
            //this.rtbReport.UseWaitCursor = false; // Methods missing from Mono
            this.flpSearchFor.Enabled = this.flpSearchIn.Enabled = this.flpFilter.Enabled = this.tlpName.Enabled =
                this.btnClose.Enabled = this.btnSearch.Enabled = true;
            this.btnSearch.Text = oldText;
            this.progressBar1.Value = 0;
            this.progressBar1.Visible = false;
            this.lbStatus.Visible = true;
            if (matches > 0)
                this.lbStatus.Text = pjse.Localization.GetString("gt_MatchesFound") + ": " + matches.ToString();
            else
                this.lbStatus.Text = pjse.Localization.GetString("gt_NoMatchesFound");
        }


        List<TextBox> lHex32 = null;
        private bool hex32_IsValid(object sender)
		{
			if (!(sender is TextBox) || lHex32.IndexOf((TextBox)sender) < 0)
				throw new Exception("hex32_IsValid not applicable to control " + sender.ToString());
			try { Convert.ToUInt32(((TextBox)sender).Text, 16); }
			catch (Exception) { return false; }
			return true;
		}


        #region ITool Members

        public bool IsEnabled(SimPe.Interfaces.Files.IPackedFileDescriptor pfd, SimPe.Interfaces.Files.IPackageFile package)
        {
            rb1CPOnly.Enabled = (package != null);
            if (!rb1CPOnly.Enabled && rb1CPOnly.Checked)
                rb1default.Checked = true;

            return true;
        }

        public SimPe.Interfaces.Plugin.IToolResult ShowDialog(ref SimPe.Interfaces.Files.IPackedFileDescriptor pfd, ref SimPe.Interfaces.Files.IPackageFile package)
        {
            Complete(null, null);
            //this.tbGUID.Text = "0x" + SimPe.Helper.HexString((uint)0);
            //this.rtbReport.Text = this.tbName.Text = "";
            this.progressBar1.Visible = false;
            this.lbStatus.Text = this.prompt;
            this.lbStatus.Visible = true;

            rb1CPOnly.Enabled = (package != null);
            if (!rb1CPOnly.Enabled && rb1CPOnly.Checked)
                rb1default.Checked = true;

            this.WindowState = FormWindowState.Normal;
            this.TopMost = true;
            this.Show();
            this.TopMost = false;

            return new SimPe.Plugin.ToolResult(false, false);
        }

        #endregion

        #region IToolPlugin Members

        public override string ToString()
        {
            return "PJSE\\" + pjse.Localization.GetString("gt_ResourceFinder");
        }

        #endregion

        #region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GUIDTool));
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.lbStatus = new System.Windows.Forms.Label();
            this.rtbReport = new System.Windows.Forms.RichTextBox();
            this.lbNumber = new System.Windows.Forms.Label();
            this.tbNumber = new System.Windows.Forms.TextBox();
            this.lbName = new System.Windows.Forms.Label();
            this.tbName = new System.Windows.Forms.TextBox();
            this.flpFilter = new System.Windows.Forms.FlowLayoutPanel();
            this.gcGroup = new SimPe.Plugin.GUIDChooser();
            this.ckbSGSearch = new System.Windows.Forms.CheckBox();
            this.btnClearFilter = new System.Windows.Forms.Button();
            this.btnSearch = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.flpSTR = new System.Windows.Forms.FlowLayoutPanel();
            this.label2 = new System.Windows.Forms.Label();
            this.ckbSTR = new System.Windows.Forms.CheckBox();
            this.ckbCTSS = new System.Windows.Forms.CheckBox();
            this.ckbTTAs = new System.Windows.Forms.CheckBox();
            this.ckbDefLang = new System.Windows.Forms.CheckBox();
            this.flpSearchFor = new System.Windows.Forms.FlowLayoutPanel();
            this.ckbObjdGUID = new System.Windows.Forms.CheckBox();
            this.ckbCallsToBHAV = new System.Windows.Forms.CheckBox();
            this.flpCallsFrom = new System.Windows.Forms.FlowLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.ckbFromBHAV = new System.Windows.Forms.CheckBox();
            this.ckbFromObjf = new System.Windows.Forms.CheckBox();
            this.ckbFromTtab = new System.Windows.Forms.CheckBox();
            this.ckbGLOB = new System.Windows.Forms.CheckBox();
            this.flpNames = new System.Windows.Forms.FlowLayoutPanel();
            this.ckbObjdName = new System.Windows.Forms.CheckBox();
            this.ckbNrefName = new System.Windows.Forms.CheckBox();
            this.ckbBhavName = new System.Windows.Forms.CheckBox();
            this.ckbBconName = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.flpSearchIn = new System.Windows.Forms.FlowLayoutPanel();
            this.rb1default = new System.Windows.Forms.RadioButton();
            this.rb1CPOnly = new System.Windows.Forms.RadioButton();
            this.btnHelp = new System.Windows.Forms.Button();
            this.tlpName = new System.Windows.Forms.TableLayoutPanel();
            this.flpFilter.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.flpSTR.SuspendLayout();
            this.flpSearchFor.SuspendLayout();
            this.flpCallsFrom.SuspendLayout();
            this.flpNames.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.flpSearchIn.SuspendLayout();
            this.tlpName.SuspendLayout();
            this.SuspendLayout();
            // 
            // progressBar1
            // 
            resources.ApplyResources(this.progressBar1, "progressBar1");
            this.progressBar1.Name = "progressBar1";
            // 
            // lbStatus
            // 
            resources.ApplyResources(this.lbStatus, "lbStatus");
            this.lbStatus.Name = "lbStatus";
            // 
            // rtbReport
            // 
            resources.ApplyResources(this.rtbReport, "rtbReport");
            this.rtbReport.DetectUrls = false;
            this.rtbReport.Name = "rtbReport";
            this.rtbReport.ReadOnly = true;
            this.rtbReport.ShowSelectionMargin = true;
            // 
            // lbNumber
            // 
            resources.ApplyResources(this.lbNumber, "lbNumber");
            this.lbNumber.Name = "lbNumber";
            // 
            // tbNumber
            // 
            resources.ApplyResources(this.tbNumber, "tbNumber");
            this.tbNumber.Name = "tbNumber";
            // 
            // lbName
            // 
            resources.ApplyResources(this.lbName, "lbName");
            this.lbName.Name = "lbName";
            // 
            // tbName
            // 
            resources.ApplyResources(this.tbName, "tbName");
            this.tbName.Name = "tbName";
            // 
            // flpFilter
            // 
            resources.ApplyResources(this.flpFilter, "flpFilter");
            this.flpFilter.Controls.Add(this.gcGroup);
            this.flpFilter.Controls.Add(this.ckbSGSearch);
            this.flpFilter.Controls.Add(this.btnClearFilter);
            this.flpFilter.Name = "flpFilter";
            // 
            // gcGroup
            // 
            resources.ApplyResources(this.gcGroup, "gcGroup");
            this.gcGroup.ComboBoxWidth = 240;
            this.gcGroup.DropDownHeight = 106;
            this.gcGroup.DropDownWidth = 240;
            this.gcGroup.Label = "Group Filter:";
            this.gcGroup.Name = "gcGroup";
            this.gcGroup.Value = ((uint)(0u));
            // 
            // ckbSGSearch
            // 
            resources.ApplyResources(this.ckbSGSearch, "ckbSGSearch");
            this.ckbSGSearch.Name = "ckbSGSearch";
            this.ckbSGSearch.UseVisualStyleBackColor = true;
            // 
            // btnClearFilter
            // 
            resources.ApplyResources(this.btnClearFilter, "btnClearFilter");
            this.btnClearFilter.Name = "btnClearFilter";
            this.btnClearFilter.UseVisualStyleBackColor = true;
            this.btnClearFilter.Click += new System.EventHandler(this.btnClearFilter_Click);
            // 
            // btnSearch
            // 
            resources.ApplyResources(this.btnSearch, "btnSearch");
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // btnClose
            // 
            resources.ApplyResources(this.btnClose, "btnClose");
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnClose.Name = "btnClose";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Controls.Add(this.tableLayoutPanel1);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.flpSTR, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.flpSearchFor, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.ckbGLOB, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.flpNames, 0, 1);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // flpSTR
            // 
            resources.ApplyResources(this.flpSTR, "flpSTR");
            this.flpSTR.Controls.Add(this.label2);
            this.flpSTR.Controls.Add(this.ckbSTR);
            this.flpSTR.Controls.Add(this.ckbCTSS);
            this.flpSTR.Controls.Add(this.ckbTTAs);
            this.flpSTR.Controls.Add(this.ckbDefLang);
            this.flpSTR.Name = "flpSTR";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // ckbSTR
            // 
            resources.ApplyResources(this.ckbSTR, "ckbSTR");
            this.ckbSTR.Name = "ckbSTR";
            this.ckbSTR.UseVisualStyleBackColor = true;
            this.ckbSTR.CheckedChanged += new System.EventHandler(this.ckbSomeName_CheckedChanged);
            // 
            // ckbCTSS
            // 
            resources.ApplyResources(this.ckbCTSS, "ckbCTSS");
            this.ckbCTSS.Name = "ckbCTSS";
            this.ckbCTSS.UseVisualStyleBackColor = true;
            this.ckbCTSS.CheckedChanged += new System.EventHandler(this.ckbSomeName_CheckedChanged);
            // 
            // ckbTTAs
            // 
            resources.ApplyResources(this.ckbTTAs, "ckbTTAs");
            this.ckbTTAs.Name = "ckbTTAs";
            this.ckbTTAs.UseVisualStyleBackColor = true;
            this.ckbTTAs.CheckedChanged += new System.EventHandler(this.ckbSomeName_CheckedChanged);
            // 
            // ckbDefLang
            // 
            resources.ApplyResources(this.ckbDefLang, "ckbDefLang");
            this.ckbDefLang.Name = "ckbDefLang";
            this.ckbDefLang.UseVisualStyleBackColor = true;
            // 
            // flpSearchFor
            // 
            resources.ApplyResources(this.flpSearchFor, "flpSearchFor");
            this.flpSearchFor.Controls.Add(this.ckbObjdGUID);
            this.flpSearchFor.Controls.Add(this.ckbCallsToBHAV);
            this.flpSearchFor.Controls.Add(this.flpCallsFrom);
            this.flpSearchFor.Name = "flpSearchFor";
            // 
            // ckbObjdGUID
            // 
            resources.ApplyResources(this.ckbObjdGUID, "ckbObjdGUID");
            this.ckbObjdGUID.Name = "ckbObjdGUID";
            this.ckbObjdGUID.CheckedChanged += new System.EventHandler(this.ckbObjdGUID_CheckedChanged);
            // 
            // ckbCallsToBHAV
            // 
            resources.ApplyResources(this.ckbCallsToBHAV, "ckbCallsToBHAV");
            this.ckbCallsToBHAV.Name = "ckbCallsToBHAV";
            this.ckbCallsToBHAV.UseVisualStyleBackColor = true;
            this.ckbCallsToBHAV.CheckedChanged += new System.EventHandler(this.ckbCallsToBHAV_CheckedChanged);
            // 
            // flpCallsFrom
            // 
            resources.ApplyResources(this.flpCallsFrom, "flpCallsFrom");
            this.flpCallsFrom.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.flpCallsFrom.Controls.Add(this.label1);
            this.flpCallsFrom.Controls.Add(this.ckbFromBHAV);
            this.flpCallsFrom.Controls.Add(this.ckbFromObjf);
            this.flpCallsFrom.Controls.Add(this.ckbFromTtab);
            this.flpSearchFor.SetFlowBreak(this.flpCallsFrom, true);
            this.flpCallsFrom.Name = "flpCallsFrom";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // ckbFromBHAV
            // 
            resources.ApplyResources(this.ckbFromBHAV, "ckbFromBHAV");
            this.ckbFromBHAV.Name = "ckbFromBHAV";
            // 
            // ckbFromObjf
            // 
            resources.ApplyResources(this.ckbFromObjf, "ckbFromObjf");
            this.ckbFromObjf.Name = "ckbFromObjf";
            // 
            // ckbFromTtab
            // 
            resources.ApplyResources(this.ckbFromTtab, "ckbFromTtab");
            this.ckbFromTtab.Name = "ckbFromTtab";
            // 
            // ckbGLOB
            // 
            resources.ApplyResources(this.ckbGLOB, "ckbGLOB");
            this.ckbGLOB.Name = "ckbGLOB";
            this.ckbGLOB.CheckedChanged += new System.EventHandler(this.ckbGLOB_CheckedChanged);
            // 
            // flpNames
            // 
            resources.ApplyResources(this.flpNames, "flpNames");
            this.flpNames.Controls.Add(this.ckbObjdName);
            this.flpNames.Controls.Add(this.ckbNrefName);
            this.flpNames.Controls.Add(this.ckbBhavName);
            this.flpNames.Controls.Add(this.ckbBconName);
            this.flpNames.Name = "flpNames";
            // 
            // ckbObjdName
            // 
            resources.ApplyResources(this.ckbObjdName, "ckbObjdName");
            this.ckbObjdName.Name = "ckbObjdName";
            this.ckbObjdName.CheckedChanged += new System.EventHandler(this.ckbSomeName_CheckedChanged);
            // 
            // ckbNrefName
            // 
            resources.ApplyResources(this.ckbNrefName, "ckbNrefName");
            this.ckbNrefName.Name = "ckbNrefName";
            this.ckbNrefName.CheckedChanged += new System.EventHandler(this.ckbSomeName_CheckedChanged);
            // 
            // ckbBhavName
            // 
            resources.ApplyResources(this.ckbBhavName, "ckbBhavName");
            this.ckbBhavName.Name = "ckbBhavName";
            this.ckbBhavName.CheckedChanged += new System.EventHandler(this.ckbSomeName_CheckedChanged);
            // 
            // ckbBconName
            // 
            resources.ApplyResources(this.ckbBconName, "ckbBconName");
            this.ckbBconName.Name = "ckbBconName";
            this.ckbBconName.CheckedChanged += new System.EventHandler(this.ckbSomeName_CheckedChanged);
            // 
            // groupBox2
            // 
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Controls.Add(this.flpSearchIn);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // flpSearchIn
            // 
            resources.ApplyResources(this.flpSearchIn, "flpSearchIn");
            this.flpSearchIn.Controls.Add(this.rb1default);
            this.flpSearchIn.Controls.Add(this.rb1CPOnly);
            this.flpSearchIn.Name = "flpSearchIn";
            // 
            // rb1default
            // 
            resources.ApplyResources(this.rb1default, "rb1default");
            this.rb1default.Checked = true;
            this.rb1default.Name = "rb1default";
            this.rb1default.TabStop = true;
            // 
            // rb1CPOnly
            // 
            resources.ApplyResources(this.rb1CPOnly, "rb1CPOnly");
            this.rb1CPOnly.Name = "rb1CPOnly";
            // 
            // btnHelp
            // 
            resources.ApplyResources(this.btnHelp, "btnHelp");
            this.btnHelp.Name = "btnHelp";
            this.btnHelp.Click += new System.EventHandler(this.btnHelp_Click);
            // 
            // tlpName
            // 
            resources.ApplyResources(this.tlpName, "tlpName");
            this.tlpName.Controls.Add(this.lbNumber, 0, 0);
            this.tlpName.Controls.Add(this.tbNumber, 1, 0);
            this.tlpName.Controls.Add(this.tbName, 1, 1);
            this.tlpName.Controls.Add(this.lbName, 0, 1);
            this.tlpName.Name = "tlpName";
            // 
            // GUIDTool
            // 
            this.AcceptButton = this.btnSearch;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnClose;
            this.Controls.Add(this.tlpName);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.flpFilter);
            this.Controls.Add(this.btnHelp);
            this.Controls.Add(this.btnSearch);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.lbStatus);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.rtbReport);
            this.Name = "GUIDTool";
            this.flpFilter.ResumeLayout(false);
            this.flpFilter.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.flpSTR.ResumeLayout(false);
            this.flpSTR.PerformLayout();
            this.flpSearchFor.ResumeLayout(false);
            this.flpSearchFor.PerformLayout();
            this.flpCallsFrom.ResumeLayout(false);
            this.flpCallsFrom.PerformLayout();
            this.flpNames.ResumeLayout(false);
            this.flpNames.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.flpSearchIn.ResumeLayout(false);
            this.flpSearchIn.PerformLayout();
            this.tlpName.ResumeLayout(false);
            this.tlpName.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

        protected override void OnClosing(CancelEventArgs e)
        {
            PersistantHeight = this.Height;
            PersistantWidth = this.Width;
            searching = false;
            if (searchThread != null && searchThread.IsAlive)
            {
                searchThread.Interrupt();
                searchThread.Join();
                searchThread = null;
            }
            e.Cancel = true;
            Hide();
        }

		private void hex32_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (hex32_IsValid(sender)) return;

			e.Cancel = true;

			uint val = 0;
			switch (lHex32.IndexOf((TextBox)sender))
			{
				case 0: val = 0; break;
			}

			((TextBox)sender).Text = "0x" + SimPe.Helper.HexString(val);
			((TextBox)sender).SelectAll();
		}

        private void btnSearch_Click(object sender, System.EventArgs e)
        {
            if (searching)
                Stop();
            else
                Start();
        }

        private void btnHelp_Click(object sender, System.EventArgs e)
        {
            string protocol = "file://";
            string relativePathToHelp = "pjse.coder.plugin/PJSE_Help";

            SimPe.RemoteControl.ShowHelp(protocol + SimPe.Helper.SimPePluginPath + "/" + relativePathToHelp + "/Finder.htm");
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Hide();
        }

        private bool isCkbObjdGUIDEnabled { get { return !ckbCallsToBHAV.Checked && !ckbGLOB.Checked; } }
        private bool isCkbCallsToBHAVEnabled { get { return !ckbObjdGUID.Checked && !ckbGLOB.Checked && !isCkbSomeTextChecked; } }
        private bool isCkbGLOBEnabled { get { return !ckbObjdGUID.Checked && !ckbCallsToBHAV.Checked && !isCkbSomeTextChecked; } }
        private bool isFlpNamesEnabled { get { return !ckbCallsToBHAV.Checked && !ckbGLOB.Checked; } }
        private static bool isChecked(CheckBox cb) { return cb.Checked; }
        private bool isCkbSomeTextChecked { get { return isCkbSomeNameChecked || isCkbSomeStringChecked; } }
        private bool isCkbSomeNameChecked
        {
            get
            {
                List<CheckBox> lcb = new List<CheckBox>(new CheckBox[] { ckbObjdName, ckbNrefName, ckbBhavName, ckbBconName, });
                return (lcb.Find(isChecked) != null);
            }
        }
        private bool isCkbSomeStringChecked
        {
            get
            {
                List<CheckBox> lcb = new List<CheckBox>(new CheckBox[] { ckbSTR, ckbCTSS, ckbTTAs, });
                return (lcb.Find(isChecked) != null);
            }
        }

        private void ckbObjdGUID_CheckedChanged(object sender, EventArgs e)
        {
            ckbCallsToBHAV.Enabled = isCkbCallsToBHAVEnabled;
            ckbGLOB.Enabled = isCkbGLOBEnabled;
            flpSTR.Enabled = flpNames.Enabled = isFlpNamesEnabled;

            if (ckbObjdGUID.Checked) ckbCallsToBHAV.Checked = ckbGLOB.Checked = false;

            tbNumber.Enabled = ckbObjdGUID.Checked;
            lbNumber.Text = ckbObjdGUID.Checked ? pjse.Localization.GetString("GUID") : "";
        }

        private void ckbCallsToBHAV_CheckedChanged(object sender, EventArgs e)
        {
            ckbGLOB.Enabled = isCkbGLOBEnabled;
            ckbObjdGUID.Enabled = isCkbObjdGUIDEnabled;
            flpSTR.Enabled = flpNames.Enabled = isFlpNamesEnabled;

            if (ckbCallsToBHAV.Checked) ckbObjdGUID.Checked = ckbGLOB.Checked = false;

            tbNumber.Enabled = ckbSGSearch.Enabled = flpCallsFrom.Enabled = ckbCallsToBHAV.Checked;
            lbNumber.Text = ckbCallsToBHAV.Checked ? pjse.Localization.GetString("OpCode") : "";
        }

        private void ckbGLOB_CheckedChanged(object sender, EventArgs e)
        {
            ckbCallsToBHAV.Enabled = isCkbObjdGUIDEnabled;
            ckbObjdGUID.Enabled = isCkbObjdGUIDEnabled;
            flpSTR.Enabled = flpNames.Enabled = isFlpNamesEnabled;

            if (ckbGLOB.Checked) ckbObjdGUID.Checked = ckbCallsToBHAV.Checked = false;
        }

        private void ckbSomeName_CheckedChanged(object sender, EventArgs e)
        {
            ckbCallsToBHAV.Enabled = isCkbCallsToBHAVEnabled;
            ckbGLOB.Enabled = isCkbGLOBEnabled;
            ckbObjdGUID.Enabled = isCkbObjdGUIDEnabled;

            lbName.Enabled = tbName.Enabled = isCkbSomeTextChecked;
            ckbDefLang.Enabled = isCkbSomeStringChecked;
        }

        private void btnClearFilter_Click(object sender, EventArgs e)
        {
            gcGroup.Value = 0;
        }
	}
}
namespace SimPe.Plugin
{
	public class WrapperFactory : AbstractWrapperFactory, IToolFactory
	{
        private static pjse.guidtool.GUIDTool theTool = new pjse.guidtool.GUIDTool();

		#region IToolFactory Members

		public SimPe.Interfaces.IToolPlugin[] KnownTools
		{
			get
			{
				IToolPlugin[] tools = {
										  theTool
									  };
				return tools;
			}
		}

		#endregion
	}

}

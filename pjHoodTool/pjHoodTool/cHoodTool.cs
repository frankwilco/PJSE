/***************************************************************************
 *   Copyright (C) 2008 by Peter L Jones                                   *
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
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using SimPe;
using SimPe.Interfaces;
using SimPe.Interfaces.Files;
using SimPe.PackedFiles.Wrapper;
using SimPe.Plugin;

namespace pjHoodTool
{
    /*
        University = 0x2,//1
        Nightlife = 0x4,//2
        Business = 0x8,//3
        (SP)FamilyFun = 0x10,//4
        (SP)Glamour = 0x20,//5
        Pets = 0x40,//6
        Seasons = 0x80,//7
        (SP)Celebrations = 0x100,//8
        (SP)Fashion = 0x200,//9
        Voyage = 0x400,//10
        (SP)Teen = 0x800,//11
        (SP)Store = 0x1000,//12
        FreeTime = 0x2000,//13
        (SP)Kitchens = 0x4000,//14
        (SP)IKEA = 0x8000,//15
        Apartments = 0x00010000,//16 --Flags2--
        (SP)Mansions = 0x00020000,//17
    */
    class cHoodTool : ITool, ICommandLine
    {
        string q(string u)
        {
            if (u == null) return u;
            //if (!u.Contains(" ") && !u.Contains(",") && !u.Contains("\r") && !u.Contains("\n") && !u.Contains("\"")) return u;
            return "\"" + u.Replace("\"", "\"\"") + "\"";
        }


        delegate void Splash(string message);
        Splash splash;

        void Rufio(string output, string hood, int group)
        {
            if (output.Length == 0)
                output = Path.Combine(Path.Combine(SimPe.PathProvider.SimSavegameFolder, "Rufio"), "ExportedSims.txt");
            else
                output = Path.Combine(Path.Combine(output, "Rufio"), "ExportedSims.txt");

            string outPath = Path.GetDirectoryName(output);
            if (!Directory.Exists(outPath))
                Directory.CreateDirectory(outPath);

            if (group < 1) group = PathProvider.Global.CurrentGroup;

            StreamWriter w1 = new StreamWriter(output);
            w1.AutoFlush = true;
            StreamWriter w2 = new StreamWriter(Path.Combine(outPath,"ExportedLots.txt"));
            w2.AutoFlush = true;
            
            splash(L.Get("pjCHoodTool"));
            try
            {
                #region ExportedSims header
                w1.WriteLine("hood" +
                    ",HoodName" +
                    ",NID,FirstName,LastName,SimDescription" +
                    ",FamilyInstance,HouseholdName" +
                    ",HouseNumber" +
                    ",AvailableCharacterData,Unlinked" +
                    ",ParentA,ParentB,Spouse" +
                    //",Ghost(Objects,Walls,People,Freely)" +
                    ",BodyType" +
                    //",AutonomyLevel"+
                    ",NPCType" +
                    //",MotivesStatic,VoiceType"+
                    ",SchoolType,Grade,CareerPerformance,Career,CareerLevel,ZodiacSign,Aspiration,Gender" +
                    ",LifeSection,AgeDaysLeft,PrevAgeDays,AgeDuration"+
                    ",BlizLifelinePoints,LifelinePoints,LifelineScore" +
                    ",GenActive,GenNeat,GenNice,GenOutgoing,GenPlayful" + // GeneticCharacter
                    ",Active,Neat,Nice,Outgoing,Playful" + // Character
                    ",Animals,Crime,Culture,Entertainment,Environment,Fashion,FemalePreference,Food,Health" + //Interests
                    ",MalePreference,Money,Paranormal,Politics,School,Scifi,Sports,Toys,Travel,Weather,Work" + //Interests
                    ",Body,Charisma,Cleaning,Cooking,Creativity,Fatness,Logic,Mechanical,Romance" + //Skills

                    ",IsAtUniversity,UniEffort,UniGrade,UniTime,UniSemester,UniInfluence,UniMajor" + // University
                    ",Species" + // Nightlife
                    ",Salary" + // Business
                    ",PrimaryAspiration,SecondaryAspiration,HobbyPredestined,LifetimeWant" + // FreeTime
                    //",Reputation" + // Aparments... not found it yet
                    ""
                    );
                #endregion

                #region ExportedLots header
                w2.WriteLine("hood" +
                    ",HoodName" +
                    ",LotInstance" +
                    ",HouseNumber,HouseName" +
                    ""
                    );
                #endregion

                ExpansionItem.NeighborhoodPaths paths = PathProvider.Global.GetNeighborhoodsForGroup(group);
                foreach (ExpansionItem.NeighborhoodPath path in paths)
                {
                    string sourcepath = path.Path;
                    string[] dirs = System.IO.Directory.GetDirectories(sourcepath, hood.Length > 0 ? hood : "????");
                    foreach (string dir in dirs)
                        AddHood(outPath, dir, w1, w2);
                }
            }
            finally
            {
                w1.Close();
                w2.Close();
            }
        }

        ExtFamilyTies eft = null;
        void SetProvider(SimPe.Interfaces.Files.IPackageFile pkg)
        {
            FileTable.ProviderRegistry.SimFamilynameProvider.BasePackage = pkg;
            FileTable.ProviderRegistry.SimDescriptionProvider.BasePackage = pkg;
            FileTable.ProviderRegistry.SimNameProvider.BaseFolder = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(pkg.FileName), "Characters");
            FileTable.ProviderRegistry.LotProvider.BaseFolder = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(pkg.FileName), "Lots");
            eft = new ExtFamilyTies();
            IPackedFileDescriptor[] pfds = pkg.FindFiles(SimPe.Data.MetaData.FAMILY_TIES_FILE);
            if (pfds != null && pfds.Length > 0)
                eft.ProcessData(pfds[0], pkg);
        }

        DateTime dt = new DateTime(0);
        bool wasUnk = true;
        void AddHood(string outPath, string dir, StreamWriter w1, StreamWriter w2)
        {
            string hood = Path.GetFileName(dir);
            string hoodFile = Path.Combine(dir, hood + "_Neighborhood.package");
            if (!File.Exists(hoodFile)) return;

            SimPe.Packages.File pkg = SimPe.Packages.File.LoadFromFile(hoodFile);
            if (pkg == null) return;

            string hoodName = Localization.GetString("Unknown");
            IPackedFileDescriptor[] pfds = pkg.FindFiles(SimPe.Data.MetaData.CTSS_FILE);
            StrWrapper ctss = null;
            if (pfds.Length == 1)
            {
                ctss = new StrWrapper();
                ctss.ProcessData(pfds[0], pkg);
                hoodName = ctss[1, 0];
            }


            if (!Directory.Exists(Path.Combine(outPath, "SimImage")))
                Directory.CreateDirectory(Path.Combine(outPath, "SimImage"));

            System.Windows.Forms.Application.DoEvents();
            splash("Loading Neighborhood " + hood + ": " + hoodName);
            SetProvider(pkg);

            dt = new DateTime(0);
            wasUnk = true;
            pfds = pkg.FindFiles(SimPe.Data.MetaData.SIM_DESCRIPTION_FILE);
            foreach (IPackedFileDescriptor spfd in pfds)
            {
                ExtSDesc sdsc = new ExtSDesc();
                sdsc.ProcessData(spfd, pkg);

                AddSim(outPath, hood, hoodName, w1, sdsc);
            }


            if (!Directory.Exists(Path.Combine(outPath, "LotImage")))
                Directory.CreateDirectory(Path.Combine(outPath, "LotImage"));

            System.Windows.Forms.Application.DoEvents();
            splash("Loading Neighborhood " + hood + ": " + hoodName);
            SetProvider(pkg);

            dt = new DateTime(0);
            wasUnk = true;
            pfds = pkg.FindFiles(SimPe.Plugin.Ltxt.Ltxttype);
            foreach (IPackedFileDescriptor spfd in pfds)
            {
                Ltxt ltxt = new Ltxt();
                ltxt.ProcessData(spfd, pkg);

                AddLot(outPath, hood, hoodName, w2, ltxt);
            }
        }

        enum bodyType : ushort { Unknown = 0, Fat = 1, PregnantFull = 2, PregnantHalf = 4, PregnantHidden = 8, };
        void AddSim(string outPath, string hood, string hoodName, StreamWriter w, ExtSDesc sdsc)
        {
            #region desc
            string desc = ",,";
            SimPe.Interfaces.Files.IPackageFile pkg = SimPe.Packages.File.LoadFromFile(sdsc.CharacterFileName);
            if (pkg == null)
            {
                System.Diagnostics.Trace.WriteLine("Could not find character package:\n" + sdsc.CharacterFileName);
                return;// severe problem...
            }
            IPackedFileDescriptor[] pfds = pkg.FindFiles(StrWrapper.CTSStype);
            if (pfds == null || pfds.Length == 0)
            {
                System.Diagnostics.Trace.WriteLine("Could not find CTSS packed file for sim");
                return;// severe problem...
            }
            try
            {
                StrWrapper ctss = new StrWrapper();
                ctss.ProcessData(pfds[0], pkg);
                desc = q(ctss[1, 0]) + // firstname
                    "," + q(ctss[1, 2]) + // lastname
                    "," + q(ctss[1, 1]) + // description
                    ""
                    ;
            }
            catch { }
            #endregion

            #region family
            string family = ",,";
            IPackedFileDescriptor pfd = sdsc.Package.FindFile(0x46414D49, 0x00000000, 0xffffffff, sdsc.FamilyInstance); // FAMI
            if (pfd != null)
            {
                Fami fami = null;
                fami = new Fami(FileTable.ProviderRegistry.SimNameProvider);
                fami.ProcessData(pfd, sdsc.Package);

                family = sdsc.FamilyInstance +
                    "," + q(sdsc.HouseholdName) +
                    "," + fami.LotInstance +
                    ""
                    ;
            }
            #endregion

            #region ties
            string ties = ",,";
            if (eft != null)
            {
                SDesc[] p = eft.ParentSims(sdsc);
                SDesc[] s = eft.SpouseSims(sdsc);
                ties = (p == null || p.Length < 2 ? "," : p[0].Instance + "," + p[1].Instance) +
                    "," + (s == null || s.Length < 1 ? "" : s[0].Instance + "") +
                    ""
                    ;
            }
            #endregion

            #region ghost
            /*string ghost = "N(,,,)";
            if (sdsc.CharacterDescription.GhostFlag.IsGhost)
            {
                ghost = "Y(" + (sdsc.CharacterDescription.GhostFlag.CanPassThroughObjects ? "Y" : "N") +
                    (sdsc.CharacterDescription.GhostFlag.CanPassThroughWalls ? "Y" : "N") +
                    (sdsc.CharacterDescription.GhostFlag.CanPassThroughPeople ? "Y" : "N") +
                    (sdsc.CharacterDescription.GhostFlag.IgnoreTraversalCosts ? "Y" : "N") +
                    ")";
            }*/
            #endregion

            #region genetics
            string genetics = sdsc.GeneticCharacter.Active +
                "," + sdsc.GeneticCharacter.Neat +
                "," + sdsc.GeneticCharacter.Nice +
                "," + sdsc.GeneticCharacter.Outgoing +
                "," + sdsc.GeneticCharacter.Playful +
                ""
            ;
            #endregion

            #region character
            string character = sdsc.Character.Active +
                "," + sdsc.Character.Neat +
                "," + sdsc.Character.Nice +
                "," + sdsc.Character.Outgoing +
                "," + sdsc.Character.Playful +
                ""
            ;
            #endregion

            #region interests
            string interests = sdsc.Interests.Animals +
                "," + sdsc.Interests.Crime +
                "," + sdsc.Interests.Culture +
                "," + sdsc.Interests.Entertainment +
                "," + sdsc.Interests.Environment +
                "," + sdsc.Interests.Fashion +
                "," + sdsc.Interests.FemalePreference +
                "," + sdsc.Interests.Food +
                "," + sdsc.Interests.Health +
                "," + sdsc.Interests.MalePreference +
                "," + sdsc.Interests.Money +
                "," + sdsc.Interests.Paranormal +
                "," + sdsc.Interests.Politics +
                "," + sdsc.Interests.School +
                "," + sdsc.Interests.Scifi +
                "," + sdsc.Interests.Sports +
                "," + sdsc.Interests.Toys +
                "," + sdsc.Interests.Travel +
                "," + sdsc.Interests.Weather +
                "," + sdsc.Interests.Work +
                ""
            ;
            #endregion

            #region skills
            string skills = sdsc.Skills.Body +
                "," + sdsc.Skills.Charisma +
                "," + sdsc.Skills.Cleaning +
                "," + sdsc.Skills.Cooking +
                "," + sdsc.Skills.Creativity +
                "," + sdsc.Skills.Fatness +
                "," + sdsc.Skills.Logic +
                "," + sdsc.Skills.Mechanical +
                "," + sdsc.Skills.Romance +
                ""
            ;
            #endregion

            #region university
            string university = "N,,,,,,";
            if (sdsc.University != null && sdsc.University.OnCampus == 0x1)
            {
                university = "Y" +
                "," + sdsc.University.Effort +
                "," + sdsc.University.Grade +
                "," + sdsc.University.Time +
                "," + sdsc.University.Semester +
                "," + sdsc.University.Influence +
                "," + sdsc.University.Major
                ;
            }
            #endregion

            #region freetime
            string freetime = ",,,";
            if (sdsc.Freetime != null)
            {
                freetime = sdsc.Freetime.PrimaryAspiration +
                    "," + sdsc.Freetime.SecondaryAspiration +
                    "," + sdsc.Freetime.HobbyPredistined +
                    "," + sdsc.Freetime.LongtermAspiration // LifetimeWant ?
                ;
                //sdsc.Freetime.BugCollection -- no...
            }
            #endregion

            //sdsc.Business.LotID


            if (dt.Equals(new DateTime(0)) || wasUnk || dt.AddMilliseconds(200).CompareTo(DateTime.UtcNow) < 0)
            {
                System.Windows.Forms.Application.DoEvents();
                if (!((string)(sdsc.SimName + " " + sdsc.SimFamilyName)).Trim().ToLower().Equals("unknown"))
                {
                    dt = new DateTime(DateTime.UtcNow.Ticks);
                    wasUnk = false;
                    splash("Saving " + sdsc.SimName + " " + sdsc.SimFamilyName);
                }
                else
                    wasUnk = true;
            }
            string csv = hood +
                "," + q(hoodName) +
                "," + sdsc.Instance +
                "," + desc +
                "," + family +
                "," + (sdsc.AvailableCharacterData ? "Y" : "N") +
                "," + (sdsc.Unlinked != 0x00 ? "Y" : "N") +
                "," + ties +
                //"," + ghost +
                "," + (bodyType)(ushort)sdsc.CharacterDescription.BodyFlag +
                //"," + sdsc.CharacterDescription.AutonomyLevel +
                "," + sdsc.CharacterDescription.NPCType +
                //"," + sdsc.CharacterDescription.MotivesStatic +
                //"," + sdsc.CharacterDescription.VoiceType +
                "," + sdsc.CharacterDescription.SchoolType +
                "," + sdsc.CharacterDescription.Grade +
                "," + sdsc.CharacterDescription.CareerPerformance +
                "," + sdsc.CharacterDescription.Career +
                "," + sdsc.CharacterDescription.CareerLevel +
                "," + sdsc.CharacterDescription.ZodiacSign +
                "," + sdsc.CharacterDescription.Aspiration +
                "," + sdsc.CharacterDescription.Gender +
                "," + sdsc.CharacterDescription.LifeSection +
                "," + sdsc.CharacterDescription.Age +
                "," + sdsc.CharacterDescription.PrevAgeDays +
                "," + sdsc.CharacterDescription.AgeDuration +
                "," + sdsc.CharacterDescription.BlizLifelinePoints +
                "," + sdsc.CharacterDescription.LifelinePoints +
                "," + sdsc.CharacterDescription.LifelineScore +
                "," + genetics +
                "," + character +
                "," + interests +
                "," + skills +
                "," + university +
                "," + (sdsc.Nightlife == null ? "Human" : sdsc.Nightlife.Species.ToString()) +
                "," + (sdsc.Business == null ? (ushort)0 : sdsc.Business.Salary) +
                "," + freetime +
                //";Reputation" +
                ""
            ;
            w.WriteLine(csv);

            AddImage(sdsc.Image, Path.Combine(Path.Combine(outPath, "SimImage"), hood + "_" + sdsc.Instance + ".png"));
        }

        void AddLot(string outPath, string hood, string hoodName, StreamWriter w, Ltxt ltxt)
        {
            if (dt.Equals(new DateTime(0)) || wasUnk || dt.AddMilliseconds(200).CompareTo(DateTime.UtcNow) < 0)
            {
                dt = new DateTime(DateTime.UtcNow.Ticks);
                wasUnk = false;
                splash("Saving " + ltxt.LotName.Trim());
            }
            w.WriteLine(hood +
                "," + q(hoodName) +
                "," + ltxt.FileDescriptor.Instance +
                "," + (ltxt.LotDescription == null ? "," : ltxt.LotDescription.Instance + "," + q(ltxt.LotDescription.LotName)) +
                ""
                );

            if (ltxt.LotDescription != null)
                AddImage(ltxt.LotDescription.Image, Path.Combine(Path.Combine(outPath, "LotImage"), hood + "_" + ltxt.FileDescriptor.Instance + ".png"));
        }


        void AddImage(Image img, string f)
        {
            if (img != null)
            {
                if (img.Size.Width > 16 && img.Size.Height > 16)
                    img.Save(f);
                else
                    System.Diagnostics.Trace.WriteLine("img too small: " + Path.GetFileNameWithoutExtension(f) + ";w=" + img.Width + ";h=" + img.Height);
            }
        }


        #region ITool Members

        SimPe.Interfaces.Plugin.IToolResult ITool.ShowDialog(ref SimPe.Interfaces.Files.IPackedFileDescriptor pfd, ref SimPe.Interfaces.Files.IPackageFile package)
        {
            if (!System.IO.Directory.Exists(PathProvider.Global.NeighborhoodFolder))
            {
                System.Windows.Forms.MessageBox.Show("The Folder " + PathProvider.Global.NeighborhoodFolder + " was not found.\n" +
                    "Please specify the correct SaveGame Folder in the Options Dialog.");
                return new ToolResult(false, false);
            }

            System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog();
            fbd.Description = L.Get("ChooseFolder");
            fbd.SelectedPath = SimPe.PathProvider.SimSavegameFolder;
            fbd.ShowNewFolderButton = true;
            System.Windows.Forms.DialogResult dr = fbd.ShowDialog();
            if (dr != System.Windows.Forms.DialogResult.OK) return new ToolResult(false, false);


            NeighborhoodForm nfm = new NeighborhoodForm();
            nfm.LoadNgbh = false;
            nfm.ShowBackupManager = false;
            nfm.Text = L.Get("nfmTitle");
            SimPe.Interfaces.Plugin.IToolResult ret = nfm.Execute(ref package, null);

            string hood = "";
            if (nfm.DialogResult == System.Windows.Forms.DialogResult.OK && nfm.SelectedNgbh != null)
                hood = Path.GetFileName(Path.GetDirectoryName(nfm.SelectedNgbh));

            try
            {
                SimPe.WaitingScreen.Wait();
                splash = delegate(string message) { SimPe.WaitingScreen.UpdateMessage(message); };
                Rufio(fbd.SelectedPath, hood, 0);
                return new SimPe.Plugin.ToolResult(false, false);
            }
            finally
            {
                SimPe.WaitingScreen.Stop();
            }
        }

        bool ITool.IsEnabled(SimPe.Interfaces.Files.IPackedFileDescriptor pfd, SimPe.Interfaces.Files.IPackageFile package)
        {
            return true;
        }

        #endregion

        #region IToolPlugin Members

        string IToolPlugin.ToString()
        {
            return "Neighborhood\\" + L.Get("pjCHoodTool");
        }

        #endregion

        #region ICommandLine Members

        public bool Parse(List<string> argv)
        {
            int i = ArgParser.Parse(argv, "-rufio");
            if (i < 0) return false;

            string outpath = "";
            string hood = "";
            string group = "";
            int groupno = 0;
            while (argv.Count > i)
            {
                if (ArgParser.Parse(argv, i, "-out", ref outpath)) continue;
                if (ArgParser.Parse(argv, i, "-hood", ref hood)) continue;
                if (ArgParser.Parse(argv, i, "-group", ref group)) continue;
                SimPe.Message.Show(Help()[0]);
                return true;
            }

            if (outpath.Length > 0 && !Directory.Exists(outpath))
            {
                SimPe.Message.Show("Use -out specify an existing folder");
                return true;
            }

            if (!Directory.Exists(PathProvider.Global.NeighborhoodFolder))
            {
                SimPe.Message.Show("The Folder " + PathProvider.Global.NeighborhoodFolder + " was not found.\r\n" +
                    "Please specify the correct SaveGame Folder in the Options Dialog.");
                return false;
            }

            if (group.Length > 0)
            {
                try { groupno = Convert.ToInt32(group); if (groupno < 1 || (groupno & PathProvider.Global.AvailableGroups) == 0) throw new FormatException(); }
                catch (FormatException)
                {
                    SimPe.Message.Show("Invalid group.  Please specify a group from expansions.xreg.");
                    return false;
                }
            }

            splash = delegate(string message) { SimPe.Splash.Screen.SetMessage(message); };
            Rufio(outpath, hood, groupno);
            splash("");
            return true;
        }

        public string[] Help()
        {
            return new string[] { "-rufio -out {outpath} {-hood hood} {-group group}", L.Get("pjCHoodHelp") };
        }

        #endregion
    }
}

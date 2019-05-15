/***************************************************************************
 *   Copyright (C) 2006 by Peter L Jones                                   *
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
using System.Windows.Forms;

namespace pjse.Updates
{
    public class Checker
    {
        static Checker()
        {
            // Should only be set to AskMe the first time through (although it might have been reset by the user)
            if (Settings.US.AutoUpdateChoice == Settings.AutoUpdateChoiceValue.AskMe)
            {
                DialogResult dr = MessageBox.Show(
                    pjse.Localization.GetString("UCAskMe")
                    , pjse.Localization.GetString("pjse_UpdateSettings")
                    , MessageBoxButtons.YesNo, MessageBoxIcon.Question
                );
                if (dr.Equals(DialogResult.Yes))
                    Settings.US.AutoUpdateChoice = Settings.AutoUpdateChoiceValue.Daily;
                else
                {
                    Settings.US.AutoUpdateChoice = Settings.AutoUpdateChoiceValue.Manual;
                    MessageBox.Show(pjse.Localization.GetString("UCSaidNo")
                        , pjse.Localization.GetString("pjse_UpdateSettings")
                    );
                }
            }
        }

        public static void Daily()
        {
            if ((Settings.US.AutoUpdateChoice == Settings.AutoUpdateChoiceValue.Daily)
                && (DateTime.UtcNow.Date != Settings.US.LastUpdateTS.Date))
            {
                try
                {
                    GetUpdate(true);
                    Settings.US.LastUpdateTS = DateTime.UtcNow; // Only the automated check updates this setting
                }
                catch (ArgumentException) { }
            }
        }

        public static bool GetUpdate(bool autoCheck)
        {
            UpdateInfo ui = null;
            try { ui = new UpdateInfo(); }
            catch (System.Net.WebException we)
            {
                MessageBox.Show(
                    ((we != null && we.Response != null) ? "URL: " + we.Response.ResponseUri + "\r\n\r\n" : "")
                    + we.Message
                    + "\r\n\r\n" + pjse.Localization.GetString("UIWebException")
                    , pjse.Localization.GetString("pjse_UpdateSettings")
                    , MessageBoxButtons.OK
                    , MessageBoxIcon.Exclamation);
                if (autoCheck) Settings.US.AutoUpdateChoice = Settings.AutoUpdateChoiceValue.AskMe;
                throw new ArgumentException();
            }

            if (UpdateApplicable(ui, autoCheck))
            {
                SkipPrompt sp = new SkipPrompt(autoCheck, ui.AvailableVersion, ui.UpdateURL);
                switch (sp.ShowDialog())
                {
                    case DialogResult.Yes: SimPe.RemoteControl.ShowHelp(ui.UpdateURL); break;
                    case DialogResult.Cancel: Settings.US.LastIgnoredTS = ui.AvailableVersion; break;
                }
                return true;
            }
            return false;
        }

        private static bool UpdateApplicable(UpdateInfo ui, bool autoCheck)
        {
            String ts = pjse.Version.BuildTS;
            SimPe.PathProvider simpe = SimPe.PathProvider.Global;
            System.Diagnostics.FileVersionInfo simpeVersion = System.Diagnostics.FileVersionInfo.GetVersionInfo(simpe.GetType().Assembly.Location);

#if DEBUG
            MessageBox.Show(
                "Update URL: " + Settings.US.AutoUpdateURL
                + "\r\n" + pjse.Localization.GetString("helpPJSEAboutPJSEVersion") + ": " + ts
                + "\r\n" + "offered PJSE version: " + ui.AvailableVersion
                + "\r\n" + "last ignored PJSE version: " + Settings.US.LastIgnoredTS
                + "\r\n"
                + "\r\n" + pjse.Localization.GetString("helpPJSEAboutSimPEVersion") + ": " + simpeVersion.FileVersion
                + "\r\n" + "Required Min SimPE version: " + ui.MinSimPEVersion
                + "\r\n" + "Required Max SimPE version: " + ui.MaxSimPEVersion
                , pjse.Localization.GetString("pjse_UpdateSettings")
                );
#endif

            if (autoCheck && ui.AvailableVersion.CompareTo(Settings.US.LastIgnoredTS) <= 0)
                return false;

            if (ui.AvailableVersion.CompareTo(ts) <= 0)
                return false;

            ulong runningSV = toVersion(simpeVersion.FileVersion, false);
            ulong offeredMinSV = toVersion(ui.MinSimPEVersion, false);
            ulong offeredMaxSV = toVersion(ui.MaxSimPEVersion, true);

            if (offeredMinSV > runningSV || offeredMaxSV < runningSV)
                return false;

            return true;
        }

        private static ulong toVersion(String v, bool hilo)
        {
            String[] s = v.Split('.');
            for (int i = 0; i < s.Length; i++) if (s[i].Equals("*")) s[i] = hilo ? UInt16.MaxValue.ToString() : "0";
            ushort[] l = new ushort[4] { Convert.ToUInt16(s[0]), Convert.ToUInt16(s[1]), Convert.ToUInt16(s[2]), Convert.ToUInt16(s[3]) };
            return (ulong)l[3] + ((ulong)l[2] * 0x10000) + ((ulong)l[1] * 0x100000000) + ((ulong)l[0] * 0x1000000000000);
        }
    }
}

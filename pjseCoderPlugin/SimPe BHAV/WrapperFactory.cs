/***************************************************************************
 *   Copyright (C) 2005 by Peter L Jones                                   *
 *   pljones@users.sf.net                                                  *
 *   Copyright (C) 2005 by Ambertation                                     *
 *   quaxi@ambertation.de                                                  *
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
using SimPe.Interfaces;
using SimPe.Interfaces.Plugin;
using SimPe.PackedFiles.UserInterface;
using SimPe.PackedFiles.Wrapper;

namespace SimPe.Plugin
{
	/// <summary>
	/// Lists all Plugins (=FileType Wrappers) available in this Package
	/// </summary>
	/// <remarks>
	/// GetWrappers() has to return a list of all Plugins provided by this Library.
	/// If a Plugin isn't returned, SimPe won't recognize it!
	/// </remarks>
    public class WrapperFactory : AbstractWrapperFactory, IHelpFactory, ISettingsFactory, SimPe.Updates.IUpdatablePlugin
	{
		/// <summary>
		/// Returns a List of all available Plugins in this Package
		/// </summary>
		/// <returns>A List of all provided Plugins (=FileType Wrappers)</returns>
		public override SimPe.Interfaces.IWrapper[] KnownWrappers
		{
			get
			{
				return new IWrapper[] {
										   new Bcon()
										  ,new Bhav()
										  ,new Objf()
										  ,new StrWrapper()
										  ,new TPRP()
										  ,new Trcn()
										  ,new Ttab()
									  };
			}
		}

		#region IHelpFactory Members

        class helpContents : IHelp
        {
            #region IHelp Members
            public System.Drawing.Image Icon { get { return null; } }
#if PJSE_STANDALONE
            public override string ToString() { return "PJSE\\" + pjse.Localization.GetString("helpPJSEContents"); }
#else
            public override string ToString() { return pjse.Localization.GetString("helpPJSE"); }
#endif
            public void ShowHelp(ShowHelpEventArgs e) { pjse.HelpHelper.Help("Contents"); }
            #endregion
        }

#if PJSE_STANDALONE
        class helpUpdates : IHelp
        {
            #region IHelp Members
            public System.Drawing.Image Icon { get { return null; } }
            public override string ToString() { return "PJSE\\" + pjse.Localization.GetString("helpPJSEUpdate"); }
            public void ShowHelp(SimPe.ShowHelpEventArgs e)
            {
                try
                {
                    if (!pjse.Updates.Checker.GetUpdate(false))
                        System.Windows.Forms.MessageBox.Show(
                            pjse.Localization.GetString("UHNoUpdate")
                            , pjse.Localization.GetString("pjse_UpdateSettings")
                        );
                }
                catch (ArgumentException) { }
            }
            #endregion
        }

        class helpAbout : IHelp
        {
            #region IHelp Members
            public System.Drawing.Image Icon { get { return null; } }
            public override string ToString() { return "PJSE\\" + pjse.Localization.GetString("helpPJSEAbout"); }
            public void ShowHelp(ShowHelpEventArgs e)
            {
                System.Diagnostics.FileVersionInfo simpeVersion
                    = System.Diagnostics.FileVersionInfo.GetVersionInfo(SimPe.PathProvider.Global.GetType().Assembly.Location);

                System.Windows.Forms.MessageBox.Show(
                    pjse.Localization.GetString("helpPJSEAboutPJSEVersion") + ": " + pjse.Version.BuildTS
                    + "\r\n" + pjse.Localization.GetString("helpPJSEAboutSimPEBuild") + ": " + pjse.Version.SimPeVersion
                    + "\r\n" + pjse.Localization.GetString("helpPJSEAboutSimPEVersion") + ": " + simpeVersion.FileVersion
                    + "\r\n"
                    + "\r\n" + pjse.Localization.GetString("helpPJSEAboutPJSEUC") + ": " + pjse.Updates.Settings.US.AutoUpdateChoice.ToString()
                    + "\r\n" + pjse.Localization.GetString("helpPJSEAboutPJSEUU") + ": " + pjse.Updates.Settings.US.AutoUpdateURL.ToString()
                    + "\r\n" + pjse.Localization.GetString("helpPJSEAboutPJSELU") + ": " + pjse.Updates.Settings.US.LastUpdateTS.ToString()
                    , pjse.Localization.GetString("helpPJSEAboutCaption")
                );
            }
            #endregion
        }
#endif
        public IHelp[] KnownHelpTopics
        {
            get
            {
                IHelp[] helpTopics = {
					new helpContents()
#if PJSE_STANDALONE
                  , new helpUpdates()
                  , new helpAbout()
#endif
				};
                return helpTopics;
            }
        }


        #endregion

        #region ISettingsFactory Members

        public ISettings[] KnownSettings
        {
            get { return new ISettings[] { pjse.Settings.PJSE, pjse.Updates.Settings.US, }; }
        }

        #endregion

        #region IUpdatablePlugin Members
        public SimPe.Updates.UpdateInfo GetUpdateInformation()
        {
            System.Diagnostics.FileVersionInfo v = System.Diagnostics.FileVersionInfo.GetVersionInfo(this.GetType().Assembly.Location);
            long build = ((long)v.FilePrivatePart & 0xFFFF)
                + (((long)v.FileBuildPart & 0xFFFF) << 16)
                + (((long)v.FileMinorPart & 0xFFFF) << 32)
                + (((long)v.FileMajorPart & 0xFFFF) << 48);
            return new SimPe.Updates.UpdateInfo(build, v.ProductName, v.FileDescription);
        }
        #endregion
    }
}

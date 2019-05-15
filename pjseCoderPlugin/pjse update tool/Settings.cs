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
using System.Resources;


namespace pjse.Updates
{
    public class Settings : SimPe.GlobalizedObject, SimPe.Interfaces.ISettings
    {
        static ResourceManager rm = new ResourceManager(typeof(pjse.Localization));

        private static Settings settings;
        public static Settings US { get { return settings; } }
        static Settings() { settings = new Settings(); }

        const string BASENAME = "PJSE\\UpdateTool";
        SimPe.XmlRegistryKey xrk = SimPe.Helper.WindowsRegistry.PluginRegistryKey;
        public Settings() : base(rm) { }

        [System.ComponentModel.Category("UpdateTool")]
        public String AutoUpdateURL
        {
            get
            {
                SimPe.XmlRegistryKey rkf = SimPe.Helper.WindowsRegistry.PluginRegistryKey.CreateSubKey(BASENAME);
                object o = rkf.GetValue("autoUpdateURL", "http://www.simlogical.com/PJSE/pjseUpdate.xml");
                return Convert.ToString(o);
            }

            set
            {
                SimPe.XmlRegistryKey rkf = SimPe.Helper.WindowsRegistry.PluginRegistryKey.CreateSubKey(BASENAME);
                rkf.SetValue("autoUpdateURL", value);
            }
        }

#if !DEBUG
        [System.ComponentModel.Browsable(false)]
#endif
        [System.ComponentModel.Category("UpdateTool")]
        public DateTime LastUpdateTS
        {
            get
            {
                SimPe.XmlRegistryKey rkf = SimPe.Helper.WindowsRegistry.PluginRegistryKey.CreateSubKey(BASENAME);
                object o = rkf.GetValue("lastUpdateTS", new DateTime(0));
                return Convert.ToDateTime(o);
            }

            set
            {
                SimPe.XmlRegistryKey rkf = SimPe.Helper.WindowsRegistry.PluginRegistryKey.CreateSubKey(BASENAME);
                rkf.SetValue("lastUpdateTS", value);
            }
        }

        [System.ComponentModel.Category("UpdateTool")]
        public AutoUpdateChoiceValue AutoUpdateChoice
        {
            get
            {
                SimPe.XmlRegistryKey rkf = SimPe.Helper.WindowsRegistry.PluginRegistryKey.CreateSubKey(BASENAME);
                object o = rkf.GetValue("autoUpdateChoice", AutoUpdateChoiceValue.Manual);
                switch (Convert.ToInt32(o))
                {
                    case 1: return AutoUpdateChoiceValue.Daily;
                    case 2: return AutoUpdateChoiceValue.Manual;
                    default: return AutoUpdateChoiceValue.AskMe;
                }
            }

            set
            {
                SimPe.XmlRegistryKey rkf = SimPe.Helper.WindowsRegistry.PluginRegistryKey.CreateSubKey(BASENAME);
                try { rkf.SetValue("autoUpdateChoice", (Int32)value); }
                catch { }
            }
        }
        public enum AutoUpdateChoiceValue : int { AskMe = 0, Daily = 1, Manual = 2 };


#if !DEBUG
        [System.ComponentModel.Browsable(false)]
#endif
        [System.ComponentModel.Category("UpdateTool")]
        public String LastIgnoredTS
        {
            get
            {
                SimPe.XmlRegistryKey rkf = SimPe.Helper.WindowsRegistry.PluginRegistryKey.CreateSubKey(BASENAME);
                object o = rkf.GetValue("lastIgnoredTS", "");
                return Convert.ToString(o);
            }

            set
            {
                SimPe.XmlRegistryKey rkf = SimPe.Helper.WindowsRegistry.PluginRegistryKey.CreateSubKey(BASENAME);
                rkf.SetValue("lastIgnoredTS", value);
            }
        }

        #region ISettings Members

        public object GetSettingsObject() { return this; }

        public override string ToString() { return pjse.Localization.GetString("pjse_UpdateSettings"); }

        [System.ComponentModel.Browsable(false)]
        public System.Drawing.Image Icon { get { return null; } }

        #endregion
    }
}

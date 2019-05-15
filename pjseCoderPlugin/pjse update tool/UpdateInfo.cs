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
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml;

namespace pjse.Updates
{
    public class UpdateInfo
    {
        private String pjseVersion = null;
        private String minSimPEVersion = null;
        private String maxSimPEVersion = null;
        private String updateURL = null;

        public UpdateInfo() : this(Settings.US.AutoUpdateURL) { }
        public UpdateInfo(String url)
        {
            XmlReader xr = XmlReader.Create(url);

            xr.MoveToContent();
            if (!xr.Name.Equals("pjseUpdate"))
                xr.Skip();

            Hashtable bucket = new Hashtable();
            while (xr.Read())
            {
                if (xr.MoveToContent() == XmlNodeType.Element)
                    bucket[xr.Name] = xr.ReadString();
            }
            xr.Close();
            xr = null;

            pjseVersion = (String)bucket["pjseVersion"];
            minSimPEVersion = (String)bucket["minSimPEVersion"];
            maxSimPEVersion = (String)bucket["maxSimPEVersion"];
            updateURL = (String)bucket["updateURL"];

            if (pjseVersion == null || minSimPEVersion == null || maxSimPEVersion == null || updateURL == null)
            {
                MessageBox.Show("URL: " + url + "\r\n\r\n" + pjse.Localization.GetString("UIMalformedContent")
                    , pjse.Localization.GetString("pjse_UpdateSettings")
                    , MessageBoxButtons.OK
                    , MessageBoxIcon.Exclamation);
                throw new ArgumentException();
            }
            pjseVersion = pjseVersion.Trim().Replace(' ', '0');
        }

        public String AvailableVersion { get { return pjseVersion; } }
        public String MinSimPEVersion { get { return minSimPEVersion; } }
        public String MaxSimPEVersion { get { return maxSimPEVersion; } }
        public String UpdateURL { get { return updateURL; } }
    }
}

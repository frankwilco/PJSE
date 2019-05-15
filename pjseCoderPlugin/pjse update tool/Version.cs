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
using System.Text;
using System.IO;

namespace pjse
{
    public class Version
    {
        static String pluginName;
        static String configuration;
        static String timestamp;
        static String simperelease;
        static Version()
        {
            String version_txt = Path.Combine(SimPe.Helper.SimPePluginPath, "pjse.coder.plugin\\version.txt");
            System.IO.StreamReader sr = new StreamReader(version_txt);
            String line1 = sr.ReadLine();
            String line2 = sr.ReadLine();
            String line3 = sr.ReadLine();
            sr.Close();
            sr.Dispose();
            sr = null;

            String[] s = line1.Trim().Split('-');
            pluginName = s[0];
            configuration = s[1];

            timestamp = line2.Trim().Replace(' ', '0');

            long srv = Convert.ToInt64(line3.Split('"')[1]);
            simperelease = (srv >> 48).ToString()
                + "." + ((srv >> 32) & 0xFFFF).ToString()
                + "." + ((srv >> 16) & 0xFFFF).ToString()
                + "." + (srv & 0xFFFF).ToString()
                ;
        }

        public static String PluginName { get { return pluginName; } }
        public static String Configuration { get { return configuration; } }
        public static String BuildTS { get { return timestamp; } }
        public static String SimPeVersion { get { return simperelease; } }
    }
}

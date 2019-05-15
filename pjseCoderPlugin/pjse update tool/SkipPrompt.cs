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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace pjse
{
    public partial class SkipPrompt : Form
    {
        public SkipPrompt(bool autoCheck, string release, string url)
        {
            InitializeComponent();

            if (!autoCheck)
            {
                this.tableLayoutPanel1.SuspendLayout();
                tableLayoutPanel1.Controls.Remove(btnIgnore);
                tableLayoutPanel1.SetColumnSpan(label1, 2);
                tableLayoutPanel1.SetColumnSpan(llURL, 2);
                tableLayoutPanel1.ColumnCount = 2;
                tableLayoutPanel1.ColumnStyles[0].SizeType =
                    tableLayoutPanel1.ColumnStyles[1].SizeType = SizeType.Percent;
                tableLayoutPanel1.ColumnStyles[0].Width =
                    tableLayoutPanel1.ColumnStyles[1].Width = 50;
                this.tableLayoutPanel1.ResumeLayout(false);
                this.tableLayoutPanel1.PerformLayout();
            }
            label1.Text = label1.Text.Replace("{0}", release);
            llURL.Text = url;
            this.Width = tableLayoutPanel1.Width + 6;
        }

        private void llURL_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.DialogResult = DialogResult.Yes;
        }
    }
}

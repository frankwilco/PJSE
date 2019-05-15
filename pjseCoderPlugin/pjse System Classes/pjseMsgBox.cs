/***************************************************************************
 *   Copyright (C) 2007 by Peter L Jones                                   *
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

namespace System.Windows.Forms
{
    public partial class pjseMsgBox : Form
    {
        // public pjseMsgBox() : this("", "", "001", null, null, null) { }

        private pjseMsgBox(string text, string caption,
            Boolset buttonsVisible, Boolset bsBtns, string[] sBtns, DialogResult[] adr)
        {
            InitializeComponent();

            this.tbMessage.Text = text;
            this.Text = caption;

            if (buttonsVisible.Length < 3)
                throw new ArgumentException("need three (or more) flags", "buttonsVisible");

            //button1.Visible = buttonsVisible[0];
            //button2.Visible = buttonsVisible[1];
            //button3.Visible = buttonsVisible[2];
            if (!buttonsVisible[0]) tlpButtons.Controls.Remove(button1);
            if (!buttonsVisible[1]) tlpButtons.Controls.Remove(button2);
            if (!buttonsVisible[2]) tlpButtons.Controls.Remove(button3);

            if (bsBtns != null && sBtns != null)
            {
                if (bsBtns.Length >= 1 && sBtns.Length >= 1 && bsBtns[0]) button1.Text = sBtns[0];
                if (bsBtns.Length >= 2 && sBtns.Length >= 2 && bsBtns[1]) button2.Text = sBtns[1];
                if (bsBtns.Length >= 3 && sBtns.Length >= 3 && bsBtns[2]) button3.Text = sBtns[2];
            }
            if (adr != null)
            {
                if (bsBtns.Length >= 1 && adr.Length >= 1 && bsBtns[0]) button1.DialogResult = adr[0];
                if (bsBtns.Length >= 2 && adr.Length >= 2 && bsBtns[1]) button2.DialogResult = adr[1];
                if (bsBtns.Length >= 3 && adr.Length >= 3 && bsBtns[2]) button3.DialogResult = adr[2];
            }
            else
            {
                button1.DialogResult = DialogResult.OK;
                button2.DialogResult = DialogResult.Cancel;
                button3.DialogResult = DialogResult.None;
            }

            int x = Convert.ToInt32((this.Width - tlpButtons.Width) / 2);
            tlpButtons.Left = x;

            this.AcceptButton = this.CancelButton = null;
            foreach (Button b in new Button[] { button1, button2, button3 })
            {
                if (b.DialogResult == DialogResult.OK) this.AcceptButton = b;
                if (b.DialogResult == DialogResult.Cancel) this.CancelButton = b;
            }
        }


        /// <summary>
        /// Displays a message box with the specified text.
        /// </summary>
        /// <param name="text">The text to display in the message box.</param>
        /// <returns>One of the System.Windows.Forms.DialogResult values.</returns>
        public static DialogResult Show(string text)
        { return (new pjseMsgBox(text, "", "001", null, null, null)).ShowDialog(); }

        /// <summary>
        /// Displays a message box in front of the specified object and with the specified text.
        /// </summary>
        /// <param name="owner">An implementation of System.Windows.Forms.IWin32Window that will own the modal dialog box.</param>
        /// <param name="text">The text to display in the message box.</param>
        /// <returns>One of the System.Windows.Forms.DialogResult values.</returns>
        public static DialogResult Show(IWin32Window owner, string text)
        { return (new pjseMsgBox(text, "", "001", null, null, null)).ShowDialog(owner); }


        /// <summary>
        /// Displays a message box with specified text and caption.
        /// </summary>
        /// <param name="text">The text to display in the message box.</param>
        /// <param name="caption">The text to display in the title bar of the message box.</param>
        /// <returns>One of the System.Windows.Forms.DialogResult values.</returns>
        public static DialogResult Show(string text, string caption)
        { return (new pjseMsgBox(text, caption, "001", null, null, null)).ShowDialog(); }

        /// <summary>
        /// Displays a message box in front of the specified object and with the specified text and caption.
        /// </summary>
        /// <param name="owner">An implementation of System.Windows.Forms.IWin32Window that will own the modal dialog box.</param>
        /// <param name="text">The text to display in the message box.</param>
        /// <param name="caption">The text to display in the title bar of the message box.</param>
        /// <returns>One of the System.Windows.Forms.DialogResult values.</returns>
        public static DialogResult Show(IWin32Window owner, string text, string caption)
        { return (new pjseMsgBox(text, caption, "001", null, null, null)).ShowDialog(owner); }


        /// <summary>
        /// Displays a message box with specified text, caption, and buttons.
        /// </summary>
        /// <param name="text">The text to display in the message box.</param>
        /// <param name="caption">The text to display in the title bar of the message box.</param>
        /// <param name="buttonsVisible">A Boolset of flags specifying which buttons should be visible.</param>
        /// <returns>One of the System.Windows.Forms.DialogResult values.</returns>
        /// <exception cref="System.ArgumentException">buttonsVisible must contain at least three flags</exception>
        public static DialogResult Show(string text, string caption, Boolset buttonsVisible)
        { return (new pjseMsgBox(text, caption, buttonsVisible, null, null, null)).ShowDialog(); }

        /// <summary>
        /// Displays a message box in front of the specified object and with specified text, caption, and buttons.
        /// </summary>
        /// <param name="owner">An implementation of System.Windows.Forms.IWin32Window that will own the modal dialog box.</param>
        /// <param name="text">The text to display in the message box.</param>
        /// <param name="caption">The text to display in the title bar of the message box.</param>
        /// <param name="buttonsVisible">A Boolset of flags specifying which buttons should be visible.</param>
        /// <returns>One of the System.Windows.Forms.DialogResult values.</returns>
        /// <exception cref="System.ArgumentException">buttonsVisible must contain at least three flags</exception>
        public static DialogResult Show(IWin32Window owner, string text, string caption, Boolset buttonsVisible)
        { return (new pjseMsgBox(text, caption, buttonsVisible, null, null, null)).ShowDialog(owner); }


        /// <summary>
        /// Displays a message box with specified text, caption, and buttons.
        /// </summary>
        /// <param name="text">The text to display in the message box.</param>
        /// <param name="caption">The text to display in the title bar of the message box.</param>
        /// <param name="buttonsVisible">A Boolset of flags specifying which buttons should be visible.</param>
        /// <param name="buttonsOverride">A Boolset of flags specifying which buttons should be overriden from buttons.</param>
        /// <param name="buttons">Text for button faces</param>
        /// <returns>One of the System.Windows.Forms.DialogResult values.</returns>
        /// <exception cref="System.ArgumentException">buttonsVisible must contain at least three flags</exception>
        public static DialogResult Show(string text, string caption, Boolset buttonsVisible, Boolset buttonsOverride, string[] buttons)
        { return (new pjseMsgBox(text, caption, buttonsVisible, buttonsOverride, buttons, null)).ShowDialog(); }

        /// <summary>
        /// Displays a message box in front of the specified object and with specified text, caption, and buttons.
        /// </summary>
        /// <param name="owner">An implementation of System.Windows.Forms.IWin32Window that will own the modal dialog box.</param>
        /// <param name="text">The text to display in the message box.</param>
        /// <param name="caption">The text to display in the title bar of the message box.</param>
        /// <param name="buttonsVisible">A Boolset of flags specifying which buttons should be visible.</param>
        /// <param name="buttonsOverride">A Boolset of flags specifying which buttons should be overriden from buttons.</param>
        /// <param name="buttons">Text for button faces</param>
        /// <returns>One of the System.Windows.Forms.DialogResult values.</returns>
        /// <exception cref="System.ArgumentException">buttonsVisible must contain at least three flags</exception>
        public static DialogResult Show(IWin32Window owner, string text, string caption, Boolset buttonsVisible, Boolset buttonsOverride, string[] buttons)
        { return (new pjseMsgBox(text, caption, buttonsVisible, buttonsOverride, buttons, null)).ShowDialog(owner); }


        /// <summary>
        /// Displays a message box with specified text, caption, and buttons.
        /// </summary>
        /// <param name="text">The text to display in the message box.</param>
        /// <param name="caption">The text to display in the title bar of the message box.</param>
        /// <param name="buttonsVisible">A Boolset of flags specifying which buttons should be visible.</param>
        /// <param name="buttonsOverride">A Boolset of flags specifying which buttons should be overriden from buttons.</param>
        /// <param name="buttons">Text for button faces</param>
        /// <param name="resultSet">DialogResult values for buttons</param>
        /// <returns>One of the System.Windows.Forms.DialogResult values.</returns>
        /// <exception cref="System.ArgumentException">buttonsVisible must contain at least three flags</exception>
        public static DialogResult Show(string text, string caption, Boolset buttonsVisible, Boolset buttonsOverride, string[] buttons, DialogResult[] resultSet)
        { return (new pjseMsgBox(text, caption, buttonsVisible, buttonsOverride, buttons, resultSet)).ShowDialog(); }

        /// <summary>
        /// Displays a message box in front of the specified object and with specified text, caption, and buttons.
        /// </summary>
        /// <param name="owner">An implementation of System.Windows.Forms.IWin32Window that will own the modal dialog box.</param>
        /// <param name="text">The text to display in the message box.</param>
        /// <param name="caption">The text to display in the title bar of the message box.</param>
        /// <param name="buttonsVisible">A Boolset of flags specifying which buttons should be visible.</param>
        /// <param name="buttonsOverride">A Boolset of flags specifying which buttons should be overriden from buttons.</param>
        /// <param name="buttons">Text for button faces</param>
        /// <param name="resultSet">DialogResult values for buttons</param>
        /// <returns>One of the System.Windows.Forms.DialogResult values.</returns>
        /// <exception cref="System.ArgumentException">buttonsVisible must contain at least three flags</exception>
        public static DialogResult Show(IWin32Window owner, string text, string caption, Boolset buttonsVisible, Boolset buttonsOverride, string[] buttons, DialogResult[] resultSet)
        { return (new pjseMsgBox(text, caption, buttonsVisible, buttonsOverride, buttons, resultSet)).ShowDialog(owner); }

#if UNDEF
        /// <summary>
        /// Displays a message box with specified text, caption, buttons, and icon.
        /// </summary>
        /// <param name="text">The text to display in the message box.</param>
        /// <param name="caption">The text to display in the title bar of the message box.</param>
        /// <param name="buttons">One of the System.Windows.Forms.MessageBoxButtons values that specifies which buttons to display in the message box.</param>
        /// <param name="icon">One of the System.Windows.Forms.MessageBoxIcon values that specifies which icon to display in the message box.</param>
        /// <returns>One of the System.Windows.Forms.DialogResult values.</returns>
        /// <exception cref="System.ComponentModel.InvalidEnumArgumentException">The buttons parameter specified is not a member of System.Windows.Forms.MessageBoxButtons.
        /// -or-The icon parameter specified is not a member of System.Windows.Forms.MessageBoxIcon.</exception>
        /// <exception cref="System.InvalidOperationException">An attempt was made to display the System.Windows.Forms.MessageBox in a process that is not running in User Interactive mode. This is specified by the System.Windows.Forms.SystemInformation.UserInteractive property.</exception>
        public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon) { }

        /// <summary>
        /// Displays a message box in front of the specified object and with specified text, caption, buttons, and icon.
        /// </summary>
        /// <param name="owner">An implementation of System.Windows.Forms.IWin32Window that will own the modal dialog box.</param>
        /// <param name="text">The text to display in the message box.</param>
        /// <param name="caption">The text to display in the title bar of the message box.</param>
        /// <param name="buttons">One of the System.Windows.Forms.MessageBoxButtons values that specifies which buttons to display in the message box.</param>
        /// <param name="icon">One of the System.Windows.Forms.MessageBoxIcon values that specifies which icon to display in the message box.</param>
        /// <returns>One of the System.Windows.Forms.DialogResult values.</returns>
        /// <exception cref="System.ComponentModel.InvalidEnumArgumentException">The buttons parameter specified is not a member of System.Windows.Forms.MessageBoxButtons.
        /// -or-The icon parameter specified is not a member of System.Windows.Forms.MessageBoxIcon.</exception>
        /// <exception cref="System.InvalidOperationException">An attempt was made to display the System.Windows.Forms.MessageBox in a process that is not running in User Interactive mode. This is specified by the System.Windows.Forms.SystemInformation.UserInteractive property.</exception>
        public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon) { }

        /// <summary>
        /// Displays a message box with the specified text, caption, buttons, icon, and default button.
        /// </summary>
        /// <param name="text">The text to display in the message box.</param>
        /// <param name="caption">The text to display in the title bar of the message box.</param>
        /// <param name="buttons">One of the System.Windows.Forms.MessageBoxButtons values that specifies which buttons to display in the message box.</param>
        /// <param name="icon">One of the System.Windows.Forms.MessageBoxIcon values that specifies which icon to display in the message box.</param>
        /// <param name="defaultButton">One of the System.Windows.Forms.MessageBoxDefaultButton values that specifies the default button for the message box.</param>
        /// <returns>One of the System.Windows.Forms.DialogResult values.</returns>
        /// <exception cref="System.ComponentModel.InvalidEnumArgumentException">The buttons parameter specified is not a member of System.Windows.Forms.MessageBoxButtons.
        /// -or- The icon parameter specified is not a member of System.Windows.Forms.MessageBoxIcon.
        /// -or- defaultButton is not a member of System.Windows.Forms.MessageBoxDefaultButton.</exception>
        /// <exception cref="System.InvalidOperationException">An attempt was made to display the System.Windows.Forms.MessageBox in a process that is not running in User Interactive mode. This is specified by the System.Windows.Forms.SystemInformation.UserInteractive property.</exception>
        public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton);

        /// <summary>
        /// Displays a message box in front of the specified object and with the specified text, caption, buttons, icon, and default button.
        /// </summary>
        /// <param name="owner">An implementation of System.Windows.Forms.IWin32Window that will own the modal dialog box.</param>
        /// <param name="text">The text to display in the message box.</param>
        /// <param name="caption">The text to display in the title bar of the message box.</param>
        /// <param name="buttons">One of the System.Windows.Forms.MessageBoxButtons values that specifies which buttons to display in the message box.</param>
        /// <param name="icon">One of the System.Windows.Forms.MessageBoxIcon values that specifies which icon to display in the message box.</param>
        /// <param name="defaultButton">One of the System.Windows.Forms.MessageBoxDefaultButton values that specifies the default button for the message box.</param>
        /// <returns>One of the System.Windows.Forms.DialogResult values.</returns>
        /// <exception cref="System.ComponentModel.InvalidEnumArgumentException">The buttons parameter specified is not a member of System.Windows.Forms.MessageBoxButtons.
        /// -or- The icon parameter specified is not a member of System.Windows.Forms.MessageBoxIcon.
        /// -or- defaultButton is not a member of System.Windows.Forms.MessageBoxDefaultButton.</exception>
        /// <exception cref="System.InvalidOperationException">An attempt was made to display the System.Windows.Forms.MessageBox in a process that is not running in User Interactive mode. This is specified by the System.Windows.Forms.SystemInformation.UserInteractive property.</exception>
        public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton) { }

        /// <summary>
        /// Displays a message box with the specified text, caption, buttons, icon, default button, and options.
        /// </summary>
        /// <param name="text">The text to display in the message box.</param>
        /// <param name="caption">The text to display in the title bar of the message box.</param>
        /// <param name="buttons">One of the System.Windows.Forms.MessageBoxButtons values that specifies which buttons to display in the message box.</param>
        /// <param name="icon">One of the System.Windows.Forms.MessageBoxIcon values that specifies which icon to display in the message box.</param>
        /// <param name="defaultButton">One of the System.Windows.Forms.MessageBoxDefaultButton values that specifies the default button for the message box.</param>
        /// <param name="options">One of the System.Windows.Forms.MessageBoxOptions values that specifies which display and association options will be used for the message box. You may pass in 0 if you wish to use the defaults.</param>
        /// <returns>One of the System.Windows.Forms.DialogResult values.</returns>
        /// <exception cref="System.ComponentModel.InvalidEnumArgumentException">The buttons parameter specified is not a member of System.Windows.Forms.MessageBoxButtons.
        /// -or- The icon parameter specified is not a member of System.Windows.Forms.MessageBoxIcon.
        /// -or- defaultButton is not a member of System.Windows.Forms.MessageBoxDefaultButton.</exception>
        /// <exception cref="System.InvalidOperationException">An attempt was made to display the System.Windows.Forms.MessageBox in a process that is not running in User Interactive mode. This is specified by the System.Windows.Forms.SystemInformation.UserInteractive property.</exception>
        /// <exception cref="System.ArgumentException">options specified both System.Windows.Forms.MessageBoxOptions.DefaultDesktopOnly and System.Windows.Forms.MessageBoxOptions.ServiceNotification.
        /// -or- buttons specified an invalid combination of System.Windows.Forms.MessageBoxButtons.</exception>
        public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton, MessageBoxOptions options) { }

        /// <summary>
        /// Displays a message box in front of the specified object and with the specified text, caption, buttons, icon, default button, and options.
        /// </summary>
        /// <param name="owner">An implementation of System.Windows.Forms.IWin32Window that will own the modal dialog box.</param>
        /// <param name="text">The text to display in the message box.</param>
        /// <param name="caption">The text to display in the title bar of the message box.</param>
        /// <param name="buttons">One of the System.Windows.Forms.MessageBoxButtons values that specifies which buttons to display in the message box.</param>
        /// <param name="icon">One of the System.Windows.Forms.MessageBoxIcon values that specifies which icon to display in the message box.</param>
        /// <param name="defaultButton">One of the System.Windows.Forms.MessageBoxDefaultButton values that specifies the default button for the message box.</param>
        /// <param name="options">One of the System.Windows.Forms.MessageBoxOptions values that specifies which display and association options will be used for the message box. You may pass in 0 if you wish to use the defaults.</param>
        /// <returns>One of the System.Windows.Forms.DialogResult values.</returns>
        /// <exception cref="System.ComponentModel.InvalidEnumArgumentException">The buttons parameter specified is not a member of System.Windows.Forms.MessageBoxButtons.
        /// -or- The icon parameter specified is not a member of System.Windows.Forms.MessageBoxIcon.
        /// -or- defaultButton is not a member of System.Windows.Forms.MessageBoxDefaultButton.</exception>
        /// <exception cref="System.InvalidOperationException">An attempt was made to display the System.Windows.Forms.MessageBox in a process that is not running in User Interactive mode. This is specified by the System.Windows.Forms.SystemInformation.UserInteractive property.</exception>
        /// <exception cref="System.ArgumentException">options specified both System.Windows.Forms.MessageBoxOptions.DefaultDesktopOnly and System.Windows.Forms.MessageBoxOptions.ServiceNotification.
        /// -or- options specified System.Windows.Forms.MessageBoxOptions.DefaultDesktopOnly or System.Windows.Forms.MessageBoxOptions.ServiceNotification and specified a value in the owner parameter. These two options should be used only if you invoke the version of this method that does not take an owner parameter.
        /// -or- buttons specified an invalid combination of System.Windows.Forms.MessageBoxButtons.</exception>
        public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton, MessageBoxOptions options) { }

        /// <summary>
        /// Displays a message box with the specified text, caption, buttons, icon, default button, options, and Help button.
        /// </summary>
        /// <param name="text">The text to display in the message box.</param>
        /// <param name="caption">The text to display in the title bar of the message box.</param>
        /// <param name="buttons">One of the System.Windows.Forms.MessageBoxButtons values that specifies which buttons to display in the message box.</param>
        /// <param name="icon">One of the System.Windows.Forms.MessageBoxIcon values that specifies which icon to display in the message box.</param>
        /// <param name="defaultButton">One of the System.Windows.Forms.MessageBoxDefaultButton values that specifies the default button for the message box.</param>
        /// <param name="options">One of the System.Windows.Forms.MessageBoxOptions values that specifies which display and association options will be used for the message box. You may pass in 0 if you wish to use the defaults.</param>
        /// <param name="helpButton">true to show the Help button; otherwise, false. The default is false.</param>
        /// <returns>One of the System.Windows.Forms.DialogResult values.</returns>
        /// <exception cref="System.ComponentModel.InvalidEnumArgumentException">The buttons parameter specified is not a member of System.Windows.Forms.MessageBoxButtons.
        /// -or- The icon parameter specified is not a member of System.Windows.Forms.MessageBoxIcon.
        /// -or- defaultButton is not a member of System.Windows.Forms.MessageBoxDefaultButton.</exception>
        /// <exception cref="System.InvalidOperationException">An attempt was made to display the System.Windows.Forms.MessageBox in a process that is not running in User Interactive mode. This is specified by the System.Windows.Forms.SystemInformation.UserInteractive property.</exception>
        /// <exception cref="System.ArgumentException">options specified both System.Windows.Forms.MessageBoxOptions.DefaultDesktopOnly and System.Windows.Forms.MessageBoxOptions.ServiceNotification.
        /// -or- buttons specified an invalid combination of System.Windows.Forms.MessageBoxButtons.</exception>
        public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton, MessageBoxOptions options, bool displayHelpButton) { }

        /// <summary>
        /// Displays a message box with the specified text, caption, buttons, icon, default button, options, and Help button, using the specified Help file.
        /// </summary>
        /// <param name="text">The text to display in the message box.</param>
        /// <param name="caption">The text to display in the title bar of the message box.</param>
        /// <param name="buttons">One of the System.Windows.Forms.MessageBoxButtons values that specifies which buttons to display in the message box.</param>
        /// <param name="icon">One of the System.Windows.Forms.MessageBoxIcon values that specifies which icon to display in the message box.</param>
        /// <param name="defaultButton">One of the System.Windows.Forms.MessageBoxDefaultButton values that specifies the default button for the message box.</param>
        /// <param name="options">One of the System.Windows.Forms.MessageBoxOptions values that specifies which display and association options will be used for the message box. You may pass in 0 if you wish to use the defaults.</param>
        /// <param name="helpFilePath">The path and name of the Help file to display when the user clicks the Help button.</param>
        /// <returns>One of the System.Windows.Forms.DialogResult values.</returns>
        /// <exception cref="System.ComponentModel.InvalidEnumArgumentException">The buttons parameter specified is not a member of System.Windows.Forms.MessageBoxButtons.
        /// -or- The icon parameter specified is not a member of System.Windows.Forms.MessageBoxIcon.
        /// -or- defaultButton is not a member of System.Windows.Forms.MessageBoxDefaultButton.</exception>
        /// <exception cref="System.InvalidOperationException">An attempt was made to display the System.Windows.Forms.MessageBox in a process that is not running in User Interactive mode. This is specified by the System.Windows.Forms.SystemInformation.UserInteractive property.</exception>
        /// <exception cref="System.ArgumentException">options specified both System.Windows.Forms.MessageBoxOptions.DefaultDesktopOnly and System.Windows.Forms.MessageBoxOptions.ServiceNotification.
        /// -or- buttons specified an invalid combination of System.Windows.Forms.MessageBoxButtons.</exception>
        public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton, MessageBoxOptions options, string helpFilePath) { }

        /// <summary>
        /// Displays a message box in front of the specified object and with the specified text, caption, buttons, icon, default button, options, and Help button, using the specified Help file.
        /// </summary>
        /// <param name="owner">An implementation of System.Windows.Forms.IWin32Window that will own the modal dialog box.</param>
        /// <param name="text">The text to display in the message box.</param>
        /// <param name="caption">The text to display in the title bar of the message box.</param>
        /// <param name="buttons">One of the System.Windows.Forms.MessageBoxButtons values that specifies which buttons to display in the message box.</param>
        /// <param name="icon">One of the System.Windows.Forms.MessageBoxIcon values that specifies which icon to display in the message box.</param>
        /// <param name="defaultButton">One of the System.Windows.Forms.MessageBoxDefaultButton values that specifies the default button for the message box.</param>
        /// <param name="options">One of the System.Windows.Forms.MessageBoxOptions values that specifies which display and association options will be used for the message box. You may pass in 0 if you wish to use the defaults.</param>
        /// <param name="helpFilePath">The path and name of the Help file to display when the user clicks the Help button.</param>
        /// <returns>One of the System.Windows.Forms.DialogResult values.</returns>
        /// <exception cref="System.ComponentModel.InvalidEnumArgumentException">The buttons parameter specified is not a member of System.Windows.Forms.MessageBoxButtons.
        /// -or- The icon parameter specified is not a member of System.Windows.Forms.MessageBoxIcon.
        /// -or- defaultButton is not a member of System.Windows.Forms.MessageBoxDefaultButton.</exception>
        /// <exception cref="System.InvalidOperationException">An attempt was made to display the System.Windows.Forms.MessageBox in a process that is not running in User Interactive mode. This is specified by the System.Windows.Forms.SystemInformation.UserInteractive property.</exception>
        /// <exception cref="System.ArgumentException">options specified both System.Windows.Forms.MessageBoxOptions.DefaultDesktopOnly and System.Windows.Forms.MessageBoxOptions.ServiceNotification.
        /// -or- buttons specified an invalid combination of System.Windows.Forms.MessageBoxButtons.</exception>
        public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton, MessageBoxOptions options, string helpFilePath) { }

        /// <summary>
        /// Displays a message box with the specified text, caption, buttons, icon, default button, options, and Help button, using the specified Help file and HelpNavigator.
        /// </summary>
        /// <param name="text">The text to display in the message box.</param>
        /// <param name="caption">The text to display in the title bar of the message box.</param>
        /// <param name="buttons">One of the System.Windows.Forms.MessageBoxButtons values that specifies which buttons to display in the message box.</param>
        /// <param name="icon">One of the System.Windows.Forms.MessageBoxIcon values that specifies which icon to display in the message box.</param>
        /// <param name="defaultButton">One of the System.Windows.Forms.MessageBoxDefaultButton values that specifies the default button for the message box.</param>
        /// <param name="options">One of the System.Windows.Forms.MessageBoxOptions values that specifies which display and association options will be used for the message box. You may pass in 0 if you wish to use the defaults.</param>
        /// <param name="helpFilePath">The path and name of the Help file to display when the user clicks the Help button.</param>
        /// <param name="navigator">One of the System.Windows.Forms.HelpNavigator values.</param>
        /// <returns>One of the System.Windows.Forms.DialogResult values.</returns>
        /// <exception cref="System.ComponentModel.InvalidEnumArgumentException">The buttons parameter specified is not a member of System.Windows.Forms.MessageBoxButtons.
        /// -or- The icon parameter specified is not a member of System.Windows.Forms.MessageBoxIcon.
        /// -or- defaultButton is not a member of System.Windows.Forms.MessageBoxDefaultButton.</exception>
        /// <exception cref="System.InvalidOperationException">An attempt was made to display the System.Windows.Forms.MessageBox in a process that is not running in User Interactive mode. This is specified by the System.Windows.Forms.SystemInformation.UserInteractive property.</exception>
        /// <exception cref="System.ArgumentException">options specified both System.Windows.Forms.MessageBoxOptions.DefaultDesktopOnly and System.Windows.Forms.MessageBoxOptions.ServiceNotification.
        /// -or- buttons specified an invalid combination of System.Windows.Forms.MessageBoxButtons.</exception>
        public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton, MessageBoxOptions options, string helpFilePath, HelpNavigator navigator) { }

        /// <summary>
        /// Displays a message box with the specified text, caption, buttons, icon, default button, options, and Help button, using the specified Help file and Help keyword.
        /// </summary>
        /// <param name="text">The text to display in the message box.</param>
        /// <param name="caption">The text to display in the title bar of the message box.</param>
        /// <param name="buttons">One of the System.Windows.Forms.MessageBoxButtons values that specifies which buttons to display in the message box.</param>
        /// <param name="icon">One of the System.Windows.Forms.MessageBoxIcon values that specifies which icon to display in the message box.</param>
        /// <param name="defaultButton">One of the System.Windows.Forms.MessageBoxDefaultButton values that specifies the default button for the message box.</param>
        /// <param name="options">One of the System.Windows.Forms.MessageBoxOptions values that specifies which display and association options will be used for the message box. You may pass in 0 if you wish to use the defaults.</param>
        /// <param name="helpFilePath">The path and name of the Help file to display when the user clicks the Help button.</param>
        /// <param name="keyword">The Help keyword to display when the user clicks the Help button.</param>
        /// <returns>One of the System.Windows.Forms.DialogResult values.</returns>
        /// <exception cref="System.ComponentModel.InvalidEnumArgumentException">The buttons parameter specified is not a member of System.Windows.Forms.MessageBoxButtons.
        /// -or- The icon parameter specified is not a member of System.Windows.Forms.MessageBoxIcon.
        /// -or- defaultButton is not a member of System.Windows.Forms.MessageBoxDefaultButton.</exception>
        /// <exception cref="System.InvalidOperationException">An attempt was made to display the System.Windows.Forms.MessageBox in a process that is not running in User Interactive mode. This is specified by the System.Windows.Forms.SystemInformation.UserInteractive property.</exception>
        /// <exception cref="System.ArgumentException">options specified both System.Windows.Forms.MessageBoxOptions.DefaultDesktopOnly and System.Windows.Forms.MessageBoxOptions.ServiceNotification.
        /// -or- buttons specified an invalid combination of System.Windows.Forms.MessageBoxButtons.</exception>
        public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton, MessageBoxOptions options, string helpFilePath, string keyword) { }

        /// <summary>
        /// Displays a message box in front of the specified object and with the specified text, caption, buttons, icon, default button, options, and Help button, using the specified Help file and HelpNavigator.
        /// </summary>
        /// <param name="owner">An implementation of System.Windows.Forms.IWin32Window that will own the modal dialog box.</param>
        /// <param name="text">The text to display in the message box.</param>
        /// <param name="caption">The text to display in the title bar of the message box.</param>
        /// <param name="buttons">One of the System.Windows.Forms.MessageBoxButtons values that specifies which buttons to display in the message box.</param>
        /// <param name="icon">One of the System.Windows.Forms.MessageBoxIcon values that specifies which icon to display in the message box.</param>
        /// <param name="defaultButton">One of the System.Windows.Forms.MessageBoxDefaultButton values that specifies the default button for the message box.</param>
        /// <param name="options">One of the System.Windows.Forms.MessageBoxOptions values that specifies which display and association options will be used for the message box. You may pass in 0 if you wish to use the defaults.</param>
        /// <param name="helpFilePath">The path and name of the Help file to display when the user clicks the Help button.</param>
        /// <param name="navigator">One of the System.Windows.Forms.HelpNavigator values.</param>
        /// <returns>One of the System.Windows.Forms.DialogResult values.</returns>
        /// <exception cref="System.ComponentModel.InvalidEnumArgumentException">The buttons parameter specified is not a member of System.Windows.Forms.MessageBoxButtons.
        /// -or- The icon parameter specified is not a member of System.Windows.Forms.MessageBoxIcon.
        /// -or- defaultButton is not a member of System.Windows.Forms.MessageBoxDefaultButton.</exception>
        /// <exception cref="System.InvalidOperationException">An attempt was made to display the System.Windows.Forms.MessageBox in a process that is not running in User Interactive mode. This is specified by the System.Windows.Forms.SystemInformation.UserInteractive property.</exception>
        /// <exception cref="System.ArgumentException">options specified both System.Windows.Forms.MessageBoxOptions.DefaultDesktopOnly and System.Windows.Forms.MessageBoxOptions.ServiceNotification.
        /// -or- buttons specified an invalid combination of System.Windows.Forms.MessageBoxButtons.</exception>
        public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton, MessageBoxOptions options, string helpFilePath, HelpNavigator navigator) { }

        /// <summary>
        /// Displays a message box in front of the specified object and with the specified text, caption, buttons, icon, default button, options, and Help button, using the specified Help file and Help keyword.
        /// </summary>
        /// <param name="owner">An implementation of System.Windows.Forms.IWin32Window that will own the modal dialog box.</param>
        /// <param name="text">The text to display in the message box.</param>
        /// <param name="caption">The text to display in the title bar of the message box.</param>
        /// <param name="buttons">One of the System.Windows.Forms.MessageBoxButtons values that specifies which buttons to display in the message box.</param>
        /// <param name="icon">One of the System.Windows.Forms.MessageBoxIcon values that specifies which icon to display in the message box.</param>
        /// <param name="defaultButton">One of the System.Windows.Forms.MessageBoxDefaultButton values that specifies the default button for the message box.</param>
        /// <param name="options">One of the System.Windows.Forms.MessageBoxOptions values that specifies which display and association options will be used for the message box. You may pass in 0 if you wish to use the defaults.</param>
        /// <param name="helpFilePath">The path and name of the Help file to display when the user clicks the Help button.</param>
        /// <param name="keyword">The Help keyword to display when the user clicks the Help button.</param>
        /// <returns>One of the System.Windows.Forms.DialogResult values.</returns>
        /// <exception cref="System.ComponentModel.InvalidEnumArgumentException">The buttons parameter specified is not a member of System.Windows.Forms.MessageBoxButtons.
        /// -or- The icon parameter specified is not a member of System.Windows.Forms.MessageBoxIcon.
        /// -or- defaultButton is not a member of System.Windows.Forms.MessageBoxDefaultButton.</exception>
        /// <exception cref="System.InvalidOperationException">An attempt was made to display the System.Windows.Forms.MessageBox in a process that is not running in User Interactive mode. This is specified by the System.Windows.Forms.SystemInformation.UserInteractive property.</exception>
        /// <exception cref="System.ArgumentException">options specified both System.Windows.Forms.MessageBoxOptions.DefaultDesktopOnly and System.Windows.Forms.MessageBoxOptions.ServiceNotification.
        /// -or- buttons specified an invalid combination of System.Windows.Forms.MessageBoxButtons.</exception>
        public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton, MessageBoxOptions options, string helpFilePath, string keyword) { }

        /// <summary>
        /// Displays a message box with the specified text, caption, buttons, icon, default button, options, and Help button, using the specified Help file and Help topic.
        /// </summary>
        /// <param name="text">The text to display in the message box.</param>
        /// <param name="caption">The text to display in the title bar of the message box.</param>
        /// <param name="buttons">One of the System.Windows.Forms.MessageBoxButtons values that specifies which buttons to display in the message box.</param>
        /// <param name="icon">One of the System.Windows.Forms.MessageBoxIcon values that specifies which icon to display in the message box.</param>
        /// <param name="defaultButton">One of the System.Windows.Forms.MessageBoxDefaultButton values that specifies the default button for the message box.</param>
        /// <param name="options">One of the System.Windows.Forms.MessageBoxOptions values that specifies which display and association options will be used for the message box. You may pass in 0 if you wish to use the defaults.</param>
        /// <param name="helpFilePath">The path and name of the Help file to display when the user clicks the Help button.</param>
        /// <param name="param">The numeric ID of the Help topic to display when the user clicks the Help button.</param>
        /// <returns>One of the System.Windows.Forms.DialogResult values.</returns>
        /// <exception cref="System.ComponentModel.InvalidEnumArgumentException">The buttons parameter specified is not a member of System.Windows.Forms.MessageBoxButtons.
        /// -or- The icon parameter specified is not a member of System.Windows.Forms.MessageBoxIcon.
        /// -or- defaultButton is not a member of System.Windows.Forms.MessageBoxDefaultButton.</exception>
        /// <exception cref="System.InvalidOperationException">An attempt was made to display the System.Windows.Forms.MessageBox in a process that is not running in User Interactive mode. This is specified by the System.Windows.Forms.SystemInformation.UserInteractive property.</exception>
        /// <exception cref="System.ArgumentException">options specified both System.Windows.Forms.MessageBoxOptions.DefaultDesktopOnly and System.Windows.Forms.MessageBoxOptions.ServiceNotification.
        /// -or- buttons specified an invalid combination of System.Windows.Forms.MessageBoxButtons.</exception>
        public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton, MessageBoxOptions options, string helpFilePath, HelpNavigator navigator, object param) { }

        /// <summary>
        /// Displays a message box in front of the specified object and with the specified text, caption, buttons, icon, default button, options, and Help button, using the specified Help file and Help topic.
        /// </summary>
        /// <param name="owner">An implementation of System.Windows.Forms.IWin32Window that will own the modal dialog box.</param>
        /// <param name="text">The text to display in the message box.</param>
        /// <param name="caption">The text to display in the title bar of the message box.</param>
        /// <param name="buttons">One of the System.Windows.Forms.MessageBoxButtons values that specifies which buttons to display in the message box.</param>
        /// <param name="icon">One of the System.Windows.Forms.MessageBoxIcon values that specifies which icon to display in the message box.</param>
        /// <param name="defaultButton">One of the System.Windows.Forms.MessageBoxDefaultButton values that specifies the default button for the message box.</param>
        /// <param name="options">One of the System.Windows.Forms.MessageBoxOptions values that specifies which display and association options will be used for the message box. You may pass in 0 if you wish to use the defaults.</param>
        /// <param name="helpFilePath">The path and name of the Help file to display when the user clicks the Help button.</param>
        /// <param name="param">The numeric ID of the Help topic to display when the user clicks the Help button.</param>
        /// <returns>One of the System.Windows.Forms.DialogResult values.</returns>
        /// <exception cref="System.ComponentModel.InvalidEnumArgumentException">The buttons parameter specified is not a member of System.Windows.Forms.MessageBoxButtons.
        /// -or- The icon parameter specified is not a member of System.Windows.Forms.MessageBoxIcon.
        /// -or- defaultButton is not a member of System.Windows.Forms.MessageBoxDefaultButton.</exception>
        /// <exception cref="System.InvalidOperationException">An attempt was made to display the System.Windows.Forms.MessageBox in a process that is not running in User Interactive mode. This is specified by the System.Windows.Forms.SystemInformation.UserInteractive property.</exception>
        /// <exception cref="System.ArgumentException">options specified both System.Windows.Forms.MessageBoxOptions.DefaultDesktopOnly and System.Windows.Forms.MessageBoxOptions.ServiceNotification.
        /// -or- buttons specified an invalid combination of System.Windows.Forms.MessageBoxButtons.</exception>
        public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton, MessageBoxOptions options, string helpFilePath, HelpNavigator navigator, object param) { }
#endif
    }
}

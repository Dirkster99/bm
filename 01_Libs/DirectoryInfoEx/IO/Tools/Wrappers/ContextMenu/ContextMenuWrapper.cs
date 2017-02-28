///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// LYCJ (c) 2009 - http://www.quickzip.org/components                                                            //
// Release under LGPL license.                                                                                   //
//                                                                                                               //
// This code used part of Steven Roebert's work (http://www.codeproject.com/KB/miscctrl/FileBrowser.aspx)    //
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using ShellDll;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Diagnostics;


namespace System.IO.Tools
{
    public class MouseHoverEventArgs : EventArgs
    {
        public string Info { get; private set; }
        public string Command { get; private set; }
        public uint CmdID { get; private set; }

        public MouseHoverEventArgs(string command, string info, uint cmdID)
        { Command = command; Info = info; ; CmdID = cmdID; }
    }

    public class InvokeCommandEventArgs : MouseHoverEventArgs
    {
        public bool ContinueInvoke { get; set; }
        public FileSystemInfoEx[] SelectedItems { get; private set; }

        public InvokeCommandEventArgs(string command, string info, uint cmdID,
            FileSystemInfoEx[] selectedItems)
            : base(command, info, cmdID)
        {
            SelectedItems = selectedItems;
            ContinueInvoke = true;
        }
    }

    public class QueryMenuItemsEventArgs : EventArgs
    {
        public bool QueryContextMenu { get; set; }
        public bool QueryContextMenu2 { get; set; }
        public bool QueryContextMenu3 { get; set; }
        public string[] ExtraMenuItems { get; set; }
        public string[] GrayedItems { get; set; }
        public string[] HiddenItems { get; set; }
        public int DefaultItem { get; set; }
        public FileSystemInfoEx[] SelectedItems { get; private set; }
        public QueryMenuItemsEventArgs(FileSystemInfoEx[] selectedItems)
        {
            ExtraMenuItems = new string[] { }; SelectedItems = selectedItems; DefaultItem = -1;
            QueryContextMenu = true; QueryContextMenu2 = true; QueryContextMenu3 = true;
        }
    }

    public class BeforePopupEventArgs : EventArgs
    {
        public IntPtr ptrPopupMenu = IntPtr.Zero;
        public IContextMenu iContextMenu = null;
        public bool ContinuePopup { get; set; }
        public uint DefaultCommandID { get; set; }
        public string DefaultCommand { get; set; }
        public BeforePopupEventArgs(IntPtr ptrPopupMenu, IContextMenu iContextMenu, uint defaultCommandID, string defaultCommand)
        {
            this.ptrPopupMenu = ptrPopupMenu;
            this.iContextMenu = iContextMenu;
            ContinuePopup = true;
            DefaultCommandID = defaultCommandID;
            DefaultCommand = defaultCommand;
        }
    }

    public delegate void MouseHoverEventHandler(object sender, MouseHoverEventArgs args);
    public delegate void InvokeCommandEventHandler(object sender, InvokeCommandEventArgs args);
    public delegate void QueryMenuItemsEventHandler(object sender, QueryMenuItemsEventArgs args);
    public delegate void BeforePopupEventHandler(object sender, BeforePopupEventArgs args);

    public class ContextMenuWrapper : NativeWindow
    {
        /// <summary>
        /// Occur user hover on a menu item in popup menu.
        /// </summary>
        public MouseHoverEventHandler OnMouseHover;
        /// <summary>
        /// Occur to query user (non-shell) items, and allow set query IContextMenu2/IContextMenu3 interface.
        /// </summary>
        public QueryMenuItemsEventHandler OnQueryMenuItems;
        /// <summary>
        /// Occur when context menu is prepared and is about to popup.
        /// </summary>
        public BeforePopupEventHandler OnBeforePopup;
        /// <summary>
        /// Occur when user selected a command from the popup and is about to invoke the command.
        /// </summary>
        public InvokeCommandEventHandler OnBeforeInvokeCommand;

        IContextMenu _iContextMenu;
        IContextMenu2 _iContextMenu2;
        IContextMenu3 _iContextMenu3;

        bool _contextMenuVisible = false;

        public ContextMenuWrapper()
        {
            this.CreateHandle(new CreateParams());
        }

        private string removeCheckedSymbol(string command)
        {
            return command.EndsWith("[*]") ? command.Substring(0, command.Length - 3) : command;
        }

        protected override void WndProc(ref Message m)
        {
            #region IContextMenu

            if (_iContextMenu != null &&
                m.Msg == (int)ShellAPI.WM.MENUSELECT &&
                ((int)ShellHelper.HiWord(m.WParam) & (int)ShellAPI.MFT.SEPARATOR) == 0)
            //&&
            //((int)ShellHelper.HiWord(m.WParam) & (int)ShellAPI.MFT.POPUP) == 0)
            {
                uint cmdID = ShellHelper.LoWord(m.WParam);
                string info = null, command = null;
                if (cmdID == 0)
                {
                    return;
                }
                else
                    if (cmdID > ShellAPI.CMD_LAST)
                    {
                        info = command = removeCheckedSymbol(queryMenuItemsEventArgs.ExtraMenuItems[cmdID - ShellAPI.CMD_LAST - 1]);
                    }
                    else
                    {
                        info = ContextMenuHelper.GetCommandString(
                            _iContextMenu, cmdID - ShellAPI.CMD_FIRST,
                            false);
                        command = ContextMenuHelper.GetCommandString(
                            _iContextMenu, cmdID - ShellAPI.CMD_FIRST,
                            true);
                    }
                if (OnMouseHover != null)
                    OnMouseHover(this, new MouseHoverEventArgs(command, info.ToString(), cmdID));



            }

            #endregion


            #region IContextMenu2

            if (_iContextMenu2 != null &&
                (m.Msg == (int)ShellAPI.WM.INITMENUPOPUP ||
                 m.Msg == (int)ShellAPI.WM.MEASUREITEM ||
                 m.Msg == (int)ShellAPI.WM.DRAWITEM))
            {
                if (_iContextMenu2.HandleMenuMsg(
                    (uint)m.Msg, m.WParam, m.LParam) == ShellAPI.S_OK)
                    return;
            }

            #endregion

            #region IContextMenu3

            if (_iContextMenu3 != null &&
                m.Msg == (int)ShellAPI.WM.MENUCHAR)
            {
                if (_iContextMenu3.HandleMenuMsg2(
                    (uint)m.Msg, m.WParam, m.LParam, IntPtr.Zero) == ShellAPI.S_OK)
                    return;
            }

            #endregion

            base.WndProc(ref m);
        }

        QueryMenuItemsEventArgs queryMenuItemsEventArgs;
        public string Popup(FileSystemInfoEx[] items, Point pt)
        {
            if (items.Length > 0 && !_contextMenuVisible)
            {
                //0.15: Fixed ShellFolder not freed correctly.
                try
                {
                    using (ShellFolder2 parentShellFolder = items[0].Parent != null ?
                            items[0].Parent.ShellFolder :
                            DirectoryInfoEx.DesktopDirectory.ShellFolder)
                        try
                        {
                            _contextMenuVisible = true;

                            ///Debug.WriteLine(items[0].Parent.FullName);

                            IntPtr ptrContextMenu = IntPtr.Zero;
                            IntPtr ptrContextMenu2 = IntPtr.Zero;
                            IntPtr PtrContextMenu3 = IntPtr.Zero;
                            IntPtr contextMenu = IntPtr.Zero;
                            List<IntPtr> menuPtrConstructed = new List<IntPtr>();
                            List<IntPtr> imgPtrConstructed = new List<IntPtr>();

                            PIDL[] pidls = IOTools.GetPIDL(items, true);

                            if (ContextMenuHelper.GetIContextMenu(parentShellFolder, IOTools.GetPIDLPtr(pidls),
                                out ptrContextMenu, out _iContextMenu))
                                try
                                {

                                    queryMenuItemsEventArgs = new QueryMenuItemsEventArgs(items);
                                    if (OnQueryMenuItems != null)
                                        OnQueryMenuItems(this, queryMenuItemsEventArgs);

                                    contextMenu = ShellAPI.CreatePopupMenu();

                                    if (queryMenuItemsEventArgs.QueryContextMenu)
                                        _iContextMenu.QueryContextMenu(contextMenu, 0, ShellAPI.CMD_FIRST,
                                            ShellAPI.CMD_LAST, ShellAPI.CMF.EXPLORE | ShellAPI.CMF.CANRENAME |
                                            ((Control.ModifierKeys & Keys.Shift) != 0 ? ShellAPI.CMF.EXTENDEDVERBS : 0));

                                    #region obsolute
                                    //for (uint i = 0; i < queryMenuItemsEventArgs.ExtraMenuItems.Length; i++)
                                    //{
                                    //    string caption = queryMenuItemsEventArgs.ExtraMenuItems[i];
                                    //    if (caption != "---")
                                    //    {
                                    //        ShellAPI.InsertMenu(contextMenu, i, ShellAPI.MFT.BYPOSITION,
                                    //            ShellAPI.CMD_LAST + i + 1, caption);
                                    //        if (queryMenuItemsEventArgs.DefaultItem == i)
                                    //            ShellAPI.SetMenuDefaultItem(contextMenu, i, true);
                                    //    }
                                    //    else ShellAPI.InsertMenu(contextMenu, i, ShellAPI.MFT.BYPOSITION |
                                    //        ShellAPI.MFT.SEPARATOR, 0, "-");
                                    //}
                                    #endregion

                                    //0.11: Added ContextMenuWrapper OnQueryMenuItems event now support multilevel directory. (e.g. @"Tools\Add")
                                    ContextMenuHelperEx.ConstructCustomMenu(contextMenu, queryMenuItemsEventArgs.ExtraMenuItems, out menuPtrConstructed, out imgPtrConstructed);

                                    if (queryMenuItemsEventArgs.QueryContextMenu2)
                                        try
                                        {
                                            Marshal.QueryInterface(ptrContextMenu, ref ShellAPI.IID_IContextMenu2,
                                                out ptrContextMenu2);
                                            _iContextMenu2 = (IContextMenu2)Marshal.GetTypedObjectForIUnknown(
                                            ptrContextMenu2, typeof(IContextMenu2));
                                        }
                                        catch (Exception) { }

                                    if (queryMenuItemsEventArgs.QueryContextMenu3)
                                        try
                                        {
                                            Marshal.QueryInterface(ptrContextMenu, ref ShellAPI.IID_IContextMenu3,
                                        out PtrContextMenu3);

                                            _iContextMenu3 = (IContextMenu3)Marshal.GetTypedObjectForIUnknown(
                                             PtrContextMenu3, typeof(IContextMenu3));
                                        }
                                        catch (Exception) { }


                                    uint GMDI_USEDISABLED = 0x0001;
                                    //uint GMDI_GOINTOPOPUPS = 0x0002;
                                    uint intDefaultItem = (uint)ShellAPI.GetMenuDefaultItem(contextMenu, false, GMDI_USEDISABLED);

                                    string strDefaultCommand = intDefaultItem >= ShellAPI.CMD_FIRST ?
                                        ContextMenuHelper.GetCommandString(_iContextMenu, intDefaultItem - ShellAPI.CMD_FIRST, true) : null;



                                    if (queryMenuItemsEventArgs.QueryContextMenu) //No need to Disable if query is not carried out in first place.
                                    {
                                        //0.11: Added queryMenuItemsEventArgs.GrayedItems / HiddenItems
                                        ContextMenuHelperEx.DisableMenuItems(contextMenu, _iContextMenu,
                                            queryMenuItemsEventArgs.GrayedItems, ContextMenuHelperEx.DisabledMethods.Gray);
                                        ContextMenuHelperEx.DisableMenuItems(contextMenu, _iContextMenu,
                                            queryMenuItemsEventArgs.HiddenItems, ContextMenuHelperEx.DisabledMethods.Remove);
                                    }

                                    //0.17: Added DefaultItem and DefaultCommand in BeforePopup
                                    bool cont = true;
                                    if (OnBeforePopup != null)
                                    {
                                        BeforePopupEventArgs args = new BeforePopupEventArgs
                                            (contextMenu, _iContextMenu, intDefaultItem - ShellAPI.CMD_FIRST, strDefaultCommand);
                                        OnBeforePopup(this, args);
                                        cont = args.ContinuePopup;
                                    }

                                    if (cont)
                                    {
                                        //0.18 Fixed Context menu disappear in some case. (By cwharmon)
                                        //http://www.codeproject.com/KB/files/DirectoryInfoEx.aspx#xx3475961xx 
                                        ShellAPI.SetForegroundWindow(this.Handle);

                                        uint selected = ShellAPI.TrackPopupMenuEx(contextMenu, ShellAPI.TPM.RETURNCMD,
                                                            pt.X, pt.Y, this.Handle, IntPtr.Zero);

                                        uint msg = 0;
                                        ShellAPI.PostMessage(this.Handle, msg, IntPtr.Zero, IntPtr.Zero);

                                        if (OnMouseHover != null)
                                            OnMouseHover(this, new MouseHoverEventArgs("", "", 0));

                                        if (selected >= ShellAPI.CMD_LAST)
                                        {
                                            return removeCheckedSymbol(queryMenuItemsEventArgs.ExtraMenuItems[selected - ShellAPI.CMD_LAST]);

                                        }

                                        if (selected >= ShellAPI.CMD_FIRST)
                                        {
                                            string command = ContextMenuHelper.GetCommandString(_iContextMenu,
                                                selected - ShellAPI.CMD_FIRST, true);
                                            if (command == null) return null;

                                            if (OnBeforeInvokeCommand != null)
                                            {
                                                InvokeCommandEventArgs args =
                                                    new InvokeCommandEventArgs(command, "", selected, items);
                                                OnBeforeInvokeCommand(this, args);
                                                if (!args.ContinueInvoke)
                                                    return command;
                                            }

                                            if (command == "rename")
                                                return "rename";
                                            else
                                            {
                                                //if (items.Length == 1 && items[0] is DirectoryInfoEx)
                                                ContextMenuHelper.InvokeCommand(_iContextMenu,
                                                    selected - ShellAPI.CMD_FIRST,
                                                    (items[0].Parent != null) ? items[0].Parent.FullName :
                                                    items[0].FullName, pt);
                                                //else
                                                //ContextMenuHelper.InvokeCommand(items[0].Parent,
                                                //    IOTools.GetPIDLPtr(items, true), selected - ShellAPI.CMD_FIRST,
                                                //    pt);

                                            }

                                        }
                                    }
                                }
                                finally
                                {
                                    IOTools.FreePIDL(pidls);

                                    if (_iContextMenu != null)
                                    {
                                        Marshal.ReleaseComObject(_iContextMenu);
                                        _iContextMenu = null;
                                    }

                                    if (_iContextMenu2 != null)
                                    {
                                        Marshal.ReleaseComObject(_iContextMenu2);
                                        _iContextMenu2 = null;
                                    }

                                    if (_iContextMenu3 != null)
                                    {
                                        Marshal.ReleaseComObject(_iContextMenu3);
                                        _iContextMenu3 = null;
                                    }

                                    foreach (IntPtr menuPtr in menuPtrConstructed)
                                        ShellAPI.DestroyMenu(menuPtr);
                                    menuPtrConstructed.Clear();

                                    if (contextMenu != null)
                                        ShellAPI.DestroyMenu(contextMenu);

                                    if (ptrContextMenu != IntPtr.Zero)
                                        Marshal.Release(ptrContextMenu);

                                    if (ptrContextMenu2 != IntPtr.Zero)
                                        Marshal.Release(ptrContextMenu2);

                                    if (PtrContextMenu3 != IntPtr.Zero)
                                        Marshal.Release(PtrContextMenu3);
                                }



                        }
                        finally
                        {

                            _contextMenuVisible = false;
                        }


                }
                catch { }
            }

            return null;
        }


    }

    //Taken from BrowserTVContextMenuWrapper.cs
    #region ContextMenuHelper

    /// <summary>
    /// This class provides static methods which are being used to retrieve IContextMenu's for specific items
    /// and to invoke certain commands.
    /// </summary>
    public static class ContextMenuHelper
    {
        #region GetCommandString

        public static string GetCommandString(IContextMenu iContextMenu, uint idcmd, bool executeString)
        {
            try
            {
                string command = GetCommandStringW(iContextMenu, idcmd, executeString);

                if (string.IsNullOrEmpty(command))
                    command = GetCommandStringA(iContextMenu, idcmd, executeString);

                return command;
            }
            catch (AccessViolationException) { return null; }
        }

        /// <summary>
        /// Retrieves the command string for a specific item from an iContextMenu (Ansi)
        /// </summary>
        /// <param name="iContextMenu">the IContextMenu to receive the string from</param>
        /// <param name="idcmd">the id of the specific item</param>
        /// <param name="executeString">indicating whether it should return an execute string or not</param>
        /// <returns>if executeString is true it will return the executeString for the item, 
        /// otherwise it will return the help info string</returns>
        internal static string GetCommandStringA(IContextMenu iContextMenu, uint idcmd, bool executeString)
        {
            string info = string.Empty;
            byte[] bytes = new byte[256];
            int index;

            iContextMenu.GetCommandString(
                idcmd,
                (executeString ? ShellAPI.GCS.VERBA : ShellAPI.GCS.HELPTEXTA),
                0,
                bytes,
                ShellAPI.MAX_PATH);

            index = 0;
            while (index < bytes.Length && bytes[index] != 0)
            { index++; }

            if (index < bytes.Length)
                info = Encoding.Default.GetString(bytes, 0, index);

            return info;
        }

        /// <summary>
        /// Retrieves the command string for a specific item from an iContextMenu (Unicode)
        /// </summary>
        /// <param name="iContextMenu">the IContextMenu to receive the string from</param>
        /// <param name="idcmd">the id of the specific item</param>
        /// <param name="executeString">indicating whether it should return an execute string or not</param>
        /// <returns>if executeString is true it will return the executeString for the item, 
        /// otherwise it will return the help info string</returns>
        internal static string GetCommandStringW(IContextMenu iContextMenu, uint idcmd, bool executeString)
        {
            string info = string.Empty;
            byte[] bytes = new byte[256];
            int index;

            iContextMenu.GetCommandString(
                idcmd,
                (executeString ? ShellAPI.GCS.VERBW : ShellAPI.GCS.HELPTEXTW),
                0,
                bytes,
                ShellAPI.MAX_PATH);

            index = 0;
            while (index < bytes.Length - 1 && (bytes[index] != 0 || bytes[index + 1] != 0))
            { index += 2; }

            if (index < bytes.Length - 1)
                info = Encoding.Unicode.GetString(bytes, 0, index); //+ 1);

            return info;
        }

        #endregion

        #region Invoke Commands

        /// <summary>
        /// Invokes a specific command from an IContextMenu
        /// </summary>
        /// <param name="iContextMenu">the IContextMenu containing the item</param>
        /// <param name="cmd">the index of the command to invoke</param>
        /// <param name="parentDir">the parent directory from where to invoke</param>
        /// <param name="ptInvoke">the point (in screen co顤dinates) from which to invoke</param>
        public static void InvokeCommand(IContextMenu iContextMenu, uint cmd, string parentDir, Point ptInvoke)
        {
            ShellAPI.CMINVOKECOMMANDINFOEX invoke = new ShellAPI.CMINVOKECOMMANDINFOEX();
            invoke.cbSize = ShellAPI.cbInvokeCommand;
            invoke.lpVerb = (IntPtr)cmd;
            invoke.lpDirectory = parentDir;
            invoke.lpVerbW = (IntPtr)cmd;
            invoke.lpDirectoryW = parentDir;
            invoke.fMask = ShellAPI.CMIC.UNICODE | ShellAPI.CMIC.PTINVOKE |
                ((Control.ModifierKeys & Keys.Control) != 0 ? ShellAPI.CMIC.CONTROL_DOWN : 0) |
                ((Control.ModifierKeys & Keys.Shift) != 0 ? ShellAPI.CMIC.SHIFT_DOWN : 0);
            invoke.ptInvoke = new ShellAPI.POINT(ptInvoke.X, ptInvoke.Y);
            invoke.nShow = ShellAPI.SW.SHOWNORMAL;

            iContextMenu.InvokeCommand(ref invoke);
        }

        /// <summary>
        /// Invokes a specific command from an IContextMenu
        /// </summary>
        /// <param name="iContextMenu">the IContextMenu containing the item</param>
        /// <param name="cmdA">the Ansi execute string to invoke</param>
        /// <param name="cmdW">the Unicode execute string to invoke</param>
        /// <param name="parentDir">the parent directory from where to invoke</param>
        /// <param name="ptInvoke">the point (in screen co顤dinates) from which to invoke</param>
        public static void InvokeCommand(IContextMenu iContextMenu, string cmd, string parentDir, Point ptInvoke)
        {
            ShellAPI.CMINVOKECOMMANDINFOEX invoke = new ShellAPI.CMINVOKECOMMANDINFOEX();
            invoke.cbSize = ShellAPI.cbInvokeCommand;
            invoke.lpVerb = Marshal.StringToHGlobalAnsi(cmd);
            invoke.lpDirectory = parentDir;
            invoke.lpVerbW = Marshal.StringToHGlobalUni(cmd);
            invoke.lpDirectoryW = parentDir;

            invoke.fMask = ShellAPI.CMIC.UNICODE | ShellAPI.CMIC.PTINVOKE |
                ((Control.ModifierKeys & Keys.Control) != 0 ? ShellAPI.CMIC.CONTROL_DOWN : 0) |
                ((Control.ModifierKeys & Keys.Shift) != 0 ? ShellAPI.CMIC.SHIFT_DOWN : 0);
            invoke.ptInvoke = new ShellAPI.POINT(ptInvoke.X, ptInvoke.Y);
            invoke.nShow = ShellAPI.SW.SHOWNORMAL;

            iContextMenu.InvokeCommand(ref invoke);
        }

        /// <summary>
        /// Invokes a specific command for a set of pidls
        /// </summary>
        /// <param name="parent">the parent ShellItem which contains the pidls</param>
        /// <param name="pidls">the pidls from the items for which to invoke</param>
        /// <param name="cmd">the execute string from the command to invoke</param>
        /// <param name="ptInvoke">the point (in screen co顤dinates) from which to invoke</param>
        public static void InvokeCommand(DirectoryInfoEx parent, IntPtr[] pidls, string cmd, Point ptInvoke)
        {
            IntPtr icontextMenuPtr;
            IContextMenu iContextMenu;

            //0.15: Fixed ShellFolder not freed.
            using (ShellFolder2 parentShellFolder = parent.ShellFolder)
                if (GetIContextMenu(parentShellFolder, pidls, out icontextMenuPtr, out iContextMenu))
                {
                    try
                    {
                        InvokeCommand(
                            iContextMenu,
                            cmd,
                            parent.FullName,
                            ptInvoke);
                    }
                    catch (Exception) { }
                    finally
                    {
                        if (iContextMenu != null)
                            Marshal.ReleaseComObject(iContextMenu);

                        if (icontextMenuPtr != IntPtr.Zero)
                            Marshal.Release(icontextMenuPtr);
                    }
                }
        }

        public static void InvokeCommand(DirectoryInfoEx parent, FileSystemInfoEx[] entries, string cmd, Point ptInvoke)
        {          
            PIDL[] pidls = IOTools.GetPIDL(entries, true);
            IntPtr[] ptrs = IOTools.GetPIDLPtr(pidls);            
            using (ShellFolder2 parentShellFolder = parent.ShellFolder)
            {
                try
                {
                    InvokeCommand(parent, ptrs, cmd, ptInvoke);
                }
                finally
                {
                    IOTools.FreePIDL(pidls);
                }
            }
        }

        public static void InvokeCommand(DirectoryInfoEx parent, IntPtr[] pidls, uint cmd, Point ptInvoke)
        {
            IntPtr icontextMenuPtr;
            IContextMenu iContextMenu;

            //0.15: Fixed ShellFolder not freed.
            using (ShellFolder2 parentShellFolder = parent.ShellFolder)
                if (GetIContextMenu(parentShellFolder, pidls, out icontextMenuPtr, out iContextMenu))
                {
                    try
                    {
                        InvokeCommand(
                            iContextMenu,
                            cmd,
                            parent.FullName,
                            ptInvoke);
                    }
                    catch (Exception) { }
                    finally
                    {
                        if (iContextMenu != null)
                            Marshal.ReleaseComObject(iContextMenu);

                        if (icontextMenuPtr != IntPtr.Zero)
                            Marshal.Release(icontextMenuPtr);
                    }
                }
        }

        public static void InvokeCommand(DirectoryInfoEx parent, FileSystemInfoEx[] entries, uint cmd, Point ptInvoke)
        {
            PIDL[] pidls = IOTools.GetPIDL(entries, true);
            IntPtr[] ptrs = IOTools.GetPIDLPtr(pidls);            
            using (ShellFolder2 parentShellFolder = parent.ShellFolder)
            {
                try
                {
                    InvokeCommand(parent, ptrs, cmd, ptInvoke);
                }
                finally
                {
                    IOTools.FreePIDL(pidls);
                }
            }
        }
        #endregion

        /// <summary>
        /// Retrieves the IContextMenu for specific items
        /// </summary>
        /// <param name="parent">the parent IShellFolder which contains the items</param>
        /// <param name="pidls">the pidls of the items for which to retrieve the IContextMenu</param>
        /// <param name="icontextMenuPtr">the pointer to the IContextMenu</param>
        /// <param name="iContextMenu">the IContextMenu for the items</param>
        /// <returns>true if the IContextMenu has been retrieved succesfully, false otherwise</returns>
        public static bool GetIContextMenu(
            IShellFolder2 parent,
            IntPtr[] pidls,
            out IntPtr iContextMenuPtr,
            out IContextMenu iContextMenu)
        {
            if (parent.GetUIObjectOf(
                        IntPtr.Zero,
                        (uint)pidls.Length,
                        pidls,
                        ref ShellAPI.IID_IContextMenu,
                        IntPtr.Zero,
                        out iContextMenuPtr) == ShellAPI.S_OK)
            {
                iContextMenu =
                    (IContextMenu)Marshal.GetTypedObjectForIUnknown(
                        iContextMenuPtr, typeof(IContextMenu));

                return true;
            }
            else
            {
                iContextMenuPtr = IntPtr.Zero;
                iContextMenu = null;

                return false;
            }
        }

        public static bool GetNewContextMenu(DirectoryInfoEx item, out IntPtr iContextMenuPtr, out IContextMenu iContextMenu)
        {
            if (ShellAPI.CoCreateInstance(
                    ref ShellAPI.CLSID_NewMenu,
                    IntPtr.Zero,
                    ShellAPI.CLSCTX.INPROC_SERVER,
                    ref ShellAPI.IID_IContextMenu,
                    out iContextMenuPtr) == ShellAPI.S_OK)
            {
                iContextMenu = Marshal.GetTypedObjectForIUnknown(iContextMenuPtr, typeof(IContextMenu)) as IContextMenu;

                IntPtr iShellExtInitPtr;
                if (Marshal.QueryInterface(
                    iContextMenuPtr,
                    ref ShellAPI.IID_IShellExtInit,
                    out iShellExtInitPtr) == ShellAPI.S_OK)
                {
                    IShellExtInit iShellExtInit = Marshal.GetTypedObjectForIUnknown(
                        iShellExtInitPtr, typeof(IShellExtInit)) as IShellExtInit;
                    
                    item.RequestPIDL(pidlLookup =>
                        {
                            iShellExtInit.Initialize(pidlLookup.Ptr, IntPtr.Zero, 0);

                            Marshal.ReleaseComObject(iShellExtInit);
                            Marshal.Release(iShellExtInitPtr);
                        });

                    return true;                    
                }
                else
                {
                    if (iContextMenu != null)
                    {
                        Marshal.ReleaseComObject(iContextMenu);
                        iContextMenu = null;
                    }

                    if (iContextMenuPtr != IntPtr.Zero)
                    {
                        Marshal.Release(iContextMenuPtr);
                        iContextMenuPtr = IntPtr.Zero;
                    }

                    return false;
                }
            }
            else
            {
                iContextMenuPtr = IntPtr.Zero;
                iContextMenu = null;
                return false;
            }
        }

        ///// <summary>
        ///// When keys are pressed, this method will check for known key combinations. For example copy and past with
        ///// Ctrl + C and Ctrl + V.
        ///// </summary>
        //public static void ProcessKeyCommands(Browser br, object sender, KeyEventArgs e)
        //{
        //    if (e.Control && !e.Shift && !e.Alt)
        //    {
        //        switch (e.KeyCode)
        //        {
        //            case Keys.C:
        //            case Keys.Insert:
        //            case Keys.V:
        //            case Keys.X:
        //                #region Copy/Paste/Cut
        //                {
        //                    Cursor.Current = Cursors.WaitCursor;
        //                    IntPtr[] pidls;
        //                    ShellItem parent;

        //                    pidls = new IntPtr[1];
        //                    pidls[0] = br.SelectedItem.PIDLRel.Ptr;
        //                    parent = (br.SelectedItem.ParentItem != null ? br.SelectedItem.ParentItem : br.SelectedItem);

        //                    if (pidls.Length > 0)
        //                    {
        //                        string cmd;
        //                        if (e.KeyCode == Keys.C || e.KeyCode == Keys.Insert)
        //                            cmd = "copy";
        //                        else if (e.KeyCode == Keys.V)
        //                            cmd = "paste";
        //                        else
        //                            cmd = "cut";

        //                        ContextMenuHelper.InvokeCommand(parent, pidls, cmd, new Point(0, 0));
        //                        Cursor.Current = Cursors.Default;
        //                    }
        //                    e.Handled = true;
        //                    e.SuppressKeyPress = true;
        //                }
        //                #endregion
        //                break;

        //            case Keys.N:
        //                #region Create New Folder
        //                if (!br.CreateNewFolder())
        //                    System.Media.SystemSounds.Beep.Play();

        //                e.Handled = true;
        //                e.SuppressKeyPress = true;
        //                #endregion
        //                break;

        //            case Keys.Z:
        //                break;

        //            case Keys.Y:
        //                break;
        //        }
        //    }
        //    else
        //    {
        //        switch (e.KeyCode)
        //        {
        //            case Keys.Insert:
        //                #region Paste
        //                if (e.Shift && !e.Control && !e.Alt)
        //                {
        //                    IntPtr[] pidls = new IntPtr[1];
        //                    pidls[0] = br.SelectedItem.PIDLRel.Ptr;
        //                    ShellItem parent = (br.SelectedItem.ParentItem != null ? br.SelectedItem.ParentItem : br.SelectedItem);
        //                    ContextMenuHelper.InvokeCommand(parent, pidls, "paste", new Point(0, 0));
        //                }
        //                e.Handled = true;
        //                e.SuppressKeyPress = true;
        //                #endregion
        //                break;

        //            case Keys.Delete:
        //                #region Delete
        //                if (!e.Control && !e.Alt)
        //                {
        //                    IntPtr[] pidls;
        //                    ShellItem parent;
        //                    pidls = new IntPtr[1];
        //                    pidls[0] = br.SelectedItem.PIDLRel.Ptr;
        //                    parent = (br.SelectedItem.ParentItem != null ? br.SelectedItem.ParentItem : br.SelectedItem);

        //                    if (pidls.Length > 0)
        //                        ContextMenuHelper.InvokeCommand(parent, pidls, "delete", new Point(0, 0));
        //                }
        //                e.Handled = true;
        //                e.SuppressKeyPress = true;
        //                #endregion
        //                break;

        //            case Keys.F2:
        //                #region Rename
        //                if (sender.Equals(br.FolderView))
        //                {
        //                    if (br.FolderView.SelectedNode != null)
        //                    {
        //                        br.FolderView.LabelEdit = true;
        //                        br.FolderView.SelectedNode.BeginEdit();
        //                    }
        //                }
        //                #endregion
        //                break;

        //            case Keys.Back:
        //                #region Up
        //                {
        //                    if (br.FolderView.SelectedNode != null && br.FolderView.SelectedNode.Parent != null)
        //                        br.FolderView.SelectedNode = br.FolderView.SelectedNode.Parent;
        //                }
        //                e.Handled = true;
        //                e.SuppressKeyPress = true;
        //                #endregion
        //                break;
        //        }
        //    }
        // }
    }

    #endregion


}

using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Tools;
using ShellDll;
using System.IO;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Drawing;

namespace System.IO.Tools
{
    public static class ContextMenuHelperEx
    {
        #region ConstructCustomMenu
        internal static CustomMenuStructure[] ParseCustomMenu(string[] menuTexts)
        {
            CustomMenuStructure root = new CustomMenuStructure();

            for (uint i = 0; i < menuTexts.Length; i++)
            {
                string menuText = menuTexts[i];
                string[] pathSplit = menuText.Split(new char[] { '\\' });
                CustomMenuStructure current = root;
                foreach (string path in pathSplit)
                {
                    if (current[path] == null)
                        current.Items.Add(
                            new CustomMenuStructure(path, i)); //folder may inherit incorrect id, but they are ignored anyway.
                    current = current[path];
                }
            }

            return root.Items.ToArray();
        }

        public static void DestroyMenuPtr(List<IntPtr> menuPtrConstructed)
        {
            foreach (IntPtr menuPtr in menuPtrConstructed)
            {
                ShellAPI.DestroyMenu(menuPtr);
            }
        }


        public static void DestroyImagePtr(List<IntPtr> imgPtrConstructed)
        {
            foreach (IntPtr imgPtr in imgPtrConstructed)
            {
                ShellAPI.DeleteObject(imgPtr);
            }
        }

        private static ShellAPI.MENUITEMINFO generateMenuItemInfo(CustomMenuStructure cms, uint idCmdFirst)
        {
            ShellAPI.MENUITEMINFO mii = new ShellAPI.MENUITEMINFO();
            mii.cbSize = Marshal.SizeOf(mii);
            mii.wID = idCmdFirst + cms.ID;
            mii.fType = ShellAPI.MFT.MFT_STRING;
            mii.fMask = ShellAPI.MIIM.ID | ShellAPI.MIIM.TYPE;
            mii.dwTypeData = cms.Text;
            mii.fState = ShellAPI.MFS.ENABLED;

            if (cms.Checked)
                mii.fState |= ShellAPI.MFS.CHECKED;
            if (cms.IsFolder)
                mii.fMask |= ShellAPI.MIIM.SUBMENU;

            //Not working, dont know why...
            //if (cms.Icon != null)
            //{
            //    IntPtr ptrBitmap = cms.Icon.GetHbitmap();
            //    imgPtrConstructed.Add(ptrBitmap);
            //    mii.hbmpItem = ptrBitmap;
            //    mii.fMask |= ShellAPI.MIIM.BITMAP;
            //    mii.fType |= ShellAPI.MFT.BITMAP;
            //}            
            return mii;
        }        

        public static void ConstructCustomMenu(IntPtr menu, CustomMenuStructure[] customItems, uint idCmdFirst,
            out List<IntPtr> menuPtrConstructed, out List<IntPtr> imgPtrConstructed)
        {
            menuPtrConstructed = new List<IntPtr>();
            imgPtrConstructed = new List<IntPtr>();


            for (int i = 0; i < customItems.Length; i++)
            {
                CustomMenuStructure item = customItems[i];

                if (item.IsFolder)
                {
                    IntPtr newPopup = ShellAPI.CreatePopupMenu();
                    List<IntPtr> submenuPtrConstructed, subimgPtrConstructed;

                    ConstructCustomMenu(newPopup, item.Items.ToArray(), idCmdFirst, out submenuPtrConstructed, out subimgPtrConstructed);
                   
                    insertMenu(menu, idCmdFirst, i, newPopup, item, ref imgPtrConstructed);
                    
                    menuPtrConstructed.Add(newPopup);
                    menuPtrConstructed.AddRange(submenuPtrConstructed);                    
                    imgPtrConstructed.AddRange(subimgPtrConstructed);
                }
                else
                    if (item.Text == "---")
                        ShellAPI.InsertMenu(menu, (uint)i, ShellAPI.MFT.BYPOSITION | ShellAPI.MFT.SEPARATOR, 0, "");
                    else
                    {                        
                        insertMenu(menu, idCmdFirst, i, IntPtr.Zero, item, ref imgPtrConstructed);
                    }

            }
        }

        private static void insertMenu(IntPtr menu, uint idCmdFirst, int pos, IntPtr newPopup, CustomMenuStructure item, ref List<IntPtr> imgPtrConstructed)
        {
            ShellAPI.MENUITEMINFO menuItemInfo = generateMenuItemInfo(item, idCmdFirst);
            if (newPopup != IntPtr.Zero)
                menuItemInfo.hSubMenu = newPopup;
            ShellAPI.InsertMenuItem(menu, (uint)pos, true, ref menuItemInfo);
            if (item.Icon != null)
            {
                IntPtr ptrBitmap = item.Icon.GetHbitmap();
                imgPtrConstructed.Add(ptrBitmap);
                ShellAPI.SetMenuItemBitmaps(menu, (uint)pos, (uint)ShellAPI.MFT.BYPOSITION, ptrBitmap, ptrBitmap);
            }            
        }

        public static void ConstructCustomMenu(IntPtr menu, string[] customItems,
            out List<IntPtr> menuPtrConstructed, out List<IntPtr> imgPtrConstructed)
        {
            ConstructCustomMenu(menu, ParseCustomMenu(customItems), ShellAPI.CMD_LAST, out menuPtrConstructed, out imgPtrConstructed);
        }
        #endregion

        #region DisableMenuItems
        internal enum MenuIDTypes { RelativePosition, CommadID }
        internal struct MenuItemInfo { public uint RelativePosition; public uint CommandID; public string Text; public string Command; }

        internal static Dictionary<uint, MenuItemInfo> GetMenuItemInfo(IntPtr menu, IContextMenu contextMenu)
        {
            Dictionary<uint, MenuItemInfo> retVal = new Dictionary<uint, MenuItemInfo>();

            int totalItems = ShellAPI.GetMenuItemCount(menu);
            for (int i = 0; i < totalItems; i++)
            {
                uint cmdID = ShellAPI.GetMenuItemID(menu, i);
                StringBuilder textsb = new StringBuilder(256);
                ShellAPI.GetMenuString(menu, (uint)i, textsb, textsb.Capacity, ShellAPI.MF_BYPOSITION);

                string command = "";
                if (cmdID > 0 && cmdID <= ShellAPI.CMD_LAST)
                {
                    command = ContextMenuHelper.GetCommandString(
                        contextMenu, cmdID - ShellAPI.CMD_FIRST,
                        true);
                }

                retVal.Add((uint)i, new MenuItemInfo() { RelativePosition = (uint)i, CommandID = cmdID, Text = textsb.ToString(), Command = command });
            }

            foreach (MenuItemInfo item in retVal.Values)
                Debug.WriteLine(String.Format("{0} - {1} {2} ({3})", item.RelativePosition, item.CommandID, item.Command, item.Text));

            return retVal;
        }

        internal static int GetMenuItemID(IntPtr menu, IContextMenu contextMenu, string lookupCommand, MenuIDTypes returnType)
        {
            Dictionary<uint, MenuItemInfo> menuItemInfo = GetMenuItemInfo(menu, contextMenu);
            for (uint i = 0; i < menuItemInfo.Count; i++)
                if (menuItemInfo[i].Command == lookupCommand)
                    return (int)(returnType == MenuIDTypes.RelativePosition ? i : menuItemInfo[i].CommandID);

            return -1;
        }
        internal static int[] GetMenuItemID(IntPtr menu, IContextMenu contextMenu, string[] lookupCommands, MenuIDTypes returnType)
        {
            Dictionary<uint, MenuItemInfo> menuItemInfo = GetMenuItemInfo(menu, contextMenu);

            List<int> retVal = new List<int>();
            foreach (string lookupCommand in lookupCommands)
            {
                int foundID = -1;
                for (uint i = 0; i < menuItemInfo.Count; i++)
                    if (menuItemInfo[i].Command == lookupCommand)
                    {
                        foundID = (int)(returnType == MenuIDTypes.RelativePosition ? i : menuItemInfo[i].CommandID);
                        break;
                    }
                retVal.Add(foundID);
            }
            return retVal.ToArray();
        }


        public enum DisabledMethods { Remove, Gray, Disable }
        public static void DisableMenuItems(IntPtr menu, IContextMenu contextMenu, string[] disabledCommands, DisabledMethods method)
        {
            if (disabledCommands == null || disabledCommands.Length == 0)
                return;

            foreach (int cmdID in GetMenuItemID(menu, contextMenu, disabledCommands, MenuIDTypes.CommadID))
                if (cmdID != -1)
                    switch (method)
                    {
                        case DisabledMethods.Remove:
                            ShellAPI.RemoveMenu(menu, (uint)cmdID, ShellAPI.MFT.BYCOMMAND);
                            break;
                        case DisabledMethods.Gray:
                            ShellAPI.EnableMenuItem(menu, (uint)cmdID, ShellAPI.MF_GRAYED);
                            break;
                        case DisabledMethods.Disable:
                            ShellAPI.EnableMenuItem(menu, (uint)cmdID, ShellAPI.MF_DISABLED);
                            break;
                    }
        }
        #endregion
    }
}


//public static class ContextMenuHelperEx
//{
//    public static List<string> GetCommands(FileSystemInfoEx[] items)
//    {
//        List<string> retVal = new List<string>();
//        ContextMenuWrapper wrapper = new ContextMenuWrapper();
//        wrapper.OnBeforePopup += (BeforePopupEventHandler)delegate(object s, BeforePopupEventArgs e)
//        {
//            for (uint cmdID = ShellAPI.CMD_FIRST; cmdID <= ShellAPI.CMD_LAST; cmdID++)
//            {
//                string command = ContextMenuHelper.GetCommandString(
//                    e.iContextMenu, cmdID,
//                    true);
//                if (command != "")
//                    retVal.Add(command);
//            }
//            e.ContinuePopup = false;
//        };
//        wrapper.Popup(items, new System.Drawing.Point(-100, -100));
//        return retVal;
//    }

//    public static List<string> GetCommands(FileSystemInfoEx item)
//    {
//        return GetCommands(new FileSystemInfoEx[] { item });
//    }

//}
//}

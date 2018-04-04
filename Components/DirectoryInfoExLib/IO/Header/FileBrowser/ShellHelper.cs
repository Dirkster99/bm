﻿namespace DirectoryInfoExLib.IO.Header.ShellDll
{
    using System;
    using DirectoryInfoExLib.IO.FileSystemInfoExt;
    using DirectoryInfoExLib.IO.Tools.Interface;

    internal static class ShellHelper
    {
        #region Low/High Word

        /// <summary>
        /// Retrieves the High Word of a WParam of a WindowMessage
        /// </summary>
        /// <param name="ptr">The pointer to the WParam</param>
        /// <returns>The unsigned integer for the High Word</returns>
        public static uint HiWord(IntPtr ptr)
        {
            if (((uint)ptr & 0x80000000) == 0x80000000)
                return ((uint)ptr >> 16);
            else
                return ((uint)ptr >> 16) & 0xffff;
        }

        /// <summary>
        /// Retrieves the Low Word of a WParam of a WindowMessage
        /// </summary>
        /// <param name="ptr">The pointer to the WParam</param>
        /// <returns>The unsigned integer for the Low Word</returns>
        public static uint LoWord(IntPtr ptr)
        {
            return (uint)ptr & 0xffff;
        }

        #endregion

        //#region IStream/IStorage

        //public static bool GetIStream(ShellItem item, out IntPtr streamPtr, out IStream stream)
        //{
        //    if (item.ParentItem.ShellFolder.BindToStorage(
        //                item.PIDLRel.Ptr,
        //                IntPtr.Zero,
        //                ref ShellAPI.IID_IStream,
        //                out streamPtr) == ShellAPI.S_OK)
        //    {
        //        stream = (IStream)Marshal.GetTypedObjectForIUnknown(streamPtr, typeof(IStream));
        //        return true;
        //    }
        //    else
        //    {
        //        stream = null;
        //        streamPtr = IntPtr.Zero;
        //        return false;
        //    }
        //}

        //public static bool GetIStorage(ShellItem item, out IntPtr storagePtr, out IStorage storage)
        //{
        //    if (item.ParentItem.ShellFolder.BindToStorage(
        //                item.PIDLRel.Ptr,
        //                IntPtr.Zero,
        //                ref ShellAPI.IID_IStorage,
        //                out storagePtr) == ShellAPI.S_OK)
        //    {
        //        storage = (IStorage)Marshal.GetTypedObjectForIUnknown(storagePtr, typeof(IStorage));
        //        return true;
        //    }
        //    else
        //    {
        //        storage = null;
        //        storagePtr = IntPtr.Zero;
        //        return false;
        //    }
        //}

        //#endregion

        #region Drag/Drop

        /// <summary>
        /// This method will use the GetUIObjectOf method of IShellFolder to obtain the IDataObject of a
        /// ShellItem. 
        /// </summary>
        /// <param name="item">The item for which to obtain the IDataObject</param>
        /// <param name="dataObjectPtr">A pointer to the returned IDataObject</param>
        /// <returns>the IDataObject the ShellItem</returns>
        public static IntPtr GetIDataObject(FileSystemInfoEx[] items)
        {
            DirectoryInfoEx parent = items[0].Parent != null ? items[0].Parent : DirectoryInfoEx.DesktopDirectory;
            IntPtr retVal = IntPtr.Zero;

            items.RequestPIDL((pidls, ptrs) =>
                {

                    IntPtr dataObjectPtr;
                    //0.15: Fixed ShellFolder not freed.
                    using (ShellFolder2 parentShellFolder = parent.ShellFolder)
                        if (parentShellFolder.GetUIObjectOf(
                                IntPtr.Zero,
                                (uint)ptrs.Length,
                                ptrs,
                                ref ShellAPI.IID_IDataObject,
                                IntPtr.Zero,
                                out dataObjectPtr) == ShellAPI.S_OK)
                        {
                            retVal = dataObjectPtr;
                        }

                });

            return retVal;
        }

        ///// <summary>
        ///// This method will use the GetUIObjectOf method of IShellFolder to obtain the IDropTarget of a
        ///// ShellItem. 
        ///// </summary>
        ///// <param name="item">The item for which to obtain the IDropTarget</param>
        ///// <param name="dropTargetPtr">A pointer to the returned IDropTarget</param>
        ///// <returns>the IDropTarget from the ShellItem</returns>
        //public static bool GetIDropTarget(ShellItem item, out IntPtr dropTargetPtr, out ShellDll.IDropTarget dropTarget)
        //{
        //    ShellItem parent = item.ParentItem != null ? item.ParentItem : item;

        //    if (parent.ShellFolder.GetUIObjectOf(
        //            IntPtr.Zero,
        //            1,
        //            new IntPtr[] { item.PIDLRel.Ptr },
        //            ref ShellAPI.IID_IDropTarget,
        //            IntPtr.Zero,
        //            out dropTargetPtr) == ShellAPI.S_OK)
        //    {
        //        dropTarget =
        //            (ShellDll.IDropTarget)Marshal.GetTypedObjectForIUnknown(dropTargetPtr, typeof(ShellDll.IDropTarget));

        //        return true;
        //    }
        //    else
        //    {
        //        dropTarget = null;
        //        dropTargetPtr = IntPtr.Zero;
        //        return false;
        //    }
        //}

        //public static bool GetIDropTargetHelper(out IntPtr helperPtr, out IDropTargetHelper dropHelper)
        //{
        //    if (ShellAPI.CoCreateInstance(
        //            ref ShellAPI.CLSID_DragDropHelper,
        //            IntPtr.Zero,
        //            ShellAPI.CLSCTX.INPROC_SERVER,
        //            ref ShellAPI.IID_IDropTargetHelper,
        //            out helperPtr) == ShellAPI.S_OK)
        //    {
        //        dropHelper =
        //            (IDropTargetHelper)Marshal.GetTypedObjectForIUnknown(helperPtr, typeof(IDropTargetHelper));

        //        return true;
        //    }
        //    else
        //    {
        //        dropHelper = null;
        //        helperPtr = IntPtr.Zero;
        //        return false;
        //    }
        //}

        //public static DragDropEffects CanDropClipboard(ShellItem item)
        //{
        //    IntPtr dataObject;
        //    ShellAPI.OleGetClipboard(out dataObject);

        //    IntPtr targetPtr;
        //    ShellDll.IDropTarget target;

        //    DragDropEffects retVal = DragDropEffects.None;
        //    if (GetIDropTarget(item, out targetPtr, out target))
        //    {
        //        #region Check Copy
        //        DragDropEffects effects = DragDropEffects.Copy;
        //        if (target.DragEnter(
        //            dataObject,
        //            ShellAPI.MK.CONTROL,
        //            new ShellAPI.POINT(0, 0),
        //            ref effects) == ShellAPI.S_OK)
        //        {
        //            if (effects == DragDropEffects.Copy)
        //                retVal |= DragDropEffects.Copy;

        //            target.DragLeave();
        //        }
        //        #endregion

        //        #region Check Move
        //        effects = DragDropEffects.Move;
        //        if (target.DragEnter(
        //            dataObject,
        //            ShellAPI.MK.SHIFT,
        //            new ShellAPI.POINT(0, 0),
        //            ref effects) == ShellAPI.S_OK)
        //        {
        //            if (effects == DragDropEffects.Move)
        //                retVal |= DragDropEffects.Move;

        //            target.DragLeave();
        //        }
        //        #endregion

        //        #region Check Lick
        //        effects = DragDropEffects.Link;
        //        if (target.DragEnter(
        //            dataObject,
        //            ShellAPI.MK.ALT,
        //            new ShellAPI.POINT(0, 0),
        //            ref effects) == ShellAPI.S_OK)
        //        {
        //            if (effects == DragDropEffects.Link)
        //                retVal |= DragDropEffects.Link;

        //            target.DragLeave();
        //        }
        //        #endregion

        //        Marshal.ReleaseComObject(target);
        //        Marshal.Release(targetPtr);
        //    }

        //    return retVal;
        //}

        #endregion
    }
}

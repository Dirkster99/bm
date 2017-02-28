///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// LYCJ (c) 2010 - http://www.quickzip.org/components                                                            //
// Release under LGPL license.                                                                                   //
//                                                                                                               //
// This code used part of Steven Roebert's work (http://www.codeproject.com/KB/miscctrl/FileBrowser.aspx)        //
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Tools;
using ShellDll;
using System.IO;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace System.IO.Tools
{
    public class DragWrapper : IDropSource, IDisposable
    {
        #region Variables
        private IntPtr dataObjectPtr; // The pointer to the IDataObject being dragged
        private MouseButtons startButton;  // The mouseButtons state when a drag starts
        private bool disposed = false;  // A bool to indicate whether this class has been disposed

        public event EventHandler BeforeDrop;
        #endregion

        public DragDropEffects StartDrag(MouseButtons startButton, FileSystemInfoEx[] selectedItems)
        {
            ReleaseCom();
            if (selectedItems.Length == 0)
                return DragDropEffects.None;
            this.startButton = startButton;
            dataObjectPtr = ShellHelper.GetIDataObject(selectedItems);
            if (dataObjectPtr != IntPtr.Zero)
            {
                DragDropEffects effects;
                ShellAPI.DoDragDrop(dataObjectPtr, this, DragDropEffects.Copy | 
                    DragDropEffects.Link /*| DragDropEffects.Move*/, out effects);
                return effects;
            }
            return DragDropEffects.None;
        }

        #region IDropSource Members

        public int QueryContinueDrag(bool fEscapePressed, ShellAPI.MK grfKeyState)
        {
            int retVal = ShellAPI.S_OK;

            if (fEscapePressed)
                retVal = ShellAPI.DRAGDROP_S_CANCEL;
            else
            {
                if ((startButton & MouseButtons.Left) != 0 && (grfKeyState & ShellAPI.MK.LBUTTON) == 0)
                    retVal = ShellAPI.DRAGDROP_S_DROP;
                else if ((startButton & MouseButtons.Right) != 0 && (grfKeyState & ShellAPI.MK.RBUTTON) == 0)
                    retVal = ShellAPI.DRAGDROP_S_DROP;
                else
                    retVal = ShellAPI.S_OK;
            }

            if (retVal == ShellAPI.DRAGDROP_S_DROP)
                if (BeforeDrop != null)
                    BeforeDrop(this, new EventArgs());

            return retVal;
        }

        public int GiveFeedback(System.Windows.Forms.DragDropEffects dwEffect)
        {
            return ShellAPI.DRAGDROP_S_USEDEFAULTCURSORS;
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// If not disposed, dispose the class
        /// </summary>
        public void Dispose()
        {
            if (!disposed)
            {
                ReleaseCom();
                GC.SuppressFinalize(this);

                disposed = true;
            }
        }

        ~DragWrapper()
        {
            ((IDisposable)this).Dispose();
        }

        /// <summary>
        /// Release the IDataObject and free's the allocated memory
        /// </summary>
        private void ReleaseCom()
        {
            if (dataObjectPtr != IntPtr.Zero)
            {
                Marshal.Release(dataObjectPtr);
                dataObjectPtr = IntPtr.Zero;
            }
        }
        #endregion
    }
}

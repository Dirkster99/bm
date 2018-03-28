using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using PreviewHandlerWPF;
using System.Diagnostics;
using ShellDll;

namespace System.IO.Tools
{
    public class PreviewerControl : Control
    {
        private static bool isVistaUp = Environment.OSVersion.Version.Major >= 6; //5 = XP, 6 = Vista

        FileInfoEx _file;          
        IPreviewHandler _previewHandler = null;

        public bool HandlerInitalized { get { return _previewHandler != null; } }

        public PreviewerControl(FileInfoEx file)
        {
            _file = file;
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            if (isVistaUp)
            {
                _previewHandler = PreviewHelper.InitalizePreviewHandler(_file);


                if (_previewHandler != null)
                {
                    ShellAPI.RECT r = new ShellAPI.RECT(this.ClientRectangle);
                    _previewHandler.SetWindow(this.Handle, ref r);
                    _previewHandler.SetRect(ref r);
                    _previewHandler.DoPreview();
                }
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (isVistaUp && _previewHandler != null)
            {
                ShellAPI.RECT r = new ShellAPI.RECT(this.ClientRectangle);
                _previewHandler.SetRect(ref r);
            }
        }

        protected override void OnControlRemoved(ControlEventArgs e)
        {
            base.OnControlRemoved(e);
            if (_previewHandler != null)
            {
                _previewHandler.Unload();
                _previewHandler = null;
            }
        }

    }
}

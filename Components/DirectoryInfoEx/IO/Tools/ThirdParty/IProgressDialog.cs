/*
 * CtrlSoft.Windows.Controls.WinProgressDialog
 * 
 * Copyright (c) 2004-2005
 * by Igor V. Velikorossov of CtrlSoft ( http://www.ctrlsoft.net )
 * All Rights Reserved
 *
 * The above copyright notice and this permission notice shall be included in all copies or substantial portions 
 * of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
 * TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
 * THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
 * CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
 * DEALINGS IN THE SOFTWARE.
 * 
 */ 
// http://www.dotnet247.com/247reference/msgs/55/276586.aspx
// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/csref/html/vcwlkCOMInteropPart1CClientTutorial.asp
//
// http://support.microsoft.com/default.aspx?scid=kb;en-us;823071
//

using System;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Text;

namespace CtrlSoft.Win.UI {

	//	#if (_WIN32_IE >= 0x0500)
	//	/// IProgressDialog
	//	// {F8383852-FCD3-11d1-A6B9-006097DF5BD4}
	//	DEFINE_GUID(CLSID_ProgressDialog,       0xf8383852, 0xfcd3, 0x11d1, 0xa6, 0xb9, 0x0, 0x60, 0x97, 0xdf, 0x5b, 0xd4);
	//	// {EBBC7C04-315E-11d2-B62F-006097DF5BD4}
	//	DEFINE_GUID(IID_IProgressDialog,        0xebbc7c04, 0x315e, 0x11d2, 0xb6, 0x2f, 0x0, 0x60, 0x97, 0xdf, 0x5b, 0xd4);
	//	#endif // _WIN32_IE >= 0x0500

	#region internal class ProgressDialog
	[ComImport]
	[Guid("F8383852-FCD3-11d1-A6B9-006097DF5BD4")] 
	internal class ProgressDialog {
	}
	#endregion // internal class ProgressDialog

	#region internal interface IProgressDialog
	[ComImport]
	[Guid("EBBC7C04-315E-11d2-B62F-006097DF5BD4")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IProgressDialog {

		[PreserveSig]
		void StartProgressDialog( 
			/*HWND */		IntPtr	hwndParent, 
			[MarshalAs(UnmanagedType.IUnknown)]
			/*IUnknown* */	object	punkEnableModless, 
			/*DWORD */		uint	dwFlags, 
			/*LPCVOID */	IntPtr	pvResevered
			);

		[PreserveSig]
		void StopProgressDialog();

		[PreserveSig]
		void SetTitle( 
			[MarshalAs(UnmanagedType.LPWStr)]
			/*LPCWSTR */	string pwzTitle 
			);

		[PreserveSig]
		void SetAnimation( 
			/*HINSTANCE */	IntPtr hInstAnimation, 
			/*UINT */		ushort idAnimation
			);

		[PreserveSig]
		[return: MarshalAs(UnmanagedType.Bool)]
		bool HasUserCancelled();

		[PreserveSig]
		void SetProgress( 
			/*DWORD */		uint dwCompleted, 
			/*DWORD */		uint dwTotal
			);
		[PreserveSig]
		void SetProgress64( 
			/*ULONGLONG */	ulong ullCompleted, 
			/*ULONGLONG */	ulong ullTotal
			);

		[PreserveSig]
		void SetLine(
			/*DWORD */		uint	dwLineNum, 
			[MarshalAs(UnmanagedType.LPWStr)]
			/*LPCWSTR */	string pwzString, 
			[MarshalAs(UnmanagedType.VariantBool)]
			/*BOOL */		bool	fCompactPath, 
			/*LPCVOID */	IntPtr	pvResevered
			);

		[PreserveSig]
		void SetCancelMsg( 
			[MarshalAs(UnmanagedType.LPWStr)]
			/*LPCWSTR */	string pwzCancelMsg, 
			/*LPCVOID */	object pvResevered
			);

		[PreserveSig]
		void Timer( 
			/*DWORD */		uint	dwTimerAction, 
			/*LPCVOID */	object	pvResevered
			);

	}
	#endregion // internal interface IProgressDialog

	#region public class WinProgressDialog : IDisposable
	/// <summary>
	/// .NET Wrapper for IProgressDialog interface and COM object
	/// </summary>
	/// <example>This is an example of how to invoke WinProgressDialog
	/// <code>
	/// WinProgressDialog pDialog;
	/// private uint _max, _step = 0;
	///
	/// private void button1_Click(object sender, System.EventArgs e) {
	/// 	this.pDialog = new WinProgressDialog( this.Handle );
	///
	/// 	if( this.pDialog != null ) {
	/// 		this.pDialog.Title			= "Hello world!";
	/// 		this.pDialog.CancelMessage	= "hold on a sec...";
	/// 		this.pDialog.Flags = 
	/// 			WinProgressDialog.IPD_Flags.Normal | 
	/// 			WinProgressDialog.IPD_Flags.Modal |
	/// 			WinProgressDialog.IPD_Flags.NoMinimize |
	/// 			WinProgressDialog.IPD_Flags.AutoTime
	/// 			;
	/// 		this.pDialog.Animation = WinProgressDialog.IPD_Animations.FileMove;
	///
	/// 		this.pDialog.Complete		= ( this._step = 0 );
	/// 		this.pDialog.Total			= ( this._max = DoCalc() );
	///
	/// 		this.pDialog.OnUserCancelled += 
	/// 			new WinProgressDialog.UserCancelledHandler( 
	/// 						pDialog_OnUserCancelled 
	/// 						);
	/// 		this.pDialog.OnBeforeProgressUpdate += 
	/// 			new WinProgressDialog.BeforeProgressUpdateHandler( 
	/// 						pDialog_OnBeforeProgressUpdate 
	/// 						);
	///
	/// 		WinProgressDialog.ProgressOperationCallback progressUpdate =
	/// 			new WinProgressDialog.ProgressOperationCallback( this.DoStep );
	///
	/// 		this.pDialog.Start( progressUpdate );
	/// 	}
	///
	/// 	this.pDialog.Dispose();
	/// }
	///
	/// private uint DoCalc() {
	/// 	// pretend to do some calc 
	/// 	System.Threading.Thread.Sleep( 2000 );
	/// 	// get some biggish number
	/// 	Random rand = new Random();
	/// 	return (uint)rand.Next( 150, 500 );
	/// }
	///
	/// private uint DoStep() {
	/// 	// pretend to do some calc
	/// 	System.Threading.Thread.Sleep( 250 );
	/// 	this._step += 13;
	/// 	return this._step;
	/// }
	///
	/// private void pDialog_OnUserCancelled(object sender, EventArgs e) {
	/// 	// pretend to do some calc 
	/// 	System.Threading.Thread.Sleep( 2000 );
	/// }
	///
	/// private void pDialog_OnBeforeProgressUpdate(object sender, EventArgs e) {
	/// 	this._step++;
	/// 	this.pDialog.SetLine( 
	/// 		WinProgressDialog.IPD_Lines.LineOne,
	/// 		"Line #1: step " + this._step.ToString(), 
	/// 		false
	/// 		);
	/// 	this.pDialog.SetLine( 
	/// 		WinProgressDialog.IPD_Lines.LineTwo,
	/// 		"Line #2: step " + this._step.ToString() + 
	/// 			". ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890", 
	/// 		true
	/// 		);
	///
	/// }
	///	</code>
	/// </example>
	public class WinProgressDialog : IDisposable {

		#region Enums / Consts
		/// <summary>
		/// Flags that control the operation of the progress dialog box
		/// </summary>
		/// <remarks>Flags for IProgressDialog::StartProgressDialog() (dwFlags)</remarks>
		[Flags]
			public enum IPD_Flags : uint {
			/// <summary>
			/// Default normal progress dlg behavior
			/// </summary>
			Normal          = 0x00000000,      // 
			/// <summary>
			/// The dialog is modal to its hwndParent (default is modeless)
			/// </summary>
			Modal           = 0x00000001,      // 
			/// <summary>
			/// automatically updates the "Line3" text with the "time remaining"
			/// </summary>
			/// <remarks>You cant call SetLine3 if you pass this!</remarks>
			AutoTime        = 0x00000002,      // 
			/// <summary>
			/// Do not show the "time remaining" if this is set. We need this if dwTotal &lt; dwCompleted for sparse files
			/// </summary>
			NoTime          = 0x00000004,      // 
			/// <summary>
			/// Do not have a minimize button in the caption bar
			/// </summary>
			NoMinimize      = 0x00000008,      // 
			/// <summary>
			/// Do not display the progress bar
			/// </summary>
			NoProgressBar   = 0x00000010       // 
		}

		/// <summary>
		/// File operation animations resource IDs in shell32.dll
		/// </summary>
		public enum IPD_Animations : ushort {
			FileMove				= 160,
			FileCopy				= 161,
			FlyingPapers			= 165,
			SearchGlobe				= 166,
			PermanentDelete			= 164,
			FromRecycleBinDelete	= 163,
			ToRecycleBinDelete		= 162,
			SearchComputer			= 152,
			SearchDocument			= 151,
			SearchFlashlight		= 150,
			Custom					= 0,
			NoAnimation				= ushort.MaxValue
		}

		/// <summary>
		/// Line numbers on which to display a message
		/// </summary>
		public enum IPD_Lines : byte {
			/// <summary>
			/// Top line of the dialog
			/// </summary>
			LineOne = 1,
			/// <summary>
			/// Middle line of the dialog
			/// </summary>
			LineTwo,
			/// <summary>
			/// Bottom line of the dialog (usually used for "time remaining")
			/// </summary>
			LineThree
		}

		/// <summary>
		/// Action to take when loading the module
		/// </summary>
		[Flags]
			private enum LoadLibraryExFlags : uint {
			DontResolveDllReferences	= 0x00000001,
			LoadLibraryAsDatafile		= 0x00000002,
			LoadWithAlteredSearchPath	= 0x00000008,
			LoadIgnoreCodeAuthzLevel	= 0x00000010
		}

		/// <summary>
		/// Time Actions (dwTimerAction)
		/// </summary>
		private	const uint	PDTIMER_RESET	= 0x00000001;
		/// <summary>
		/// Maximum message length
		/// </summary>
		private const int	MAX_CAPACITY	= 45;

		#endregion // Enums / Consts


		#region WinAPI imports
		// WinAPI imports
		[DllImport("shlwapi.dll", CharSet=CharSet.Auto)]
		static extern int PathCompactPathEx(
			[Out] 
			StringBuilder pszOut, 
			string	szPath, 
			int		cchMax, 
			int		dwFlags
			);

		[DllImport("kernel32",CharSet=CharSet.Auto,SetLastError=true)]
		static extern IntPtr LoadLibraryEx(
			[MarshalAs(UnmanagedType.LPTStr)] 
			string	lpFileName,
			IntPtr	hFile,
			LoadLibraryExFlags dwFlags
			);

		[DllImport("user32.dll")]
		static extern bool SetForegroundWindow(IntPtr hWnd);

		#endregion // WinAPI imports


		#region events and delegates
		/// <summary>
		/// Handler for <see cref="OnUserCancelled"/> event
		/// </summary>
		public delegate void UserCancelledHandler( object sender, EventArgs e );
		/// <summary>
		/// Event to be fired when a user cancels the dialog
		/// </summary> 
		public event UserCancelledHandler OnUserCancelled;

		/// <summary>
		/// Handler for <see cref="OnBeforeProgressUpdate"/> event
		/// </summary>
		public delegate void BeforeProgressUpdateHandler( object sender, EventArgs e );
		/// <summary>
		/// Event to be fired before <see cref="ProgressOperationCallback"/> function is called
		/// </summary> 
		public event BeforeProgressUpdateHandler OnBeforeProgressUpdate;

		/// <summary>
		/// Callback function which performs some activities and updates the progress information
		/// </summary>
		public delegate uint ProgressOperationCallback();

		#endregion // events and delegates


		private IProgressDialog		m_ipDialog		= null;
		private IPD_Flags			m_flags			= IPD_Flags.Normal;
		private IPD_Animations		m_animation		= IPD_Animations.NoAnimation;
		private IntPtr				m_hwnd			= IntPtr.Zero;
		private IntPtr				m_hinstance 	= IntPtr.Zero;
		private string				m_title			= string.Empty;
		private string				m_cancelMsg		= string.Empty;
		private ulong				m_dwComplete	= 0;
		private ulong				m_dwTotal		= 0;
		private bool				m_useULong		= false;
		private bool				m_dlgClosed		= true;
		private bool				m_disposed		= false;


		#region public WinProgressDialog( IntPtr hWnd )
		/// <summary>
		/// .ctor
		/// </summary>
		/// <param name="hWnd">Owner's handle</param>
		public WinProgressDialog( IntPtr hWnd ) {
			this.m_hwnd = hWnd;
			//	IProgressDialog * ppd;
			//	CoCreateInstance(CLSID_ProgressDialog, NULL, CLSCTX_INPROC_SERVER, IID_IProgressDialog, (void **)&ppd);
			ProgressDialog pDialog	= new ProgressDialog();
			this.m_ipDialog			= (IProgressDialog) pDialog;
		}
		/// <summary>
		/// Destructor
		/// </summary>
		~WinProgressDialog() {
			if( !this.m_disposed )
				this.Dispose();
		}
		#endregion // public WinProgressDialog( IntPtr hWnd )


		#region public IPD_Animations Animation
		/// <summary>
		/// Gets or sets an AVI clip that will run in the dialog box
		/// </summary>
		public IPD_Animations Animation {
			get{ return this.m_animation; }
			set{ 
				if( this.m_animation == value )
					return;
				this.m_animation = value;

				if( this.m_animation != IPD_Animations.NoAnimation ) {
					if( this.m_hinstance == IntPtr.Zero )
						this.m_hinstance = LoadLibraryEx( "shell32.dll\0", 
							IntPtr.Zero,
							LoadLibraryExFlags.LoadLibraryAsDatafile | 
							LoadLibraryExFlags.DontResolveDllReferences
							);

					if( this.m_hinstance == IntPtr.Zero )
						throw new Exception( "Failed to map shell32.dll resource" );

					// Set the title of the dialog.
					//	ppd->SetTitle(L"My Slow Operation");  
					this.m_ipDialog.SetAnimation( this.m_hinstance, (ushort)this.m_animation ); 
				}
				else 
					this.m_ipDialog.SetAnimation( IntPtr.Zero, 0 ); 
			}
		}
		#endregion // public IPD_Animations Animation

		#region public string CancelMessage
		/// <summary>
		/// Gets or sets a message to be displayed if the user cancels the operation
		/// </summary>
		public string CancelMessage {
			get{ return this.m_cancelMsg; }
			set{ 
				if( this.m_cancelMsg == value )
					return;
				this.m_cancelMsg = value;
				// null-terminate if required
				if( !this.m_cancelMsg.EndsWith("\0") )
					this.m_cancelMsg += '\0';
				// Will only be displayed if Cancel button is pressed.
				//	ppd->SetCancelMsg(L"Please wait while the current operation is cleaned up", NULL);
				this.m_ipDialog.SetCancelMsg( this.m_cancelMsg, null );
			}
		}
		#endregion // public string CancelMessage

		#region public uint Complete
		/// <summary>
		/// Gets or sets application-defined value that indicates 
		/// what proportion of the operation has been completed at 
		/// the time the method was called
		/// </summary>
		public uint Complete {
			get{ return (uint)this.m_dwComplete; }
			set{ 
				this.m_dwComplete	= value; 
				this.m_useULong		= false;
			}
		}
		/// <summary>
		/// Gets or sets application-defined value that indicates 
		/// what proportion of the operation has been completed at 
		/// the time the method was called
		/// </summary>
		/// <remarks>Allows the use of values larger than one DWORD (4 GB)</remarks>
		public ulong Complete64 {
			get{ return this.m_dwComplete; }
			set{ 
				this.m_dwComplete	= value; 
				this.m_useULong		= true;
			}
		}
		#endregion // public uint Complete

		#region public IPD_Flags Flags
		/// <summary>
		/// Gets or sets flags that control the operation of the progress dialog box
		/// </summary>
		public IPD_Flags Flags {
			get{ return this.m_flags; }
			set{ this.m_flags = value; }
		}
		#endregion // public IProgressDialogFlags Flags

		#region public string Title
		/// <summary>
		/// Gets or sets the title of the progress dialog box
		/// </summary>
		public string Title {
			get{ return this.m_title; }
			set{ 
				if( this.m_title == value )
					return;
				this.m_title = value;
				// null-terminate if required
				if( !this.m_title.EndsWith("\0") )
					this.m_title += "\0";
				// Set the title of the dialog.
				//	ppd->SetTitle(L"My Slow Operation");  
				this.m_ipDialog.SetTitle( this.m_title ); 
			}
		}
		#endregion // public string Title

		#region public uint Total
		/// <summary>
		/// Gets or sets application-defined value that specifies 
		/// what value <i>Complete</i> will have when the operation is complete
		/// </summary>
		public uint Total {
			get{ return (uint)this.m_dwTotal; }
			set{ 
				this.m_dwTotal	= value; 
				this.m_useULong	= false;
			}
		}
		/// <summary>
		/// Gets or sets application-defined value that specifies 
		/// what value <i>Complete</i> will have when the operation is complete
		/// </summary>
		/// <remarks>Allows the use of values larger than one DWORD (4 GB)</remarks>
		public ulong Total64 {
			get{ return this.m_dwTotal; }
			set{ 
				this.m_dwTotal	= value; 
				this.m_useULong	= true;
			}
		}
		#endregion // public uint Total


		#region public void SetLine( IPD_Lines line, string text, bool compactPath )
		/// <summary>
		/// Displays a message on a specified line
		/// </summary>
		/// <param name="line">Line on which a message to be displayed</param>
		/// <param name="text">Message's text</param>
		/// <param name="compactPath">Indicates whether a long text needs to be shortened</param>
		public void SetLine( IPD_Lines line, string text, bool compactPath ) {
			// compact path if required
			if( compactPath ) {
				StringBuilder sb = new StringBuilder( MAX_CAPACITY );
				PathCompactPathEx( sb, text, MAX_CAPACITY, 0 );
				text = sb.ToString();
				sb = null;
			}
			// null-terminate if required
			if( !text.EndsWith("\0") )
				text += "\0";
			//	ppd->SetLine(2, L"I'm processing item n", FALSE, NULL);
			this.m_ipDialog.SetLine( (uint)line, text, false, IntPtr.Zero ); 
		}
		#endregion // public void SetLine( IProgressDialogLines line, string text, bool compactPath )

		#region public void Start( ProgressOperationCallback callback )
		/// <summary>
		/// Starts the progress dialog box
		/// </summary>
		/// <param name="callback">Callback function which to be performed on each step</param>
		/// <remarks>This method automatically calls <see cref="Stop"/> upon <see cref="Complete"/> >= <see cref="Total"/></remarks>
		public void Start( ProgressOperationCallback callback ) {
			//	ppd->StartProgressDialog(hwndParent, punk, PROGDLG_AUTOTIME, NULL); // Display and enable automatic estimated time remaining.
			this.m_ipDialog.StartProgressDialog( 
				this.m_hwnd, 
				null, 
				(uint)( this.m_flags ), 
				IntPtr.Zero );

			this.m_dlgClosed = false;

			//	// Reset because CalcTotalUnitsToDo() took a long time and the estimated time
			//	// is based on the time between ::StartProgressDialog() and the first
			//	// ::SetProgress() call.
			//	ppd->Timer(PDTIMER_RESET, NULL);
			this.m_ipDialog.Timer( PDTIMER_RESET, null );

			//	for (nIndex = 0; nIndex < nTotal; nIndex++) {
			//for( ulong i=0; i<this.m_dwTotal; i++ ) {
			while( this.m_dwTotal > this.m_dwComplete ) {
				//
				//	ppd->SetLine(2, L"I'm processing item n", FALSE, NULL);
				if( OnBeforeProgressUpdate != null ) 
					OnBeforeProgressUpdate( this, EventArgs.Empty );

				//	dwComplete += DoSlowOperation();
				if( callback != null )
					this.m_dwComplete = callback();
				else
					this.m_dwComplete++;
				//
				//	ppd->SetProgress(dwCompleted, dwTotal);
				if( this.m_useULong ) 
					this.m_ipDialog.SetProgress64( this.m_dwComplete, this.m_dwTotal );
				else
					this.m_ipDialog.SetProgress( (uint)this.m_dwComplete, (uint)this.m_dwTotal );
				//	if (TRUE == ppd->HasUserCancelled())
				//		break;
				if( this.m_ipDialog.HasUserCancelled() ) {
					// raise an event if required
					if( OnUserCancelled != null ) {
						OnUserCancelled( this, EventArgs.Empty );
					}
					break;
				}
			}
			//}

			this.Stop();
		}
		#endregion // public void Start( ProgressOperationCallback callback )

		#region public void Stop()
		/// <summary>
		/// Stops the progress dialog box and removes it from the screen
		/// </summary>
		public void Stop() {
			if( this.m_dlgClosed )
				return;
			//	ppd->StopProgressDialog();
			this.m_ipDialog.StopProgressDialog();
			// looks like after the dialog is closed the owner window gets lost behind other windows
			//SetForegroundWindow( this.m_hwnd );
			// flag dialog as closed
			this.m_dlgClosed = true;
		}
		#endregion // public void Stop()


		#region IDisposable Members

		/// <summary>
		/// Disposes the object and releases resources used
		/// </summary>
		public void Dispose() {
			if( this.m_disposed )
				return;
			// make sure the dialog is closed
			if( !this.m_dlgClosed )
				this.Stop();
			this.m_ipDialog = null;
			this.m_disposed = true;
		}

		#endregion
	}
	#endregion // public class WinProgressDialog : IDisposable

}

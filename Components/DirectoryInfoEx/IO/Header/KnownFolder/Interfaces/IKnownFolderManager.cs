using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ShellDll
{
    [Guid("8BE2D872-86AA-4d47-B776-32CCA40C7018"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IKnownFolderManager
    {
        void FolderIdFromCsidl(int Csidl, [Out] out Guid knownFolderID);
        void FolderIdToCsidl([In, MarshalAs(UnmanagedType.LPStruct)] Guid id, [Out] out int Csidl);         
        void GetFolderIds([Out] out IntPtr folders,[Out] out UInt32 count);
        void GetFolder([In, MarshalAs(UnmanagedType.LPStruct)] Guid id, [Out, MarshalAs(UnmanagedType.Interface)] out IKnownFolder knownFolder);
        void GetFolderByName(string canonicalName, [Out, MarshalAs(UnmanagedType.Interface)] out IKnownFolder knowFolder);
        void RegisterFolder();  //([in] REFKNOWNFOLDERID rfid,[in] KNOWNFOLDER_DEFINITION const *pKFD);
        void UnregisterFolder(); //([in] REFKNOWNFOLDERID rfid);
        void FindFolderFromPath([In, MarshalAs(UnmanagedType.LPWStr)] string path, [In] KnownFolderFindMode mode, [Out, MarshalAs(UnmanagedType.Interface)] out IKnownFolder knownFolder);
        
        //void FindFolderFromIDList(IntPtr pidl, [Out, MarshalAs(UnmanagedType.Interface)] out IKnownFolder knownFolder);

        [PreserveSig]        
        int FindFolderFromIDList(IntPtr pidl, [Out, MarshalAs(UnmanagedType.Interface)] out IKnownFolder knownFolder);

        void Redirect(); //( [in] REFKNOWNFOLDERID rfid, [in, unique] HWND hwnd, [in] KF_REDIRECT_FLAGS flags,[in, unique, string] LPCWSTR pszTargetPath, [in] UINT cFolders, [in, size_is(cFolders), unique] KnownFolderID const *pExclusion,[out, string] LPWSTR* ppszError);
    }
}

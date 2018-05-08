namespace DirectoryInfoExLib.IO.Header.ShellDll
{
    using System;
    using System.Text;
    using System.Runtime.InteropServices;
    using System.Collections;

    /// <summary>
    /// The Shell namespace organizes the file system and other objects managed
    /// by the Shell into a single tree-structured hierarchy. Conceptually, it is
    /// a larger and more inclusive version of the file system.
    /// 
    /// https://msdn.microsoft.com/en-us/library/windows/desktop/cc144090(v=vs.85).aspx
    /// https://www.codeproject.com/Articles/88/Namespace-extensions-the-undocumented-Windows-Shel
    /// </summary>
    internal class PIDL : IEnumerable
    {
        //, IDisposable //11-01-09 : Added automatic disposer (LYCJ)
        //0.13 : Removed IDisposable in PIDL as it causing AccessViolationException, user have to free calling the Free() method.
        #region fields
        private static int Counter = 0;
        private IntPtr pidl = IntPtr.Zero;
        #endregion fields

        #region Constructors
        /// <summary>
        /// Class constructor from IntPtr
        /// </summary>
        /// <param name="pidl"></param>
        /// <param name="clone"></param>
        public PIDL(IntPtr pidl, bool clone)
        {
            System.Threading.Interlocked.Increment(ref Counter);
            if (clone)            
                this.pidl = ILClone(pidl);
            else
                this.pidl = pidl;
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="pidl"></param>
        /// <param name="clone"></param>
        public PIDL(PIDL pidl, bool clone)
        {
            if (clone)
            {
                this.pidl = ILClone(pidl.Ptr);
                System.Threading.Interlocked.Increment(ref Counter);
            }
            else
                this.pidl = pidl.Ptr;
        }
        #endregion

        #region Public
        /// <summary>
        /// Gets the item id list that describes the path of a shell object
        /// through an Item Id List (an ordered sequence of items).
        /// https://msdn.microsoft.com/en-us/library/windows/desktop/cc144090(v=vs.85).aspx
        /// </summary>
        public IntPtr Ptr { get { return pidl; } }

        /// <summary>
        /// Gets the size of an item id list that describes the path of a PIDL object.
        /// </summary>
        public int Size { get { return ItemIDListSize(Ptr); } }

        /// <summary>
        /// Appends the given <paramref name="appendPidl"/> into
        /// the current representation of this instances PIDL.
        /// </summary>
        /// <param name="appendPidl"></param>
        public void Append(IntPtr appendPidl)
        {
            IntPtr newPidl = ILCombine(pidl, appendPidl);

            Marshal.FreeCoTaskMem(pidl);
            pidl = newPidl;
        }

        /// <summary>
        /// Inserts the given <paramref name="insertPidl"/> at the
        /// beginning of the current representation of this instances PIDL.
        /// </summary>
        /// <param name="insertPidl"></param>
        public void Insert(IntPtr insertPidl)
        {
            IntPtr newPidl = ILCombine(insertPidl, pidl);

            Marshal.FreeCoTaskMem(pidl);
            pidl = newPidl;
        }

        /// <summary>
        /// Gets whether a given PIDL is empty
        /// (has no internal representation) or not.
        /// </summary>
        /// <param name="pidl"></param>
        /// <returns></returns>
        public static bool IsEmpty(IntPtr pidl)
        {
            if (pidl == IntPtr.Zero)
                return true;

            byte[] bytes = new byte[2];
            Marshal.Copy(pidl, bytes, 0, 2);
            int size = bytes[0] + bytes[1] * 256;

            return (size <= 2);
        }

        /// <summary>
        /// Writes a PIDL out to the console.
        /// </summary>
        /// <param name="pidl"></param>
        public static void Write(IntPtr pidl)
        {
            StringBuilder path = new StringBuilder(256);
            ShellAPI.SHGetPathFromIDList(pidl, path);

            Console.Out.WriteLine("Pidl: {0}", path);
        }

        /// <summary>
        /// Writes byte values of a PIDL out to the console.
        /// </summary>
        /// <param name="pidl"></param>
        public static void WriteBytes(IntPtr pidl)
        {
            int size = Marshal.ReadByte(pidl, 0) + Marshal.ReadByte(pidl, 1) * 256 - 2;

            for (int i = 0; i < size; i++)
            {
                Console.Out.WriteLine(Marshal.ReadByte(pidl, i + 2));
            }

            Console.Out.WriteLine(Marshal.ReadByte(pidl, size + 2));
            Console.Out.WriteLine(Marshal.ReadByte(pidl, size + 3));
        }

        /// <summary>
        /// Frees the internal ITEMIDLIST structure to support disposing
        /// of allocated memory.
        /// </summary>
        public void Free()
        {
            if (pidl != IntPtr.Zero)
            {
                System.Threading.Interlocked.Decrement(ref Counter);
                Marshal.FreeCoTaskMem(pidl);
                pidl = IntPtr.Zero;
            }
        }
        #endregion

        #region Shell
        /// <summary>
        /// Clones an ITEMIDLIST structure
        /// </summary>
        /// <param name="pidl"></param>
        /// <returns></returns>
        public static IntPtr ILClone(IntPtr pidl)
        {
            int size = ItemIDListSize(pidl);

            byte[] bytes = new byte[size + 2];
            Marshal.Copy(pidl, bytes, 0, size);

            IntPtr newPidl = Marshal.AllocCoTaskMem(size + 2);
            Marshal.Copy(bytes, 0, newPidl, size + 2);

            return newPidl;
        }

        /// <summary>
        /// Clones the first SHITEMID structure in an ITEMIDLIST structure
        /// </summary>
        /// <param name="pidl"></param>
        /// <returns></returns>
        public static IntPtr ILCloneFirst(IntPtr pidl)
        {
            int size = ItemIDSize(pidl);

            byte[] bytes = new byte[size + 2];
            Marshal.Copy(pidl, bytes, 0, size);

            IntPtr newPidl = Marshal.AllocCoTaskMem(size + 2);
            Marshal.Copy(bytes, 0, newPidl, size + 2);

            return newPidl;
        }

        /// <summary>
        /// Gets the next SHITEMID structure in an ITEMIDLIST structure
        /// </summary>
        /// <param name="pidl"></param>
        /// <returns></returns>
        public static IntPtr ILGetNext(IntPtr pidl)
        {
            int size = ItemIDSize(pidl);
            IntPtr nextPidl = new IntPtr((Int64)pidl + size);
            return nextPidl;
        }

        /// <summary>
        /// Returns a pointer to the last SHITEMID structure in an ITEMIDLIST structure
        /// </summary>
        /// <param name="pidl"></param>
        /// <returns></returns>
        public static IntPtr ILFindLastID(IntPtr pidl)
        {
            IntPtr ptr1 = pidl;
            IntPtr ptr2 = ILGetNext(ptr1);

            while (ItemIDSize(ptr2) > 0)
            {
                ptr1 = ptr2;
                ptr2 = ILGetNext(ptr1);
            }

            return ptr1;
        }

        /// <summary>
        /// Removes the last SHITEMID structure from an ITEMIDLIST structure
        /// </summary>
        /// <param name="pidl"></param>
        /// <returns>true if there was a last pidl to remove, false otheerwise.</returns>
        public static bool ILRemoveLastID2(ref IntPtr pidl)
        {
            IntPtr lastPidl = ILFindLastID(pidl);

            if (lastPidl != pidl)
            {
                int newSize = (int)((Int64)lastPidl - (Int64)pidl + 2);
                IntPtr newPidl = Marshal.AllocCoTaskMem(newSize);
                byte[] bytes = new byte[newSize];

                Marshal.Copy(pidl, bytes, 0, newSize - 2);
                Marshal.Copy(bytes, 0, newPidl, bytes.Length);
                Marshal.FreeCoTaskMem(pidl);
                pidl = newPidl;

                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Combines two ITEMIDLIST structures
        /// </summary>
        /// <param name="pidl1"></param>
        /// <param name="pidl2"></param>
        /// <returns></returns>
        public static IntPtr ILCombine(IntPtr pidl1, IntPtr pidl2)
        {
            int size1 = ItemIDListSize(pidl1);
            int size2 = ItemIDListSize(pidl2);

            IntPtr newPidl = Marshal.AllocCoTaskMem(size1 + size2 + 2);
            byte[] bytes = new byte[size1 + size2 + 2];

            Marshal.Copy(pidl1, bytes, 0, size1);
            Marshal.Copy(pidl2, bytes, size1, size2);

            Marshal.Copy(bytes, 0, newPidl, bytes.Length);

            return newPidl;
        }

        /// <summary>
        /// Copies the ITEMIDLIST structure into the given
        /// <see cref="IntPtr"/> <paramref name="pidl"/> parameter
        /// if it appears to be <see cref="IntPtr.Zero"/>
        /// and returns the size of the copy as int.
        /// 
        /// Otherwise, if <paramref name="pidl"/> is already set
        /// (<paramref name="pidl"/> != <see cref="IntPtr.Zero"/>),
        /// there is no copy and 0 is returned.
        /// </summary>
        private static int ItemIDSize(IntPtr pidl)
        {
            if (pidl.Equals(IntPtr.Zero) == false)
            {
                byte[] buffer = new byte[2];
                Marshal.Copy(pidl, buffer, 0, 2);

                return buffer[1] * 256 + buffer[0];
            }
            else
                return 0;
        }

        /// <summary>
        /// Returns the size of an ItemIDList in bytes.
        /// </summary>
        /// <param name="pidl"></param>
        /// <returns></returns>
        private static int ItemIDListSize(IntPtr pidl)
        {
            if (pidl.Equals(IntPtr.Zero))
                return 0;
            else
            {
                int size = ItemIDSize(pidl);
                int nextSize = Marshal.ReadByte(pidl, size) + (Marshal.ReadByte(pidl, size + 1) * 256);
                while (nextSize > 0)
                {
                    size += nextSize;
                    nextSize = Marshal.ReadByte(pidl, size) + (Marshal.ReadByte(pidl, size + 1) * 256);
                }

                return size;
            }
        }
        #endregion

        #region IEnumerable Members
        /// <summary>
        /// Gets an enumerator over all SHITEMID structures
        /// in an ITEMIDLIST structure of a PIDL.
        /// </summary>
        /// <returns></returns>
        public IEnumerator GetEnumerator()
        {
            return new PIDLEnumerator(pidl);
        }

        #endregion

        #region Override
        /// <summary>
        /// Determines if thé given object is equal to this PIDL or not.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            try
            {
                if (obj is IntPtr)
                    return ShellAPI.ILIsEqual(this.Ptr, (IntPtr)obj);

                if (obj is PIDL)
                    return ShellAPI.ILIsEqual(this.Ptr, ((PIDL)obj).Ptr);
                else
                    return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the HashCode for this PIDL object.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return pidl.GetHashCode();
        }

        #endregion

        #region private classes
        /// <summary>
        /// Implements an enumerator over all SHITEMID structures
        /// SHITEMID structure in an ITEMIDLIST structure
        /// </summary>
        private class PIDLEnumerator : IEnumerator
        {
          #region fields
            private IntPtr _pidl;
            private IntPtr _currentPidl;
            private IntPtr _clonePidl;
            private bool _start;
          #endregion fields

          #region constructors
            /// <summary>
            /// Class constructor
            /// </summary>
            public PIDLEnumerator(IntPtr pidl)
            {
                _start = true;
                _pidl = pidl;
                _currentPidl = _pidl;
                _clonePidl = IntPtr.Zero;
            }
          #endregion constructors

          #region IEnumerator Members
            /// <summary>
            /// Gets the current item from the ItemIdList. 
            /// </summary>
            public object Current
            {
                get
                {
                    if (_clonePidl != IntPtr.Zero)
                    {
                        Marshal.FreeCoTaskMem(_clonePidl);
                        _clonePidl = IntPtr.Zero;
                    }

                    _clonePidl = PIDL.ILCloneFirst(_currentPidl);

                    return _clonePidl;
                }
            }

            public bool MoveNext()
            {
                if (_clonePidl != IntPtr.Zero)
                {
                    Marshal.FreeCoTaskMem(_clonePidl);
                    _clonePidl = IntPtr.Zero;
                }

                if (_start)
                {
                    _start = false;
                    return true;
                }
                else
                {
                    IntPtr newPidl = ILGetNext(_currentPidl);

                    if (!PIDL.IsEmpty(newPidl))
                    {
                        _currentPidl = newPidl;
                        return true;
                    }
                    else
                        return false;
                }
            }

            public void Reset()
            {
                _start = true;
                _currentPidl = _pidl;

                if (_clonePidl != IntPtr.Zero)
                {
                    Marshal.FreeCoTaskMem(_clonePidl);
                    _clonePidl = IntPtr.Zero;
                }
            }

            #endregion
        }
        #endregion private classes
    }
}

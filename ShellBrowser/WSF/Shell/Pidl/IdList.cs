namespace WSF.Shell.Pidl
{
    using WSF.Shell.Interop.Knownfolders;
    using WSF.Shell.Interop.ShellFolders;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Reprents a Shell ID list that can be used to replace <see cref="IntPtr"/> based
    /// values. This replacement can be useful since the <see cref="IdList"/> does not
    /// require additional memory management or <see cref="IDisposable"/> interface logics.
    /// 
    /// Used with the <see cref="PidlManager" /> and various other classes.
    /// See <see cref="PidlManager.PidlToIdlist(System.IntPtr)" /> and
    /// <see cref="PidlManager.IdListToPidl(IdList)"/> for more details on conversion.
    /// 
    /// https://docs.microsoft.com/de-de/windows/desktop/api/shtypes/ns-shtypes-_itemidlist
    /// 
    /// Source: SharpShell on GitHub - https://github.com/dwmkerr/sharpshell
    /// </summary>
    public sealed class IdList
    {
        #region fields
        /// <summary>
        /// The ids.
        /// </summary>
        private readonly List<ShellId> ids;
        #endregion fields

        #region ctors
        /// <summary>
        /// Creates an IdList.
        /// </summary>
        /// <param name="ids">The ids.</param>
        /// <returns></returns>
        internal static IdList Create(IList<ShellId> ids = null)
        {
            return new IdList(ids);
        }

        /// <summary>
        /// Prevents a default instance of the <see cref="IdList"/> class from being created.
        /// </summary>
        /// <param name="ids">The ids.</param>
        private IdList(IList<ShellId> ids)
        {
            if (ids != null)
                this.ids = new List<ShellId>(ids);
            else
                this.ids = new List<ShellId>();
        }
        #endregion ctors
        
        #region properties
        /// <summary>
        /// Gets the ids.
        /// </summary>
        public ReadOnlyCollection<ShellId> Ids { get { return ids.AsReadOnly(); } }

        /// <summary>
        /// Gets the size of the internal <seealso cref="ShellId"/> collection.
        /// </summary>
        public int Size
        {
            get
            {
                if (Ids == null)
                    return 0;

                return Ids.Count;
            }
        }
        #endregion properties

        #region methods
        /// <summary>
        /// Converts an idlist to a parsing string.
        /// </summary>
        /// <returns>The id list in parsing string format.</returns>
        public string ToParsingString()
        {
            var sb = new StringBuilder(ids.Sum(id => id.Length*2 + 4));
            foreach (var id in ids)
            {
                sb.AppendFormat("{0:x4}", (short)id.Length);
                foreach(var idi in id.RawId)
                    sb.AppendFormat("{0:x2}", idi);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Creates an idlist from parsing string format.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <returns>The idlist represented by the string.</returns>
        public static IdList FromParsingString(string str)
        {
            //  Create the id storage.
            var ids = new List<ShellId>();

            //  Repeatedly read a short length then the data.
            int index = 0;
            while (index < str.Length)
            {
                var length = Convert.ToInt16(str.Substring(index, 4), 16);
                var id = new byte[length];
                index += 4;
                for (var i = 0; i < length; i++, index += 2)
                    id[i] = Convert.ToByte(str.Substring(index, 2), 16);

                ids.Add(ShellId.FromData(id));
            }

            //  Return the list.
            return new IdList(ids);
        }

        /// <summary>
        /// Returns an <see cref="IdList"/> (PIDL) for a known folder given a
        /// globally unique identifier.
        /// </summary>
        /// <param name="knownfolderId">A GUID for the requested known folder.</param>
        /// <returns>A known folder representing the specified name.</returns>
        /// <exception cref="System.ArgumentException">Thrown if the given Known Folder ID is invalid.</exception>
        public static IdList FromKnownFolderGuid(Guid knownfolderId)
        {
            using (KnownFolderNative KF = KnownFolderHelper.FromKnownFolderGuid(knownfolderId))
            {
                if (KF != null)
                {
                    IntPtr parentPidl = default(IntPtr);
                    try
                    {
                        parentPidl = KF.KnownFolderToPIDL();

                        // Convert PIDL into list of shellids and remove last id
                        return IdList.Create(PidlManager.Decode(parentPidl));
                    }
                    finally
                    {
                        parentPidl = PidlManager.ILFree(parentPidl);
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Determines whether two idlists are equal.
        /// </summary>
        /// <param name="idList">The ID list to compare against.</param>
        /// <returns>True if the id lists are equal, false otherwise.</returns>
        public bool Matches(IdList idList)
        {
            //  We must have a valid set to match against.
            if (idList == null || idList.ids == null || idList.ids.Count != ids.Count)
                return false;
            
            //  If there is any id that doesn't match, we return false.
            return !ids.Where((t, i) => !t.Equals(idList.ids[i])).Any();
        }

        /// <summary>
        /// Get Relative Child Id which is always the last id in the sequence
        /// </summary>
        /// <returns></returns>
        public IdList GetRelativeChildId()
        {
            if (Ids == null)
                return null;

            if (Ids.Count <= 0)
                return null;

            return new IdList(new List<ShellId>() { Ids.Last() });
        }

        /// <summary>
        /// Get Parent Id which is always the first part
        /// minus last (child) id in the sequence.
        /// </summary>
        /// <returns>ParentId or null if the id list
        /// contains only 1 or less items (parent is unknown).</returns>
        public IdList GetParentId()
        {
            if (Size <= 1)
                return null;

            IdList parentList = null;

            var parent = new List<ShellId>();

            for (int i = 0; i < Ids.Count - 1; i++)
                parent.Add(ShellId.FromData(Ids[i].RawId));

            parentList = IdList.Create(parent);

            return parentList;
        }

        /// <summary>
        /// Standard override for debugging use.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return ToParsingString();
        }
        #endregion methods
    }
}
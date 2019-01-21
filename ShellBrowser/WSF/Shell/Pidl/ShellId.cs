namespace WSF.Shell.Pidl
{
    using System;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// A ShellId is a representation of a shell item that makes up an id list.
    /// Simply put, a ShellId names a Shell Item uniquely in the context of it's
    /// parent Shell Folder.
    /// 
    /// Source: SharpShell on GitHub - https://github.com/dwmkerr/sharpshell
    /// </summary>
    public class ShellId
    {
        #region fields
        /// <summary>
        /// The identifier.
        /// </summary>
        private readonly byte[] _id;
        #endregion fields

        #region constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ShellId"/> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        internal ShellId(byte[] id)
        {
            this._id = id;
        }

        /// <summary>
        /// Creates a new Shell ID from a string.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <returns>A new Shell ID from the given string.</returns>
        public static ShellId FromString(string str)
        {
            return new ShellId(Encoding.UTF8.GetBytes(str));
        }

        /// <summary>
        /// Creates a Shell ID frm raw data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns>A new Shell ID from the given data.</returns>
        /// <exception cref="System.NullReferenceException">'data' cannot be null.</exception>
        /// <exception cref="System.InvalidOperationException">'data' must contain data.</exception>
        public static ShellId FromData(byte[] data)
        {
            if (data == null)
                throw new NullReferenceException("'data' cannot be null.");

            if (data.Length == 0)
                throw new InvalidOperationException("'data' must contain data.");

            return new ShellId(data);
        }
        #endregion constructors

        #region methods

        #endregion methods
        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            //  Write the bytes of the ID and the string if we can get one.
            var bytesString = BitConverter.ToString(_id.ToArray());
            string ascii, utf8;
            try
            {
                ascii = Encoding.ASCII.GetString(_id);
            }
            catch
            {
                ascii = "[Undecodable]";
            }
            try
            {
                utf8 = Encoding.UTF8.GetString(_id.ToArray());
            }
            catch
            {
                utf8 = "[Undecodable]";
            }

            return string.Format("{0} ASCII {1} UTF8 {2}", bytesString, ascii, utf8);
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
              
            var rhs = obj as ShellId;
            if (rhs == null)
                return false;
              
            if (Length != rhs.Length)
                return false;
              
            for(int i=0;i<Length;i++)
            {
                if (_id[i] != rhs._id[i])
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return _id.GetHashCode();
        }

        /// <summary>
        /// Gets the raw identifier.
        /// </summary>
        public byte[] RawId { get { return _id; }}

        /// <summary>
        /// Gets the length of the identifier.
        /// </summary>
        public int Length {get { return _id.Length; }}

        /// <summary>
        /// Gets a UTF8 encoded string that is human readable (although its hex number display)
        /// and denotes the contents of this object.
        /// </summary>
        /// <returns></returns>
        public string AsUTF8()
        {
            var utf8 = string.Empty;
            try
            {
                utf8 = Encoding.UTF8.GetString(_id.ToArray());
            }
            catch
            {
                utf8 = "[Undecodable]";
            }
            
            return utf8;
        }

        /// <summary>
        /// Gets an ASCII encoded string that is human readable (although its hex number display)
        /// and denotes the contents of this object.
        /// </summary>
        /// <returns></returns>
        public string AsASCII()
        {
            var utf8 = string.Empty;
            try
            {
                utf8 = Encoding.ASCII.GetString(_id.ToArray());
            }
            catch
            {
                utf8 = "[Undecodable]";
            }
            
            return utf8;
        }
    }
}
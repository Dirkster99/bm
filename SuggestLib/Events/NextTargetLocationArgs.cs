namespace SuggestLib.Events
{
    using System;

    public class NextTargetLocationArgs : EventArgs
    {
        #region ctors
        /// <summary>
        /// Parameterized standard constructor
        /// </summary>
        /// <param name="oldLocation"></param>
        /// <param name="newLocation"></param>
        public NextTargetLocationArgs(string oldLocation, string newLocation)
            : this()
        {
            this.OldLocation = oldLocation;
            this.NewLocation = newLocation;
        }

        /// <summary>
        /// Hidden standard constructor
        /// </summary>
        protected NextTargetLocationArgs()
        {
        }
        #endregion ctors

        public string OldLocation { get; }

        public string NewLocation { get; }
    }
}

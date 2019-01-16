namespace SuggestLib.Events
{
    using System;

    /// <summary>
    /// Determines whether the textual editing of the path was:
    /// 1) Cancell'ed (eg: user pressed Escape key -> rollback to previous location) or
    /// 2) OK'ed (eg: user pressed Enter key -> tree view should shown new location)
    /// </summary>
    public enum EditPathResult
    {
        /// <summary>
        /// The process of changing location has been cancelled:
        /// The application should stay at the previous known location.
        /// </summary>
        Cancel = 0,

        /// <summary>
        /// The process of changing location has been OK'ed:
        /// The application should change to the new location to
        /// reflect the selection.
        /// </summary>
        OK = 1
    }

    /// <summary>
    /// Implements a simple class to store what the new loaction of
    /// a path edit workflow is and whether editing was cancell'ed or OK'ed.
    /// </summary>
    public class EditResult
    {
        #region ctors
        /// <summary>
        /// Parameterized standard constructor
        /// </summary>
        /// <param name="result"></param>
        /// <param name="newLocation"></param>
        public EditResult(
            EditPathResult result,
            string newLocation)
            : this()
        {
            this.Result = result;
            this.NewLocation = newLocation;
        }

        /// <summary>
        /// Hidden standard constructor
        /// </summary>
        protected EditResult()
        {
        }
        #endregion ctors

        /// <summary>
        /// Gets whether the path editing should be canceled or not.
        /// </summary>
        public EditPathResult Result { get; }

        /// <summary>
        /// Gets the new path location.
        /// </summary>
        public string NewLocation { get; }
    }

    /// <summary>
    /// Class implements an event that tells subscribers
    /// when the user has finished edinting a path and what
    /// the result of that editing is.
    /// </summary>
    public class NextTargetLocationArgs : EventArgs
    {
        #region ctors
        /// <summary>
        /// Parameterized standard constructor
        /// </summary>
        /// <param name="editResult"></param>
        public NextTargetLocationArgs(EditResult editResult)
            : this()
        {
            this.EditResult = editResult;
        }

        /// <summary>
        /// Hidden standard constructor
        /// </summary>
        protected NextTargetLocationArgs()
        {
        }
        #endregion ctors

        /// <summary>
        /// Gets the result of the path editing workflow.
        /// </summary>
        public EditResult EditResult { get; }
    }
}

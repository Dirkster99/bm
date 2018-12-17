namespace SSCoreLib.Browse
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public enum RequestType
    {
        Navigational = 0,

        RefreshChildrenBelowItem = 1,

        RefreshItem = 2
    }

    /// <summary>
    /// Class models a request to browse to a certain location (path)
    /// in the file system. A controller should use this class to formulate
    /// a request and Id the corresponding result using the BrowsingEvent
    /// and RequestId property.
    /// </summary>
    public class BrowseRequest<M>
    {
        #region ctors
        /// <summary>
        /// Parameterized class constructor.
        /// </summary>
        public BrowseRequest(M newLocation,
                             RequestType typeOfRequest,
                             CancellationToken cancelToken = default(CancellationToken),
                             CancellationTokenSource cancelTokenSource = null,
                             Task t = null)
          : this()
        {
            NewLocation = (M)(newLocation as ICloneable).Clone();
            ActionRequested = typeOfRequest;
            CancelTok = cancelToken;
            CancelTokenSource = cancelTokenSource;
            BrowseTask = t;
        }

        public BrowseRequest(M[] locationsPath,
                             RequestType typeOfRequest,
                             CancellationToken cancelToken = default(CancellationToken),
                             CancellationTokenSource cancelTokenSource = null,
                             Task t = null)
          : this()
        {
            NewLocation = (M)(locationsPath[locationsPath.Length-1] as ICloneable).Clone();

            LocationsPath = new M[locationsPath.Length];
            for (int i = 0; i < locationsPath.Length; i++)
                LocationsPath[i] = (M)(locationsPath[i] as ICloneable).Clone();

            ActionRequested = typeOfRequest;
            CancelTok = cancelToken;
            CancelTokenSource = cancelTokenSource;
            BrowseTask = t;
        }


        /// <summary>
        /// Class constructor.
        /// </summary>
        protected BrowseRequest()
        {
            RequestId = Guid.NewGuid();
        }
        #endregion ctors

        #region properties
        /// <summary>
        /// Gets the new location (a path in the file system) to indicate
        /// the target of this browse request.
        /// </summary>
        public M NewLocation { get; }

        public M[] LocationsPath { get; }

        public RequestType ActionRequested { get; }

        /// <summary>
        /// Gets the CancelationToken (if any) that can be used by the requesting
        /// process to cancel this request during its processing (on timeout or by user).
        /// </summary>
        public CancellationToken CancelTok { get; }

        public CancellationTokenSource CancelTokenSource { get; }

        /// <summary>
        /// The task that fullfills this request (if any)
        /// </summary>
        public Task BrowseTask { get; protected set; }

        /// <summary>
        /// Gets the Id that identifies this request among all other requests that may
        /// occur if multiple browse requests are initiated or if user interaction also
        /// causes additional browse processing...
        /// </summary>
        public Guid RequestId { get; }

        /// <summary>
        /// Determines if we have a list of path items in <see cref="LocationsPath"/>
        /// (these are elements from a previously confirmed path) or
        /// 
        /// not: (we just have a string - know the destionation exists -
        /// but still have to do the parsing
        /// and conversion into model and viewmodel items.
        /// </summary>
        public bool PathConfirmed
        {
            get
            {
                if (LocationsPath == null)
                    return false;

                if (LocationsPath.Length <= 0)
                    return false;

                return true;
            }
        }
        #endregion properties

        #region methods
        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return string.Format("RequestId: '{0}', NewLocation: {1}", RequestId, NewLocation);
        }

        /// <summary>
        /// Set the task after construction if the task requires the browse request and the
        /// browse request requires the task ...
        /// </summary>
        /// <param name="myWorkTask"></param>
        public void SetTask(Task myWorkTask)
        {
            BrowseTask = myWorkTask;
        }
        #endregion methods
    }
}

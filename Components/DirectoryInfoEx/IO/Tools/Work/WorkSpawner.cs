///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// LYCJ (c) 2010 - http://www.quickzip.org/components                                                            //
// Release under LGPL license.                                                                                   //
//                                                                                                               //
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Diagnostics;

namespace System.IO.Tools
{
    #region Events
    public class WorkAddedEventArgs : CancelEventArgs
    {
        public int ID { get; private set; }
        public IWork Work { get; private set; }

        public WorkAddedEventArgs(int id, IWork Work)
        {
            this.ID = id;
            this.Work = Work;
        }
    }

    public delegate void WorkAddedEventHandler(object sender, WorkAddedEventArgs e);    
    #endregion

    public class WorkSpawner
    {
        protected static Dictionary<int, IWork> onGoingWorkList = new Dictionary<int, IWork>();
        protected static Dictionary<int, IWork> fullWorkList = new Dictionary<int, IWork>();
        protected static Dictionary<int, OverwriteMode> defaultOverwriteModeList = new Dictionary<int, OverwriteMode>();
        protected static List<int> muteList = new List<int>();
        protected static Random rand = new Random(0814);        
        public static EventHandler MonitorWorkListUpdated;
        public static event WorkAddedEventHandler MonitorWorkListAdded;
        public static event WorkProgressEventHandler WorkProgress;
        public static event WorkStartEventHandler WorkStart;
        public static event WorkFinishedEventHandler WorkFinished;
        public static event WorkOverwriteEventHandler WorkOverwrite;
        public static event WorkListEventHandler WorkList;
        public static event WorkMessageEventHandler WorkMessage;

        #region Spawn New Work
        protected static int newKey()
        {
            int key = rand.Next();
            while (onGoingWorkList.ContainsKey(key))
                key = rand.Next();
            return key;
        }

        protected static int addWork(int key, IWork Work)
        {
            fullWorkList.Add(key, Work);
            if (MonitorWorkListAdded != null)
                MonitorWorkListAdded(null, new WorkAddedEventArgs(key, Work));
            return key;
        }

        #region SpawnWork
        public static int SpawnListWork(DirectoryInfoEx[] dirs, bool listDir, bool listFile, string fileMask, bool showProgressDialog)
        {
            lock (onGoingWorkList)
            {
                IExWork retVal = new ListWork(newKey(), dirs, listDir, listFile, fileMask);
                retVal.IsProgressDialogEnabled = showProgressDialog;
                hookWorkEvent(retVal);
                return addWork(retVal.ID, retVal);
            }
        }


        public static int SpawnCopyWork(FileSystemInfoEx[] src, DirectoryInfoEx dest, bool showProgressDialog)
        {
            lock (onGoingWorkList)
            {
                IExWork retVal = new CopyWork(newKey(), src, dest);
                retVal.IsProgressDialogEnabled = showProgressDialog;
                hookWorkEvent(retVal);
                return addWork(retVal.ID, retVal);
            }
        }

        public static int SpawnMoveWork(FileSystemInfoEx[] src, DirectoryInfoEx dest, bool showProgressDialog)
        {
            lock (onGoingWorkList)
            {
                IExWork retVal = new MoveWork(newKey(), src, dest);
                retVal.IsProgressDialogEnabled = showProgressDialog;
                hookWorkEvent(retVal);
                return addWork(retVal.ID, retVal);
            }
        }

        public static int SpawnDeleteWork(FileSystemInfoEx[] items, bool showProgressDialog)
        {
            lock (onGoingWorkList)
            {
                IExWork retVal = new DeleteWork(newKey(), items);
                retVal.IsProgressDialogEnabled = showProgressDialog;
                hookWorkEvent(retVal);
                return addWork(retVal.ID, retVal);
            }
        }
        #endregion



        #endregion

        #region Work Event

        protected static void hookWorkEvent(IExWork work)
        {
            work.WorkProgress += new WorkProgressEventHandler(OnWorkProgress);
            work.WorkStart += new WorkStartEventHandler(retVal_WorkStart);
            work.WorkFinished += new WorkFinishedEventHandler(retVal_WorkFinished);
            work.WorkMessage += new WorkMessageEventHandler(OnWorkMessage);
            work.WorkList += new WorkListEventHandler(OnWorkList);
            work.WorkOverwrite += new WorkOverwriteEventHandler(OnWorkOverwrite);
        }


        protected static void ReportMonitorUpdated()
        {
            if (MonitorWorkListUpdated != null)
                MonitorWorkListUpdated(null, new EventArgs());
        }

       

        protected static void retVal_WorkStart(object sender, WorkStartEventArgs e)
        {
            int id = (sender as IWork).ID;            

            if (!GetIsMuted(id))
                if (WorkStart != null)
                    WorkStart(sender, e);

            if (!onGoingWorkList.ContainsKey(e.ID) && fullWorkList.ContainsKey(e.ID))
                lock (onGoingWorkList)
                {
                    onGoingWorkList.Add(e.ID, fullWorkList[e.ID]);
                    ReportMonitorUpdated();
                }
        }
       
        protected static void OnWorkOverwrite(object sender, WorkOverwriteEventArgs e)
        {
            IWork work = (sender as IWork);
            if (GetDefaultOverwriteMode(work.ID) != OverwriteMode.Ask)
                e.Overwrite = GetDefaultOverwriteMode(work.ID);
            if (work.Aborted)
                e.Overwrite = OverwriteMode.KeepOriginal;

            if (!GetIsMuted((sender as IWork).ID))
                if (WorkOverwrite != null)
                {
                    WorkOverwrite(sender, e);
                    if (e.ApplyToAll)
                        SetDefaultOverwriteMode(work.ID, e.Overwrite);
                }     
        }

        protected static void OnWorkMessage(object sender, WorkMessageEventArgs e)
        {
            if (!GetIsMuted((sender as IWork).ID))
                if (WorkMessage != null)
                    WorkMessage(sender, e);
        }

        protected static void OnWorkList(object sender, WorkListEventArgs e)
        {
            if (!GetIsMuted((sender as IWork).ID))
                if (WorkList != null)
                    WorkList(sender, e);
        }

        protected static void retVal_WorkFinished(object sender, WorkFinishedEventArgs e)
        {
            int id = (sender as IWork).ID;

            ResetOverwriteMode(id);
            if (!GetIsMuted(id))
                if (WorkFinished != null)
                    WorkFinished(sender, e);

            if (onGoingWorkList.ContainsKey(e.ID))
                lock (onGoingWorkList)
                {
                    onGoingWorkList.Remove(e.ID);
                    ReportMonitorUpdated();
                }
        }

        protected static void OnWorkProgress(object sender, WorkProgressEventArgs e)
        {
            if (!GetIsMuted((sender as IWork).ID))
                if (WorkProgress != null)
                    WorkProgress(sender, e);
        }
        #endregion

        #region Mute

        public static bool GetIsMuted(int key)
        {
            return muteList.Contains(key);
        }

        public static void SetIsMuted(int key, bool value)
        {
            lock (muteList)
                if (value)
                { if (!muteList.Contains(key)) muteList.Add(key); }
                else if (muteList.Contains(key)) muteList.Remove(key);

        }

        #endregion

        #region OverwriteMode
        protected static void ResetOverwriteMode(int id)
        {
            lock (defaultOverwriteModeList)
                if (defaultOverwriteModeList.ContainsKey(id))
                    defaultOverwriteModeList.Remove(id);
        }

        protected static OverwriteMode GetDefaultOverwriteMode(int id)
        {
            lock (defaultOverwriteModeList)
                if (defaultOverwriteModeList.ContainsKey(id))
                    return defaultOverwriteModeList[id];
            return OverwriteMode.Ask;
        }

        protected static void SetDefaultOverwriteMode(int id, OverwriteMode mode)
        {
            lock (defaultOverwriteModeList)
            {
                if (defaultOverwriteModeList.ContainsKey(id))
                    defaultOverwriteModeList.Remove(id);
                defaultOverwriteModeList.Add(id, mode);
            }
        }
        #endregion


        private static void startID(int[] workIDs, int currentIdx)
        {
            if (currentIdx < workIDs.Length - 1)
                Works[workIDs[currentIdx]].WorkFinished +=
                    delegate { startID(workIDs, currentIdx + 1); };
            Works[workIDs[currentIdx]].Start(true);
        }

        public static void Start(int[] workIDs)
        {
            startID(workIDs, 0);
        }

        public static void Start(int workID)
        {
            Works[workID].Start(true);
        }

        #region Public Properties

        public static Dictionary<int, IWork> Works { get { return fullWorkList; } }
        #endregion
    }
}

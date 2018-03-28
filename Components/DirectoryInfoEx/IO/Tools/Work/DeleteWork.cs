///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// LYCJ (c) 2010 - http://www.quickzip.org/components                                                            //
// Release under LGPL license.                                                                                   //
//                                                                                                               //
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Text;

namespace System.IO.Tools
{
    public class DeleteWork : ExWorkBase
    {
        public DeleteWork(int id, FileSystemInfoEx[] items)
            : base(id,
            items.Length > 0 ? items[0].Parent : null, null)
        {
            _deleteItems = items;
            WorkType = WorkType.Delete;
        }

        #region Methods

        protected override void DoWork()
        {
            AddTotalCount(_deleteItems.Length);
            foreach (FileSystemInfoEx item in _deleteItems)
                if (!Aborted)
                {
                    CheckPause();
                    AddCompletedCount(item.FullName);
                    item.Delete();
                }
        }

        #endregion

        #region Data

        FileSystemInfoEx[] _deleteItems;

        #endregion

    }
}

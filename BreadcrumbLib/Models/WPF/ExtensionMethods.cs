using BreadcrumbLib.Defines;
using BreadcrumbLib.Interfaces;
using BreadcrumbLib.Models.DiskIO;
using BreadcrumbLib.Profile;
using BreadcrumbLib.Utils.WPF;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace BreadcrumbLib.Models.WPF
{
    public static partial class ExtensionMethods
    {
        //public static IShellDragDropHandler DragDrop(this IProfile profile)
        //{
        //    return (profile is IWPFProfile) ? (profile as IWPFProfile).DragDrop : null;
        //}

        //public static IEventAggregator Events(this IProfile profile)
        //{
        //    return profile.Events;
        //}

////        public static IScriptCommand Parse(this IProfile[] profiles, string path,
////            Func<IEntryModel, IScriptCommand> ifFoundFunc, IScriptCommand ifNotFound)
////        {
////            return new ParsePathCommand(profiles, path, ifFoundFunc, ifNotFound);
////        }
////
////
////        public static IScriptCommand Parse(this IProfile profile, string path,
////            Func<IEntryModel, IScriptCommand> ifFoundFunc, IScriptCommand ifNotFound)
////        {
////            return new ParsePathCommand(new[] { profile }, path, ifFoundFunc, ifNotFound);
////        }

////        public static IEnumerable<IModelIconExtractor<IEntryModel>> GetIconExtractSequence(this IProfile[] profiles, IEntryModel entry)
////        {
////            foreach (var p in profiles)
////            {
////                var result = p.GetIconExtractSequence(entry);
////                if (result != null)
////                    return result;
////            }
////            return new List<IModelIconExtractor<IEntryModel>>();
////        }

////        public static void NotifyEntryChanges(this IProfile profile, object sender, string fullPath, ChangeType changeType, string orgParseName = null)
////        {
////            if (profile == null || profile.Events == null)
////                return;
////
////            if (changeType == ChangeType.Moved)
////                profile.Events.PublishOnUIThread(new EntryChangedEvent(fullPath, orgParseName));
////            else profile.Events.PublishOnUIThread(new EntryChangedEvent(changeType, fullPath));
////        }


        public static string GetName(this IEntryModel model)
        {
            return model.Profile.Path.GetFileName(model.FullPath);
        }

        public static string GetExtension(this IEntryModel model)
        {
            return model.Profile.Path.GetExtension(model.FullPath);
        }


        public static string Combine(this IEntryModel model, params string[] paths)
        {
            return model.Profile.Path.Combine(model.FullPath, paths);
        }

        public static async Task<Stream> OpenStreamAsync(this IDiskIOHelper ioHelper, string fullPath,
            BreadcrumbLib.Defines.FileAccess access, CancellationToken ct)
        {
            IEntryModel entryModel = await ioHelper.Profile.ParseAsync(fullPath);
            ct.ThrowIfCancellationRequested();
            if (entryModel == null)
                if (access == BreadcrumbLib.Defines.FileAccess.Write)
                    entryModel = await ioHelper.CreateAsync(fullPath, false, ct);
                else throw new IOException("File not found.");
            ct.ThrowIfCancellationRequested();
            return await ioHelper.OpenStreamAsync(entryModel, access, ct);
        }



        public static async Task<string> WriteToCacheAsync(this IDiskIOHelper ioHelper, IEntryModel entry, CancellationToken ct, bool force = false)
        {
            var mapping = ioHelper.Mapper[entry];

            if (!mapping.IsCached || force)
            {
                if (entry.IsDirectory)
                {
                    System.IO.Directory.CreateDirectory(mapping.IOPath);
                    var listing = await entry.Profile.ListAsync(entry, ct).ConfigureAwait(false);
                    foreach (var subEntry in listing)
                        await WriteToCacheAsync(ioHelper, subEntry, ct, force).ConfigureAwait(false);
                }
                else
                {
                    using (var srcStream = await ioHelper.OpenStreamAsync(entry.FullPath,
                        BreadcrumbLib.Defines.FileAccess.Read, ct))
                    using (var outputStream = System.IO.File.OpenWrite(mapping.IOPath))
                        await StreamUtils.CopyStreamAsync(srcStream, outputStream).ConfigureAwait(false);
                }
            }

            return mapping.IOPath;
        }

        public static async Task DeleteAsync(this IDiskIOHelper ioHelper, IEntryModel[] entryModels, CancellationToken ct)
        {
            foreach (var em in entryModels)
                await ioHelper.DeleteAsync(em, ct);
        }

        public static async Task<IEntryModel> GetParentAsync(this IProfile profile, IEntryModel entry)
        {
            var parentFullPath = profile.Path.GetDirectoryName(entry.FullPath);
            if (String.IsNullOrEmpty(parentFullPath))
                return null;
            return await profile.ParseAsync(parentFullPath);
        }

        public static IProfile[] GetProfiles(this IEntryModel[] entryModels)
        {
            return entryModels.Select(em => em.Profile).Distinct().ToArray();
        }

        public static async Task<ImageSource> GetIconForModelAsync(this IModelIconExtractor<IEntryModel> extractor,
            IEntryModel model, CancellationToken ct)
        {
            byte[] bytes = await extractor.GetIconBytesForModelAsync(model, ct);
            ct.ThrowIfCancellationRequested();
            return bytes == null ?
                new BitmapImage() :
                BitmapSourceUtils.CreateBitmapSourceFromBitmap(bytes);
        }


////        public static IDataObject GetDataObject(this IDragDropHandler dragDropHandler, IEnumerable<IEntryModel> entries)
////        {
////            var dragHelper = dragDropHandler.GetDragHelper(entries);
////            if (dragHelper is ISupportShellDrag)
////                return (dragHelper as ISupportShellDrag).GetDataObject(entries.Cast<IDraggable>());
////            else return new DataObject();
////        }
////        public static IEnumerable<IEntryModel> GetEntryModels(this IDragDropHandler dragDropHandler, IDataObject dataObject)
////        {
////            var dropHelper = dragDropHandler.GetDropHelper(null);
////            if (dropHelper is ISupportShellDrop)
////                return (dropHelper as ISupportShellDrop).QueryDropDraggables(dataObject).Cast<IEntryModel>();
////            else return new List<IEntryModel>();
////        }
////
////        public static void OnDragCompleted(this IDragDropHandler dragDropHandler,
////            IEnumerable<IEntryModel> entries, IDataObject da, DragDropEffectsEx effect)
////        {
////            var dragHelper = dragDropHandler.GetDragHelper(entries);
////            if (dragHelper is ISupportShellDrag)
////                (dragHelper as ISupportShellDrag).OnDragCompleted(entries.Cast<IDraggable>(), da, effect);
////            else dragHelper.OnDragCompleted(entries.Cast<IDraggable>(), effect);
////        }
////
////        public static DragDropEffectsEx QueryDrag(this IDragDropHandler dragDropHandler, IEnumerable<IEntryModel> ems)
////        {
////            var dragHelper = dragDropHandler.GetDragHelper(ems);
////            return dragHelper.QueryDrag(ems.Cast<IDraggable>());
////        }
////
////        public static bool QueryCanDrop(this IDragDropHandler dragDropHandler, IEntryModel dest)
////        {
////            var dropHelper = dragDropHandler.GetDropHelper(dest);
////            return dropHelper.IsDroppable;
////        }
////
////        public static QueryDropEffects QueryDrop(this IDragDropHandler dragDropHandler,
////            IEnumerable<IEntryModel> entries, IEntryModel dest, DragDropEffectsEx allowedEffects)
////        {
////            var dropHelper = dragDropHandler.GetDropHelper(dest);
////            return dropHelper.QueryDrop(entries.Cast<IDraggable>(), allowedEffects);
////        }
////
////        public static DragDropEffectsEx OnDropCompleted(this IDragDropHandler dragDropHandler,
////            IEnumerable<IEntryModel> entries, IDataObject da, IEntryModel dest, DragDropEffectsEx allowedEffects)
////        {
////            var dropHelper = dragDropHandler.GetDropHelper(dest);
////            if (dropHelper is ISupportShellDrop)
////                return (dropHelper as ISupportShellDrop).Drop(entries.Cast<IDraggable>(), da, allowedEffects);
////            else return dropHelper.Drop(entries.Cast<IDraggable>(), allowedEffects);
////        }

        //        public interface IModelIconExtractor<IEntryModel>
        //{
        //    Task<byte[]> GetIconForModelAsync(IEntryModel model, CancellationToken ct);
        //}
    }
}

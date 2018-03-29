using BreadcrumbLib.Interfaces;
using BreadcrumbLib.Profile;
using BreadcrumbLib.Utils.WPF;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using XCSJ.Collections;
using XCSJ.EditorExtension.Base.XUnityEditor;
using XCSJ.EditorExtension.Base.XUnityEditorInternal;
using XCSJ.Interfaces;
using XCSJ.LitJson;
using XCSJ.PluginCommonUtils;

namespace XCSJ.EditorTools.Windows.Packages
{
    public class PackageHelper
    {
        public const string Ext = ".unitypackage";

        /// <summary>
        /// 获取工程Asset对应的资产项;同步构建子父关系；
        /// </summary>
        /// <returns></returns>
        public static AssetItem GetAssets()
        {
            var items = GetAssetItems();

            AssetItem assetsItem = items.FirstOrDefault(i => i.assetPath == "Assets");
            if (assetsItem == null)
            {
                assetsItem = new AssetItem()
                {
                    assetPath = "Assets",
                    guid = AssetDatabase_LinkType.assetFolderGUID,
                    isFolder = true,
                };
            }

            items.Sort((x, y) => string.Compare(x.assetPath, y.assetPath));

            foreach (var i in items)
            {
                var namePath = "/" + i.assetPath;
                if (namePath.TryParseNamePath(out string parentNamePath, out string name))
                {
                    var parent = assetsItem.FindByNamePath(parentNamePath);

                    if (parent != null)
                    {
                        i.parent = parent;
                        parent.assetItems.Add(i);
                    }
                }
            }

            return assetsItem;
        }

        /// <summary>
        /// 获取工程中所有可导出的资产项，即工程Assets文件夹中的所有文件与文件夹；仅获取到资产项的数据信息，其子父关系未构建；
        /// </summary>
        /// <param name="includeDependencies"></param>
        /// <returns></returns>
        public static List<AssetItem> GetAssetItems(bool includeDependencies = true)
        {
            string[] collection = new string[0];
            var guids = new HashSet<string>(AssetDatabase_LinkType.CollectAllChildren(AssetDatabase_LinkType.assetFolderGUID, collection));

            ExportPackageItem[] source = PackageUtility.BuildExportPackageItemsList(guids.ToArray(), includeDependencies);
            if (includeDependencies && source.Any((ExportPackageItem asset) => InternalEditorUtility_LinkType.IsScriptOrAssembly(asset.assetPath)))
            {
                source = PackageUtility.BuildExportPackageItemsList(guids.Union(InternalEditorUtility_LinkType.GetAllScriptGUIDs()).ToArray(), includeDependencies);
            }
            return source.ToList(i => new AssetItem(i));
        }

        /// <summary>
        /// 同步文件信息到资产项
        /// </summary>
        /// <param name="exportFile"></param>
        /// <param name="assetsItem"></param>
        public static void SyncFileToAssetItem(ExportFile exportFile, AssetItem assetsItem)
        {
            if (assetsItem == null || exportFile == null) return;

            assetsItem.SetEnable(false);

            foreach (var p in exportFile.exportList)
            {
                if (assetsItem.FindByNamePath("/" + p) is AssetItem item)
                {
                    item.SetEnable(true);
                }
            }

            foreach (var p in exportFile.ignoreList)
            {
                if (assetsItem.FindByNamePath("/" + p) is AssetItem item)
                {
                    item.SetEnable(false);
                }
            }
        }

        /// <summary>
        /// 同步资产项信息到文件
        /// </summary>
        /// <param name="assetsItem"></param>
        /// <param name="exportFile"></param>
        public static void SyncAssetItemToFile(AssetItem assetsItem, ExportFile exportFile)
        {
            if (assetsItem == null || exportFile == null) return;

            assetsItem.Foreach(item =>
            {
                var path = item.assetPath + "/";
                switch (item.enabledState)
                {
                    case EEnableState.All:
                        {
                            if (exportFile.exportList.Any(p => {
                                var p1 = p + "/";
                                return path.StartsWith(p1) && path.Count(c => c == '/') != p1.Count(c => c == '/');
                            }))
                            {
                                break;
                            }

                            exportFile.exportList.Add(item.assetPath);
                            break;
                        }
                }
            });
        }

        /// <summary>
        /// 导出unitypackage文件
        /// </summary>
        /// <param name="exportFile"></param>
        /// <param name="assetsItem"></param>
        /// <param name="dir"></param>
        public static void Export(ExportFile exportFile, AssetItem assetsItem, string dir, string prefix, string suffix)
        {
            List<string> list = new List<string>();

            SyncFileToAssetItem(exportFile, assetsItem);
            assetsItem.Foreach(i =>
            {
                if (i.enabledStatus > 0) list.Add(i.guid);
            });

            PackageUtility.ExportPackage(list.ToArray(), Path.Combine(dir, GetFileName(exportFile, prefix, suffix)));
        }

        /// <summary>
        /// 创建导出文件
        /// </summary>
        /// <param name="assetsItem"></param>
        /// <returns></returns>
        public static ExportFile Create(AssetItem assetsItem)
        {
            var file = new ExportFile();
            SyncAssetItemToFile(assetsItem, file);
            return file;
        }

        /// <summary>
        /// 获取文件名
        /// </summary>
        /// <param name="exportFile"></param>
        /// <param name="prefix"></param>
        /// <param name="suffix"></param>
        /// <returns></returns>
        public static string GetFileName(ExportFile exportFile, string prefix, string suffix)
        {
            return prefix + exportFile.name + suffix + Ext;
        }
    }
}

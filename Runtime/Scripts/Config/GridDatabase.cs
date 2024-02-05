// ******************************************************************
//       /\ /|       @file       GridDatabase
//       \ V/        @brief      地块数据库
//       | "")       @author     Shadowrabbit, yingtu0401@gmail.com
//       /  |                    
//      /  \\        @Modified   2024-02-04 14:12
//    *(__\_\        @Copyright  Copyright (c) 2024, Shadowrabbit
// ******************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Rabi.Map
{
    public class GridDatabase
    {
        private readonly Dictionary<string, RowCfgGrid> _configs = new Dictionary<string, RowCfgGrid>(); //cfgId映射row

        public static GridDatabase Instance => Inner.InternalInstance;

        private static class Inner
        {
            internal static readonly GridDatabase InternalInstance = new GridDatabase();
        }

        private readonly Dictionary<string, List<RowCfgGrid>> _packNameConfigGroup =
            new Dictionary<string, List<RowCfgGrid>>(); //pack分组配置

        public RowCfgGrid this[string key] => _configs.ContainsKey(key)
            ? _configs[key]
            : throw new Exception($"找不到配置 Cfg:{GetType()} key:{key}");

        public RowCfgGrid this[int id] => _configs.ContainsKey(id.ToString())
            ? _configs[id.ToString()]
            : throw new Exception($"找不到配置 Cfg:{GetType()} key:{id}");

        public List<RowCfgGrid> AllConfigs => _configs.Values.ToList();

        /// <summary>
        /// 获取行数据
        /// </summary>
        public RowCfgGrid Find(int i)
        {
            return this[i];
        }

        /// <summary>
        /// 获取行数据
        /// </summary>
        public RowCfgGrid Find(string i)
        {
            return this[i];
        }

        /// <summary>
        /// 根据packName值获取分组
        /// </summary>
        public List<RowCfgGrid> GetListByPackName(string groupValue)
        {
            return _packNameConfigGroup.ContainsKey(groupValue)
                ? _packNameConfigGroup[groupValue]
                : throw new Exception($"找不到组 Cfg:{GetType()} groupId:{groupValue}");
        }

        /// <summary>
        /// 以packName分组的形式 获取所有配置数据
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, List<RowCfgGrid>> GetAllConfigsByPackName()
        {
            return _packNameConfigGroup;
        }

        /// <summary>
        /// 加载表数据
        /// </summary>
        public void Load()
        {
            if (!Directory.Exists(MapEditorDef.GridFolder))
            {
                Directory.CreateDirectory(MapEditorDef.GridFolder);
            }

            //扩展包目录列表
            var packDirectories = Directory.GetDirectories(MapEditorDef.GridFolder);
            foreach (var packDirectory in packDirectories)
            {
                var packName = packDirectory.Replace(MapEditorDef.GridFolder, "").Replace("\\", "");
                //地块数据根目录
                var iconPathArray = Directory.GetFiles($"{packDirectory}/", "*.png");
                if (iconPathArray.Length <= 0)
                {
                    continue;
                }

                foreach (var iconPath in iconPathArray)
                {
                    var gridId = Path.GetFileNameWithoutExtension(iconPath);
                    if (_configs.ContainsKey(gridId))
                    {
                        continue;
                    }

                    var indexOfAsset = iconPath.IndexOf("Assets/", StringComparison.Ordinal);
                    var rowCfgGrid = new RowCfgGrid
                    {
                        gridId = gridId,
                        iconPath = iconPath.Substring(indexOfAsset).Replace("\\", "/"),
                        packName = packName
                    };
                    _configs.Add(gridId, rowCfgGrid);
                    if (!_packNameConfigGroup.ContainsKey(packName))
                    {
                        _packNameConfigGroup.Add(packName, new List<RowCfgGrid>());
                    }

                    _packNameConfigGroup[packName].Add(rowCfgGrid);
                }
            }
        }
    }
}
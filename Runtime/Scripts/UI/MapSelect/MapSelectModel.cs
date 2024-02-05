// ******************************************************************
//       /\ /|       @file       MapSelectModel
//       \ V/        @brief      
//       | "")       @author     Shadowrabbit, yingtu0401@gmail.com
//       /  |                    
//      /  \\        @Modified   2024-02-04 17:05
//    *(__\_\        @Copyright  Copyright (c) 2024, Shadowrabbit
// ******************************************************************

using System.Collections.Generic;
using System.IO;

namespace Rabi.Map
{
    public class MapSelectModel
    {
        private readonly List<string> _mapNameList = new List<string>(); //地图名称列表

        /// <summary>
        /// 同步数据
        /// </summary>
        public void FetchMapNameList()
        {
            _mapNameList.Clear();
            if (!Directory.Exists(MapEditorDef.MapDataFolder))
            {
                Directory.CreateDirectory(MapEditorDef.MapDataFolder);
            }

            //地图序列化数据文件路径
            var mapDataFiles = Directory.GetFiles($"{MapEditorDef.MapDataFolder}/", "*.json");
            if (mapDataFiles.Length <= 0)
            {
                return;
            }

            foreach (var mapDataPath in mapDataFiles)
            {
                var mapName = Path.GetFileNameWithoutExtension(mapDataPath);
                _mapNameList.Add(mapName);
            }
        }

        /// <summary>
        /// 获取地图名称列表
        /// </summary>
        /// <returns></returns>
        public List<string> GetMapNameList()
        {
            return _mapNameList;
        }
    }
}
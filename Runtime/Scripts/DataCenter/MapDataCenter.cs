// ******************************************************************
//       /\ /|       @file       MapDataCenter
//       \ V/        @brief      地图序列化数据中心
//       | "")       @author     Shadowrabbit, yingtu0401@gmail.com
//       /  |                    
//      /  \\        @Modified   2024-02-03 15:02
//    *(__\_\        @Copyright  Copyright (c) 2024, Shadowrabbit
// ******************************************************************

using System.IO;
using UnityEngine;

namespace Rabi.Map
{
    public class MapDataCenter
    {
        private static class Inner
        {
            internal static readonly MapDataCenter InternalInstance = new MapDataCenter();
        }

        public static MapDataCenter Instance => Inner.InternalInstance;

        private MapData _curMapData = new MapData(); //当前地图数据

        /// <summary>
        /// 加载地图数据
        /// </summary>
        public void LoadMapData(string fileName)
        {
            var mapJson = File.ReadAllText($"{MapEditorDef.MapDataFolder}/{fileName}.json");
            //加载数据
            _curMapData = JsonUtility.FromJson<MapData>(mapJson);
            if (_curMapData != null)
            {
                return;
            }

            Debug.LogError($"加载地图失败:{fileName}");
        }

        /// <summary>
        /// 获取当前地图数据
        /// </summary>
        /// <returns></returns>
        public MapData GetCurMapData()
        {
            return _curMapData;
        }

        /// <summary>
        /// 设置当前地图数据
        /// </summary>
        /// <param name="mapData"></param>
        public void SetCurMapData(MapData mapData)
        {
            _curMapData = mapData;
        }
    }
}
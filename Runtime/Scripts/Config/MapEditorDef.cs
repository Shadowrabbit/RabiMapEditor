// ******************************************************************
//       /\ /|       @file       MapEditorDef
//       \ V/        @brief      数据相关定义
//       | "")       @author     Shadowrabbit, yingtu0401@gmail.com
//       /  |                    
//      /  \\        @Modified   2022-08-26 22:05
//    *(__\_\        @Copyright  Copyright (c) 2022, Shadowrabbit
// ******************************************************************

using UnityEngine;

namespace Rabi.Map
{
    public static class MapEditorDef
    {
        public static readonly string
            GridFolder = $"{Application.dataPath}/RabiMapEditor/Editor/Config/Grids"; //地块配置数据目录

        public static readonly string MapDataFolder = $"{Application.dataPath}/AddressableAssets/MapConfig"; //地图数据目录

        public const int DefaultMapWidth = 5; //默认的地图尺寸
        public const int DefaultMapHeight = 5; //默认的地图尺寸
    }
}
// ******************************************************************
//       /\ /|       @file       MapEditModel
//       \ V/        @brief      地图编辑 页面数据层
//       | "")       @author     Shadowrabbit, yingtu0401@gmail.com
//       /  |                    
//      /  \\        @Modified   2024-02-04 19:02
//    *(__\_\        @Copyright  Copyright (c) 2024, Shadowrabbit
// ******************************************************************

namespace Rabi.Map
{
    public class MapEditModel
    {
        private string _mapName; //地图名称

        public string GetMapName()
        {
            return _mapName;
        }

        public void SetMapName(string mapName)
        {
            _mapName = mapName;
        }
    }
}
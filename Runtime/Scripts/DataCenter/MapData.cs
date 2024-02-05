// ******************************************************************
//       /\ /|       @file       MapData
//       \ V/        @brief      地图序列化数据
//       | "")       @author     Shadowrabbit, yingtu0401@gmail.com
//       /  |                    
//      /  \\        @Modified   2024-02-03 14:24
//    *(__\_\        @Copyright  Copyright (c) 2024, Shadowrabbit
// ******************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Rabi.Map
{
    [Serializable]
    public class MapData
    {
        public int height; //地图尺寸
        public int width; //地图尺寸
        public List<GridData> gridDataList = new List<GridData>(); //地块数据

        /// <summary>
        /// 获取当前位置上的地块信息
        /// </summary>
        /// <param name="cellPos"></param>
        /// <returns></returns>
        public GridData FindMapGridDataByCellPos(Vector2Int cellPos)
        {
            return gridDataList.FirstOrDefault(gridData => gridData.cellPos == cellPos);
        }

        /// <summary>
        /// 格子坐标是否在地图尺寸内
        /// </summary>
        /// <param name="cellPos"></param>
        public bool IsCellPosInMap(Vector2Int cellPos)
        {
            return Mathf.Abs(cellPos.x) < width &&
                   Mathf.Abs(cellPos.y) < height;
        }
    }
}
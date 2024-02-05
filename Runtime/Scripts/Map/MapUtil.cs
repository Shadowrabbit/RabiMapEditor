// ******************************************************************
//       /\ /|       @file       MapUtil
//       \ V/        @brief      
//       | "")       @author     Shadowrabbit, yingtu0401@gmail.com
//       /  |                    
//      /  \\        @Modified   2024-02-04 23:03
//    *(__\_\        @Copyright  Copyright (c) 2024, Shadowrabbit
// ******************************************************************

using UnityEngine;

namespace Rabi
{
    public static class MapUtil
    {
        /// <summary>
        /// 计算cell坐标系到世界坐标系的偏移
        /// </summary>
        /// <returns></returns>
        public static (float, float) CalcCellToWorldOffset(int width, int height)
        {
            var pivotOffset = new Vector2(0.5f, 0.5f); //一个单位1尺寸的地块 居中的重心偏移量
            var cellToWorldOffsetX = -(float) width / 2;
            var cellToWorldOffsetY = -(float) height / 2;
            return (pivotOffset.x + cellToWorldOffsetX, pivotOffset.y + cellToWorldOffsetY);
        }
    }
}
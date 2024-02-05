// ******************************************************************
//       /\ /|       @file       GridData
//       \ V/        @brief      地块序列化数据
//       | "")       @author     Shadowrabbit, yingtu0401@gmail.com
//       /  |                    
//      /  \\        @Modified   2024-02-03 14:11
//    *(__\_\        @Copyright  Copyright (c) 2024, Shadowrabbit
// ******************************************************************

using System;
using UnityEngine;

namespace Rabi.Map
{
    [Serializable]
    public class GridData
    {
        public string gridId; //地块配置id
        public Vector2Int cellPos; //当前地块的cell坐标系 X坐标 原点在左下角
    }
}
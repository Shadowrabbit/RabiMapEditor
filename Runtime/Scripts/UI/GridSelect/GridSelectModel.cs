// ******************************************************************
//       /\ /|       @file       GridSelectModel
//       \ V/        @brief      地块选择 数据
//       | "")       @author     Shadowrabbit, yingtu0401@gmail.com
//       /  |                    
//      /  \\        @Modified   2024-02-04 13:34
//    *(__\_\        @Copyright  Copyright (c) 2024, Shadowrabbit
// ******************************************************************

using System.Collections.Generic;
using UnityEngine;

namespace Rabi.Map
{
    public class GridSelectModel
    {
        private readonly List<string> _packNameList = new List<string>(); //地块配置分组名列表
        private readonly List<string> _gridIdList = new List<string>(); //地块id列表

        public void Init()
        {
            var packNameConfigGroup = GridDatabase.Instance.GetAllConfigsByPackName();
            if (packNameConfigGroup == null || packNameConfigGroup.Count <= 0)
            {
                Debug.LogWarning($"找不到地块配置 请查看{MapEditorDef.GridFolder}");
                return;
            }

            foreach (var (packName, rowCfgGridList) in packNameConfigGroup)
            {
                _packNameList.Add(packName);
                if (rowCfgGridList == null || rowCfgGridList.Count <= 0)
                {
                    continue;
                }

                foreach (var rowCfgGrid in rowCfgGridList)
                {
                    _gridIdList.Add(rowCfgGrid.gridId);
                }
            }
        }

        /// <summary>
        /// 获取包名列表
        /// </summary>
        /// <returns></returns>
        public List<string> GetPackNameList()
        {
            return _packNameList;
        }

        /// <summary>
        /// 获取地块id列表
        /// </summary>
        /// <returns></returns>
        public List<string> GetGridIdList()
        {
            return _gridIdList;
        }

        /// <summary>
        /// 获取默认pack名称
        /// </summary>
        /// <returns></returns>
        public string GetDefaultPackName()
        {
            return _packNameList.Count > 0 ? _packNameList[0] : string.Empty;
        }
    }
}
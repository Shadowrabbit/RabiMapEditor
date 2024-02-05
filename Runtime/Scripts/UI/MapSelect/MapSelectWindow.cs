// ******************************************************************
//       /\ /|       @file       MapSelectWindow
//       \ V/        @brief      地图选择页面
//       | "")       @author     Shadowrabbit, yingtu0401@gmail.com
//       /  |                    
//      /  \\        @Modified   2024-02-03 16:40
//    *(__\_\        @Copyright  Copyright (c) 2024, Shadowrabbit
// ******************************************************************

using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Rabi.Map
{
    public class MapSelectWindow : MonoBehaviour
    {
        private static MapSelectWindow _instance;
        [SerializeField] private GameObject buttonTemplate; //按钮模板
        [SerializeField] private GameObject nodeContent; //按钮挂载节点
        private readonly MapSelectModel _mapSelectModel = new MapSelectModel();

        public static MapSelectWindow Instance
        {
            get
            {
                //实例不存在
                if (_instance != null) return _instance;
                //场景里找
                var obj = FindObjectOfType<MapSelectWindow>(true);
                if (!obj)
                {
                    throw new Exception($"场景中找不到单例物体 name:{typeof(MapSelectWindow)}");
                }

                //创建个新的
                _instance = obj;
                return _instance;
            }
        }

        public void RefreshVisitable(bool isVisitable)
        {
            gameObject.SetActive(isVisitable);
        }

        /// <summary>
        /// 更新地图名称列表
        /// </summary>
        public void UpdateMapNameList()
        {
            Clear();
            _mapSelectModel.FetchMapNameList();
            //新地图
            var btnNewMap = Instantiate(buttonTemplate, nodeContent.transform);
            btnNewMap.GetComponentInChildren<TextMeshProUGUI>().text = "New Map";
            btnNewMap.GetComponent<Button>().onClick.AddListener(OnClickNewMap);
            var mapNameList = _mapSelectModel.GetMapNameList();
            if (mapNameList == null || mapNameList.Count <= 0)
            {
                return;
            }

            foreach (var mapName in mapNameList)
            {
                var mapSelectItem = Instantiate(buttonTemplate, nodeContent.transform);
                mapSelectItem.name = mapName;
                mapSelectItem.GetComponentInChildren<TextMeshProUGUI>().text = mapName;
                mapSelectItem.GetComponent<Button>().onClick.AddListener(delegate
                {
                    MapEditorManager.Instance.LoadMap(mapName);
                });
            }
        }

        /// <summary>
        /// 销毁地图名称按钮
        /// </summary>
        private void Clear()
        {
            //清理
            foreach (Transform child in nodeContent.transform)
            {
                Destroy(child.gameObject);
            }
        }

        /// <summary>
        /// 新地图点击回调
        /// </summary>
        private static void OnClickNewMap()
        {
            MapEditorManager.Instance.CreateNewMap();
        }
    }
}
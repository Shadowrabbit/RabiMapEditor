// ******************************************************************
//       /\ /|       @file       MapEditWindow
//       \ V/        @brief      地图编辑页面
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
    public class MapEditWindow : MonoBehaviour
    {
        [SerializeField] private TMP_InputField inputFiledMapName;
        [SerializeField] private TextMeshProUGUI tmpMapWidth;
        [SerializeField] private TextMeshProUGUI tmpMapHeight;
        [SerializeField] private Button btnIncreaseMapWidth;
        [SerializeField] private Button btnReduceMapWidth;
        [SerializeField] private Button btnIncreaseMapHeight;
        [SerializeField] private Button btnReduceMapHeight;
        [SerializeField] private Button btnErase;
        [SerializeField] private Button btnSave;
        [SerializeField] private Button btnCancel;
        [SerializeField] private Color disabledToolColor;
        [SerializeField] private Color enabledToolColor;
        private readonly MapEditModel _mapEditModel = new MapEditModel();
        private static MapEditWindow _instance;

        public static MapEditWindow Instance
        {
            get
            {
                //实例不存在
                if (_instance != null) return _instance;
                //场景里找
                var obj = FindObjectOfType<MapEditWindow>(true);
                if (!obj)
                {
                    throw new Exception($"场景中找不到单例物体 name:{typeof(MapEditWindow)}");
                }

                //创建个新的
                _instance = obj;
                return _instance;
            }
        }

        protected void OnEnable()
        {
            btnErase.onClick.AddListener(OnClickErase);
            btnSave.onClick.AddListener(OnClickSave);
            btnCancel.onClick.AddListener(OnClickCancel);
            btnIncreaseMapWidth.onClick.AddListener(OnClickIncreaseMapWidth);
            btnReduceMapWidth.onClick.AddListener(OnClickReduceMapWidth);
            btnIncreaseMapHeight.onClick.AddListener(OnClickIncreaseMapHeight);
            btnReduceMapHeight.onClick.AddListener(OnClickReduceMapHeight);
            inputFiledMapName.onEndEdit.AddListener(OnMapNameEndEdit);
        }

        protected void OnDisable()
        {
            btnErase.onClick.RemoveListener(OnClickErase);
            btnSave.onClick.RemoveListener(OnClickSave);
            btnCancel.onClick.RemoveListener(OnClickCancel);
            btnIncreaseMapWidth.onClick.RemoveListener(OnClickIncreaseMapWidth);
            btnReduceMapWidth.onClick.RemoveListener(OnClickReduceMapWidth);
            btnIncreaseMapHeight.onClick.RemoveListener(OnClickIncreaseMapHeight);
            btnReduceMapHeight.onClick.RemoveListener(OnClickReduceMapHeight);
            inputFiledMapName.onEndEdit.RemoveListener(OnMapNameEndEdit);
        }

        public void RefreshVisitable(bool isVisitable)
        {
            gameObject.SetActive(isVisitable);
        }

        /// <summary>
        /// 刷新当前地图名称
        /// </summary>
        /// <param name="mapName"></param>
        public void UpdateMapName(string mapName)
        {
            _mapEditModel.SetMapName(mapName);
            inputFiledMapName.text = mapName;
        }

        /// <summary>
        /// 刷新消除模式状态
        /// </summary>
        /// <param name="isErasingModeOn"></param>
        public void RefreshErasingState(bool isErasingModeOn)
        {
            btnErase.GetComponent<Image>().color = isErasingModeOn ? enabledToolColor : disabledToolColor;
        }

        /// <summary>
        /// 刷新地图尺寸
        /// </summary>
        /// <param name="width"></param>
        public void RefreshWidth(int width)
        {
            tmpMapWidth.text = width.ToString();
        }

        /// <summary>
        /// 刷新地图尺寸
        /// </summary>
        /// <param name="height"></param>
        public void RefreshHeight(int height)
        {
            tmpMapHeight.text = height.ToString();
        }

        /// <summary>
        /// 获取地图名称
        /// </summary>
        /// <returns></returns>
        public string GetMapName()
        {
            return _mapEditModel.GetMapName();
        }

        /// <summary>
        /// 消除图标点击回调
        /// </summary>
        private static void OnClickErase()
        {
            MapEditorManager.Instance.ChangeEraseState();
        }

        /// <summary>
        /// 保存并退出点击回调
        /// </summary>
        private static void OnClickSave()
        {
            MapEditorManager.Instance.SaveMap();
            MapEditorManager.Instance.ClearMap();
            MapEditorManager.Instance.UpdateWindow(MapEditorState.SelectMap);
            Instance.RefreshVisitable(false);
            GridSelectWindow.Instance.RefreshVisitable(false);
            MapSelectWindow.Instance.RefreshVisitable(true);
            MapSelectWindow.Instance.UpdateMapNameList();
        }

        /// <summary>
        /// 取消并退出点击回调
        /// </summary>
        private static void OnClickCancel()
        {
            MapEditorManager.Instance.ClearMap();
            MapEditorManager.Instance.UpdateWindow(MapEditorState.SelectMap);
            Instance.RefreshVisitable(false);
            GridSelectWindow.Instance.RefreshVisitable(false);
            MapSelectWindow.Instance.RefreshVisitable(true);
        }

        /// <summary>
        /// 增加地图宽度
        /// </summary>
        private static void OnClickIncreaseMapWidth()
        {
            MapEditorManager.Instance.UpdateMapWidth();
        }

        /// <summary>
        /// 降低地图宽度
        /// </summary>
        private static void OnClickReduceMapWidth()
        {
            MapEditorManager.Instance.UpdateMapWidth(false);
        }

        /// <summary>
        /// 增加地图高度
        /// </summary>
        private static void OnClickReduceMapHeight()
        {
            MapEditorManager.Instance.UpdateMapHeight(false);
        }

        /// <summary>
        /// 降低地图高度
        /// </summary>
        private static void OnClickIncreaseMapHeight()
        {
            MapEditorManager.Instance.UpdateMapHeight();
        }

        /// <summary>
        /// 地图名称编辑完成回调
        /// </summary>
        /// <param name="mapName"></param>
        private void OnMapNameEndEdit(string mapName)
        {
            _mapEditModel.SetMapName(mapName);
        }
    }
}
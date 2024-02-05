// ******************************************************************
//       /\ /|       @file       GridSelectWindow
//       \ V/        @brief      地块选择页面
//       | "")       @author     Shadowrabbit, yingtu0401@gmail.com
//       /  |                    
//      /  \\        @Modified   2024-02-03 16:40
//    *(__\_\        @Copyright  Copyright (c) 2024, Shadowrabbit
// ******************************************************************

using System;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Rabi.Map
{
    public class GridSelectWindow : MonoBehaviour
    {
        [SerializeField] private GameObject nodeGridContent;
        [SerializeField] private GameObject nodePackContent;
        [SerializeField] private GameObject gridSelectItemTemplate;
        [SerializeField] private GameObject btnTabTemplate;
        private string _currentSelectedPackName; //选中的扩展包名称
        private readonly GridSelectModel _gridSelectModel = new GridSelectModel(); //数据层
        private static GridSelectWindow _instance;

        public static GridSelectWindow Instance
        {
            get
            {
                //实例不存在
                if (_instance != null) return _instance;
                //场景里找
                var obj = FindObjectOfType<GridSelectWindow>(true);
                if (!obj)
                {
                    throw new Exception($"场景中找不到单例物体 name:{typeof(GridSelectWindow)}");
                }

                //创建个新的
                _instance = obj;
                return _instance;
            }
        }

        public void Init()
        {
            _gridSelectModel.Init();
            _currentSelectedPackName = _gridSelectModel.GetDefaultPackName();
            LoadItem();
            RefreshGridInfoList();
        }

        public void RefreshVisitable(bool isVisitable)
        {
            gameObject.SetActive(isVisitable);
        }

        /// <summary>
        /// 加载地块item和pack按钮
        /// </summary>
        private void LoadItem()
        {
            ClearTabBtn();
            var packNameList = _gridSelectModel.GetPackNameList();
            foreach (var packName in packNameList)
            {
                var packItem = Instantiate(btnTabTemplate, nodePackContent.transform);
                packItem.name = packName;
                packItem.GetComponentInChildren<TextMeshProUGUI>().text = packName;
                packItem.GetComponent<Button>().onClick.AddListener(delegate { UpdatePack(packName); });
            }

            //全加载 不符合packName的部分后面会隐藏
            var gridIdList = _gridSelectModel.GetGridIdList();
            foreach (var gridId in gridIdList)
            {
                var rowCfgGrid = GridDatabase.Instance.Find(gridId);
                var gridSelectItem = Instantiate(gridSelectItemTemplate, nodeGridContent.transform);
                gridSelectItem.name = gridId;
#if UNITY_EDITOR
                var sp = AssetDatabase.LoadAssetAtPath<Sprite>(rowCfgGrid.iconPath);
                gridSelectItem.GetComponent<Button>().image.sprite = sp;
                gridSelectItem.GetComponentInChildren<TextMeshProUGUI>().text = gridId;
                gridSelectItem.GetComponent<Button>().onClick.AddListener(delegate
                {
                    MapEditorManager.Instance.SetCurGridId(gridId);
                    MapEditorManager.Instance.UpdateWindow(MapEditorState.Place);
                });
#endif
            }
        }

        /// <summary>
        /// 选中扩展包
        /// </summary>
        /// <param name="packName"></param>
        private void UpdatePack(string packName)
        {
            _currentSelectedPackName = packName;
            RefreshGridInfoList();
        }

        /// <summary>
        /// 删除页签节点
        /// </summary>
        private void ClearTabBtn()
        {
            //清理
            foreach (Transform child in nodePackContent.transform)
            {
                Destroy(child.gameObject);
            }
        }

        /// <summary>
        /// 刷新地块列表
        /// </summary>
        private void RefreshGridInfoList()
        {
            foreach (var btnGrid in nodeGridContent.GetComponentsInChildren<Button>(true))
            {
                var gridId = btnGrid.name;
                var rowCfgGrid = GridDatabase.Instance.Find(gridId);
                btnGrid.gameObject.SetActive(rowCfgGrid.packName.Equals(_currentSelectedPackName));
            }
        }
    }
}
// ******************************************************************
//       /\ /|       @file       MapEditorManager
//       \ V/        @brief      地图管理器 管理场景中的地图的 也是总入口
//       | "")       @author     Shadowrabbit, yingtu0401@gmail.com
//       /  |                    
//      /  \\        @Modified   2024-02-03 16:09
//    *(__\_\        @Copyright  Copyright (c) 2024, Shadowrabbit
// ******************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Rabi.Map
{
    public class MapEditorManager : MonoBehaviour
    {
        private readonly Dictionary<Vector2Int, GameObject>
            _pos2GridObj = new Dictionary<Vector2Int, GameObject>(); //网格id对地块物体映射

        private readonly Dictionary<Vector3Int, GameObject>
            _pos2TerrainObj = new Dictionary<Vector3Int, GameObject>(); //网格id对地皮映射

        private MapEditorState _mapEditorState = MapEditorState.SelectMap; //编辑器状态
        private string _curGridId = string.Empty; //当前笔刷选中的地块id
        private MapData _mapData; //当前使用的地图序列化数据
        [SerializeField] private Transform gridContainer; //地块容器
        [SerializeField] private Transform terrainContainer; //地皮容器
        [SerializeField] private GameObject gridTemplate; //地块模板
        [SerializeField] private SpriteRenderer gridCursor; //光标
        [SerializeField] private Sprite normalGridCursor; //正常光标资源
        [SerializeField] private Sprite eraserGridCursor; //消除模式光标资源
        private static MapEditorManager _instance;

        public static MapEditorManager Instance
        {
            get
            {
                //实例不存在
                if (_instance != null) return _instance;
                //场景里找
                var obj = FindObjectOfType<MapEditorManager>(true);
                if (!obj)
                {
                    throw new Exception($"场景中找不到单例物体 name:{typeof(MapEditorManager)}");
                }

                //创建个新的
                _instance = obj;
                return _instance;
            }
        }

        protected void Awake()
        {
            //地块配置数据加载
            GridDatabase.Instance.Load();
            InitWindow();
            //刚打开编辑器时显示选择地图
            UpdateWindow(MapEditorState.SelectMap);
            MapSelectWindow.Instance.UpdateMapNameList();
        }

        protected void Update()
        {
            OnPressE();
            OnPressTab();
            InputCheckErase();
            InputCheckPlace();
            UpdateCursor();
        }

        /// <summary>
        /// 更新当前地图数据
        /// </summary>
        public void UpdateMapData()
        {
            _mapData = MapDataCenter.Instance.GetCurMapData();
        }

        /// <summary>
        /// 设置地块id
        /// </summary>
        /// <param name="gridId"></param>
        public void SetCurGridId(string gridId)
        {
            _curGridId = gridId;
        }

        /// <summary>
        /// 设置编辑器状态
        /// </summary>
        /// <param name="mapEditorState"></param>
        public void UpdateWindow(MapEditorState mapEditorState)
        {
            _mapEditorState = mapEditorState;
            RefreshWindow();
        }

        /// <summary>
        /// 加载新地图
        /// </summary>
        public void CreateNewMap()
        {
            var ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            var timeStamp = Convert.ToInt64(ts.TotalSeconds);
            SetCurGridId(string.Empty);
            _mapData = new MapData
            {
                width = MapEditorDef.DefaultMapWidth,
                height = MapEditorDef.DefaultMapHeight
            };
            MapDataCenter.Instance.SetCurMapData(_mapData);
            UpdateMapData();
            RefreshMap();
            UpdateWindow(MapEditorState.Place);
            MapEditWindow.Instance.UpdateMapName(timeStamp.ToString());
            MapEditWindow.Instance.RefreshWidth(_mapData.width);
            MapEditWindow.Instance.RefreshHeight(_mapData.height);
        }

        /// <summary>
        /// 加载地图
        /// </summary>
        public void LoadMap(string mapName)
        {
            SetCurGridId(string.Empty);
            MapDataCenter.Instance.LoadMapData(mapName);
            UpdateMapData();
            RefreshMap();
            UpdateWindow(MapEditorState.Place);
            MapEditWindow.Instance.UpdateMapName(mapName);
            MapEditWindow.Instance.RefreshWidth(_mapData.width);
            MapEditWindow.Instance.RefreshHeight(_mapData.height);
        }

        /// <summary>
        /// 保存地图
        /// </summary>
        public void SaveMap()
        {
            var mapName = MapEditWindow.Instance.GetMapName();
            var mapDataPath = $"{MapEditorDef.MapDataFolder}/{mapName}.json";
            try
            {
                if (File.Exists(mapDataPath))
                {
                    File.Delete(mapDataPath);
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
                throw;
            }
            finally
            {
                var json = JsonUtility.ToJson(_mapData, true);
                File.WriteAllText(mapDataPath, json);
#if UNITY_EDITOR
                AssetDatabase.Refresh();
#endif
            }
        }

        /// <summary>
        /// 消除工具切换状态
        /// </summary>
        public void ChangeEraseState()
        {
            _mapEditorState = _mapEditorState == MapEditorState.Erase ? MapEditorState.Place : MapEditorState.Erase;
            MapEditWindow.Instance.RefreshErasingState(_mapEditorState == MapEditorState.Erase);
        }

        /// <summary>
        /// 清理地图
        /// </summary>
        public void ClearMap()
        {
            foreach (var gridObj in _pos2GridObj.Values)
            {
                Destroy(gridObj);
            }

            foreach (var gridObj in _pos2TerrainObj.Values)
            {
                Destroy(gridObj);
            }

            _pos2GridObj.Clear();
            _pos2TerrainObj.Clear();
        }

        /// <summary>
        /// 更新地图尺寸
        /// </summary>
        public void UpdateMapWidth(bool isIncrease = true)
        {
            _mapData.width = isIncrease ? _mapData.width + 1 : _mapData.width - 1;
            if (_mapData.width < 1)
            {
                _mapData.width = 1;
            }

            var gridDataList = _mapData.gridDataList;
            if (gridDataList != null)
            {
                //剔除多余的地块
                for (var i = gridDataList.Count - 1; i >= 0; i--)
                {
                    if (gridDataList[i].cellPos.x >= _mapData.width)
                    {
                        _mapData.gridDataList.RemoveAt(i);
                    }
                }
            }

            RefreshMapTerrain();
            RefreshGridObj();
            MapEditWindow.Instance.RefreshWidth(_mapData.width);
        }

        /// <summary>
        /// 更新地图尺寸
        /// </summary>
        public void UpdateMapHeight(bool isIncrease = true)
        {
            _mapData.height = isIncrease ? _mapData.height + 1 : _mapData.height - 1;
            if (_mapData.height < 1)
            {
                _mapData.height = 1;
            }

            var gridDataList = _mapData.gridDataList;
            if (gridDataList != null)
            {
                //剔除多余的地块
                for (var i = gridDataList.Count - 1; i >= 0; i--)
                {
                    if (gridDataList[i].cellPos.y >= _mapData.height)
                    {
                        _mapData.gridDataList.RemoveAt(i);
                    }
                }
            }

            RefreshMapTerrain();
            RefreshGridObj();
            MapEditWindow.Instance.RefreshHeight(_mapData.height);
        }

        /// <summary>
        /// 初始化页面
        /// </summary>
        private static void InitWindow()
        {
            GridSelectWindow.Instance.Init();
        }

        /// <summary>
        /// 更新UI
        /// </summary>
        private void RefreshWindow()
        {
            MapEditWindow.Instance.RefreshVisitable(_mapEditorState != MapEditorState.SelectMap);
            GridSelectWindow.Instance.RefreshVisitable(_mapEditorState == MapEditorState.SelectingGrid);
            MapSelectWindow.Instance.RefreshVisitable(_mapEditorState == MapEditorState.SelectMap);
        }

        /// <summary>
        /// E键按下回调
        /// </summary>
        private void OnPressE()
        {
            if (Keyboard.current.eKey.wasPressedThisFrame)
            {
                ChangeEraseState();
            }
        }

        /// <summary>
        /// Tab键按下回调
        /// </summary>
        private void OnPressTab()
        {
            if (Keyboard.current.tabKey.wasPressedThisFrame)
            {
                ChangeGridSelect();
            }
        }

        /// <summary>
        /// 消除输入检测
        /// </summary>
        private void InputCheckErase()
        {
            //必须在消除模式下
            if (_mapEditorState != MapEditorState.Erase)
            {
                return;
            }

            //右键退出
            if (Mouse.current.rightButton.wasPressedThisFrame)
            {
                ChangeEraseState();
                return;
            }

            //必须是鼠标左键按下或按住
            if (!Mouse.current.leftButton.wasPressedThisFrame && !Mouse.current.leftButton.isPressed)
            {
                return;
            }

            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            var cellPos = MouseUIPos2CellPos();
            //当前点击的地块
            var gridData = _mapData.FindMapGridDataByCellPos(cellPos);
            //当前位置没有地块数据
            if (gridData == null)
            {
                return;
            }

            _mapData.gridDataList.Remove(gridData);
            Destroy(_pos2GridObj[cellPos]);
            _pos2GridObj.Remove(cellPos);
        }

        /// <summary>
        /// 放置指令检测
        /// </summary>
        private void InputCheckPlace()
        {
            //放置模式
            if (_mapEditorState != MapEditorState.Place)
            {
                return;
            }

            var cellPos = MouseUIPos2CellPos();
            var isPlacementValid = IsGridPlacementValid(cellPos);
            //当前位置无法放置 已经有地块数据了 需要先消除
            if (!isPlacementValid)
            {
                return;
            }

            //必须是鼠标左键按下或按住
            if (!Mouse.current.leftButton.wasPressedThisFrame && !Mouse.current.leftButton.isPressed)
            {
                return;
            }

            //UI响应优先级更高
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            //笔刷没有选择地块
            if (_curGridId == string.Empty)
            {
                return;
            }

            //越界无法放置
            if (!_mapData.IsCellPosInMap(cellPos))
            {
                return;
            }

            //放置地块
            var gridData = new GridData
            {
                gridId = _curGridId,
                cellPos = cellPos
            };
            _mapData.gridDataList.Add(gridData);
            var gridObj = Instantiate(gridTemplate, gridContainer);
            var (offsetX, offsetY) = MapUtil.CalcCellToWorldOffset(_mapData.width, _mapData.height);
            gridObj.transform.position =
                new Vector3(cellPos.x + offsetX, cellPos.y + offsetY);
#if UNITY_EDITOR
            var rowCfgGrid = GridDatabase.Instance.Find(_curGridId);
            var sp = AssetDatabase.LoadAssetAtPath<Sprite>(rowCfgGrid.iconPath);
            gridObj.GetComponent<SpriteRenderer>().sprite = sp;
            _pos2GridObj.Add(cellPos, gridObj);
#endif
        }

        /// <summary>
        /// 切换地块选择页面
        /// </summary>
        private void ChangeGridSelect()
        {
            _mapEditorState = _mapEditorState switch
            {
                MapEditorState.Place => MapEditorState.SelectingGrid,
                MapEditorState.SelectingGrid => MapEditorState.Place,
                _ => _mapEditorState
            };

            GridSelectWindow.Instance.RefreshVisitable(_mapEditorState == MapEditorState.SelectingGrid);
        }

        /// <summary>
        /// 更新光标
        /// </summary>
        private void UpdateCursor()
        {
            gridCursor.gameObject.SetActive(
                (_mapEditorState == MapEditorState.Erase || _mapEditorState == MapEditorState.Place) &&
                !EventSystem.current.IsPointerOverGameObject());
            //加载界面 不显示
            if (_mapEditorState == MapEditorState.SelectMap)
            {
                return;
            }

            var worldPositionOnGrid = MouseUIPos2CellPos();
            var (offsetX, offsetY) = MapUtil.CalcCellToWorldOffset(_mapData.width, _mapData.height);
            gridCursor.transform.position =
                new Vector3(worldPositionOnGrid.x + offsetX, worldPositionOnGrid.y + offsetY);
            //消除模式中
            if (_mapEditorState == MapEditorState.Erase)
            {
                gridCursor.sprite = eraserGridCursor;
                gridCursor.color = Color.red;
                return;
            }

            //普通状态
            gridCursor.sprite = normalGridCursor;
            if (_curGridId == string.Empty)
            {
                gridCursor.color = Color.gray;
                return;
            }

            var isPlacementValid = IsGridPlacementValid(worldPositionOnGrid);
            gridCursor.color = isPlacementValid && _mapData.IsCellPosInMap(worldPositionOnGrid)
                ? Color.green
                : Color.red;
        }

        /// <summary>
        /// 当前地块是否可放置
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        private bool IsGridPlacementValid(Vector2Int pos)
        {
            return _mapData.gridDataList.All(mapGridData => mapGridData.cellPos != pos);
        }

        /// <summary>
        /// 鼠标UI坐标转格子坐标
        /// </summary>
        /// <returns></returns>
        private Vector2Int MouseUIPos2CellPos()
        {
            if (Camera.main is null) return default;
            var uiPos = Mouse.current.position.ReadValue();
            var screenPos = new Vector3(uiPos.x, uiPos.y, Camera.main.nearClipPlane);
            var worldPos = Camera.main.ScreenToWorldPoint(screenPos);
            var (offsetX, offsetY) = MapUtil.CalcCellToWorldOffset(_mapData.width, _mapData.height);
            var worldPositionOnGrid =
                new Vector2Int(Mathf.RoundToInt(worldPos.x - offsetX), Mathf.RoundToInt(worldPos.y - offsetY));
            return worldPositionOnGrid;
        }

        /// <summary>
        /// 刷新地皮
        /// </summary>
        private void RefreshMapTerrain()
        {
            foreach (var terrainObj in _pos2TerrainObj.Values)
            {
                Destroy(terrainObj);
            }

            _pos2TerrainObj.Clear();
            var (offsetX, offsetY) = MapUtil.CalcCellToWorldOffset(_mapData.width, _mapData.height);
            for (var x = 0; x < _mapData.width; x++)
            {
                for (var y = 0; y < _mapData.height; y++)
                {
                    var terrainObj = Instantiate(gridTemplate, terrainContainer);
                    var worldPos = new Vector3(x + offsetX, y + offsetY, 0);
                    terrainObj.name = $"{x}-{y}";
                    terrainObj.transform.position = worldPos;
                    terrainObj.GetComponent<SpriteRenderer>().sortingOrder = -99;
                    _pos2TerrainObj.Add(new Vector3Int(x, y, 0), terrainObj);
                }
            }
        }

        /// <summary>
        /// 更新地块信息
        /// </summary>
        private void RefreshGridObj()
        {
            foreach (var terrainObj in _pos2GridObj.Values)
            {
                Destroy(terrainObj);
            }

            _pos2GridObj.Clear();
            var (offsetX, offsetY) = MapUtil.CalcCellToWorldOffset(_mapData.width, _mapData.height);
            foreach (var grid in _mapData.gridDataList)
            {
                var pos = new Vector3(grid.cellPos.x + offsetX, grid.cellPos.y + offsetY);
                var gridObj = Instantiate(gridTemplate, gridContainer);
                gridObj.transform.position = pos;
                gridObj.name = $"{grid.cellPos.x}-{grid.cellPos.y}";
                var objSpriteRenderer = gridObj.GetComponent<SpriteRenderer>();
                _pos2GridObj.Add(grid.cellPos, gridObj);
                objSpriteRenderer.sortingOrder = 0;
                var rowCfgGrid = GridDatabase.Instance.Find(grid.gridId);
#if UNITY_EDITOR
                var sp = AssetDatabase.LoadAssetAtPath<Sprite>(rowCfgGrid.iconPath);
                objSpriteRenderer.sprite = sp;
#endif
            }
        }

        /// <summary>
        /// 更新地图
        /// </summary>
        private void RefreshMap()
        {
            //更新表现
            RefreshMapTerrain();
            //更新地块信息
            RefreshGridObj();
        }
    }
}
// ******************************************************************
//       /\ /|       @file       MapEditorState
//       \ V/        @brief      编辑器的三个状态 分别对应三个UI
//       | "")       @author     Shadowrabbit, yingtu0401@gmail.com
//       /  |                    
//      /  \\        @Modified   2022-08-28 21:44
//    *(__\_\        @Copyright  Copyright (c) 2022, Shadowrabbit
// ******************************************************************

namespace Rabi.Map
{
    public enum MapEditorState
    {
        SelectMap, //选择加载的地图中
        Place, //放置中
        Erase, //消除中
        SelectingGrid //选择当前笔刷的地块id中
    }
}
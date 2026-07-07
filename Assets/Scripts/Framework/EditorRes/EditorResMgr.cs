using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 编辑器资源管理器
/// 用于在编辑器模式下加载资源，不依赖AB包
/// 提供Prefab、材质、图片、音频等资源的加载功能
/// </summary>
public class EditorResMgr : BaseManager<EditorResMgr>
{
    //资源根路径，指向Editor文件夹下的ArtRes目录
    private string rootPath = "Assets/Editor/ArtRes/";

    private EditorResMgr() { }

    /// <summary>
    /// 加载编辑器资源
    /// </summary>
    /// <typeparam name="T">资源类型</typeparam>
    /// <param name="path">资源路径（不含后缀）</param>
    /// <returns>加载的资源对象</returns>
    public T LoadEditorRes<T>(string path) where T : Object
    {
#if UNITY_EDITOR
        string suffixName = "";
        //根据类型自动添加文件后缀
        if (typeof(T) == typeof(GameObject))
            suffixName = ".prefab";
        else if (typeof(T) == typeof(Material))
            suffixName = ".mat";
        else if (typeof(T) == typeof(Texture))
            suffixName = ".png";
        else if (typeof(T) == typeof(AudioClip))
            suffixName = ".mp3";
        T res = AssetDatabase.LoadAssetAtPath<T>(rootPath + path + suffixName);
        return res;
#else
        return null;
#endif
    }

    /// <summary>
    /// 加载指定名称的精灵
    /// </summary>
    /// <param name="path">精灵所在文件路径</param>
    /// <param name="spriteName">精灵名称</param>
    /// <returns>找到的精灵对象</returns>
    public Sprite LoadSprite(string path, string spriteName)
    {
#if UNITY_EDITOR
        //加载图片文件中的所有精灵
        Object[] sprites = AssetDatabase.LoadAllAssetRepresentationsAtPath(rootPath + path);
        //遍历查找指定名称的精灵
        foreach (var item in sprites)
        {
            if (spriteName == item.name)
                return item as Sprite;
        }
        return null;
#else
        return null;
#endif
    }

    /// <summary>
    /// 加载图片文件中的所有精灵
    /// </summary>
    /// <param name="path">精灵所在文件路径</param>
    /// <returns>精灵字典，key为精灵名称</returns>
    public Dictionary<string, Sprite> LoadSprites(string path)
    {
#if UNITY_EDITOR
        Dictionary<string, Sprite> spriteDic = new Dictionary<string, Sprite>();
        Object[] sprites = AssetDatabase.LoadAllAssetRepresentationsAtPath(rootPath + path);
        foreach (var item in sprites)
        {
            spriteDic.Add(item.name, item as Sprite);
        }
        return spriteDic;
#else
        return null;
#endif
    }
}
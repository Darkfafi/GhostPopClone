using System;
using System.Collections.Generic;
using UnityEngine;

public class LevelTile
{
    public int Index
    {
        get; private set;
    }

    public GameObject TileInstance
    {
        get; private set;
    }

    public LevelTileData LevelTileData
    {
        get; private set;
    }

    public LevelTile(int index, GameObject tileGameObject, LevelTileData data)
    {
        Index = index;
        TileInstance = tileGameObject;
        LevelTileData = data == null ? new LevelTileData() : data;
    }
}

public class LevelTileData
{
    private Dictionary<Type, ITileDataObject> _tileData = new Dictionary<Type, ITileDataObject>();

    public T GetTileData<T>() where T : class, ITileDataObject, new()
    {
        ITileDataObject returnData;
        if (_tileData.TryGetValue(typeof(T), out returnData))
        {
            return (T)returnData;
        }

        return null;
    }

    public LevelTileData AddTileData<T>() where T : class, ITileDataObject, new()
    {
        Type t = typeof(T);

        if (!_tileData.ContainsKey(t))
        {
            _tileData.Add(t, new T());
        }
        else
        {
            Debug.LogWarningFormat("TileDataObject of type {0} was already added to this LevelTileData", t.ToString());
        }

        return this;
    }

    public void Clean()
    {
        foreach (var pair in _tileData)
        {
            pair.Value.Clean();
        }

        _tileData.Clear();
        _tileData = null;
    }
}

public interface ITileDataObject
{
    void Clean();
}
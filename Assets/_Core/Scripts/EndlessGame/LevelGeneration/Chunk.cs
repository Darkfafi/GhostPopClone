using System;
using UnityEngine;

public struct Chunk
{
    public GameObject ChunkObject { get; private set; }
    public int ChunkIndex;
    public ChunkTheme ChunkTheme { get; private set; }

    public Chunk(int chunkTotalIndex, GameObject chunkObject, ChunkTheme chunkTheme)
    {
        ChunkIndex = chunkTotalIndex;
        ChunkObject = chunkObject;
        ChunkTheme = chunkTheme;
    }
}

[Serializable]
public struct ChunkTheme
{
    public Color ThemeColor
    {
        get
        {
            return _themeColor;
        }
    }

    [SerializeField]
    private Color _themeColor;

    public ChunkTheme(Color themeColor)
    {
        _themeColor = themeColor;
    }
}
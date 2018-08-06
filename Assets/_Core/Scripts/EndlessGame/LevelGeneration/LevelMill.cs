using System.Collections.Generic;
using UnityEngine;

public class LevelMill : MonoBehaviour
{
    public Vector2 ChunkSize
    {
        get
        {
            return new Vector2(_size.x, _size.y / _chunkAmount);
        }
    }

    [Header("Options")]

    [SerializeField]
    private Vector2 _size;

    [SerializeField]
    private int _chunkAmount;

    [SerializeField]
    private float _speed;

    [Header("Requirements")]

    [SerializeField]
    private GameObject _groundChunkPrefab;

    private GameObject _mill;
    private int _amountOfChunksCreated = 0;
    private List<LevelTile> _currentlyActiveChunks = new List<LevelTile>();
    private Queue<LevelTileData> _levelDataQueue = new Queue<LevelTileData>();

    private float _distanceTraveled;
    private float _previousDistanceTraveled;

    [Header("Editor")]
    [SerializeField]
    private Color _sizeRenderColor = Color.cyan;

    protected void Awake()
    {
        _mill = new GameObject("Mill");
        _mill.transform.localPosition = transform.position;
        _mill.transform.SetParent(transform, false);
    }

    protected void Update()
    {
        float delta = _speed * Time.deltaTime;
        _mill.transform.Translate(0, 0, delta, Space.Self);
        _distanceTraveled += delta;

        float a = _previousDistanceTraveled - ChunkSize.y * 0.5f;
        float b = _distanceTraveled - ChunkSize.y * 0.5f;

        if (Mathf.RoundToInt(a / ChunkSize.y) != Mathf.RoundToInt(b / ChunkSize.y))
        {
            LevelTile c = _currentlyActiveChunks[0];
            _currentlyActiveChunks.Remove(c);
            Destroy(c.TileInstance);
        }

        while(_currentlyActiveChunks.Count < _chunkAmount + 1)
        {
            CreateChunk(_levelDataQueue.Count > 0 ? _levelDataQueue.Dequeue() : null);
        }

        _previousDistanceTraveled = _distanceTraveled;
    }

    private void CreateChunk(LevelTileData optionalTileData)
    {
        GameObject chunk = GameObject.Instantiate(_groundChunkPrefab);
        chunk.transform.localScale = new Vector3(ChunkSize.x / 10, 1, ChunkSize.y / 10);
        chunk.GetComponent<MeshRenderer>().material.mainTextureScale = new Vector2(ChunkSize.x / 10, ChunkSize.y / 10);
        chunk.transform.SetParent(_mill.transform, false);
        chunk.transform.localPosition = new Vector3(0, 0, CalculateZPositionByIndex(_amountOfChunksCreated));

        _currentlyActiveChunks.Add(new LevelTile(_amountOfChunksCreated, chunk, optionalTileData));
        _amountOfChunksCreated++;
    }

    private float CalculateZPositionByIndex(int index)
    {
        return -((index * ChunkSize.y) - _size.y * 0.5f + ChunkSize.y * 0.5f);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = _sizeRenderColor;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireCube(transform.position, new Vector3(_size.x, 0.2f, _size.y));

        if (_chunkAmount > 0)
        {
            for (int i = 0; i < _chunkAmount; i++)
            {
                Gizmos.color = i % 2 == 0 ? new Color(0, 0, 0, 0.4f) : new Color(0.3f, 0.3f, 0.3f, 0.5f);
                Gizmos.DrawCube(new Vector3(transform.position.x, transform.position.y, transform.position.z + CalculateZPositionByIndex(i)), new Vector3(ChunkSize.x, 0.001f, ChunkSize.y));
            }
        }
    }
#endif
}

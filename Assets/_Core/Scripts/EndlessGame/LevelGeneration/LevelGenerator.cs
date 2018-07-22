using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public delegate void ChunkHandler(Chunk chunk);
    public event ChunkHandler ChunkWasCreatedEvent;
    public event ChunkHandler ChunkEnteredEvent;

    [SerializeField]
    private Transform _rootLevelObject;

    [SerializeField]
    private GameObject _groundChunkPrefab;

    [SerializeField]
    private int _maximumChunkAmount = 4;

    [SerializeField]
    private float _zBounds = -12;

    [SerializeField]
    private Vector2 _chunkSize;

    [SerializeField]
    private ChunkTheme _levelTheme;

    public float MetersTraveled
    {
        get; private set;
    }

    public int TotalChunksCreated
    {
        get
        {
            return _chunksCreated;
        }
    }

    public float CurrentSpeed
    {
        get
        {
            return _speed;
        }
    }

    private Queue<ChunkTheme> _themeQueue = new Queue<ChunkTheme>();

    private float _speed = 1.5f;

    private List<Chunk> _chunks = new List<Chunk>();
    private int _chunksCreated = 0;

    private float _previousPosition;

    protected void Awake()
    {
        MetersTraveled = (_chunkSize.y * 0.5f);
        _previousPosition = MetersTraveled - _chunkSize.y;
    }

    private void Update()
    {
        if (_speed == 0)
            return;

        float distanceZ = _speed * Time.deltaTime;
        MetersTraveled += distanceZ;
        _rootLevelObject.Translate(0, 0, distanceZ);

        if(_chunks.Count > 0)
        {
            if(_chunks[_chunks.Count - 1].ChunkObject.transform.position.z > _zBounds)
            {
                CreateChunk();
                if (_chunks.Count > _maximumChunkAmount)
                {
                    GameObject c = _chunks[0].ChunkObject;
                    _chunks.RemoveAt(0);
                    Destroy(c);
                }
            }
        }
        else
        {
            CreateChunk();
        }

        if(Mathf.FloorToInt(_previousPosition / _chunkSize.y) != Mathf.FloorToInt(MetersTraveled / _chunkSize.y))
        {
            // Entered new Chunk
            for (int i = 0; i < _chunks.Count; i++)
            {
                if (Mathf.FloorToInt(Mathf.Abs(_chunks[i].ChunkObject.transform.localPosition.z) / _chunkSize.y) == Mathf.FloorToInt(MetersTraveled / _chunkSize.y))
                {
                    if(ChunkEnteredEvent != null)
                    {
                        ChunkEnteredEvent(_chunks[i]);
                    }
                    break;
                }
            }
        }

        _previousPosition = MetersTraveled;
    }

    public Chunk[] GetAllCurrentChunks()
    {
        return _chunks.ToArray();
    }

    private void CreateChunk()
    {
        ChunkTheme theme = _themeQueue.Count > 0 ? _themeQueue.Dequeue() : _levelTheme;
        GameObject chunk = GameObject.Instantiate(_groundChunkPrefab);
        chunk.transform.localScale = new Vector3(_chunkSize.x / 10, 1, _chunkSize.y / 10);
        chunk.GetComponent<MeshRenderer>().material.mainTextureScale = new Vector2(_chunkSize.x / 10, _chunkSize.y / 10);
        chunk.transform.SetParent(_rootLevelObject, false);
        chunk.transform.localPosition = new Vector3(0, 0, -(_chunksCreated * _chunkSize.y));
        chunk.GetComponent<MeshRenderer>().material.color = theme.ThemeColor;
        Chunk chunkData = new Chunk(_chunksCreated, chunk, theme);
        _chunks.Add(chunkData);
        _chunksCreated++;

        if(ChunkWasCreatedEvent != null)
        {
            ChunkWasCreatedEvent(chunkData);
        }
    }
}

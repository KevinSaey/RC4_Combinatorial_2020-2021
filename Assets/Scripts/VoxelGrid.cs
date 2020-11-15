using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class VoxelGrid
{
    #region Public fields

    public Voxel[,,] Voxels;
    public Vector3Int GridSize;
    public readonly float VoxelSize;
    public Corner[,,] Corners;
    public Face[][,,] Faces = new Face[3][,,];
    public Edge[][,,] Edges = new Edge[3][,,];
    public Vector3 Origin;
    public Vector3 Corner;

    #endregion

    #region private fields
    private bool _showVoxels = false;
    private GameObject _goVoxelPrefab;
    private List<Block> _blocks = new List<Block>();
    private PatternType _currentPattern = PatternType.PatternA;

    private List<Block> _currentBlocks => _blocks.Where(b => b.State != BlockState.Placed).ToList();
    #endregion

    #region Public dynamic getters
    public bool ShowVoxels
    {
        get
        {
            return _showVoxels;
        }
        set
        {
            foreach (var voxel in FlattenedVoxels)
            {
                voxel.ShowVoxel = value;
            }
            _showVoxels = value;

        }
    }
    /// <summary>
    /// Counts the number of blocks placed in the voxelgrid
    /// </summary>
    public int NumberOfBlocks => _blocks.Count(b => b.State == BlockState.Placed);
    /// <summary>
    /// what percentage of the grid has been filled up
    /// </summary>
    public float Efficiency
    {
        get
        {
            //if we don't cast this value to a float, it always returns 0 as it is rounding down to integer values
            return (float)FlattenedVoxels.Count(v => v.Status == VoxelState.Alive) / FlattenedVoxels.Count() * 100;
        }
    }

    private Dictionary<PatternType, GameObject> _goPatternPrefabs;
    public Dictionary<PatternType, GameObject> GOPatternPrefabs
    {
        get
        {
            if (_goPatternPrefabs == null)
            {
                _goPatternPrefabs = new Dictionary<PatternType, GameObject>();
                _goPatternPrefabs.Add(PatternType.PatternA, Resources.Load("Prefabs/PrefabPatternA") as GameObject);
                _goPatternPrefabs.Add(PatternType.PatternB, Resources.Load("Prefabs/PrefabPatternB") as GameObject);
            }
            return _goPatternPrefabs;
        }
    }

    /// <summary>
    /// Return the voxels in a flat list rather than a threedimensional array
    /// </summary>
    public IEnumerable<Voxel> FlattenedVoxels
    {
        get
        {
            for (int x = 0; x < GridSize.x; x++)
                for (int y = 0; y < GridSize.y; y++)
                    for (int z = 0; z < GridSize.z; z++)
                        yield return Voxels[x, y, z];
        }
    }

    public Voxel GetVoxelByIndex(Vector3Int index) => Voxels[index.x, index.y, index.z];
    /// <summary>
    /// Return all blocks that are not allready place in the grid
    /// </summary>
    
    #endregion
    
    #region constructor

    /// <summary>
    /// Constructor for the voxelgrid object. To be called in the Building manager
    /// </summary>
    /// <param name="gridDimensions">The dimensions of the grid</param>
    /// <param name="voxelSize">The size of one voxel</param>
    public VoxelGrid(Vector3Int gridDimensions, float voxelSize)
    {
        GridSize = gridDimensions;
        _goVoxelPrefab = Resources.Load("Prefabs/VoxelCube") as GameObject;
        VoxelSize = voxelSize;

        CreateVoxelGrid();
    }

    /// <summary>
    /// Generate the voxelgrid
    /// </summary>
    private void CreateVoxelGrid()
    {
        Voxels = new Voxel[GridSize.x, GridSize.y, GridSize.z];
        for (int x = 0; x < GridSize.x; x++)
        {
            for (int y = 0; y < GridSize.y; y++)
            {
                for (int z = 0; z < GridSize.z; z++)
                {
                    Voxels[x, y, z] = new Voxel(new Vector3Int(x, y, z), _goVoxelPrefab, this);
                }
            }
        }

        MakeFaces();
        MakeCorners();
        MakeEdges();
    }


    #region Grid elements constructors

    /// <summary>
    /// Creates the Faces of each <see cref="Voxel"/>
    /// </summary>
    private void MakeFaces()
    {
        // make faces
        Faces[0] = new Face[GridSize.x + 1, GridSize.y, GridSize.z];

        for (int x = 0; x < GridSize.x + 1; x++)
            for (int y = 0; y < GridSize.y; y++)
                for (int z = 0; z < GridSize.z; z++)
                {
                    Faces[0][x, y, z] = new Face(x, y, z, Axis.X, this);
                }

        Faces[1] = new Face[GridSize.x, GridSize.y + 1, GridSize.z];

        for (int x = 0; x < GridSize.x; x++)
            for (int y = 0; y < GridSize.y + 1; y++)
                for (int z = 0; z < GridSize.z; z++)
                {
                    Faces[1][x, y, z] = new Face(x, y, z, Axis.Y, this);
                }

        Faces[2] = new Face[GridSize.x, GridSize.y, GridSize.z + 1];

        for (int x = 0; x < GridSize.x; x++)
            for (int y = 0; y < GridSize.y; y++)
                for (int z = 0; z < GridSize.z + 1; z++)
                {
                    Faces[2][x, y, z] = new Face(x, y, z, Axis.Z, this);
                }
    }

    /// <summary>
    /// Creates the Corners of each Voxel
    /// </summary>
    private void MakeCorners()
    {
        Corner = new Vector3(Origin.x - VoxelSize / 2, Origin.y - VoxelSize / 2, Origin.z - VoxelSize / 2);

        Corners = new Corner[GridSize.x + 1, GridSize.y + 1, GridSize.z + 1];

        for (int x = 0; x < GridSize.x + 1; x++)
            for (int y = 0; y < GridSize.y + 1; y++)
                for (int z = 0; z < GridSize.z + 1; z++)
                {
                    Corners[x, y, z] = new Corner(new Vector3Int(x, y, z), this);
                }
    }

    /// <summary>
    /// Creates the Edges of each Voxel
    /// </summary>
    private void MakeEdges()
    {
        Edges[2] = new Edge[GridSize.x + 1, GridSize.y + 1, GridSize.z];

        for (int x = 0; x < GridSize.x + 1; x++)
            for (int y = 0; y < GridSize.y + 1; y++)
                for (int z = 0; z < GridSize.z; z++)
                {
                    Edges[2][x, y, z] = new Edge(x, y, z, Axis.Z, this);
                }

        Edges[0] = new Edge[GridSize.x, GridSize.y + 1, GridSize.z + 1];

        for (int x = 0; x < GridSize.x; x++)
            for (int y = 0; y < GridSize.y + 1; y++)
                for (int z = 0; z < GridSize.z + 1; z++)
                {
                    Edges[0][x, y, z] = new Edge(x, y, z, Axis.X, this);
                }

        Edges[1] = new Edge[GridSize.x + 1, GridSize.y, GridSize.z + 1];

        for (int x = 0; x < GridSize.x + 1; x++)
            for (int y = 0; y < GridSize.y; y++)
                for (int z = 0; z < GridSize.z + 1; z++)
                {
                    Edges[1][x, y, z] = new Edge(x, y, z, Axis.Y, this);
                }
    }

    #endregion
    #endregion

    #region Block functionality
    /// <summary>
    /// Temporary add a block to the grid. To confirm the block at it's current position, use the TryAddCurrentBlocksToGrid function
    /// </summary>
    /// <param name="anchor">The voxel where the pattern will start building from index(0,0,0) in the pattern</param>
    /// <param name="rotation">The rotation for the current block. This will be rounded to the nearest x,y or z axis</param>
    public void AddBlock(Vector3Int anchor, Quaternion rotation) => _blocks.Add(new Block(_currentPattern, anchor, rotation, this));

    /// <summary>
    /// Try to add the blocks that are currently pending to the grids
    /// </summary>
    /// <returns>true if the function managed to place all the current blocks. False in all other cases</returns>
    public bool TryAddCurrentBlocksToGrid()
    {
        if (_currentBlocks == null || _currentBlocks.Count == 0)
        {
            //Debug.LogWarning("No blocks to add");
            return false;
        }
        if (_currentBlocks.Count(b => b.State != BlockState.Valid) > 0)
        {
            //if we use $ in front of ", variables can be added inline between {} when defining a string
            //Debug.LogWarning($"{_currentBlocks.Count(b => b.State != BlockState.Valid)} blocks could not be place because their position is not valid");
            return false;
        }
        int counter = 0;
        //Keep adding blocks to the grid untill all the pending blocks are added
        while (_currentBlocks.Count > 0)
        {
            _currentBlocks.First().ActivateVoxels();
            counter++;
        }
        //Debug.Log($"Added {counter} blocks to the grid");
        return true;
    }

    /// <summary>
    /// Remove all pending blocks from the grid
    /// </summary>
    public void PurgeUnplacedBlocks()
    {
        _blocks.RemoveAll(b => b.State != BlockState.Placed);
    }

    public void PurgeAllBlocks()
    {
        foreach (var block in _blocks)
        {
            block.DestroyBlock();
        }
        _blocks = new List<Block>();
    }

    /// <summary>
    /// Set a random PatternType based on all the possible patterns in te PatternType Enum.
    /// </summary>
    public void SetRandomType()
    {
        PatternType[] values = System.Enum.GetValues(typeof(PatternType)).Cast<PatternType>().ToArray();
        _currentPattern = (PatternType)values[Random.Range(0, values.Length)];
    }
    #endregion
}

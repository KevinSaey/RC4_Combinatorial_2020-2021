using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VoxelGrid
{
    public static Voxel[,,] Voxels;
    public Vector3Int GridDimensions;

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
            return (float)GetFlattenedVoxels.Count(v => v.Status == VoxelState.Alive) / GetFlattenedVoxels.Count() * 100;
        }
    }

    public readonly float VoxelSize;

    private GameObject _goVoxelPrefab;

    /// <summary>
    /// Return all blocks that are not allready place in the grid
    /// </summary>
    private List<Block> _currentBlocks => _blocks.Where(b => b.State != BlockState.Placed).ToList();

    private readonly List<Block> _blocks = new List<Block>();
    private PatternType _currentPattern = PatternType.PatternA;

    /// <summary>
    /// Return the voxels in a flat list rather than a threedimensional array
    /// </summary>
    public IEnumerable<Voxel> GetFlattenedVoxels
    {
        get
        {
            for (int x = 0; x < GridDimensions.x; x++)
                for (int y = 0; y < GridDimensions.y; y++)
                    for (int z = 0; z < GridDimensions.z; z++)
                        yield return Voxels[x, y, z];
        }
    }

    /// <summary>
    /// Constructor for the voxelgrid object. To be called in the Building manager
    /// </summary>
    /// <param name="gridDimensions">The dimensions of the grid</param>
    /// <param name="voxelSize">The size of one voxel</param>
    public VoxelGrid(Vector3Int gridDimensions, float voxelSize)
    {
        GridDimensions = gridDimensions;
        _goVoxelPrefab = Resources.Load("Prefabs/VoxelCube") as GameObject;
        VoxelSize = voxelSize;

        CreateVoxelGrid();
    }

    /// <summary>
    /// Generate the voxelgrid
    /// </summary>
    private void CreateVoxelGrid()
    {
        Voxels = new Voxel[GridDimensions.x, GridDimensions.y, GridDimensions.z];
        for (int x = 0; x < GridDimensions.x; x++)
        {
            for (int y = 0; y < GridDimensions.y; y++)
            {
                for (int z = 0; z < GridDimensions.z; z++)
                {
                    Voxels[x, y, z] = new Voxel(new Vector3Int(x, y, z), _goVoxelPrefab, this);
                }
            }
        }
    }
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
            Debug.LogWarning("No blocks to add");
            return false;
        }
        if (_currentBlocks.Count(b => b.State != BlockState.Valid) > 0)
        {
            //if we use $ in front of ", variables can be added inline between {} when defining a string
            Debug.LogWarning($"{_currentBlocks.Count(b => b.State != BlockState.Valid)} blocks could not be place because their position is not valid");
            return false;
        }
        int counter = 0;
        //Keep adding blocks to the grid untill all the pending blocks are added
        while (_currentBlocks.Count > 0)
        {
            _currentBlocks.First().ActivateVoxels();
            counter++;
        }
        Debug.Log($"Added {counter} blocks to the grid");
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
            block.DeactivateVoxels();
        }
        _blocks.Clear();
    }
}

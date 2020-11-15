using System.Collections.Generic;
using UnityEngine;

public class Corner
{
    #region Public fields

    public Vector3 Position;
    public Vector3Int Index;

    #endregion

    #region Private fields

    protected VoxelGrid _grid;

    #endregion

    #region Constructor

    /// <summary>
    /// Constructs a Corner based on an index
    /// </summary>
    /// <param name="index">Index of the corner</param>
    /// <param name="grid"><see cref="VoxelGrid"/> to create the Corner in</param>
    public Corner(Vector3Int index, VoxelGrid grid)
    {
        _grid = grid;
        Index = index;
        Position = grid.Corner + new Vector3(index.x, index.y, index.z) * grid.VoxelSize;
    }

    public Corner(Corner corner) : this(corner.Index, corner._grid)
    {
        _grid.Corners[corner.Index.x, corner.Index.y, corner.Index.z] = this;
    }

    #endregion

    #region Public methods

    /// <summary>
    /// Get the Voxels connected to this corner
    /// </summary>
    /// <returns>All the Voxels</returns>
    public IEnumerable<Voxel> GetConnectedVoxels()
    {
        for (int zi = -1; zi <= 0; zi++)
        {
            int z = zi + Index.z;
            if (z == -1 || z == _grid.GridSize.z) continue;

            for (int yi = -1; yi <= 0; yi++)
            {
                int y = yi + Index.y;
                if (y == -1 || y == _grid.GridSize.y) continue;

                for (int xi = -1; xi <= 0; xi++)
                {
                    int x = xi + Index.x;
                    if (x == -1 || x == _grid.GridSize.x) continue;

                    var i = new Vector3Int(x, y, z);

                    yield return _grid.Voxels[x, y, z];
                }
            }
        }
    }

    #endregion
}
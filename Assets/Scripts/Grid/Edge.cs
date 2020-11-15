using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Edge
{
    #region Public fields

    public Vector3Int Index;
    public Axis Direction;
    public Vector3 Center;
    public Voxel[] Voxels;
    public Face[] Faces;
    public Face[] ClimbableFaces => Faces.Where(f => f?.IsSkin == true).ToArray();

    /// <summary>
    /// Get the normal of this edge
    /// </summary>
    public Vector3 Normal
    {
        get
        {
            Vector3 normal = Vector3.zero;

            foreach (var face in Faces.Where(f => f != null))
                normal += face.Normal;

            return normal.normalized;
        }
    }

    #endregion

    #region Private fields

    VoxelGrid _grid;

    #endregion

    #region Constructors

    /// <summary>
    /// Constructs the Edge element based on its Index and parent <see cref="VoxelGrid"/>
    /// </summary>
    /// <param name="x">X Index</param>
    /// <param name="y">Y Index</param>
    /// <param name="z">Z Index</param>
    /// <param name="direction">Direction Axis</param>
    /// <param name="grid">Parent <see cref="VoxelGrid"/></param>
    public Edge(int x, int y, int z, Axis direction, VoxelGrid grid)
    {
        _grid = grid;
        Index = new Vector3Int(x, y, z);
        Direction = direction;
        Center = GetCenter();
        Voxels = GetVoxels();
        Faces = GetFaces();
    }

    #endregion

    #region Public methods

    /// <summary>
    /// Get the center of the Edge
    /// </summary>
    /// <returns>The center <see cref="Vector3"/></returns>
    Vector3 GetCenter()
    {
        int x = Index.x;
        int y = Index.y;
        int z = Index.z;

        switch (Direction)
        {
            case Axis.X:
                return _grid.Corner + new Vector3(x + 0.5f, y, z) * _grid.VoxelSize;
            case Axis.Y:
                return _grid.Corner + new Vector3(x, y + 0.5f, z) * _grid.VoxelSize;
            case Axis.Z:
                return _grid.Corner + new Vector3(x, y, z + 0.5f) * _grid.VoxelSize;
            default:
                throw new Exception("Wrong direction.");
        }
    }

    /// <summary>
    /// Get the Voxels associated with this Edge
    /// </summary>
    /// <returns>The voxels' array</returns>
    Voxel[] GetVoxels()
    {
        int x = Index.x;
        int y = Index.y;
        int z = Index.z;

        switch (Direction)
        {
            case Axis.X:
                return new[]
                {
                   (z == 0 || y == 0) ? null : _grid.Voxels[x, y - 1, z - 1],
                   (z == _grid.GridSize.z || y == 0) ? null : _grid.Voxels[x, y - 1, z],
                   (z == 0 || y == _grid.GridSize.y) ? null : _grid.Voxels[x, y, z - 1],
                   (z == _grid.GridSize.z || y == _grid.GridSize.y) ? null : _grid.Voxels[x, y, z]
                 };
            case Axis.Y:
                return new[]
                {
                   (x == 0 || z == 0) ? null : _grid.Voxels[x - 1, y, z - 1],
                   (x == _grid.GridSize.x || z == 0) ? null : _grid.Voxels[x, y, z - 1],
                   (x == 0 || z == _grid.GridSize.z) ? null : _grid.Voxels[x - 1, y, z],
                   (x == _grid.GridSize.x || z == _grid.GridSize.z) ? null : _grid.Voxels[x, y, z]
                };
            case Axis.Z:
                return new[]
                {
                   (x == 0 || y == 0) ? null : _grid.Voxels[x - 1, y - 1, z],
                   (x == _grid.GridSize.x || y == 0) ? null : _grid.Voxels[x, y - 1, z],
                   (x == 0 || y == _grid.GridSize.y) ? null : _grid.Voxels[x - 1, y, z],
                   (x == _grid.GridSize.x || y == _grid.GridSize.y) ? null : _grid.Voxels[x, y, z]
                };
            default:
                throw new Exception("Wrong direction.");
        }
    }

    /// <summary>
    /// Get the Faces associated with this Edge
    /// </summary>
    /// <returns>The Faces' array</returns>
    Face[] GetFaces()
    {
        int x = Index.x;
        int y = Index.y;
        int z = Index.z;

        switch (Direction)
        {
            case Axis.X:
                return new[]
                {
                    y == 0 ? Voxels[2]?.Faces[2] : Voxels[0]?.Faces[3],
                    y == 0 ? Voxels[3]?.Faces[2] : Voxels[1]?.Faces[3],
                    z == 0 ? Voxels[1]?.Faces[4] : Voxels[0]?.Faces[5],
                    z == 0 ? Voxels[3]?.Faces[4] : Voxels[2]?.Faces[5],
                };
            case Axis.Y:
                return new[]
                {
                    x == 0 ? Voxels[1]?.Faces[0] : Voxels[0]?.Faces[1],
                    x == 0 ? Voxels[3]?.Faces[0] : Voxels[2]?.Faces[1],
                    z == 0 ? Voxels[2]?.Faces[4] : Voxels[0]?.Faces[5],
                    z == 0 ? Voxels[3]?.Faces[4] : Voxels[1]?.Faces[5],
                };
            case Axis.Z:
                return new[]
                {
                    x == 0 ? Voxels[1]?.Faces[0] : Voxels[0]?.Faces[1],
                    x == 0 ? Voxels[3]?.Faces[0] : Voxels[2]?.Faces[1],
                    y == 0 ? Voxels[2]?.Faces[2] : Voxels[0]?.Faces[3],
                    y == 0 ? Voxels[3]?.Faces[2] : Voxels[1]?.Faces[3],
                };
            default:
                throw new Exception("Wrong direction.");
        }
    }

    #endregion
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class Face
{
    //Implemented and adapted from https://github.com/ADRC4/Voxel

    /// <summary>
    /// Type of the face based on the position on the grid
    /// </summary>
    public enum BoundaryType { Inside = 0, Left = 1, Right = 2, Outside = 3 };

    #region Public fields

    public Voxel[] Voxels;
    public Vector3Int Index;
    public Vector3 Center;
    public Axis Direction;

    /// <summary>
    /// Get the type of the face based on its position on the grid
    /// </summary>
    public BoundaryType Boundary
    {
        get
        {
            bool left = Voxels[0] != null;
            bool right = Voxels[1] != null;

            if (!left && right) return BoundaryType.Left;
            if (left && !right) return BoundaryType.Right;
            if (left && right) return BoundaryType.Inside;
            return BoundaryType.Outside;
        }
    }

    /// <summary>
    /// Get the normal <see cref="Vector3"/> of the Face
    /// </summary>
    public Vector3 Normal
    {
        get
        {
            int f = (int)Boundary;
            if (Boundary == BoundaryType.Outside) f = 0;

            if (Index.y == 0 && Direction == Axis.Y)
            {
                f = Boundary == BoundaryType.Outside ? 1 : 0;
            }

            switch (Direction)
            {
                case Axis.X:
                    return Vector3.right * f;
                case Axis.Y:
                    return Vector3.up * f;
                case Axis.Z:
                    return Vector3.forward * f;
                default:
                    throw new Exception("Wrong direction.");
            }
        }
    }

    /// <summary>
    /// Bool determining if the face is part of the skin of the grid
    /// </summary>
    public bool IsSkin
    {
        get
        {
            if (Index.y == 0 && Direction == Axis.Y)
            {
                return Boundary == BoundaryType.Outside;
            }

            return Boundary == BoundaryType.Left || Boundary == BoundaryType.Right;
        }
    }

    #endregion

    #region Private fields

    VoxelGrid _grid;

    #endregion

    #region Constructor

    /// <summary>
    /// Creates a Face
    /// </summary>
    /// <param name="x">X Index</param>
    /// <param name="y">Y Index</param>
    /// <param name="z">Z Index</param>
    /// <param name="direction">Direction Axis</param>
    /// <param name="grid">Grid to create the Face in</param>
    public Face(int x, int y, int z, Axis direction, VoxelGrid grid)
    {
        _grid = grid;
        Index = new Vector3Int(x, y, z);
        Direction = direction;
        Voxels = GetVoxels();

        foreach (var v in Voxels.Where(v => v != null))
            v.Faces.Add(this);

        Center = GetCenter();
    }

    #endregion

    #region Private methods

    /// <summary>
    /// Get the Voxels associated with this face
    /// </summary>
    /// <returns>The voxels as an Array</returns>
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
                   x == 0 ? null : _grid.Voxels[x - 1, y, z],
                   x == _grid.GridSize.x ? null : _grid.Voxels[x, y, z]
                };
            case Axis.Y:
                return new[]
                {
                   y == 0 ? null : _grid.Voxels[x, y - 1, z],
                   y == _grid.GridSize.y ? null : _grid.Voxels[x, y, z]
                };
            case Axis.Z:
                return new[]
                {
                   z == 0 ? null : _grid.Voxels[x, y, z - 1],
                   z == _grid.GridSize.z ? null : _grid.Voxels[x, y, z]
                };
            default:
                throw new Exception("Wrong direction.");
        }
    }

    /// <summary>
    /// Get the center <see cref="Vector3"/> of the face
    /// </summary>
    /// <returns>The center</returns>
    Vector3 GetCenter()
    {
        int x = Index.x;
        int y = Index.y;
        int z = Index.z;

        switch (Direction)
        {
            case Axis.X:
                return _grid.Origin + new Vector3(x, y + 0.5f, z + 0.5f) * _grid.VoxelSize;
            case Axis.Y:
                return _grid.Origin + new Vector3(x + 0.5f, y, z + 0.5f) * _grid.VoxelSize;
            case Axis.Z:
                return _grid.Origin + new Vector3(x + 0.5f, y + 0.5f, z) * _grid.VoxelSize;
            default:
                throw new Exception("Wrong direction.");
        }
    }

    #endregion
}
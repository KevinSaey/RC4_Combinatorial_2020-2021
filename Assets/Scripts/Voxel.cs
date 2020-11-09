using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum VoxelState { Dead = 0, Alive = 1}
public class Voxel
{
    private Vector3Int _index;
    private GameObject _goVoxel;
    private VoxelState _voxelStatus;
    private VoxelGrid _grid;

    /// <summary>
    /// Get the centre point of the voxel in worldspace
    /// </summary>
    private Vector3 _centre => (Vector3)_index * _grid.VoxelSize + Vector3.one * 0.5f * _grid.VoxelSize;

    /*private Vector3 _centre
    {
        get
        {
            return (Vector3)_index * _grid.VoxelSize + Vector3.one * 0.5f * _grid.VoxelSize;
        }
    }*/

   /// <summary>
   /// Get and set the status of the voxel. When setting the status, the linked gameobject will be enable or disabled depending on the state.
   /// </summary>
    public VoxelState Status
    {
        get
        {
            return _voxelStatus;
        }
        set
        {
            _goVoxel.SetActive(value == VoxelState.Alive);
            _voxelStatus = value;
        }
    }

    /// <summary>
    /// Voxel constructor. Will construct a voxel object and a Gameobject linked to this to the voxelgrid.
    /// </summary>
    /// <param name="index">index of the voxel</param>
    /// <param name="goVoxel">prefab of the voxel gameobject</param>
    public Voxel (Vector3Int index, GameObject goVoxel, VoxelGrid grid)
    {
        _grid = grid;
        _index = index;
        _goVoxel = GameObject.Instantiate(goVoxel, _centre, Quaternion.identity);
        _goVoxel.GetComponent<VoxelTrigger>().TriggerVoxel = this;
        _goVoxel.transform.localScale = Vector3.one * _grid.VoxelSize*0.95f;
        Status = VoxelState.Dead;
    }
}

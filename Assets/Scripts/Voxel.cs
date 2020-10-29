using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum VoxelState { Dead = 0, Alive = 1}
public class Voxel
{
    private Vector3Int _index;
    private VoxelGrid _voxelGrid;
    private GameObject _goVoxel;
    private VoxelState _status = VoxelState.Alive;
    public VoxelState Status
    {
        get
        {
            return _status;
        }
        set
        {
            _goVoxel.SetActive(value == VoxelState.Alive);
            _status = value;
        }
    }

    public Voxel (Vector3Int index, GameObject goVoxel, VoxelGrid grid)
    {
        _index = index;
        _voxelGrid = grid;
        _goVoxel = GameObject.Instantiate(goVoxel, _index, Quaternion.identity);
        _goVoxel.GetComponent<VoxelTrigger>().TriggerVoxel = this;
    }
}

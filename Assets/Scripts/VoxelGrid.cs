using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelGrid : MonoBehaviour
{
    public Vector3Int GridDimensions = new Vector3Int(10, 20, 5);
    private Voxel[,,] _voxels;
    private GameObject _goVoxelPrefab;

    // Start is called before the first frame update
    void Start()
    {
        _goVoxelPrefab = Resources.Load("Prefabs/VoxelCube") as GameObject;
        CreateVoxelGrid();
    }

    void CreateVoxelGrid()
    {
        _voxels = new Voxel[GridDimensions.x, GridDimensions.y, GridDimensions.z];
        for (int x = 0; x < GridDimensions.x; x++)
        {
            for (int y = 0; y < GridDimensions.y; y++)
            {
                for (int z = 0; z < GridDimensions.z; z++)
                {
                    _voxels[x, y, z] = new Voxel(new Vector3Int(x, y, z), _goVoxelPrefab, this);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            HandleRaycast(Input.mousePosition);
        }
    }
    private void HandleRaycast(Vector3 screenPosition)
    {

        Ray ray = Camera.main.ScreenPointToRay(screenPosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {

            Transform objectHit = hit.transform;
            if (objectHit.CompareTag("Voxel"))
            {
                objectHit.gameObject.GetComponent<VoxelTrigger>().TriggerVoxel.Status = VoxelState.Dead;
            }
        }
    }
}

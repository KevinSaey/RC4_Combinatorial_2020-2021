using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Steps:
// Scale and position voxelgrid according to voxelsize
// Add block class
// Add pattern class
// Make voxelgrid a singleton - explain concept


public class BuildingManager : MonoBehaviour
{
    [SerializeField]
    private Vector3Int _gridDimensions = new Vector3Int(10, 20, 5);
    [SerializeField]
    private float _voxelSize = 0.2f;

    public VoxelGrid VGrid;

    public VoxelGrid CreateVoxelGrid(Vector3Int gridDimensions, float voxelSize, Vector3 origin)
    {
        VGrid = new VoxelGrid(gridDimensions, voxelSize, origin);
        return VGrid;
    }

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

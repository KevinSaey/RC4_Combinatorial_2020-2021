using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BruteForceFiller : MonoBehaviour
{
    int _counter = 0;
    int _tries = 1000;
    bool generating=false;
    /// <summary>
    /// Generate a random index within the voxelgrid
    /// </summary>
    /// <returns>The index</returns>
    Vector3Int RandomIndex()
    {
        int x = Random.Range(0, VoxelGrid.Instance.GridDimensions.x);
        int y = Random.Range(0, VoxelGrid.Instance.GridDimensions.y);
        int z = Random.Range(0, VoxelGrid.Instance.GridDimensions.z);
        return new Vector3Int(x, y, z);
    }

    /// <summary>
    /// Get a random rotation alligned with the x,y or z axis
    /// </summary>
    /// <returns>The rotation</returns>
    Quaternion RandomRotation()
    {
        int x = Random.Range(0, 4) * 90;
        int y = Random.Range(0, 4) * 90;
        int z = Random.Range(0, 4) * 90;
        return Quaternion.Euler(x, y, z);
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            generating = true;
            // TryAddRandomBlock();
            StartCoroutine(BruteForce());
        }
    }
    /// OnGUI is used to display all the scripted graphic user interface elements in the Unity loop
    private void OnGUI()
    {
        
        int padding = 10;
        int labelHeight=50;
        int labelWidth = 150;
        int counter = 0;

        if (generating)
        {
            GUI.Label(new Rect(padding, (padding + counter++) * labelHeight, labelWidth, labelHeight),
                $"Grid {VoxelGrid.Instance.Efficiency} % filled");
            GUI.Label(new Rect(padding, (padding + counter++) * labelHeight, labelWidth, labelHeight),
                $"Grid {VoxelGrid.Instance.NumberOfBlocks} Blocks added");
        }
    }

    /// <summary>
    /// Method to test adding one block to the brid
    /// </summary>
    private void BlockTest()
    {
        var anchor = new Vector3Int(2, 8, 0);
        var rotation = Quaternion.Euler(0, 0, -90);
        VoxelGrid.Instance.AddBlock                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                            (anchor, rotation);
        VoxelGrid.Instance.TryAddCurrentBlocksToGrid();
    }

    /// <summary>
    /// Method to add a random block to the grid
    /// </summary>
    /// <returns>returns true if it managed to add the block to the grid</returns>
    private bool TryAddRandomBlock()
    {
        VoxelGrid.Instance.AddBlock(RandomIndex(), RandomRotation());
        bool blockAdded = VoxelGrid.Instance.TryAddCurrentBlocksToGrid();
        VoxelGrid.Instance.PurgeUnplacedBlocks();
        return blockAdded;
    }

    /// <summary>
    /// Try adding a random block to the grid every given time. This will run as much times as defined in the _tries field
    /// </summary>
    /// <returns>Wait 0.01 seconds between each iteration</returns>
    IEnumerator BruteForce()
    {
        while(_counter<_tries)
        {
            TryAddRandomBlock();
            _counter++;
            yield return new WaitForSeconds(0.01f);
        }
    }

}

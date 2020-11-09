using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BruteForceFiller : MonoBehaviour
{
    int _tryCounter = 0;
    int _iterationCounter = 0;
    int _triesPerIteration = 1000;
    int _iterations = 100;
    bool generating = false;
    int _seed = 0;

    private BuildingManager _buildingManager;
    private VoxelGrid _grid;
    public VoxelGrid VGrid
    {
        get
        {
            if (_grid == null)
            {
                GameObject manager = GameObject.Find("Manager");
                _buildingManager = manager.GetComponent<BuildingManager>();
                _grid = _buildingManager.VGrid;
            }
            return _grid;
        }
    }

    /// <summary>
    /// Generate a random index within the voxelgrid
    /// </summary>
    /// <returns>The index</returns>
    Vector3Int RandomIndex()
    {
        int x = Random.Range(0, VGrid.GridDimensions.x);
        int y = Random.Range(0, VGrid.GridDimensions.y);
        int z = Random.Range(0, VGrid.GridDimensions.z);
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
        Random.seed = _seed;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            generating = true;
            // TryAddRandomBlock();
            //StartCoroutine(BruteForce());
            //BruteForceStep();
            StartCoroutine(BruteForceEngine());
        }
    }
    /// OnGUI is used to display all the scripted graphic user interface elements in the Unity loop
    private void OnGUI()
    {

        int padding = 10;
        int labelHeight = 50;
        int labelWidth = 150;
        int counter = 0;

        if (generating)
        {
            GUI.Label(new Rect(padding, (padding + counter++) * labelHeight, labelWidth, labelHeight),
                $"Grid {VGrid.Efficiency} % filled");
            GUI.Label(new Rect(padding, (padding + counter++) * labelHeight, labelWidth, labelHeight),
                $"Grid {VGrid.NumberOfBlocks} Blocks added");
        }
    }

    /// <summary>
    /// Method to test adding one block to the brid
    /// </summary>
    private void BlockTest()
    {
        var anchor = new Vector3Int(2, 8, 0);
        var rotation = Quaternion.Euler(0, 0, -90);
        VGrid.AddBlock(anchor, rotation);
        VGrid.TryAddCurrentBlocksToGrid();
    }

    /// <summary>
    /// Method to add a random block to the grid
    /// </summary>
    /// <returns>returns true if it managed to add the block to the grid</returns>
    private bool TryAddRandomBlock()
    {
        VGrid.AddBlock(RandomIndex(), RandomRotation());
        bool blockAdded = VGrid.TryAddCurrentBlocksToGrid();
        VGrid.PurgeUnplacedBlocks();
        return blockAdded;
    }

    /// <summary>
    /// Try adding a random block to the grid every given time. This will run as much times as defined in the _tries field
    /// </summary>
    /// <returns>Wait 0.01 seconds between each iteration</returns>
    IEnumerator BruteForce()
    {
        while (_tryCounter < _triesPerIteration)
        {
            TryAddRandomBlock();
            _tryCounter++;
            yield return new WaitForSeconds(0.01f);
        }
    }

    private void BruteForceStep()
    {
        while (_tryCounter < _triesPerIteration)
        {
            TryAddRandomBlock();
            _tryCounter++;
        }
    }

    IEnumerator BruteForceEngine()
    {
        

        while (_iterationCounter < _iterations)
        {
            VGrid.PurgeAllBlocks();
            Random.seed = _seed++;
            BruteForceStep();
            _iterationCounter++;
            yield return new WaitForSeconds(0.01f);
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem.XR.Haptics;

public class BruteForceFiller : MonoBehaviour
{
    int _tryCounter = 0;
    int _iterationCounter = 0;
    int _triesPerIteration = 2000;
    int _iterations = 500;
    bool generating = false;
    int _seed = 0;

    private Dictionary<int, float> _efficiencies = new Dictionary<int, float>();
    List<int> orderedEfficiencyIndex = new List<int>();
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
            if (!generating)
            {
                generating = true;
                // TryAddRandomBlock();
                //StartCoroutine(BruteForce());
                //BruteForceStep();
                StartCoroutine(BruteForceEngine());
            }
            else
            {
                generating = false;
                StopAllCoroutines();
            }
        }
    }
    /// OnGUI is used to display all the scripted graphic user interface elements in the Unity loop
    private void OnGUI()
    {

        int padding = 10;
        int labelHeight = 20;
        int labelWidth = 150;
        int counter = 0;

        if (generating)
        {
            GUI.Label(new Rect(padding, (padding + labelHeight) * ++counter, labelWidth, labelHeight),
                $"Grid {VGrid.Efficiency} % filled");
            GUI.Label(new Rect(padding, (padding + labelHeight) * ++counter, labelWidth, labelHeight),
                $"Grid {VGrid.NumberOfBlocks} Blocks added");
        }
        Debug.LogWarning("here");
        for (int i = 0; i < Mathf.Min(orderedEfficiencyIndex.Count,10); i++)
        {
            string text = $"Seed: {orderedEfficiencyIndex[i]} Efficiency: {_efficiencies[orderedEfficiencyIndex[i]]}";
            GUI.Label(new Rect(padding, (padding + labelHeight) * ++counter, labelWidth, labelHeight),
               text);
            
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
        VGrid.PurgeAllBlocks();
        _tryCounter = 0;
        while (_tryCounter < _triesPerIteration)
        {
            TryAddRandomBlock();
            _tryCounter++;
        }

        _efficiencies.Add(_seed, _grid.Efficiency);
        orderedEfficiencyIndex = _efficiencies.Keys.OrderByDescending(k => _efficiencies[k]).Take(11).ToList();
        if(orderedEfficiencyIndex.Count == 11)
            _efficiencies.Remove(orderedEfficiencyIndex[10]);

        
    }

    IEnumerator BruteForceEngine()
    {
        while (_iterationCounter < _iterations)
        {
            Random.seed = _seed++;
            BruteForceStep();
            _iterationCounter++;
            yield return new WaitForSeconds(0.05f);
        }

        foreach (var value in _efficiencies.Values)
        {
            Debug.Log(value);
        }
    }

}

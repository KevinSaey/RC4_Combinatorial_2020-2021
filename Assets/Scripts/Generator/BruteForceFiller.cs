using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BruteForceFiller : MonoBehaviour
{
    private float _voxelSize = 0.2f;
    private int _voxelOffset = 2;
    private int _triesPerIteration = 2500;
    private int _iterations = 100;

    private int _tryCounter = 0;
    private int _iterationCounter = 0;

    private bool generating = false;
    private int _seed = 0;

    private List<int> test = new List<int>();

    private Dictionary<int, float> _efficiencies = new Dictionary<int, float>();
    private List<int> orderedEfficiencyIndex = new List<int>();
    private BuildingManager _buildingManager;
    private VoxelGrid _grid;

    public BuildingManager BManager
    {
        get
        {
            if (_buildingManager == null)
            {
                GameObject manager = GameObject.Find("Manager");
                _buildingManager = manager.GetComponent<BuildingManager>();
            }
            return _buildingManager;
        }
    }

    /// <summary>
    /// Generate a random index within the voxelgrid
    /// </summary>
    /// <returns>The index</returns>
    Vector3Int RandomIndex()
    {
        int x = Random.Range(0, _grid.GridSize.x);
        int y = Random.Range(0, _grid.GridSize.y);
        int z = Random.Range(0, _grid.GridSize.z);
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
        _grid = BManager.CreateVoxelGrid(BoundingMesh.GetGridDimensions(_voxelOffset, _voxelSize), _voxelSize, BoundingMesh.GetOrigin(_voxelOffset, _voxelSize));
        Debug.Log(_grid.GridSize);
        _grid.DisableOutsideBoundingMesh();
        Random.seed = _seed;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            //TryAddRandomBlock();

            if (!generating)
            {
                generating = true;

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
        if (Input.GetKeyDown("r")) _grid.SetRandomType();

    }
    /// OnGUI is used to display all the scripted graphic user interface elements in the Unity loop
    private void OnGUI()
    {
        int padding = 10;
        int labelHeight = 20;
        int labelWidth = 250;
        int counter = 0;

        if (generating)
        {
            _grid.ShowVoxels = GUI.Toggle(new Rect(padding, (padding + labelHeight) * ++counter, labelWidth, labelHeight), _grid.ShowVoxels, "Show voxels");

            GUI.Label(new Rect(padding, (padding + labelHeight) * ++counter, labelWidth, labelHeight),
                $"Grid {_grid.Efficiency} % filled");
            GUI.Label(new Rect(padding, (padding + labelHeight) * ++counter, labelWidth, labelHeight),
                $"Grid {_grid.NumberOfBlocks} Blocks added");
        }
        for (int i = 0; i < Mathf.Min(orderedEfficiencyIndex.Count, 10); i++)
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
        _grid.AddBlock(anchor, rotation);
        _grid.TryAddCurrentBlocksToGrid();
    }

    /// <summary>
    /// Method to add a random block to the grid
    /// </summary>
    /// <returns>returns true if it managed to add the block to the grid</returns>
    private bool TryAddRandomBlock()
    {
        _grid.SetRandomType();
        _grid.AddBlock(RandomIndex(), RandomRotation());
        bool blockAdded = _grid.TryAddCurrentBlocksToGrid();
        Debug.Log("HEllo");
        _grid.PurgeUnplacedBlocks();
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

    /// <summary>
    /// Brute force random blocks in the available grid
    /// </summary>
    private void BruteForceStep()
    {
        _grid.PurgeAllBlocks();
        _tryCounter = 0;
        while (_tryCounter < _triesPerIteration)
        {
            TryAddRandomBlock();
            _tryCounter++;
        }

        //Keep track of the most efficient seeds
        _efficiencies.Add(_seed, _grid.Efficiency);
        orderedEfficiencyIndex = _efficiencies.Keys.OrderByDescending(k => _efficiencies[k]).Take(11).ToList();
        if (orderedEfficiencyIndex.Count == 11)
            _efficiencies.Remove(orderedEfficiencyIndex[10]);


    }

    /// <summary>
    /// Brute force an entire iteration every tick
    /// </summary>
    /// <returns></returns>
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

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoundingMesh
{
    #region Private fields
    private IEnumerable<MeshCollider> _meshColliders;
    #endregion

    #region Public fields
    public Bounds MeshBounds;

    #endregion

    public BoundingMesh()
    {
        GameObject[] boundingMeshes = GameObject.FindGameObjectsWithTag("BoundingMesh");
        _meshColliders = boundingMeshes.Select(g => g.GetComponent<MeshCollider>());

        Bounds meshBounds = new Bounds();
        foreach (var collider in _meshColliders)
        {
            meshBounds.Encapsulate(collider.bounds);
        }
        MeshBounds = meshBounds;
    }

    public bool IsInside(Voxel voxel)
    {
        Physics.queriesHitBackfaces = true;

        var point = voxel.Centre;
        var sortedHits = new Dictionary<Collider, int>();
        foreach (var collider in _meshColliders)
            sortedHits.Add(collider, 0);

        while (Physics.Raycast(new Ray(point, Vector3.forward), out RaycastHit hit))
        {
            var collider = hit.collider;

            if (sortedHits.ContainsKey(collider))
                sortedHits[collider]++;

            point = hit.point + Vector3.forward * 0.00001f;
        }

        bool isInside = sortedHits.Any(kv => kv.Value % 2 != 0);
        return isInside;
    }
}

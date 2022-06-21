using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class VoxelMapShading : VoxelMap
{   
    Mesh mesh;

    private List<Vector3> points;
    private List<int> indices;
    private List<Color> colors;

    //TODO: points are being rendered, add size viz. the second project
    public override void AddVoxel(Vector3 location)
    {
        //TODO: struct + only one list
        points.Add(location);
        indices.Add(indices.Count);
        colors.Add(Color.cyan);
        
        //assign values to renderer
        //TODO: possibly not after every voxel added
        mesh.vertices = points.ToArray();
        mesh.SetIndices(indices.ToArray(), MeshTopology.Points, 0);
        mesh.colors = colors.ToArray();
    }

    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        points = new List<Vector3>();
        indices = new List<int>();
        colors = new List<Color>();
    }
}

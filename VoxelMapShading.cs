using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class VoxelMapShading : VoxelMap
{
    
    private Voxel[,,] voxels;
    
    //rendering vertex data
    Mesh mesh;
    private List<Vector3> points;   //point in worldspace
    private List<int> indices;
    private List<Color> colors;     //color based on voxel color
    
    //map preferences
    public bool visualizeWeigth;    //render voxel colors according to weigth

    //TODO: points are being rendered, add size pcd shader to enlarge them
    public override void AddVoxel(int3 location)
    {
        voxels[location.x, location.y, location.z] = new Voxel();
         
        points.Add(getWorldPos(location));
        indices.Add(indices.Count);
        colors.Add(voxels[location.x, location.y, location.z].color);
        
        //assign values to renderer
        //TODO: possibly not after every voxel added
        
        
    }

    public override void ObserveVoxel(int3 location, Color color, bool surface)
    {
        
        if (voxels[location.x, location.y, location.z] == null)
        {
            voxels[location.x, location.y, location.z] = new Voxel(surface, color);
        }
        else
        {
            Voxel vox = voxels[location.x, location.y, location.z];
            //voxel observed again
            if (surface == vox.surface)
            {
                vox.incrementWeight();
                vox.color = Color.Lerp(vox.color, color, vox.weight); //mix colors according to weight
            }
            else
            {
                vox.decrementWeight();
                if (vox.weight <= 0.1f)
                {
                    vox.surface = !vox.surface;
                    vox.weight = 0.1f;
                }
            }
        }
    }

    /// <summary>
    /// Update rendered object
    /// </summary>
    private int surfaceVoxels = 0;
    public override void Render()
    {
        //MOST TERRIBLY SLOW, temporary. should not loop over empty space    
        for (int i = 0; i <= voxels.GetUpperBound(0); i++)
        {
            for (int j = 0; j <= voxels.GetUpperBound(1); j++)
            {
                for (int k = 0; k <= voxels.GetUpperBound(2); k++)
                {
                    if (voxels[i, j, k] != null)
                    {
                        if (voxels[i, j, k].surface)    //TODO: weight condition?
                        {
                            colors.Add(voxels[i, j, k].color);
                            points.Add(getWorldPos(new int3(i,j,k)));
                            indices.Add(i);
                        }
                    }
                }
            }
        }
        
        mesh.vertices = points.ToArray();
        mesh.colors = colors.ToArray();
        mesh.SetIndices(indices.ToArray(), MeshTopology.Points, 0);
        
        Debug.Log("Map rendered, containing " + points.Count + " points. " + "");
        
        //clear 
        points = new List<Vector3>();
        indices = new List<int>();
        colors = new List<Color>();
    }
    
    public override void Clear()
    {
        points = new List<Vector3>();
        indices = new List<int>();
        colors = new List<Color>();
        
        mesh.Clear();
    }

    public override Vector3 getWorldPos(int3 index)
    {
        return new Vector3(index.x, index.y, index.z) * voxelSize;
    }

    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        points = new List<Vector3>();
        indices = new List<int>();
        colors = new List<Color>();

        voxels = new Voxel[mapSize.x, mapSize.y, mapSize.z];
    }
}

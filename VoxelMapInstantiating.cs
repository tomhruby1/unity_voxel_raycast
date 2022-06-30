using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public class VoxelMapInstantiating : VoxelMap
{
    public GameObject voxel; //voxel prefab to be instantiated

    private Voxel[,,] voxels;
    
    //map preferences
    public bool visualizeWeigth;    //render voxel colors according to weigth

    //TODO: points are being rendered, add size pcd shader to enlarge them
    public override void AddVoxel(int3 location)
    {
        voxels[location.x, location.y, location.z] = new Voxel();
         
        GameObject currentVoxel = GameObject.Instantiate(voxel, getWorldPos(location), 
            Quaternion.identity, this.transform);
        currentVoxel.transform.localScale *= voxelSize;
    }
    
    //integrate observation of a voxel given its position, color and surface status
    public override void ObserveVoxel(int3 location, Color color, bool surface)
    {
        if (voxels[location.x, location.y, location.z] == null)
        {
            voxels[location.x, location.y, location.z] = new Voxel(surface);
        }
        else
        {
            Voxel vox = voxels[location.x, location.y, location.z];
            //voxel observed again
            if(surface == vox.surface)
                vox.incrementWeight();
            else
            {
                vox.decrementWeight();
                if (vox.weight <= 0.1f)
                {
                    vox.surface = !vox.surface;
                    vox.weight = 0.1f;
                }
            }
            /*
            if(visualizeWeigth)//visualize weight in color
                voxels[location.x, location.y, location.z].color = Color.Lerp(Color.cyan, Color.magenta,
                    voxels[location.x, location.y, location.z].weight);
            */
        }
        //TESTING, SHOULD NOT BELONG HERE??!
        if(surface) 
        {
            //Debug.Log("INSTANTIATING CUBE ");
            GameObject currentVoxel = GameObject.Instantiate(voxel, getWorldPos(location), 
                Quaternion.identity, this.transform);
            currentVoxel.transform.localScale *= voxelSize;
        }
    }
    
    /// <summary>
    /// Update rendered object
    /// </summary>
    public override void Render()
    {
        
       //display voxels with weight > T TODO: how to do it without super expensive looping through
       /*
       if (surface && voxels[location.x, location.y, location.z].weight > 0.2f) ;
       {
           GameObject currentVoxel = GameObject.Instantiate(voxel, getWorldPos(location), 
               Quaternion.identity, this.transform);
           currentVoxel.transform.localScale *= voxelSize;
       }
       */
    }
    public override Vector3 getWorldPos(int3 index)
    {
        return new Vector3(boundStart.x, boundStart.y, boundStart.z) + new Vector3(index.x, index.y, index.z) * voxelSize; 
    }
    public override void Clear()
    {
        for (int i = 0; i < transform.childCount; i++)
            Destroy(transform.GetChild(i).gameObject);
    }
    
    void Start()
    {
        voxels = new Voxel[mapSize.x, mapSize.y, mapSize.z];
    }
}

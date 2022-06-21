using System.Collections;
using System.Collections.Generic;
using unity_voxel_raycast;
using UnityEngine;
using Unity.Mathematics;
public class VoxelMapInstantiating : VoxelMap
{
    public GameObject voxel; //voxel prefab to be instantiated

    public override void AddVoxel(Vector3 location) 
    {
        GameObject currentVoxel = GameObject.Instantiate(voxel, location, Quaternion.identity, this.transform);
        currentVoxel.transform.localScale *= voxelSize;
    }
    
 

}

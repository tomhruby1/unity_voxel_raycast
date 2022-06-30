using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

/// <summary>
/// Point cloud raycasting visualizer to be used manually between two points
/// </summary>
public class RaycastDemonstrator : MonoBehaviour
{
    //refs to necess. gameobjects
    public VoxelMap map;

    public GameObject rayStart;

    public GameObject rayDir;
    
    private float3 boundStart;
    
    //options
    public bool integrate = false;
    

    void RayCast()
    {
        boundStart = map.transform.position; //end bound given by mapsize * voxelsize
        
        float3 rayStart = this.rayStart.transform.position;
        float3 rayDir = this.rayDir.transform.position;
        float3 dir = Vector3.Normalize(rayDir - rayStart); 
        
        float3 tDelta = new Vector3(1 / dir.x, 1 / dir.y, 1 / dir.z) * map.voxelSize;
        tDelta = math.abs(tDelta);
        //calc step := -1 || 0 || +1 
        int3 step = new int3((int)math.sign(dir.x), (int)math.sign(dir.y), (int)math.sign(dir.z));
        int3 finVoxel = map.getIdx(rayDir); //voxel of direction - final voxel
        
        int3 currentIndex = map.getIdx(rayStart);
        float3 tMax = math.abs((boundStart + (float3)currentIndex * map.voxelSize - rayStart) / dir); //how far until crossing x y z voxel boundary
        
        //create initial voxel
        Debug.Log("current index: "+currentIndex);
        if(integrate)
            map.ObserveVoxel(currentIndex, Color.white, false);
        else    
            map.AddVoxel(currentIndex);
     
        
        //raycast traversing
        for (int i = 0; i < 1000; i++)
        {
            int3 currentDist = math.abs(currentIndex - finVoxel);
            if (math.all(currentDist <= new int3(1,1,1)))
            {
                Debug.Log("Final voxel reached");
                map.ObserveVoxel(currentIndex, Color.white, true);
                break;
            }
            Debug.Log("current: "+currentIndex+", final: "+finVoxel);
            Debug.Log("Init raycast: \n dir=" + dir + ", tDelta="+tDelta+", step="+step+", currentIndex="+currentIndex+", tMax="+tMax );
            if (tMax.x < tMax.y)
            {
                if (tMax.x < tMax.z)
                {
                    //traverse in x dir
                    if (map.getIdx(currentIndex.x += step.x).x >= 0)    //not -1 := outside the bounds
                    {
                        tMax.x += tDelta.x;
                    }
                    else
                        break;
                }
                else
                {
                    //traverse in Z dir
                    if (map.getIdx(currentIndex.z += step.z).x >= 0)
                    {
                        tMax.z += tDelta.z;
                    }
                    else
                        break;
                }
            }
            else
            {
                if (tMax.y < tMax.z)
                {
                    // traverse in Y dir
                    if (map.getIdx(currentIndex.y += step.y).x >= 0)
                    {
                        tMax.y += tDelta.y;
                    }
                    else
                        break;
                }
                else
                {
                    //traverse in Z dir
                    if (map.getIdx(currentIndex.z += step.z).x >= 0)
                    {
                        tMax.z += tDelta.z;
                    }
                    else
                        break;
                }
            }
            //loop didnt finish due being outside of the grid
            //create voxel
            if(integrate)
                map.ObserveVoxel(currentIndex, Color.white, false);
            else
                map.AddVoxel(currentIndex);
        }
    }
    
    void Awake()
    {
       // RayCast();
        
        rayDirPos = rayDir.transform.position;
        rayStartPos = rayStart.transform.position;
    }

    private Vector3 rayDirPos;
    private Vector3 rayStartPos;
    void Update()
    {
        Debug.DrawLine(rayStart.transform.position, rayDir.transform.position, Color.green);    //visualize ray
        map.DrawBounds();   //visualize map bounds
        
        //realtime ray change
        if (rayDirPos != rayDir.transform.position || rayStartPos != rayStart.transform.position)
        {
            //clear voxels (or not when in integrator mode)
            if(!integrate)
                map.Clear();
            
            RayCast();
            
            map.Render();
            //store last position
            rayDirPos = rayDir.transform.position;
            rayStartPos = rayStart.transform.position;
        }
    }
}

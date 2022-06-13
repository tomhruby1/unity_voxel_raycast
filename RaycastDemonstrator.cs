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

public class RaycastDemonstrator : MonoBehaviour
{
    //refs to necess. gameobjects
    public GameObject voxel;

    public GameObject map;

    public GameObject rayStart;

    public GameObject rayDir;

    //parameters
    private int3 mapSize = new int3(100, 100, 100);
    private float voxelSize = 0.2f;

    private float3 boundStart;


    void RayCast()
    {
        boundStart = map.transform.position; //end bound given by mapsize * voxelsize
        
        float3 rayStart = this.rayStart.transform.position;
        float3 rayDir = this.rayDir.transform.position;
        float3 dir = Vector3.Normalize(rayDir - rayStart); 
        
        float3 tDelta = new Vector3(1 / dir.x, 1 / dir.y, 1 / dir.z) * voxelSize;
        tDelta = math.abs(tDelta);
        //calc step -- -1 || 0 || +1 
        //int3 aux = 
        int3 step = new int3((int)math.sign(dir.x), (int)math.sign(dir.y), (int)math.sign(dir.z));
        int3 finVoxel = getIdx(rayDir); //voxel of direction - final voxel
        
        int3 currentIndex = getIdx(rayStart);
        float3 tMax = math.abs((boundStart + (float3)currentIndex * voxelSize - rayStart) / dir); //how far until crossing x y z voxel boundary
        
        //create initial voxel
        AddVoxel(getWorldPos(currentIndex));
        
        //raycast traversing
        for (int i = 0; i < 1000; i++)
        {
            if (currentIndex.x == finVoxel.x && currentIndex.y == finVoxel.y && currentIndex.z == finVoxel.z)
            {
                Debug.Log("Final voxel reached");
                break;
            }
            Debug.Log("current: "+currentIndex+", final: "+finVoxel);
            Debug.Log("Init raycast: \n dir=" + dir + ", tDelta="+tDelta+", step="+step+", currentIndex="+currentIndex+", tMax="+tMax );
            if (tMax.x < tMax.y)
            {
                if (tMax.x < tMax.z)
                {
                    //traverse in x dir
                    if (getIdx(currentIndex.x += step.x).x >= 0)    //not -1 := outside the bounds
                    {
                        tMax.x += tDelta.x;
                    }
                    else
                        break;
                }
                else
                {
                    //traverse in Z dir
                    if (getIdx(currentIndex.z += step.z).x >= 0)
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
                    if (getIdx(currentIndex.y += step.y).x >= 0)
                    {
                        tMax.y += tDelta.y;
                    }
                    else
                        break;
                }
                else
                {
                    //traverse in Z dir
                    if (getIdx(currentIndex.z += step.z).x >= 0)
                    {
                        tMax.z += tDelta.z;
                    }
                    else
                        break;
                }
            }
            //loop didnt finish due being outside of the grid
            //create voxel
            AddVoxel(getWorldPos(currentIndex));
        }
    }
    int3 getIdx(float3 position)
    {
       
        if (position.x > boundStart.x && position.y > boundStart.y && position.z > boundStart.z) //in bounds
            if (position.x < boundStart.x + voxelSize * mapSize.x && position.y < boundStart.y + voxelSize * mapSize.y
                                                                  && position.z < boundStart.z + voxelSize * mapSize.z)
            {
                int3 index = (int3)math.floor((boundStart + position) / voxelSize);
                return index;
            }
        
        return -1;

    }
    
    //just to check whether in boundaries
    int3 getIdx(int3 position)
    {
        if (position.x > boundStart.x && position.y > boundStart.y && position.z > boundStart.z) //in bounds
            if (position.x < boundStart.x + voxelSize * mapSize.x && position.y < boundStart.y + voxelSize * mapSize.y
                                                                  && position.z < boundStart.z + voxelSize * mapSize.z)
            {
                return position;
            }
        
        return -1;

    }
    Vector3 getWorldPos(int3 index)
    {
        return new Vector3(boundStart.x, boundStart.y, boundStart.z) + new Vector3(index.x, index.y, index.z) * voxelSize; ;
    }
    
    void AddVoxel(Vector3 location)
    {
        GameObject currentVoxel = GameObject.Instantiate(voxel, location, Quaternion.identity, map.transform);
        currentVoxel.transform.localScale *= voxelSize;
    }

    void DrawBounds()
    {
        Debug.DrawLine(boundStart, boundStart+new float3(1, 0,0)*mapSize);
        Debug.DrawLine(boundStart, boundStart+new float3(0, 1,0)*mapSize);
        Debug.DrawLine(boundStart, boundStart+new float3(0, 0,1)*mapSize);
        
        Debug.DrawLine(boundStart+new float3(1,0,1)*mapSize, boundStart+new float3(1, 0,0)*mapSize);
        Debug.DrawLine(boundStart+new float3(1,0,1)*mapSize, boundStart+new float3(0, 0,1)*mapSize);
        Debug.DrawLine(boundStart+new float3(1,0,1)*mapSize, boundStart+new float3(1, 1,1)*mapSize);
        
        Debug.DrawLine(boundStart+new float3(1,1,0)*mapSize, boundStart+new float3(0, 1,0)*mapSize);
        Debug.DrawLine(boundStart+new float3(1,1,0)*mapSize, boundStart+new float3(1, 1,1)*mapSize);
        Debug.DrawLine(boundStart+new float3(1,1,0)*mapSize, boundStart+new float3(1, 0,0)*mapSize);
        
        Debug.DrawLine(boundStart+new float3(0,1,1)*mapSize, boundStart+new float3(0, 0,1)*mapSize);
        Debug.DrawLine(boundStart+new float3(0,1,1)*mapSize, boundStart+new float3(1, 1,1)*mapSize);
        Debug.DrawLine(boundStart+new float3(0,1,1)*mapSize, boundStart+new float3(0, 1,0)*mapSize);
        
    }
    
    void Start()
    {
        RayCast();
        
        rayDirPos = rayDir.transform.position;
        rayStartPos = rayStart.transform.position;
    }

    private Vector3 rayDirPos;
    private Vector3 rayStartPos;
    void Update()
    {
        Debug.DrawLine(rayStart.transform.position, rayDir.transform.position, Color.green);
        
        DrawBounds();
        //realtime

        if (rayDirPos != rayDir.transform.position || rayStartPos != rayStart.transform.position)
        {
            //clear voxels
            for (int i = 0; i < map.transform.childCount; i++)
                Destroy(map.transform.GetChild(i).gameObject);
            RayCast();
            rayDirPos = rayDir.transform.position;
            rayStartPos = rayStart.transform.position;
        }
    }
}

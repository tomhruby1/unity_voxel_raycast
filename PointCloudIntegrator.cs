using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using PointCloud;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class PointCloudIntegrator : MonoBehaviour
{
    //refs to necess. gameobjects
    public VoxelMap map;
    

    //parameters
    /*
    public int3 mapSize = new int3(100, 100, 100);
    public float voxelSize = 0.25f;
    */
    private float3 boundStart;
    
    //options
    public bool integrate = false;
    

    void RayCast(float3 origin, float3 point, Color color)
    {
        boundStart = map.transform.position; //end bound given by mapsize * voxelsize
        
        float3 rayStart = origin;
        float3 rayDir = point;
        float3 dir = Vector3.Normalize(rayDir - rayStart); 
        
        float3 tDelta = new Vector3(1 / dir.x, 1 / dir.y, 1 / dir.z) * map.voxelSize;
        tDelta = math.abs(tDelta);
        //calc step -- -1 || 0 || +1 
        //int3 aux = 
        int3 step = new int3((int)math.sign(dir.x), (int)math.sign(dir.y), (int)math.sign(dir.z));
        int3 finVoxel = map.getIdx(rayDir); //voxel of direction - final voxel
        
        int3 currentIndex = map.getIdx(rayStart);
        float3 tMax = math.abs((boundStart + (float3)currentIndex * map.voxelSize - rayStart) / dir); //how far until crossing x y z voxel boundary
        
        //raycast traversing
        for (int i = 0; i < 1000; i++)
        {
            int3 currentDist = math.abs(currentIndex - finVoxel);
            if (math.all(currentDist <= new int3(1,1,1)))
            {
                map.ObserveVoxel(currentIndex, color, true);    //observed surface voxel
                break;
            }
            //Debug.Log("current: "+currentIndex+", final: "+finVoxel);
            //Debug.Log("Init raycast: \n dir=" + dir + ", tDelta="+tDelta+", step="+step+", currentIndex="+currentIndex+", tMax="+tMax );
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
            //create voxel
            if(integrate)
                map.ObserveVoxel(currentIndex, color, false);   //observe "air" voxel
            else
                map.AddVoxel(currentIndex);
        }
    }
    
    (Vector3, Color) PCDpoint2Vector3(PointXYZRGB point)
    {
        Vector3 position = new Vector3(point.X, point.Y, point.Z);
        Color color = new Color(point.R/255f, point.G/255f, point.B/255f, 1f);
      
        return (position, color);
    }
    
    private Vector3 rayDirPos;
    private Vector3 rayStartPos;
    public void Integrate(PointCloud<PointXYZRGB> pcd, Vector3 sensorPosition)
    {
        Debug.Log("integrating pcd");
        // map.DrawBounds();   //visualize map bounds
        for (int i = 0; i < pcd.Size; i++)
        {
            float3 vertex;
            Color color;
            (vertex, color) = PCDpoint2Vector3(pcd.At(i));
            RayCast(sensorPosition, vertex, color);
        }
        map.Render();
    }
}

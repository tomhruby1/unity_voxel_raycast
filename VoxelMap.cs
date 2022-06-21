using UnityEngine;
using Unity.Mathematics;
using UnityEngine;


    public abstract class VoxelMap : MonoBehaviour
    {
        public int3 mapSize = new int3(100, 100, 100);
        public float voxelSize = 1f;
     
        private float3 boundStart;


        public abstract void AddVoxel(Vector3 location);
        
        public void Start()
        {
            boundStart = transform.position;
        }

        public void DrawBounds()
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
    }

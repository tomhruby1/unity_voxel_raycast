using UnityEngine;
using Unity.Mathematics;
using UnityEngine;


    public abstract class VoxelMap : MonoBehaviour
    {
        public int3 mapSize = new int3(100, 100, 100);
        public float voxelSize = 1f;
     
        protected float3 boundStart;

        //just hard-add voxel
        public abstract void AddVoxel(int3 location);
        //add observe raycasted point and create voxel
        public abstract void ObserveVoxel(int3 location, Color color, bool surface);
        public abstract Vector3 getWorldPos(int3 index);
        public abstract void Clear();
        public abstract void Render();
        
        public void Awake()
        {
            boundStart = transform.position;
            Debug.Log(boundStart);
        }
        
        public void DrawBounds()
        {
            Debug.DrawLine(boundStart, boundStart+new float3(1, 0,0)*mapSize*voxelSize);
            Debug.DrawLine(boundStart, boundStart+new float3(0, 1,0)*mapSize*voxelSize);
            Debug.DrawLine(boundStart, boundStart+new float3(0, 0,1)*mapSize*voxelSize);
        
            Debug.DrawLine(boundStart+new float3(1,0,1)*mapSize, boundStart+new float3(1, 0,0)*mapSize*voxelSize);
            Debug.DrawLine(boundStart+new float3(1,0,1)*mapSize, boundStart+new float3(0, 0,1)*mapSize*voxelSize);
            Debug.DrawLine(boundStart+new float3(1,0,1)*mapSize, boundStart+new float3(1, 1,1)*mapSize*voxelSize);
        
            Debug.DrawLine(boundStart+new float3(1,1,0)*mapSize, boundStart+new float3(0, 1,0)*mapSize*voxelSize);
            Debug.DrawLine(boundStart+new float3(1,1,0)*mapSize, boundStart+new float3(1, 1,1)*mapSize*voxelSize);
            Debug.DrawLine(boundStart+new float3(1,1,0)*mapSize, boundStart+new float3(1, 0,0)*mapSize*voxelSize);
        
            Debug.DrawLine(boundStart+new float3(0,1,1)*mapSize, boundStart+new float3(0, 0,1)*mapSize*voxelSize);
            Debug.DrawLine(boundStart+new float3(0,1,1)*mapSize, boundStart+new float3(1, 1,1)*mapSize*voxelSize);
            Debug.DrawLine(boundStart+new float3(0,1,1)*mapSize, boundStart+new float3(0, 1,0)*mapSize*voxelSize);
        
        }
        
        public int3 getIdx(float3 position)
        {
       
            if (position.x > boundStart.x && position.y > boundStart.y && position.z > boundStart.z) //in bounds
                if (position.x < boundStart.x + voxelSize * mapSize.x && position.y < boundStart.y + voxelSize * mapSize.y
                                                                              && position.z < boundStart.z + voxelSize * mapSize.z)
                {
                    int3 index = (int3)math.floor((-boundStart + position) / voxelSize);
                    return index;
                }
        
            return -1;
        }
    
        //just to check whether in boundaries
        public int3 getIdx(int3 index)
        {
            if (math.all(index > boundStart))
                if (math.all(index < (float3)boundStart + (float3)mapSize))
                    return index;

            return -1;
        }
    }

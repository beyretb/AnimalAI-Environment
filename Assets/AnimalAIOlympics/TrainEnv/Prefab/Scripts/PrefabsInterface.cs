using UnityEngine;

namespace PrefabInterface
{
    
    public interface IPrefab
    {
        void SetSize(Vector3 size);
        void SetColor(Vector3 color);
        Vector3 GetRotation(float rotationY);
        Vector3 GetPosition(Vector3 position, Vector3 boundingBox, float rangeX, float rangeZ);
    }

}
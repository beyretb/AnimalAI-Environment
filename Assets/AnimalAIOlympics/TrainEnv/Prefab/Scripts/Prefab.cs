using UnityEngine;
using System;
using Random = UnityEngine.Random;
using PrefabInterface;


/// <summary>
/// A Prefab represents a GameObject that can be spawned in an arena, it also contains the range of
/// values that the user can pass as parameters
/// </summary>
public class Prefab : MonoBehaviour, IPrefab
{

    public Vector2 rotationRange;
    public Vector3 sizeMin;
    public Vector3 sizeMax;
    public bool canRandomizeColor = true;
    public Vector3 ratioSize;
    public float sizeAdjustement = 0.999f;

    protected float _height;

    public virtual void SetColor(Vector3 color)
    {
        if (canRandomizeColor)
        {
            Color newColor = new Color();
            newColor.a = 1f;
            newColor.r = color.x >=0 ? color.x/255f : Random.Range(0f,1f);
            newColor.g = color.y >=0 ? color.y/255f : Random.Range(0f,1f);
            newColor.b = color.z >=0 ? color.z/255f : Random.Range(0f,1f);

            if (GetComponent<Renderer>() != null)
            {
                GetComponent<Renderer>().material.color = newColor;
            }
            foreach (Renderer r in GetComponentsInChildren<Renderer>())
            {
                r.material.color = newColor;
            }
        }
    }

    public virtual void SetSize(Vector3 size)
    {
        Vector3 clippedSize = Vector3.Max(sizeMin, Vector3.Min(sizeMax, size)) * sizeAdjustement;
        float sizeX = size.x < 0 ? Random.Range(sizeMin[0], sizeMax[0]) : clippedSize.x;
        float sizeY = size.y < 0 ? Random.Range(sizeMin[1], sizeMax[1]) : clippedSize.y;
        float sizeZ = size.z < 0 ? Random.Range(sizeMin[2], sizeMax[2]) : clippedSize.z;

        _height = sizeY;
        transform.localScale = new Vector3(sizeX * ratioSize.x,
                                            sizeY * ratioSize.y,
                                            sizeZ * ratioSize.z);
    }

    public virtual Vector3 GetRotation(float rotationY)
    {
        return new Vector3(0,
                        rotationY < 0 ? Random.Range(rotationRange.x, rotationRange.y) : rotationY,
                        0);
    }

    public virtual Vector3 GetPosition(Vector3 position,
                                        Vector3 boundingBox,
                                        float rangeX,
                                        float rangeZ)
    {
        float xBound = boundingBox.x;
        float zBound = boundingBox.z;
        float xOut = position.x < 0 ? Random.Range(xBound, rangeX - xBound) 
                                    : Math.Max(0,Math.Min(position.x, rangeX));
        float yOut = Math.Max(position.y,0);
        float zOut = position.z < 0 ? Random.Range(zBound, rangeZ - zBound) 
                                    : Math.Max(0,Math.Min(position.z, rangeZ));

        return new Vector3(xOut, AdjustY(yOut), zOut);
    }

    protected virtual float AdjustY(float yIn)
    {
        return yIn + _height / 2 + 0.01f;
    }

}
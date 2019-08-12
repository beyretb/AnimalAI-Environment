using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class BallGoal : Goal
{
    public override void SetSize(Vector3 size)
    {
        Vector3 clippedSize = Vector3.Max(sizeMin, Vector3.Min(sizeMax, size)) * sizeAdjustement;
        if (size.x < 0 || size.y < 0 || size.z < 0)
        {
            float sizeAllAxes = Random.Range(sizeMin[0], sizeMax[0]);
            clippedSize = sizeAllAxes * Vector3.one;
        }
        _height = clippedSize.x;
        transform.localScale = new Vector3(clippedSize.x * ratioSize.x,
                                            clippedSize.x * ratioSize.x,
                                            clippedSize.x * ratioSize.x);
        reward = Math.Sign(reward) * clippedSize.x;
    }
}

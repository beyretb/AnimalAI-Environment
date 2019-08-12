using UnityEngine;

public class Grounded : Prefab
{

    public float heightSpawn;

    protected override float AdjustY(float yIn)
    {
        return 0;
    }
}

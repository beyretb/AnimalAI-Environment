using System.Collections.Generic;
using System;
using UnityEngine;
using MLAgents.CommunicatorObjects;
using Lights;


namespace ArenasParameters
{
    /// <summary>
    /// The list of prefabs that can be passed as items to spawn in the various arenas instantiated 
    /// </summary>
    [System.Serializable]
    public class ListOfPrefabs
    {
        public List<GameObject> allPrefabs;
        public List<GameObject> GetList()
        {
            return allPrefabs;
        }
    }

    /// <summary>
    /// We define a Spawnable item as a GameObject and a list of parameters to spawn it. These 
    /// include whether or not colors and sizes should be rancomized, as well as lists of positions
    /// rotations and sizes the user can provide. Any of these parameters left empty by the user
    /// will be randomized at the time we spawn the associated GameObject
    /// </summary>
    public class Spawnable
    {
        public String name = "";
        public GameObject gameObject = null;
        public List<Vector3> positions = null;
        public List<float> rotations = null;
        public List<Vector3> sizes = null;
        public List<Vector3> colors = null;

        public Spawnable(GameObject obj)
        {
            name = obj.name;
            gameObject = obj;
            positions = new List<Vector3>();
            rotations = new List<float>();
            sizes = new List<Vector3>();
            colors = new List<Vector3>();
        }

        public Spawnable(ArenaParametersProto.Types.ItemsToSpawn proto)
        {
            name = proto.Name;
            positions = new List<Vector3>();
            foreach (ArenaParametersProto.Types.ItemsToSpawn.Types.Vector3 v in proto.Positions)
            {
                positions.Add(new Vector3(v.X, v.Y, v.Z));
            }
            rotations = new List<float>(proto.Rotations);
            sizes = new List<Vector3>();
            foreach (ArenaParametersProto.Types.ItemsToSpawn.Types.Vector3 v in proto.Sizes)
            {
                sizes.Add(new Vector3(v.X, v.Y, v.Z));
            }
            colors = new List<Vector3>();
            foreach (ArenaParametersProto.Types.ItemsToSpawn.Types.Vector3 v in proto.Colors)
            {
                colors.Add(new Vector3(v.X, v.Y, v.Z));
            }
        }

    }

    /// <summary>
    /// An ArenaConfiguration contains the list of items that can be spawned in the arena, the 
    /// maximum number of steps which can vary from one episode to the next (T) and whether all
    /// sizes and colors can be randomized
    /// </summary>
    public class ArenaConfiguration
    {
        public int T = 1000;
        public List<Spawnable> spawnables = new List<Spawnable>();
        public LightsSwitch lightsSwitch = new LightsSwitch();
        public bool toUpdate = false;
        public string protoString = "";

        public ArenaConfiguration()
        {
        }

        public ArenaConfiguration(ListOfPrefabs listPrefabs)
        {
            foreach (GameObject prefab in listPrefabs.allPrefabs)
            {
                spawnables.Add(new Spawnable(prefab));
            }
            T = 0;
            toUpdate = true;
        }

        public ArenaConfiguration(ArenaParametersProto proto)
        {
            T = proto.T;
            spawnables = new List<Spawnable>();
            foreach (ArenaParametersProto.Types.ItemsToSpawn item in proto.Items)
            {
                spawnables.Add(new Spawnable(item));
            }
            List<int> blackouts = new List<int>();
            foreach (int blackout in proto.Blackouts)
            {
                blackouts.Add(blackout);
            }
            lightsSwitch = new LightsSwitch(T, blackouts);
            toUpdate = true;
            protoString = proto.ToString();
        }

        public void SetGameObject(List<GameObject> listObj)
        {
            foreach (Spawnable spawn in spawnables)
            {
                spawn.gameObject = listObj.Find(x => x.name == spawn.name);
            }
        }
    }

    /// <summary>
    /// ArenaConfigurations is a dictionary of configurations for each arena
    /// </summary>
    public class ArenasConfigurations
    {
        public Dictionary<int, ArenaConfiguration> configurations;
        public int numberOfArenas = 1;

        public ArenasConfigurations()
        {
            configurations = new Dictionary<int, ArenaConfiguration>();
        }

        public void Add(int k, ArenaParametersProto arena)
        {
            if (k<numberOfArenas)
            {
                if (!configurations.ContainsKey(k))
                {
                    configurations.Add(k, new ArenaConfiguration(arena));
                }
                else
                {
                    if (arena.ToString() != configurations[k].protoString)
                    {
                        configurations[k] = new ArenaConfiguration(arena);
                    }
                }
            }
        }

        public void Clear()
        {
            configurations.Clear();
        }
    }


}
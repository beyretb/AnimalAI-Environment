using System.Collections.Generic;
using UnityEngine;
using MLAgents;
using ArenaBuilders;
using UnityEngineExtensions;
using ArenasParameters;

public class TrainingArea : Area
{

    public ListOfPrefabs prefabs = new ListOfPrefabs();
    public GameObject spawnedObjectsHolder;
    public int maxSpawnAttemptsForAgent = 100;
    public int maxSpawnAttemptsForPrefabs = 20;
    [HideInInspector]
    public int arenaID = -1;
    [HideInInspector]
    public Agent agent;

    private ArenaBuilder _builder;
    private ArenaConfiguration _arenaConfiguration = new ArenaConfiguration();
    private ArenasConfigurations _arenasConfigurations;
    private int _agentDecisionInterval;

    public void Start()
    {
        _builder = new ArenaBuilder(gameObject,
                                    spawnedObjectsHolder,
                                    maxSpawnAttemptsForPrefabs,
                                    maxSpawnAttemptsForAgent);
        _arenasConfigurations = GameObject.FindObjectOfType<Academy>().arenasConfigurations;
        if (!_arenasConfigurations.configurations.TryGetValue(arenaID, out _arenaConfiguration))
        {
            _arenaConfiguration = new ArenaConfiguration(prefabs);
            _arenasConfigurations.configurations.Add(arenaID, _arenaConfiguration);
        }
        agent = transform.FindChildWithTag("agent").GetComponent<Agent>();
        _agentDecisionInterval = agent.agentParameters.numberOfActionsBetweenDecisions;
    }


    public override void ResetArea()
    {
        DestroyImmediate(transform.FindChildWithTag("spawnedObjects"));

        ArenaConfiguration newConfiguration;
        if (_arenasConfigurations.configurations.TryGetValue(arenaID, out newConfiguration))
        {
            _arenaConfiguration = newConfiguration;
            if (_arenaConfiguration.toUpdate)
            {
                _arenaConfiguration.SetGameObject(prefabs.GetList());
                _builder.Spawnables = _arenaConfiguration.spawnables;
                _arenaConfiguration.toUpdate = false;
                agent.agentParameters.maxStep = _arenaConfiguration.T * _agentDecisionInterval;
            }
        }
        _builder.Build();
        _arenaConfiguration.lightsSwitch.Reset();
    }

    public bool UpdateLigthStatus(int stepCount)
    {
        return _arenaConfiguration.lightsSwitch.LightStatus(stepCount, _agentDecisionInterval);
    }

}

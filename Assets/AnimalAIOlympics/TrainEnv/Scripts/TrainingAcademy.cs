using System;
using System.Linq;
using UnityEngine;
using MLAgents;
using UnityEngineExtensions;

public class TrainingAcademy : Academy
{

    public GameObject arena;
    public Brain playerBrain;
    public int maximumResolution = 512;
    public int minimumResolution = 4;

    private TrainingArea[] _arenas;
    private Agent _agent;

    public override void InitializeAcademy()
    {
        bool receiveConfiguration = false;
        int numberOfArenas = -1;
        int resolution = 84;
        ParseArguments(ref receiveConfiguration, ref numberOfArenas, ref resolution);

        if (numberOfArenas == -1 || externalInferenceMode)
        {
            playerMode = true;
            numberOfArenas = 1;
        }

        ChangeResolution(resolution);

        _arenas = new TrainingArea[numberOfArenas];
        arenasConfigurations.numberOfArenas = numberOfArenas;
        InstantiateArenas(numberOfArenas);
        ConfigureIfPlayer(receiveConfiguration);
    }

    private void ParseArguments(ref bool receiveConfiguration, 
                                ref int numberOfArenas,
                                ref int resolution)
    {
        string[] args = System.Environment.GetCommandLineArgs();

        for (int i = 0; i < args.Length; i++)
        {
            switch (args[i])
            {
                case "--nArenas":
                    numberOfArenas = int.Parse(args[i + 1]);
                    break;
                case "--port":
                    receiveConfiguration = true;
                    break;
                case "--inference":
                    externalInferenceMode = true;
                    break;
                case "--resolution":
                    if (i < args.Length - 1)
                    {
                        var resolutionInput = args[i + 1];
                        try
                        {
                            resolution = Math.Max(minimumResolution, Math.Min(maximumResolution,
                                                                Int32.Parse(resolutionInput)));
                        }
                        catch (FormatException)
                        {
                        }
                    }
                    break;
            }
        }
    }

    private void ChangeResolution(int resolution)
    {
        var controlledBrains = broadcastHub.broadcastingBrains.Where(
                x => x != null && x is LearningBrain && broadcastHub.IsControlled(x));
        foreach (LearningBrain brain in controlledBrains)
        {
            if (brain.brainParameters.cameraResolutions.Length>0)
            {
                brain.brainParameters.cameraResolutions[0].height = resolution;
                brain.brainParameters.cameraResolutions[0].width = resolution;
            }
        }
    }

    private void ConfigureIfPlayer(bool receiveConfiguration)
    {
        if (playerMode)
        {
            if (!receiveConfiguration) // && !Application.isEditor) //uncomment for use in Editor
            {
                this.broadcastHub.Clear();
            }
            if (!externalInferenceMode)
            {
                _arenas[0].gameObject.GetComponentInChildren<Agent>().brain = playerBrain;
            }
            GameObject.FindObjectOfType<PlayerControls>().activate = true;
            _agent = GameObject.FindObjectOfType<Agent>();
        }
        else
        {
            GameObject.FindGameObjectWithTag("agent")
                        .transform.Find("AgentCamMid")
                        .GetComponent<Camera>()
                        .enabled = false;
            GameObject.FindGameObjectWithTag("score").SetActive(false);
        }
    }

    private void InstantiateArenas(int numberOfArenas)
    {
        // We organize the arenas in a grid and position the main camera at the center, high enough
        // to see all arenas at once

        Vector3 boundingBox = arena.GetBoundsWithChildren().extents;
        float width = 2 * boundingBox.x + 5f;
        float height = 2 * boundingBox.z + 5f;
        int n = (int)Math.Round(Math.Sqrt(numberOfArenas));

        for (int i = 0; i < numberOfArenas; i++)
        {
            float x = (i % n) * width;
            float y = (i / n) * height;
            GameObject arenaInst = Instantiate(arena, new Vector3(x, 0f, y), Quaternion.identity);
            _arenas[i] = arenaInst.GetComponent<TrainingArea>();
            _arenas[i].arenaID = i;
        }

        GameObject.FindGameObjectWithTag("MainCamera").transform.localPosition =
            new Vector3(n * width / 2, 50 * (float)n, (float)n * height / 2);
    }

    public override void AcademyReset()
    {
    }

    public override void AcademyStep()
    {
    }
}

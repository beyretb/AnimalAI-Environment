using System.Linq;
using System;
using UnityEngine;
using Random = UnityEngine.Random;
using MLAgents;
using PrefabInterface;

public class TrainingAgent : Agent, IPrefab
{
    public void RandomSize() { }
    public void SetColor(Vector3 color) { }
    public void SetSize(Vector3 scale) { }

    public virtual Vector3 GetPosition(Vector3 position,
                                        Vector3 boundingBox,
                                        float rangeX,
                                        float rangeZ)
    {
        float xBound = boundingBox.x;
        float zBound = boundingBox.z;
        float xOut = position.x < 0 ? Random.Range(xBound, rangeX - xBound)
                                    : Math.Max(0, Math.Min(position.x, rangeX));
        float yOut = Math.Max(position.y, 0) + transform.localScale.y / 2 + 0.01f;
        float zOut = position.z < 0 ? Random.Range(zBound, rangeZ - zBound)
                                    : Math.Max(0, Math.Min(position.z, rangeZ));

        return new Vector3(xOut, yOut, zOut);
    }

    public virtual Vector3 GetRotation(float rotationY)
    {
        return new Vector3(0,
                        rotationY < 0 ? Random.Range(0f, 360f) : rotationY,
                        0);
    }

    public float speed = 30f;
    public float rotationSpeed = 100f;
    public float rotationAngle = 0.25f;
    [HideInInspector]
    public int numberOfGoalsCollected = 0;

    private Rigidbody _rigidBody;
    private bool _isGrounded;
    private ContactPoint _lastContactPoint;
    private TrainingArea _area;
    private float _rewardPerStep;
    private Color[] _allBlackImage;
    private PlayerControls _playerScript;

    public override void InitializeAgent()
    {
        _area = GetComponentInParent<TrainingArea>();
        _rigidBody = GetComponent<Rigidbody>();
        _rewardPerStep = agentParameters.maxStep > 0 ? -1f / agentParameters.maxStep : 0;
        _playerScript = GameObject.FindObjectOfType<PlayerControls>();
    }

    public override void CollectObservations()
    {
        Vector3 localVel = transform.InverseTransformDirection(_rigidBody.velocity);
        AddVectorObs(localVel);
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        int actionForward = Mathf.FloorToInt(vectorAction[0]);
        int actionRotate = Mathf.FloorToInt(vectorAction[1]);

        moveAgent(actionForward, actionRotate);

        AddReward(_rewardPerStep);
    }

    private void moveAgent(int actionForward, int actionRotate)
    {
        Vector3 directionToGo = Vector3.zero;
        Vector3 rotateDirection = Vector3.zero;

        if (_isGrounded)
        {
            switch (actionForward)
            {
                case 1:
                    directionToGo = transform.forward * 1f;
                    break;
                case 2:
                    directionToGo = transform.forward * -1f;
                    break;
            }
        }
        switch (actionRotate)
        {
            case 1:
                rotateDirection = transform.up * 1f;
                break;
            case 2:
                rotateDirection = transform.up * -1f;
                break;
        }

        transform.Rotate(rotateDirection, Time.fixedDeltaTime * rotationSpeed);
        _rigidBody.AddForce(directionToGo * speed * Time.fixedDeltaTime, ForceMode.VelocityChange);
    }

    public override void AgentReset()
    {
        _playerScript.prevScore = GetCumulativeReward();
        numberOfGoalsCollected = 0;
        _area.ResetArea();
        _rewardPerStep = agentParameters.maxStep > 0 ? -1f / agentParameters.maxStep : 0;
        _isGrounded = false;
    }


    void OnCollisionEnter(Collision collision)
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            if (contact.normal.y > 0)
            {
                _isGrounded = true;
            }
        }
        _lastContactPoint = collision.contacts.Last();
    }

    void OnCollisionStay(Collision collision)
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            if (contact.normal.y > 0)
            {
                _isGrounded = true;
            }
        }
        _lastContactPoint = collision.contacts.Last();
    }

    void OnCollisionExit(Collision collision)
    {
        if (_lastContactPoint.normal.y > 0)
        {
            _isGrounded = false;
        }
    }

    public void AgentDeath(float reward)
    {
        AddReward(reward);
        Done();
    }

    public void AddExtraReward(float rewardFactor)
    {
        AddReward(Math.Min(rewardFactor * _rewardPerStep,-0.00001f));
    }

    public override bool LightStatus()
    {
        return _area.UpdateLigthStatus(GetStepCount());
    }
}

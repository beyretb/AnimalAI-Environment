using UnityEngine;

public class BallGoalMove : BallGoal
{

    public float maximumVelocity = 20;
    public float forceToApply = 5;

    private Rigidbody rBody;

    void Start()
    {
        rBody = GetComponent<Rigidbody>();

        rBody.AddForce(forceToApply * transform.forward * Time.fixedDeltaTime,
                            ForceMode.VelocityChange);
    }
}

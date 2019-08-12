using UnityEngine;


public class Goal : Prefab
{

    public int numberOfGoals = 1;
    public float reward = 1;
    public bool isMulti = false;

    void Awake()
    {
        canRandomizeColor = false;
    }

    public virtual void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("agent"))
        {
            collision.GetComponent<TrainingAgent>().AgentDeath(reward);
        }
    }

    public virtual void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("agent"))
        {
            if (!isMulti)
            {
                collision.gameObject.GetComponent<TrainingAgent>().AgentDeath(reward);
            }
            else
            {
                TrainingAgent agentScript = collision.gameObject.GetComponent<TrainingAgent>();
            agentScript.numberOfGoalsCollected++;
            if (agentScript.numberOfGoalsCollected == numberOfGoals)
            {
                agentScript.AgentDeath(reward);
            }
            else
            {
                agentScript.AddReward(reward);
            }
            Object.Destroy(this.gameObject);
            }
        }
    }

}
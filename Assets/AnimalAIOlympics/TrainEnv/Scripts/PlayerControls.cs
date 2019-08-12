using MLAgents;
using UnityEngine;
using UnityEngine.UI;

public class PlayerControls : MonoBehaviour
{

    private Camera _cameraAbove;
    private Camera _cameraAgent;
    private Camera _cameraBlack;
    private Agent _agent;
    private Text _score;
    public bool activate = false;
    public float prevScore = 0;

    void Start()
    {
        _agent = GameObject.FindGameObjectWithTag("agent").GetComponent<Agent>();
        _cameraAbove = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        _cameraAgent = _agent.transform.Find("AgentCamMid")
                                                                        .GetComponent<Camera>();
        _cameraBlack = GameObject.FindGameObjectWithTag("camBlack").GetComponent<Camera>();
        _score = GameObject.FindObjectOfType<Text>();
        
        _cameraAbove.enabled = true;
        _cameraAgent.enabled = false;
        _cameraBlack.enabled = false;
    }

    void FixedUpdate()
    {
        if (activate)
        {
            bool cDown = Input.GetKeyDown(KeyCode.C);
            if (cDown)
            {
                _cameraAbove.enabled = !_cameraAbove.enabled;
                _cameraAgent.enabled = !_cameraAgent.enabled;
            }
            else if (!_cameraAbove.enabled)
            {
                if (!_agent.LightStatus() && _cameraAgent.enabled)
                {
                    _cameraAgent.enabled = false;
                    _cameraBlack.enabled = true;
                }
                if (_agent.LightStatus() && !_cameraAgent.enabled)
                {
                    _cameraAgent.enabled = true;
                    _cameraBlack.enabled = false;
                }
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                GameObject.FindObjectOfType<Agent>().Done();
            }
            _score.text = "Prev reward: "+ prevScore.ToString("0.000")+ "\n"
                            + "Reward: "+ GameObject.FindObjectOfType<Agent>()
                                                .GetCumulativeReward().ToString("0.000");
        }
    }
}

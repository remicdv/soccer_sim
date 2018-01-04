using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WorldController : MonoBehaviour {

    GameObject canvas;
    Transform child;
    Transform time;
    Transform endC;
    Text endCText;
    GameObject kickOff;
    Text kickOffText;
    Text timeText;
    Text scoreText;
    bool start;
    bool end;
    float timer;
    GameObject ball;
    float delta;
    int seconds;
    public float whokickOff;
    int minuts;
    GameObject menu_btn;

    [FMODUnity.EventRef]
    public string crowdEvent;
    FMOD.Studio.EventInstance crowd;

    [FMODUnity.EventRef]
    public string gameStartingSpeechEventRef;
    FMOD.Studio.EventInstance gameStartingSpeechEvent;

    [FMODUnity.EventRef]
    public string reactionToGoalSpeechEventRef;
    FMOD.Studio.EventInstance reactionToGoalSpeechEvent;

    [FMODUnity.EventRef]
    public string crowdReactionToGoalEventRef;
    FMOD.Studio.EventInstance crowdReactionToGoalEvent;

    [FMODUnity.EventRef]
    public string ambianceReactionSpeechEventRef;
    FMOD.Studio.EventInstance ambianceReactionSpeechEvent;

    public float conflictDribble;
    GameObject p1;
    GameObject p2;

    GameObject[] player;
    GameObject[] player1;

    public float crowdExcitationRatio = 0.2f;

    private float intervalBeforeExcitationDecrease = 25f;
    private float excitationTimer = 0f;

    private float intervalBeforeAmbianceComment = 25f;
    private float ambianceCommentTimer = 0f;

    // Use this for initialization
    void Start () {

        // Play speech at the beginning of the game
        gameStartingSpeechEvent = FMODUnity.RuntimeManager
                                           .CreateInstance(gameStartingSpeechEventRef);
        gameStartingSpeechEvent.start();
        gameStartingSpeechEvent.release();

        // Create reactionToGoalSpeechEvent
        reactionToGoalSpeechEvent = FMODUnity.RuntimeManager
                                           .CreateInstance(reactionToGoalSpeechEventRef);

        // Create crowdReactionToGoalEvent
        crowdReactionToGoalEvent = FMODUnity.RuntimeManager
                                           .CreateInstance(crowdReactionToGoalEventRef);

        // Create ambianceReactionSpeechEvent
        ambianceReactionSpeechEvent = FMODUnity.RuntimeManager.CreateInstance(ambianceReactionSpeechEventRef);

        crowd = FMODUnity.RuntimeManager.CreateInstance(crowdEvent);
        crowd.start();

        menu_btn = GameObject.Find("Menu");
        menu_btn.SetActive(false);

        player = GameObject.FindGameObjectsWithTag("RedTeam");
        player1 = GameObject.FindGameObjectsWithTag("BlueTeam");
        canvas = GameObject.Find("Canvas");
        child = canvas.transform.Find("ScoreLabel");
        time = canvas.transform.Find("Time");
        endC = canvas.transform.Find("end");
        endC.gameObject.SetActive(false);
        endCText = endC.GetComponent<Text>();
        kickOff = GameObject.Find("KickOff");
        kickOff.SetActive(false);
        kickOffText = kickOff.GetComponent<Text>();
        timeText = time.GetComponent<Text>();
        minuts = 0;
        timer = 0;
        scoreText = child.GetComponent<Text>();
        start = false;
        end = false;
        ball = GameObject.Find("Ball");
        whokickOff = Random.Range(0f, 1f);
        if (whokickOff > 0.5f)
        {
            //Debug.Log(whokickOff + " bleu commence");
            kickOffText.text += " Blues start";
            GameObject.Find("Player").GetComponent<AI_Player>().homeRegion += 1;
        }
        else
        {
            //Debug.Log(whokickOff + " rouge commence");
            kickOffText.text += " Reds start";
            GameObject.Find("Player (10)").GetComponent<AI_Player>().homeRegion -= 1;
        }

    }

    void GoalScored(string team)
    {

        // Crowd and speaker's reaction to goal are played
        crowdReactionToGoalEvent.start();
        //crowdReactionToGoalEvent.release();

        reactionToGoalSpeechEvent.start();
        //reactionToGoalSpeechEvent.release();

        if (team == "Red")
        {
            ++GameConstants.RedScore;
            scoreText.text = "Score : " + GameConstants.RedScore + " - " + GameConstants.BlueScore;
            GameObject.Find("Player").GetComponent<AI_Player>().homeRegion += 1;

            if (crowdExcitationRatio <= 0.85f)
                crowdExcitationRatio += 0.15f;
        }
        else if (team == "Blue")
        {
            ++GameConstants.BlueScore;
            scoreText.text = "Score : " + GameConstants.RedScore + " - " + GameConstants.BlueScore;
            GameObject.Find("Player (10)").GetComponent<AI_Player>().homeRegion -= 1;

            if (crowdExcitationRatio <= 0.85f)
                crowdExcitationRatio += 0.15f;

        }

        foreach (GameObject p in player)
        {
            AI_Player a = p.GetComponent("AI_Player") as AI_Player;
            if (a != null)
            {
                p.transform.position = GameConstants.centers[a.homeRegion];
                a.stateMachine.ChangeState(FirstState.Instance);
            }
            p.GetComponent<Rigidbody>().velocity *= 0;

        }

        foreach (GameObject p in player1)
        {
            AI_Player a = p.GetComponent("AI_Player") as AI_Player;
            if (a != null)
            {
                p.transform.position = GameConstants.centers[a.homeRegion];
                a.stateMachine.ChangeState(FirstState.Instance);
            }
            p.GetComponent<Rigidbody>().velocity *= 0;

        }
    }

    private void FixedUpdate()
    {
        excitationTimer += Time.deltaTime;
        ambianceCommentTimer += Time.deltaTime;

        if(excitationTimer > intervalBeforeExcitationDecrease && crowdExcitationRatio >= 0.05f)
        {
            Debug.Log("Le public s'emmerde !");
            crowdExcitationRatio -= 0.05f;
            excitationTimer = 0.0f;
        }

        float parameterValue = 0f;
        crowd.getParameterValue("CrowdExcitation", out parameterValue, out parameterValue);
        //Debug.Log(parameterValue);
        if (parameterValue != crowdExcitationRatio)
        {
            Debug.Log("changing crowd excitation to " + crowdExcitationRatio);
            crowd.setParameterValue("CrowdExcitation", crowdExcitationRatio);

        }

        if (ambianceCommentTimer > intervalBeforeAmbianceComment && crowdExcitationRatio >= 0.5f && Random.Range(0,100) < 50)
        {
            Debug.Log("L'ambiance !");
            ambianceReactionSpeechEvent.start();
            //ambianceReactionSpeechEvent.release();
            ambianceCommentTimer = 0f;
        }
        //seconds++;
        if (start)
        {
            timer += Time.deltaTime;
            if(!end)
            {
                seconds = (int)timer % 60;
                minuts = (int)System.Math.Ceiling(timer / 60 - 1);
            }
            if (minuts == 5)
            {
                end = true;
            }

            timeText.text = "Time : " + minuts.ToString("00") + " : " + seconds.ToString("00");
        }

        if(doubleDribbling())
        {
            p1 = getPlayerDribbling(player);
            p2 = getPlayerDribbling(player1);
            conflictDribble = Random.Range(0f, 1f);
            if(conflictDribble < 0.5f)
            {
                //Debug.Log("le 1 " + p1);
                AI_Player a = p1.GetComponent("AI_Player") as AI_Player;
                if (a != null)
                {
                    a.transform.position = new Vector3(a.transform.position.x - 3, a.transform.position.y, a.transform.position.z -3);
                    a.stateMachine.ChangeState(FirstState.Instance);
                }
            }
            else
            {
                //Debug.Log("le 2 " + p2);
                AI_Player a = p2.GetComponent("AI_Player") as AI_Player;
                if (a != null)
                {
                    a.transform.position = new Vector3(a.transform.position.x + 3, a.transform.position.y, a.transform.position.z +3 );
                    a.stateMachine.ChangeState(FirstState.Instance);
                }
            }
        }

        if (end)
        {
            if (GameConstants.RedScore > GameConstants.BlueScore)
                endCText.text = "Victoire des rouges !";
            else if(GameConstants.RedScore < GameConstants.BlueScore)
                endCText.text = "Victoire des bleues !";
            else
                endCText.text = "Match nul";

            crowd.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            ball.GetComponent<Rigidbody>().velocity *= 0;
            ball.transform.position = new Vector3(0f, 1.5f, 144f);
            endC.gameObject.SetActive(true);
            menu_btn.SetActive(true);
            foreach (GameObject p in player)
            {
                AI_Player a = p.GetComponent("AI_Player") as AI_Player;
                if (a != null)
                {
                    a.finish = true;
                    a.stateMachine.ChangeState(BeginState.Instance);
                }

            }

            foreach (GameObject p in player1)
            {
                AI_Player a = p.GetComponent("AI_Player") as AI_Player;
                if (a != null)
                {
                    a.finish = true;
                    a.stateMachine.ChangeState(BeginState.Instance);
                }

            }
        }
        


    }
    

    public GameObject getPlayerDribbling(GameObject[] players)
    {
        GameObject dribbler = null;
        foreach(GameObject g in players)
        {
            AI_Player a = g.GetComponent("AI_Player") as AI_Player;
            if (a != null)
            {
                if (a.stateMachine.currentState == Dribbling.Instance)
                {
                    dribbler = g;
                    break;
                }
            }
        }
        return dribbler;
    }

    public bool doubleDribbling()
    {
        bool t = false; ;

        foreach (GameObject p in player)
        {
            AI_Player a = p.GetComponent("AI_Player") as AI_Player;
            if (a != null)
            {
                if (a.stateMachine.currentState == Dribbling.Instance)
                {
                    t = true;
                    break;
                }
            }
        }
        if (t)
        {
            t = false;
            foreach (GameObject p in player1)
            {
                AI_Player a = p.GetComponent("AI_Player") as AI_Player;
                if (a != null)
                {
                    if (a.stateMachine.currentState == Dribbling.Instance)
                    {
                        t = true;
                        break;
                    }
                }
            }
        }
        return t;

    }
    // Update is called once per frame
    void Update () {
        if (!start)
        {

            bool t = isReadyForKickOff(player);
            bool t1 = isReadyForKickOff(player1);

            if (t && t1)
            {
                delta = Time.time;
                kickOff.SetActive(true);
                if (whokickOff > 0.5f)
                {
                    //Debug.Log(whokickOff + " bleu commence");
                    GameObject.Find("Player").GetComponent<AI_Player>().homeRegion -= 1;
                }
                else
                {
                    //Debug.Log(whokickOff + " rouge commence");
                    GameObject.Find("Player (10)").GetComponent<AI_Player>().homeRegion += 1;
                }
                start = true;
                ball.transform.position = new Vector3(0, 4f, 0);
                foreach (GameObject p in player)
                {
                    AI_Player a = p.GetComponent("AI_Player") as AI_Player;
                    if (a != null)
                    {
                        a.stateMachine.ChangeState(FirstState.Instance);
                    }
                    p.GetComponent<Rigidbody>().velocity *= 0;

                }

                foreach (GameObject p in player1)
                {
                    AI_Player a = p.GetComponent("AI_Player") as AI_Player;
                    if (a != null)
                    {
                        a.stateMachine.ChangeState(FirstState.Instance);
                    }
                    p.GetComponent<Rigidbody>().velocity *= 0;

                }
            }
        }
        else
        {
            kickOff.SetActive(false);
        }

    }

    public bool isReadyForKickOff(GameObject[] player)
    {
        bool t = true;
        foreach (GameObject p in player)
        {
            AI_Player a = p.GetComponent("AI_Player") as AI_Player;
            if (a != null)
            {
                if (!a.ready)
                {
                    t = false;
                    break;
                }
            }
            p.GetComponent<Rigidbody>().velocity *= 0;

        }
        return t;
    }


}

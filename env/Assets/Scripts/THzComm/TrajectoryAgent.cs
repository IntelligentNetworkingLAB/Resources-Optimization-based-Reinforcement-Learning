using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class TrajectoryAgent : Agent
{
    EnvironmentParameters m_ResetParams;
    public Coordination mCoordi;
    int USERSIZE;
   
    BufferSensorComponent m_BufferSensor;

    public override void Initialize()
    {
        m_ResetParams = Academy.Instance.EnvironmentParameters;
        m_BufferSensor = GetComponent<BufferSensorComponent>();
        InitEpisode();
    }

    public override void OnEpisodeBegin()
    {
        
        InitEpisode();
    }

    void InitEpisode()
    {
        //mCoordi.ResetEnvironment();
        USERSIZE = mCoordi.User_Num;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // 위치, 대역, 전력, 알파
        for (int i = 0; i < USERSIZE; ++i) // 15x2 = 30
        {
            float[] buf = new float[2];
            Vector3 userPos = mCoordi.transform.GetChild(i).position;
            buf[0] = (mCoordi.cur_Pos - new Vector2(userPos.x, userPos.z)).x;
            buf[1] = (mCoordi.cur_Pos - new Vector2(userPos.x, userPos.z)).y;
            //Vector3 userPos = mCoordi.transform.GetChild(i).position;
            //sensor.AddObservation(mCoordi.cur_Pos - new Vector2(userPos.x, userPos.z));
            m_BufferSensor.AppendObservation(buf);
        }
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float preFit = mCoordi.GetStepReward();

        var move_x = 2.5f * Mathf.Clamp(actions.ContinuousActions[0], -1f, 1f);
        var move_z = 2.5f * Mathf.Clamp(actions.ContinuousActions[1], -1f, 1f);
        mCoordi.cur_Pos.x += move_x;
        mCoordi.cur_Pos.y += move_z;
        mCoordi.cur_Vel = Mathf.Pow(move_x * move_x + move_z * move_z, 0.5f);
        
        float afterFit = mCoordi.GetStepReward();
        // Debug.Log(afterFit);
        if (preFit > afterFit)
        {
            SetReward(-1.0f);
            EndEpisode();
        }
        else
        {
            this.transform.position = new Vector3(mCoordi.cur_Pos.x, 0.0f, mCoordi.cur_Pos.y);
            SetReward(afterFit - preFit);
        }
    }
}

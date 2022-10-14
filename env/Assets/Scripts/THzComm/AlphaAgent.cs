using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class AlphaAgent : Agent
{
    EnvironmentParameters m_ResetParams;
    public Coordination mCoordi;
    int USERSIZE;
    int MAXUSER;

    BufferSensorComponent m_BufferSensor;

    public override void Initialize()
    {
        MAXUSER = 20;
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
        for (int i = 0; i < USERSIZE; ++i) // 15x6 = 90
        {
            Vector3 userPos = mCoordi.transform.GetChild(i).position;
            float[] buf = new float[2];
            buf[0] = i;
            buf[1] = Vector2.Distance(mCoordi.cur_Pos, new Vector2(userPos.x, userPos.z));
            //buf[1] = mCoordi.cur_Pow[i];
            m_BufferSensor.AppendObservation(buf);
            //Debug.Log(i.ToString() + "/" + Vector2.Distance(mCoordi.cur_Pos, new Vector2(userPos.x, userPos.z)).ToString());
        }
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float curAlpha = 0;
        float preFit = mCoordi.GetStepReward();
        for (int i = 0; i < USERSIZE; ++i)
        {
            mCoordi.cur_Alpa[i] = 0.2f * Mathf.Clamp(actions.ContinuousActions[i], -1f, 1f) + 0.5f;
            curAlpha += mCoordi.cur_Alpa[i];
        }
        //Debug.Log(curAlpha/USERSIZE);
        float afterFit = mCoordi.GetStepReward();
        if (preFit > afterFit)
        {
            SetReward(-1.0f);
            EndEpisode();
        }
        else
        {
            SetReward(0.2f * (afterFit - preFit));
        }
    }
}

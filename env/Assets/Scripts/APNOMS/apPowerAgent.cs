using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class apPowerAgent : Agent
{
    EnvironmentParameters m_ResetParams;
    public apCoordination mCoordi;
    int USERSIZE;

    public override void Initialize()
    {
        m_ResetParams = Academy.Instance.EnvironmentParameters;
        InitEpisode();
    }

    public override void OnEpisodeBegin()
    {
        InitEpisode();
    }

    void InitEpisode()
    {
        //mCoordi.ResetEnvironment();
        USERSIZE = mCoordi.UserNum;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // 위치, 대역, 전력, 알파
        for (int i = 0; i < USERSIZE; ++i) // 15
        {
            // sensor.AddObservation(mCoordi.vBand);
            Vector3 userPos = mCoordi.transform.GetChild(i).position;
            sensor.AddObservation(Vector2.Distance(mCoordi.vPos, new Vector2(userPos.x, userPos.z)));
        }
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float preFit = mCoordi.GetDataRate();
        for (int i = 0; i < USERSIZE; ++i)
        {
            mCoordi.vPower[i] = (1f / (2 * USERSIZE)) * Mathf.Clamp(actions.ContinuousActions[i], -1f, 1f) + (1f / (USERSIZE));
        }
        float afterFit = mCoordi.GetDataRate();
        if (IsContraint() || preFit > afterFit)
        {
            SetReward(-1.0f);
            EndEpisode();
        }
        else
        {
            SetReward(afterFit - preFit);
        }
    }

    bool IsContraint()
    {
        float sum = 0f;
        for (int i = 0; i < USERSIZE; ++i)
        {
            sum += mCoordi.vPower[i];
        }
        if (sum > 1.0f)
        {
            return true;
        }
        return false;
    }
}

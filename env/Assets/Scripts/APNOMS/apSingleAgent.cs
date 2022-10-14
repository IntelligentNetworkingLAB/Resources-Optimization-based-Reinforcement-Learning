using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class apSingleAgent : Agent
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
        for (int i = 0; i < USERSIZE; ++i) // 15x2 = 30
        {
            Vector3 userPos = mCoordi.transform.GetChild(i).position;
            sensor.AddObservation(mCoordi.vPos - new Vector2(userPos.x, userPos.z));
        }
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float preFit = mCoordi.GetDataRate();
        var move_x = 6f * Mathf.Clamp(actions.ContinuousActions[0], -1f, 1f);
        var move_z = 6f * Mathf.Clamp(actions.ContinuousActions[1], -1f, 1f);

        for (int i = 0; i < USERSIZE; ++i)
        {
            mCoordi.vBand[i] = (1f / (2 * USERSIZE)) * Mathf.Clamp(actions.ContinuousActions[2 * i + 2], -1f, 1f) + (1f / (USERSIZE));
            mCoordi.vPower[i] = (1f / (2 * USERSIZE)) * Mathf.Clamp(actions.ContinuousActions[2 * i + 3], -1f, 1f) + (1f / (USERSIZE));
        }

        mCoordi.vPos.x += move_x;
        mCoordi.vPos.y += move_z;

        float afterFit = mCoordi.GetDataRate();
        if (IsContraintBand() || IsContraintPower() || preFit > afterFit)
        {
            SetReward(-1.0f);
            EndEpisode();
        }
        else
        {
            this.transform.position = new Vector3(mCoordi.vPos.x, 0.0f, mCoordi.vPos.y);
            SetReward(afterFit - preFit);
        }
    }

    bool IsContraintBand()
    {
        float sum = 0f;
        for (int i = 0; i < USERSIZE; ++i)
        {
            sum += mCoordi.vBand[i];
        }
        if (sum > 1.0f)
        {
            return true;
        }
        return false;
    }

    bool IsContraintPower()
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

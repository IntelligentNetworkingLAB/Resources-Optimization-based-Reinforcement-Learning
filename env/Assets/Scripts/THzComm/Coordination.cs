using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Coordination : MonoBehaviour
{
    /// <summary>
    /// Optimiation Param.
    /// </summary>
    public float[] cur_Wup;
    public float[] cur_Wdown;
    public Vector2 cur_Pos;
    public float cur_Vel;
    public float[] cur_Pow;
    public float[] cur_Alpa;

    ///////////////////////////////////////////////////////
    /// <summary>
    /// Calculation Param.
    /// </summary>
    float D_pre;
    float[] User_D_mec;
    float[] User_D_in;
    float[] User_D_post;

    float[] User_R_ul;
    float[] User_R_dl;

    float[] User_t_ul;
    float[] User_t_dl;
    float[] User_t_comp;

    float[] User_E_ul;
    float[] User_E_dl;
    float[] User_E_comp;
    float UAV_E_fly;
    ///////////////////////////////////////////////////////
    
    public int User_Num;
    public int MAXUSER;
    float[] User_c_mec;
    float[] User_c_in;

    void Start()
    {
        ResetEnvironment();
        timeslot = 0;
        rrrr = "";
        totalpos = "";
        totalDis = "";
        totalresource = "";
        totalAlpha = "";
        GetStepReward();
    }

    public void ResetEnvironment()
    {
        MAXUSER = 20;
        //User_Num = Random.Range(15, 20);
        User_Num = 17;

        User_D_mec = new float[User_Num];
        User_D_in = new float[User_Num];
        User_D_post = new float[User_Num];
        User_R_ul = new float[User_Num];
        User_R_dl = new float[User_Num];
        User_t_ul = new float[User_Num];
        User_t_dl = new float[User_Num];
        User_t_comp = new float[User_Num];
        User_E_ul = new float[User_Num];
        User_E_dl = new float[User_Num];
        User_E_comp = new float[User_Num];
        UAV_E_fly = new float();
        User_c_mec = new float[User_Num];
        User_c_in = new float[User_Num];

        cur_Wup = new float[MAXUSER];
        cur_Wdown = new float[MAXUSER];
        cur_Pow = new float[MAXUSER];
        cur_Alpa = new float[MAXUSER];

        //InitEnviron();
        TestEnviron();
    }
    
    int timeslot;
    string rrrr;
    string totalpos;
    string totalDis;
    string totalresource;
    string totalAlpha;
    string totalWup;
    string totalWdown;
    string totalPow;

    private void LateUpdate()
    {
        timeslot++;
        rrrr += GetStepReward().ToString() + "/";
        if (timeslot <= 100)
        {
            totalWup += cur_Wup.Sum() + "/";
            totalWdown += cur_Wdown.Sum() + "/";
            totalPow += cur_Pow.Sum() + "/";
        }
        else
        {
            Debug.Log(totalWup);
            Debug.Log(totalWdown);
            Debug.Log(totalPow);
        }

        //if (timeslot == 1)
        //{
        //    totalDis = "";
        //    totalresource = "";
        //    totalAlpha = "";
        //    Debug.Log(cur_Pos);
        //    CalculateParam();
        //    Debug.Log(totalpos);
        //    for (int i = 0; i < User_Num; ++i)
        //    {
        //        totalDis += Vector3.Distance(new Vector3(cur_Pos.x, 50f, cur_Pos.y), transform.GetChild(i).gameObject.transform.position) + " ";
        //        totalAlpha += ((User_R_ul[i] + User_R_dl[i]) / 2f).ToString() + " ";
        //        totalresource += cur_Wup[i] + "," + cur_Wdown[i] + "," + cur_Pow[i] + " ";
        //    }
        //    Debug.Log(totalDis);
        //    Debug.Log(User_R_ul.Sum() + "/" + User_R_dl.Sum());
        //    Debug.Log(totalAlpha);
        //    Debug.Log(totalresource);
        //}

        //if (timeslot >= 1)
        //{
        //    if (cur_Wup[0] != 1f / User_Num && cur_Wdown[0] != 1f / User_Num && cur_Pow[0] != 1f / User_Num)
        //    {
        //        totalDis = "";
        //        totalresource = "";
        //        totalAlpha = "";
        //        //Debug.Log(rrrr);
        //        CalculateParam();
        //        Debug.Log(totalpos);
        //        for (int i = 0; i < User_Num; ++i)
        //        {
        //            totalDis += Vector3.Distance(new Vector3(cur_Pos.x, 50f, cur_Pos.y), transform.GetChild(i).gameObject.transform.position) + " ";
        //            totalAlpha += ((User_R_ul[i] + User_R_dl[i]) / 2f).ToString() + " ";
        //            totalresource += cur_Wup[i] + "," + cur_Wdown[i] + "," + cur_Pow[i] + " ";
        //        }
        //        Debug.Log(totalDis);
        //        Debug.Log(User_R_ul.Sum() + "/" + User_R_dl.Sum());
        //        Debug.Log(totalAlpha);
        //        Debug.Log(totalresource);
        //    }
        //}
    }

    void InitEnviron()
    {
        for (int i = 0; i < MAXUSER; ++i)
        {
            cur_Wup[i] = 1f / User_Num;
            cur_Wdown[i] = 1f / User_Num;
            cur_Pow[i] = 1f / User_Num;
            cur_Alpa[i] = 0.5f;
        }
        for (int i = 0; i < User_Num; ++i)
        {
            User_c_mec[i] = 1f / User_Num;
            User_c_in[i] = 1f / User_Num;
        }
        cur_Pos = new Vector2(Random.Range(-300f, 300f), Random.Range(-300f, 300f));
        cur_Vel = 0f;

        int sector = Random.Range(0, 9);
        switch (sector)
        {
            case 0:
                {
                    for (int i = 0; i < User_Num; ++i)
                    {
                        transform.GetChild(i).transform.position = new Vector3(Random.Range(-100.0f - 200.0f, 100.0f - 200.0f), 0f, Random.Range(-100.0f - 200.0f, 100.0f - 200.0f));
                    }
                }
                break;
            case 1:
                {
                    for (int i = 0; i < User_Num; ++i)
                    {
                        transform.GetChild(i).transform.position = new Vector3(Random.Range(-100.0f, 100.0f), 0f, Random.Range(-100.0f - 200.0f, 100.0f - 200.0f));
                    }
                }
                break;
            case 2:
                {
                    for (int i = 0; i < User_Num; ++i)
                    {
                        transform.GetChild(i).transform.position = new Vector3(Random.Range(-100.0f + 200.0f, 100.0f + 200.0f), 0f, Random.Range(-100.0f - 200.0f, 100.0f - 200.0f));
                    }
                }
                break;
            case 3:
                {
                    for (int i = 0; i < User_Num; ++i)
                    {
                        transform.GetChild(i).transform.position = new Vector3(Random.Range(-100.0f - 200.0f, 100.0f - 200.0f), 0f, Random.Range(-100.0f, 100.0f));
                    }
                }
                break;
            case 4:
                {
                    for (int i = 0; i < User_Num; ++i)
                    {
                        transform.GetChild(i).transform.position = new Vector3(Random.Range(-100.0f, 100.0f), 0f, Random.Range(-100.0f, 100.0f));
                    }
                }
                break;
            case 5:
                {
                    for (int i = 0; i < User_Num; ++i)
                    {
                        transform.GetChild(i).transform.position = new Vector3(Random.Range(-100.0f + 200.0f, 100.0f + 200.0f), 0f, Random.Range(-100.0f, 100.0f));
                    }
                }
                break;
            case 6:
                {
                    for (int i = 0; i < User_Num; ++i)
                    {
                        transform.GetChild(i).transform.position = new Vector3(Random.Range(-100.0f - 200.0f, 100.0f - 200.0f), 0f, Random.Range(-100.0f + 200.0f, 100.0f + 200.0f));
                    }
                }
                break;
            case 7:
                {
                    for (int i = 0; i < User_Num; ++i)
                    {
                        transform.GetChild(i).transform.position = new Vector3(Random.Range(-100.0f, 100.0f), 0f, Random.Range(-100.0f + 200.0f, 100.0f + 200.0f));
                    }
                }
                break;
            case 8:
                {
                    for (int i = 0; i < User_Num; ++i)
                    {
                        transform.GetChild(i).transform.position = new Vector3(Random.Range(-100.0f + 200.0f, 100.0f + 200.0f), 0f, Random.Range(-100.0f + 200.0f, 100.0f + 200.0f));
                    }
                }
                break;
        }
    }

    void TestEnviron()
    {
        //for (int i = 0; i < MAXUSER; ++i)
        //{
        //    cur_Wup[i] = 0f;
        //    cur_Wdown[i] = 0f;
        //    cur_Pow[i] = 0f;
        //    cur_Alpa[i] = 0.5f;
        //}
        //for (int i = 0; i < 70; ++i)
        //{
        //    cur_Wup[Random.Range(0, 16)] += 1f / 100f;
        //    cur_Wdown[Random.Range(0, 16)] += 1f / 100f;
        //    cur_Pow[Random.Range(0, 16)] += 1f / 100f;
        //}


        for (int i = 0; i < MAXUSER; ++i)
        {
            cur_Wup[i] = 1f / User_Num;
            cur_Wdown[i] = 1f / User_Num;
            cur_Pow[i] = 1f / User_Num;
            cur_Alpa[i] = 0.5f;
        }
        for (int i = 0; i < User_Num; ++i)
        {
            User_c_mec[i] = 1f / User_Num;
            User_c_in[i] = 1f / User_Num;
        }
        //cur_Pos = new Vector2(Random.Range(-300f, 300f), Random.Range(-300f, 300f));
        //cur_Pos = new Vector2(95.1f, -131.2f);
        cur_Pos = new Vector2(0f, 0f);
        cur_Vel = 0f;

        string totalPos = "-31.44334914,100.8701021,37.95582881,175.1982718,208.6271266,101.744534,66.4633281,129.7745329,132.9954467,173.0413988,168.9863431,209.789158,133.6161026,242.9687544,126.6451863,237.637448,27.07501009,106.9674838,89.86829471,73.83950935,100.4604736,203.5166626,42.04572777,231.9548271,86.35926272,104.8139008,176.712602,-9.214176257,-28.03094271,158.1382188,-69.02144557,207.8243638,126.127857,55.26947502";
        string[] words = totalPos.Split(',');

        for (int i = 0; i < User_Num; ++i)
        {
            transform.GetChild(i).transform.position = new Vector3(float.Parse(words[2 * i]), 0f, float.Parse(words[2 * i + 1]));
        }

    }

    public float GetRate()
    {
        CalculateParam();
        float result = User_R_ul.Sum() + User_R_dl.Sum();
        return GetRate();
    }

    public float GetStepReward()
    {
        float eta = 0.5f;

        CalculateParam();

        float totalEnergy = 0f;
        float totalDelay = 0f;
        for(int i = 0; i < User_Num; ++i)
        {
            //totalEnergy += User_E_ul[i] + User_E_dl[i] + User_E_comp[i];
            //totalDelay += User_t_ul[i] + User_t_dl[i] + User_t_comp[i];
            totalEnergy += User_E_ul[i] + User_E_dl[i];
            totalDelay += User_t_ul[i] + User_t_dl[i] + User_t_comp[i];
        }
        totalEnergy += UAV_E_fly;
        totalEnergy *= 0.004f;

        float result = eta * totalEnergy + (1 - eta) * totalDelay;
        result = 1 / result;
        result *= 1e+4f;
        return result;
    }

    void CalculateParam()
    {
        D_pre = 100;
        float B_ul = 1e+3f;
        float B_dl = 1e+3f;
        float N = -174f; // fix
        float p_0 = 5e+2f;
        float p_max = 5e+3f;
        float c_in_max = 2e+1f;
        float c_mec_max = 2e+2f;
        float h_0 = -40f;
        float a_f = 0.005f; // fix

        float delta = 1.0f;
        float beta_v = 0.5f;
        float beta_u = 25.0f;
        float q_v = 0.1f;
        float q_u = 0.1f;
        
        float t_fly = 0.5f;

        for (int i = 0; i < User_Num; ++i)
        {
            //Debug.Log(transform.GetChild(i).name);
            float dis = Vector3.Distance(new Vector3(cur_Pos.x, 100f, cur_Pos.y), transform.GetChild(i).gameObject.transform.position);

            User_R_ul[i] = 1e+4f * cur_Wup[i] * B_ul * Mathf.Log(1 + p_0 * h_0 / (cur_Wup[i] * B_ul * Mathf.Pow(dis, 2) * Mathf.Exp(a_f * dis) * N), 2);
            //User_R_ul[i] = 1e+4f * cur_Wup[i] * B_ul * Mathf.Log(1 + cur_Pow[i] * 8e+3f * h_0 / (cur_Wup[i] * B_ul * Mathf.Pow(dis, 2) * Mathf.Exp(a_f * dis) * N), 2);
            User_R_dl[i] = 1e+4f * cur_Wdown[i] * B_dl * Mathf.Log(1 + cur_Pow[i] * p_max * h_0 / (cur_Wdown[i] * B_dl * Mathf.Pow(dis, 2) * Mathf.Exp(a_f * dis) * N), 2);
            // Debug.Log(User_R_ul[i].ToString() + "/" + User_R_dl[i].ToString() + "/" + dis.ToString());

            User_D_mec[i] = cur_Alpa[i] * D_pre;
            User_D_in[i] = (1 - cur_Alpa[i]) * D_pre;

            User_t_ul[i] = User_D_mec[i] / User_R_ul[i];

            User_D_post[i] = delta * User_D_mec[i];
            User_t_dl[i] = User_D_post[i] / User_R_dl[i];

            User_t_comp[i] = beta_v * User_D_mec[i] / (User_c_mec[i] * c_mec_max) + beta_u * User_D_in[i] / (User_c_in[i] * c_in_max);
            User_t_comp[i] *= 0.02f;

            User_E_ul[i] = User_t_ul[i] * p_0;
            User_E_dl[i] = User_t_dl[i] * cur_Pow[i] * p_max;
            User_E_comp[i] = q_v * Mathf.Pow(User_c_mec[i] * c_mec_max, 2) * beta_v * User_D_mec[i] + q_u * Mathf.Pow(User_c_in[i] * c_in_max, 2) * beta_u * User_D_in[i];
            User_E_comp[i] *= 0.1f;
            //if(cur_Vel == 0f)
            //{
            //    UAV_E_fly = 0.0f;
            //}
            //else
            //{
            //    UAV_E_fly = cur_Vel * (c_1 * Mathf.Pow(cur_Vel / t_fly, 2) + c_2 / Mathf.Pow(cur_Vel / t_fly, 2));
            //}
            UAV_E_fly = Mathf.Pow(cur_Vel, 0.5f) * t_fly;
        }
    }
}
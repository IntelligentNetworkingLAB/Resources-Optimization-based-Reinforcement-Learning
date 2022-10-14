using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class apCoordination : MonoBehaviour
{
    public float[] vBand;
    public float[] vPower;
    public Vector2 vPos;
    public int UserNum;
    
    float p_0 = 4.0f;
    float w_0 = 30e3F;

    private void Start()
    {
        ResetEnvironment();
    }

    public void ResetEnvironment()
    {
        UserNum = transform.childCount;
        vBand = new float[UserNum];
        vPower = new float[UserNum];
        vPos = new Vector2();

        RandomSetting();
    }

    public void RandomSetting()
    {
        //UAV position
        vPos.x = Random.Range(-300.0f, 300.0f);
        vPos.y = Random.Range(-300.0f, 300.0f);

        //Resource Set
        for (int i = 0; i < UserNum; ++i)
        {
            //vBand[i] = 0.01f;
            //vPower[i] = 0.01f;
            vBand[i] = 1f / UserNum;
            vPower[i] = 1f / UserNum;
        }
        //for (int i = 0; i < 85; ++i)
        //{
        //    int sed1= Random.Range(0, UserNum);
        //    vBand[sed1] += 0.01f;
        //    int sed2 = Random.Range(0, UserNum);
        //    vPower[sed2] += 0.01f;
        //}

        int sector = Random.Range(0, 9);
        switch (sector)
        {
            case 0:
                {
                    for (int i = 0; i < UserNum; ++i)
                    {
                        transform.GetChild(i).transform.position = new Vector3(Random.Range(-100.0f - 200.0f, 100.0f - 200.0f), 0f, Random.Range(-100.0f - 200.0f, 100.0f - 200.0f));
                    }
                }
                break;
            case 1:
                {
                    for (int i = 0; i < UserNum; ++i)
                    {
                        transform.GetChild(i).transform.position = new Vector3(Random.Range(-100.0f, 100.0f), 0f, Random.Range(-100.0f - 200.0f, 100.0f - 200.0f));
                    }
                }
                break;
            case 2:
                {
                    for (int i = 0; i < UserNum; ++i)
                    {
                        transform.GetChild(i).transform.position = new Vector3(Random.Range(-100.0f + 200.0f, 100.0f + 200.0f), 0f, Random.Range(-100.0f - 200.0f, 100.0f - 200.0f));
                    }
                }
                break;
            case 3:
                {
                    for (int i = 0; i < UserNum; ++i)
                    {
                        transform.GetChild(i).transform.position = new Vector3(Random.Range(-100.0f - 200.0f, 100.0f - 200.0f), 0f, Random.Range(-100.0f, 100.0f));
                    }
                }
                break;
            case 4:
                {
                    for (int i = 0; i < UserNum; ++i)
                    {
                        transform.GetChild(i).transform.position = new Vector3(Random.Range(-100.0f, 100.0f), 0f, Random.Range(-100.0f, 100.0f));
                    }
                }
                break;
            case 5:
                {
                    for (int i = 0; i < UserNum; ++i)
                    {
                        transform.GetChild(i).transform.position = new Vector3(Random.Range(-100.0f + 200.0f, 100.0f + 200.0f), 0f, Random.Range(-100.0f, 100.0f));
                    }
                }
                break;
            case 6:
                {
                    for (int i = 0; i < UserNum; ++i)
                    {
                        transform.GetChild(i).transform.position = new Vector3(Random.Range(-100.0f - 200.0f, 100.0f - 200.0f), 0f, Random.Range(-100.0f + 200.0f, 100.0f + 200.0f));
                    }
                }
                break;
            case 7:
                {
                    for (int i = 0; i < UserNum; ++i)
                    {
                        transform.GetChild(i).transform.position = new Vector3(Random.Range(-100.0f, 100.0f), 0f, Random.Range(-100.0f + 200.0f, 100.0f + 200.0f));
                    }
                }
                break;
            case 8:
                {
                    for (int i = 0; i < UserNum; ++i)
                    {
                        transform.GetChild(i).transform.position = new Vector3(Random.Range(-100.0f + 200.0f, 100.0f + 200.0f), 0f, Random.Range(-100.0f + 200.0f, 100.0f + 200.0f));
                    }
                }
                break;
        }
    }
    
    public float GetDataRate()
    {
        float g_0 = 10.0f;
        float sigma = 174f;
        float alpha = 0.7f;

        float result = 0.0f;
        for (int i = 0; i < UserNum; ++i)
        {
            float dist = Vector3.Distance(new Vector3(vPos.x, 50.0f, vPos.y), transform.GetChild(i).transform.position);
            float gain = g_0 / Mathf.Pow(dist, alpha);
            float sinr = (vPower[i] * p_0 * gain) / sigma;
            result += vBand[i] * w_0 * Mathf.Log(1 + sinr);
        }
        Debug.Log(result);
        return result;
    }
}

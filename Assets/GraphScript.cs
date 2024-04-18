using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GraphScript : MonoBehaviour
{
    public int pixelX;
    public int pixelY;

    private float countResult;

    // Start is called before the first frame update
    void Start()
    {
       
    }

    public void UpdateGraph(int[] matchPosition)
    {
        countResult = gameObject.transform.parent.parent.gameObject.GetComponent<GameMasterController>().tournamentCount * GameObject.Find("GameMaster").GetComponent<GameMasterController>().matchCount;
        for (int i = 0; i < 4; i++) 
        {
            gameObject.transform.GetChild(i).gameObject.GetComponent<LineRenderer>().positionCount += 1;
            gameObject.transform.GetChild(i).gameObject.GetComponent<LineRenderer>().SetPosition(gameObject.transform.GetChild(i).gameObject.GetComponent<LineRenderer>().positionCount-1,new Vector3 (gameObject.transform.GetChild(i).gameObject.GetComponent<LineRenderer>().positionCount * 1920 / countResult  , matchPosition[i] *1080 / (3*countResult) ,0));
        }
        if (gameObject.transform.GetChild(0).gameObject.GetComponent<LineRenderer>().positionCount == countResult + 1)
        {
            UpdateHeight();
        }
    }



    private void UpdateHeight()
    {
        float maxY=0;
        Vector3 ZW ;
        for (int i = 0;i < 4;i++) 
        {
            ZW = gameObject.transform.GetChild(i).gameObject.GetComponent<LineRenderer>().GetPosition(gameObject.transform.GetChild(i).gameObject.GetComponent<LineRenderer>().positionCount-1);
            if(ZW.y > maxY) maxY = ZW.y;
        }
        for (int i = 0; i < 4; i++)
        {
            float[] Ai = new float[gameObject.transform.GetChild(i).gameObject.GetComponent<LineRenderer>().positionCount];
            Vector3[] AiZW = new Vector3[gameObject.transform.GetChild(i).gameObject.GetComponent<LineRenderer>().positionCount];
            gameObject.transform.GetChild(i).gameObject.GetComponent<LineRenderer>().GetPositions(AiZW);

            for (int j = 0; j < AiZW.Length; j++)
            {
                Ai[j] = AiZW[j].y;
            }

            for (int j = 0; j < Ai.Length; j++)
            {
                Ai[j] = Ai[j] / maxY * 1080;
            }

            for (int j = 0; j < AiZW.Length; j++)
            {
                AiZW[j].y = Ai[j];
            }

            gameObject.transform.GetChild(i).gameObject.GetComponent<LineRenderer>().SetPositions(AiZW);
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

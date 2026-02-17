using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enterMeasure : MonoBehaviour
{
    private List<float> enterValues = new List<float>();
    [SerializeField] private float inputLimitTime = 1f;
    private float inputTimer = 0f;


    void Update()
    {
        if (Input.GetKey(KeyCode.Return) && inputTimer > 0f)
        {
            inputTimer -= Time.deltaTime;

            float enterValue = Input.GetAxis("enter");
            if (enterValue > 0f && enterValue < 1f)
            {
                enterValues.Add(1f / enterValue);
                //Debug.Log("Enter Value: " + enterValues[enterValues.Count - 1]);
            }
        }
        else if (Input.GetKeyUp(KeyCode.Return))
        {
            //加速度の平均を求める場合
            float acceleration = 0f;
            for (int i = 2; i < enterValues.Count - 1; i++)
            {
                acceleration += Mathf.Abs(enterValues[i] - enterValues[i - 1]);
            }
            Debug.Log("Acceleration: " + acceleration / (enterValues.Count - 1));
            /*
            //最も大きな加速度を求める場合
            float acceleration = 0f;
            for (int i = 1; i < enterValues.Count - 1; i++)
            {
                float diff = Mathf.Abs(enterValues[i] - enterValues[i - 1]);
                if (diff > acceleration)
                {
                    acceleration = diff;
                }
            }
            Debug.Log("Acceleration: " + acceleration);
            */

            enterValues.Clear();
            inputTimer = inputLimitTime;
        }
    }
}

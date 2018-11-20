using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RTSCoreFramework;

namespace RTSPrototype.PerformanceTest
{
    public class CSharpPerformanceTestComponent : MonoBehaviour
    {
        System.DateTime MyTime;
        [Range(1, 1000000)]
        public int NumberOfLoops = 10;

        #region UnityMessages
        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.transform.tag == "Ally")
            {
                MyTime = System.DateTime.Now;
                //Normal Test
                //float _results = GetTotalSum(NumberOfLoops);
                //Owner Test
                //float _results = GetTotalSumFromOwner(NumberOfLoops);
                //Ally Test
                float _results = GetTotalSumFromAlly(NumberOfLoops, other);
                PrintResults(_results);
            }
        }
        #endregion

        #region GetTotalSumFormulas
        public float GetTotalSum(float N)
        {
            float result = 0;
            for (int i = 1; i <= N; i++)
            {
                result += SumN(i);
            }
            return result;
        }

        public float SumN(float n)
        {
            float _temp = 0;
            for (int i = 1; i <= n; i++)
            {
                _temp += i;
            }
            return _temp;
        }
        #endregion

        #region GetTotalSumFromOwnerFormula
        public float GetTotalSumFromOwner(float N)
        {
            float result = 0;
            for (int i = 1; i <= N; i++)
            {
                result += GetOwnerSumNTest(i);
            }
            return result;
        }

        public float GetOwnerSumNTest(float n)
        {
            float _temp = 0;
            for (int i = 1; i <= n; i++)
            {
                _temp += transform.position.x;
            }
            return _temp;
        }
        #endregion

        #region GetTotalSumFromAllyOwnerFormula
        public float GetTotalSumFromAlly(float N, Collider other)
        {
            float result = 0;
            for (int i = 1; i <= N; i++)
            {
                result += GetAllySumNTest(i, other);
            }
            return result;
        }

        public float GetAllySumNTest(float n, Collider other)
        {
            float _temp = 0;
            for (int i = 1; i <= n; i++)
            {
                var _ally = other.GetComponent<AllyMember>();
                if(_ally != null)
                {
                    _temp += _ally.AllyHealth;
                }
            }
            return _temp;
        }
        #endregion

        #region PrintResults
        void PrintResults(float _results)
        {
            var _lengthOfTime = System.DateTime.Now - MyTime;
            string _output = _lengthOfTime.TotalMilliseconds.ToString() +
                " ms - result: " + _results.ToString();
            Debug.Log("Performance Test With " + NumberOfLoops + " Loops...");
            Debug.Log(_output);
        }
        #endregion
    }
}
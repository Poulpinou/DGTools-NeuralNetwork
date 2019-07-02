using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DGTools.NeuralNetwork.Test
{
    public class PlatformBrain : QBrain
    {
        //VARIABLES
        [Header("Specific Settings")]
        public BallState ball;
        public float tiltSpeed = 0.5f;

        Vector3 startPos;
        int failCount = 0;

        //METHODS
        public override void ResetToDefault()
        {
            ball.transform.position = startPos;
            ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
            ball.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            transform.rotation = Quaternion.identity;
        }

        protected override bool CheckIfShouldTrain()
        {
            return ball.GetComponent<BallState>().dropped;
        }

        protected override float GetReward()
        {
            return ball.GetComponent<BallState>().dropped ? -1 : 0.1f;
        }

        protected override List<double> GetStates()
        {
            List<double> states = new List<double>();

            states.Add(transform.rotation.x);
            states.Add(ball.transform.position.z);
            states.Add(ball.GetComponent<Rigidbody>().angularVelocity.x);

            return states;
        }

        protected override void PerformAction(int actionIndex, float outputValue)
        {
            switch (actionIndex) {
                case 0:
                    transform.Rotate(Vector3.right, tiltSpeed * outputValue);
                    break;
                case 1:
                    transform.Rotate(Vector3.right, -tiltSpeed * outputValue);
                    break;
                default:
                    throw new System.Exception();
            }
        }

        protected override void PostTraining()
        {
            ball.GetComponent<BallState>().dropped = false;
            transform.rotation = Quaternion.identity;
            ResetToDefault();
            //memory.experiences.Clear();
            failCount++;
        }

        //RUNTIME METHODS
        protected override void Start()
        {
            base.Start();
            startPos = ball.transform.position;
            Time.timeScale = 5.0f;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace DGTools.NeuralNetwork {
    public abstract class QBrain : Brain
    {
        //VARIABLES
        [Header("Quality settings")]
        public float discount = 0.99f;

        [Header("Behaviour exploration")]
        public bool enableExploration = true;
        [Readonly] public float exploreRate = 100.0f;
        [Range(0, 100)] public float maxExploreRate = 100.0f;
        [Range(0, 100)] public float minExploreRate = 0.01f;
        public float exploreDecay = 0.001f;

        //METHODS
        /// <summary>
        /// All the brain job is done here and is called every frame
        /// (does not inherit from Brain)
        /// </summary>
        protected override void Run()
        {
            List<double> states = GetStates();
            List<double> qStates = network.CalcOutput(states).SoftMax();

            double maxQ = qStates.Max();
            int maxQIndex = qStates.IndexOf(maxQ);

            if (enableExploration) {
                exploreRate = Mathf.Clamp(exploreRate - exploreDecay, minExploreRate, maxExploreRate);

                if (Random.Range(0, 100) < exploreRate)
                {
                    maxQIndex = Random.Range(0, qStates.Count);
                }
            }

            PerformAction(maxQIndex, (float)qStates[maxQIndex]);

            if (canTrain) {
                memory.AddExperience(new Experience(GetReward(), states));

                if (CheckIfShouldTrain())
                {
                    Train();
                }
            }
        }

        /// <summary>
        /// All the training job is done here and is called if CheckIfShouldTrain() == true
        /// (does not inherit from Brain)
        /// </summary>
        protected override void Train()
        {
            for (int i = memory.experiences.Count - 1; i >= 0; i--)
            {
                List<double> tOutputsOld = new List<double>();
                List<double> tOutputsNew = new List<double>();

                tOutputsOld = network.CalcOutput(memory.experiences[i].states).SoftMax();
                double maxQ = tOutputsOld.Max();
                int action = tOutputsOld.IndexOf(maxQ);

                double feedback;
                if (i == memory.experiences.Count - 1 || memory.experiences[i].reward == -1.0f)
                    feedback = memory.experiences[i].reward;
                else
                {
                    tOutputsNew = network.CalcOutput(memory.experiences[i + 1].states).SoftMax();
                    maxQ = tOutputsNew.Max();
                    feedback = (memory.experiences[i].reward + discount * maxQ);
                }

                tOutputsOld[action] = feedback;
                network.Train(memory.experiences[i].states, tOutputsOld);
            }
            PostTraining();
        }

        /// <summary>
        /// Draw a debug on GUI
        /// </summary>
        /// <returns>Returns the last block Rect to help you to place your debug</returns>
        protected override Rect DrawDebug()
        {
            Rect lastBlock = base.DrawDebug();
            Rect block = new Rect(lastBlock.xMin, lastBlock.yMax, lastBlock.width, 20);

            GUI.BeginGroup(block);
            GUI.Label(new Rect(10, 0, block.width, 20), enableExploration? string.Format("Exploration Rate : {0}% (-{1}%/f)", exploreRate.ToString("0.00"), exploreDecay): "Exploration : Disabled");
            GUI.EndGroup();
            return block;
        }

        /// <summary>
        /// Ovrride this method to check brain validity
        /// </summary>
        /// <returns>Returns a list of error (if count = 0 brain is valid)</returns>
        protected override List<string> CheckRules()
        {
            List<string> errors = base.CheckRules();

            int statesCount = GetStates().Count;
            if (statesCount != network.numInputs)
                errors.Add(string.Format("<b>GetStates()</b> should return as many states as NeuralNetwork inputs ({0} returned for {1} inputs)", statesCount, network.numInputs));

            try
            {
                PerformAction(network.numOutputs - 1, 0);
            }
            catch {
                errors.Add(string.Format("<b>PerformAction()</b> failed with index {0}, the amount of action to perform should be equal to {1}", network.numOutputs - 1, network.numOutputs));
            }

            return errors;
        }

        //ABSTRACT METHODS
        /// <summary>
        /// This method should provide all states that your brain needs (count should be equal to Network.nbInputs)
        /// </summary>
        /// <returns>Returns a list of states</returns>
        protected abstract List<double> GetStates();

        /// <summary>
        /// Performs the selected action from an index, a switch on this index is a good option
        /// </summary>
        /// <param name="actionIndex">Index of the action (min index = 0; max index = Network.nbOutputs - 1)</param>
        /// <param name="outputValue">Output value of the neural network for this action</param>
        protected abstract void PerformAction(int actionIndex, float outputValue);

        /// <summary>
        /// Put here the value of the reward for the current state
        /// </summary>
        /// <returns>Value of the reward (min = -1.0f; max = 1.0f)</returns>
        protected abstract float GetReward();

        //EDITOR METHODS
        private void OnValidate()
        {
            //Check explore rates
            if (minExploreRate > maxExploreRate) minExploreRate = maxExploreRate;
            exploreRate = maxExploreRate;
            
        }
    }
}


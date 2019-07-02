using System.Collections.Generic;
using UnityEngine;

namespace DGTools.NeuralNetwork {
    public class Neuron
    {
        //VARIABLES
        public int numInputs;
        public double bias;
        public double output;
        public double errorGradient;
        public List<double> weights = new List<double>();
        public List<double> inputs = new List<double>();

        //CONSTRUCTORS
        public Neuron(int nIntputs)
        {
            bias = Random.Range(-1f, 1f);
            numInputs = nIntputs;
            for (int i = 0; i < nIntputs; i++)
            {
                weights.Add(Random.Range(-1f, 1f));
            }
        }
    }
}


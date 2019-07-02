using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace DGTools.NeuralNetwork {
    [Serializable]
    public class NeuralNetwork
    {
        //VARIABLES
        [Header("Structure")]
        public int numInputs;
        public int numOutputs;
        public int numHidden;
        public int numNeuronPerHidden;
        [Range(0, 1)]
        public double alpha;

        [Header("Activation functions")]
        public ActivationFunction activationFunctionHidden;
        public ActivationFunction activationFunctionOutput;

        [HideInInspector] public List<Layer> layers;

        //CONSTRUCTORS
        public NeuralNetwork(int nInputs, int nOutputs, int nHidden, int nNeuronPerHidden, double a)
        {
            numInputs = nInputs;
            numOutputs = nOutputs;
            numHidden = nHidden;
            numNeuronPerHidden = nNeuronPerHidden;
            alpha = a;

            Init();
        }

        //METHODS
        public void Init()
        {
            layers = new List<Layer>();
            if (numHidden > 0)
            {
                layers.Add(new Layer(numNeuronPerHidden, numInputs));
                for (int i = 0; i < numHidden - 1; i++)
                {
                    layers.Add(new Layer(numNeuronPerHidden, numNeuronPerHidden));
                }
                layers.Add(new Layer(numOutputs, numNeuronPerHidden));
            }
            else
            {
                layers.Add(new Layer(numOutputs, numInputs));
            }
        }

        public List<double> Train(List<double> inputValues, List<double> desiredOutput)
        {
            List<double> outputValues = new List<double>();
            outputValues = CalcOutput(inputValues, desiredOutput);
            UpdateWeights(outputValues, desiredOutput);
            return outputValues;
        }

        public List<double> CalcOutput(List<double> inputValues, List<double> desiredOutput = null)
        {
            List<double> inputs = new List<double>();
            List<double> outputs = new List<double>();
            int currentInput = 0;

            if (inputValues.Count != numInputs)
            {
                throw new Exception("Number of inputs must be " + numInputs);
            }

            inputs = new List<double>(inputValues);

            for (int i = 0; i < numHidden + 1; i++)
            {
                if (i > 0)
                {
                    inputs = new List<double>(outputs);
                }
                outputs.Clear();

                for (int j = 0; j < layers[i].numNeurons; j++)
                {
                    double n = 0;
                    layers[i].neurons[j].inputs.Clear();

                    for (int k = 0; k < layers[i].neurons[j].numInputs; k++)
                    {
                        layers[i].neurons[j].inputs.Add(inputs[currentInput]);
                        n += layers[i].neurons[j].weights[k] * inputs[currentInput];
                        currentInput++;
                    }
                    n -= layers[i].neurons[j].bias;

                    layers[i].neurons[j].output = ActivationFunction(n, i == numHidden);

                    outputs.Add(layers[i].neurons[j].output);
                    currentInput = 0;
                }
            }

            return outputs;
        }

        void UpdateWeights(List<double> outputs, List<double> desiredOutputs)
        {
            double error;

            for (int i = numHidden; i >= 0; i--)
            {
                for (int j = 0; j < layers[i].numNeurons; j++)
                {
                    if (i == numHidden)
                    {
                        error = desiredOutputs[j] - outputs[j];
                        layers[i].neurons[j].errorGradient = outputs[j] * (1 - outputs[j]) * error;
                    }
                    else
                    {
                        layers[i].neurons[j].errorGradient = layers[i].neurons[j].output * (1 - layers[i].neurons[j].output);
                        double errorGradientSum = 0;
                        for (int p = 0; p < layers[i + 1].numNeurons; p++)
                        {
                            errorGradientSum += layers[i + 1].neurons[p].errorGradient * layers[i + 1].neurons[p].weights[j];
                        }
                        layers[i].neurons[j].errorGradient *= errorGradientSum;
                    }
                    for (int k = 0; k < layers[i].neurons[j].numInputs; k++)
                    {
                        if (i == numHidden)
                        {
                            error = desiredOutputs[j] - outputs[j];
                            layers[i].neurons[j].weights[k] += alpha * layers[i].neurons[j].inputs[k] * error;
                        }
                        else
                        {
                            layers[i].neurons[j].weights[k] += alpha * layers[i].neurons[j].inputs[k] * layers[i].neurons[j].errorGradient;
                        }
                    }
                    layers[i].neurons[j].bias += alpha * -1 * layers[i].neurons[j].errorGradient;
                }
            }
        }

        public void PrintWeights()
        {
            string weightsString = "\n|----- Network weights -----|\n";

            int lc = 0, nc = 0, wc = 0;
            foreach (Layer l in layers)
            {
                lc++;
                weightsString += string.Format("Layer {0} :\n", lc);
                foreach (Neuron n in l.neurons)
                {
                    nc++;
                    weightsString += string.Format("     Neuron {0} [", nc);
                    foreach (double w in n.weights)
                    {
                        wc++;
                        weightsString += string.Format(wc == 1 ? "w{0}={1}" : ", w{0}={1}", wc, w);
                    }
                    weightsString += "]\n";
                    wc = 0;
                }
                nc = 0;
            }
            weightsString += "|---------------------------|\n";

            Debug.Log(weightsString);
        }

        public string SerializeWeights()
        {
            string weightsString = "";
            foreach (Layer l in layers)
            {
                foreach (Neuron n in l.neurons)
                {
                    foreach (double w in n.weights)
                    {
                        weightsString += w + ",";
                    }
                }
            }
            return weightsString;
        }

        public void LoadWeights(string weights)
        {
            if (string.IsNullOrEmpty(weights)) return;

            string[] weightsValues = weights.Split(',');
            int w = 0;

            foreach (Layer l in layers)
            {
                foreach (Neuron n in l.neurons)
                {
                    for (int i = 0; i < n.weights.Count; i++)
                    {
                        n.weights[i] = Convert.ToDouble(weightsValues[w]);
                        w++;
                    }
                }
            }
        }

        double ActivationFunction(double value, bool isOutput = false)
        {
            return ActivationFunctions.GetResult(value, isOutput ? activationFunctionOutput : activationFunctionHidden);
        }
    }
}


using System.Collections.Generic;

namespace DGTools.NeuralNetwork {
    public class Layer
    {

        //VARIABLES
        public int numNeurons;
        public List<Neuron> neurons = new List<Neuron>();

        //CONSTRUCTORS
        public Layer(int nNeurons, int nNeuronInputs)
        {
            numNeurons = nNeurons;
            for (int i = 0; i < nNeurons; i++)
            {
                neurons.Add(new Neuron(nNeuronInputs));
            }
        }
    }
}


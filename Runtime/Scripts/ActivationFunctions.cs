using System;

namespace DGTools.NeuralNetwork {
    public enum ActivationFunction { sigmoid, step, tanH, ReLU, PReLU, identity }

    public static class ActivationFunctions
    {
        public static double GetResult(double x, ActivationFunction activationFunction) {
            switch (activationFunction)
            {
                case ActivationFunction.sigmoid: return Sigmoid(x);
                case ActivationFunction.step: return Step(x);
                case ActivationFunction.tanH: return TanH(x);
                case ActivationFunction.ReLU: return Relu(x);
                case ActivationFunction.PReLU: return PReLu(x);
                case ActivationFunction.identity: return Identity(x);
            }
            return Sigmoid(x);
        }
        
        public static double Step(double x)
        {
            return x < 0 ? 0 : 1;
        }

        public static double Sigmoid(double x)
        {
            double k = (double)Math.Exp(x);
            return k / (1.0f + k);
        }

        public static double TanH(double x)
        {
            double k = (double)Math.Exp(-2 * x);
            return 2 / (1.0f + k) - 1;
        }

        public static double Relu(double x)
        {
            return x < 0 ? 0 : x;
        }

        public static double PReLu(double x)
        {
            return x < 0 ? 0.01 * x : x;
        }

        public static double Identity(double x)
        {
            return x;
        }
    }
}


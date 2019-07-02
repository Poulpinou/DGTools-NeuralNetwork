using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace DGTools.NeuralNetwork
{
    public static class NetworkExtensions
    {
        /// <summary>
        /// Normalize values between 0 and 1
        /// </summary>
        public static List<double> SoftMax(this List<double> values)
        {
            double max = values.Max();
            float scale = 0;

            for (int i = 0; i < values.Count; i++)
                scale += Mathf.Exp((float)(values[i] - max));

            List<double> result = new List<double>();
            if (scale == 0) scale = 1;
            for (int i = 0; i < values.Count; i++)
                result.Add(Mathf.Exp((float)(values[i] - max)) / scale);

            return result;
        }
    }
}

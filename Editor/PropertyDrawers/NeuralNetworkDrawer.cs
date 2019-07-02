using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using DGTools.Editor;

namespace DGTools.NeuralNetwork.Editor {

    [CustomPropertyDrawer(typeof(NeuralNetwork))]
    public class NeuralNetworkDrawer : DGToolsPropertyDrawer
    {
        //SETTINGS
        // Height of schema and curve boxes (in pixel)
        const int sb_height = 200;

        // Schema background color
        Color bg_color = Color.grey;

        // Neurons colors
        Color input_color = Color.red;
        Color hidden_color = Color.yellow;
        Color output_color = Color.green;

        // Neurons size
        const int n_size = 10;


        //GETTERS/SETTERS
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.isExpanded) {
                return EditorGUI.GetPropertyHeight(property) + sb_height;
            }
            return base.GetPropertyHeight(property, label);
        }


        //EDITOR METHODS
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.isExpanded) {
                EditorGUI.BeginProperty(position, label, property);

                //Draw background
                Rect schema = new Rect(new Vector2(position.x, position.y), new Vector2(position.size.x, sb_height));
                EditorGUI.DrawRect(schema, bg_color);
                
                //Get datas
                int nbLayers = 2 + property.FindPropertyRelative("numHidden").intValue;
                int[] neuronPerLayer = new int[nbLayers];

                for (int i = 0; i < neuronPerLayer.Length; i++)
                {
                    if (i == 0)
                    {
                        neuronPerLayer[i] = property.FindPropertyRelative("numInputs").intValue;new Rect(new Vector2(position.x, position.y), new Vector2(position.size.x, sb_height));
                    }
                    else if (i == nbLayers - 1)
                    {
                        neuronPerLayer[i] = property.FindPropertyRelative("numOutputs").intValue;
                    }
                    else {
                        neuronPerLayer[i] = property.FindPropertyRelative("numNeuronPerHidden").intValue;
                    }
                }

                //Compute column infos
                float layerWidth = position.size.x / neuronPerLayer.Length;

                //Draw Network
                List<Vector2> lastPos = null;
                for (int i = 0; i < neuronPerLayer.Length; i++) {
                    Rect layer = new Rect(
                        new Vector2(i * layerWidth + schema.position.x, schema.yMin),
                        new Vector2(layerWidth, schema.size.y)
                    );

                    if (i == 0)
                    {
                        lastPos = DrawLayer(layer, "Inputs", neuronPerLayer[i], input_color);
                    }
                    else if (i == nbLayers - 1)
                    {
                        lastPos = DrawLayer(layer, "Outputs", neuronPerLayer[i], output_color, lastPos);
                    }
                    else
                    {
                        lastPos = DrawLayer(layer, "Hidden " + i, neuronPerLayer[i], hidden_color, lastPos);
                    }
                }
                Handles.DrawLine(new Vector2(position.xMin, position.y + 20), new Vector2(position.xMax, position.y + 20));
         
                position.y += schema.size.y;
                EditorGUI.EndProperty();
            }
            
            //Draw default drawer
            EditorGUI.PropertyField(position, property, new GUIContent("Network Properties"), true);
        }


        List<Vector2> DrawLayer(Rect layer, string title, int numElements, Color color, List<Vector2> lastPos = null) {
            List<Vector2> neuronsPos = new List<Vector2>();

            //Draw label
            GUIStyle style = skin.GetStyle("Label");
            style.alignment = TextAnchor.UpperCenter;

            GUI.Label(new Rect(layer.x, layer.yMin, layer.width, 20), title, style);

            if (numElements <= 0) return null;

            // Compute layer's infos
            float neuronOffsetY = (layer.height - 20) / numElements;
            float offset = layer.yMin + 10 + neuronOffsetY / 2;

            for (int i = 0; i < numElements; i++)
            {
                Rect neuronRect = new Rect(new Vector2(layer.center.x, offset), new Vector2(n_size, n_size));

                //Draw Neuron
                EditorGUI.DrawRect(neuronRect, color);

                if (lastPos != null) {
                    //Draw lines
                    foreach (Vector2 pos in lastPos)
                    {
                        Handles.DrawLine(new Vector2(neuronRect.xMin, neuronRect.center.y), pos);
                    }
                }

                offset += neuronOffsetY;

                // Store last pos
                neuronsPos.Add(new Vector2(neuronRect.xMax, neuronRect.center.y));
            }

            return neuronsPos;
        }
        

        public override bool CanCacheInspectorGUI(SerializedProperty property)
        {
            return true;
        }
    }

}

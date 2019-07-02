using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using Newtonsoft.Json.Linq;

namespace DGTools.NeuralNetwork
{
    [Serializable]
    public class Memory
    {
        //VARIABLES
        [Header("Save Settings")]
        [FolderPath] public string savePath;
        public string fileName = "memory";
        public bool loadOnStart = false;
        public KeyCode saveKey = KeyCode.S;
        public KeyCode loadKey = KeyCode.L;

        [Header("Experience Settings")]
        public int experienceCapacity = 1000;

        [HideInInspector] public List<Experience> experiences = new List<Experience>();

        //METHODS
        public void Save(NeuralNetwork activeNetwork) {
            string path = (Application.isEditor ? Application.dataPath : Application.persistentDataPath) + savePath;
            if (!Directory.Exists(path))
                throw new DirectoryNotFoundException(path);

            JObject datas = new JObject();
            datas.Add("layers", SerializeLayers(activeNetwork.layers));
            datas.Add("experiences", SerializeExperiences());

            File.WriteAllText(path + "/" + fileName + ".json", datas.ToString());

            Debug.Log(fileName + ".json saved at " + path);
        }

        public void Load(NeuralNetwork activeNetwork) {
            string path = (Application.isEditor ? Application.dataPath : Application.persistentDataPath) + savePath + "/" + fileName + ".json";
            if (!File.Exists(path))
                throw new FileNotFoundException(path);

            JObject datas = JObject.Parse(File.ReadAllText(path));
            LoadLayers(ref activeNetwork, (JArray)datas.SelectToken("layers"));
            LoadExperiencies((JArray)datas.SelectToken("experiences"));

            Debug.Log("Network datas sucessfully loaded!");
        }

        public void AddExperience(Experience experience) {
            if (experiences.Count >= experienceCapacity)
                experiences.RemoveAt(0);
            experiences.Add(experience);
        }

        JArray SerializeLayers(List<Layer> layers)
        {
            JArray layersDatas = new JArray();
            foreach (Layer l in layers)
            {
                JObject layerDatas = new JObject();
                JArray neuronsDatas = new JArray();
                foreach (Neuron n in l.neurons)
                {
                    JObject neuronDatas = new JObject();
                    JArray weightsDatas = new JArray();
                    foreach (double w in n.weights)
                    {
                        weightsDatas.Add(w);
                    }
                    neuronDatas.Add("weights", weightsDatas);
                    neuronsDatas.Add(neuronDatas);
                }
                layerDatas.Add("neurons", neuronsDatas);
                layersDatas.Add(layerDatas);
            }

            return layersDatas;
        }

        JArray SerializeExperiences() {
            JArray experiencesDatas = new JArray();
            foreach (Experience e in experiences) {
                JObject experienceDatas = new JObject();
                experienceDatas.Add("reward", e.reward);

                JArray statesDatas = new JArray();
                foreach (double s in e.states) {
                    statesDatas.Add(s);
                }
                experienceDatas.Add("states", statesDatas);
                experiencesDatas.Add(experienceDatas);
            }
            return experiencesDatas;
        }
        
        void LoadLayers(ref NeuralNetwork network, JArray datas)
        {
            List<Layer> layers = network.layers;
            for (int l = 0; l < layers.Count; l++) {
                JArray neuronsDatas = (JArray)datas[l].SelectToken("neurons");
                for (int n = 0; n < neuronsDatas.Count; n++) {
                    JArray weightsDatas = (JArray)neuronsDatas[n].SelectToken("weights");
                    for (int w = 0; w < weightsDatas.Count; w++)
                    {
                        layers[l].neurons[n].weights[w] = (double)weightsDatas[w];
                    }
                }
            }

            network.layers = layers;
        }

        void LoadExperiencies(JArray datas) {
            experiences = new List<Experience>();
            foreach (JObject experienceDatas in datas) {
                Experience experience = new Experience();
                experience.reward = (double)experienceDatas.SelectToken("reward");
                experience.states = new List<double>();

                foreach (JToken state in (JArray)experienceDatas.SelectToken("states")) {
                    experience.states.Add((double)state);
                }

                experiences.Add(experience);
            }
        }
    }

    public struct Experience {
        //VARIABLES
        public List<double> states;
        public double reward;

        //CONSTRUCTORS
        public Experience(double reward, params double[] states)
        {
            this.reward = reward;
            this.states = new List<double>();
            foreach (double state in states)
            {
                this.states.Add(state);
            }
        }

        public Experience(double reward, List<double> states) : this(reward, states.ToArray()) { }
    }
}
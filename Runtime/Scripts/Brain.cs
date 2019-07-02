using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace DGTools.NeuralNetwork {
    public abstract class Brain : MonoBehaviour
    {
        //VARIABLES
        public NeuralNetwork network;
        public Memory memory;

        [Header("Train Settings")]
        public bool canTrain = true;

        [Header("Debug")]
        public bool showDebug;

        [Header("Controls")]
        public KeyCode resetKey = KeyCode.R;

        protected bool isValid = false;
        double runTime;
        double maxRunTime;

        //METHODS
        void CheckBrainValidity() {
            List<string> errors = CheckRules();

            isValid = errors.Count == 0;

            if (!isValid) {
                string errorToDisplay = "<b>Invalid brain!</b> \n<b>"+ errors.Count +" errors : </b>\n";
                for (int i = 0; i < errors.Count; i++)
                {
                    errorToDisplay += "   <b>" + (i + 1) + ")</b> " + errors[i] + "\n";
                }
                throw new Exception(errorToDisplay);
            }
        }

        /// <summary>
        /// Ovrride this method to check brain validity
        /// </summary>
        /// <returns>Returns a list of error (if count = 0 brain is valid)</returns>
        protected virtual List<string> CheckRules() {
            List<string> errors = new List<string>();

            return errors;
        }

        /// <summary>
        /// All the brain job is done here and is called every frame
        /// </summary>
        protected virtual void Run() {
            throw new NotImplementedException();
        }

        /// <summary>
        /// All the training job is done here and is called if CheckIfShouldTrain() == true
        /// </summary>
        protected virtual void Train() {
            throw new NotImplementedException();

            PostTraining();
        }

        /// <summary>
        /// Called at the end of the training method
        /// </summary>
        protected virtual void PostTraining() {

        }

        /// <summary>
        /// Draw a debug on GUI
        /// </summary>
        /// <returns>Returns the last block Rect to help you to place your debug</returns>
        protected virtual Rect DrawDebug() {
            Rect block = new Rect(10, 10, 600, 80);
            GUI.BeginGroup(block);
            GUI.Label(new Rect(0, 0, block.width, 20), "Stats");
            GUI.Label(new Rect(10, 20, block.width - 10, 20), string.Format("Run Time : {0}ms", runTime));
            GUI.Label(new Rect(10, 40, block.width - 10, 20), string.Format("Max Run time : {0}ms", maxRunTime));
            GUI.Label(new Rect(10, 60, block.width - 10, 20), string.Format("Memory Capacity : {0} / {1}", memory.experiences.Count, memory.experienceCapacity));
            GUI.EndGroup();

            return block;
        }

        void CheckMemoryActions() {
            if (Input.GetKeyDown(memory.saveKey)) memory.Save(network);
            if (Input.GetKeyDown(memory.loadKey)) memory.Load(network);
        }

        //ABSTRACT METHODS
        /// <summary>
        /// Do all you need in this method to come back to default state
        /// </summary>
        public abstract void ResetToDefault();

        /// <summary>
        /// Check here if your brain should train (called every frame)
        /// </summary>
        /// <returns>True if should train</returns>
        protected abstract bool CheckIfShouldTrain();

        //RUNTIME METHODS
        protected virtual void Start()
        {
            network.Init();
            if (memory.loadOnStart) memory.Load(network);
            CheckBrainValidity();
        }

        protected virtual void Update()
        {
            if (Input.GetKeyDown(resetKey)) ResetToDefault();
            CheckMemoryActions();
        }

        protected virtual void FixedUpdate()
        {
            DateTime startTime = DateTime.Now;
            if(isValid) Run();
            runTime = DateTime.Now.Subtract(startTime).TotalMilliseconds;

            if (runTime > maxRunTime)
                maxRunTime = runTime;
        }

        private void OnGUI()
        {
            if (showDebug) DrawDebug();
        }
    }
}


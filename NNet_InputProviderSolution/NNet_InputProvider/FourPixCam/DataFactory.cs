﻿using MatrixHelper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NNet_InputProvider.FourPixCam
{
    // In ..?:
    public enum Label
    {
        Undefined, AllBlack, AllWhite, LeftBlack, LeftWhite, SlashBlack, SlashWhite, TopBlack, TopWhite
    }

    public class DataFactory : BaseDataFactory
    {
        #region ctor & fields

        const SetName name = SetName.FourPixelCamera;

        Random rnd;
        Dictionary<Label, Matrix> rawInputs;
        Dictionary<Label, Matrix> distortedInputs;
        Dictionary<Label, Matrix> validInputs;
        Dictionary<Label, Matrix> validOutputs;
        Sample[] validSamples;

        public DataFactory() : base(name) { }

        #endregion

        #region BaseDataFactory

        protected override Sample[] CreateSamples(int samples, float inputDistortion, float targetTolerance)
        {
            rnd = RandomProvider.GetThreadRandom();
            rawInputs = GetRawInputs();

            distortedInputs = GetDistortedInputs(inputDistortion);
            validInputs = GetValidInputs(distortedInputs);
            validOutputs = GetValidOutputs();
            Sample.Tolerance = targetTolerance;
            validSamples = GetValidSamples();
            return GetValidTrainingData(samples, validSamples);
        }
        protected override Sample[] GetSamplesFromStream(FileStream fs_labels, FileStream fs_imgs)
        {
            throw new NotImplementedException();
        }

        #region helpers

        Dictionary<Label, Matrix> GetRawInputs()
        {
            return new Dictionary<Label, Matrix>
            {
                [Label.AllBlack] = new Matrix(new float[,] {
                    { -1, -1 },
                    { -1, -1 } }),

                [Label.AllWhite] = new Matrix(new float[,] {
                    { 1, 1 },
                    { 1, 1 } }),

                [Label.TopBlack] = new Matrix(new float[,] {
                    { -1, -1 },
                    { 1, 1 } }),

                [Label.TopWhite] = new Matrix(new float[,] {
                    { 1, 1 },
                    { -1, -1 } }),

                [Label.LeftBlack] = new Matrix(new float[,] {
                    { -1, 1 },
                    { -1, 1 } }),

                [Label.LeftWhite] = new Matrix(new float[,] {
                    { 1, -1 },
                    { 1, -1 } }),

                [Label.SlashBlack] = new Matrix(new float[,] {
                    { 1, -1 },
                    { -1, 1 } }),

                [Label.SlashWhite] = new Matrix(new float[,] {
                    { -1, 1 },
                    { 1, -1 } })
            };
        }
        Sample[] GetValidTrainingData(int sampleSize, Sample[] _validSamples)
        {
            List<Sample> tmpResult = new List<Sample>();
            int amountOfCompleteSampleSets = (int)Math.Round((double)sampleSize / rawInputs.Values.Count, 0);

            for (int i = 0; i < amountOfCompleteSampleSets; i++)
            {
                tmpResult.AddRange(_validSamples);
            }
            Sample[] result = tmpResult.Shuffle().ToArray();

            return result;
        }
        Sample[] GetValidSamples()
        {
            var result = new List<Sample>();

            var labels = Enum.GetValues(typeof(Label)).ToList<Label>().Skip(1);
            foreach (var label in labels)
            {
                result.Add(new Sample
                {
                    Label = label,
                    RawInput = rawInputs[label],
                    Input = validInputs[label],
                    ExpectedOutput = validOutputs[label]
                });
            }

            return result.ToArray();
        }
        Dictionary<Label, Matrix> GetDistortedInputs(float d)
        {
            return new Dictionary<Label, Matrix>
            {
                [Label.AllBlack] = new Matrix(new float[,] {
                    { -(GetDistortedValue(d)), -(GetDistortedValue(d)) },
                    { -(GetDistortedValue(d)), -(GetDistortedValue(d)) } }),

                [Label.AllWhite] = new Matrix(new float[,] {
                    { (GetDistortedValue(d)), (GetDistortedValue(d)) },
                    { (GetDistortedValue(d)), (GetDistortedValue(d)) } }),

                [Label.TopBlack] = new Matrix(new float[,] {
                    { -(GetDistortedValue(d)), -(GetDistortedValue(d)) },
                    { (GetDistortedValue(d)), (GetDistortedValue(d)) } }),

                [Label.TopWhite] = new Matrix(new float[,] {
                    { (GetDistortedValue(d)), (GetDistortedValue(d)) },
                    { -(GetDistortedValue(d)), -(GetDistortedValue(d)) } }),

                [Label.LeftBlack] = new Matrix(new float[,] {
                    { -(GetDistortedValue(d)), (GetDistortedValue(d)) },
                    { -(GetDistortedValue(d)), (GetDistortedValue(d)) } }),

                [Label.LeftWhite] = new Matrix(new float[,] {
                    { (GetDistortedValue(d)), -(GetDistortedValue(d)) },
                    { (GetDistortedValue(d)), -(GetDistortedValue(d)) } }),

                [Label.SlashBlack] = new Matrix(new float[,] {
                    { (GetDistortedValue(d)), -(GetDistortedValue(d)) },
                    { -(GetDistortedValue(d)), (GetDistortedValue(d)) } }),

                [Label.SlashWhite] = new Matrix(new float[,] {
                    { -(GetDistortedValue(d)), (GetDistortedValue(d)) },
                    { (GetDistortedValue(d)), -(GetDistortedValue(d)) } })
            };
        }
        float GetDistortedValue(float distortionDeviation)
        {
            return 1f - (float)rnd.NextDouble() * distortionDeviation;
        }
        Dictionary<Label, Matrix> GetValidInputs(Dictionary<Label, Matrix> _rawInputs)
        {
            var test = _rawInputs.ToDictionary(x => x.Key, x => Operations.FlattenToOneColumn(x.Value));
            return test;
        }
        Dictionary<Label, Matrix> GetValidOutputs()
        {
            return new Dictionary<Label, Matrix>
            {
                [Label.AllWhite] = new Matrix(new float[] { 1, 0, 0, 0 }),

                [Label.AllBlack] = new Matrix(new float[] { 1, 0, 0, 0 }),

                [Label.TopWhite] = new Matrix(new float[] { 0, 1, 0, 0 }),

                [Label.TopBlack] = new Matrix(new float[] { 0, 1, 0, 0 }),

                [Label.LeftWhite] = new Matrix(new float[] { 0, 0, 1, 0 }),

                [Label.LeftBlack] = new Matrix(new float[] { 0, 0, 1, 0 }),

                [Label.SlashWhite] = new Matrix(new float[] { 0, 0, 0, 1 }),

                [Label.SlashBlack] = new Matrix(new float[] { 0, 0, 0, 1 })
            };
        }

        #endregion

        #endregion
    }
}

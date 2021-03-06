﻿using System;
using System.IO;

namespace DeepLearningDataProvider.Builders
{
    public class PathBuilder
    {
        #region fields

        private readonly Action<string> _onDataProviderChanged;
        private string sampleSetParameters, sampleSet;

        public PathBuilder(Action<string> onDataProviderChanged)
        {
            _onDataProviderChanged = onDataProviderChanged;
        }

        #endregion

        #region properties

        public string FileName_SampleSet { get; set; } = "SampleSet";
        public string FileName_SampleSetParameters { get; set; } = "SampleSetParameters";
        public string FileName_Prefix { get; set; } = string.Empty;
        public string FileName_Suffix { get; set; } = ".txt";

        public string General { get; set; } = Path.GetTempPath();
        public string SampleSetParameters
        {
            get
            {
                if (string.IsNullOrEmpty(sampleSetParameters))
                    return sampleSetParameters = Path.Combine(General, FileName_Prefix, FileName_SampleSetParameters + FileName_Suffix);
                else return sampleSetParameters;
            }
            set { sampleSetParameters = value; }
        }
        public string SampleSet
        {
            get
            {
                if (string.IsNullOrEmpty(sampleSet))
                    return sampleSet = Path.Combine(General, FileName_Prefix, FileName_SampleSet + FileName_Suffix);
                else return sampleSet;
            }
            set { sampleSet = value; }
        }

        #endregion

        #region methods

        public bool SetGeneralPath(string path)
        {
            if (!Directory.Exists(path))
            {
                _onDataProviderChanged("Path not found!");
                return false;
            }

            General = path;
            _onDataProviderChanged("General path is set.");
            UseGeneralPathAndDefaultNames();    // no default names here?
            return true;
        }
        public void SetFileNamePrefix(string prefix)
        {
            FileName_Prefix = prefix;
            _onDataProviderChanged($"The file name has prefix {prefix} now.");
        }
        public void SetFileNameSuffix(string suffix)
        {
            FileName_Suffix = suffix;
            _onDataProviderChanged($"The file name has suffix {suffix} now.");
        }
        public void ResetPaths()
        {
            General = Path.GetTempPath();

            sampleSetParameters = string.Empty;
            sampleSet = string.Empty;

            _onDataProviderChanged($"Path for all files has been reset.");
        }
        public void UseGeneralPathAndDefaultNames()
        {
            SetSampleSetParametersPath(Path.Combine(General, FileName_Prefix, FileName_SampleSetParameters + FileName_Suffix));
            SetSampleSetPath(Path.Combine(General, FileName_Prefix, FileName_SampleSet + FileName_Suffix));
        }

        #region redundant?

        public void SetSampleSetPath(string path)
        {
            SampleSet = path;
            _onDataProviderChanged("Path to the sample set has been set.");
        }
        public void SetSampleSetParametersPath(string path)
        {
            SampleSetParameters = path;
            _onDataProviderChanged("Path to parameters for the sample set has been set.");
        }

        #endregion

        #endregion
    }
}

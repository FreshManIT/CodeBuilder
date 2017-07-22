using System;
using System.Collections.Generic;

// ReSharper disable once CheckNamespace
namespace CodeBuilder.WinForm.UI
{
    using PhysicalDataModel;
    using Configuration;

    public class GenerationParameter
    {
        public GenerationParameter(Dictionary<string, Model> models, Dictionary<string, List<String>> generationObjects, GenerationSettings settings)
        {
            Models = models;
            GenerationObjects = generationObjects;
            Settings = settings;
        }

        public Dictionary<string, Model> Models { get; }

        public Dictionary<string, List<String>> GenerationObjects { get; }

        public GenerationSettings Settings { get; }
    }
}

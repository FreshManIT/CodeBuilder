using System.ComponentModel;

// ReSharper disable once CheckNamespace
namespace CodeBuilder.WinForm.UI
{
    public delegate void GenerationProgressChangedEventHandler(GenerationProgressChangedEventArgs args);

    public class GenerationProgressChangedEventArgs : ProgressChangedEventArgs
    {
        public GenerationProgressChangedEventArgs(int generatedCount, int errorCount, string currentFileName,
            int progressPercentage, object userToken)
            : base(progressPercentage, userToken)
        {
            GeneratedCount = generatedCount;
            ErrorCount = errorCount;
            CurrentFileName = currentFileName;
        }

        public int GeneratedCount { get; }

        public int ErrorCount { get; }

        public string CurrentFileName { get; }
    }
}

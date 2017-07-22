using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading;
using CodeBuilder.Framework.Configuration;
using CodeBuilder.PhysicalDataModel;
using CodeBuilder.TemplateEngine;
using CodeBuilder.Util;
using CodeBuilder.WinForm.Properties;

// ReSharper disable once CheckNamespace
namespace CodeBuilder.WinForm.UI
{
    public sealed partial class CodeGeneration : Component
    {
        /// <summary>
        /// Write logger
        /// </summary>
        private static readonly Logger Logger = InternalTrace.GetLogger(typeof(CodeGeneration));

        /// <summary>
        /// Do auto code delegate.
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="asyncOp"></param>
        private delegate void WorkerEventHandler(GenerationParameter parameter, AsyncOperation asyncOp);

        /// <summary>
        /// Update progress delegate
        /// </summary>
        private SendOrPostCallback _onProgressReportDelegate;

        /// <summary>
        /// Completed delegate.
        /// </summary>
        private SendOrPostCallback _onCompletedDelegate;

        private readonly HybridDictionary _userStateToLifetime = new HybridDictionary();

        /// <summary>
        /// Construct function
        /// </summary>
        public CodeGeneration()
        {
            InitializeComponent();
            InitializeDelegates();
        }

        /// <summary>
        /// Construct function
        /// </summary>
        /// <param name="container"></param>
        public CodeGeneration(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
            InitializeDelegates();
        }

        private void InitializeDelegates()
        {
            _onProgressReportDelegate = ReportProgress;
            _onCompletedDelegate = GenerateCompleted;
        }

        /// <summary>
        /// progress change event
        /// </summary>
        public event GenerationProgressChangedEventHandler ProgressChanged;

        /// <summary>
        /// Completed event
        /// </summary>
        public event GenerationCompletedEventHandler Completed;

        public void GenerateAsync(GenerationParameter parameter, object taskId)
        {
            AsyncOperation asyncOp = AsyncOperationManager.CreateOperation(taskId);
            lock (_userStateToLifetime.SyncRoot)
            {
                if (_userStateToLifetime.Contains(taskId))
                    throw new ArgumentException(Resources.TaskId, nameof(taskId));

                _userStateToLifetime[taskId] = asyncOp;
            }

            WorkerEventHandler workerDelegate = GenerateWorker;
            workerDelegate.BeginInvoke(parameter, asyncOp, null, null);
        }

        public void CancelAsync(object taskId)
        {
            // ReSharper disable once InconsistentlySynchronizedField
            AsyncOperation asyncOp = _userStateToLifetime?[taskId] as AsyncOperation;
            if (asyncOp != null)
            {
                lock (_userStateToLifetime.SyncRoot)
                {
                    _userStateToLifetime.Remove(taskId);
                }
            }
        }

        /// <summary>
        /// Async operation work
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="asyncOp"></param>
        private void GenerateWorker(GenerationParameter parameter, AsyncOperation asyncOp)
        {
            if (IsTaskCanceled(asyncOp.UserSuppliedState)) return;

            int genratedCount = 0;
            int errorCount = 0;
            int progressCount = 0;

            try
            {
                string adapterTypeName = ConfigManager.SettingsSection.TemplateEngines[parameter.Settings.TemplateEngine].Adapter;
                if (adapterTypeName != null)
                {
                    var type = Type.GetType(adapterTypeName);
                    if (type == null) return;
                    ITemplateEngine templateEngine = (ITemplateEngine)Activator.CreateInstance(type);

                    foreach (string templateName in parameter.Settings.TemplatesNames)
                    {
                        GenerateCode(parameter, templateEngine, templateName, ref genratedCount, ref errorCount, ref progressCount, asyncOp);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("", ex);
            }

            CompletionMethod(null, IsTaskCanceled(asyncOp.UserSuppliedState), asyncOp);
        }

        /// <summary>
        /// Realy to auto code
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="templateEngine"></param>
        /// <param name="templateName"></param>
        /// <param name="genratedCount"></param>
        /// <param name="errorCount"></param>
        /// <param name="progressCount"></param>
        /// <param name="asyncOp"></param>
        private void GenerateCode(GenerationParameter parameter, ITemplateEngine templateEngine, string templateName,
            ref int genratedCount, ref int errorCount, ref int progressCount, AsyncOperation asyncOp)
        {
            foreach (string modelId in parameter.GenerationObjects.Keys)
            {
                string codeFileNamePath = PathHelper.GetCodeFileNamePath(ConfigManager.GenerationCodeOuputPath,
                    ConfigManager.SettingsSection.Languages[parameter.Settings.Language].Alias,
                    ConfigManager.SettingsSection.TemplateEngines[parameter.Settings.TemplateEngine].Name,
                    ConfigManager.TemplateSection.Templates[templateName].DisplayName, parameter.Settings.Package, modelId);
                PathHelper.CreateCodeFileNamePath(codeFileNamePath);

                foreach (string objId in parameter.GenerationObjects[modelId])
                {
                    IMetaData modelObject = ModelManager.GetModelObject(parameter.Models[modelId], objId);
                    TemplateData templateData = TemplateDataBuilder.Build(modelObject, parameter.Settings,
                            templateName, parameter.Models[modelId].Database, modelId);

                    if (templateData == null || !templateEngine.Run(templateData)) errorCount++; else genratedCount++;
                    string currentCodeFileName = templateData == null ? string.Empty : templateData.CodeFileName;

                    var args = new GenerationProgressChangedEventArgs(genratedCount,
                            errorCount, currentCodeFileName, ++progressCount, asyncOp.UserSuppliedState);
                    asyncOp.Post(this._onProgressReportDelegate, args);
                }
            }
        }

        private void GenerateCompleted(object operationState)
        {
            GenerationCompletedEventArgs e = operationState as GenerationCompletedEventArgs;
            OnCompleted(e);
        }

        /// <summary>
        /// ReportProgress
        /// </summary>
        /// <param name="state"></param>
        private void ReportProgress(object state)
        {
            GenerationProgressChangedEventArgs e = state as GenerationProgressChangedEventArgs;
            OnProgressChanged(e);
        }

        /// <summary>
        /// Completed auto code.
        /// </summary>
        /// <param name="e"></param>
        private void OnCompleted(GenerationCompletedEventArgs e)
        {
            Completed?.Invoke(this, e);
        }

        /// <summary>
        /// change progress.
        /// </summary>
        /// <param name="e"></param>
        private void OnProgressChanged(GenerationProgressChangedEventArgs e)
        {
            ProgressChanged?.Invoke(e);
        }

        /// <summary>
        /// Completion method.
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="canceled"></param>
        /// <param name="asyncOp"></param>
        private void CompletionMethod(Exception exception, bool canceled, AsyncOperation asyncOp)
        {
            if (!canceled)
            {
                lock (_userStateToLifetime.SyncRoot)
                {
                    _userStateToLifetime.Remove(asyncOp.UserSuppliedState);
                }
            }

            GenerationCompletedEventArgs e = new GenerationCompletedEventArgs(
                exception,
                canceled,
                asyncOp.UserSuppliedState);

            asyncOp.PostOperationCompleted(_onCompletedDelegate, e);
        }

        /// <summary>
        /// is canceled task.
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        private bool IsTaskCanceled(object taskId)
        {
            lock (_userStateToLifetime.SyncRoot)
            {
                return (_userStateToLifetime[taskId] == null);
            }
        }
    }
}

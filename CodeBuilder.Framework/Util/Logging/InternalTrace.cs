using System;

namespace CodeBuilder.Util
{
    public enum InternalTraceLevel
    {
        Default,
        Off,
        Error,
        Warning,
        Info,
        Verbose
    }
    
	public class InternalTrace
	{
        /// <summary>
        /// had initialized
        /// </summary>
		private static bool _initialized;

        private static InternalTraceWriter _writer;

        /// <summary>
        /// Write log
        /// </summary>
        public static InternalTraceWriter Writer => _writer;

	    public static InternalTraceLevel Level;
        public static void Initialize(string logName)
        {
            int lev = (int) new System.Diagnostics.TraceSwitch("Trace", "CodeBuilder internal trace").Level;
            Initialize(logName, (InternalTraceLevel)lev);
        }

        public static void Initialize(string logName, InternalTraceLevel level)
        {
			if (!_initialized)
			{
				Level = level;

				if (_writer == null && Level > InternalTraceLevel.Off)
				{
					_writer = new InternalTraceWriter(logName);
					_writer.WriteLine("InternalTrace: Initializing at level " + Level);
				}

				_initialized = true;
			}
        }

        public static void ReInitialize(string logName, InternalTraceLevel level)
        {
            if (_initialized){ 
                Close();
                _initialized=false; 
            }

            Initialize(logName, level);
        }

        public static void Flush()
        {
            _writer?.Flush();
        }

	    public static void Close()
        {
	        _writer?.Close();

	        _writer = null;
        }

        public static Logger GetLogger(string name)
		{
			return new Logger( name );
		}

		public static Logger GetLogger( Type type )
		{
			return new Logger( type.FullName );
		}

        public static void Log(InternalTraceLevel level, string message, string category)
        {
            Log(level, message, category, null);
        }

        public static void Log(InternalTraceLevel level, string message, string category, Exception ex)
        {
            Writer.WriteLine("{0} {1,-5} [{2,2}] {3}: {4}",
                DateTime.Now,
                level == InternalTraceLevel.Verbose ? "Debug" : level.ToString(),
                System.Threading.Thread.CurrentThread.ManagedThreadId,
                category,
                message);

            if (ex != null)
                Writer.WriteLine(ex.ToString());
        }
    }
}

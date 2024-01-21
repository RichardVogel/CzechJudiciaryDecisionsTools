﻿using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CzechJudiciaryDecisionsTools.Common.Services
{
    // Customized ILoggerProvider, writes logs to text files
    public class CustomFileLoggerProvider(StreamWriter logFileWriter) : ILoggerProvider
    {
        private readonly StreamWriter _logFileWriter = logFileWriter ?? throw new ArgumentNullException(nameof(logFileWriter));

        public ILogger CreateLogger(string categoryName)
        {
            return new CustomFileLogger(categoryName, _logFileWriter);
        }

        public void Dispose()
        {
            _logFileWriter.Dispose();
        }
    }

    // Customized ILogger, writes logs to text files
    public class CustomFileLogger(string categoryName, StreamWriter logFileWriter) : ILogger
    {
        private readonly string _categoryName = categoryName;
        private readonly StreamWriter _logFileWriter = logFileWriter;

        public IDisposable? BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            // Ensure that only information level and higher logs are recorded
            return logLevel >= LogLevel.Information;
        }

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception, string> formatter)
        {
            // Ensure that only information level and higher logs are recorded
            if (!IsEnabled(logLevel))
            {
                return;
            }

            // Get the formatted log message
            var message = formatter(state, exception ?? new Exception("Unknow exception."));

            //Write log messages to text file
            _logFileWriter.WriteLine($"[{logLevel}] [{_categoryName}] {message}");
            _logFileWriter.Flush();
        }
    }
}

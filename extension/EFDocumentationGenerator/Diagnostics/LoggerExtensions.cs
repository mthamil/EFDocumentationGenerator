//  Entity Designer Documentation Generator
//  Copyright 2017 Matthew Hamilton - matthamilton@live.com
// 
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
// 
//      http://www.apache.org/licenses/LICENSE-2.0
// 
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.

using System;

namespace DocumentationGenerator.Diagnostics
{
    /// <summary>
    /// Provides extensions to <see cref="ILogger"/>.
    /// </summary>
    public static class LoggerExtensions
    {
        /// <summary>
        /// Prepends a timestamp to the given log message.
        /// </summary>
        public static void LogTimestamped(this ILogger logger, string message, params object[] arguments)
        {
            logger.Log($"{DateTime.Now:yyyy-MM-dd HH:mm:ss:ffff}: {message}", arguments);
        }
    }
}


﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CppCheckSensor.cs" company="Copyright © 2013 Tekla Corporation. Tekla is a Trimble Company">
//     Copyright (C) 2013 [Jorge Costa, Jorge.Costa@tekla.com]
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License
// as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty
// of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details. 
// You should have received a copy of the GNU Lesser General Public License along with this program; if not, write to the Free
// Software Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
// --------------------------------------------------------------------------------------------------------------------

namespace CxxPlugin.LocalExtensions
{
    using System.Collections.Generic;
    using System.Linq;

    using ExtensionHelpers;

    using ExtensionTypes;

    using RestSharp;
    using RestSharp.Deserializers;

    /// <summary>
    /// The cpp check sensor.
    /// </summary>
    public class CppCheckSensor : ASensor
    {
        /// <summary>
        /// The repository key.
        /// </summary>
        public const string SKey = "cppcheck";

        /// <summary>
        /// Initializes a new instance of the <see cref="CppCheckSensor"/> class.
        /// </summary>
        /// <param name="ctrl">
        /// The ctrl.
        /// </param>
        /// <param name="options">
        /// The options.
        /// </param>
        public CppCheckSensor(ICommandExecution ctrl, Dictionary<string, string> options)
            : base(SKey, ctrl, true)
        {
            this.Command = options["CppCheckExecutable"];
            this.Args = options["CppCheckArguments"];
            this.Environment = VsSonarUtils.GetEnvironmentFromString(options["CppCheckEnvironment"]);
        }

        /// <summary>
        /// The get violations.
        /// </summary>
        /// <param name="lines">
        /// The lines.
        /// </param>
        /// <returns>
        /// The VSSonarPlugin.SonarInterface.ResponseMappings.Violations.ViolationsResponse.
        /// </returns>
        public override List<Issue> GetViolations(List<string> lines)
        {
            var violations = new List<Issue>();

            if (lines == null || lines.Count == 0)
            {
                return violations;
            }

            var xml = new XmlDeserializer();
            var output = xml.Deserialize<Results>(new RestResponse { Content = string.Join("\r\n", lines) });

            violations.AddRange(from result in output.Errors select new Issue { Line = result.Line, Message = result.Msg, Rule = this.RepositoryKey + "." + result.Id, Component = result.File });

            return violations;
        }

        /// <summary>
        /// The cpp check xml.
        /// </summary>
        internal class Results
        {
            /// <summary>
            /// Gets or sets the errors.
            /// </summary>
            public List<Error> Errors { get; set; }
        }

        /// <summary>
        /// The error.
        /// </summary>
        internal class Error
        {
            /// <summary>
            /// Gets or sets the file.
            /// </summary>
            public string File { get; set; }

            /// <summary>
            /// Gets or sets the line.
            /// </summary>
            public int Line { get; set; }

            /// <summary>
            /// Gets or sets the id.
            /// </summary>
            public string Id { get; set; }

            /// <summary>
            /// Gets or sets the severity.
            /// </summary>
            public string Severity { get; set; }

            /// <summary>
            /// Gets or sets the msg.
            /// </summary>
            public string Msg { get; set; }
        }
    }
}
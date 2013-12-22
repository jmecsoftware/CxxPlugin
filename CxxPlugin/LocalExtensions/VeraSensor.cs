// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VeraSensor.cs" company="Copyright � 2013 Tekla Corporation. Tekla is a Trimble Company">
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
    using System;
    using System.Collections.Generic;
    using System.IO;

    using ExtensionHelpers;

    using ExtensionTypes;

    /// <summary>
    /// The vera sensor.
    /// </summary>
    public class VeraSensor : ASensor
    {
        /// <summary>
        /// The s key.
        /// </summary>
        public const string SKey = "vera++";

        /// <summary>
        /// Initializes a new instance of the <see cref="VeraSensor"/> class.
        /// </summary>
        /// <param name="ctrl">
        /// The ctrl.
        /// </param>
        /// <param name="options">
        /// The options.
        /// </param>
        public VeraSensor(ICommandExecution ctrl, Dictionary<string, string> options, EventHandler handler)
            : base(SKey, ctrl, true, handler)            
        {
            this.Command = options["VeraExecutable"];
            this.Args = options["VeraArguments"];
            this.Environment = VsSonarUtils.GetEnvironmentFromString(options["VeraEnvironment"]);
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

            foreach (var line in lines)
            {
                try
                {
                    var elems = line.Split(':');
                    var file = elems[0] + ":" + elems[1];
                    var linenumber = Convert.ToInt32(elems[2]);
                    var data = elems[3].Split('(');
                    var id = data[1].Split(')')[0];
                    var msg = data[1].Split(')')[1];

                    var entry = new Issue
                                    {
                                        Line = linenumber,
                                        Message = msg,
                                        Rule = this.RepositoryKey + "." + id,
                                        Component = file
                                    };

                    violations.Add(entry);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("error: " + ex.Message);
                }
            }

            return violations;
        }
    }
}

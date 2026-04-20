using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using System.DirectoryServices;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Principal;
using System.Security.Cryptography.Pkcs;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Net.Http;
using System.Threading.Tasks;

using CSGenio.framework;
using CSGenio.persistence;
using CSGenio.core.persistence;
using GenioServer.security;
using Quidgest.Persistence;
using Quidgest.Persistence.GenericQuery;

// USE /[MANUAL FOR IMPORTS]/
// USE /[MANUAL FOR IMPORTS GlobalFunctions]/

namespace CSGenio.business
{
	/// <summary>
	/// Summary description for GlobalFunctions.
	/// </summary>
	public sealed partial class GlobalFunctions
	{
                public class ParameterInformation
                {
                        private string name;
                        private string type;
                        private string description;
                        private bool opcional;
                        private List<string> possibleValues;

                        public string Name { get => name; set => name = value; }
                        public string Type { get => type; set => type = value; }
                        public string Description { get => description; set => description = value; }
                        public bool Opcional { get => opcional; set => opcional = value; }
                        public List<string> PossibleValues { get => possibleValues; set => possibleValues = value; }
                }

                public class FunctionInformation
                {
                        private string name;
                        private string description;
                        private bool asyncModifier;
                        private List<ParameterInformation> parameters;

                        public string Name { get => name; set => name = value; }
                        public string Description { get => description; set => description = value; }
                        public bool Async { get => asyncModifier; set => asyncModifier = value; }
                        public List<ParameterInformation> Parameters { get => parameters; set => parameters = value; }
                }

                public static List<FunctionInformation> GetSchedulerFuncs()
                {
                        List<FunctionInformation> Funcs = new List<FunctionInformation>()
                        {
                                new FunctionInformation
                                {
                                        Name="NOTIFICATIONS",
                                        Parameters = new List<ParameterInformation>(){
                                                new ParameterInformation
                                                {
                                                        Name="NOTIFICATIONID",
                                                        Type="string"
                                                }
                                        }
                                },
                                new FunctionInformation
                                {
                                        Name="TRANSFERLOGS",
                                        Parameters = new List<ParameterInformation>(){
                                                new ParameterInformation
                                                {
                                                        Name="YEARAPP",
                                                        Type="string"
                                                }
                                        }
                                },
                                new FunctionInformation
                                {
                                        Name="REINDEX",
                                        Parameters = new List<ParameterInformation>(){
                                                new ParameterInformation
                                                {
                                                        Name="SCRIPTS",
                                                        Type="string"
                                                },
                                                new ParameterInformation
                                                {
                                                        Name="ZEROTRUE",
                                                        Type="bool"
                                                },
                                                new ParameterInformation
                                                {
                                                        Name="YEARAPP",
                                                        Type="string"
                                                },
                                                new ParameterInformation
                                                {
                                                        Name="PASSWORD",
                                                        Type="string"
                                                },
                                                new ParameterInformation
                                                {
                                                        Name="USERNAME",
                                                        Type="string"
                                                }
                                        }
                                },
                                new FunctionInformation
                                {
                                    Name="SCHEDULEDPROCESS",
                                    Parameters= new List<ParameterInformation>()
                                },
                
                        };
                return Funcs;
                }

        }

}

using System;
using System.Text.RegularExpressions;

namespace Quidgest.Persistence.GenericQuery
{
    /// <summary>
    /// Sql template of a function
    /// </summary>
    /// <remarks>
    /// <!--
    /// Author: CX 2011.06.28
    /// Modified:
    /// Reviewed:
    /// -->
    /// </remarks>
    public class SqlFunctionTemplate
    {
        /// <summary>
        /// Finds the args ({N}) in a string
        /// </summary>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        private static readonly Regex ARGS_REGEX = new Regex(@"(^|[^\{])\{(?<index>[0-9]+)\}([^\}]|$)", RegexOptions.Compiled);

        /// <summary>
        /// The code template of the function
        /// </summary>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public string Template
        {
            get;
            private set;
        }

        /// <summary>
        /// True if * can be used as argument, otherwise false
        /// </summary>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public bool AllowsAsterik
        {
            get;
            private set;
        }

        /// <summary>
        /// True if the number of arguments is variable, otherwise false
        /// </summary>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public bool HasVariableNumOfArgs
        {
            get;
            private set;
        }

        /// <summary>
        /// The number of arguments supported by this template. If null then it has a variable number of arguments.
        /// </summary>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public int? NumOfArgs
        {
            get;
            private set;
        }

        /// <summary>
        /// The string used to separate the arguments when using a variable number of arguments
        /// </summary>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public string ArgsSeparator
        {
            get;
            private set;
        }

        /// <summary>
        /// List of keywords accepted as arguments
        /// </summary>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public string[] Keywords
        {
            get;
            private set;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="template">Required. The code template of the function</param>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public SqlFunctionTemplate(string template)
            : this(template, false, false, ",", null)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="template">Required. The code template of the function</param>
        /// <param name="hasVariableNumOfArgs">True if the number of arguments is variable, otherwise false. Defaults to <code>false</code>.</param>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public SqlFunctionTemplate(string template, bool hasVariableNumOfArgs)
            : this(template, hasVariableNumOfArgs, false, ",", null)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="template">Required. The code template of the function</param>
        /// <param name="hasVariableNumOfArgs">True if the number of arguments is variable, otherwise false. Defaults to <code>false</code>.</param>
        /// <param name="allowsAsterisk">True if * can be used as argument, otherwise false. Defaults to <code>false</code>.</param>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public SqlFunctionTemplate(string template, bool hasVariableNumOfArgs, bool allowsAsterisk)
            : this(template, hasVariableNumOfArgs, allowsAsterisk, ",", null)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="template">Required. The code template of the function</param>
        /// <param name="hasVariableNumOfArgs">True if the number of arguments is variable, otherwise false. Defaults to <code>false</code>.</param>
        /// <param name="allowsAsterisk">True if * can be used as argument, otherwise false. Defaults to <code>false</code>.</param>
        /// <param name="argsSeparator">Required. The string used to separate the arguments when using a variable number of arguments. Defaults to <code>","</code>.</param>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public SqlFunctionTemplate(string template, bool hasVariableNumOfArgs, bool allowsAsterisk, string argsSeparator)
            : this(template, hasVariableNumOfArgs, allowsAsterisk, argsSeparator, null)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="template">Required. The code template of the function</param>
        /// <param name="hasVariableNumOfArgs">True if the number of arguments is variable, otherwise false. Defaults to <code>false</code>.</param>
        /// <param name="allowsAsterisk">True if * can be used as argument, otherwise false. Defaults to <code>false</code>.</param>
        /// <param name="argsSeparator">Required. The string used to separate the arguments when using a variable number of arguments. Defaults to <code>","</code>.</param>
        /// <param name="keywords">The list of keywords accepted as arguments. Defaults to <code>null</code>.</param>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public SqlFunctionTemplate(string template, bool hasVariableNumOfArgs, bool allowsAsterisk, string argsSeparator, string[] keywords)
        {
            if (String.IsNullOrEmpty(template))
            {
                throw new ArgumentNullException("template");
            }

            if (String.IsNullOrEmpty(argsSeparator))
            {
                throw new ArgumentNullException("argsSeparator");
            }

            Template = template;
            HasVariableNumOfArgs = hasVariableNumOfArgs;
            AllowsAsterik = allowsAsterisk;
            ArgsSeparator = argsSeparator;
            Keywords = keywords;

            if (!HasVariableNumOfArgs)
            {
                // find the number of arguments specified in the template
                MatchCollection mc = ARGS_REGEX.Matches(Template);
                if (mc.Count == 0)
                {
                    NumOfArgs = 0;
                }
                else
                {
                    int max = 0;
                    foreach (Match match in mc)
                    {
                        // the arguments index starts in 0, so the number of arguments is the max index + 1.
                        max = Math.Max(max, Convert.ToInt32(match.Groups["index"].Value) + 1);
                    }
                    NumOfArgs = max;
                }
            }
        }
    }
}

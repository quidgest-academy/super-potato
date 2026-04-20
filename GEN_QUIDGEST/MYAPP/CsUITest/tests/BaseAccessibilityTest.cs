using System.Collections.Generic;
using System.IO;

using Deque.AxeCore.Commons;
using Deque.AxeCore.Selenium;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace quidgest.uitests.tests;

public class BaseAccessibilityTest : BaseSeleniumTest
{
    /// <summary>
	/// Do an accessibility scan and log the results to the console and to JSON files.
	/// </summary>
	/// <param name="pageName">Name of the page and JSON file to log the results to</param>
	/// <param name="cssSelector">CSS selector to specify a part of the DOM to do the accessibility scan</param>
    public void AccessibilityScanAndLog(string pageName, string cssSelector = null)
    {
        // Accessibility scan
        AxeBuilder axeBuilder = new AxeBuilder(Driver)
            //.WithTags("wcag2a", "wcag2aa", "wcag2aaa", "wcag21a", "wcag21aa", "section508", "ACT", "best-practice")
            .WithTags("wcag2a", "wcag2aa", "wcag21a", "wcag21aa")
            .DisableRules("color-contrast");

        if(!string.IsNullOrEmpty(cssSelector))
            axeBuilder.Include(cssSelector);

        AxeResult axeResult = axeBuilder.Analyze();

        string resultJson = "";

        try
        {
            // Create formatted JSON string to log
            resultJson = JsonConvert.SerializeObject(axeResult, Formatting.Indented);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return;
        }

        // Optional
        try
        {
            //< Set unneeded properties to empty
            JObject modifiedResult = JsonConvert.DeserializeObject<JObject>(resultJson);
            modifiedResult["Passes"] = JArray.FromObject(new List<string> { });
            modifiedResult["Inapplicable"] = JArray.FromObject(new List<string> { });
            resultJson = JsonConvert.SerializeObject(modifiedResult, Formatting.Indented);
            //> Set unneeded properties to empty
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        //< Log results as items

        // Log violations
        foreach (AxeResultItem violation in axeResult.Violations)
        {
            foreach (AxeResultNode node in violation.Nodes)
            {
                Console.WriteLine(MessageType.Warning + ": " + violation.Help);
                Console.WriteLine("\t" + violation.Description);
                Console.WriteLine("\tURL:\t" + axeResult.Url);
                Console.WriteLine("\tTarget:\t" + node.Target);
                Console.WriteLine("\tHTML:\t" + node.Html);
                if (node.Any.Length > 0)
                    Console.WriteLine("\tChecks:");
                foreach (AxeResultCheck any in node.Any)
                    Console.WriteLine("\t\t" + any.Message);
                Console.WriteLine("");
            }
        }

        // Log incompletes (things that could not be determined automatically and need to be reviewed manually)
        foreach (AxeResultItem incomplete in axeResult.Incomplete)
        {
            foreach (AxeResultNode node in incomplete.Nodes)
            {
                Console.WriteLine(MessageType.Warning + ": (Needs manual review) " + incomplete.Help);
                Console.WriteLine("\t" + incomplete.Description);
                Console.WriteLine("\tURL:\t" + axeResult.Url);
                Console.WriteLine("\tTarget:\t" + node.Target);
                Console.WriteLine("\tHTML:\t" + node.Html);
                if (node.Any.Length > 0)
                    Console.WriteLine("\tChecks:");
                foreach (AxeResultCheck any in node.Any)
                    Console.WriteLine("\t\t" + any.Message);
                Console.WriteLine("");
            }
        }
        //> Log results as items

        // Log results to files
        try
        {
            // Get directory for test results and create if it doesn't exist
            string projectDirectory = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;
            string testSubDirectory = "test_results";
            string testDirectory = Path.Combine(projectDirectory, testSubDirectory);
            Directory.CreateDirectory(testDirectory);

            string fileName = pageName + ".json";
            File.WriteAllText(Path.Combine(testDirectory, fileName), resultJson);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}

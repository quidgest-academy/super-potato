using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using Ganss.Xss;
using AngleSharp.Html.Dom;
using CSGenio.framework;

namespace GenioMVC.Helpers
{
    public static class HtmlSanitizerHelper
    {
        // Reusable HtmlSanitizer instance to improve performance.
        // Note: The Sanitize() and SanitizeDocument() methods are thread-safe
        private static readonly HtmlSanitizer sanitizer = new HtmlSanitizer();

        static HtmlSanitizerHelper()
        {
            // Add allowed schemes and attributes during initialization
            // https://github.com/mganss/HtmlSanitizer/wiki
            /*
            * sanitizer.AllowedSchemes.Add("mailto");
            * 
            * // Note: to prevent classjacking (https://html5sec.org/#123) and interference with classes where the sanitized fragment is to be integrated, the class attribute is not in the whitelist by default
            * sanitizer.AllowedAttributes.Add("class")
            */
            sanitizer.RemovingAttribute += SanitizeAttribute;
        }

        /// <summary>
        /// Sanitizes HTML content.
        /// </summary>
        /// <param name="plainText">The HTML content to be sanitized.</param>
        /// <param name="isDocument">Indicates whether the content is a complete HTML document.</param>
        /// <returns>Sanitized HTML content.</returns>
        public static string SanitizeHTML(string plainText, bool isDocument)
        {
            // Determine the base URL for resolving relative URLs. No resolution if empty
            string baseUrl = GetBaseUrl();

            // Sanitize the HTML content, treating it as a document if necessary.
            // Note: In the case of using TinyMCE, the content is the full HTML of a document
            return isDocument ? sanitizer.SanitizeDocument(plainText, baseUrl) : sanitizer.Sanitize(plainText, baseUrl);
        }

        /// <summary>
        /// Handles the attribute removal event, validating the "src" attribute for images with base64 content.
        /// </summary>
        private static void SanitizeAttribute(object sender, RemovingAttributeEventArgs e)
        {
            // Keep the TinyMCE images that use "src" attribute with base64 image
            if (e.Tag is IHtmlImageElement img && e.Attribute.Name.Equals("src", StringComparison.OrdinalIgnoreCase))
            {
                string src = img.Source;

                // Validate base64-encoded images
                if (src?.StartsWith("data:image/", StringComparison.OrdinalIgnoreCase) == true)
                {
                    if (IsValidBase64Image(src))
                    {
                        e.Cancel = true; // Retain the valid image source
                    }
                    else
                    {
                        e.Cancel = false; // Remove invalid image sources
                    }
                }
            }
        }

        /// <summary>
        /// Validates whether a base64-encoded image is a valid and safe image.
        /// </summary>
        /// <param name="dataUrl">The data URL containing the base64-encoded image.</param>
        /// <returns>True if the image is valid; otherwise, false.</returns>
        private static bool IsValidBase64Image(string dataUrl)
        {
            try
            {
                string base64Data = dataUrl.Substring(dataUrl.IndexOf(",") + 1);
                byte[] imageData = Convert.FromBase64String(base64Data);
                string fileContent = Encoding.UTF8.GetString(imageData);

                /* 
                 * Even though they are not the safest, we cannot prohibit SVG images. In any case,
                 *  what is truly necessary is some form of sanitization for the SVG content (including during image upload).
                 */
                if (IsValidSvg(fileContent))
                    return true;

                using (var ms = new MemoryStream(imageData))
                {
                    Image img = Image.FromStream(ms);
                    img.Dispose();
                    return true; // Successfully loaded the image
                }
            }
            catch (Exception ex)
            {
                Log.Error($"HTML Sanitizer - Invalid base64 image detected: {ex.Message} | Source: {dataUrl}");
                return false;
            }
        }

        /// <summary>
        /// Retrieves the base URL for resolving relative URLs in the HTML content.
        /// </summary>
        /// <returns>The base URL as a string.</returns>
        private static string GetBaseUrl()
        {
            return Configuration.ExistsProperty("PUBLIC_BASE_URL")
                ? Configuration.GetProperty("PUBLIC_BASE_URL")
                : string.Empty;
        }

        // Define a set of dangerous elements and attributes in SVG content for fast lookups.
        private static readonly HashSet<string> SvgDangerousElements = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "script", "foreignObject", "iframe", "object", "embed", "image", "audio", "video", "animation", "set", "animate", "link"
        };

        private static readonly HashSet<string> SvgDangerousAttributes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "onload", "onclick", "onmouseover", "href", "xlink:href"
        };

        /// <summary>
        /// Validates whether the provided SVG content is safe by checking for dangerous elements and attributes.
        /// </summary>
        /// <param name="svgContent">The SVG content to be validated.</param>
        /// <returns>Returns true is the SVG content is valid and safe, otherwise false.</returns>
        public static bool IsValidSvg(string svgContent)
        {
            // Performe a basic check for SVG root element and namespace before proceeding to XML parsing.
            if (string.IsNullOrEmpty(svgContent) || svgContent.IndexOf("<svg", StringComparison.OrdinalIgnoreCase) == -1 || svgContent.IndexOf("http://www.w3.org/2000/svg", StringComparison.OrdinalIgnoreCase) == -1)
                return false;

            // Load the SVG content into an XML parser.
            var xmlDoc = new XmlDocument();
            try
            {
                // Parse the XML content.
                xmlDoc.LoadXml(svgContent);
            }
            catch (Exception ex)
            {
                // Log error if XML parsing fails, indicates invalid or corrupt SVG content.
                Log.Error($"SVG validation failed. Invalid XML content: {ex.Message}");
                return false;
            }

            // Check for dangerous elements in the SVG.
            foreach (var element in SvgDangerousElements)
            {
                var nodes = xmlDoc.GetElementsByTagName(element);
                if (nodes.Count > 0)
                {
                    // Log and return false immediately if a dangerous element is found.
                    Log.Error($"Dangerouse SVG content detected. Element: <{element}>");
                    return false;
                }
            }

            // Check for dangerous attributes in all elements.
            var allElements = xmlDoc.GetElementsByTagName("*");
            foreach(XmlElement elem in allElements)
            {
                foreach(var attr in SvgDangerousAttributes)
                {
                    if(elem.HasAttribute(attr))
                    {
                        // Log and return immediately if a dangerous attribute is found.
                        Log.Error($"Dangerous SVG content detected. Attribute: {attr} in element <{elem.Name}>");
                        return false;
                    }
                }
            }

            // Return true if no dangerous elements or attributes are found.
            return true;
        }
       
        /// <summary>
        /// Extracts only the text content from HTML, removing all HTML tags, images, and formatting.
        /// This is useful for preparing content to be sent to a Large Language Model (LLM).
        /// </summary>
        /// <param name="htmlContent">The HTML content to extract text from.</param>
        /// <returns>Plain text content.</returns>
        public static string ExtractTextOnly(string htmlContent, bool isDocument)
        {
            if (string.IsNullOrEmpty(htmlContent))
            {
                return string.Empty;
            }

            // First sanitize the HTML to remove unwanted elements like base64 images
            var sanitizedHtml = SanitizeHTML(htmlContent, isDocument);

            // Parse HTML and extract only the text content
            var parser = new AngleSharp.Html.Parser.HtmlParser();
            var htmlDocument = parser.ParseDocument(sanitizedHtml);

            // Simply get the text content of the body
            return htmlDocument.Body?.TextContent?.Trim() ?? string.Empty;
        }
    }
}
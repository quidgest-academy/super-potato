using CSGenio.business;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using CSGenio.framework;

namespace CSGenio.core.ai
{
    public interface IChatbotService
    {
        /// <summary>
        /// Sends a request to the chatbot service with specified path, method and optional content
        /// </summary>
        Task<string> SendChatbotRequestAsync(string path, HttpMethod method, Stream content, User user);

        /// <summary>
        /// Gets a stream response from the chatbot service
        /// </summary>
        Task<Stream> GetChatbotStreamAsync(Stream requestData, User user);
		
		/// <summary>
        /// Gets a stream response from the chatbot service(formdata handling)
        /// </summary
        Task<Stream> GetChatbotStreamAsync(
            IEnumerable<KeyValuePair<string, string>> fields,
            IEnumerable<(string FileName, string ContentType, Stream Content)> files,
            User user);
			
        /// <summary>
        /// Gets the respective file from the chatbot server
        /// </summary
        Task<Stream> GetChatbotFileAsync(string fileName, HttpMethod method);
		
        /// <summary>
        /// Makes a function call to the chatbot service and returns the result of type T
        /// </summary>
        Task<T> CallChatbotFunctionAsync<T>(AgentRequestData requestData);

        /// <summary>
        /// Calls a specific function on the Chatbot API and deserializes the response.
        /// </summary>
        [Obsolete("Use CallChatbotFunctionAsync<T>(AgentRequestData requestData) instead.")]
        Task<T> CallChatbotFunctionAsync<T>(object requestData);


        /// <summary>
        /// Makes a function call to the chatbot service and returns the result of type T
        /// </summary>
        Task<T> CallChabotAgentPromptAsync<T>(AgentRequestData requestData);
    }
}
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using CSGenio.framework;
using JsonIgnoreAttribute = System.Text.Json.Serialization.JsonIgnoreAttribute;

namespace GenioMVC.Models.Navigation
{
	/// <summary>
	/// Interface Message
	/// </summary>
	[Serializable]
	public class Message
	{
		private string m_content;
		public string Content
		{
			get
			{
				return m_content;
			}
		}

		private string m_title;
		public string Title
		{
			get
			{
				return m_title;
			}
		}

		private string m_id;
		public string ID
		{
			get
			{
				return m_id;
			}
		}

		private Status m_status;
		[JsonIgnore]
		public Status Status
		{
			get
			{
				return m_status;
			}
		}

		// MH - To simplify serialization and to not create a dependency of the Newtonsoft library on the CSGenio.framework
		[JsonPropertyName("Status")]
		public String StrStatus
		{
			get
			{
				return m_status.ToString();
			}
		}

		private bool m_containsHTML;
		public bool ContainsHtml
		{
			get
			{
				return m_containsHTML;
			}
		}

		[JsonConstructor]
		public Message(string content, string strstatus)
		{
            m_content = content;
            m_id = Guid.NewGuid().ToString();
			m_status = new Status(strstatus);
        }

		public Message(string content, Status status, bool containsHtml = false)
		{
			this.m_content = content;
			this.m_id = Guid.NewGuid().ToString();
			this.m_status = status;
			this.m_containsHTML = containsHtml;
		}

		public Message(string title, string content, Status status, bool containsHtml = false)
		{
			this.m_title = title;
			this.m_content = content;
			this.m_id = Guid.NewGuid().ToString();
			this.m_status = status;
			this.m_containsHTML = containsHtml;
		}
	}

	public class Messages
	{
		public static string getID(String navigationID)
		{
			return "Messages_" + navigationID;
		}
	}
}

using System;
using System.Collections.Generic;

using GenioMVC.Models.Navigation;

namespace GenioMVC.ViewModels
{
	public class Messages_ViewModel
	{
		private List<Message> m_messages;

		public IList<Message> Messages
		{
			get { return m_messages; }
		}

		public Messages_ViewModel(List<Message> messages)
		{
			m_messages = messages;
		}
	}
}

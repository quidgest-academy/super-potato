using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

using CSGenio.business;
using CSGenio.framework;
using CSGenio.persistence;
using GenioMVC.Models.Navigation;
using Quidgest.Persistence;
using Quidgest.Persistence.GenericQuery;

namespace GenioMVC.ViewModels
{
	public class Alerts_ViewModel
	{
		private UserContext m_userContext;

		private NavigationContext Navigation => m_userContext.CurrentNavigation;

		private NameValueCollection QueryString { get; set; }

		private bool IsAjaxRequest { get; set; }

		public Alerts_ViewModel(UserContext userContext, NameValueCollection queryString = null, bool isAjaxRequest = true)
		{
			m_userContext = userContext;
			QueryString = queryString;
			IsAjaxRequest = isAjaxRequest;
		}

		public List<Models.Navigation.Alert> GenAlerts()
		{
			var user = m_userContext.User;
			var sp = m_userContext.PersistentSupport;

			List<Models.Navigation.Alert> alerts = new List<Models.Navigation.Alert>();

			sp.openConnection();
			sp.closeConnection();

			return alerts;
		}

	}
}

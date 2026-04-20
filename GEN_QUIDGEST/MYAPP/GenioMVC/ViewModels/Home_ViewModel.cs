using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Globalization;
using System.Linq;

using CSGenio.business;
using GenioMVC.Helpers;
using GenioMVC.Models.Navigation;
using Quidgest.Persistence.GenericQuery;
using JsonIgnoreAttribute = System.Text.Json.Serialization.JsonIgnoreAttribute;

namespace GenioMVC.ViewModels
{
	public class Home_ViewModel : ViewModelBase
	{
		public ViewModels.Home.HomePage_ViewModel HomePage_model;

// USE /[MANUAL FOR HOME_VIEWMODEL]/

		public Home_ViewModel(UserContext userContext) : base(userContext) { }
	}
}

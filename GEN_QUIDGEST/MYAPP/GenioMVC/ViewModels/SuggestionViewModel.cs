using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

using CSGenio.business;
using CSGenio.framework;
using GenioMVC.Models;

namespace GenioMVC.ViewModels
{
	public class SuggestionViewModel
	{
		[Display(Name = "SUGGESTION_TEXT21450",  ResourceType = typeof(Resources.Resources))]
		public String SuggestionText { get; set; }

		[Display(Name = "NOME_DO_CAMPO53580",  ResourceType = typeof(Resources.Resources))]
		public String NewLabel { get; set; }

		[Display(Name = "TEXTO_DA_AJUDA50435",  ResourceType = typeof(Resources.Resources))]
		[DataType(DataType.MultilineText)]
		public String NewHelp { get; set; }

		public String FieldId { get; set; }

		public List<ArrayElement> Elements { get; }

		public String ArrayName { get; set;}

		public SuggestionViewModel()
		{
			Elements = new List<ArrayElement>();
		}

		public SuggestionViewModel(string arrayName, string language)
		{
			Elements = new List<ArrayElement>();
			if (String.IsNullOrEmpty(arrayName))
				return;

			ArrayName = arrayName;
			ArrayInfo arrayInfo = new ArrayInfo(arrayName);

			foreach (var id in arrayInfo.Elements)
			{
				string description = arrayInfo.GetDescription(id, language);
				string help = arrayInfo.GetHelp(id, language);
				Elements.Add(new ArrayElement
				{
					Id = id,
					NewLabel = description,
					NewHelp = help
				});
			}
		}
	}

	public class ArrayElement
	{
		public string Id { get; set; }

		[Display(Name ="DESCRICAO_DO_ELEMENT42151",  ResourceType = typeof(Resources.Resources))]
		public String NewLabel { get; set; }

		[Display(Name ="TEXTO_DA_AJUDA_DO_EL65207",  ResourceType = typeof(Resources.Resources))]
		[DataType(DataType.MultilineText)]
		public String NewHelp {get;set;}
	}

	public class SuggestionListViewModel
	{
		public List<SuggestionListElement> Suggestions { get; }

		public SuggestionListViewModel() { }

		public SuggestionListViewModel(string currentUser, string currentLocation)
		{
			Collaboration collaboration = Collaboration.ReadXml(currentLocation);
			var userSuggestions = collaboration.AllSuggestions()
				.Where(x => x.User == currentUser);
			Suggestions = new List<SuggestionListElement>();

			foreach (var element in userSuggestions)
			{
				string tipo = "";
				if (!String.IsNullOrEmpty(element.NewLabel))
				{
					if (element is ArrayElementSuggestion)
						tipo = Resources.Resources.ELEMENTO_DO_ENUMERAD07806;
					else
						tipo = Resources.Resources.NOME_DO_CAMPO53580;
					Suggestions.Add(new SuggestionListElement(element.OldLabel, element.NewLabel, tipo));
				}
				if (!String.IsNullOrEmpty(element.NewHelp))
				{
					if (element is ArrayElementSuggestion)
						tipo = Resources.Resources.AJUDA_DO_ELEMENTO_DO40083;
					else
						tipo = Resources.Resources.AJUDA_DO_CAMPO44959;
					Suggestions.Add(new SuggestionListElement(element.OldLabel, element.NewHelp, tipo));
				}
				if (element is OpenSuggestion)
				{
					tipo = Resources.Resources.SUGESTAO_ABERTA21277;
					var openSuggestion = (OpenSuggestion)element;
					Suggestions.Add(new SuggestionListElement("-", openSuggestion.SuggestionText, tipo));
				}
			}
		}
	}

	public class SuggestionListElement
	{
		public string OldValue { get; }

		public string NewValue { get; }

		public string SuggestionType { get; }

		public SuggestionListElement() { }

		public SuggestionListElement(string oldValue, string newValue, string tipo)
		{
			OldValue = oldValue;
			NewValue = newValue;
			SuggestionType = tipo;
		}
	}
}

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Search.Spans;
using Lucene.Net.Store;

using GenioMVC.Models.Navigation;
using User = CSGenio.framework.User;
using Role = CSGenio.framework.Role;
using Directory = Lucene.Net.Store.Directory;
using Lucene.Net.Util;

namespace GenioMVC.Helpers.Menus
{
	public class MenuSearch
	{
		private static readonly string ID = "id";
		private static readonly string TEXT = "text";
		private static readonly string FLAT = "flat";
		private static readonly string MODULE = "module";
		private static readonly string ROLE = "role";

		private readonly static MenuSearch instance = new MenuSearch();

		//Dictionary with a search index for each language
		private readonly Dictionary<string, Directory> directories = new Dictionary<string, Directory>();

		private MenuSearch() { }

		/// <summary>
		/// Builds an index from the menu list
		/// </summary>
		/// <param name="menus"></param>
		private void BuildIndex(List<MenuResult> menus, string language)
		{
			var version = LuceneVersion.LUCENE_48;
			Analyzer analyzer = new StandardAnalyzer(version);
			directories[language] = new RAMDirectory();
			var directory = directories[language];

			var config = new IndexWriterConfig(version, analyzer);
			using (IndexWriter indexWriter = new IndexWriter(directory, config))
			{
				foreach (var menu in menus)
					BuildMenuEntry(indexWriter, menu);
			}
		}

		/// <summary>
		/// Adds the menu entry and all its children to the index
		/// </summary>
		/// <param name="indexWriter">A lucene indexwriter</param>
		/// <param name="entry">The menu entry to be added to the index</param>
		private void BuildMenuEntry(IndexWriter indexWriter, MenuResult result)
		{
			var entry = Menus.FindMenu(result.Module, result.Id);

			//Field id will not be searched but must be stored.
			Field idField = new StringField(ID, entry.ID, Field.Store.YES);
			Field textField = new TextField(TEXT, result.Text, Field.Store.YES);
			Field flatMenuField = new TextField(FLAT, result.FlatMenu, Field.Store.YES);
            Field moduleField = new StringField(MODULE, result.Module, Field.Store.YES);

			Document document = new Document();
			document.Add(idField);
			document.Add(textField);
			document.Add(moduleField);
			document.Add(flatMenuField);

			var role = Role.GetRole(entry.RoleId);
			var roleList = role.AllRolesAbove();
			roleList.ForEach(r => document.Add(new StringField(ROLE, r.Id, Field.Store.YES)));

			indexWriter.AddDocument(document);
		}

		private BooleanQuery BuildPhraseQuery(string searchText, string field, Directory directory)
		{
			var terms = searchText.Split(' ');
			using (var reader = DirectoryReader.Open(directory))
			{

				List<BooleanClause> clauses = new List<BooleanClause>();
				foreach (var term in terms)
				{
					var fuzzyQuery = new FuzzyQuery(new Term(field, term.ToLower()));
					BooleanClause termClause = new BooleanClause(fuzzyQuery, Occur.SHOULD);
					clauses.Add(termClause);
				}

				BooleanQuery finalQuery = new BooleanQuery();
				for (int num = 1; num <= clauses.Count; num++)
				{
					BooleanQuery query = new BooleanQuery();
					foreach (var clause in clauses)
					{
						query.Add(clause);
					}
					query.Boost = num;
					query.MinimumNumberShouldMatch = num;
					finalQuery.Add(query, Occur.SHOULD);
				}

				return finalQuery;
			}
		}

		/// <summary>
		/// Builds a LuceneQuery to search the title and the flattened structure while respecting levels
		/// </summary>
		private BooleanQuery BuildQuery(string searchText, User user, Directory directory)
		{

			//The slop indicates how many words can be between two terms
			var flatQuery = BuildPhraseQuery(searchText, FLAT, directory);
			var titleQuery = BuildPhraseQuery(searchText, TEXT, directory);
			//Items with the result in the title are more important than others
			titleQuery.Boost = 2;

			BooleanQuery query = new BooleanQuery();
			BooleanClause titleClause = new BooleanClause(titleQuery, Occur.SHOULD);
			query.Add(titleClause);
			//The search term must be present in the flattened structure
			BooleanClause flatClause = new BooleanClause(flatQuery, Occur.MUST);
			query.Add(flatClause);

			//There must be an instance of the module with required role level
			BooleanQuery accessQuery = new BooleanQuery();
			IEnumerable<string> modules = CSGenio.framework.Configuration.Application.Modules.Select(m => m.Key);
			foreach (string module in modules)
			{
				foreach (var moduleRole in user.GetModuleRoles(module))
				{
					BooleanQuery moduleQuery = new BooleanQuery() {
						{ new TermQuery(new Term(MODULE, module)), Occur.MUST },
						{ new TermQuery(new Term(ROLE, moduleRole.Id)), Occur.MUST }
					};

					accessQuery.Add(new BooleanClause(moduleQuery, Occur.SHOULD));
				}
			}

			query.Add(accessQuery, Occur.MUST);
			return query;
		}

		private static readonly object buildLock = new object();

		public static List<MenuResult> Search(UserContext userContext, string searchText, User user, CultureInfo cultureInfo)
		{
			var language = cultureInfo.Name;
			lock (buildLock)
			{
				if (!instance.directories.ContainsKey(language))
				{
					instance.BuildIndex(MenuResult.SearchableMenus(userContext, cultureInfo), language);
				}
			}
			return instance.Search(userContext, searchText, cultureInfo, user);
		}

		/// <summary>
		/// Builds a query and searches the index of a given language with the query, checking all the necessary permissions
		/// </summary>
		/// <param name="searchText">The text to be searched</param>
		/// <param name="cultureInfo">The user cultureInfo</param>
		/// <param name="levels">User access levels for each module</param>
		/// <returns></returns>
		private List<MenuResult> Search(UserContext userContext, string searchText, CultureInfo cultureInfo, User user)
		{
			var directory = instance.directories[cultureInfo.Name];
			var reader = DirectoryReader.Open(directory);
			IndexSearcher searcher = new IndexSearcher(reader);

			var query = BuildQuery(searchText, user, directory);
			var hits = searcher.Search(query, 30);

			var menus = MenuResult.SearchableMenus(userContext, cultureInfo);
			var results = new List<MenuResult>();
			foreach (var hit in hits.ScoreDocs)
			{
				var document = searcher.Doc(hit.Doc);
				var result = menus.Find(m => m.Id == document.Get(ID) && m.Module == document.Get(MODULE));
				if (result != null)
					results.Add(result);
			}

			return results;
		}
	}
}

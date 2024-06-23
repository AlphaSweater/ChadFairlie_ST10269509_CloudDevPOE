using Azure.Search.Documents.Models;
using Azure.Search.Documents;
using System;

namespace CloudDevPOE.Services;

public class SearchService
{
	private readonly SearchClient _searchClient;

	public SearchService(SearchClient searchClient)
	{
		_searchClient = searchClient;
	}

	public async Task<IEnumerable<SearchDocument>> SearchProductsAsync(string searchText)
	{
		try
		{
			var options = new SearchOptions
			{
				IncludeTotalCount = true,
				Select = { "product_id", "name", "category", "description", "price", "availability" }
			};

			var searchResults = await _searchClient.SearchAsync<SearchDocument>(searchText, options);
			return searchResults.Value.GetResults().Select(r => r.Document);
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Error performing search: {ex.Message}");
			throw;
		}
	}

	public async Task<IEnumerable<SearchDocument>> GetSuggestionsAsync(string query)
	{
		try
		{
			var suggesterOptions = new SuggestOptions
			{
				UseFuzzyMatching = true,
				Size = 5,
				Select = { "product_id", "name", "category", "description", "price" }
			};

			var suggestions = await _searchClient.SuggestAsync<SearchDocument>(query, "sg", suggesterOptions);
			return suggestions.Value.Results.Select(r => r.Document);
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Error fetching suggestions: {ex.Message}");
			throw;
		}
	}
}
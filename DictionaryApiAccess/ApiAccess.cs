using DictionaryApiAccess.Exceptions;
using System.Net;

namespace DictionaryApiAccess
{
    public static class ApiAccess
    {
        static readonly HttpClient client = new();

        public static async Task<bool> IsWordValid(string word)
        {
            var response = await client.GetAsync($"https://api.dictionaryapi.dev/api/v2/entries/en/{word}");
            if (!response.IsSuccessStatusCode && response.StatusCode != HttpStatusCode.NotFound)
                throw new ApiException($"Api returned {response.StatusCode}!");
            if (response.StatusCode == HttpStatusCode.NotFound)
                return false;
            return true;
        }
    }
}

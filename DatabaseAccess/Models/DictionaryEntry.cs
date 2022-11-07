namespace DatabaseAccess.Models
{
    public class DictionaryEntry
    {
        public int Id { get; set; }
        public string Word { get; set; }
        public ulong GuildId { get; set; }

        public DictionaryEntry(string word)
            => Word = word;
    }
}

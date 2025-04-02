using System;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Impostor.Api.Innersloth.GameFilters
{
    [Serializable]
    public class GameFilter
    {
        [JsonIgnore]
        public ISubFilter SubFilter { get; private set; }

        public GameFilter(string key, ISubFilter subFilter)
        {
            this.OptionType = subFilter.FilterType;
            this.Key = key;
            this.SubFilter = subFilter;
        }

        public void ModifySubFilter(ISubFilter subFilter)
        {
            this.SubFilter = subFilter;
        }

        [JsonConstructor]
        private GameFilter(string optionType, string key, string subFilterString)
        {
            this.OptionType = optionType;
            this.Key = key;
            this.SubFilterString = subFilterString;
        }

        [OnSerializing]
        internal void OnSerializing(StreamingContext context)
        {
            this.SubFilterString = JsonSerializer.Serialize(this.SubFilter);
        }

        [OnDeserialized]
        internal void OnDeserialized(StreamingContext context)
        {
            this.SubFilter = this.ResolveSubFilter(this.OptionType, this.SubFilterString);
        }

        private ISubFilter ResolveSubFilter(string type, string filterString)
        {
            if (type != null)
            {
                switch (type)
                {
                    case "int":
                        return JsonSerializer.Deserialize<IntGameFilter>(filterString);
                    case "platform":
                        return JsonSerializer.Deserialize<PlatformGameFilter>(filterString);
                    case "cat":
                        return JsonSerializer.Deserialize<CategorizedGameFilter>(filterString);
                    case "languages":
                        return JsonSerializer.Deserialize<LanguageFilter>(filterString);
                    case "chat":
                        return JsonSerializer.Deserialize<ChatModeGameFilter>(filterString);
                    case "map":
                        return JsonSerializer.Deserialize<MapGameFilter>(filterString);
                    case "bool":
                        return JsonSerializer.Deserialize<BoolGameFilter>(filterString);
                    default:
                        return null;
                }
            }

            return null;
        }

        public const string FILTER_TYPE_BOOL = "bool";
        public const string FILTER_TYPE_INT = "int";
        public const string FILTER_TYPE_CATEGORIZED = "cat";
        public const string FILTER_TYPE_MAP = "map";
        public const string FILTER_TYPE_PLATFORM = "platform";
        public const string FILTER_TYPE_CHAT = "chat";
        public const string FILTER_TYPE_LANG = "languages";

        public string OptionType;
        public string Key;
        public string SubFilterString;
    }
}

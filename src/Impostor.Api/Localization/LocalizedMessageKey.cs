namespace Impostor.Api.Localization
{
    /// <summary>
    /// A message identifier scoped to a plugin ID.
    /// </summary>
    public readonly record struct LocalizedMessageKey
    {
        public LocalizedMessageKey(string owner, string key)
        {
            Owner = owner;
            Key = key;
        }

        public string Owner { get; }

        public string Key { get; }

        public override string ToString() => $"{Owner}:{Key}";
    }
}

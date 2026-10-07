using Impostor.Api.Innersloth;

namespace Impostor.Api.Localization
{
    public interface ILocalizationService
    {
        /// <summary>
        /// Resolves and formats a message for a recipient.
        /// Unknown languages use the catalog's default language.
        /// Missing keys return the identifier for diagnosis.
        /// </summary>
        /// <param name="message">The owner and message key.</param>
        /// <param name="language">The recipient's language.</param>
        /// <param name="arguments">Values for .NET composite format placeholders.</param>
        /// <returns>The formatted message, or the identifier when no candidate can be formatted.</returns>
        string Get(LocalizedMessageKey message, Language language, params object?[] arguments);
    }
}

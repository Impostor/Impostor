using System;
using System.Diagnostics.CodeAnalysis;

namespace Impostor.Api.Net.Custom
{
    public interface ICustomMessageManager<T>
        where T : ICustomMessage
    {
        bool TryGet(byte id, [MaybeNullWhen(false)] out T message);

        /// <summary>
        ///     Register a custom message.
        /// </summary>
        /// <param name="message"><typeparamref name="T"/> message.</param>
        /// <returns>Disposable that unregisters the message.</returns>
        IDisposable Register(T message);
    }
}

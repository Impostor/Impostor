using System.Collections.Generic;
using System.Threading.Tasks;
using Impostor.Api.Net;
using Impostor.Api.Net.Custom;
using Impostor.Api.Net.Inner;
using Impostor.Api.Net.Inner.Objects.GameManager;
using Impostor.Server.Net.Inner.Objects.GameManager.Logic;
using Impostor.Server.Net.State;
using Microsoft.Extensions.Logging;

namespace Impostor.Server.Net.Inner.Objects.GameManager;

internal abstract class InnerGameManager : InnerNetObject, IInnerGameManager
{
    private readonly ILogger<InnerGameManager> _logger;
    private readonly List<GameLogicComponent> _logicComponents = new();

    public InnerGameManager(ICustomMessageManager<ICustomRpc> customMessageManager, Game game, ILogger<InnerGameManager> logger) : base(customMessageManager, game)
    {
        _logger = logger;

        Components.Add(this);
    }

    public LogicGameFlow LogicFlow { get; protected init; } = null!;

    public LogicMinigame LogicMinigame { get; protected init; } = null!;

    public LogicRoleSelection LogicRoleSelection { get; protected init; } = null!;

    public LogicUsables LogicUsables { get; protected init; } = null!;

    public LogicOptions LogicOptions { get; protected init; } = null!;

    protected T AddGameLogic<T>(T logic)
        where T : GameLogicComponent
    {
        _logicComponents.Add(logic);
        return logic;
    }

    /// <summary>
    ///     Finds the tag of the registered <see cref="GameLogicComponent"/>.
    /// </summary>
    /// <param name="logic">Instance to search for.</param>
    /// <typeparam name="T">Intance type to search for.</typeparam>
    /// <returns>Tag of the registered <see cref="GameLogicComponent"/>, or -1 if not found.</returns>
    internal int GetGameLogicTag<T>(T logic)
        where T : GameLogicComponent
    {
        return _logicComponents.IndexOf(logic);
    }

    public override async ValueTask<bool> HandleRpcAsync(ClientPlayer sender, ClientPlayer? target, RpcCalls call, IMessageReader reader)
    {
        // Let all logic components process this RPC. If at least one component handles it, return true.
        var result = false;
        foreach (var logicComponent in _logicComponents)
        {
            result |= await logicComponent.HandleRpcAsync(call, reader);
        }

        // If no component accepted it, try and find a custom RPC that can deal with it.
        if (!result)
        {
            result |= await base.HandleRpcAsync(sender, target, call, reader);
        }

        return result;
    }

    public override ValueTask<bool> SerializeAsync(IMessageWriter writer, bool initialState)
    {
        throw new System.NotImplementedException();
    }

    public override async ValueTask DeserializeAsync(IClientPlayer sender, IClientPlayer? target, IMessageReader reader, bool initialState)
    {
        while (reader.Position < reader.Length)
        {
            var innerReader = reader.ReadMessage();
            var tag = (int)innerReader.Tag;
            if (tag < 0 || tag > this._logicComponents.Count)
            {
                _logger.LogError("Out of bounds in DeserializeAsync of InnerGameManager");
                continue;
            }

            var component = this._logicComponents[tag];

            await component.DeserializeAsync(innerReader, initialState);

            if (innerReader.Position < innerReader.Length)
            {
                _logger.LogWarning(
                    "Server did not consume all bytes from {0} component {1} ({2} < {3}).",
                    nameof(InnerGameManager),
                    component.GetType().FullName,
                    innerReader.Position,
                    innerReader.Length);
            }
        }
    }
}

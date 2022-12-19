using System.Collections.Generic;
using System.Threading.Tasks;
using Impostor.Api.Net;
using Impostor.Api.Net.Custom;
using Impostor.Api.Net.Inner.Objects.GameManager;
using Impostor.Server.Net.Inner.Objects.GameManager.Logic;
using Impostor.Server.Net.State;
using Microsoft.Extensions.Logging;

namespace Impostor.Server.Net.Inner.Objects.GameManager;

internal abstract class InnerGameManager : InnerNetObject, IInnerGameManager
{
    private readonly ILogger<InnerGameManager> _logger;

    public InnerGameManager(ICustomMessageManager<ICustomRpc> customMessageManager, Game game, ILogger<InnerGameManager> logger) : base(customMessageManager, game)
    {
        _logger = logger;

        Components.Add(this);
    }

    protected readonly List<GameLogicComponent> LogicComponents = new();

    public LogicGameFlow LogicFlow { get; protected init; } = null!;

    public LogicMinigame LogicMinigame { get; protected init; } = null!;

    public LogicRoleSelection LogicRoleSelection { get; protected init; } = null!;

    public LogicUsables LogicUsables { get; protected init; } = null!;

    public LogicOptions LogicOptions { get; protected init; } = null!;

    protected T AddGameLogic<T>(T logic)
        where T : GameLogicComponent
    {
        LogicComponents.Add(logic);
        return logic;
    }

    internal int? GetGameLogicTag<T>(T logic)
        where T : GameLogicComponent
    {
        for (var i = 0; i < LogicComponents.Count; i++)
        {
            var component = LogicComponents[i];
            if (component == logic)
            {
                return i;
            }
        }

        return null;
    }

    public override ValueTask<bool> SerializeAsync(IMessageWriter writer, bool initialState)
    {
        throw new System.NotImplementedException();
    }

    public override ValueTask DeserializeAsync(IClientPlayer sender, IClientPlayer? target, IMessageReader reader, bool initialState)
    {
        while (reader.Position < reader.Length)
        {
            var innerReader = reader.ReadMessage();
            var tag = (int)innerReader.Tag;
            if (tag < 0 || tag > this.LogicComponents.Count)
            {
                _logger.LogError("Out of bounds in DeserializeAsync of InnerGameManager");
                continue;
            }

            this.LogicComponents[tag].Deserialize(innerReader, initialState);
        }

        return ValueTask.CompletedTask;
    }
}

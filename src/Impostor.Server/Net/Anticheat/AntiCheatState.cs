using System.Diagnostics;
using Impostor.Api.Config;
using Impostor.Api.Innersloth.GameOptions;

namespace Impostor.Server.Net.Anticheat
{
    internal sealed class AntiCheatState
    {
        private const int TimelineCount = 256;
        private const int UnknownTimelineIndex = TimelineCount - 1;
        private readonly PlayerTimeline[] _timelines = new PlayerTimeline[TimelineCount];
        private readonly IGameOptions _options;
        private double _roundStartedAt = -1;
        private double _meetingOpenedAt = -1;
        private bool _meetingActive;

        public AntiCheatState(IGameOptions options, AntiCheatConfig config)
        {
            _options = options;
            Config = config;

            for (var i = 0; i < _timelines.Length; i++)
            {
                _timelines[i] = new PlayerTimeline();
            }
        }

        public AntiCheatConfig Config { get; }

        public static double Now => (double)Stopwatch.GetTimestamp() / Stopwatch.Frequency;

        public bool ShipLoaded => _roundStartedAt >= 0;

        public bool ShipGraceOver
        {
            get
            {
                if (_roundStartedAt < 0)
                {
                    return true;
                }

                var cooldown = _options is NormalGameOptions normal ? normal.EmergencyCooldown : 0;
                return cooldown <= 0 || Now - _roundStartedAt >= cooldown / 2;
            }
        }

        public bool InMeeting => _meetingActive;

        public double SecondsSinceRoundStarted => _roundStartedAt < 0 ? -1 : Now - _roundStartedAt;

        public bool MeetingGuardArmed =>
            _meetingActive && _meetingOpenedAt >= 0 && Now - _meetingOpenedAt >= 1;

        public PlayerTimeline For(byte playerId)
        {
            return playerId < TimelineCount ? _timelines[playerId] : _timelines[UnknownTimelineIndex];
        }

        public void NoteRoundStarted()
        {
            _roundStartedAt = Now;
            _meetingActive = false;
            _meetingOpenedAt = -1;

            foreach (var timeline in _timelines)
            {
                timeline.ResetRound();
            }
        }

        public void NoteRoundEnded()
        {
            _roundStartedAt = -1;
            _meetingActive = false;
            _meetingOpenedAt = -1;
        }

        public void NoteMeetingOpened()
        {
            _meetingActive = true;
            _meetingOpenedAt = Now;
        }

        public void NoteMeetingClosed()
        {
            _meetingActive = false;
            _meetingOpenedAt = -1;
        }
    }
}

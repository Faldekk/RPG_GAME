using System;

namespace RPG_GAME.Model
{
    public sealed class GameTimer
    {
        private readonly DateTime _startTime;
        private bool _isFrozen;

        public GameTimer()
        {
            _startTime = DateTime.UtcNow;
            _isFrozen = false;
        }

        public int ElapsedSeconds
        {
            get
            {
                if (_isFrozen)
                    return _frozenElapsedSeconds;

                var elapsed = DateTime.UtcNow - _startTime;
                return (int)elapsed.TotalSeconds;
            }
        }

        private int _frozenElapsedSeconds;

        public bool IsFrozen => _isFrozen;

        public void Freeze()
        {
            if (!_isFrozen)
            {
                _frozenElapsedSeconds = ElapsedSeconds;
                _isFrozen = true;
            }
        }

        public string FormatTime()
        {
            int seconds = ElapsedSeconds;
            int minutes = seconds / 60;
            int secs = seconds % 60;
            return $"{minutes:D2}:{secs:D2}";
        }
    }
}

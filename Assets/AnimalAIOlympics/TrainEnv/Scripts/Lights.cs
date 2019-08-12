using System;
using System.Collections;
using System.Collections.Generic;

namespace Lights
{
    public class InfiniteEnumerator : IEnumerator<int>
    {
        private int _initialValue = 0;
        private int _currentValue = 0;

        public InfiniteEnumerator()
        {
        }

        public InfiniteEnumerator(int initialValue)
        {
            _initialValue = initialValue;
            _currentValue = 0;
        }

        public bool MoveNext()
        {
            _currentValue += _initialValue;
            return true;
        }

        public void Reset()
        {
            _currentValue = 0;
        }

        public int Current
        {
            get
            {
                return _currentValue;
            }
        }

        object IEnumerator.Current
        {
            get
            {
                return Current;
            }
        }

        void IDisposable.Dispose() { }
    }

    public class LightsSwitch
    {
        private int _episodeLength = 0;
        private bool _lightStatus = true;
        private List<int> _blackouts = new List<int>();
        private IEnumerator<int> _blackoutsEnum;
        private int _nextFrameSwitch = -1;

        public LightsSwitch()
        {
            _blackoutsEnum = _blackouts.GetEnumerator();
        }

        public LightsSwitch(int episodeLength, List<int> blackouts)
        {
            _episodeLength = episodeLength;
            _blackouts = blackouts;
            if (_blackouts.Count > 0 && _blackouts[0] < 0)
            {
                _blackoutsEnum = new InfiniteEnumerator(-_blackouts[0]);
            }
            else
            {
                _blackoutsEnum = _blackouts.GetEnumerator();
            }
            Reset();
        }

        public void Reset()
        {
            _lightStatus = true;
            _blackoutsEnum.Reset();
            if (_blackoutsEnum.MoveNext())
            {
                _nextFrameSwitch = _blackoutsEnum.Current;
            }
            else
            {
                _nextFrameSwitch = -1;
            }
        }

        public bool LightStatus(int step, int agentDecisionInterval)
        {
            if (step == _nextFrameSwitch * agentDecisionInterval)
            {
                _lightStatus = !_lightStatus;
                if (_blackoutsEnum.MoveNext())
                {
                    _nextFrameSwitch = _blackoutsEnum.Current;
                }
            }
            return _lightStatus;
        }
    }
}
using System;

namespace Spect.Net.SpectrumEmu.Devices.Sound
{
    /// <summary>
    /// This class implements a Low-pass filter
    /// </summary>
    public class BandPassFilter
    {
        private readonly int _numTaps;
        private readonly double[] _taps;
        private readonly double[] _sr;

        public BandPassFilter(int numTaps, double freqSamplig, double freqLow, double freqHigh)
        {
            _numTaps = numTaps;
            _taps = new double[numTaps];
            _sr = new double[numTaps];
            var lambda = Math.PI * freqLow / (freqSamplig / 2);
            var phi = Math.PI * freqHigh / (freqSamplig / 2);
            for (var i = 0; i < _numTaps; i++) _sr[i] = 0.0;

            for (var n = 0; n < _numTaps; n++)
            {
                var mm = n - (_numTaps - 1.0) / 2.0;
                if (Math.Abs(mm) < double.Epsilon)
                {
                    _taps[n] = (phi - lambda) / Math.PI;
                }
                else
                {
                    _taps[n] = (Math.Sin(mm * phi) - Math.Sin(mm * lambda)) / (mm * Math.PI);
                }
            }
        }

        public double DoSample(double sample)
        {
            for (var i = _numTaps - 1; i >= 1; i--)
            {
                _sr[i] = _sr[i - 1];
            }
            _sr[0] = sample;

            double result = 0;
            for (var i = 0; i < _numTaps; i++) result += _sr[i] * _taps[i];
            return result / 3;
        }
    }
}
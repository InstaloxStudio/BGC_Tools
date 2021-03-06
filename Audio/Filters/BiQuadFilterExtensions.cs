﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BGC.Audio.Filters
{
    public static class SpecializedFilterExtensions
    {
        public static IBGCStream BiQuadBandpassFilter(
            this IBGCStream stream,
            float centralFrequency,
            double qFactor = double.NaN) =>
            BiQuadFilter.BandpassFilter(stream, centralFrequency, qFactor);

        public static IBGCStream BiQuadHighpassFilter(
            this IBGCStream stream,
            float criticalFrequency,
            double qFactor = double.NaN) =>
            BiQuadFilter.HighpassFilter(stream, criticalFrequency, qFactor);

        public static IBGCStream BiQuadLowpassFilter(
            this IBGCStream stream,
            float criticalFrequency,
            double qFactor = double.NaN) =>
            BiQuadFilter.LowpassFilter(stream, criticalFrequency, qFactor);

        public static IBGCStream BiQuadNotchFilter(
            this IBGCStream stream,
            float criticalFrequency,
            double qFactor = double.NaN) =>
            BiQuadFilter.NotchFilter(stream, criticalFrequency, qFactor);

        public static IBGCStream BiQuadLowShelfFilter(
            this IBGCStream stream,
            float criticalFrequency,
            double dbGain) =>
            BiQuadFilter.LowShelfFilter(stream, criticalFrequency, dbGain);

        public static IBGCStream BiQuadHighShelfFilter(
            this IBGCStream stream,
            float criticalFrequency,
            double dbGain) =>
            BiQuadFilter.HighShelfFilter(stream, criticalFrequency, dbGain);
    }
}

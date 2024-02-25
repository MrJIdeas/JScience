﻿using System.Numerics;
using JScience.Physik.Simulationen.Spins.Enums;
using JScience.Physik.Simulationen.Wavefunctions.Enums;
using JScience.Physik.Simulationen.Wavefunctions.VarTypes.StandardWF;
using System;
using System.Collections.Concurrent;
using System.Drawing;
using System.Threading.Tasks;

namespace JScience.Physik.Simulationen.Wavefunctions.Interfaces
{
    public interface IWavefunction
    {
        int Dimensions { get; }
        ELatticeBoundary Boundary { get; }

        WFInfo WFInfo { get; }

        OrderablePartitioner<Tuple<int, int>> rangePartitioner { get; }

        IWavefunction GetShift(EShift shift);

        void Clear();

        Image GetImage(int width, int height);

        #region Feld

        Complex this[int x] { get; }

        void SetField(int x, Complex value);

        IWavefunction Conj();

        IWavefunction Clone();

        #endregion Feld

        #region Norm

        double Norm();

        double getNorm(int x);

        #endregion Norm

        #region Operatoren WF-Complex

        public static IWavefunction operator +(Complex b, IWavefunction a)
        {
            Parallel.ForEach(a.rangePartitioner, (range, loopState) =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    a.SetField(i, a[i] + b);
            });
            return a;
        }

        public static IWavefunction operator -(Complex b, IWavefunction a)
        {
            Parallel.ForEach(a.rangePartitioner, (range, loopState) =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    a.SetField(i, a[i] - b);
            });
            return a;
        }

        public static IWavefunction operator *(Complex b, IWavefunction a)
        {
            Parallel.ForEach(a.rangePartitioner, (range, loopState) =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    a.SetField(i, a[i] * b);
            });
            return a;
        }

        public static IWavefunction operator +(IWavefunction a, Complex b) => b + a;

        public static IWavefunction operator -(IWavefunction a, Complex b) => b - a;

        public static IWavefunction operator *(IWavefunction a, Complex b) => b * a;

        public static IWavefunction operator /(IWavefunction a, Complex b)
        {
            Parallel.ForEach(a.rangePartitioner, (range, loopState) =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    a.SetField(i, a[i] / b);
            });
            return a;
        }

        #endregion Operatoren WF-Complex

        #region Operatoren WF-double

        public static IWavefunction operator +(double b, IWavefunction a)
        {
            IWavefunction c = a.Clone();
            Parallel.ForEach(a.rangePartitioner, (range, loopState) =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    a.SetField(i, a[i] + b);
            });
            return c;
        }

        public static IWavefunction operator -(double b, IWavefunction a)
        {
            IWavefunction c = a.Clone();
            Parallel.ForEach(a.rangePartitioner, (range, loopState) =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    a.SetField(i, a[i] - b);
            });
            return c;
        }

        public static IWavefunction operator *(double b, IWavefunction a)
        {
            IWavefunction c = a.Clone();
            Parallel.ForEach(a.rangePartitioner, (range, loopState) =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    a.SetField(i, a[i] * b);
            });
            return c;
        }

        public static IWavefunction operator +(IWavefunction a, double b) => b + a;

        public static IWavefunction operator -(IWavefunction a, double b) => b - a;

        public static IWavefunction operator *(IWavefunction a, double b) => b * a;

        public static IWavefunction operator /(IWavefunction a, double b)
        {
            IWavefunction c = a.Clone();
            Parallel.ForEach(a.rangePartitioner, (range, loopState) =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    a.SetField(i, a[i] / b);
            });
            return c;
        }

        #endregion Operatoren WF-double

        #region Operatoren WF-WF

        public static IWavefunction operator +(IWavefunction a, IWavefunction b)
        {
            if (!(a.Dimensions == b.Dimensions))
                throw new Exception("Error with Dimensions.");
            IWavefunction c = a.Clone();
            Parallel.ForEach(c.rangePartitioner, (range, loopState) =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    c.SetField(i, c[i] + b[i]);
            });
            return c;
        }

        public static IWavefunction operator -(IWavefunction a, IWavefunction b)
        {
            if (!(a.Dimensions == b.Dimensions))
                throw new Exception("Error with Dimensions.");
            IWavefunction c = a.Clone();
            Parallel.ForEach(c.rangePartitioner, (range, loopState) =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    c.SetField(i, c[i] - b[i]);
            });
            return c;
        }

        public static IWavefunction operator *(IWavefunction a, IWavefunction b)
        {
            if (!(a.Dimensions == b.Dimensions))
                throw new Exception("Error with Dimensions.");
            IWavefunction c = a.Clone();
            Parallel.ForEach(c.rangePartitioner, (range, loopState) =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    c.SetField(i, c[i] * b[i]);
            });
            return c;
        }

        #endregion Operatoren WF-WF
    }
}
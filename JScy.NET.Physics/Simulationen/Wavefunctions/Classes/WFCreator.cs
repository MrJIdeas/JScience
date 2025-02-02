﻿using System;
using System.IO;
using System.Numerics;
using JScy.NET.Enums;
using JScy.NET.Physics.Simulationen.Spins.Enums;
using JScy.NET.Physics.Simulationen.Wavefunctions.Enums;
using JScy.NET.Physics.Simulationen.Wavefunctions.Interfaces;
using JScy.NET.Physics.Simulationen.Wavefunctions.VarTypes.StandardWF;

namespace JScy.NET.Physics.Simulationen.Wavefunctions.Classes
{
    public static class WFCreator
    {
        private static T NormWave<T>(T wave) where T : IWavefunction
        {
            double norm = Math.Sqrt(wave.Norm());
            for (int i = 0; i < wave.field.Length; i++)
                wave.field[i] /= norm;
            return wave;
        }

        #region Free Electron

        /// <summary>
        /// Erstellt eine 1D Freie Welle
        /// </summary>
        /// <param name="k">Wellenzahl.</param>
        /// <param name="DimX">Anzahl Gitterplätze x</param>
        /// <param name="boundary">Randbedingungen.</param>
        /// <param name="CalcMethod">Kalkulationsmethode.</param>
        /// <returns>Wellenfunktion.</returns>
        public static IWF_1D CreateFreeWave(double k, int DimX, ELatticeBoundary boundary, ECalculationMethod CalcMethod)
        {
            WFInfo wfinfo = new(DimX, 1, 1, boundary, EWaveType.FreeWave, CalcMethod);
            wfinfo.AddAdditionalInfo("k", k);
            WF_1D erg = new(wfinfo);
            for (int i = 0; i < DimX; i++)
                erg.SetField(i, Complex.Exp(-Complex.ImaginaryOne * k * i));
            return NormWave(erg);
        }

        /// <summary>
        /// Erstellt eine 2D Freie Welle
        /// </summary>
        /// <param name="kx">Wellenzahl x.</param>
        /// <param name="ky">Wellenzahl y.</param>
        /// <param name="DimX">Anzahl Gitterplätze x</param>
        /// <param name="DimY">Anzahl Gitterplätze y</param>
        /// <param name="boundary">Randbedingungen.</param>
        /// <param name="CalcMethod">Kalkulationsmethode.</param>
        /// <returns>Wellenfunktion.</returns>
        public static IWF_2D CreateFreeWave(double kx, double ky, int DimX, int DimY, ELatticeBoundary boundary, ECalculationMethod CalcMethod)
        {
            WFInfo wfinfo = new(DimX, DimY, 1, boundary, EWaveType.FreeWave, CalcMethod);
            wfinfo.AddAdditionalInfo("kx", kx);
            wfinfo.AddAdditionalInfo("ky", ky);
            WF_2D erg = new(wfinfo);
            for (int i = 0; i < erg.field.Length; i++)
            {
                var tuple = erg.getCoordinatesXY(i);
                erg.SetField(tuple.Item1, tuple.Item2, Complex.Exp(-Complex.ImaginaryOne * (kx * tuple.Item1 + ky * tuple.Item2)));
            }
            return NormWave(erg);
        }

        #endregion Free Electron

        #region Gauß

        /// <summary>
        /// Erstellt eine 1D Gaußsche Welle.
        /// </summary>
        /// <param name="k">Wellenzahl x.</param>
        /// <param name="DimX">Anzahl Gitterplätze x</param>
        /// <param name="StartX">Startwert x</param>
        /// <param name="sigma">Breite Gaußkurve x.</param>
        /// <param name="boundary">Randbedingungen.</param>
        /// <param name="CalcMethod">Kalkulationsmethode.</param>
        /// <returns>Wellenfunktion.</returns>
        public static IWF_1D CreateGaußWave(double k, double sigma, int DimX, int StartX, ELatticeBoundary boundary, ECalculationMethod CalcMethod)
        {
            WFInfo wfinfo = new(DimX, 1, 1, boundary, EWaveType.Gauß, CalcMethod);
            wfinfo.AddAdditionalInfo("k", k);
            wfinfo.AddAdditionalInfo("sigma", sigma);
            wfinfo.AddAdditionalInfo("startX", StartX);
            WF_1D erg = new(wfinfo);
            for (int i = 0; i < DimX; i++)
                erg.SetField(i, Complex.Exp(new Complex(-Math.Pow(i - StartX, 2) / (4 * Math.Pow(sigma, 2)), 0)));
            return (IWF_1D)(NormWave(erg * CreateFreeWave(k, DimX, boundary, CalcMethod)));
        }

        /// <summary>
        /// Erstellt eine 2D Gaußsche Welle
        /// </summary>
        /// <param name="kx">Wellenzahl x.</param>
        /// <param name="ky">Wellenzahl y.</param>
        /// <param name="DimX">Anzahl Gitterplätze x</param>
        /// <param name="DimY">Anzahl Gitterplätze y</param>
        /// <param name="sigmaX">Breite Gaußkurve x.</param>
        /// <param name="sigmaY">Breite Gaußkurve y.</param>
        /// <param name="StartX">Startwert x</param>
        /// <param name="StartY">Startwert y</param>
        /// <param name="boundary">Randbedingungen.</param>
        /// <param name="CalcMethod">Kalkulationsmethode.</param>
        /// <returns>Wellenfunktion.</returns>
        public static IWF_2D CreateGaußWave(double kx, double ky, double sigmaX, double sigmaY, int DimX, int DimY, int StartX, int StartY, ELatticeBoundary boundary, ECalculationMethod CalcMethod)
        {
            WFInfo wfinfo = new(DimX, DimY, 1, boundary, EWaveType.Gauß, CalcMethod);
            wfinfo.AddAdditionalInfo("kx", kx);
            wfinfo.AddAdditionalInfo("ky", ky);
            wfinfo.AddAdditionalInfo("sigmaX", sigmaX);
            wfinfo.AddAdditionalInfo("sigmaY", sigmaY);
            wfinfo.AddAdditionalInfo("startX", StartX);
            wfinfo.AddAdditionalInfo("startY", StartY);
            WF_2D erg = new(wfinfo);
            for (int i = 0; i < erg.field.Length; i++)
            {
                var tuple = erg.getCoordinatesXY(i);
                double exponentX = -Math.Pow(tuple.Item1 - StartX, 2) / sigmaX;
                double exponentY = -Math.Pow(tuple.Item2 - StartY, 2) / sigmaY;
                // Gaußscher Faktor (Amplitudenwert)
                double amplitude = Math.Exp(exponentX + exponentY);
                // Speichern als komplexer Wert (z.B. ohne Phase oder mit Phase)
                Complex value = new Complex(amplitude, 0); // Reine Realteil-Wellenpaket
                erg.SetField(i, value);
            }
            return (IWF_2D)NormWave(erg * CreateFreeWave(kx, ky, DimX, DimY, boundary, CalcMethod));
        }

        #endregion Gauß

        #region Delta

        /// <summary>
        /// Erstellt eine 1D Delta Peak.
        /// </summary>
        /// <param name="DimX">Anzahl Gitterplätze x</param>
        /// <param name="StartX">Startwert x</param>
        /// <param name="boundary">Randbedingungen.</param>
        /// <param name="CalcMethod">Kalkulationsmethode.</param>
        /// <returns>Wellenfunktion.</returns>
        public static IWF_1D CreateDelta(int DimX, int StartX, ELatticeBoundary boundary, ECalculationMethod CalcMethod)
        {
            WFInfo wfinfo = new(DimX, 1, 1, boundary, EWaveType.Delta, CalcMethod);
            wfinfo.AddAdditionalInfo("startX", StartX);
            WF_1D erg = new(wfinfo);
            erg.SetField(StartX, Complex.One);
            return NormWave(erg);
        }

        /// <summary>
        /// Erstellt einen 2D Delta-Peak.
        /// </summary>
        /// <param name="DimX">Anzahl Gitterplätze x</param>
        /// <param name="DimY">Anzahl Gitterplätze y</param>
        /// <param name="StartX">Startwert x</param>
        /// <param name="StartY">Startwert y</param>
        /// <param name="boundary">Randbedingungen.</param>
        /// <param name="CalcMethod">Kalkulationsmethode.</param>
        /// <returns>Wellenfunktion.</returns>
        public static IWF_2D CreateDelta(int DimX, int DimY, int StartX, int StartY, ELatticeBoundary boundary, ECalculationMethod CalcMethod)
        {
            WFInfo wfinfo = new(DimX, DimY, 1, boundary, EWaveType.Delta, CalcMethod);
            wfinfo.AddAdditionalInfo("startX", StartX);
            wfinfo.AddAdditionalInfo("startY", StartY);
            WF_2D erg = new(wfinfo);
            erg.SetField(StartX, StartY, Complex.One);
            return NormWave(erg);
        }

        #endregion Delta

        #region From File

        /// <summary>
        /// Erstellt eine Wellenfunktion anhand einer Datei.
        /// </summary>
        /// <param name="FilePath">Dateipfad.</param>
        /// <param name="Delimiter">Trennzeichen für Real-/Imaginärteil.</param>
        /// <param name="boundary">Randbedingungen.</param>
        /// <param name="CalcMethod">Kalulationsmethode.</param>
        /// <returns>Wellenfunktion.</returns>
        /// <exception cref="FileNotFoundException">Wenn Datei nicht gefunden wird.</exception>
        /// <exception cref="ArgumentException">Falsches Trennzeichen?</exception>
        public static IWF_1D FromFile1D(string FilePath, char Delimiter, ELatticeBoundary boundary, ECalculationMethod CalcMethod)
        {
            if (!File.Exists(FilePath))
                throw new FileNotFoundException("Invalid Path for Wavefunctíon File.");
            var lines = File.ReadAllLines(FilePath);
            WFInfo wfinfo = new(lines.Length, 1, 1, boundary, EWaveType.Custom, CalcMethod);
            WF_1D erg = new(wfinfo);
            for (int i = 0; i < lines.Length; i++)
            {
                var parts = lines[i].Split(Delimiter);
                if (parts.Length != 2)
                    throw new ArgumentException("Invalid Arguments for Wavefunction (2 Elements per Row Real+Imaginary).");
                erg.SetField(i, new Complex(double.Parse(parts[0]), double.Parse(parts[1])));
            }
            return erg;
        }

        /// <summary>
        /// Erstellt eine Wellenfunktion anhand einer Datei.
        /// </summary>
        /// <param name="FilePath">Dateipfad.</param>
        /// <param name="Delimiter">Trennzeichen für Real-/Imaginärteil.</param>
        /// <param name="boundary">Randbedingungen.</param>
        /// <param name="CalcMethod">Kalulationsmethode.</param>
        /// <returns>Wellenfunktion.</returns>
        /// <exception cref="FileNotFoundException">Wenn Datei nicht gefunden wird.</exception>
        /// <exception cref="ArgumentException">Falsches Trennzeichen?</exception>
        public static IWF_2D FromFile2D(string FilePath, char Delimiter, ELatticeBoundary boundary, ECalculationMethod CalcMethod)
        {
            if (!File.Exists(FilePath))
                throw new FileNotFoundException("Invalid Path for Wavefunctíon File.");
            var lines = File.ReadAllLines(FilePath);
            WFInfo wfinfo = new(lines.Length, lines[0].Length, 1, boundary, EWaveType.Custom, CalcMethod);
            WF_2D erg = new(wfinfo);
            for (int i = 0; i < lines.Length; i++)
            {
                var parts = lines[i].Split(Delimiter);
                if (parts.Length % 2 != 0)
                    throw new ArgumentException("Invalid Arguments for Wavefunction (2 Elements per Row Real+Imaginary).");
                for (int j = 0; j < parts.Length; j += 2)
                    erg.SetField(i, j, new Complex(double.Parse(parts[j]), double.Parse(parts[j + 1])));
            }
            return erg;
        }

        #endregion From File
    }
}
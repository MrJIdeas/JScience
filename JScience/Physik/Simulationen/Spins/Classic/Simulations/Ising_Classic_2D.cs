﻿using JScience.Physik.Enums;
using JScience.Physik.Simulationen.Spins.Enums;

namespace JScience.Physik.Simulationen.Spins.Classic.Simulations
{
    public class Ising_Classic_2D : Ising_Classic_3D
    {
        protected override string CONST_FNAME => "ISING_2D_CLASSIC";

        public Ising_Classic_2D(double j, double b, double t, uint dimX, uint dimY, EParticleType types, ELatticeBoundary boundary, uint StepsPerSaving) :
            base(j, b, t, dimX, dimY, 1, types, boundary, StepsPerSaving)
        {
        }
    }
}
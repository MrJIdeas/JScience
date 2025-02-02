﻿using System;
using JScy.NET.Physics.Enums;
using JScy.NET.Physics.Simulationen.Spins.Classic.Interfaces;
using JScy.NET.Physics.Simulationen.Spins.Enums;

namespace JScy.NET.Physics.Simulationen.Spins.Classic.VarTypes
{
    public class Spin_Heisenberg_Classic : Spin_Classic, IEquatable<ISpin_Classic>
    {
        public double Spin_X { get; private set; }

        public double Spin_Y { get; private set; }

        public double Spin_Z { get; private set; }

        public Spin_Heisenberg_Classic(EParticleType eParticleType, uint x, uint y, uint z) : base(x, y, z, eParticleType)
        {
            Spin_Z = 1;
            spinType = ESpinType.Heisenberg;
        }

        public override double getComponent(uint index)
        {
            switch (index)
            {
                case 0:
                    return Spin * Spin_X;

                case 1:
                    return Spin * Spin_Y;

                case 2:
                    return Spin * Spin_Z;

                default:
                    throw new Exception("No Valid Component Index");
            }
        }

        public override void Flip()
        {
            Spin_X *= -1;
            Spin_Y *= -1;
            Spin_Z *= -1;
        }

        public void Random()
        {
            Random rand = new Random();
            var phi = rand.NextDouble() * 360;
            var theta = rand.NextDouble() * 180;
            Spin_X = Math.Sin(theta) * Math.Cos(phi);
            Spin_Y = Math.Sin(theta) * Math.Sin(phi);
            Spin_Z = Math.Cos(theta);
        }
    }
}
﻿using System;
using System.ComponentModel;

namespace VVSAssistant.Models.DataSheets
{
    [DisplayName(@"Datablad for solpanel")]
    internal class SolarCollectorDataSheet : DataSheet
    {
        [DisplayName(@"Areal")]
        [Description(@"Arealet for en enkelt solfanger i kvadratmeter")]
        public float Area { get; set; }
        [DisplayName(@"Solfangereffektivitet")]
        [Description(@"Solfangereeffektivitet i procent")]
        public float Efficency { get; set; }
        // Zero loss efficiency
        [DisplayName(@"Effektivitet ved nulbelastning")]
        public float N0 { get; set; }
        // First order heat loss coll
        [DisplayName(@"1. Ordens koefficient")]// w/(m^2 K)
        [Description(@"Værdi i W/(m\u00b2 K)")]
        public float a1 { get; set; }
        // Second order heat loss coll
        [DisplayName(@"2. Ordens koefficient")]
        [Description(@"Værdi i W/(m\u00b2 K\u00b2)")]
        public float a2 { get; set; }
        // Incidence Angle Modifier 
        [DisplayName(@"Indfaldsvinkel korrektionsfaktor")]
        public float IAM { get; set; }

        public override string ToString()
        {
            var n0String = "";
            var a1String = "";
            var a2String = "";
            var iamString = "";
            if (Math.Abs(N0) > 0)
                n0String = $", Effektivitet ved nulbelastning: {N0}";
            if (Math.Abs(a1) > 0)
                a1String = $", 1. Ordens koefficient: {a1} W/(m\u00b2 K)";
            if (Math.Abs(a2) > 0)
                a2String = $", 2. Ordens koefficient: {a2}";
            if (Math.Abs(IAM) > 0)
                iamString = $", Indfaldsvinkel korrektionsfaktor: {IAM}";

            return $"Areal: {Area}m\u00b2, Energieffektivitet: {Efficency}%{n0String}{a1String}{a2String}{iamString}";
        }
    }
}

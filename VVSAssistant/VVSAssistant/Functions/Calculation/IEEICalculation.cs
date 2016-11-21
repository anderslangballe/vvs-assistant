﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VVSAssistant.Models;

namespace VVSAssistant.Functions.Calculation
{
    public interface IEEICalculation
    {
        EEICalculationResult CalculateEEI(PackagedSolution PackagedSolutionForCalculation);
    }
}
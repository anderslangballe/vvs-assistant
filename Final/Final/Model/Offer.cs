﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Final.Model
{
    public class Offer
    {
        public virtual PackagedSolution PackagedSolution { get; set; }
        public virtual Client Client { get; set; }
        public virtual OfferInformation OfferInformation { get; set; }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bug_tracker.Models.Classes
{
    public class TicketPriority
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<Tickets> Tickets { get; set; }
    }
}
﻿using System;
using Votus.Core.Infrastructure.EventSourcing;

namespace Votus.Core.Tasks
{
    public class TaskCreatedEvent : AggregateRootEvent
    {
        public string   Title           { get; set; }
        public Guid     InitialIdeaId   { get; set; }
    }
}

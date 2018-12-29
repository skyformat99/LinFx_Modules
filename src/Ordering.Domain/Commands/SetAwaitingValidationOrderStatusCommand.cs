﻿using MediatR;
using System.Runtime.Serialization;

namespace Ordering.Domain.Commands
{
    public class SetAwaitingValidationOrderStatusCommand : IRequest<bool>
    {
        [DataMember]
        public int OrderNumber { get; private set; }

        public SetAwaitingValidationOrderStatusCommand(int orderNumber)
        {
            OrderNumber = orderNumber;
        }
    }
}
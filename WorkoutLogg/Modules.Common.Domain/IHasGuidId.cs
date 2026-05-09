using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Common.Domain
{
    public interface IHasGuidId
    {
        public Guid Id { get; set; }
    }
}

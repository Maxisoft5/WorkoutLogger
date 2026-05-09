using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Common.Domain
{
    public interface IHasNumberId
    {
        public long Id { get; set; }
    }
}

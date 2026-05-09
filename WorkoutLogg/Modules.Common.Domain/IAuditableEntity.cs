using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Common.Domain
{
    public interface IAuditableEntity
    {
        DateTime CreatedAtUtc { get; set; }

        DateTime? UpdatedAtUtc { get; set; }
    }
}

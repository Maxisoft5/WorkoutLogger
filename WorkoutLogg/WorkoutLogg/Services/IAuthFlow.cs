using System;
using System.Collections.Generic;
using System.Text;

namespace WorkoutLogg.Services
{
    public interface IAuthFlow
    {
        Task<bool> TryRefreshAsync(CancellationToken ct);
        void SignOutAndRedirectToLogin();
    }
}

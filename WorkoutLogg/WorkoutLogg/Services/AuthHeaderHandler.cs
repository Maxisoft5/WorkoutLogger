using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace WorkoutLogg.Services
{
    public class AuthHeaderHandler : DelegatingHandler
    {
        private static readonly SemaphoreSlim _refreshLock = new(1, 1);

     
        private readonly IAuthFlow _authFlow;

        public AuthHeaderHandler(IAuthFlow authFlow)
        {
            _authFlow = authFlow;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken ct)
        {
            // не трогаем публичные эндпоинты
            var path = request.RequestUri?.AbsolutePath ?? "";
            var isPublic = path.Contains("/Auth/Login")
                        || path.Contains("/Auth/CreateAccount");

            var tokenUsed = await LoginService.GetActiveToken();
            if (!isPublic && !string.IsNullOrEmpty(tokenUsed))
            {
                request.Headers.Authorization =
                    new AuthenticationHeaderValue("Bearer", tokenUsed);
            }

            var response = await base.SendAsync(request, ct);

            if (response.StatusCode != HttpStatusCode.Unauthorized || isPublic)
                return response;

            // Возможно, пока мы ждали ответ, другой запрос уже обновил токен.
            // Сначала забираем lock, потом проверяем.
            await _refreshLock.WaitAsync(ct);
            bool refreshed;
            try
            {
                var currentToken = await LoginService.GetActiveToken();
                if (currentToken != tokenUsed && !string.IsNullOrEmpty(currentToken))
                {
                    // Токен уже обновили — просто ретраим
                    refreshed = true;
                }
                else
                {
                    refreshed = await _authFlow.TryRefreshAsync(ct);
                }
            }
            finally
            {
                _refreshLock.Release();
            }

            if (!refreshed)
            {
                _authFlow.SignOutAndRedirectToLogin();
                return response;
            }

            response.Dispose();
            var retry = await CloneRequestAsync(request);
            var newToken = await LoginService.GetActiveToken();
            retry.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", newToken);
            return await base.SendAsync(retry, ct);
        }

        private static async Task<HttpRequestMessage> CloneRequestAsync(HttpRequestMessage req)
        {
            var clone = new HttpRequestMessage(req.Method, req.RequestUri) { Version = req.Version };

            if (req.Content != null)
            {
                var ms = new MemoryStream();
                await req.Content.CopyToAsync(ms);
                ms.Position = 0;
                clone.Content = new StreamContent(ms);
                foreach (var h in req.Content.Headers)
                    clone.Content.Headers.TryAddWithoutValidation(h.Key, h.Value);
            }

            foreach (var h in req.Headers)
                if (!h.Key.Equals("Authorization", StringComparison.OrdinalIgnoreCase))
                    clone.Headers.TryAddWithoutValidation(h.Key, h.Value);

            return clone;
        }
    }
}

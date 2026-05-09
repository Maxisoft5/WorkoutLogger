using Grpc.Core;
using Grpc.Net.Client;
using System;
using System.Collections.Generic;
using System.Text;
using WorkoutLogger.Grpc.Contracts;

namespace WorkoutLogg.Services
{
    public class ExercisesGrpcClient : IDisposable
    {
        private readonly GrpcChannel _channel;
        private readonly ExercisesService.ExercisesServiceClient _client;

        public ExercisesGrpcClient(string baseAddress)
        {
            var handler = new HttpClientHandler();
            // На время разработки доверяем dev-сертификату
#if DEBUG
            handler.ServerCertificateCustomValidationCallback =
                HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
#endif

            _channel = GrpcChannel.ForAddress(baseAddress, new GrpcChannelOptions
            {
                HttpHandler = handler
            });
            _client = new ExercisesService.ExercisesServiceClient(_channel);
        }

        public async Task<ExerciseDto> GetAsync(string id, CancellationToken ct = default)
        {
            return await _client.GetExerciseAsync(new GetExerciseRequest { Id = id }, cancellationToken: ct);
        }

        public async IAsyncEnumerable<ExerciseDto> StreamAsync(
            string? muscleGroup = null,
            [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken ct = default)
        {
            using var call = _client.StreamExercises(
                new StreamExercisesRequest { MuscleGroup = muscleGroup ?? "" },
                cancellationToken: ct);

            await foreach (var exercise in call.ResponseStream.ReadAllAsync(ct))
            {
                yield return exercise;
            }
        }

        public void Dispose() => _channel.Dispose();
    }
}

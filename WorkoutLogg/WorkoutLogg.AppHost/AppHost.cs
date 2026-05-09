var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.WorkoutLogg>("workoutlogg");

builder.AddProject<Projects.WorkoutLogger_WebApi>("workoutlogger-webapi");

builder.AddProject<Projects.WorkoutLogger_EventsConsumer>("workoutlogger-eventsconsumer");

builder.Build().Run();

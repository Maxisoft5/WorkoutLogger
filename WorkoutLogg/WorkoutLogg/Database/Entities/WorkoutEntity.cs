using Moduels.Workouts.DTO.Enums;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace WorkoutLogg.Database.Entities
{
    public class WorkoutEntity
    {
        [PrimaryKey]
        public Guid Id { get; set; }

        public WorkoutType MuscleGroup { get; set; }

        [Indexed]
        public DateTime StartDate { get; set; }
        [Indexed]
        public DateTime EndDate { get; set; }

        // Серверный Guid — чтобы синхронизировать с бэкендом
        [Indexed]
        public Guid RemoteId { get; set; }

        // Флаг "ещё не отправлено на сервер" — для офлайн-режима
        public bool IsSynced { get; set; }

        // Навигацию НЕ храним в БД — собираем в коде
        [Ignore]
        public List<WorkoutSetEntity> Exercises { get; set; } = new();
    }
}

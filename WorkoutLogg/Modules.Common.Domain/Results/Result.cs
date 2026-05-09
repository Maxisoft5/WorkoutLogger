using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Modules.Common.Domain.Results
{
    public class Result<TValue> : IResult<TValue>
    {
        private readonly TValue? _value = default;

        [JsonConstructor]
        public Result(TValue value, List<Error> errors)
        {
            _value = value;
            Errors = errors;
        }

        public Result(TValue value)
        {
            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            _value = value;
        }

        public Result(Error error)
        {
            Errors = [error];
        }

  
        public Result(List<Error> errors)
        {
            Errors = errors;
        }

        /// <summary>
        /// Gets a value indicating whether the state is a success.
        /// </summary>
        public bool IsSuccess => Errors is null or [];

        /// <summary>
        /// Gets a value indicating whether the state is error.
        /// </summary>
        public bool IsError => Errors.Count > 0;

        /// <summary>
        /// Gets the collection of errors.
        /// </summary>
        public List<Error> Errors { get; set; } = [];

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when no value is present.</exception>
        public TValue? Value
        {
            get
            {
                return _value;
            }
        }

        /// <summary>
        /// Gets the first error.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when no errors are present.</exception>
        public Error FirstError
        {
            get
            {
                if (Errors == null || Errors.Count == 0) return null;
                return Errors[0];
            }
        }
    }
}

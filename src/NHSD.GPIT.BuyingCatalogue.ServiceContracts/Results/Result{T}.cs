using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Errors;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Results
{
    public sealed class Result<T> : IEquatable<Result<T>>
    {
        internal Result(bool isSuccess, IEnumerable<ErrorDetails> errors, T value)
        {
            IsSuccess = isSuccess;
            Errors = new ReadOnlyCollection<ErrorDetails>(errors?.ToList() ?? new List<ErrorDetails>());
            Value = value;
        }

        public bool IsSuccess { get; }

        public IReadOnlyCollection<ErrorDetails> Errors { get; }

        [MaybeNull]
        public T Value { get; }

        public Result ToResult()
        {
            return IsSuccess ? Result.Success() : Result.Failure(Errors);
        }

        public bool Equals(Result<T> other)
        {
            if (other is null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return IsSuccess == other.IsSuccess
                && AreErrorsEqual(Errors, other.Errors)
                && Equals(Value, other.Value);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Result<T>);
        }

        public override int GetHashCode() => HashCode.Combine(IsSuccess, Errors, Value!);

        private static bool AreErrorsEqual(IEnumerable<ErrorDetails> first, IEnumerable<ErrorDetails> second)
        {
            return first.SequenceEqual(second);
        }
    }
}

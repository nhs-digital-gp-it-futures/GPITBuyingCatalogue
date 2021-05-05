using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Results
{
    public sealed class Result : IEquatable<Result>
    {
        private static readonly Result SuccessfulResult = new();

        private Result()
            : this(true, Enumerable.Empty<ErrorDetails>())
        {
        }

        private Result(IEnumerable<ErrorDetails> errors)
            : this(false, errors)
        {
        }

        private Result(bool isSuccess, IEnumerable<ErrorDetails>? errors)
        {
            IsSuccess = isSuccess;
            Errors = new ReadOnlyCollection<ErrorDetails>(errors?.ToList() ?? new List<ErrorDetails>());
        }

        public bool IsSuccess { get; }

        public IReadOnlyCollection<ErrorDetails> Errors { get; }

        public static Result Success()
        {
            return SuccessfulResult;
        }

        public static Result<T> Success<T>(T value)
        {
            return new(true, new List<ErrorDetails>(), value);
        }

        public static Result Failure(IEnumerable<ErrorDetails> errors)
        {
            return new(errors);
        }

        public static Result Failure(params ErrorDetails[] errors)
        {
            return new(errors);
        }

        public static Result<T> Failure<T>(params ErrorDetails[] errors)
        {
            return new(false, errors, default!);
        }

        public static Result<T> Failure<T>(IEnumerable<ErrorDetails> errors)
        {
            return new(false, errors, default!);
        }

        public bool Equals(Result? other)
        {
            if (other is null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return IsSuccess == other.IsSuccess
                && AreErrorsEqual(Errors, other.Errors);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as Result);
        }

        public override int GetHashCode() => HashCode.Combine(IsSuccess, Errors);

        private static bool AreErrorsEqual(IEnumerable<ErrorDetails> first, IEnumerable<ErrorDetails> second)
        {
            return first.SequenceEqual(second);
        }
    }

    // MJRTODO - own class
    public sealed class Result<T> : IEquatable<Result<T>>
    {
        internal Result(bool isSuccess, IEnumerable<ErrorDetails>? errors, T value)
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

        public bool Equals(Result<T>? other)
        {
            if (other is null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return IsSuccess == other.IsSuccess
                && AreErrorsEqual(Errors, other.Errors)
                && Equals(Value, other.Value);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as Result<T>);
        }

        public override int GetHashCode() => HashCode.Combine(IsSuccess, Errors, Value!);

        private static bool AreErrorsEqual(IEnumerable<ErrorDetails> first, IEnumerable<ErrorDetails> second)
        {
            return first.SequenceEqual(second);
        }
    }

    // MJRTODO - Own class
    public sealed class ErrorDetails : IEquatable<ErrorDetails>
    {
        public ErrorDetails(string id, string? field = null)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Field = field;
        }

        public string Id { get; }

        public string? Field { get; }

        public bool Equals(ErrorDetails? other)
        {
            if (other is null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return string.Equals(Id, other.Id, StringComparison.Ordinal)
                && string.Equals(Field, other.Field, StringComparison.Ordinal);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as ErrorDetails);
        }

        public override int GetHashCode() => HashCode.Combine(Id, Field);
    }
}

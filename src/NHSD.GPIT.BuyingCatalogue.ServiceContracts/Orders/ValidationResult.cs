using System;
using System.Collections.Generic;
using System.Linq;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders
{
    public sealed class ValidationResult
    {
        public ValidationResult()
            : this(Array.Empty<ErrorDetails>())
        {
        }

        public ValidationResult(ErrorDetails error)
            : this(new[] { error })
        {
        }

        public ValidationResult(IReadOnlyList<ErrorDetails> errors)
        {
            Errors = errors;
        }

        public ValidationResult(params ValidationResult[] results)
        {
            Errors = results.SelectMany(r => r.Errors).ToList();
        }

        public IReadOnlyList<ErrorDetails> Errors { get; }

        public bool Success => Errors.Count == 0;
    }
}

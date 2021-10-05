using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace NHSD.GPIT.BuyingCatalogue.Test.Framework.SharedMocks
{
    public sealed class MockHttpSession : ISession
    {
        private readonly Dictionary<string, byte[]> sessionStorage = new Dictionary<string, byte[]>();

        public string Id
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsAvailable
        {
            get { throw new NotImplementedException(); }
        }

        public IEnumerable<string> Keys
        {
            get { return sessionStorage.Keys; }
        }

        public Task CommitAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task LoadAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            sessionStorage.Clear();
        }

        public void Remove(string key)
        {
            sessionStorage.Remove(key);
        }

        public void Set(string key, byte[] value)
        {
            sessionStorage[key] = value;
        }

        public bool TryGetValue(string key, out byte[] value) => sessionStorage.TryGetValue(key, out value);
    }
}

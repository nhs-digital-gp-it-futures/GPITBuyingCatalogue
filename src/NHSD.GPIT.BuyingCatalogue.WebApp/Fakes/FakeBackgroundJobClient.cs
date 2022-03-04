using Hangfire;
using Hangfire.Common;
using Hangfire.States;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Fakes
{
    public class FakeBackgroundJobClient : IBackgroundJobClient
    {
        public string Create(Job job, IState state) => string.Empty;

        public bool ChangeState(string jobId, IState state, string expectedState) => true;
    }
}

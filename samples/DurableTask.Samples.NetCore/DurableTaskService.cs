using System.Threading;
using System.Threading.Tasks;
using DurableTask.Core;
using Microsoft.Extensions.Hosting;

namespace DurableTask.Samples.NetCore
{
    public class DurableTaskService : IHostedService
    {
        private readonly TaskHubWorker _taskHubWorker;

        public DurableTaskService(TaskHubWorker taskHubWorker)
        {
            _taskHubWorker = taskHubWorker;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _taskHubWorker.StartAsync().ConfigureAwait(false);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _taskHubWorker.StopAsync().ConfigureAwait(false);
        }
    }
}
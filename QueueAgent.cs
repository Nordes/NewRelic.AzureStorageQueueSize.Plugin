using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using NewRelic.Platform.Sdk;
using ScalableBytes.NewRelic.AzureStorageQueueSize.Plugin.Models;
using Serilog.Core;

namespace ScalableBytes.NewRelic.AzureStorageQueueSize.Plugin
{
    /// <summary>
    /// Plugin Agent in order to monitor storage account.
    /// </summary>
    /// <remarks>
    /// One Agent instance per Storage Account (<see cref="https://github.com/newrelic-platform/newrelic_dotnet_sdk"/> and 
    /// <seealso cref="https://docs.newrelic.com/docs/plugins/developing-plugins/writing-code/using-net-sdk"/>)
    /// </remarks>
    public class QueueAgent : Agent
    {
        private readonly Logger _eventLogLogger;
        private readonly SystemConfiguration _systemConfiguration;

        /// <summary>
        /// Plugin Guid
        /// </summary>

#if DEBUG
        public override string Guid { get { return "com.scalablebytes.newrelic.azurestoragequeuesizedev"; } }
#else
        public override string Guid { get { return "com.scalablebytes.newrelic.azurestoragequeuesize"; } }
#endif

        /// <summary>
        /// Return the assembly version
        /// </summary>
        public override string Version { get { return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString(3); } }

        /// <summary>
        /// Queue Agent
        /// </summary>
        /// <param name="systemConfiguration">The system configuration.</param>
        /// <param name="eventLogLogger">The event log logger.</param>
        public QueueAgent(SystemConfiguration systemConfiguration, Logger eventLogLogger)
        {
            _systemConfiguration = systemConfiguration;
            _eventLogLogger = eventLogLogger;
        }

        /// <summary>
        /// Returns a human-readable string to differentiate different hosts/entities in the site UI
        /// </summary>
        /// <returns>The current system agent name</returns>
        public override string GetAgentName()
        {
            return _systemConfiguration.Name;
        }

        /// <summary>
        /// This is where logic for fetching and reporting metrics should exist.
        /// Call off to a REST head, SQL DB, virtually anything you can programmatically
        /// get metrics from and then call ReportMetric.
        /// </summary>
        public override void PollCycle()
        {
            foreach (var storageAccountConfig in _systemConfiguration.StorageAccounts)
            {
                var config = storageAccountConfig;
                PollStorageAccount(config);
            }
        }

        /// <summary>
        /// Polls the storage account asynchronous.
        /// </summary>
        /// <param name="config">The configuration.</param>
        private void PollStorageAccount(StorageAccount config)
        {
            var storageAccount = CloudStorageAccount.Parse(config.ConnectionString);
            var queueClient = storageAccount.CreateCloudQueueClient();

            var continuationToken = new QueueContinuationToken();

            while (continuationToken != null)
            {
                var listResponse = queueClient.ListQueuesSegmented(continuationToken);

                // We must ask Azure for the size of each queue individually.
                // This can be done in parallel.
                foreach (var queue in listResponse.Results)
                {
                    queue.FetchAttributes();
                }

                // ReportMetric is not thread-safe, so we can't call it in the parallel
                foreach (var queue in listResponse.Results)
                {
                    var approximateMessageCount = queue.ApproximateMessageCount ?? 0;

                    // No groups, then just send and continue.
                    var metricName = string.Format("Queues/{0}/all/{1}/size", config.Name, queue.Name);
                    ReportMetric(metricName, "messages", approximateMessageCount);

                    if (config.Groups != null)
                    {
                        // Send the data to the proper group.
                        foreach (var storageQueueGroup in config.Groups)
                        {
                            if (storageQueueGroup.AllowedInGroup(queue.Name))
                            {
                                metricName = string.Format("Queues/{0}/groups/{1}/{2}/size", config.Name, storageQueueGroup.Name, queue.Name);
                                ReportMetric(metricName, "messages", approximateMessageCount);
                            }
                        }
                    }
                }

                continuationToken = listResponse.ContinuationToken;
            }
        }
    }
}

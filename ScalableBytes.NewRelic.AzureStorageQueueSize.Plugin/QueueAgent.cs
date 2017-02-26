using System;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using NewRelic.Platform.Sdk;
using ScalableBytes.NewRelic.AzureStorageQueueSize.Plugin.Models;
using System.Linq;
using Serilog;

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
        private readonly ILogger _eventLogLogger;
        private readonly SystemConfiguration _systemConfiguration;

        /// <summary>
        /// Plugin Guid
        /// </summary>
        /// <remarks>
        /// As proposed by NewRelic, if we are in debug we should use a different guid. Since we don't want that
        /// Guid to be in the App.Config, here's the trick.
        /// </remarks>
#if DEBUG
        public override string Guid => "com.scalablebytes.newrelic.azurestoragequeuesizedev";
#else
        public override string Guid => "com.scalablebytes.newrelic.azurestoragequeuesize";
#endif

        /// <summary>
        /// Return the assembly version
        /// </summary>
        public override string Version => System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString(3);

        /// <summary>
        /// Queue Agent
        /// </summary>
        /// <param name="systemConfiguration">The system configuration.</param>
        /// <param name="eventLogLogger">The event log logger.</param>
        public QueueAgent(SystemConfiguration systemConfiguration, ILogger eventLogLogger)
        {
            ValidateAgentConfiguration(systemConfiguration);

            _systemConfiguration = systemConfiguration;
            _eventLogLogger = eventLogLogger;
        }

        private void ValidateAgentConfiguration(SystemConfiguration systemConfiguration)
        {
            if (systemConfiguration == null)
                throw new ArgumentNullException(nameof(systemConfiguration), "The systemConfiguration must be specified for the agent to initialize");
            if (string.IsNullOrEmpty(systemConfiguration.Name))
                throw new ArgumentNullException(nameof(systemConfiguration.Name), "The system name must be specified for the agent to initialize");
            if (systemConfiguration.StorageAccounts == null || !systemConfiguration.StorageAccounts.Any())
                throw new ArgumentNullException(nameof(systemConfiguration.StorageAccounts), "The system have no storage accounts set and it must be specified for the agent to initialize");
            foreach (var storageAccount in systemConfiguration.StorageAccounts)
            {
                if (string.IsNullOrEmpty(storageAccount.Name))
                    throw new ArgumentNullException(nameof(storageAccount.Name), "The name of the storage account must be specified for the agent to initialize");
                if (string.IsNullOrEmpty(storageAccount.ConnectionString))
                    throw new ArgumentNullException(nameof(storageAccount.ConnectionString), $"The connectionString of the storage account \"{storageAccount.Name}\" must be specified for the agent to initialize");
                if (storageAccount.Groups != null)
                {
                    foreach (var group in storageAccount.Groups)
                    {
                        if (string.IsNullOrEmpty(group.Name))
                            throw new ArgumentNullException(nameof(group.Name), "The name of the group, if defined, must be specified for the agent to initialize");
                        if (string.IsNullOrEmpty(group.Regex))
                            throw new ArgumentNullException(nameof(group.Regex), $"The regex of the group {group.Name} must be specified for the agent to initialize");
                    }
                }
            }
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
                    var metricName = $"Queues/{config.Name}/all/{queue.Name}/size";
                    ReportMetric(metricName, "messages", approximateMessageCount);

                    if (config.Groups != null)
                    {
                        // Send the data to the proper group.
                        foreach (var storageQueueGroup in config.Groups)
                        {
                            if (storageQueueGroup.AllowedInGroup(queue.Name))
                            {
                                metricName = $"Queues/{config.Name}/groups/{storageQueueGroup.Name}/{queue.Name}/size";
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

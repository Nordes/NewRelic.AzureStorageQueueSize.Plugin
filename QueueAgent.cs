using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using NewRelic.Platform.Sdk;
using ScalableBytes.NewRelic.AzureStorageQueueSize.Plugin.Models;
using Serilog;
using Serilog.Core;

namespace ScalableBytes.NewRelic.AzureStorageQueueSize.Plugin
{
    /// <summary>
    /// Plugin Agent in order to monitor storage account.
    /// </summary>
    /// <remarks>
    /// One Agent instance per Storage Account (<see cref="https://github.com/newrelic-platform/newrelic_dotnet_sdk"/>)
    /// </remarks>
    public class QueueAgent : Agent
    {
        private readonly Logger _eventLogLogger;
        private SystemConfiguration _systemConfiguration;

        /// <summary>
        /// Plugin Guid
        /// </summary>
        public override string Guid { get { return "com.scalablebytes.newrelic.azurestoragequeuesize"; } }

        /// <summary>
        /// Return the assembly version
        /// </summary>
        public override string Version { get { return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString(3); } }

        /// <summary>
        /// Queue Agent
        /// </summary>
        /// <param name="systemName"></param>
        /// <param name="accounts"></param>
        /// <param name="eventLogLogger"></param>
        public QueueAgent(Models.SystemConfiguration systemConfiguration, Logger eventLogLogger)
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
                var storageAccount = CloudStorageAccount.Parse(storageAccountConfig.ConnectionString);
                var queueClient = storageAccount.CreateCloudQueueClient();

                var continuationToken = new QueueContinuationToken();

                while (continuationToken != null)
                {
                    var listResponse = queueClient.ListQueuesSegmented(continuationToken);

                    // We must ask Azure for the size of each queue individually.
                    // This can be done in parallel.
                    Parallel.ForEach(listResponse.Results, queue =>
                    {
                        try
                        {
                            queue.FetchAttributes();
                        }
                        catch (Exception)
                        {
                            // Failed to communicate with Azure Storage, or queue is gone.
                            // Write in the eventlog
                            _eventLogLogger.Error("Failed to communicate with Azure Storage, or queue is not available anymore");
                        }
                    });

                    // ReportMetric is not thread-safe, so we can't call it in the parallel
                    foreach (var queue in listResponse.Results)
                    {
                        int count = queue.ApproximateMessageCount.HasValue ? queue.ApproximateMessageCount.Value : 0;
                        string metricName = string.Format("Queues/{0}/{1}/size", storageAccountConfig.AccountName, queue.Name);

                        ReportMetric(metricName, "messages", count);
                    }

                    continuationToken = listResponse.ContinuationToken;
                }
            }
        }
    }
}

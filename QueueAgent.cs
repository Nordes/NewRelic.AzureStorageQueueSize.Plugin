using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using NewRelic.Platform.Sdk;

namespace ScalableBytes.NewRelic.AzureStorageQueueSize.Plugin
{
    public class QueueAgent : Agent
    {
        public override string Version { get { return "1.0.0"; } }

        public string SystemName;
        public List<Dictionary<string, string>> StorageAccounts;

        public QueueAgent(string systemName, List<Dictionary<string, string>> accounts)
        {
            SystemName = systemName;
            StorageAccounts = accounts;
        }

        /// <summary>
        /// Returns a human-readable string to differentiate different hosts/entities in the site UI
        /// </summary>
        /// <returns></returns>
        public override string GetAgentName()
        {
            return SystemName;
        }

        public override string Guid
        {
            get
            {
                return "com.scalablebytes.newrelic.azurestoragequeuesize";
            }
        }

        /// <summary>
        // This is where logic for fetching and reporting metrics should exist.  
        // Call off to a REST head, SQL DB, virtually anything you can programmatically 
        // get metrics from and then call ReportMetric.
        /// </summary>
        public override void PollCycle()
        {
            foreach (var storageAccountInfo in StorageAccounts)
            {

                var accountName = storageAccountInfo["accountName"];
                var connectionString = storageAccountInfo["connectionString"];

                var storageAccount = CloudStorageAccount.Parse(connectionString);
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
                        }

                    });

                    // ReportMetric is not thread-safe, so we can't call it in the parallel
                    foreach (var queue in listResponse.Results)
                    {
                        int count = queue.ApproximateMessageCount.HasValue ? queue.ApproximateMessageCount.Value : 0;
                        string metricName = string.Format("Queues/{0}/{1}/size", accountName, queue.Name);

                        ReportMetric(metricName, "messages", count);
                    }

                    continuationToken = listResponse.ContinuationToken;
                }
            }
        }
    }
}

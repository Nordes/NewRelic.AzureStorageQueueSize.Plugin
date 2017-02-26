using System.Collections.Generic;

namespace ScalableBytes.NewRelic.AzureStorageQueueSize.Plugin.Models
{
    public class StorageAccount
    {
        public string Name { get; set; }
        public string ConnectionString { get; set; }
        public List<StorageQueueGroup> Groups { get; set; }
    }
}

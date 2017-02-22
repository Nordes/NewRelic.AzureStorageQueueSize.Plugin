using System.Collections.Generic;

namespace ScalableBytes.NewRelic.AzureStorageQueueSize.Plugin.Models
{
    public class SystemConfiguration
    {
        public string Name { get; set; }
        public List<StorageAccount> StorageAccounts { get; set; }
    }
}

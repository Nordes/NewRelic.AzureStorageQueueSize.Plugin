using System.Collections.Generic;

namespace ScalableBytes.NewRelic.AzureStorageQueueSize.Plugin.Tests
{
    internal static class TestHelper
    {
        /// <summary>
        /// Generate a basic configuration with nothing special.
        /// </summary>
        /// <returns></returns>
        internal static Models.SystemConfiguration GetBasicConfiguration()
        {
            /*
                {
                    "agents": [
                        {
                            "name": "TestConfig",
                            "storageAccounts": [
                                {
                                    "name": "AccountQueueA",
                                    "connectionString": "fake"
                                }
                            ]
                        }
                    ]
                }
             */
            var config = new Models.SystemConfiguration()
            {
                Name = "TestConfig",
                StorageAccounts = new List<Models.StorageAccount>()
                {
                    new Models.StorageAccount() {
                        Name = "AccountQueueA",
                        ConnectionString = "fake",
                        Groups = null
                    }
                }
            };

            return config;
        }
    }
}

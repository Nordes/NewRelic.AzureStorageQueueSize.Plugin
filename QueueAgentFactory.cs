using System.Collections.Generic;
using NewRelic.Platform.Sdk;

namespace ScalableBytes.NewRelic.AzureStorageQueueSize.Plugin
{
    public class QueueAgentFactory : AgentFactory
    {
        public QueueAgentFactory()
            : base("queue-agent-config.json")
        {
        }

        // This will return the deserialized properties from the specified configuration file
        // It will be invoked once per JSON object in the configuration file
        public override Agent CreateAgentWithConfiguration(IDictionary<string, object> properties)
        {
            var systemName = (string)properties["systemName"];
            var storageAccounts = (List<object>) properties["storageAccounts"];

            var typedStorageAccounts = new List<Dictionary<string, string>>();

            foreach (var obj in storageAccounts)
            {
                var dic = (Dictionary<string, object>) obj;

                var newDic = new Dictionary<string, string>();

                foreach (var acc in dic)
                    newDic[acc.Key] = (string) acc.Value;

                typedStorageAccounts.Add(newDic);
            }

            return new QueueAgent(systemName, typedStorageAccounts);
        }
    }
}

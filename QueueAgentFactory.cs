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
            var name = (string)properties["name"];
            var connectionString = (string)properties["connectionString"];
            
            return new QueueAgent(name, connectionString);
        }
    }
}

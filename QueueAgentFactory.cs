using System.Collections.Generic;
using NewRelic.Platform.Sdk;
using Serilog.Core;

namespace ScalableBytes.NewRelic.AzureStorageQueueSize.Plugin
{
    /// <summary>
    /// Azure Storage Queue Agent. It  is an abstract base class that is meant to help facilitate creation of 
    /// Agents from the well-defined configuration file 'plugin.json'
    /// </summary>
    public class QueueAgentFactory : AgentFactory
    {
        private Logger _eventLogLogger;

        /// <summary>
        /// Constructor
        /// </summary>
        public QueueAgentFactory(Logger eventLogLogger)
            : base()
        {
            _eventLogLogger = eventLogLogger;
        }

        /// <summary>
        /// This will return the deserialized properties from the specified configuration file
        /// It will be invoked once per JSON object in the configuration file
        /// </summary>
        /// <param name="properties"></param>
        /// <returns></returns>
        public override Agent CreateAgentWithConfiguration(IDictionary<string, object> properties)
        {
            var configuration = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.SystemConfiguration>(Newtonsoft.Json.JsonConvert.SerializeObject(properties));

            return new QueueAgent(configuration, _eventLogLogger);
        }
    }
}

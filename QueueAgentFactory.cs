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
        private readonly Logger _eventLogLogger;

        /// <summary>
        /// Constructor
        /// </summary>
        public QueueAgentFactory(Logger eventLogLogger)
        {
            _eventLogLogger = eventLogLogger;
        }

        /// <summary>
        /// This will return the deserialized properties from the specified configuration file
        /// It will be invoked once per JSON object in the configuration file
        /// </summary>
        /// <param name="properties">The properties.</param>
        /// <returns>The queue agent</returns>
        public override Agent CreateAgentWithConfiguration(IDictionary<string, object> properties)
        {
            // Trick to have real object to manage.
            var configuration = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.SystemConfiguration>(Newtonsoft.Json.JsonConvert.SerializeObject(properties));

            return new QueueAgent(configuration, _eventLogLogger);
        }
    }
}

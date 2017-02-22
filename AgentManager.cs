using System.Threading;
using NewRelic.Platform.Sdk;
using Serilog.Core;

namespace ScalableBytes.NewRelic.AzureStorageQueueSize.Plugin
{
    class AgentManager
    {
        private Thread _pollThread;
        private Logger _eventLogLogger;

        public AgentManager(Logger eventLogLogger)
        {
            _eventLogLogger = eventLogLogger;
        }

        /// <summary>
        /// Start the Runner. NewRelic Runner is the main entrypoint class for the SDK. Essentially you will create an instance of a Runner, 
        /// assign either your programmatically configured Agents or your AgentFactory to it, and then call SetupAndRun() 
        /// which will begin invoking the PollCycle() method on each of your Agents once per polling interval.
        /// </summary>
        public void Start()
        {
            _pollThread = new Thread(() =>
            {
                var runner = new Runner();

                // You can add either a factory to generate Components from a configuration file or 
                // you can directly add a component programmatically
                runner.Add(new QueueAgentFactory(_eventLogLogger));

                runner.SetupAndRun();
            });

            _pollThread.Start();
        }

        public void Stop()
        {
            _pollThread.Abort();
        }
    }
}

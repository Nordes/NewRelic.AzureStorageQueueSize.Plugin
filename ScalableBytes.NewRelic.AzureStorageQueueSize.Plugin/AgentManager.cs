using System.Threading;
using NewRelic.Platform.Sdk;
using Serilog;

namespace ScalableBytes.NewRelic.AzureStorageQueueSize.Plugin
{
    /// <summary>
    /// NewRelic Agent manager
    /// </summary>
    class AgentManager
    {
        private Thread _pollThread;
        private readonly ILogger _eventLogLogger;

        /// <summary>
        /// Initializes a new instance of the <see cref="AgentManager"/> class.
        /// </summary>
        /// <param name="eventLogLogger">The event log logger.</param>
        public AgentManager(ILogger eventLogLogger)
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

        /// <summary>
        /// Stops this instance.
        /// </summary>
        public void Stop()
        {
            _pollThread.Abort();
        }
    }
}

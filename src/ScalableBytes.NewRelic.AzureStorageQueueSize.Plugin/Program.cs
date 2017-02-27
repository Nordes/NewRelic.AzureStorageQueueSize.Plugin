using Serilog;
using Topshelf;

namespace ScalableBytes.NewRelic.AzureStorageQueueSize.Plugin
{
    /// <summary>
    /// Plugin application
    /// </summary>
    class Program
    {
        // Should be one static per class
        private static Serilog.Core.Logger _eventLogLogger = new LoggerConfiguration()
                .WriteTo.EventLog("Test", manageEventSource: true)
                .CreateLogger();

        /// <summary>
        /// Defines the entry point of the application.
        /// </summary>
        public static void Main()
        {
            HostFactory.Run(x =>
            {
                var svc = x.Service<AgentManager>(s =>
                     {
                         s.ConstructUsing(name => new AgentManager(_eventLogLogger));
                         s.WhenStarted(tc => tc.Start());
                         s.WhenStopped(tc => tc.Stop());
                     });
                x.RunAsLocalSystem();

                x.SetDescription("NewRelic Windows Azure Storage Queue Size plugin");
                x.SetDisplayName("ScalableBytes NewRelic Azure Queues");
                x.SetServiceName("ScalableBytes.NewRelic.AzureStorageQueueSize.Plugin");
            });
        }
    }
}

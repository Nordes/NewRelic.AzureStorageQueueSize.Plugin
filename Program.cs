using Topshelf;

namespace ScalableBytes.NewRelic.AzureStorageQueueSize.Plugin
{
   class Program
   {
      public static void Main()
      {
         HostFactory.Run(x =>                                
         {
            x.Service<AgentManager>(s =>                     
            {
               s.ConstructUsing(name => new AgentManager()); 
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

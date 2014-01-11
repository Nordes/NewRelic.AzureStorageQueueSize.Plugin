newrelic_azurequeue_agent
=========================

newrelic_azurequeue_agent is a NewRelic plug-in which monitors the size of the queues in one or more Windows Azure Storage accounts.

*NOTE:* This plug-in uses the NewRelic .NET  API which is currently in beta.

Installation
=========================

1) Build the solution ScalableBytes.NewRelic.AzureStorageQueueSize.Plugin.sln using Visual Studio 2013
2) Open the folder Bin\Release
3) Edit ScalableBytes.NewRelic.AzureStorageQueueSize.Plugin.exe.config and replace [ENTER_LICENSE_KEY] with your NewRelic license key.
4) Take a copy of queue-agent-config.json.example and give it the name queue-agent-config.json
5) Edit queue-agent-config.json and add your Windows Azure Storage accounts.
6) Execute ScalableBytes.NewRelic.AzureStorageQueueSize.Plugin.exe to start monitoring.

To install the plug-in as a Windows Service, execute "ScalableBytes.NewRelic.AzureStorageQueueSize.Plugin.exe install".

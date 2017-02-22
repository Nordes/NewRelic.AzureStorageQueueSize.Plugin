# NewRelic_AzureQueue_Agent
NewRelic Azure Queue Agent is a NewRelic Plugin which monitors the size of the queues in one or more Windows Azure Storage accounts. 
Use it to see how the sizes of queues evolve over time.

* The plugin is NPI-compatible.
* When there's errors contacting NewRelic or Azure Storage Queue, an eventlog is created in Windows EventLogs ournals

# Building & Using
1. Build the solution ScalableBytes.NewRelic.AzureStorageQueueSize.Plugin.sln using Visual Studio 2013
2. Open the folder Bin\Release
3. Edit and rename _./config/newrelic.template.json_ to _./config/newrelic.json_ and replace the __[NEW_RELIC_LICENSE_KEY]__ with your NewRelic license key.
4. Edit and rename _./config/plugin.template.json_ to _./config/plugin.json_ and add your Windows Azure Storage accounts.
5. Execute _plugin.exe_ to start monitoring.

# Install as Windows Service
To install the plug-in as a Windows Service, execute "plugin.exe install". This will add it was a Windows Service which will start automatically when windows start.

# Todo's
* Add an image with new relic graph for Insights and Plugin
* Add an image with new relic example on alert
* Change the GUID of the plugin to have something shorter?
* Add more content to this file (configuration, etc.)
* ...

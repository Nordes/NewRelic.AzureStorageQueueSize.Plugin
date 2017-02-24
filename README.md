# NewRelic AzureQueue Agent
NewRelic Azure Queue Agent is a NewRelic Plugin which monitors the size of the queues in one or more Windows Azure Storage accounts. 
Use it to see how the sizes of queues evolve over time.

* The plugin is NPI-compatible. *(On it's way)*
* When there's errors contacting NewRelic or Azure Storage Queue, an eventlog is created in Windows EventLogs ournals

# NewRelic Plugin Dashboard example
![plugin-dashboard](https://cloud.githubusercontent.com/assets/446572/23294952/4ca15e1c-fa6e-11e6-918e-a9d89cd2ab11.png)

# Building & Using
1. Build the solution ScalableBytes.NewRelic.AzureStorageQueueSize.Plugin.sln using Visual Studio 2013 or higher (Framework v4.6.1)
2. Open the folder Bin\Release
3. Edit and rename `./config/newrelic.template.json` to `./config/newrelic.json` and replace the __[NEW_RELIC_LICENSE_KEY]__ with your NewRelic license key.
4. Edit and rename `./config/plugin.template.json` to `./config/plugin.json` and add your Windows Azure Storage accounts.
5. Execute _plugin.exe_ to start monitoring.

# Install as Windows Service
To install the plug-in as a Windows Service, execute "plugin.exe install". This will add it was a Windows Service which will start automatically when windows start.

# Example of configuration 
## ./config/plugin.json
```javascript
{
  // Mandatory root
  "agents": [
    // App configuration
    {
      "name": "App Production",
      // Storage(s) configuration
      "storageAccounts": [
        {
          "name": "Account name",
          "connectionString": "DefaultEndpointsProtocol=https;AccountName=app***;AccountKey=d0Wo************;BlobEndpoint=https://app***.blob.core.windows.net/;QueueEndpoint=https://app***.queue.core.windows.net/;TableEndpoint=https://app****.table.core.windows.net/;FileEndpoint=https://app***.file.core.windows.net/;"
        }
    }
}
```

### Configuration
| Plugin.Config                                      | Type   | Description                                            |
| :------------------------------------------------- |:------:| :------------------------------------------------------|
| agents                                             | array  | Define the agents that will run in the background      |
| agents[*0*].name                                   | string | Define the agent name, by example *MyApp-Production*   |
| agents[*0*].storageAccounts                        | array  | Define the list of storage account you want to pull the statistics |
| agents[*0*].storageAccounts[*0*].name              | string | Define the account name that will be displayed in NewRelic. This is not the official storage account name |
| agents[*0*].storageAccounts[*0*].connectionString  | string | Define the connection string to the azure storage account |
| agents[*0*].storageAccounts[*0*].groups            | array  | *(Optional)* Define grouping we want to capture         |
| agents[*0*].storageAccounts[*0*].groups[*0*].name  | string | Gives a name to the group that will be sent to NewRelic |
| agents[*0*].storageAccounts[*0*].groups[*0*].regex | string | Define the regex to capture the group based on the queue name |

## ./config/newrelic.json
```javascript
{
  "license_key": "NEW_RELIC_LICENSE_KEY"
}
```

### Configuration
| Plugin.Config                                     | Type   | Description                                            |
| :------------------------------------------------ |:------:| :------------------------------------------------------|
| license_key                                       | string | NewRelic license key (Available from NewRelic site)    |

# Todo's
* Add an image with new relic example on alert
* Change the GUID of the plugin to have something shorter?
* Add more content to this file (configuration, etc.)
* ...

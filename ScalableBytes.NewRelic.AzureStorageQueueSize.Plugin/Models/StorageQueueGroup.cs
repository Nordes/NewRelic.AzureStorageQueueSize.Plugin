using System.Text.RegularExpressions;

namespace ScalableBytes.NewRelic.AzureStorageQueueSize.Plugin.Models
{
    public class StorageQueueGroup
    {
        public string Name { get; set; }
        public string Regex { get; set; }

        public bool AllowedInGroup(string queueName)
        {
            return new Regex(Regex, RegexOptions.CultureInvariant).IsMatch(queueName);
        }
    }
}

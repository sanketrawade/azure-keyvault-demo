using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using System;

namespace azure_keyvault_demo
{
    internal class Program
    {
        public async Task AccessKeysAsync(string secretName, string secretValue, string keyVaultUri, string keyVaultName)
        {
            var client = new SecretClient(new Uri(keyVaultUri), new DefaultAzureCredential());

            //create an secret in the keyvault.
            Console.Write($"Creating a secret in {keyVaultName} called '{secretName}' with the value '{secretValue}' ...");
            await client.SetSecretAsync(secretName, secretValue);
            Console.WriteLine(" done.");

            Console.WriteLine("Forgetting your secret.");
            secretValue = string.Empty;
            Console.WriteLine($"Your secret is '{secretValue}'.");

            //retrive an secret from the keyvault.
            Console.WriteLine($"Retrieving your secret from {keyVaultName}.");
            var secret = await client.GetSecretAsync(secretName);
            Console.WriteLine($"Your secret is '{secret.Value.Value}'.");

            Console.Write($"Deleting your secret from {keyVaultName} ...");
            DeleteSecretOperation operation = await client.StartDeleteSecretAsync(secretName);
            // You only need to wait for completion if you want to purge or recover the secret.
            await operation.WaitForCompletionAsync();
            Console.WriteLine(" done.");

            Console.Write($"Purging your secret from {keyVaultName} ...");
            await client.PurgeDeletedSecretAsync(secretName);
            Console.WriteLine(" done.");
        }

        static void Main(string[] args)
        {
            const string secretName = "SECRETNAME";
            var secretValue = "SECRETVALUE";
            var keyVaultName = "demo-keyvault-v3"; //change the keyvault name, 1 keyvault may have multiple secrets
            var kvUri = $"https://{keyVaultName}.vault.azure.net";

            var program = new Program();
            program.AccessKeysAsync(secretName, secretValue, kvUri, keyVaultName).Wait();
        }
    }
}
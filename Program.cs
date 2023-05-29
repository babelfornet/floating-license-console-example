using Babel.Licensing;

// Create a new configuration object
BabelLicensingConfiguration config = new BabelLicensingConfiguration() {
    // Set the service URL
    ServiceUrl = "http://localhost:5005",
    
    // Set the public key used to verify the license signature
    SignatureProvider = RSASignature.FromKeys("MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQDE1VRiIdr6fiVZKve7NVgjIvGdRiRx0Mjjm+Yzf6tLbzFnxLs0fat5EoRcubxx0QQQDfydsJBE/fc7cwRWSrE2xK6X4Eb4W8O47pCMjqvTQZfDqQywEZJrLlxpp9hlKz6FDYX4SagrjmP1gdw8olo+n+IBz8ubkNxRhvycikxuDQIDAQAB"),
    
    // Set a unique client ID in case multiple instances of the application are running on the same machine
    ClientId = Guid.NewGuid().ToString()
};

// Create the client object used to communicate with the server
BabelLicensing client = new BabelLicensing(config);

// Ask the user to enter the floating license key
Console.WriteLine("Please enter your floating license key: ");
string? userKey = Console.ReadLine();

try
{
    var result = await client.RequestFloatingLicenseAsync(userKey, typeof(Program));

    Console.WriteLine($"License {result.License.Id} requested.");
}
catch (Exception ex)
{
    Console.WriteLine($"Could not request floating license: {ex.Message}");
    return;
}

try
{
    // Simulate a long task which requires the allocation of a license
    for (int i = 0; i < 10; i++)
    {
        // Validate the license
        // This will contact the server to validate the license
        var result = await client.ValidateLicenseAsync(userKey, typeof(Program));
        Console.WriteLine($"{i}) License {result.License.Id} valid.");

        // Wait 1 second
        await Task.Delay(1000);
    }

    try
    {
        // Release the license to allow other clients to use it
        await client.ReleaseFloatingLicenseAsync(userKey);
        Console.WriteLine("License released.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Could not release license: {ex.Message}");
    }

}
catch (Exception ex)
{
    Console.WriteLine($"License not valid: {ex.Message}");
    return;
}

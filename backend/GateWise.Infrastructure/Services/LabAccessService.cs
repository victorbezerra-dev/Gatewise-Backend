using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using GateWise.Core.DTOs;
using GateWise.Core.Enums;
using GateWise.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using MQTTnet;
using MQTTnet.Client;

public class LabAccessService : ILabAccessService
{
    private readonly IAccessLogRepository _accessLogRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMqttClient _mqttClient;
    private readonly MqttClientOptions _mqttOptions;
    private readonly string _privateKey;
    private readonly string _espPublicKey;

    public LabAccessService(
        IAccessLogRepository accessLogRepository,
        IUserRepository userRepository,
        IMqttClient mqttClient,
        MqttClientOptions mqttOptions,
        IConfiguration config)
    {
        _accessLogRepository = accessLogRepository;
        _userRepository = userRepository;
        _mqttClient = mqttClient;
        _mqttOptions = mqttOptions;

        var privateKeyPath = config["CryptoKeys:PrivateKeyPath"];
        var espPublicKeyPath = config["CryptoKeys:EspPublicKeyPath"];

        _privateKey = File.ReadAllText(privateKeyPath);
        _espPublicKey = File.ReadAllText(espPublicKeyPath);
    }

    public async Task<string> RequestLabAccessAsync(string userId, AccessLogCreateDto dto)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        var devicePublicKey = user?.DevicePublicKeyPem?.Replace("\\n", "\n");

        if (string.IsNullOrWhiteSpace(devicePublicKey))
            throw new InvalidOperationException("Device public key not registered.");

        var message = $"open:{dto.Timestamp}";
        var data = Encoding.UTF8.GetBytes(message);

        var isValid = VerifyWithPublicKey(message, dto.Signature, devicePublicKey);
        if (!isValid)
            throw new SecurityException("Invalid signature from client device.");

        if (!_mqttClient.IsConnected)
        {
            var tcpOptions = _mqttOptions.ChannelOptions as MQTTnet.Client.MqttClientTcpOptions;
            var host = tcpOptions?.Server ?? "undefined";
            var port = tcpOptions?.Port ?? 1883;

            Console.WriteLine("Attempting to connect to the MQTT broker...");
            Console.WriteLine($"Host: {host}");
            Console.WriteLine($"Port: {port}");

            try
            {
                await _mqttClient.ConnectAsync(_mqttOptions);
                Console.WriteLine("Successfully connected to the MQTT broker.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to connect to MQTT broker: {ex.Message}");
                throw;
            }
        }

        var commandId = Guid.NewGuid().ToString();
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var messageToSign = $"open:{timestamp}";
        var signature = SignWithPrivateKey(messageToSign, _privateKey);

        var mqttPayload = new
        {
            command = "open",
            commandId,
            timestamp,
            signature
        };

        var payloadJson = JsonSerializer.Serialize(mqttPayload);

        var messageMQQT = new MqttApplicationMessageBuilder()
            .WithTopic("command/open-lock")
            .WithPayload(payloadJson)
            .WithQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce)
            .Build();

        await _mqttClient.PublishAsync(messageMQQT);

        var log = new AccessLog
        {
            CommandId = commandId,
            UserId = userId,
            RawRequestJson = payloadJson,
            Status = AccessStatus.PENDING_CONFIRMATION
        };

        await _accessLogRepository.CreateAsync(log);
        await _accessLogRepository.SaveChangesAsync();

        return payloadJson;
    }

    private string SignWithPrivateKey(string message, string privateKeyPem)
    {
        using var rsa = RSA.Create();
        rsa.ImportFromPem(privateKeyPem.ToCharArray());
        var data = Encoding.UTF8.GetBytes(message);
        var signature = rsa.SignData(data, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        return Convert.ToBase64String(signature);
    }

    private bool VerifyWithPublicKey(string message, string signatureBase64, string publicKeyPem)
    {
        try
        {
            using var rsa = RSA.Create();
            rsa.ImportFromPem(publicKeyPem.ToCharArray());
            var data = Encoding.UTF8.GetBytes(message);
            var signature = Convert.FromBase64String(signatureBase64);
            return rsa.VerifyData(data, signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        }
        catch
        {
            return false;
        }
    }
}

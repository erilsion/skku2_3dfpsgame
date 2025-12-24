using System;
using System.Security.Cryptography;
using System.Text;

public static class AESCrypto
{
    private static readonly string SecretKey = "MySecretKey123!";

    private const int KeySize = 32;
    private const int IvSize = 16;

    public static string Encrypt(string plainText)
    {
        using Aes aes = Aes.Create();
        aes.Key = Encoding.UTF8.GetBytes(SecretKey.PadRight(KeySize));
        aes.GenerateIV(); // 매번 랜덤 IV 생성

        byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);

        using ICryptoTransform encryptor = aes.CreateEncryptor();
        byte[] cipherBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

        byte[] result = new byte[aes.IV.Length + cipherBytes.Length];
        Buffer.BlockCopy(aes.IV, 0, result, 0, aes.IV.Length);
        Buffer.BlockCopy(cipherBytes, 0, result, aes.IV.Length, cipherBytes.Length);

        return Convert.ToBase64String(result);
    }

    public static string Decrypt(string cipherText)
    {
        byte[] fullBytes = Convert.FromBase64String(cipherText);

        byte[] iv = new byte[IvSize];
        byte[] cipherBytes = new byte[fullBytes.Length - IvSize];

        Buffer.BlockCopy(fullBytes, 0, iv, 0, IvSize);
        Buffer.BlockCopy(fullBytes, IvSize, cipherBytes, 0, cipherBytes.Length);

        using Aes aes = Aes.Create();
        aes.Key = Encoding.UTF8.GetBytes(SecretKey.PadRight(KeySize));
        aes.IV = iv;

        using ICryptoTransform decryptor = aes.CreateDecryptor();
        byte[] plainBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);

        return Encoding.UTF8.GetString(plainBytes);
    }
}

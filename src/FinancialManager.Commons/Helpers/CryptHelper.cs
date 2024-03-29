﻿using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Text;

namespace FinancialManager.Commons.Helpers;

public static class CryptHelper
{
    public static string CryptEncodingKey { get; set; }
    public static string EncryptString(string str)
    {
        return Convert.ToBase64String(KeyDerivation.Pbkdf2(str, Encoding.ASCII.GetBytes(CryptEncodingKey), KeyDerivationPrf.HMACSHA256, iterationCount: 100000, numBytesRequested: 256 / 8));
    }
}
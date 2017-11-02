using System.Security.Cryptography;

/// <summary>
/// This class includes handy methods
/// </summary>
public static class Tools
{
    /// <summary>
    /// Get HashAlgorithmName
    /// </summary>
    public static HashAlgorithmName GetHashAlgorithmName(string signature_algorithm)
    {
        if (signature_algorithm == "SHA-256")
        {
            return HashAlgorithmName.SHA256;
        }
        else if (signature_algorithm == "SHA-384")
        {
            return HashAlgorithmName.SHA384;
        }
        else if (signature_algorithm == "SHA-512")
        {
            return HashAlgorithmName.SHA512;
        }
        else
        {
            return HashAlgorithmName.SHA1;
        }

    } // End of the GetHashAlgorithmName method

    /// <summary>
    /// Get RSASignaturePadding
    /// </summary>
    public static RSASignaturePadding GetRSASignaturePadding(string signature_padding)
    {
        if(signature_padding == "Pss")
        {
            return RSASignaturePadding.Pss;
        }
        else
        {
            return RSASignaturePadding.Pkcs1;
        }

    } // End of the GetRSASignaturePadding method

} // End of the class
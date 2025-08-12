using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Security.Cryptography.X509Certificates;

[AttributeUsage(AttributeTargets.Property)]
public class ValidateClientCertificatesAttribute : ValidateArgumentsAttribute
{
    protected override void Validate(object arguments, EngineIntrinsics engineIntrinsics)
    {
        if (!(arguments is X509Certificate2[] certificates)) { return; }

        List<string> invalidCertificateThumbprints = new List<string>();

        foreach (X509Certificate2 certificate in certificates)
        {
            if (!certificate.HasPrivateKey)
            {
                invalidCertificateThumbprints.Add(certificate.Thumbprint);
            }
        }

        if (invalidCertificateThumbprints.Count > 0)
        {
            throw new ValidationMetadataException($"Сertificates with the following thumbprints are missing private keys: {string.Join(", ", invalidCertificateThumbprints)}");
        }
    }
}
---
document type: cmdlet
external help file: PSMailKit-Help.xml
HelpUri: ''
Locale: en-US
Module Name: PSMailKit
ms.date: 08.15.2025
PlatyPS schema version: 2024-05-01
title: Get-SMTPCapabilities
---

# Get-SMTPCapabilities

## SYNOPSIS

Gets SMTP extensions supported by a server, with descriptions and RFC links.

## SYNTAX

### __AllParameterSets

```
Get-SMTPCapabilities [-SmtpServer] <string> [-Port] <int> [-Credential <pscredential>]
 [-SecureSocketOption <string>] [-CheckCertificateRevocation <bool>] [<CommonParameters>]
```

## ALIASES

This cmdlet doesn't have any aliases.

## DESCRIPTION

The Get-SMTPCapabilities cmdlet retrieves a list of SMTP extensions supported by the specified server. These extensions provide additional features and capabilities beyond the basic SMTP protocol, often related to security and functionality. If the remote server requires authentication, credentials can be supplied directly using the -Credential parameter or will be requested interactively. For each extension received from the server, the cmdlet will output a short name, description, and a link to the relevant RFC for further details.

## EXAMPLES

### Example 1: Get list of SMTP extensions from server

```Powershell
PS C:\> Get-SMTPCapabilities -SMTPServer smtp.contoso.com -Port 25
```

### Example 2: Get list of SMTP extensions from server using credential object and StartTls

```Powershell
PS C:\> Get-SMTPCapabilities -SMTPServer exchange-01.contoso.com -Port 465 -SecureSocketOption StartTls -Credential (Get-Credential username)
```

## PARAMETERS

### -CheckCertificateRevocation

Parameter `-CheckCerificateRevocation:$false` could be specified to disable certificate revocation when connecting via SSL/TLS.

Normally, certificate revocation check should not be disabled for security reasons, but there are times when it may be necessary to disable it.

For example, most Certificate Authorities are probably pretty good at keeping their CRL and/or OCSP servers up 24/7, but occasionally they do go down or are otherwise unreachable due to other network problems between the client and the Certificate Authority. When this happens, it becomes impossible to check the revocation status of one or more of the certificates in the chain resulting in an SslHandshakeException being thrown in the Connect method. If this becomes a problem, it may become desirable to set CheckCertificateRevocation to $false.

```yaml
Type: System.Boolean
DefaultValue: ''
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: (All)
  Position: Named
  IsRequired: false
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -Credential

The PSCredential object that represents a set of security credentials: username and password.

```yaml
Type: System.Management.Automation.PSCredential
DefaultValue: ''
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: (All)
  Position: Named
  IsRequired: false
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -Port

The port to connect to. If the specified port is 0, then the default port will be used.

```yaml
Type: System.Int32
DefaultValue: ''
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: (All)
  Position: 1
  IsRequired: true
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -SecureSocketOption

Secure socket options. Provides a way of specifying the SSL and/or TLS encryption that should be used for a connection.

```yaml
Type: System.String
DefaultValue: ''
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: (All)
  Position: Named
  IsRequired: false
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: ["Auto", "StartTls", "StartTlsWhenAvailable", "SslOnConnect"]
HelpMessage: ''
```

### -SmtpServer

The SMTP server host name to connect to.

```yaml
Type: System.String
DefaultValue: ''
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: (All)
  Position: 0
  IsRequired: true
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### CommonParameters

This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable,
-InformationAction, -InformationVariable, -OutBuffer, -OutVariable, -PipelineVariable,
-ProgressAction, -Verbose, -WarningAction, and -WarningVariable. For more information, see
[about_CommonParameters](https://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

## OUTPUTS

### PSMailKit.Commands.GetSMTPCapabilitiesCommand+ExtensionInfo

For each extension received from the server, the cmdlet will output a short name, description, and a link to the relevant RFC for further details.

## NOTES

## RELATED LINKS

Online Version: https://github.com/ahpooch/PSMailKit/raw/refs/heads/main/maml/PSMailKit/PSMailKit-Help.xml

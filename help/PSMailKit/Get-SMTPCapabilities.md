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

{{ Fill CheckCertificateRevocation Description }}

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

{{ Fill Credential Description }}

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

{{ Fill Port Description }}

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

{{ Fill SecureSocketOption Description }}

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
AcceptedValues: []
HelpMessage: ''
```

### -SmtpServer

{{ Fill SmtpServer Description }}

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

{{ Fill in the Description }}

## NOTES

{{ Fill in the Notes }}

## RELATED LINKS

{{ Fill in the related links here }}


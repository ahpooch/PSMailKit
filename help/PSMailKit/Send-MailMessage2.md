---
document type: cmdlet
external help file: PSMailKit-Help.xml
HelpUri: https://github.com/ahpooch/PSMailKit/blob/main/help/PSMailKit/Send-MailMessage2.md
Locale: en-US
Module Name: PSMailKit
ms.date: 08.15.2025
PlatyPS schema version: 2024-05-01
title: Send-MailMessage2
---
<!-- markdownlint-disable MD025 -->
# Send-MailMessage2
<!-- markdownlint-enable MD025 -->
## SYNOPSIS

Send-MailMessage2 – A secure, feature-rich replacement for Send-MailMessage, supporting attachments, inline images (HTML emails), priority flags, DSN, delegation ("Send As"/"On Behalf"), and custom Reply-To. Built on MailKit/MimeKit.

## SYNTAX

### __AllParameterSets
<!-- markdownlint-disable MD040 -->
```
Send-MailMessage2 [-SmtpServer] <string> [-Port] <int> -From <InternetAddressList>
 -To <InternetAddressList> [-Sender <MailboxAddress>] [-ReplyTo <InternetAddressList>]
 [-CcList <InternetAddressList>] [-BccList <InternetAddressList>] [-Subject <string>]
 [-TextBody <string>] [-HtmlBody <string>] [-Attachments <string[]>]
 [-InlineAttachments <hashtable[]>] [-DeliveryStatusNotificationTypeOption <string>]
 [-Date <DateTimeOffset>] [-Credential <pscredential>] [-SecureSocketOption <string>]
 [-ClientCertificates <X509Certificate2[]>] [-LogPath <string>] [-Importance <string>]
 [-DeliveryStatusNotificationOptions <string[]>] [-CheckCertificateRevocation <bool>] [-WhatIf]
 [-Confirm] [<CommonParameters>]
```
<!-- markdownlint-enable MD040 -->

## ALIASES

## DESCRIPTION

The Send-MailMessage2 cmdlet is designed to replace the deprecated Send-MailMessage cmdlet. Leveraging the MailKit and MimeKit libraries, Send-MailMessage2 ensures secure communication with an SMTP server (e.g., using client certificates) and offers advanced functionality.

This cmdlet enables creating emails with attachments and inline attachments, which are essential for building HTML email templates where images are embedded directly into the HTML body.
It also supports:

- Setting email importance (priority) and Delivery Status Notifications (DSN).
- Sending emails as "Send As" or "Send on Behalf" (if the user has the required permissions on the mail server).
- Specifying a custom Reply-To address and other advanced features.

## EXAMPLES

### Example 1: Send simple message

```Powershell
Send-MailMessage2 -To "bob@contoso.com" -From "fred@contoso.com" -Subject "Welcome" -TextBody "Welcolme to Contoso!" -SmtpServer "smtp.contoso.com" -Port 25
```

### Example 2: Send message using SecureSocketOption and user credential

```Powershell
Send-MailMessage2 -To "bob@contoso.com" -From "fred@contoso.com" -Subject "Welcome" -TextBody "Welcolme to Contoso!" -SmtpServer "exch.contoso.com" -Port 465 -SecureSocketOption StartTls -Credential (Get-Credential fred)
```

### Example 3: Send message using client certificate

```Powershell
Send-MailMessage2 -To "bob@contoso.com" -From "fred@contoso.com" -Subject "Welcome" -TextBody "Welcolme to Contoso!" -SmtpServer "exch.contoso.com" -Port 25 -ClientCertificates (Get-Item -Path 'Cert:\CurrentUser\My\44A0351360E57179D87D0CCDB20A291238E7CBD4')
```

### Example 4: Send message with Cc and Bcc and attachment

```Powershell
Send-MailMessage2 -To "bob@contoso.com" -From "fred@contoso.com" -Subject "Daily reports" -TextBody "See daily reports in attachments." -SmtpServer "smtp.contoso.com" -Port 25 -CCList "raul@contoso.com,jim@contoso.com" -BCCList "bartholomew@contoso.com" -Attachments "C:\DailyReport.jpg","C:\DailyReport2.jpg"
```

### Example 5: Send message with ReplyTo and specified Importance

```Powershell
Send-MailMessage2 -To "bob@contoso.com" -From "fred@contoso.com" -Subject "Welcome" -TextBody "Welcolme to Contoso!" -SmtpServer "smtp.contoso.com" -Port 25 -ReplyTo "hr@contoso.com" -Importance High
```

### Example 6: Send message as `Send as`

```Powershell
Send-MailMessage2 -To "bob@contoso.com" -From "hr@contoso.com" -Subject "Welcome" -TextBody "Welcolme to Contoso!" -SmtpServer "smtp.contoso.com" -Port 25 -Credential (Get-Credential fred)
```

### Example 7: Send message as `Send on behalf`

```Powershell
Send-MailMessage2 -To "bob@contoso.com" -From "hr@contoso.com" -Sender "fred@rowi.com" -Subject "Welcome" -TextBody "Welcolme to Contoso!" -SmtpServer "smtp.contoso.com" -Port 25 -Credential (Get-Credential fred)
```

### Example 8: Send message without certificate revocation check

```Powershell
Send-MailMessage2 -To "bob@contoso.com" -From "fred@contoso.com" -Subject "We have a problem!" -TextBody "Hey, Bob! What's with our CRL and/or OCSP servers? The certificate revocation check process is failing right now and I forced to use -CheckCertificateRevocation:`$false to send this email!" -SmtpServer "exch.contoso.com" -Port 465 -SecureSocketOption StartTls -Credential (Get-Credential fred) -CheckCertificateRevocation:$false
```

### Example 9: Send message with inline attachments and HTML body

To use inline attachments or embedded images we should use `-InlineAttachments` parameter. Inline attachments specified by Hashtable[] which means that we provide inline attachments as array of hashtables.
Example: `-InlineAttachments @{src="C:\1.jpg";cid="image1"},@{src="C:\2.jpg";cid="image2"}`.
Next we should provide an HTML template to `-HTMLBody` parameter.
You could create pretty HTML templates by using mjml. See the [documentation for mjml](https://documentation.mjml.io/) to use it.
In the HTML template we should referenced inline attachments by its ContentID or `cid`.
Example: `<img height="auto" src="cid:my_image" style="border:0;display:block;outline:none;text-decoration:none;height:auto;width:100%;font-size:13px;" width="100">`.

See example of usage below. HTMLBody for this example was generated using [live version of mjml rebderer](https://mjml.io/try-it-live/).

```Powershell
# See `src="cid:my_image"` at HTMLBody. It means we should provide InlineAttachment with cid=my_image to be able to use this embedded image.
$HTMLBody = @"
<!doctype html><html xmlns="http://www.w3.org/1999/xhtml" xmlns:v="urn:schemas-microsoft-com:vml" xmlns:o="urn:schemas-microsoft-com:office:office"><head><title></title><!--[if !mso]><!--><meta http-equiv="X-UA-Compatible" content="IE=edge"><!--<![endif]--><meta http-equiv="Content-Type" content="text/html; charset=UTF-8"><meta name="viewport" content="width=device-width,initial-scale=1"><style type="text/css">#outlook a { padding:0; }
          body { margin:0;padding:0;-webkit-text-size-adjust:100%;-ms-text-size-adjust:100%; }
          table, td { border-collapse:collapse;mso-table-lspace:0pt;mso-table-rspace:0pt; }
          img { border:0;height:auto;line-height:100%; outline:none;text-decoration:none;-ms-interpolation-mode:bicubic; }
          p { display:block;margin:13px 0; }</style><!--[if mso]>
        <noscript>
        <xml>
        <o:OfficeDocumentSettings>
          <o:AllowPNG/>
          <o:PixelsPerInch>96</o:PixelsPerInch>
        </o:OfficeDocumentSettings>
        </xml>
        </noscript>
        <![endif]--><!--[if lte mso 11]>
        <style type="text/css">
          .mj-outlook-group-fix { width:100% !important; }
        </style>
        <![endif]--><style type="text/css">@media only screen and (min-width:480px) {
        .mj-column-per-100 { width:100% !important; max-width: 100%; }
      }</style><style media="screen and (min-width:480px)">.moz-text-html .mj-column-per-100 { width:100% !important; max-width: 100%; }</style><style type="text/css">@media only screen and (max-width:480px) {
      table.mj-full-width-mobile { width: 100% !important; }
      td.mj-full-width-mobile { width: auto !important; }
    }</style></head><body style="word-spacing:normal;"><div><!--[if mso | IE]><table align="center" border="0" cellpadding="0" cellspacing="0" class="" style="width:600px;" width="600" ><tr><td style="line-height:0px;font-size:0px;mso-line-height-rule:exactly;"><![endif]--><div style="margin:0px auto;max-width:600px;"><table align="center" border="0" cellpadding="0" cellspacing="0" role="presentation" style="width:100%;"><tbody><tr><td style="direction:ltr;font-size:0px;padding:20px 0;text-align:center;"><!--[if mso | IE]><table role="presentation" border="0" cellpadding="0" cellspacing="0"><tr><td class="" style="vertical-align:top;width:600px;" ><![endif]--><div class="mj-column-per-100 mj-outlook-group-fix" style="font-size:0px;text-align:left;direction:ltr;display:inline-block;vertical-align:top;width:100%;"><table border="0" cellpadding="0" cellspacing="0" role="presentation" style="vertical-align:top;" width="100%"><tbody><tr><td align="left" style="font-size:0px;padding:10px 25px;word-break:break-word;"><div style="font-family:helvetica;font-size:20px;line-height:1;text-align:left;color:#0862a2;">Example of embedded image using `-InlineAttachments` parameter.</div></td></tr><tr><td align="center" style="font-size:0px;padding:10px 25px;word-break:break-word;"><p style="border-top:solid 4px #0862a2;font-size:1px;margin:0px auto;width:100%;"></p><!--[if mso | IE]><table align="center" border="0" cellpadding="0" cellspacing="0" style="border-top:solid 4px #0862a2;font-size:1px;margin:0px auto;width:550px;" role="presentation" width="550px" ><tr><td style="height:0;line-height:0;"> &nbsp;
</td></tr></table><![endif]--></td></tr><tr><td align="center" style="font-size:0px;padding:10px 25px;word-break:break-word;"><table border="0" cellpadding="0" cellspacing="0" role="presentation" style="border-collapse:collapse;border-spacing:0px;"><tbody><tr><td style="width:100px;"><img height="auto" src="cid:my_image" style="border:0;display:block;outline:none;text-decoration:none;height:auto;width:100%;font-size:13px;" width="100"></td></tr></tbody></table></td></tr></tbody></table></div><!--[if mso | IE]></td></tr></table><![endif]--></td></tr></tbody></table></div><!--[if mso | IE]></td></tr></table><![endif]--></div></body></html>
"@

Send-MailMessage2 -To "bob@contoso.com" -From "fred@contoso.com" -Subject "Test embedded image" -HTMLBody $HTMLBody -InlineAttachments @{src="C:\ImageWithAnyName.jpg";cid="my_image"} -SmtpServer "smtp.contoso.com" -Port 25
```

### Example 10: Send message Delivery Status Notifications (DSN)

```Powershell
Send-MailMessage2 -To "bob@contoso.com" -From "fred@contoso.com" -Subject "Welcome" -TextBody "Welcolme to Contoso!" -SmtpServer "smtp.contoso.com" -Port 25 -DeliveryStatusNotificationOptions "Failure", "Delay" -DeliveryStatusNotificationTypeOption "HeadersOnly"
```

## PARAMETERS

### -Attachments

The attachments. Specify an array of paths to file attachments that will be added to the message.

```yaml
Type: System.String[]
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

### -BccList

Set the list of addresses in the Bcc header. Recipients in the Blind-Carbon-Copy list will not be visible to the other recipients of the message.

```yaml
Type: MimeKit.InternetAddressList
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

### -CcList

Set the list of addresses that will be in the Cc header.

The addresses in the Cc header are secondary recipients of the message and are usually not the individuals being directly addressed in the content of the message.

```yaml
Type: MimeKit.InternetAddressList
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

### -ClientCertificates

Some servers may require the client SSL certificates in order to allow the user to connect. This parameter set the client SSL certificates that will be used when connecting to SMTP server.

```yaml
Type: System.Security.Cryptography.X509Certificates.X509Certificate2[]
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

### -Confirm

If used as `-Confirm:$true` then prompts you for confirmation before sending each message.

```yaml
Type: System.Management.Automation.SwitchParameter
DefaultValue: ''
SupportsWildcards: false
Aliases:
- cf
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

### -Date

Set the date of the message. Defaults to current date and offset based on timezone.

```yaml
Type: System.DateTimeOffset
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

### -DeliveryStatusNotificationOptions

Set the delivery status notification types.

You can specify the conditions under which the mail server will send a Delivery Status Notification. To define the conditions for receiving a DSN, you can use an array of possible values: "Failure", "Delay", "Success".

```yaml
Type: System.String[]
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
AcceptedValues: ["Failure", "Delay", "Success"]
HelpMessage: ''
```

### -DeliveryStatusNotificationTypeOption

When using Delivery Status Notification (DSN) via the DeliveryStatusNotificationOptions parameter, DeliveryStatusNotificationTypeOption determines whether the SMTP server's DSN response contains only headers or the full original message. The default setting is Unspecified, letting the server decide the level of detail.

```yaml
Type: System.String
DefaultValue: 'Unspecified'
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
AcceptedValues: ["Unspecified", "Full", "HeadersOnly"]
HelpMessage: ''
```

### -From

The list of addresses that will be set in the `From` header.

The `From` header specifies the author(s) of the message.

If more than one mail address is provided to `-From` parameter, `-Sender` parameter should be used to provide single mail address of the personal actually sending the message.

```yaml
Type: MimeKit.InternetAddressList
DefaultValue: ''
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: (All)
  Position: Named
  IsRequired: true
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -HtmlBody

Specify the html formatted version of the message body. Html template may link to any of the LinkedResources if they are provided with -InlineAttachments parameter.

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

### -Importance

The message importance value. Indicates the importance of a message.

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
AcceptedValues: ["Low", "Normal", "High"]
HelpMessage: ''
```

### -InlineAttachments

Inline Attachments could be referenced in message HTML body by their ContentID.
Image paths and their CIDs should be provided in a form of hashtables array.

Each hashtable must have `src` and `cid` keys.
Hashtable structure: `@{src="path_to_file";cid="unique_content_id"}`
Usage example: -InlineAttachments @{src="C:\1.jpg";cid="image_1"},@{src="C:\2.jpg";cid="image_2"}

```yaml
Type: System.Collections.Hashtable[]
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

### -LogPath

Log file path. If specified file is missing it will be created.

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

### -ReplyTo

The list of addresses in the Reply-To header.

When the list of addresses in the Reply-To header is not empty, it contains the address(es) where the author(s) of the message prefer that replies be sent.

When the list of addresses in the Reply-To header is empty, replies should be sent to the mailbox(es) specified in the From header.

```yaml
Type: MimeKit.InternetAddressList
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

### -Sender

The address in the Sender header. The sender may differ from the addresses in From if the message was sent by someone on behalf of someone else.

```yaml
Type: MimeKit.MailboxAddress
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

### -Subject

Set the subject of the message.

The Subject is typically a short string denoting the topic of the message.

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

### -TextBody

Set the text body of the message.

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

### -To

The list of addresses that will be set in the `To` header.

The addresses in the `To` header are the primary recipients of the message.

```yaml
Type: MimeKit.InternetAddressList
DefaultValue: ''
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: (All)
  Position: Named
  IsRequired: true
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -WhatIf

Runs the command in a mode that only reports what would happen without performing the actions.

```yaml
Type: System.Management.Automation.SwitchParameter
DefaultValue: ''
SupportsWildcards: false
Aliases:
- wi
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

### CommonParameters

This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable,
-InformationAction, -InformationVariable, -OutBuffer, -OutVariable, -PipelineVariable,
-ProgressAction, -Verbose, -WarningAction, and -WarningVariable. For more information, see
[about_CommonParameters](https://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

## OUTPUTS

### System.Void

None

## NOTES

## RELATED LINKS

[Online Version](https://github.com/ahpooch/PSMailKit/blob/main/help/PSMailKit/Send-MailMessage2.md)

#region Using directives
using System;
using System.Management.Automation;
using System.Security.Cryptography.X509Certificates;
using System.Runtime.InteropServices;
using MailKit;
using MailKit.Net.Smtp;
using MimeKit;
using System.Linq;
using System.Collections;
using MimeKit.Utils;
#endregion Using directives

namespace PSMailKit.Commands
{
    [Cmdlet(VerbsCommunications.Send, "MailMessage2", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.None)]
    [OutputType(typeof(void))]
    public class SendMailMessage2Command : PSCmdlet
    {
        #region Parameter Properties
        [Parameter(Mandatory = true, Position = 0)]
        [ValidateNotNullOrEmpty]
        public string SmtpServer { get; set; }

        [Parameter(Mandatory = true, Position = 1)]
        [ValidateRange(0, 65535)]
        public int Port { get; set; }

        [Parameter(Mandatory = true)]
        public InternetAddressList From { get; set; }

        [Parameter(Mandatory = false)]
        public MailboxAddress Sender { get; set; }

        [Parameter(Mandatory = true)]
        public InternetAddressList To { get; set; }

        [Parameter(Mandatory = false)]
        [ValidateNotNullOrEmpty]
        public InternetAddressList ReplyTo { get; set; }

        [Parameter(Mandatory = false)]
        [ValidateNotNullOrEmpty]
        public InternetAddressList CcList { get; set; }

        [Parameter(Mandatory = false)]
        [ValidateNotNullOrEmpty]
        public InternetAddressList BccList { get; set; }

        [Parameter(Mandatory = false)]
        public string Subject { get; set; } = string.Empty;

        [Parameter(Mandatory = false)]
        public string TextBody { get; set; }

        [Parameter(Mandatory = false)]
        public string HtmlBody { get; set; }

        [Parameter(Mandatory = false)]
        public string[] Attachments { get; set; }

        [Parameter(Mandatory = false)]
        [ValidateInlineAttachments]
        public Hashtable[] InlineAttachments { get; set; }

        [Parameter(Mandatory = false)]
        [ValidateSet("Low", "Normal", "High")]
        public string Importance = "Normal";

        [Parameter(Mandatory = false)]
        [ValidateSet("Failure", "Delay", "Success")]
        public string[] DeliveryStatusNotificationOptions;

        [Parameter(Mandatory = false)]
        [ValidateSet("Unspecified", "Full", "HeadersOnly")]
        public string DeliveryStatusNotificationTypeOption { get; set; }

        [Parameter(Mandatory = false)]
        public DateTimeOffset Date { get; set; }

        [Parameter(Mandatory = false)]
        public PSCredential Credential { get; set; }

        [Parameter(Mandatory = false)]
        [ValidateSet("Auto", "StartTls", "StartTlsWhenAvailable", "SslOnConnect")]
        public string SecureSocketOption { get; set; }

        [Parameter(Mandatory = false)]
        [ValidateClientCertificates]
        public X509Certificate2[] ClientCertificates { get; set; }

        [Parameter(Mandatory = false)]
        public bool CheckCertificateRevocation = true;

        [Parameter(Mandatory = false)]
        public string LogPath { get; set; }
        #endregion Parameter Properties

        #region Instance Data
        private ExtendedSmtpClient client;
        private string password;
        #endregion Instance Data

        #region Event Handlers
        private void OnConnected(object sender, ConnectedEventArgs e)
        {
            WriteDebug($"Successfully connected to the SMTP server: {e.Host}:{e.Port}");
            WriteDebug($"Currently using parameters: {e.Options}");
        }

        private void OnDisconnected(object sender, DisconnectedEventArgs e)
        {
            WriteDebug($"Disconnected from the SMTP server: {e.Host}:{e.Port}");
            WriteDebug($"Previously used parameters: {e.Options}");
            WriteDebug($"Is service was explicitly asked to disconnect: {e.IsRequested}");
        }

        private void OnAuthenticated(object sender, AuthenticatedEventArgs e)
        {
            WriteDebug($"Successfully authenticated on the SMTP server.");
            if (!string.IsNullOrEmpty(e.Message))
            {
                WriteDebug($"SMTP server send this free-form text in response to a successful login: {e.Message}");
            }
            else
            {
                WriteDebug($"SMTP server did not send any free-form text in response to a successful login.");
            }
        }
        #endregion Event Handlers
        protected override void BeginProcessing()
        {
            if (LogPath != null)
            {
                client = new ExtendedSmtpClient(new ProtocolLogger(LogPath));
            }
            else
            {
                client = new ExtendedSmtpClient();
            }

            client.CheckCertificateRevocation = CheckCertificateRevocation;

            if (ClientCertificates?.Count() > 0)
            {
                foreach (X509Certificate2 certificate in ClientCertificates)
                {
                    client.ClientCertificates.Add(certificate);
                }
            }

            MailKit.Security.SecureSocketOptions SecureSocketOptions = MailKit.Security.SecureSocketOptions.None;
            if (SecureSocketOption == "StartTls") { SecureSocketOptions = MailKit.Security.SecureSocketOptions.StartTls; }
            else if (SecureSocketOption == "StartTlsWhenAvailable") { SecureSocketOptions = MailKit.Security.SecureSocketOptions.StartTlsWhenAvailable; }
            else if (SecureSocketOption == "SslOnConnect") { SecureSocketOptions = MailKit.Security.SecureSocketOptions.SslOnConnect; }
            else if (SecureSocketOption == "Auto") { SecureSocketOptions = MailKit.Security.SecureSocketOptions.Auto; }

            #region DeliveryStatusNotification
            if (DeliveryStatusNotificationOptions?.Count() > 0)
            {
                client.DSNOptions = DeliveryStatusNotificationOptions;

                #region DeliveryStatusNotificationType
                if (DeliveryStatusNotificationTypeOption == "Full")
                {
                    client.DeliveryStatusNotificationType = DeliveryStatusNotificationType.Full;
                }
                else if (DeliveryStatusNotificationTypeOption == "HeadersOnly")
                {
                    client.DeliveryStatusNotificationType = DeliveryStatusNotificationType.HeadersOnly;
                }
                else
                {
                    client.DeliveryStatusNotificationType = DeliveryStatusNotificationType.Unspecified;
                }
                #endregion DeliveryStatusNotificationType
            }
            #endregion DeliveryStatusNotification

            #region SmtpClient Connection
            try
            {
                client.Connected += OnConnected;
                client.Disconnected += OnDisconnected;
                client.Connect(SmtpServer, Port, SecureSocketOptions);
                client.Connected -= OnConnected;

                WriteDebug($"Negotiated the following SSL options with {SmtpServer}:");
                WriteDebug($"        Protocol Version: {client.SslProtocol}");
                WriteDebug($"        Cipher Algorithm: {client.SslCipherAlgorithm}");
                WriteDebug($"         Cipher Strength: {client.SslCipherStrength}");
                WriteDebug($"          Hash Algorithm: {client.SslHashAlgorithm}");
                WriteDebug($"           Hash Strength: {client.SslHashStrength}");
                WriteDebug($"  Key-Exchange Algorithm: {client.SslKeyExchangeAlgorithm}");
                WriteDebug($"   Key-Exchange Strength: {client.SslKeyExchangeStrength}");
            }
            catch (Exception e)
            {
                var errorRecord = new ErrorRecord(e, "Connection failed.", ErrorCategory.ConnectionError, null);
                ThrowTerminatingError(errorRecord);
            }
            #endregion SmtpClient Connection

            #region SmtpClient Authentication
            if (client.Capabilities.HasFlag(SmtpCapabilities.Authentication))
            {
                if (Credential == null)
                {
                    PSCredential ProvidedСredential = Host.UI.PromptForCredential(
                        caption: "SMTP Server requires Authentication to proceed",
                        message: "Enter your credentials for SMTP connection",
                        userName: "",
                        targetName: ""
                    );
                    Credential = ProvidedСredential;
                }
                IntPtr ptr = Marshal.SecureStringToBSTR(Credential.Password);
                password = Marshal.PtrToStringBSTR(ptr);

                try
                {
                    client.Authenticated += OnAuthenticated;
                    client.Authenticate(Credential.UserName, password);
                    client.Authenticated -= OnAuthenticated;
                }
                catch (Exception e)
                {
                    var errorRecord = new ErrorRecord(e, "Authentication failed", ErrorCategory.AuthenticationError, null);
                    ThrowTerminatingError(errorRecord);
                }
            }
            #endregion SmtpClient Authentication
        }

        // protected override void ProcessRecord()
        // {

        // }

        protected override void EndProcessing()
        {
            #region Message Construction
            MimeMessage Message = new MimeMessage();

            Message.From.AddRange(From);

            if (Sender != null)
            {
                Message.Sender = Sender;
            }

            Message.To.AddRange(To);

            if (ReplyTo != null)
            {
                Message.ReplyTo.AddRange(ReplyTo);
            }

            if (CcList != null)
            {
                Message.Cc.AddRange(CcList);
            }

            if (BccList != null)
            {
                Message.Bcc.AddRange(BccList);
            }

            Message.Subject = Subject;

            BodyBuilder Body = new BodyBuilder();

            if (!string.IsNullOrWhiteSpace(TextBody))
            {
                Body.TextBody = TextBody;
            }

            if (!string.IsNullOrWhiteSpace(HtmlBody))
            {
                Body.HtmlBody = HtmlBody;
            }

            if (Attachments?.Length > 0)
            {
                foreach (string Attachment in Attachments)
                {
                    Body.Attachments.Add(Attachment);
                }
            }

            if (InlineAttachments?.Length > 0)
            {
                foreach (Hashtable InlineAttachment in InlineAttachments)
                {
                    var image = Body.LinkedResources.Add(InlineAttachment["src"] as string);
                    image.ContentId = MimeUtils.ParseMessageId(InlineAttachment["cid"] as string);
                }
            }

            Message.Body = Body.ToMessageBody();

            if (Importance != null)
            {
                Message.Importance = (MessageImportance)Enum.Parse(typeof(MessageImportance), Importance);
            }

            if (Date != null)
            {
                Message.Date = Date;
            }
            #endregion Message Construction

            #region Message Sending
            string WhatIfActionDescription = "Mail message for sending:";
            WhatIfActionDescription += $"\n    From: {From}";
            if (Sender != null) { WhatIfActionDescription += $"\n    Sender: {Sender}."; }
            WhatIfActionDescription += $"\n    To: {To}.";
            if (ReplyTo != null) { WhatIfActionDescription += $"\n    ReplyTo: {ReplyTo}."; }
            if (CcList != null) { WhatIfActionDescription += $"\n    CcList: {CcList}."; }
            if (BccList != null) { WhatIfActionDescription += $"\n    BccList: {CcList}."; }
            WhatIfActionDescription += Subject != null ? $"\n    Subject: {Subject}." : "Subject: None.";
            if (TextBody != null) { WhatIfActionDescription += $"\n    TextBody: {TextBody.Length} characters."; }
            if (HtmlBody != null) { WhatIfActionDescription += $"\n    HtmlBody: {HtmlBody.Length} characters."; }
            if (Attachments?.Count() > 0) { WhatIfActionDescription += $"\n    Attachmens: has {Attachments.Count()} item(s)."; }
            WhatIfActionDescription += $"\n    Importance: {Importance}";

            if (ShouldProcess(
                WhatIfActionDescription,
                "Message sending confirmation."
                ))
            {
                client.Send(Message);
            }
            #endregion Message Sending

            #region Cleaning
            try
            {
                if (client != null && client.IsConnected)
                {
                    client.Disconnect(true);
                    client.Disconnected -= OnDisconnected;
                }

                // Clearing password from memory for security reasons
                if (password != null)
                {
                    for (int i = 0; i < password.Length; i++)
                    {
                        password = password.Remove(i, 1).Insert(i, "\0");
                    }
                }
            }
            finally
            {
                client?.Dispose();
            }
            #endregion Cleaning
        }
    }
}


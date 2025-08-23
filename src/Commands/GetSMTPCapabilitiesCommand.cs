#region Using Directives
using System;
using System.Management.Automation;
using MailKit;
using MailKit.Net.Smtp;
using MimeKit;
using System.Runtime.InteropServices;
#endregion Using Directives

namespace PSMailKit.Commands
{
    [Cmdlet(VerbsCommon.Get, "SMTPCapabilities")]
    [OutputType(typeof(ExtensionInfo))]
    public class GetSMTPCapabilitiesCommand : PSCmdlet
    {
        #region Parameter Properties
        [Parameter(Mandatory = true, Position = 0)]
        [ValidateNotNullOrEmpty]
        public string SmtpServer { get; set; }

        [Parameter(Mandatory = true, Position = 1)]
        [ValidateRange(0, 65535)]
        public int Port { get; set; }

        [Parameter(Mandatory = false)]
        public PSCredential Credential { get; set; }

        [Parameter(Mandatory = false)]
        [ValidateSet("Auto", "StartTls", "StartTlsWhenAvailable", "SslOnConnect")]
        public string SecureSocketOption { get; set; }

        [Parameter(Mandatory = false)]
        public bool CheckCertificateRevocation = true;
        #endregion Parameter Properties

        #region Instance Data
        private SmtpClient client;
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
                WriteDebug($"SMTP server did not send any free-form text in response to a successful login");
            }
        }
        #endregion Event Handlers

        public class ExtensionInfo
        {
            public string Extension { get; set; }
            public string Description { get; set; }
            public string Link { get; set; }
        }
        protected override void BeginProcessing()
        {
            client = new SmtpClient
            {
                CheckCertificateRevocation = CheckCertificateRevocation
            };
            #region SmtpClient Connection
            try
            {
                MailKit.Security.SecureSocketOptions SecureSocketOptions = MailKit.Security.SecureSocketOptions.None;
                if (SecureSocketOption == "StartTls") { SecureSocketOptions = MailKit.Security.SecureSocketOptions.StartTls; }
                else if (SecureSocketOption == "StartTlsWhenAvailable") { SecureSocketOptions = MailKit.Security.SecureSocketOptions.StartTlsWhenAvailable; }
                else if (SecureSocketOption == "SslOnConnect") { SecureSocketOptions = MailKit.Security.SecureSocketOptions.SslOnConnect; }
                else if (SecureSocketOption == "Auto") { SecureSocketOptions = MailKit.Security.SecureSocketOptions.Auto; }

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
                    this.Credential = ProvidedСredential;
                }
                IntPtr ptr = Marshal.SecureStringToBSTR(this.Credential.Password);
                password = Marshal.PtrToStringBSTR(ptr);

                try
                {
                    client.Authenticated += OnAuthenticated;
                    client.Authenticate(this.Credential.UserName, password);
                    client.Authenticated -= OnAuthenticated;
                }
                catch (Exception e)
                {
                    var errorRecord = new ErrorRecord(e, "Authentication failed", ErrorCategory.AuthenticationError, null);
                    ThrowTerminatingError(errorRecord);
                }
            }
            #endregion SmtpClient Authentication

            if (client.Capabilities.HasFlag(SmtpCapabilities.Size))
            {
                ExtensionInfo ext_size = new ExtensionInfo
                {
                    Extension = "SIZE",
                    Description = $"The SMTP server supports SIZE extension and has a size restriction on messages: {string.Format("{0:0.00} Mb", (client.MaxSize / (1024 * 1024)))} ({client.MaxSize}).\nRead more about SIZE extension in RFC 1870.\n",
                    Link = "https://datatracker.ietf.org/doc/html/rfc1870"
                };
                WriteObject(ext_size);
            }

            if (client.Capabilities.HasFlag(SmtpCapabilities.Dsn))
            {
                ExtensionInfo ext_dsn = new ExtensionInfo
                {
                    Extension = "DSN",
                    Description = $"The SMTP server supports DSN extension for delivery-status notifications.\nRead more about DSN extension in RFC 1891.\n",
                    Link = "https://datatracker.ietf.org/doc/html/rfc1891"
                };
                WriteObject(ext_dsn);
            }

            if (client.Capabilities.HasFlag(SmtpCapabilities.EnhancedStatusCodes))
            {
                ExtensionInfo ext_enhancedstatuscodes = new ExtensionInfo
                {
                    Extension = "ENHANCEDSTATUSCODES",
                    Description = "The server supports the ENHANCEDSTATUSCODES extension.\nRead more about ENHACEDSTATUSCODES extension in RFC 2034.\n",
                    Link = "https://datatracker.ietf.org/doc/html/rfc2034"
                };
                WriteObject(ext_enhancedstatuscodes);
            }

            if (client.Capabilities.HasFlag(SmtpCapabilities.Authentication))
            {
                String mechanisms = string.Join(", ", client.AuthenticationMechanisms);
                ExtensionInfo ext_auth = new ExtensionInfo
                {
                    Extension = "AUTH",
                    Description = $"The SMTP server supports AUTH extension and the following SASL mechanisms: {mechanisms}.\nRead more about AUTH extension in RFC 2554.\n",
                    Link = "https://datatracker.ietf.org/doc/html/rfc2554"
                };
                WriteObject(ext_auth);
            }

            if (client.Capabilities.HasFlag(SmtpCapabilities.EightBitMime))
            {
                ExtensionInfo ext_8bitmime = new ExtensionInfo
                {
                    Extension = "8BITMIME",
                    Description = "The SMTP server supports 8BITMIME extension.\nContent-Transfer-Encoding: 8bit.\nRead more about 8BITMIME extension in RFC 2821.\n",
                    Link = "https://datatracker.ietf.org/doc/html/rfc2821"
                };
                WriteObject(ext_8bitmime);
            }

            if (client.Capabilities.HasFlag(SmtpCapabilities.Pipelining))
            {
                ExtensionInfo ext_pipelining = new ExtensionInfo
                {
                    Extension = "PIPELINING",
                    Description = "The SMTP server supports PIPELINING extension, allowing clients to send multiple commands at once in order to reduce round-trip latency.\nRead more about IPELINING extension in RFC 2920.\n",
                    Link = "https://datatracker.ietf.org/doc/html/rfc2920"
                };
                WriteObject(ext_pipelining);
            }

            if (client.Capabilities.HasFlag(SmtpCapabilities.BinaryMime))
            {
                ExtensionInfo ext_binarymime = new ExtensionInfo
                {
                    Extension = "BINARYMIME",
                    Description = "The SMTP server supports BINARYMIME extension.\nContent-Transfer-Encoding: binary.\nRead more about BINARYMIME extension in RFC 3030.\n",
                    Link = "https://datatracker.ietf.org/doc/html/rfc3030"
                };
                WriteObject(ext_binarymime);
            }

            if (client.Capabilities.HasFlag(SmtpCapabilities.Chunking))
            {
                ExtensionInfo ext_chunking = new ExtensionInfo
                {
                    Extension = "CHUNKING",
                    Description = "The SMTP server supports CHUNKING extension allowing clients to upload messages in chunks.\nRead more about CHUNKING extension in RFC 3030.\n",
                    Link = "https://datatracker.ietf.org/doc/html/rfc3030"
                };
                WriteObject(ext_chunking);
            }

            if (client.Capabilities.HasFlag(SmtpCapabilities.StartTLS))
            {
                ExtensionInfo ext_starttls = new ExtensionInfo
                {
                    Extension = "STARTTLS",
                    Description = "The SMTP server supports STARTTLS extension allowing clients to switch to an encrypted SSL/TLS connection after connecting.\nRead more about STARTTLS extension in RFC 3207.\n",
                    Link = "https://datatracker.ietf.org/doc/html/rfc3207"
                };
                WriteObject(ext_starttls);
            }

            if (client.Capabilities.HasFlag(SmtpCapabilities.UTF8))
            {
                ExtensionInfo ext_smtputf8 = new ExtensionInfo
                {
                    Extension = "SMTPUTF8",
                    Description = "The SMTP server supports SMTPUTF8 extension allowing usage of UTF-8 in message headers.\nRead more about SMTPUTF8 extension in RFC 6531.\n",
                    Link = "https://datatracker.ietf.org/doc/html/rfc6531"
                };
                WriteObject(ext_smtputf8);
            }

            if (client.Capabilities.HasFlag(SmtpCapabilities.RequireTLS))
            {
                ExtensionInfo ext_requiretls = new ExtensionInfo
                {
                    Extension = "SMTPUTF8",
                    Description = "The SMTP Server supports the REQUIRETLS extension allowing usage of message header field TLS-Required.\nRead more about REQURETLS extension in RFC 8689.\n",
                    Link = "https://datatracker.ietf.org/doc/html/rfc8689"
                };
                WriteObject(ext_requiretls);
            }
        }

        // protected override void ProcessRecord()
        // {

        // }

        protected override void EndProcessing()
        {
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
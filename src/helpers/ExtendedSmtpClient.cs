#region Using directives
using System;
using MailKit;
using MailKit.Net.Smtp;
using MimeKit;
#endregion Using directives

public class ExtendedSmtpClient : SmtpClient
{
    private DeliveryStatusNotification _dsnFlags = DeliveryStatusNotification.Never;

    public string[] DSNOptions
    {
        set
        {
            foreach (var type in value)
            {
                if (Enum.TryParse(type, out DeliveryStatusNotification flag))
                {
                    _dsnFlags |= flag;
                }
            }
        }
    }

    public ExtendedSmtpClient() : base()
    {
    }

    public ExtendedSmtpClient(ProtocolLogger protocolLogger) : base(protocolLogger)
    {
    }

    protected override string GetEnvelopeId(MimeMessage message)
    {
        return message.MessageId;
    }

    protected override DeliveryStatusNotification? GetDeliveryStatusNotifications(MimeMessage message, MailboxAddress mailbox)
    {
        return _dsnFlags;
    }
}
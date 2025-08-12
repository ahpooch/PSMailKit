using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Management.Automation;

[AttributeUsage(AttributeTargets.Property)]
public class ValidateInlineAttachmentsAttribute : ValidateArgumentsAttribute
{
    protected override void Validate(object arguments, EngineIntrinsics engineIntrinsics)
    {
        if (!(arguments is Hashtable[] attachments)) { return; }

        var cidSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var index = 0;

        foreach (Hashtable attachment in attachments)
        {
            index++;
            try
            {
                ValidateAttachment(attachment, index, cidSet);
            }
            catch (ValidationMetadataException ex)
            {
                throw new ValidationMetadataException($"Invalid attachment at index {index}: {ex.Message}");
            }
        }
    }

    private void ValidateAttachment(Hashtable attachment, int index, HashSet<string> cidSet)
    {
        if (attachment == null)
        {
            throw new ValidationMetadataException("Attachment cannot be null");
        }

        if (!attachment.ContainsKey("src"))
        {
            throw new ValidationMetadataException("Missing required 'src' key");
        }

        if (!attachment.ContainsKey("cid"))
        {
            throw new ValidationMetadataException("Missing required 'cid' key");
        }

        string src = attachment["src"] as string;
        string cid = attachment["cid"] as string;

        if (string.IsNullOrWhiteSpace(src))
        {
            throw new ValidationMetadataException("'src' must be a non-empty string");
        }

        if (string.IsNullOrWhiteSpace(cid))
        {
            throw new ValidationMetadataException("'cid' must be a non-empty string");
        }

        if (!File.Exists(src))
        {
            throw new ValidationMetadataException($"File not found: '{src}'");
        }

        if (!cidSet.Add(cid))
        {
            throw new ValidationMetadataException($"Duplicate CID detected: '{cid}'");
        }
    }
}
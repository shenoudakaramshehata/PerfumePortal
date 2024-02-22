using System.ComponentModel.DataAnnotations;

namespace CRM.Models
{
    public partial class SocialMediaLink
    {
        [Key]
        public int SocialMediaLinkId { get; set; }
        public string Facebook { get; set; }=string.Empty;
        public string LinkedIn { get; set; }=string.Empty ;
        public string Instgram { get; set; } = string.Empty;
        public string Twitter { get; set; } = string.Empty;
        public string Fax { get; set; } = string.Empty;
        public string Youtube { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string WhatSapp { get; set; } = string.Empty;

        public string ContactMail { get; set; } = string.Empty;
        public string ContactPhone1 { get; set; } = string.Empty;
        public string ContactPhone2 { get; set; } = string.Empty;
    }
}

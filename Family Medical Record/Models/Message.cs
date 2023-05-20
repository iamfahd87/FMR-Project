using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Family_Medical_Record.Models
{
    public class Message
    {
        [Key]
        public int MessageId { get; set; }
        public string Text { get; set; }
        public DateTime TimeStamp { get; set; }

        public string Sender { get; set; }
        public string Receptient { get; set; }

        [ScaffoldColumn(false)]
        public bool HasSeen { get; set; } = false;
    }
}
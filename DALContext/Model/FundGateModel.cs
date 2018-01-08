using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DALContext.Model
{
    public class FUNDGATE_REQUEST
    {
        public int ID { get; set; }
        [MaxLength(25)]
        public string action { get; set; }
        [MaxLength(25)]
        public string terminal { get; set; }
        [MaxLength(25)]
        public string amount { get; set; }
        [MaxLength(25)]
        public string destination { get; set; }
        [MaxLength(50)]
        public string clientRef { get; set; }
        [MaxLength(25)]
        public string endPoint { get; set; }
        [MaxLength(25)]
        public string lineType { get; set; }
        [MaxLength(50)]
        public string ipAddress { get; set; }
        public DateTime created {get;set;}
        [MaxLength(50)]
        public string sender { get; set; }
 
    }

    public class FUNDGATE_RESPONSE
    {
        [Key]
        public int respId { get; set; }
        [MaxLength(50)]
        public string action {get;set;}
        [MaxLength(50)]
        public string terminal {get;set; }
        [MaxLength(50)]
        public string etzRef {get;set; }
        
        public string respmessage { get; set; }
        [MaxLength(50)]
        public string clientRef {get;set;}
 public DateTime created { get; set; }
        [MaxLength(5)]
        public string respCode { get; set; }
 
    }
}

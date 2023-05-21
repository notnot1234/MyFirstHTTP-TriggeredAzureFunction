using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace MyFirstHTTP_TriggeredAzureFunction
{


    public class Sales{

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public Guid Id { get; set; }
        //BranchId, INT, not nullable

        public int BranchId { get; set; }

        [Required]
        [Column(TypeName = "NVARCHAR")]
        [StringLength(30)]
        public string TransactionId { get; set; }

        [Column(TypeName = "Datetime2")]
        public DateTime TransactionDate { get; set; }

        public decimal Amount { get; set; }

        [Column(TypeName = "NVARCHAR")]
        [StringLength(30)]
        public string? LoyaltyCardNumber { get; set; }
    


    }
}

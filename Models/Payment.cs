using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Melotrade.Models
{
    [Table("payments")]
    public class Payment
    {
        [Key]
        public int id { get; set; }

        [Column("tblname")]
        public string TblName { get; set; } = string.Empty;

        [Column("tblsurname")]
        public string TblSurname { get; set; } = string.Empty;

        [Column("tblhospitalid")]
        public string TblHospitalId { get; set; } = string.Empty;

        [Column("tblitemid")]
        public string TblItemId { get; set; } = string.Empty;

        [Column("type")]
        public string Type { get; set; } = string.Empty;

        [Column("active")]
        public string Active { get; set; } = "N";

        [Column("dday")]
        public int DDay { get; set; }

        [Column("mmonth")]
        public int MMonth { get; set; }

        [Column("yyear")]
        public int YYear { get; set; }

        [Column("tbltime")]
        public string TblTime { get; set; } = string.Empty;

        [Column("orderno")]
        public string OrderNo { get; set; } = string.Empty;
        public string Amount { get; set; } = string.Empty;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcApplication.Models.EF
{
    public partial class PhoneList
    {
        public List<FlowDetail> flowDetail { get; set; }
        public List<FlowBill> flowBill { get; set; }
        public List<BillData> billData { get; set; }
        public List<MessageData> messageData { get; set; }
        public List<TelData> telData { get; set; }
    }
}
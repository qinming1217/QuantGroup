using MvcApplication.Models;
using MvcApplication.Models.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcApplication.DAL
{
    public class BaseDAL
    {
        LoanEntities _loanEntities = new LoanEntities();
        public bool SavePhoneList(PhoneList phoneList)
        {
            bool result = true;
            int phoneListId = 0;
            try
            {
                if (_loanEntities.PhoneList.Select(o => o.phone == phoneList.phone).Count() == 0)
                {
                    _loanEntities.PhoneList.Add(phoneList);
                    _loanEntities.SaveChanges();
                    phoneListId = phoneList.id;

                    if (phoneList.billData != null)
                    {
                        foreach (BillData billData in phoneList.billData)
                        {
                            billData.phoneListId = phoneListId;
                            _loanEntities.BillData.Add(billData);
                        }
                    }

                    if (phoneList.flowBill != null)
                    {
                        foreach (FlowBill flowBill in phoneList.flowBill)
                        {
                            flowBill.phoneListId = phoneListId;
                            _loanEntities.FlowBill.Add(flowBill);
                        }
                    }

                    if (phoneList.flowDetail != null)
                    {
                        foreach (FlowDetail flowDetail in phoneList.flowDetail)
                        {
                            flowDetail.phoneListId = phoneListId;
                            _loanEntities.FlowDetail.Add(flowDetail);
                        }
                    }

                    if (phoneList.messageData != null)
                    {
                        foreach (MessageData messageData in phoneList.messageData)
                        {
                            messageData.phoneListId = phoneListId;
                            _loanEntities.MessageData.Add(messageData);
                        }
                    }

                    if (phoneList.telData != null)
                    {
                        foreach (TelData telData in phoneList.telData)
                        {
                            telData.phoneListId = phoneListId;
                            _loanEntities.TelData.Add(telData);
                        }
                    }
                    _loanEntities.SaveChanges();
                }
            }
            catch (Exception e)
            {
                if (phoneListId != 0)
                {
                    _loanEntities.PhoneList.Remove(phoneList);
                }
            }
            return result;
        }
    }
}
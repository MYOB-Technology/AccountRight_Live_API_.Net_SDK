﻿using MYOB.AccountRight.SDK.Contracts.Version2.Contact;
using MYOB.AccountRight.SDK.Contracts.Version2.GeneralLedger;

namespace MYOB.AccountRight.SDK.Contracts.Version2.Sale
{
    public class Invoice : Sale
    {
        public Invoice()
        {
            IsTaxInclusive = true;
            InvoiceDeliveryStatus = DocumentAction.Print;
        }

        /// <summary>
        /// The invoice type - this is only populated when querying the "/Sale/Invoice" endpoint
        /// </summary>
        public InvoiceLayoutType? InvoiceType { get; set; }

        /// <summary>
        /// ShipTo address of the sale invoice.
        /// </summary>
        /// <remarks>
        /// Not supported by Professional or Miscellaneous invoices
        /// </remarks>
        public string ShipToAddress { get; set; }

        /// <summary>
        /// Customer payment terms
        /// </summary>
        public InvoiceTerms Terms { get; set; }

        /// <summary>
        /// True indicates the transaction is set to tax inclusive.
        /// False indicates the transaction is not tax inclusive.
        /// </summary>
        public bool IsTaxInclusive { get; set; }

        /// <summary>
        /// Sum of all tax exclusive line amounts applicable to the sale invoice.
        /// </summary>
        public decimal Subtotal { get; set; }

        /// <summary>
        /// Tax freight amount applicable to the sale invoice.
        /// </summary>
        /// <remarks>
        /// Not supported by Professional or Miscellaneous invoices
        /// </remarks>
        public decimal Freight { get; set; }

        /// <summary>
        /// The freight Tax code
        /// </summary>
        /// <remarks>
        /// Not supported by Professional or Miscellaneous invoices
        /// </remarks>
        public TaxCodeLink FreightTaxCode { get; set; }

        /// <summary>
        /// Total of all tax amounts applicable to the sale invoice.
        /// </summary>
        public decimal TotalTax { get; set; }

        /// <summary>
        /// Total amount of the sale invoice.
        /// </summary>
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// The employee contact
        /// </summary>
        public CardLink Salesperson { get; set; }

        /// <summary>
        /// Sale invoice comment.
        /// </summary>
        /// <remarks>
        /// Not supported by Miscellaneous invoices
        /// </remarks>
        public string Comment { get; set; }

        /// <summary>
        /// Shipping method text.
        /// </summary>
        /// <remarks>
        /// Not supported by Professional or Miscellaneous invoices
        /// </remarks>
        public string ShippingMethod { get; set; }

        /// <summary>
        /// Journal memo text describing the sale.
        /// </summary>
        public string JournalMemo { get; set; }

        /// <summary>
        /// Referral Source selected on the sale invoice.
        /// </summary>
        public string ReferralSource { get; set; }

        /// <summary>
        /// Invoice delivery status assigned.
        /// </summary>
        /// <remarks>
        /// Not supported by Miscellaneous invoices
        /// </remarks>
        public DocumentAction InvoiceDeliveryStatus { get; set; }
    }
}
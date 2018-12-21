﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BiensiPosDbContext;
using try_bi;
using Newtonsoft.Json;
namespace try_bi
{
    class DiscountCalculate
    {
        private readonly BiensiPosDbDataContext _context;

        public DiscountCalculate(BiensiPosDbDataContext context)
        {
            _context = context;
        }


        public DiscountMaster Post(Transaction transactionApi)
        {
            List<DiscountItemAPI> discItemApiEmployee = new List<DiscountItemAPI>();
            List<DiscountApi> promotions = new List<DiscountApi>();
            List<DiscountApi> promotionsDistinc = new List<DiscountApi>();
            DiscountMaster discountMaster = new DiscountMaster();
            BiensiPosDbContext.Store store = _context.Stores.Where(c => c.CODE == transactionApi.storeCode).First();
            //for discount employee and customer voucher
            if (transactionApi.customerId != "")
            {
                try
                {
                    //for discount employee
                    //IMPORTANT : harus cek employee ID untuk discount employee
                    // bool employeeExistInCustomer = _context.Customers.Any(c => c.Address == transactionApi.customerId);
                    bool employeeInMaster = _context.Employees.Any(c => c.EMPLOYEEID == transactionApi.customerId);
                    if (employeeInMaster)
                    {
                        //Customer customer = _context.Customer.Where(c => c.CustId == transactionApi.customerId).First();

                        List<Discountretail> getDiscountEmployee = _context.Discountretails.Where(c => c.CustomerGroupId == _context.CustomerGroups.Where(d => d.DESCRIPTION == "KARYAWAN TOKO").First().Id
                        && c.DiscountCategory == 1
                        && c.Status == "true").ToList();
                        if (getDiscountEmployee.Count > 0)

                        {
                            Discountretail dl = getDiscountEmployee.First();
                            bool isExistForCustomer = true;
                            if (isExistForCustomer)
                            {
                                int numberOfUsedDiscount = 0;

                                try
                                {
                                    List<BiensiPosDbContext.Transaction> list = _context.Transactions.Where(c => c.CUSTOMERID == transactionApi.customerId).ToList();
                                    //  && c.DATE.Value.Month == DateTime.Now.Month) // ini buat validasi per bulan
                                    //cek gmn query per bulan
                                    // yang in buat validate bulan di tambahin ya
                                    ////NOTES FOR DATE

                                    for (int i = 0; i < list.Count; i++)
                                    {
                                        BiensiPosDbContext.Transaction trans = list[i];
                                        String transactionDate = trans.DATE;
                                        DateTime transactionDateTime = DateTime.Parse(transactionDate);

                                        if (transactionDateTime.Month == DateTime.Now.Month)
                                        {
                                            int numbLineDisc = _context.TransactionLines.Where(c => c.TRANSACTIONID == trans.TRANSACTIONID && c.DISCOUNT > 0).Sum(c => c.QUANTITY);
                                            numberOfUsedDiscount = numberOfUsedDiscount + numbLineDisc;
                                        }

                                    }

                                }
                                catch
                                {

                                }
                                for (int i = 0; i < transactionApi.transactionLines.Count; i++)
                                {
                                    DiscountItemAPI discount = new DiscountItemAPI();
                                    discount.amountDiscount = (transactionApi.transactionLines[i].quantity * transactionApi.transactionLines[i].price * dl.DiscountPercent) / 100;
                                    discount.articleId = transactionApi.transactionLines[i].article.articleId;
                                    discount.articleIdFk = transactionApi.transactionLines[i].articleIdFk;
                                    discount.discountCode = dl.DiscountCode;
                                    discount.discountDesc = dl.DiscountPercent + " %";
                                    discount.discountType = 1 + "";
                                    numberOfUsedDiscount = numberOfUsedDiscount + transactionApi.transactionLines[i].quantity;
                                    if (numberOfUsedDiscount <= 3)
                                    {
                                        //jika qty kurang dari 3
                                        discItemApiEmployee.Add(discount);
                                    }
                                }

                                discountMaster.discountItems = trimDiscountEmployee(discItemApiEmployee, transactionApi.customerId);// add here for auto apply
                                discountMaster.discounts = new List<DiscountApi>();

                                return discountMaster;

                            }
                        }
                        else
                        {

                            return discountMaster;
                        }
                    }
                    else
                    {

                        //   return discountMaster;
                    }
                }
                catch (Exception ex)
                {


                }
            }
            //end of employee discount


            try
            {
                //normal discount
                for (int i = 0; i < transactionApi.transactionLines.Count; i++)
                {
                    if (transactionApi.transactionLines[i].price > transactionApi.transactionLines[i].discount)
                    {
                        Article article = transactionApi.transactionLines[i].article;
                        //check brand
                        try
                        {
                            Itemdimensionbrand brand = _context.Itemdimensionbrands.Where(c => c.Description == article.brand).First();
                            List<Discountretailline> discountLinesByBrand = _context.Discountretaillines.Where(c => c.BrandCode == brand.Id).ToList();
                            for (int j = 0; j < discountLinesByBrand.Count; j++)
                            {
                                Discountretailline dl = discountLinesByBrand[j];
                                int discountId = dl.DiscountRetailId;
                                //check if store is applied to the discount or not;
                                bool isExistForCustomer = _context.Discountstores.Any(c => c.DiscountId == discountId && c.StoreId == store.Id);
                                //for check if no customer for discount and discount will be applied to all store
                                //    int numOFCustomer = _context.DiscountStore.Where(c => c.DiscountId == discountId).ToList().Count;
                                // if (isExistForCustomer || numOFCustomer == 0)
                                if (isExistForCustomer)
                                {
                                    try
                                    {
                                        //Discountretail dc = _context.Discountretails.Where(c => c.Id == dl.DiscountRetailId).First();

                                        Discountretail dc = _context.Discountretails.Where(c => c.Id == dl.DiscountRetailId && c.Status == "true").First();
                                        if (_context.CustomerGroups.Where(c => c.Id == dc.CustomerGroupId).First().DESCRIPTION.Equals("Default"))
                                        {
                                            String startDate = dc.StartDate;
                                            String endDate = dc.EndDate;

                                            DateTime startDateTime = DateTime.Parse(startDate);
                                            DateTime endDateTime = DateTime.Parse(endDate);

                                            if (startDateTime <= DateTime.UtcNow.ToLocalTime() && endDateTime >= DateTime.Now.ToLocalTime())
                                            {
                                                if (dc.DiscountCategory == RetailEnum.discountNormal)
                                                {

                                                    DiscountApi discount = new DiscountApi();
                                                    discount.id = dc.Id;
                                                    discount.discountCode = dc.DiscountCode;
                                                    discount.discountPercent = dl.DiscountPrecentage;
                                                    discount.discountAmount = dl.CashDiscount;
                                                    //for disc percent

                                                    if (discount.discountPercent > 0 && discount.discountAmount == 0)
                                                    {
                                                        discount.discountDesc = discount.discountPercent + " %";
                                                    }
                                                    else if (discount.discountPercent == 0 && discount.discountAmount > 0)
                                                    {
                                                        discount.discountDesc = (Math.Floor(discount.discountAmount) / 1000) + "K";
                                                    }

                                                    discount.status = 1;
                                                    discount.discountType = RetailEnum.discountNormal;
                                                    discount.articleId = article.articleId;
                                                    discount.articleIdFk = article.id;
                                                    if (dl.DiscountPrecentage > 0)
                                                    {
                                                        //decimal ds = (decimal)(dl.DiscountPrecentage / 100);
                                                        Decimal a = Convert.ToDecimal(dl.DiscountPrecentage);
                                                        Decimal b = Convert.ToDecimal(100);
                                                        Decimal ds = a / b;
                                                        discount.totalDiscount = transactionApi.transactionLines[i].quantity * (article.price * ds);
                                                    }
                                                    else if (dl.CashDiscount > 0)
                                                    {
                                                        discount.totalDiscount = transactionApi.transactionLines[i].quantity * dl.CashDiscount;
                                                    }

                                                    //if discount has been applied then dont apply for normal discount
                                                    if (transactionApi.transactionLines[i].discountCode == "" || transactionApi.transactionLines[i].discountCode == null)
                                                    {
                                                        promotions.Add(discount);
                                                    }

                                                }


                                                if (promotions.Any(c => c.discountCode == dc.DiscountCode) == false)
                                                {

                                                    if (dc.DiscountCategory == RetailEnum.discountMixAndMatch)
                                                    {
                                                        DiscountApi discount = new DiscountApi();
                                                        discount.id = dc.Id;
                                                        discount.status = 0;
                                                        discount.discountCode = dc.DiscountCode;
                                                        discount.discountPercent = dl.DiscountPrecentage;
                                                        discount.discountAmount = dl.CashDiscount;

                                                        //for disc percent

                                                        if (discount.discountPercent > 0 && discount.discountAmount == 0)
                                                        {
                                                            discount.discountDesc = discount.discountPercent + " %";
                                                        }
                                                        else if (discount.discountPercent == 0 && discount.discountAmount > 0)
                                                        {
                                                            discount.discountDesc = (Math.Floor(discount.discountAmount) / 1000) + "K";
                                                        }

                                                        if (transactionApi.transactionLines[i].quantity >= dl.Qty && dl.Qty > 0)
                                                        {
                                                            discount.status = 1;
                                                        }


                                                        if (dl.AmountTransaction > 0 && transactionApi.transactionLines[i].subtotal >= dl.AmountTransaction)
                                                        {
                                                            discount.status = 1;
                                                        }

                                                        discount.articleId = article.articleId;
                                                        discount.articleIdFk = article.id;
                                                        discount.discountType = RetailEnum.discountMixAndMatch;
                                                        promotions.Add(discount);


                                                    }
                                                    if (dc.DiscountCategory == RetailEnum.discountBuyAndGet)
                                                    {
                                                        DiscountApi discount = new DiscountApi();
                                                        discount.id = dc.Id;
                                                        discount.discountItem = dl.ArticleIdDiscount;
                                                        discount.discountCode = dc.DiscountCode;
                                                        discount.discountPercent = 0;
                                                        discount.discountAmount = 0;

                                                        if (transactionApi.transactionLines[i].quantity >= dl.Qty || transactionApi.transactionLines[i].subtotal >= dl.AmountTransaction)
                                                        {
                                                            discount.status = 1;
                                                        }
                                                        else
                                                        {
                                                            discount.status = 0;
                                                        }
                                                        discount.articleId = article.articleId;
                                                        discount.articleIdFk = article.id;
                                                        discount.discountType = RetailEnum.discountBuyAndGet;
                                                        promotions.Add(discount);
                                                    }

                                                }
                                            }
                                        }
                                        //===========================================================================================

                                    }
                                    catch (Exception ex)
                                    {
                                        String error = ex.ToString();
                                    }
                                    //mix and match

                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            String error = ex.ToString();


                        }

                        //by department
                        try
                        {
                            Itemdimensiondepartment department = _context.Itemdimensiondepartments.Where(c => c.Description == article.department).First();
                            List<Discountretailline> discountLinesByDepartment = _context.Discountretaillines.Where(c => c.Department == department.Id).ToList();
                            foreach (Discountretailline dl in discountLinesByDepartment)
                            {
                                //Discountretail dc = _context.Discountretails.Where(c => c.Id == dl.DiscountRetailId).First();
                                Discountretail dc = _context.Discountretails.Where(c => c.Id == dl.DiscountRetailId && c.Status == "true").First();
                                if (_context.CustomerGroups.Where(c => c.Id == dc.CustomerGroupId).First().DESCRIPTION.Equals("Default"))
                                {
                                    String startDate = dc.StartDate;
                                    String endDate = dc.EndDate;

                                    DateTime startDateTime = DateTime.Parse(startDate);
                                    DateTime endDateTime = DateTime.Parse(endDate);

                                    if (startDateTime <= DateTime.UtcNow.ToLocalTime() && endDateTime >= DateTime.Now.ToLocalTime())
                                    {
                                        if (promotions.Any(c => c.discountCode == dc.DiscountCode) == false)
                                        {
                                            if (dc.DiscountCategory == RetailEnum.discountNormal)
                                            {
                                                DiscountApi discount = new DiscountApi();
                                                discount.id = dc.Id;
                                                discount.discountCode = dc.DiscountCode;
                                                discount.discountPercent = dl.DiscountPrecentage;
                                                discount.discountAmount = dl.CashDiscount;

                                                //for disc percent

                                                if (discount.discountPercent > 0 && discount.discountAmount == 0)
                                                {
                                                    discount.discountDesc = discount.discountPercent + " %";
                                                }
                                                else if (discount.discountPercent == 0 && discount.discountAmount > 0)
                                                {
                                                    discount.discountDesc = (Math.Floor(discount.discountAmount) / 1000) + "k";
                                                }

                                                discount.status = 1;
                                                discount.discountType = RetailEnum.discountNormal;
                                                discount.articleId = article.articleId;
                                                discount.articleIdFk = article.id;
                                                if (dl.DiscountPrecentage > 0)
                                                {
                                                    discount.totalDiscount = transactionApi.transactionLines[i].quantity * (article.price * (dl.DiscountPrecentage / 100));
                                                }
                                                else if (dl.CashDiscount > 0)
                                                {
                                                    discount.totalDiscount = transactionApi.transactionLines[i].quantity * dl.CashDiscount;
                                                }
                                                //if discount has been applied then dont apply for normal discount
                                                if (transactionApi.transactionLines[i].discountCode == "" || transactionApi.transactionLines[i].discountCode == null)
                                                {
                                                    promotions.Add(discount);
                                                }
                                            }
                                            if (dc.DiscountCategory == RetailEnum.discountMixAndMatch)
                                            {

                                                DiscountApi discount = new DiscountApi();
                                                discount.id = dc.Id;
                                                discount.status = 0;
                                                discount.discountCode = dc.DiscountCode;
                                                discount.discountPercent = dl.DiscountPrecentage;
                                                discount.discountAmount = dl.CashDiscount;

                                                //for disc percent

                                                if (discount.discountPercent > 0 && discount.discountAmount == 0)
                                                {
                                                    discount.discountDesc = discount.discountPercent + " %";
                                                }
                                                else if (discount.discountPercent == 0 && discount.discountAmount > 0)
                                                {
                                                    discount.discountDesc = (Math.Floor(discount.discountAmount) / 1000) + "k";
                                                }


                                                if (transactionApi.transactionLines[i].quantity >= dl.Qty && dl.Qty > 0)
                                                {
                                                    discount.status = 1;
                                                }


                                                if (dl.AmountTransaction > 0 && transactionApi.transactionLines[i].subtotal >= dl.AmountTransaction)
                                                {
                                                    discount.status = 1;
                                                }

                                                discount.articleId = article.articleId;
                                                discount.articleIdFk = article.id;
                                                discount.discountType = RetailEnum.discountMixAndMatch;
                                                promotions.Add(discount);


                                            }
                                            //buy and get
                                            if (dc.DiscountCategory == RetailEnum.discountBuyAndGet)
                                            {
                                                DiscountApi discount = new DiscountApi();
                                                discount.id = dc.Id;
                                                discount.discountItem = dl.ArticleIdDiscount;
                                                discount.discountCode = dc.DiscountCode;
                                                discount.discountPercent = 0;
                                                discount.discountAmount = 0;

                                                if (transactionApi.transactionLines[i].quantity >= dl.Qty || transactionApi.transactionLines[i].subtotal >= dl.AmountTransaction)
                                                {
                                                    discount.status = 1;
                                                }
                                                else
                                                {
                                                    discount.status = 0;
                                                }
                                                discount.articleId = article.articleId;
                                                discount.articleIdFk = article.id;
                                                discount.discountType = RetailEnum.discountBuyAndGet;
                                                promotions.Add(discount);
                                            }
                                        }
                                    }
                                    //==============================================================================================
                                }
                            }
                        }
                        catch (Exception ex)
                        {

                        }
                        try
                        {
                            //by department type
                            Itemdimensiondepartmenttype departmentType = _context.Itemdimensiondepartmenttypes.Where(c => c.Description == article.departmentType).First();
                            List<Discountretailline> discountLinesByDepartmentType = _context.Discountretaillines.Where(c => c.DepartmentType == departmentType.Id).ToList();
                            foreach (Discountretailline dl in discountLinesByDepartmentType)
                            {
                                //Discountretail dc = _context.Discountretails.Where(c => c.Id == dl.DiscountRetailId).First();

                                Discountretail dc = _context.Discountretails.Where(c => c.Id == dl.DiscountRetailId && c.Status == "true").First();
                                if (_context.CustomerGroups.Where(c => c.Id == dc.CustomerGroupId).First().DESCRIPTION.Equals("Default"))
                                {
                                    String startDate = dc.StartDate;
                                    String endDate = dc.EndDate;

                                    DateTime startDateTime = DateTime.Parse(startDate);
                                    DateTime endDateTime = DateTime.Parse(endDate);

                                    if (startDateTime <= DateTime.UtcNow.ToLocalTime() && endDateTime >= DateTime.Now.ToLocalTime())
                                    {
                                        if (promotions.Any(c => c.discountCode == dc.DiscountCode) == false)
                                        {
                                            if (dc.DiscountCategory == RetailEnum.discountNormal)
                                            {
                                                DiscountApi discount = new DiscountApi();
                                                discount.id = dc.Id;
                                                discount.discountCode = dc.DiscountCode;
                                                discount.discountPercent = dl.DiscountPrecentage;
                                                discount.discountAmount = dl.CashDiscount;

                                                //for disc percent

                                                if (discount.discountPercent > 0 && discount.discountAmount == 0)
                                                {
                                                    discount.discountDesc = discount.discountPercent + " %";
                                                }
                                                else if (discount.discountPercent == 0 && discount.discountAmount > 0)
                                                {
                                                    discount.discountDesc = (Math.Floor(discount.discountAmount) / 1000) + "k";
                                                }

                                                discount.status = 1;
                                                discount.discountType = RetailEnum.discountNormal;
                                                discount.articleId = article.articleId;
                                                discount.articleIdFk = article.id;
                                                if (dl.DiscountPrecentage > 0)
                                                {
                                                    discount.totalDiscount = transactionApi.transactionLines[i].quantity * (article.price * (dl.DiscountPrecentage / 100));
                                                }
                                                else if (dl.CashDiscount > 0)
                                                {
                                                    discount.totalDiscount = transactionApi.transactionLines[i].quantity * dl.CashDiscount;
                                                }
                                                //if discount has been applied then dont apply for normal discount
                                                if (transactionApi.transactionLines[i].discountCode == "" || transactionApi.transactionLines[i].discountCode == null)
                                                {
                                                    promotions.Add(discount);
                                                }
                                            }
                                            if (dc.DiscountCategory == RetailEnum.discountMixAndMatch)
                                            {

                                                DiscountApi discount = new DiscountApi();
                                                discount.id = dc.Id;
                                                discount.status = 0;
                                                discount.discountCode = dc.DiscountCode;
                                                discount.discountPercent = dl.DiscountPrecentage;
                                                discount.discountAmount = dl.CashDiscount;

                                                //for disc percent

                                                if (discount.discountPercent > 0 && discount.discountAmount == 0)
                                                {
                                                    discount.discountDesc = discount.discountPercent + " %";
                                                }
                                                else if (discount.discountPercent == 0 && discount.discountAmount > 0)
                                                {
                                                    discount.discountDesc = (Math.Floor(discount.discountAmount) / 1000) + "k";
                                                }


                                                if (transactionApi.transactionLines[i].quantity >= dl.Qty && dl.Qty > 0)
                                                {
                                                    discount.status = 1;
                                                }


                                                if (dl.AmountTransaction > 0 && transactionApi.transactionLines[i].subtotal >= dl.AmountTransaction)
                                                {
                                                    discount.status = 1;
                                                }

                                                discount.articleId = article.articleId;
                                                discount.articleIdFk = article.id;
                                                discount.discountType = RetailEnum.discountMixAndMatch;
                                                promotions.Add(discount);


                                            }
                                            //buy and get
                                            if (dc.DiscountCategory == RetailEnum.discountBuyAndGet)
                                            {
                                                DiscountApi discount = new DiscountApi();
                                                discount.id = dc.Id;
                                                discount.discountItem = dl.ArticleIdDiscount;
                                                discount.discountCode = dc.DiscountCode;
                                                discount.discountPercent = 0;
                                                discount.discountAmount = 0;

                                                if (transactionApi.transactionLines[i].quantity >= dl.Qty || transactionApi.transactionLines[i].subtotal >= dl.AmountTransaction)
                                                {
                                                    discount.status = 1;
                                                }
                                                else
                                                {
                                                    discount.status = 0;
                                                }
                                                discount.articleId = article.articleId;
                                                discount.articleIdFk = article.id;
                                                discount.discountType = RetailEnum.discountBuyAndGet;
                                                promotions.Add(discount);
                                            }
                                        }
                                    }
                                    //=====================================================================================

                                }
                            }
                        }
                        catch (Exception ex)
                        {


                        }
                        //by genre
                        try
                        {
                            Itemdimensiongender gender = _context.Itemdimensiongenders.Where(c => c.Description == article.gender).First();
                            List<Discountretailline> discountLinesByGender = _context.Discountretaillines.Where(c => c.Gender == gender.Id).ToList();
                            foreach (Discountretailline dl in discountLinesByGender)
                            {
                                //Discountretail dc = _context.Discountretails.Where(c => c.Id == dl.DiscountRetailId).First();
                                Discountretail dc = _context.Discountretails.Where(c => c.Id == dl.DiscountRetailId && c.Status == "true").First();
                                if (_context.CustomerGroups.Where(c => c.Id == dc.CustomerGroupId).First().DESCRIPTION.Equals("Default"))
                                {
                                    String startDate = dc.StartDate;
                                    String endDate = dc.EndDate;

                                    DateTime startDateTime = DateTime.Parse(startDate);
                                    DateTime endDateTime = DateTime.Parse(endDate);





                                    if (startDateTime <= DateTime.UtcNow.ToLocalTime() && endDateTime >= DateTime.Now.ToLocalTime())
                                    {
                                        if (promotions.Any(c => c.discountCode == dc.DiscountCode) == false)
                                        {
                                            if (dc.DiscountCategory == RetailEnum.discountNormal)
                                            {
                                                DiscountApi discount = new DiscountApi();
                                                discount.id = dc.Id;
                                                discount.discountCode = dc.DiscountCode;
                                                discount.discountPercent = dl.DiscountPrecentage;
                                                discount.discountAmount = dl.CashDiscount;

                                                //for disc percent

                                                if (discount.discountPercent > 0 && discount.discountAmount == 0)
                                                {
                                                    discount.discountDesc = discount.discountPercent + " %";
                                                }
                                                else if (discount.discountPercent == 0 && discount.discountAmount > 0)
                                                {
                                                    discount.discountDesc = (Math.Floor(discount.discountAmount) / 1000) + "k";
                                                }

                                                discount.status = 1;
                                                discount.discountType = RetailEnum.discountNormal;
                                                discount.articleId = article.articleId;
                                                discount.articleIdFk = article.id;
                                                if (dl.DiscountPrecentage > 0)
                                                {
                                                    discount.totalDiscount = transactionApi.transactionLines[i].quantity * (article.price * (dl.DiscountPrecentage / 100));
                                                }
                                                else if (dl.CashDiscount > 0)
                                                {
                                                    discount.totalDiscount = transactionApi.transactionLines[i].quantity * dl.CashDiscount;
                                                }

                                                //if discount has been applied then dont apply for normal discount
                                                if (transactionApi.transactionLines[i].discountCode == "" || transactionApi.transactionLines[i].discountCode == null)
                                                {
                                                    promotions.Add(discount);
                                                }
                                            }

                                            if (dc.DiscountCategory == RetailEnum.discountMixAndMatch)
                                            {

                                                DiscountApi discount = new DiscountApi();
                                                discount.id = dc.Id;
                                                discount.status = 0;
                                                discount.discountCode = dc.DiscountCode;
                                                discount.discountPercent = dl.DiscountPrecentage;
                                                discount.discountAmount = dl.CashDiscount;
                                                //for disc percent

                                                if (discount.discountPercent > 0 && discount.discountAmount == 0)
                                                {
                                                    discount.discountDesc = discount.discountPercent + " %";
                                                }
                                                else if (discount.discountPercent == 0 && discount.discountAmount > 0)
                                                {
                                                    discount.discountDesc = (Math.Floor(discount.discountAmount) / 1000) + "k";
                                                }

                                                if (transactionApi.transactionLines[i].quantity >= dl.Qty && dl.Qty > 0)
                                                {
                                                    discount.status = 1;
                                                }


                                                if (dl.AmountTransaction > 0 && transactionApi.transactionLines[i].subtotal >= dl.AmountTransaction)
                                                {
                                                    discount.status = 1;
                                                }

                                                discount.articleId = article.articleId;
                                                discount.articleIdFk = article.id;
                                                discount.discountType = RetailEnum.discountMixAndMatch;
                                                promotions.Add(discount);


                                            }
                                            //buy and get
                                            if (dc.DiscountCategory == RetailEnum.discountBuyAndGet)
                                            {
                                                DiscountApi discount = new DiscountApi();
                                                discount.id = dc.Id;
                                                discount.discountItem = dl.ArticleIdDiscount;
                                                discount.discountCode = dc.DiscountCode;
                                                discount.discountPercent = 0;
                                                discount.discountAmount = 0;


                                                if (transactionApi.transactionLines[i].quantity >= dl.Qty || transactionApi.transactionLines[i].subtotal >= dl.AmountTransaction)
                                                {
                                                    discount.status = 1;
                                                }
                                                else
                                                {
                                                    discount.status = 0;
                                                }
                                                discount.articleId = article.articleId;
                                                discount.articleIdFk = article.id;
                                                promotions.Add(discount);
                                            }
                                        }
                                    }
                                    //==================================================================
                                }
                            }
                        }
                        catch (Exception ex)
                        {

                        }


                        //by item
                        try
                        {
                            BiensiPosDbContext.Article item = _context.Articles.Where(c => c.ARTICLEID == transactionApi.transactionLines[i].article.articleId).First();
                            List<Discountretailline> discountLinesByGender = _context.Discountretaillines.Where(c => c.ArticleId == item.Id).ToList();
                            foreach (Discountretailline dl in discountLinesByGender)
                            {
                                try
                                {
                                    //Discountretail dc = _context.Discountretails.Where(c => c.Id == dl.DiscountRetailId).First();
                                    Discountretail dc = _context.Discountretails.Where(c => c.Id == dl.DiscountRetailId && c.Status == "true").First();
                                    if (_context.CustomerGroups.Where(c => c.Id == dc.CustomerGroupId).First().DESCRIPTION.Equals("Default"))
                                    {
                                        String startDate = dc.StartDate;
                                        String endDate = dc.EndDate;

                                        DateTime startDateTime = DateTime.Parse(startDate);
                                        DateTime endDateTime = DateTime.Parse(endDate);

                                        if (startDateTime <= DateTime.UtcNow.ToLocalTime() && endDateTime >= DateTime.Now.ToLocalTime())
                                        {
                                            if (promotions.Any(c => c.discountCode == dc.DiscountCode) == false)
                                            {
                                                if (dc.DiscountCategory == RetailEnum.discountMixAndMatch)
                                                {
                                                    DiscountApi discount = new DiscountApi();
                                                    discount.id = dc.Id;
                                                    discount.status = 0;
                                                    discount.discountCode = dc.DiscountCode;
                                                    discount.discountPercent = dl.DiscountPrecentage;
                                                    discount.discountAmount = dl.CashDiscount;

                                                    //for disc percent

                                                    if (discount.discountPercent > 0 && discount.discountAmount == 0)
                                                    {
                                                        discount.discountDesc = discount.discountPercent + " %";
                                                    }
                                                    else if (discount.discountPercent == 0 && discount.discountAmount > 0)
                                                    {
                                                        discount.discountDesc = (Math.Floor(discount.discountAmount) / 1000) + "K";
                                                    }

                                                    if (transactionApi.transactionLines[i].quantity >= dl.Qty && dl.Qty > 0)
                                                    {
                                                        discount.status = 1;
                                                    }


                                                    if (dl.AmountTransaction > 0 && transactionApi.transactionLines[i].subtotal >= dl.AmountTransaction)
                                                    {
                                                        discount.status = 1;
                                                    }

                                                    discount.articleId = article.articleId;
                                                    discount.articleIdFk = article.id;
                                                    discount.discountType = RetailEnum.discountMixAndMatch;
                                                    promotions.Add(discount);
                                                }
                                                else if (dc.DiscountCategory == RetailEnum.discountNormal)
                                                {
                                                    DiscountApi discount = new DiscountApi();
                                                    discount.id = dc.Id;
                                                    discount.discountCode = dc.DiscountCode;
                                                    discount.discountPercent = dl.DiscountPrecentage;
                                                    discount.discountAmount = dl.CashDiscount;

                                                    //for disc percent

                                                    if (discount.discountPercent > 0 && discount.discountAmount == 0)
                                                    {
                                                        discount.discountDesc = discount.discountPercent + " %";
                                                    }
                                                    else if (discount.discountPercent == 0 && discount.discountAmount > 0)
                                                    {
                                                        discount.discountDesc = (Math.Floor(discount.discountAmount) / 1000) + "k";
                                                    }

                                                    discount.status = 1;
                                                    discount.discountType = RetailEnum.discountNormal;
                                                    discount.articleId = article.articleId;
                                                    discount.articleIdFk = article.id;
                                                    if (dl.DiscountPrecentage > 0)
                                                    {
                                                        discount.totalDiscount = transactionApi.transactionLines[i].quantity * ((decimal)article.price * ((decimal)dl.DiscountPrecentage / 100));
                                                    }
                                                    else if (dl.CashDiscount > 0)
                                                    {
                                                        discount.totalDiscount = transactionApi.transactionLines[i].quantity * dl.CashDiscount;
                                                    }

                                                    //if discount has been applied then dont apply for normal discount
                                                    if (transactionApi.transactionLines[i].discountCode == "" || transactionApi.transactionLines[i].discountCode == null)
                                                    {
                                                        promotions.Add(discount);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    //=================================================================================

                                }
                                catch (Exception ex)
                                {
                                }
                            }

                        }
                        catch (Exception ex)
                        {


                        }

                        //customer group
                        try
                        {
                            //karena by customer group jadi cek dlu does coestomer exist
                            if (transactionApi.customerId != "")
                            {
                                bool custExist = _context.Customers.Any(c => c.CustId == transactionApi.customerId);
                                if (custExist)
                                {
                                    BiensiPosDbContext.Customer customer = _context.Customers.Where(c => c.CustId == transactionApi.customerId).First();
                                    List<Discountretail> discountRetailsCustGroup = _context.Discountretails.Where(c => c.CustomerGroupId == customer.CustGroupId && c.Status == "true").ToList();

                                    for (int l = 0; l < discountRetailsCustGroup.Count; l++)
                                    {
                                        Discountretail dc = discountRetailsCustGroup[l];
                                        String startDate = dc.StartDate;
                                        String endDate = dc.EndDate;

                                        DateTime startDateTime = DateTime.Parse(startDate);
                                        DateTime endDateTime = DateTime.Parse(endDate);

                                        if (startDateTime <= DateTime.UtcNow.ToLocalTime() && endDateTime >= DateTime.Now.ToLocalTime())
                                        {
                                            if (!_context.CustomerGroups.Where(c => c.Id == dc.CustomerGroupId).First().DESCRIPTION.Equals("Default"))
                                            {
                                                BiensiPosDbContext.Article item = _context.Articles.Where(c => c.ARTICLEID == transactionApi.transactionLines[i].article.articleId).First();
                                                List<Discountretailline> discountLineCustGroup = _context.Discountretaillines.Where(c => c.DiscountRetailId == dc.Id
                                                && c.ArticleId == item.Id).ToList();
                                                for (int j = 0; j < discountLineCustGroup.Count; j++)
                                                {
                                                    Discountretailline dl = discountLineCustGroup[j];
                                                    if (dc.DiscountCategory == RetailEnum.discountNormal)
                                                    {
                                                        //for normal discount
                                                        DiscountApi discount = new DiscountApi();
                                                        discount.id = dc.Id;
                                                        discount.discountCode = dc.DiscountCode;
                                                        discount.discountPercent = dl.DiscountPrecentage;
                                                        discount.discountAmount = dl.CashDiscount;

                                                        //for disc percent

                                                        if (discount.discountPercent > 0 && discount.discountAmount == 0)
                                                        {
                                                            discount.discountDesc = discount.discountPercent + " %";
                                                        }
                                                        else if (discount.discountPercent == 0 && discount.discountAmount > 0)
                                                        {
                                                            discount.discountDesc = (Math.Floor(discount.discountAmount) / 1000) + "k";
                                                        }

                                                        discount.status = 1;
                                                        discount.discountType = RetailEnum.discountNormal;
                                                        discount.articleId = article.articleId;
                                                        discount.articleIdFk = article.id;
                                                        if (dl.DiscountPrecentage > 0)
                                                        {
                                                            discount.totalDiscount = transactionApi.transactionLines[i].quantity * ((decimal)article.price * ((decimal)dl.DiscountPrecentage / 100));
                                                        }
                                                        else if (dl.CashDiscount > 0)
                                                        {
                                                            discount.totalDiscount = transactionApi.transactionLines[i].quantity * dl.CashDiscount;
                                                        }
                                                        //if discount has been applied then dont apply for normal discount
                                                        //  if (transactionApi.transactionLines[i].discountCode == "" || transactionApi.transactionLines[i].discountCode == null)
                                                        //  {
                                                        promotions.Add(discount);
                                                        //  }
                                                    }
                                                }

                                            }

                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception ex) { }

                    }
                }
            }
            catch (Exception ex)
            {
                //return Ok(ex.ToString());
            }


            try
            {
                //for mix and match
                //check by brand and qty
                List<ResultLine> result = transactionApi.transactionLines.GroupBy(l => l.article.brand)
                              .Select(cl => new ResultLine
                              {
                                  category = cl.First().article.brand,
                                  qty = cl.Sum(c => c.quantity),
                                  amount = cl.Sum(c => c.subtotal)

                              }).ToList();

                for (int i = 0; i < result.Count; i++)
                {
                    Itemdimensionbrand brand = _context.Itemdimensionbrands.Where(c => c.Description == result[i].category).First();
                    List<Discountretailline> discountLinesByBrand = _context.Discountretaillines.Where(c => c.BrandCode == brand.Id).ToList();
                    for (int j = 0; j < discountLinesByBrand.Count; j++)
                    {
                        Discountretailline dl = discountLinesByBrand[j];
                        int discountId = dl.DiscountRetailId;
                        //check if store is applied to the discount or not;

                        try
                        {
                            //Discountretail dc = _context.Discountretails.Where(c => c.Id == dl.DiscountRetailId).First();
                            Discountretail dc = _context.Discountretails.Where(c => c.Id == dl.DiscountRetailId && c.Status == "true").First();

                            String startDate = dc.StartDate;
                            String endDate = dc.EndDate;

                            DateTime startDateTime = DateTime.Parse(startDate);
                            DateTime endDateTime = DateTime.Parse(endDate);

                            if (startDateTime <= DateTime.UtcNow.ToLocalTime() && endDateTime >= DateTime.Now.ToLocalTime())
                            {

                                if (dc.DiscountCategory == RetailEnum.discountMixAndMatch)
                                {
                                    DiscountApi discount = new DiscountApi();
                                    discount.id = dc.Id;
                                    discount.status = 0;
                                    discount.discountCode = dc.DiscountCode;
                                    discount.discountPercent = dl.DiscountPrecentage;
                                    discount.discountAmount = dl.CashDiscount;

                                    //for disc percent

                                    if (discount.discountPercent > 0 && discount.discountAmount == 0)
                                    {
                                        discount.discountDesc = discount.discountPercent + " %";
                                    }
                                    else if (discount.discountPercent == 0 && discount.discountAmount > 0)
                                    {
                                        discount.discountDesc = (Math.Floor(discount.discountAmount) / 1000) + "K";
                                    }

                                    if (result[i].qty >= dl.Qty && dl.Qty > 0)
                                    {
                                        discount.status = 1;
                                    }

                                    if (dl.AmountTransaction > 0 && result[i].amount >= dl.AmountTransaction)
                                    {
                                        discount.status = 1;
                                    }
                                    //get transaction lines
                                    discount.articleId = transactionApi.transactionLines.Where(c => c.article.brand == result[i].category).First().article.articleId;
                                    discount.articleIdFk = transactionApi.transactionLines.Where(c => c.article.brand == result[i].category).First().articleIdFk;
                                    discount.discountType = RetailEnum.discountMixAndMatch;
                                    bool isExist = promotions.Any(c => c.discountCode == discount.discountCode);
                                    if (isExist)
                                    {
                                        DiscountApi p = promotions.Where(c => c.discountCode == discount.discountCode).First();
                                        promotions.Remove(p);
                                        promotions.Add(discount);
                                    }

                                }
                            }
                            //===================================================================================================
                        }
                        catch (Exception ex)
                        {
                            String error = ex.ToString();
                        }
                        //mix and match

                    }
                }

            }
            catch (Exception ex)
            {

            }


            discountMaster.discounts = eliminateDiscount(promotions);
            discountMaster.discountItems = getListDiscountItem(promotions);
            //get normal discount;
            Console.WriteLine(JsonConvert.SerializeObject(discountMaster));
            return discountMaster;
        }

        private List<DiscountApi> eliminateDiscount(List<DiscountApi> discountApi)
        {
            List<DiscountApi> list = new List<DiscountApi>();
            for (int i = 0; i < discountApi.Count; i++)
            {
                if (discountApi[i].discountType != RetailEnum.discountNormal)
                //|| (discountApi[i].discountType != Config.RetailEnum.discountEmployee))
                {
                    list.Add(discountApi[i]);
                }
            }
            return list;
        }
        private List<DiscountItemAPI> trimDiscountEmployee(List<DiscountItemAPI> discItemApiEmployee, String employeeCode)
        {
            int numberOfUsedDiscount = 0;
            try
            {
                List<BiensiPosDbContext.Transaction> list = _context.Transactions.Where(c => c.CUSTOMERID == employeeCode).ToList();
                //&& c.TransactionDate.Value.Month == DateTime.Now.Month) buat validate bulan
                // yang in buat validate bulan di tambahin ya
                //NOTES FOR DATE
                for (int i = 0; i < list.Count; i++)
                {
                    BiensiPosDbContext.Transaction trans = list[i];
                    String transactionDate = trans.DATE;
                    DateTime transactionDateTime = DateTime.Parse(transactionDate);

                    if (transactionDateTime.Month == DateTime.Now.Month)
                    {
                        int numbLineDisc = _context.TransactionLines.Where(c => c.TRANSACTIONID == trans.TRANSACTIONID && c.DISCOUNT > 0).Sum(c => c.QUANTITY);
                        numberOfUsedDiscount = numberOfUsedDiscount + numbLineDisc;
                    }
                }

            }
            catch
            {


            }
            List<DiscountItemAPI> listDiscountItemAPi = new List<DiscountItemAPI>();
            if (numberOfUsedDiscount < 3)
            {
                if ((discItemApiEmployee.Count + numberOfUsedDiscount) >= 3)
                {
                    listDiscountItemAPi = discItemApiEmployee.OrderByDescending(c => c.amountDiscount).ToList();
                    for (int i = 0; i < listDiscountItemAPi.Count; i++)
                    {
                        if (i > (2 - numberOfUsedDiscount))
                        {
                            listDiscountItemAPi[i].amountDiscount = 0;
                            listDiscountItemAPi[i].discountDesc = "";
                            listDiscountItemAPi[i].discountCode = "";
                            // listDiscountItemAPi.Remove(listDiscountItemAPi[i]);
                        }
                    }
                }
                else
                {
                    listDiscountItemAPi = discItemApiEmployee;
                }
            }
            return listDiscountItemAPi;
        }

        private List<DiscountItemAPI> getListDiscountItem(List<DiscountApi> discountApiPram)
        {
            List<DiscountItemAPI> discountItemApi = new List<DiscountItemAPI>();
            for (int i = 0; i < discountApiPram.Count; i++)
            {

                DiscountItemAPI discountItemAPI = new DiscountItemAPI();
                discountItemAPI.articleId = discountApiPram[i].articleId;
                discountItemAPI.amountDiscount = discountApiPram[i].totalDiscount;
                discountItemAPI.articleIdFk = discountApiPram[i].articleIdFk;
                discountItemAPI.discountDesc = discountApiPram[i].discountPercent.ToString() != "0" ? discountApiPram[i].discountPercent.ToString() + " %" : discountApiPram[i].discountAmount + "";
                discountItemAPI.discountCode = discountApiPram[i].discountCode;
                discountItemAPI.discountType = discountApiPram[i].discountType.ToString();
                // discountItemApi.Add(discountItemAPI);

                if (discountApiPram[i].discountType.ToString() == RetailEnum.discountNormal.ToString())
                {
                    discountItemApi.Add(discountItemAPI);
                }
            }

            List<DiscountItemAPI> discountItemApiGroup = discountItemApi.Where(c => c.articleId != null | c.amountDiscount > 0).GroupBy(item => item.articleId)
             .Select(grp => grp.Aggregate((max, cur) => (max == null
             || cur.amountDiscount > max.amountDiscount) ? cur : max)).ToList();

            return discountItemApiGroup;
        }
    }
}

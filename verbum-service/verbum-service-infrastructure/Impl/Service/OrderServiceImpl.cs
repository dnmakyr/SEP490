using AutoMapper;
using Lombok.NET;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Org.BouncyCastle.Asn1.Ocsp;
using PayPal.Api;
using System;
using verbum_service_application.Service;
using verbum_service_domain.Common;
using verbum_service_domain.Common.ErrorModel;
using verbum_service_domain.DTO.Request;
using verbum_service_domain.DTO.Response;
using verbum_service_domain.Models;
using verbum_service_domain.Utils;
using verbum_service_infrastructure.DataContext;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace verbum_service_infrastructure.Impl.Service
{
    [RequiredArgsConstructor]
    public partial class OrderServiceImpl : OrderService
    {
        private readonly verbumContext context;
        private readonly IMapper mapper;
        private readonly CurrentUser currentUser;
        private readonly IConfiguration Configuration;
        private Payment payment;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly MailService mailService;

        public async Task CreateOrder(verbum_service_domain.Models.Order info)
        {
            try
            {
                info.ClientId = currentUser.Id;
                context.Orders.Add(info);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task AddRangeMiddle(Guid orderId, List<string> languageIds)
        {
            try
            {
                var categories = context.Languages.Where(c => languageIds.Contains(c.LanguageId)).ToList();

                var order = context.Orders.Find(orderId);
                if (order != null)
                {
                    foreach (var category in categories)
                    {
                        order.TargetLanguages.Add(category);
                    }
                }

                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public async Task<List<OrderDetailsResponse>> GetAllOrder()
        {
            List<verbum_service_domain.Models.Order> orders = new List<verbum_service_domain.Models.Order>();
            Guid clientId = currentUser.Id;
            switch (currentUser.Role)
            {
                case UserRole.CLIENT:
                    orders = await context.Orders.Include(x => x.Discount).Include(o => o.TargetLanguages).Include(o => o.OrderReferences).Include(x => x.Works).ThenInclude(x => x.Jobs).Include(x => x.Works).ThenInclude(x => x.ServiceCodeNavigation)
                        .Where(x => x.ClientId == clientId)
                        .ToListAsync();
                    break;
                case UserRole.ADMIN:
                case UserRole.STAFF:
                case UserRole.DIRECTOR:
                case UserRole.LINGUIST:
                case UserRole.EDIT_MANAGER:
                case UserRole.TRANSLATE_MANAGER:
                case UserRole.EVALUATE_MANAGER:
                    orders = await context.Orders.Include(x => x.Discount).Include(o => o.TargetLanguages).Include(o => o.OrderReferences).Include(x => x.Works).ThenInclude(x => x.Jobs).Include(x => x.Works).ThenInclude(x => x.ServiceCodeNavigation)
                        .ToListAsync();
                    break;
                default:
                    throw new BusinessException(AlertMessage.Alert(ValidationAlertCode.NOT_FOUND, "Role"));
            }
            List<OrderDetailsResponse> list = mapper.Map<List<OrderDetailsResponse>>(orders);
            return list;
        }

        public async Task<OrderDetailsResponse> GetOrderDetails(Guid id)
        {
            verbum_service_domain.Models.Order orders = new verbum_service_domain.Models.Order();
            orders = await context.Orders.Include(x => x.Discount).Include(x => x.TargetLanguages).Include(x => x.OrderReferences).Include(x => x.Receipts).Include(x => x.Works).ThenInclude(x => x.Jobs).Include(x => x.Works).ThenInclude(x => x.ServiceCodeNavigation).FirstOrDefaultAsync(x => x.OrderId == id);
            if (ObjectUtils.IsEmpty(orders))
            {
                throw new BusinessException(AlertMessage.Alert(ValidationAlertCode.NOT_FOUND, "Order"));
            }
            OrderDetailsResponse orderResponse = mapper.Map<OrderDetailsResponse>(orders);
            return orderResponse;
        }

        public async Task<string> DoPayment(Guid orderId, bool depositeOrPayment)
        {
            var ClientID = Configuration.GetValue<string>("PayPal:Key");
            var ClientSecret = Configuration.GetValue<string>("PayPal:Secret");
            var mode = Configuration.GetValue<string>("PayPal:mode");
            APIContext apiContext = PaypalConfiguration.GetAPIContext(ClientID, ClientSecret, mode);
            try
            {
                Guid receiptId = Guid.NewGuid();
                //the uri on which paypal send back the data
                string baseURI = SystemConfig.BE_DOMAIN + "/order/confirm-payment?guid=" + receiptId;
                double orderPrice = (double)await context.Orders.Where(x => x.OrderId == orderId).Select(x => x.OrderPrice).FirstOrDefaultAsync();
                var createdPayment = this.CreatePayment(apiContext, baseURI, orderId.ToString(), orderPrice);
                //get links returned from paypal in response to Create function call  
                var links = createdPayment.links.GetEnumerator();
                string paypalRedirectUrl = string.Empty;
                while (links.MoveNext())
                {
                    Links lnk = links.Current;
                    if (lnk.rel.ToLower().Trim().Equals("approval_url"))
                    {
                        //saving the payapalredirect URL to which user will be redirected for payment  
                        paypalRedirectUrl = lnk.href;
                    }
                }
                // saving to receipt
                context.Receipts.Add(new Receipt
                {
                    ReceiptId = receiptId,
                    OrderId = orderId,
                    PayDate = DateTime.Now,
                    DepositeOrPayment = depositeOrPayment,
                    PaymentId = createdPayment.id,
                    Amount = createdPayment.transactions.Sum(transaction => decimal.Parse(transaction.amount.total)),
                    Done = false
                });
                await context.SaveChangesAsync();
                return paypalRedirectUrl;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private Payment ExecutePayment(APIContext apiContext, string payerId, string paymentId)
        {
            var paymentExecution = new PaymentExecution()
            {
                payer_id = payerId
            };
            this.payment = new Payment()
            {
                id = paymentId
            };
            return this.payment.Execute(apiContext, paymentExecution);
        }

        private Payment CreatePayment(APIContext apiContext, string redirectUrl, string orderName, double orderPrice)
        {
            //create itemlist and add item objects to it  

            var itemList = new ItemList()
            {
                items = new List<Item>()
            };
            //Adding Item Details like name, currency, price etc  
            itemList.items.Add(new Item()
            {
                name = "Order " + orderName,
                currency = "USD",
                price = (Math.Ceiling((orderPrice / 2) * 100) / 100).ToString("F2"),
                quantity = "1",
                sku = "asd"
            });
            var payer = new Payer()
            {
                payment_method = "paypal"
            };
            // Configure Redirect Urls here with RedirectUrls object  
            var redirUrls = new RedirectUrls()
            {
                cancel_url = redirectUrl + "&Cancel=true",
                return_url = redirectUrl
            };
            // Adding Tax, shipping and Subtotal details  
            //var details = new Details()
            //{
            //    tax = "1",
            //    shipping = "1",
            //    subtotal = "1"
            //};
            //Final amount with details  
            var amount = new Amount()
            {
                currency = "USD",
                total = (Math.Ceiling((orderPrice / 2) * 100) / 100).ToString("F2"), // Total must be equal to sum of tax, shipping and subtotal.  
                                //details = details
            };
            var transactionList = new List<Transaction>();
            // Adding description about the transaction  
            transactionList.Add(new Transaction()
            {
                description = "Transaction description",
                invoice_number = Guid.NewGuid().ToString(), //Generate an Invoice No  
                amount = amount,
                item_list = itemList
            });
            this.payment = new Payment()
            {
                intent = "sale",
                payer = payer,
                transactions = transactionList,
                redirect_urls = redirUrls
            };
            // Create a payment using a APIContext  
            return this.payment.Create(apiContext);
        }

        public async Task UpdateOrder(OrderUpdate request)
        {
            int records = await context.Orders
                .Where(x => x.OrderId == request.OrderId)
                .ExecuteUpdateAsync(x => x.SetProperty(u => u.OrderName, request.OrderName)
                                        .SetProperty(u => u.OrderStatus, OrderStatus.NEW.ToString())
                                        .SetProperty(u => u.OrderNote, request.OrderNote)
                                        .SetProperty(u => u.SourceLanguageId, request.SourceLanguageId)
                                        .SetProperty(u => u.DueDate, request.DueDate)
                                        .SetProperty(u => u.HasTranslateService, request.TranslateService)
                                        .SetProperty(u => u.HasEditService, request.EditService)
                                        .SetProperty(u => u.HasEvaluateService, request.EvaluateService));
            if (records < 1) throw new BusinessException(ValidationAlertCode.UPDATE_RECORD_FAIL);
        }

        public async Task UpdateOrderPrice(Guid orderId, decimal price)
        {
            verbum_service_domain.Models.Order order = context.Orders.Include(o => o.Discount).FirstOrDefault(x => x.OrderId == orderId);
            if (price <= 0)
            {
                throw new BusinessException(AlertMessage.Alert(ValidationAlertCode.INVALID,"Price"));
            }
            if (ObjectUtils.IsEmpty(order))
            {
                throw new BusinessException(AlertMessage.Alert(ValidationAlertCode.NOT_FOUND, "Order"));
            }
            if (ObjectUtils.IsNotEmpty(order.DiscountId)) price = price - (price * (order.Discount.DiscountPercent.GetValueOrDefault() / 100));
            order.OrderPrice = price;
            int records = await context.SaveChangesAsync();
            if (records < 1) throw new BusinessException(ValidationAlertCode.UPDATE_RECORD_FAIL);
        }

        public async Task UpdateOrderTargetLanguage(OrderUpdate request)
        {
            var order = context.Orders
                        .Include(o => o.TargetLanguages)
                        .FirstOrDefault(o => o.OrderId == request.OrderId);

            if (order != null)
            {
                order.TargetLanguages.Clear();
                await context.SaveChangesAsync();
            }
            await AddRangeMiddle(request.OrderId, request.TargetLanguageIdList);
        }

        public async Task ChangeOrderStatus(Guid orderId, string orderStatus)
        {
            if (OrderStatus.NEW.ToString().Equals(orderStatus)
                || (UserRole.CLIENT.Equals(currentUser.Role) && !OrderStatus.CANCELLED.ToString().Equals(orderStatus))
                || (UserRole.STAFF.Equals(currentUser.Role) && OrderStatus.CANCELLED.ToString().Equals(orderStatus)))
            {
                throw new BusinessException(AlertMessage.Alert(ValidationAlertCode.INVALID, "Order Status"));
            }
            if (OrderStatus.ACCEPTED.ToString().Equals(orderStatus))
            {
                await context.Orders
                .Where(x => x.OrderId == orderId)
                .ExecuteUpdateAsync(x => x.SetProperty(u => u.RejectReason, (string?)null));
                //send mail
                verbum_service_domain.Models.Order order = await context.Orders.Include(x => x.Client).FirstAsync(x => x.OrderId == orderId);
                string mailBody = await MailUtils.BuildVerificationEmail(SystemConfig.FE_DOMAIN + "/orders/details/" + orderId, "accept_order_mail");
                _ = Task.Run(() => mailService.SendEmailAsync(order.Client.Email, string.Format(MailConstant.ACCEPT_ORDER_HEADER, order.OrderName), mailBody));
            }
            int records = await context.Orders
                .Where(x => x.OrderId == orderId)
                .ExecuteUpdateAsync(x => x.SetProperty(u => u.OrderStatus, orderStatus));
            if (records < 1) throw new BusinessException(ValidationAlertCode.UPDATE_RECORD_FAIL);
        }

        public async Task DeleteOrderReferenceFile(Guid orderId, string url)
        {
            int records = await context.OrderReferences
                .Where(x => x.OrderId == orderId && x.ReferenceFileUrl.Equals(url))
                .ExecuteUpdateAsync(x => x.SetProperty(u => u.IsDeleted, true));

            await context.Orders.Where(o => o.OrderId == orderId).ExecuteUpdateAsync(x => x.SetProperty(u => u.OrderStatus, OrderStatus.NEW.ToString()));
            if (records < 1) throw new BusinessException(ValidationAlertCode.UPDATE_RECORD_FAIL);
        }

        public async Task UploadOrderReferenceFile(List<UploadOrderFileRequest> request)
        {
            foreach (UploadOrderFileRequest one in request)
            {
                if (!Enum.IsDefined(typeof(OrderFileTag), one.Tag))
                {
                    throw new BusinessException(AlertMessage.Alert(ValidationAlertCode.INVALID, "Tag"));
                }
            }
            using (IDbContextTransaction transaction = context.Database.BeginTransaction())
            {
                try
                {
                    context.OrderReferences.AddRange(mapper.Map<List<OrderReference>>(request));
                    int records = await context.SaveChangesAsync();
                    if (records != request.Count) throw new BusinessException(ValidationAlertCode.UPDATE_RECORD_FAIL);

                    await context.Orders.Where(x => request.Select(x => x.OrderId).Contains(x.OrderId))
                        .ExecuteUpdateAsync(x => x.SetProperty(u => u.OrderStatus, OrderStatus.NEW.ToString()));
                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public async Task<List<UploadOrderFileRequest>> GetAllOrderRefrenceFiles()
        {
            return mapper.Map<List<UploadOrderFileRequest>>(await context.OrderReferences.Where(x => !x.IsDeleted).ToListAsync());
        }

        public async Task UpdateOrderRejectResponse(ResponseRequest request)
        {
            if (await context.Orders.Where(x => x.OrderId == request.Id)
                               .ExecuteUpdateAsync(o => o.SetProperty(x => x.RejectReason, request.ResponseContent)) < 1) throw new BusinessException(ValidationAlertCode.UPDATE_RECORD_FAIL);
            //send mail
            verbum_service_domain.Models.Order order = await context.Orders.Include(x => x.Client).FirstOrDefaultAsync(x => x.OrderId == request.Id);
            string body = await MailUtils.BuildVerificationEmail(SystemConfig.FE_DOMAIN + "/orders/details/" + order.OrderId, "reject_order_mail", order.RejectReason);
            _ = Task.Run(() => mailService.SendEmailAsync(order.Client.Email, string.Format(MailConstant.REJECT_ORDER_HEADER, order.OrderName), body));
        }

        public async Task CreateRevelancy(Guid orderId)
        {
            using (IDbContextTransaction transaction = context.Database.BeginTransaction())
            {
                try
                {
                    List<Work> works = context.Works
                .Include(w => w.Order)
                .Include(w => w.Categories)
                .Include(w => w.Jobs)
                .ThenInclude(w => w.Assignees).ToList();
                    List<Revelancy> list = new List<Revelancy>();
                    foreach (Work work in works)
                    {
                        foreach (Job job in work.Jobs)
                        {
                            foreach (User assignee in job.Assignees)
                            {
                                foreach (Category category in work.Categories)
                                {
                                    Revelancy revelancy = new Revelancy
                                    {
                                        RevelancyId = Guid.NewGuid(),
                                        UserId = assignee.Id,
                                        SourceLanguageId = work.Order.SourceLanguageId,
                                        TargetLanguageId = job.TargetLanguageId,
                                        ServiceCode = work.ServiceCode,
                                        CategoryId = category.CategoryId
                                    };

                                    list.Add(revelancy);
                                    //context.Revelancies.Add(revelancy);
                                }
                            }
                        }
                    }

                    int count = list.Count;
                    //await context.SaveChangesAsync();
                    transaction.Commit();
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw;
                }
            }

        }

        public async Task<string> ConfirmPayment(ConfirmPaymentDTO request)
        {
            string ClientID = Configuration.GetValue<string>("PayPal:Key") ?? string.Empty;
            string ClientSecret = Configuration.GetValue<string>("PayPal:Secret") ?? string.Empty;
            string mode = Configuration.GetValue<string>("PayPal:mode") ?? string.Empty;
            APIContext apiContext = PaypalConfiguration.GetAPIContext(ClientID, ClientSecret, mode);
            // This function exectues after receving all parameters for the payment  
            Receipt receipt = await context.Receipts.FirstOrDefaultAsync(x => x.ReceiptId.ToString() == request.guid);
            var executedPayment = ExecutePayment(apiContext, request.PayerID, receipt.PaymentId);
            if (executedPayment.state.ToLower() != "approved")
            {
                throw new BusinessException(AlertMessage.Alert(ValidationAlertCode.INVALID, "Payment"));
            }
            string status = receipt.DepositeOrPayment ? OrderStatus.IN_PROGRESS.ToString() : OrderStatus.DELIVERED.ToString();
            if (await context.Orders.Where(x => x.OrderId == receipt.OrderId)
                               .ExecuteUpdateAsync(x => x.SetProperty(u => u.OrderStatus, status)) < 1
                               || await context.Receipts.Where(x => x.ReceiptId.ToString() == request.guid).ExecuteUpdateAsync(x => x.SetProperty(u => u.Done, true)) < 1)
                throw new BusinessException(ValidationAlertCode.UPDATE_RECORD_FAIL);
            //{API_URL}/orders/details/{orderId}
            return SystemConfig.FE_DOMAIN + "/orders/details/" + receipt.OrderId;
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SR.Hubs;
using WebSellingCosmetics.Models;
using WebSellingCosmetics.Models.ViewModel;
using WebSellingCosmetics.Service;

namespace WebSellingCosmetics.Controllers
{

    public class ChatController : Controller
    {

        private readonly WebMyPhamContext _dbContext;
        public ChatController(WebMyPhamContext dbContext)
        {
            _dbContext = dbContext;
        }


        public async Task SendMessageDB(string fromUserId, string toUserId, string content)
        {
            var account = await _dbContext.Accounts.FirstOrDefaultAsync(x => x.UserName == fromUserId);
            var toaccount = await _dbContext.Accounts.FirstOrDefaultAsync(x => x.UserName == toUserId);

            var message = new Message
            {
                FromUserId = account.AccountId,
                ToUserId = toaccount.AccountId,
                Content = content,
                IsFromUserId = true,
                IsToUserId = true,
                Status = 1 // Assuming 1 means the message is sent
            };

            _dbContext.Messages.Add(message);
            await _dbContext.SaveChangesAsync();
        }
        [HttpPost]
        public async Task<IActionResult> SendMessage(string fromUserId, string toUserId, string content)
        {
            try
            {
                await SendMessageDB(fromUserId, toUserId, content);
                return Ok(1);
            }
            catch (Exception ex)
            {
                // Log the exception details
                Console.Error.WriteLine($"Error sending message: {ex}");
                return StatusCode(500, "Internal Server Error1");
            }
        }

        public async Task<IActionResult> GetChatData(string fromUser, string toUser)
        {
            var account = await _dbContext.Accounts.FirstOrDefaultAsync(x => x.UserName == fromUser);
            var toaccount = await _dbContext.Accounts.FirstOrDefaultAsync(x => x.UserName == toUser);

            var messchat = await (from message in _dbContext.Messages
                                  join m in _dbContext.Accounts on message.FromUserId equals m.AccountId
                                  where (message.FromUserId == account.AccountId && message.ToUserId == toaccount.AccountId) ||
                                    (message.FromUserId == toaccount.AccountId && message.ToUserId == account.AccountId)
                                  select new
                                  {
                                      fromId = message.FromUserId,
                                      ToId = message.ToUserId,
                                      messages = message.Content,
                                      status = message.Status,
                                      avatar = message.FromUser.Avartar,
                                      touser = message.FromUser.UserName
                                  })
                       .ToListAsync();

            return Ok(messchat);
        }

        public async Task<IActionResult> GetMessage()
        {
            var distinctMessages = _dbContext.Messages
                    .Include(x => x.FromUser)
                    .Where(x => x.FromUserId != 1)
                    .GroupBy(x => x.FromUserId)
                    .Select(group => new
                    {
                        FromUserId = group.Key,
                        ToUserId = group.FirstOrDefault().ToUserId,
                        UserName = group.FirstOrDefault().FromUser.UserName,
                        Avatar = group.FirstOrDefault().FromUser.Avartar,
                        Messages = group.FirstOrDefault().Content,
                        Name = group.FirstOrDefault().FromUser.FullName,
                        Status = group.FirstOrDefault().Status
                        
                    })
                    .ToList();


            return Ok(distinctMessages);
        }
    }
}

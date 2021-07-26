using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZimbraPasswordReset.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ZimbraServices : ControllerBase
    {
      
        private readonly ILogger<ZimbraServices> _logger;
        private readonly IConfiguration _configuration;

        public ZimbraServices(ILogger<ZimbraServices> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<string> ResetPassword(string email)
        {
            try
            {
                //In the production environment, consider moving these variables to the configuration file or another security store
                var AdminUserID = "{type here Zimbra User ID with admin rights}";
                var AdminUserPassword = "{type here the user password with admin right}";
                var EmailToResetPassword = "{type here the email you want reset password }";
                var EmailToResetNewPassword = email;// " OR {type here new email's password}";

                //Change the Zimbra Server URL on file ConnectedServices/ZimbraAPI/ConnectedService.json
                // for more details see https://wiki.zimbra.com/wiki/WSDL
                ZimbraAPI.zcsAdminPortTypeClient client = new ZimbraAPI.zcsAdminPortTypeClient();
                var authReq = await client.authRequestAsync(new ZimbraAPI.HeaderContext(), new ZimbraAPI.authRequest1 { account = new ZimbraAPI.accountSelector { by = ZimbraAPI.accountBy.id, Value = AdminUserID }, password = AdminUserPassword });
                var clientID = await client.getAccountRequestAsync(new ZimbraAPI.HeaderContext { authToken = authReq.AuthResponse.authToken }, new ZimbraAPI.getAccountRequest { account = new ZimbraAPI.accountSelector { by = ZimbraAPI.accountBy.name, Value = EmailToResetPassword } });
                var response = await client.setPasswordRequestAsync(new ZimbraAPI.HeaderContext { authToken = authReq.AuthResponse.authToken }, new ZimbraAPI.setPasswordRequest { id = clientID.GetAccountResponse.account.id, newPassword = EmailToResetNewPassword });
                return response.SetPasswordResponse.message;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return String.Concat("Error: ", ex.Message);
            }
        }
    }
}

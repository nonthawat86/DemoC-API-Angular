using PokemonAPI.Models.Response;
using PokemonAPI.Models;
using PokemonAPI.Models.Entites;
using System.IdentityModel.Tokens.Jwt;
using PokemonAPI.Data;
using Microsoft.EntityFrameworkCore;
using PokemonAPI.Extensions;

namespace PokemonAPI.Repository
{
    public interface IAuthRepository
    {
        Task<AuthResponse> IssueToken(UserAuthen userAuthentication);
        Task<AuthResponse> IssueTokenAzureAD(UserAuthenMSAL userAuthentication);
        Task<AuthResponse> RefreshToken(TokenApi tokenAuthentication);
        Task<ResponseMessageData<string>> RevokeToken(UserAuthen userAuthentication);
        Task<ResponseMessageData<string>> ForgotPass(UserForgot userForgot);
        Task<ResponseMessageData<string>> ResetPass(UserReset userReset);

    }
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext _db;
        private readonly DapperContext _dapper;
        private readonly JwtHandler _jwtHandler;
        public AuthRepository(DataContext context, DapperContext dappercontext, JwtHandler jwtHandler)
        {
            _db = context;
            _dapper = dappercontext;
            _jwtHandler = jwtHandler;
        }
        public async Task<AuthResponse> IssueToken(UserAuthen userAuthen)
        {
            //checkbypass
            var userbypass = await _db.User.FirstOrDefaultAsync(x => x.IsDeleted == false && x.Active && x.Email.Equals(userAuthen.Username));
            if (userbypass != null)
            {
                var pass = await _db.SystemConfig.FirstOrDefaultAsync(x => x.ConfigName.Equals("BYPASS"));
                if (pass.ConfigParameter1 == userAuthen.Password)
                {
                    var jwtToken = GenJwtToken(userbypass);
                    userbypass.RefreshToken = jwtToken.UserEntity.RefreshToken;
                    userbypass.RefreshTokenExpireTime = jwtToken.UserEntity.RefreshTokenExpireTime;
                    await _db.SaveChangesAsync();

                    ResponseUser responseUser = await GetInfo(userbypass);
                    responseUser.FirstTimeLogin = string.Empty;
                    return new AuthResponse
                    {
                        IsAuthSuccessful = true,
                        Token = jwtToken.Token,
                        RefreshToken = jwtToken.UserEntity.RefreshToken,
                        UserInfo = responseUser
                    };
                }
            }

            var Accounts = await _db.User.FirstOrDefaultAsync(x => x.IsDeleted == false && x.Active && x.EmployeeTypeId == userAuthen.EmployeeTypeId
            && x.UserName.Equals(userAuthen.Username));

            if (Accounts != null)
            {
                bool verified = BCrypt.Net.BCrypt.EnhancedVerify(userAuthen.Password, Accounts.Password);

                if (verified)
                {
                    if (Accounts.PasswordExpireTime != null && Accounts.PasswordExpireTime <= DateTime.Now)
                    {

                        string Id = Accounts.UserId + "|" + Accounts.Email;
                        var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(Id);
                        return new AuthResponse
                        {
                            IsAuthSuccessful = false,
                            Token = "ExpPass",
                            RefreshToken = System.Convert.ToBase64String(plainTextBytes)
                        };
                    }

                    var jwtToken = GenJwtToken(Accounts);
                    Accounts.RefreshToken = jwtToken.UserEntity.RefreshToken;
                    Accounts.RefreshTokenExpireTime = jwtToken.UserEntity.RefreshTokenExpireTime;
                    await _db.SaveChangesAsync();

                    ResponseUser responseUser = await GetInfo(Accounts);

                    return new AuthResponse
                    {
                        IsAuthSuccessful = true,
                        Token = jwtToken.Token,
                        RefreshToken = jwtToken.UserEntity.RefreshToken,
                        UserInfo = responseUser
                    };

                }
            }

            return new AuthResponse { IsAuthSuccessful = false, Token = "" };
        }

        private UserToken GenJwtToken(User Accounts)
        {
            UserToken userToken = new UserToken();
            userToken.UserEntity = Accounts;
            var signingCredentials = _jwtHandler.GetSigningCredentials();
            var claims = _jwtHandler.GetClaims(Accounts);
            var tokenOptions = _jwtHandler.GenerateTokenOptions(signingCredentials, claims);
            userToken.Token = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
            userToken.UserEntity.RefreshToken = _jwtHandler.GenerateRefreshToken();
            userToken.UserEntity.RefreshTokenExpireTime = DateTime.Now.AddDays(1);
            return userToken;
        }

        public async Task<AuthResponse> IssueTokenAzureAD(UserAuthenMSAL userAuthen)
        {
            string empcode = userAuthen.Id.Split('@')[0];
            var Accounts = await _db.User.FirstOrDefaultAsync(x => x.EmployeeCode.Equals(empcode) || x.Email.Equals(userAuthen.Email.ToLower()));
            // var Accounts = await _db.User.FirstOrDefaultAsync(x => x.Email.Equals(userAuthen.Email));

            if (Accounts != null)
            {
                if (Accounts.IsDeleted == true || Accounts.Active == false)
                {
                    return new AuthResponse { IsAuthSuccessful = false, Token = "" };
                }

                var jwtToken = GenJwtToken(Accounts);
                Accounts.RefreshToken = jwtToken.UserEntity.RefreshToken;
                Accounts.RefreshTokenExpireTime = jwtToken.UserEntity.RefreshTokenExpireTime;
                await _db.SaveChangesAsync();

                ResponseUser response = await GetInfo(Accounts);

                return new AuthResponse
                {
                    IsAuthSuccessful = true,
                    Token = jwtToken.Token,
                    RefreshToken = jwtToken.UserEntity.RefreshToken,
                    UserInfo = response
                };

            }
            else
            {

                UserRepository userRepository = new UserRepository(_db, _dapper);
                var response = await userRepository.EmployeeSave(empcode, 4);
                if (response != null)
                {
                    var Acc = await _db.User.FirstOrDefaultAsync(x => x.IsDeleted == false && x.Active && x.EmployeeCode.Equals(empcode));
                    var jwtToken = GenJwtToken(Acc);
                    Acc.RefreshToken = jwtToken.UserEntity.RefreshToken;
                    Acc.RefreshTokenExpireTime = jwtToken.UserEntity.RefreshTokenExpireTime;
                    await _db.SaveChangesAsync();

                    ResponseUser responseUser = await GetInfo(Acc);

                    return new AuthResponse
                    {
                        IsAuthSuccessful = true,
                        Token = jwtToken.Token,
                        RefreshToken = jwtToken.UserEntity.RefreshToken,
                        UserInfo = responseUser
                    };
                }

            }

            return new AuthResponse { IsAuthSuccessful = false, Token = "" };
        }

        private async Task<ResponseUser> GetInfo(User? Accounts)
        {
            var company = await _db.Company.FirstOrDefaultAsync(x => x.CompanyId == Accounts.CompanyId && x.IsDeleted == false);
            ResponseUser response = new ResponseUser();
            response.UserId = Accounts.UserId;
            response.EmployeeCode = Accounts.EmployeeCode;
            response.EmployeeName = Accounts.FirstName + " " + Accounts.LastName;
            response.UnitCode = Accounts.UnitCode;
            response.UnitName = Accounts.UnitName;
            response.CompanyName = company != null ? company.CompanyName : "";
            response.Email = Accounts.Email;
            response.RoleId = Accounts.UserRoleId;
            response.UserName = Accounts.UserName;

            if (!Accounts.FirstTimeLogin)
            {
                string Id = Accounts.UserId + "|" + Accounts.Email;
                var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(Id);
                // var UrlReset = MailSetting.UrlLink + "/reset-password?uid=" + System.Convert.ToBase64String(plainTextBytes);            
                response.FirstTimeLogin = System.Convert.ToBase64String(plainTextBytes);
            }


            var currentDateUtc = DateTime.Now.Date.ToUniversalTime();
            var otrainer = await _db.Trainer.FirstOrDefaultAsync(t => t.UserId == response.UserId && t.Active == true && t.IsDeleted == false
                && currentDateUtc >= t.EffectiveDate && currentDateUtc < t.ExpiredDate);

            if (otrainer != null)
            {
                response.TrainerId = otrainer.Id;
            }

            if (response.EmployeeCode != null && !string.IsNullOrWhiteSpace(response.EmployeeCode) && response.EmployeeCode.Length == 6)
            {
                int empCode2Number = -1;

                if (int.TryParse(response.EmployeeCode, out empCode2Number))
                {
                    PisRepository pisRepository = new PisRepository(_db);
                    var pisUserInfo = await pisRepository.GetUserDirectoryInfo(response.EmployeeCode);
                    if (pisUserInfo != null)
                    {
                        response.Position = pisUserInfo.PositionNameTH;

                        if (!string.IsNullOrWhiteSpace(pisUserInfo.DepartmentNameTH))
                        {
                            response.UnitName = pisUserInfo.DepartmentNameTH;
                        }

                        if (!string.IsNullOrWhiteSpace(pisUserInfo.EMailAddress))
                        {
                            response.Email = pisUserInfo.EMailAddress;
                        }

                        var ouser = await _db.User.FirstOrDefaultAsync(x => x.UserId == response.UserId && x.Active && !x.IsDeleted);
                        if (ouser != null)
                        {
                            //ouser.UnitCode = pisUserInfo.DepartmentCode;
                            ouser.UnitName = response.UnitName;
                            await _db.SaveChangesAsync();
                        }
                    }
                }
            }

            return response;
        }

        public async Task<AuthResponse> RefreshToken(TokenApi tokenAuthentication)
        {
            string accessToken = tokenAuthentication.Token;
            string refreshToken = tokenAuthentication.RefreshToken;
            var principal = _jwtHandler.GetPrincipalFromExpiredToken(accessToken);
            var username = tokenAuthentication.Username;
            if (string.IsNullOrWhiteSpace(username))
                username = principal.Identity.Name; //this is mapped to the Name claim by default
            var user = await _db.User.FirstOrDefaultAsync(u => u.IsDeleted == false && u.Active && u.UserName == username || u.EmployeeCode == username);

            if (user is null || user.RefreshToken != refreshToken || user.RefreshTokenExpireTime <= DateTime.Now)
                return new AuthResponse
                {
                    IsAuthSuccessful = false,
                    ErrorMessage = "Invalid client request"
                };

            var jwtToken = GenJwtToken(user);
            user.RefreshToken = jwtToken.UserEntity.RefreshToken;
            user.RefreshTokenExpireTime = jwtToken.UserEntity.RefreshTokenExpireTime;

            await _db.SaveChangesAsync();

            ResponseUser responseUser = await GetInfo(user);
            return new AuthResponse
            {
                IsAuthSuccessful = true,
                Token = jwtToken.Token,
                RefreshToken = jwtToken.UserEntity.RefreshToken,
                UserInfo = responseUser,
            };
        }


        public async Task<ResponseMessageData<string>> RevokeToken(UserAuthen userAuthen)
        {
            var user = await _db.User.SingleOrDefaultAsync(u => u.IsDeleted == false && u.Active && u.UserName == userAuthen.Username);
            if (user is null)
                return new ResponseMessageData<string> { Data = "", Success = false, Message = "Invalid client request" };

            bool verified = BCrypt.Net.BCrypt.EnhancedVerify(userAuthen.Password, user.Password);

            if (verified)
            {
                user.RefreshToken = null;
                await _db.SaveChangesAsync();
                return new ResponseMessageData<string> { Data = "", Success = true, Message = "" };
            }

            return new ResponseMessageData<string> { Data = "", Success = false, Message = "Invalid client request" };
        }

        public async Task<ResponseMessageData<string>> ForgotPass(UserForgot userForgot)
        {
            try
            {
                ResponseMessageData<string> response = new ResponseMessageData<string>();
                var Accounts = await _db.User.FirstOrDefaultAsync(x => x.IsDeleted == false && x.Active && x.Email.ToLower() == userForgot.Email.ToLower());

                if (Accounts != null)
                {

                    var _emailUtil = new EmailUtil(_db);
                    var emailLog = await _emailUtil.SendMail_Forgot(Accounts);

                    if (emailLog.Status == "Success")
                    {
                        response.Success = true;
                        response.Message = "Email Send";
                    }

                    return response;
                }
                else
                {
                    response.Data = "User Not Found";
                    response.Success = false;
                    return response;
                }
            }
            catch (Exception ex)
            {
                return new ResponseMessageData<string> { Data = null, Message = ex.Message, Success = false };
            }
        }
        public async Task<ResponseMessageData<string>> ResetPass(UserReset userReset)
        {
            try
            {

                var base64EncodedBytes = System.Convert.FromBase64String(userReset.Id);
                string idtext = System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
                var userparam = idtext.Split('|');
                int _Id = Convert.ToInt32(userparam[0]);
                string _Email = userparam[1];
                var existingUser = await _db.User.FirstOrDefaultAsync(x => x.IsDeleted == false && x.Active
                && (x.EmployeeTypeId == 2 || x.EmployeeTypeId == 3)
                && x.UserId.Equals(_Id)
                && x.Email.ToLower().Equals(_Email.ToLower()));

                if (existingUser != null)
                {

                    if (existingUser.EmployeeTypeId == 2 || existingUser.EmployeeTypeId == 3) //contract
                    {
                        bool checkOld = existingUser.Password == userReset.Password;
                        if (!checkOld && !string.IsNullOrEmpty(userReset.Password))
                        {
                            existingUser.Password = await GenPassword(userReset.Password, _Id);
                            existingUser.PasswordExpireTime = DateTime.Now.AddDays(90);
                            existingUser.FirstTimeLogin = true;
                            await _db.SaveChangesAsync();
                        }
                        else
                        {
                            return new ResponseMessageData<string> { Data = null, Message = "Error Password Invalid", Success = false };
                        }


                        var _emailUtil = new EmailUtil(_db);
                        var emailLog = await _emailUtil.SendMail_ResetPassword(existingUser, userReset.Password);
                        if (emailLog.Status == "Success")
                        {

                            return new ResponseMessageData<string> { Data = null, Message = "Email Send", Success = true };
                        }
                        else
                        {
                            return new ResponseMessageData<string> { Data = null, Message = "Email Send Error", Success = false };
                        }

                    }
                }
                return new ResponseMessageData<string> { Data = null, Message = "User not found", Success = false };
            }
            catch (Exception ex)
            {
                return new ResponseMessageData<string> { Data = null, Message = ex.Message, Success = false };
            }

        }
        private async Task<string> GenPassword(string pass, int UserId)
        {
            var checkUsed = await CheckPasswordHasBeenUsed(UserId, pass);
            if (checkUsed) throw new Exception("Password นี้เคยถูกใช้แล้ว");

            var newPass = BCrypt.Net.BCrypt.EnhancedHashPassword(pass);


            await _db.PasswordHistory.AddAsync(new PasswordHistory
            {
                UserId = UserId,
                EncryptPassword = newPass,
                Active = true,
                CreateDate = DateTime.Now,
                UpdateDate = DateTime.Now,
                CreateBy = UserId,
                UpdateBy = UserId,
                IsDeleted = false

            });
            await _db.SaveChangesAsync();
            return newPass;
        }
        public async Task<bool> CheckPasswordHasBeenUsed(int UserId, string pass)
        {
            var encryptedPasswords = await _db.PasswordHistory
                                              .Where(u => u.UserId == UserId && u.Active && !u.IsDeleted)
                                              .OrderByDescending(x => x.CreateDate)
                                              .Take(24)
                                              .Select(p => p.EncryptPassword)
                                              .ToListAsync();

            var passwordCheckTasks = encryptedPasswords.Select(password =>
                Task.Run(() => BCrypt.Net.BCrypt.EnhancedVerify(pass, password))
            ).ToList();

            var checkResults = await Task.WhenAll(passwordCheckTasks);

            return checkResults.Any(result => result);
        }
    }
}

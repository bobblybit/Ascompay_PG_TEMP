using AscomPayPG.Data;
using AscomPayPG.Data.Enum;
using AscomPayPG.Models;
using AscomPayPG.Models.Shared;
using AscomPayPG.Models.WAAS;
using AscomPayPG.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace AscomPayPG.Services.Implementation
{
    public class AccountService : IAccountService
    {
        private readonly AppDbContext _appcontext;
        private readonly IWalletService _walletService;
        public AccountService(AppDbContext appdbContext, IWalletService walletService)
        {
            _appcontext = appdbContext;
            _walletService = walletService;
        }

        public async Task<ApiBaseResponse<bool>> UpdateTier2AccountUpgradeStatus(string newStatus, AccountUpgrade accountUpgrade, string declineReason = "")
        {
            try
            {
             
                var accountTier = await _appcontext.AccountTiers.FirstOrDefaultAsync(x => x.Name.ToLower() == accountUpgrade.UpgradeT0.ToLower());
               var userExistingAccount = await _appcontext.Accounts.FirstOrDefaultAsync(x => x.AccountNumber == accountUpgrade.AccountNumber);

                var upgradeResponse = new PlainResponse();
                if (newStatus == "Approved")
                {

                    var upgradePostData = new WalletUpgradeTier2Request()
                    {
                        userPhoto = await ConverImageToBase64String(accountUpgrade.UserPhoto),
                        proofOfAddressVerification = await ConverImageToBase64String(accountUpgrade.ProofAddressURl),
                        customerSignature = await ConverImageToBase64String(accountUpgrade.CustomerSignature),
                        utilityBill = await ConverImageToBase64String(accountUpgrade.UtilityBill),
                        idCardFront = await ConverImageToBase64String(accountUpgrade.IdCardFront),
                        idCardBack = await ConverImageToBase64String(accountUpgrade.IdCardBack),

                        phoneNumber = accountUpgrade.PhoneNumber,
                        email = accountUpgrade.Email,
                        placeOfBirth = accountUpgrade.PlaceOfBirth,
                        idNumber = accountUpgrade.IdNumber,
                        bvn = accountUpgrade.Bvn,
                        nin = accountUpgrade.NIN,
                        idType = accountUpgrade.IdType,
                        accountName = accountUpgrade.AccountName,
                        accountNumber = accountUpgrade.AccountNumber,
                        idExpiryDate = accountUpgrade.IdExpiryDate,
                        idIssueDate = accountUpgrade.IdIssueDate,
                        houseNumber = accountUpgrade.HouseNumber,
                        streetName = accountUpgrade.StreetName,
                        localGovernment = accountUpgrade.LocalGovernment,
                        city = accountUpgrade.City,
                        state = accountUpgrade.State,
                        nearestLandmark = accountUpgrade.NearestLandmark,
                        approvalStatus = accountUpgrade.ApprovalStatus,
                        pep = accountUpgrade.Pep,
                        channelType = accountUpgrade.ChannelType,
                        tier = accountUpgrade.UpgradeT0
                    };


                    upgradeResponse = await _walletService.WalletUpgradeTier2(upgradePostData);

                    if (upgradeResponse.IsSuccessful)
                    {
                        // update upgrade
                        accountUpgrade.Status = newStatus;
                        accountUpgrade.LastUpdated = DateTime.Now;
                        _appcontext.AccountUpgrades.Update(accountUpgrade);

                        // update teir
                        userExistingAccount.AccountTeirId = accountTier.Id.ToString();
                        userExistingAccount.AccountTeir = accountTier;
                        _appcontext.Accounts.Update(userExistingAccount);
                        var responseAccount = _appcontext.SaveChanges() > 0;
                      
                        // send mail

                        //return response
                        return new ApiBaseResponse<bool>
                        {
                            IsSuccessful = true,
                            Data = true,
                            Errors = null,
                            Message = upgradeResponse.Message,
                            ResponseCode = upgradeResponse.ResponseCode
                        };

                    }
                    else
                    {
                        return new ApiBaseResponse<bool>
                        {
                            IsSuccessful = false,
                            Data = false,
                            Errors = null,
                            Message = upgradeResponse.Message,
                            ResponseCode = upgradeResponse.ResponseCode
                        };
                    }
                }

                accountUpgrade.Status = newStatus;
                accountUpgrade.LastUpdated = DateTime.Now;
                accountUpgrade.DeclineReason = declineReason;
                _appcontext.AccountUpgrades.Update(accountUpgrade);

                if (!(_appcontext.SaveChanges() > 0))
                {
                    return new ApiBaseResponse<bool>
                    {
                        Message = "something went wrong while updating upgrade status",
                        ResponseCode = StatusCodes.Status200OK,
                        IsSuccessful = false,
                        Data = false,
                        Errors = null
                    };
                }
                else
                {
                    return new ApiBaseResponse<bool>
                    {
                        Message = "Upgrade was successfully updated",
                        ResponseCode = StatusCodes.Status200OK,
                        IsSuccessful = true,
                        Data = true,
                        Errors = null
                    };
                }
            }
            catch (Exception ex)
            {

                return new ApiBaseResponse<bool>
                {
                    Message = ex.Message,
                    ResponseCode = StatusCodes.Status400BadRequest,
                    IsSuccessful = false,
                    Data = false,
                    Errors = null
                };
            }
        }

        public async Task<ApiBaseResponse<bool>> UpdateTier3AccountUpgradeStatus(string newStatus, AccountUpgrade accountUpgrade, string declineReason = "")
        {


            var accountTier = await _appcontext.AccountTiers.FirstOrDefaultAsync(x => x.Name.ToLower() == accountUpgrade.UpgradeT0.ToLower());
            var userExistingAccount = await _appcontext.Accounts.FirstOrDefaultAsync(x => x.AccountNumber == accountUpgrade.AccountNumber);


            var upgradeResponse = new PlainResponse();
            if (newStatus == "Approved")
            {
                string base64ImageString = await ConverImageToBase64String(accountUpgrade.ProofAddressURl);
                upgradeResponse = await _walletService.WalletUpgradeTier3multipart(new WalletUpgradeTier3Request { nin = accountUpgrade.NIN, bvn = userExistingAccount.Bvn, accountNumber = userExistingAccount.AccountNumber, proofOfAddressVerification = base64ImageString});
                
                if (upgradeResponse.IsSuccessful)
                {
                    // upgrade aacount
                    userExistingAccount.AccountTeirId = accountTier.Id.ToString();
                    userExistingAccount.AccountTeir = accountTier;
                    _appcontext.Accounts.Update(userExistingAccount);
                    // update upgrade status
                    accountUpgrade.Status = newStatus;
                    accountUpgrade.LastUpdated = DateTime.Now;
                    _appcontext.AccountUpgrades.Update(accountUpgrade);
                    // send mail 
                    // return response
                    return new ApiBaseResponse<bool>
                    {
                        IsSuccessful = true,
                        Data = true,
                        Errors = null,
                        Message = upgradeResponse.Message,
                        ResponseCode = upgradeResponse.ResponseCode
                    };
                }

                return new ApiBaseResponse<bool>
                {
                    IsSuccessful = false,
                    Data = false,
                    Errors = null,
                    Message = upgradeResponse.Message,
                    ResponseCode = upgradeResponse.ResponseCode
                };
            }

            accountUpgrade.Status = newStatus;
            accountUpgrade.DeclineReason = declineReason;
            accountUpgrade.LastUpdated = DateTime.Now;

            _appcontext.AccountUpgrades.Update(accountUpgrade);

            if (!(_appcontext.SaveChanges() > 0))
            {
                return new ApiBaseResponse<bool> 
                { 
                  Message = "something went wrong while approving upgrade",
                  ResponseCode = StatusCodes.Status200OK,
                  IsSuccessful = false,
                  Data = false,
                  Errors = null 
                };
            }
            else
            {
                // send mail

                        return new ApiBaseResponse<bool>
                        {
                            Message = "Status was successfull updated",
                            ResponseCode = StatusCodes.Status200OK,
                            IsSuccessful = true,
                            Data = true,
                            Errors = null
                        };
            }
        }

        public async Task<ApiBaseResponse<bool>> UpdateTierAccountUpgradeStatus(string accountUpgradeId, string newStatus, string declineReason = "")
        {
            var accountUpgrade = _appcontext.AccountUpgrades.FirstOrDefault(x => x.Id.ToString() == accountUpgradeId);
            if (accountUpgrade == null)
                return new ApiBaseResponse<bool> { Message = "Upgrade request was not found", ResponseCode = StatusCodes.Status200OK, IsSuccessful = false, Data = false, Errors = null };

            if (accountUpgrade.UpgradeT0 == AccountTierTypes.Tier2.ToString())
            
                return await UpdateTier2AccountUpgradeStatus( newStatus, accountUpgrade, declineReason = "");

            return await UpdateTier3AccountUpgradeStatus(newStatus, accountUpgrade,  declineReason = "");
        }

        private async Task<string> ConverImageToBase64String(string imagUrl)
        {
            byte[] imageByte = File.ReadAllBytes(imagUrl);
            return Convert.ToBase64String(imageByte);
        }
    }
}

using AutoMapper;
using FluentValidation;
using FSAKEB.Application.Extensions.FluentValidationRules;
using Hangfire.Logging;
using KEB.Application.DTOs.UserDTO;
using KEB.Application.Services.Interfaces;
using KEB.Application.Utils;
using KEB.Domain.Entities;
using KEB.Domain.ValueObject;
using KEB.Infrastructure.UnitOfWorks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UserCreateDTO = KEB.Application.DTOs.UserDTO.UserCreateDTO;


namespace KEB.Application.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        //private readonly ILog _logger;

        public UserService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }



        public IUnitOfWork UnitOfWork { get; }
        public IMapper Mapper { get; }

        public async Task<APIResponse<UserDisplayDTO>> AddUser(UserCreateDTO userCreateDTO, string ipAddress)
        {
            APIResponse<UserDisplayDTO> response = new();

           //kiem tra user co quyen tao khong
            var creator = await _unitOfWork.Users.GetUserById(userCreateDTO.CreatedBy);
            if (creator == null)
            {
                response.IsSuccess = false;
                response.StatusCode = System.Net.HttpStatusCode.MethodNotAllowed;
                response.Message = "Không tìm thấy người dùng";
                return response;
            }
            var creatorRoleId = creator?.RoleId ?? new Guid(LogicString.Role.LecturerRoleId); //id lecturer
            bool adminCreate = creatorRoleId.ToString().Equals(LogicString.Role.AdminRoleId) && !userCreateDTO.RoleId.ToString().Equals(LogicString.Role.AdminRoleId);
            bool leaderCreate = creatorRoleId.ToString().Equals(LogicString.Role.TeamLeadRoleId) && userCreateDTO.RoleId.ToString().Equals(LogicString.Role.LecturerRoleId);
            if (!adminCreate && !leaderCreate)
            {
                response.IsSuccess = false;
                response.StatusCode = System.Net.HttpStatusCode.Forbidden;
                response.Message = "Bạn không có quyền thực hiện tác vụ này!";
                return response;
            }

            //validation user
            UserCreateRequestValidator userValidator = new();
            var validateResult = await userValidator.ValidateAsync(userCreateDTO);
            if (!validateResult.IsValid)
            {
                response.Message = string.Join(" ", validateResult.Errors.First().ErrorMessage);
                response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                response.IsSuccess = false;
                return response;
            }

            //validation email
            var email = userCreateDTO.Email;
            var duplicate = await _unitOfWork.Users.GetUserByEmail(email);
            if (duplicate != null)
            {
                response.IsSuccess = false;
                response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                response.Message = "Email đã tồn tại!";
                return response;
            }
            //xu ly anh
           var image = await GetImageFile(userCreateDTO.ImageFile);
            await _unitOfWork.ImageFiles.AddAsync(image);

            var newUser = _mapper.Map<User>(userCreateDTO);
            
            string userName = email[..email.IndexOf('@')];
            newUser.Id = Guid.NewGuid();
            newUser.UserName = userName;
            newUser.Avatar = image.Id;
            newUser.IsActive = true;
            newUser.PhoneNumber = "";
            newUser.CreatedDate = DateTime.Now;
            newUser.UpdatedDate = DateTime.Now;
            newUser.IsDeleted = false;

            // Auto generate a password of length 8
            var randomPassword = "1234567";

            // Hash the password 
            string passwordHash = BitConverter.ToString(
            SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(randomPassword))
            ).Replace("-", "").ToLower();

            newUser.Password = passwordHash;
            try
            {
                // Perform Add
                await _unitOfWork.Users.AddAsync(newUser);
                // Access logging
                await _unitOfWork.AccessLogs.AddAsync(new SystemAccessLog
                {
                    UserId = userCreateDTO.CreatedBy,
                    AccessTime = DateTime.Now,
                    ActionName = "Add",
                    TargetObject = "Users",
                    IpAddress = ipAddress,
                    Details = creator?.UserName + " tạo tài khoản cho email " + newUser.Email + " với user name " + newUser.UserName
                });

                //Send email to the new user
                string subject = LogicString.Common.NEWACCOUNTSUBJECT;
                string body = string.Format(LogicString.Common.NEWACCOUTEMAIL,
                                            userName,
                                            LogicString.Common.ACTIVEALLOWEDTIMEVALUE,
                                            newUser.Email,
                                            userName,
                                            randomPassword);
                _unitOfWork.EmailService.SendEmail(newUser.Email, subject, body, newUser.FullName);

                //_unitOfWork.Schedule<UserService>((x) => AutoDeactiveAccount(newUser.Id), TimeSpan.FromMinutes(LogicString.Common.ACTIVEALLOWEDTIME));
                Console.WriteLine($"FullName: {newUser.FullName}, Email: {newUser.Email}, Avatar: {newUser.Avatar}");
                var result = _mapper.Map<UserDisplayDTO>(newUser);
                if (result != null)
                {
                    response.Result.Add(result);
                }
                else
                {
                    Console.WriteLine("Result is null");
                }
                response.IsSuccess = true;
                response.Message = AppMessages.CREATE_ACCOUNT_SUCCESS;
                response.StatusCode = System.Net.HttpStatusCode.Created;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                response.Message = ex.Message; 
                Console.WriteLine("ERROR: " + ex);
            }
            return response;


        }
        public static async Task<ImageFile> GetImageFile(IFormFile file)
        {
            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            Guid idImage = Guid.NewGuid();
            var image = new ImageFile
            {
                Id = idImage,
                FileName = file.FileName,
                FileData = memoryStream.ToArray(),
                ContentType = file.ContentType
            };
            return image;
        }
        public async Task AutoDeactiveAccount(Guid userId)
        {
            try
            {
                var targetUser = await _unitOfWork.Users.GetAsync(x => x.Id == userId);
                if (targetUser == null)
                {
                    return;
                }

                // Perform change active status
                targetUser.IsActive = false;
                targetUser.UpdatedDate = DateTime.Now;
                await _unitOfWork.SaveChangesAsync();

                string subject = LogicString.Common.AUTODEACTIVATE_SUBJECT;
                string body = string.Format(LogicString.Common.AUTODEACTIVEEMAIL, targetUser.UserName);
                //_unitOfWork.EmailService.SendEmail(targetUser.Email, subject, body, targetUser.FullName);

                // Access logging
                SystemAccessLog accessLog = new()
                {
                    UserId = userId,
                    AccessTime = DateTime.Now,
                    ActionName = "Auto-deactivate",
                    TargetObject = "Users",
                    IpAddress = "",
                    Details = targetUser?.UserName + " account has been deactivated automatically "
                };
                await _unitOfWork.AccessLogs.AddAsync(accessLog);
            }
            catch (Exception ex)
            {
            }
        }


        public async Task<APIResponse<UserDisplayDTO>> ChangeActiveStatus(ChangeActiveStatus request, string ipAddress)
        {
            APIResponse<UserDisplayDTO> response = new();

            var updateUser = await _unitOfWork.Users.GetAsync(x => x.Id == request.UpdatedBy, includeProperties: "Role");
            if (updateUser == null)
            {
                response.IsSuccess = false;
                response.Message = AppMessages.NO_PERMISSION;
                response.StatusCode = System.Net.HttpStatusCode.Forbidden;
                return response;
            }
            var targetUser = await _unitOfWork.Users.GetAsync(x => x.Id == request.TargertUserId, includeProperties: "Role");
            if (targetUser == null)
            {
                response.IsSuccess = false;
                response.Message = AppMessages.TARGET_ITEM_NOTFOUND;
                response.StatusCode = System.Net.HttpStatusCode.NotFound;
                return response;
            }

            bool userIsAdmin = targetUser.RoleId.ToString() == LogicString.Role.AdminRoleId;
            bool adminCanUpdate = updateUser.RoleId.ToString() == LogicString.Role.AdminRoleId;
            bool teamLeadCanUpdate = (updateUser.RoleId.ToString() == LogicString.Role.TeamLeadRoleId)
                                  && (targetUser.RoleId.ToString() == LogicString.Role.LecturerRoleId);
            if ((adminCanUpdate || teamLeadCanUpdate) && !userIsAdmin)
            { }
            else
            {
                response.IsSuccess = false;
                response.StatusCode = System.Net.HttpStatusCode.Forbidden;
                response.Message = AppMessages.NO_PERMISSION;
                return response;
            }
            try
            {
                SystemAccessLog accessLog = new() { AccessTime = DateTime.Now, UserId = request.UpdatedBy, TargetObject = LogicString.AccessLogConstant.USERS_CONTROLLER, IpAddress = ipAddress };
                string subject = "";
                string body = "";
                // Perform change active status
                if (targetUser.IsActive)
                {
                    subject = LogicString.Common.DEACTIVATESUBJECT;
                    body = string.Format(LogicString.Common.DEACTIVEEMAIL, targetUser.UserName);
                    accessLog.ActionName = "Deactivate";
                    accessLog.Details = string.Format(LogicString.AccessLogConstant.DEACTIVE_SUCCESS, updateUser.UserName, targetUser.UserName);
                    // targetUser.PasswordNeedChange = false;
                    response.Message = AppMessages.LOCK_ACCOUNT_SUCCESS;

                }
                else
                {
                    var randomPassword = CommonUntils.RandomGenerateString(LogicString.Common.PossibleCharsInPassword, 8);
                    using (SHA256 sha256Hash = SHA256.Create())
                    {
                        string hash = CommonUntils.GetHash(sha256Hash, randomPassword);
                        //targetUser.PasswordHash = hash;
                    }
                    subject = LogicString.Common.REACTIVATESUBJECT;
                    body = string.Format(
                        LogicString.Common.REACTIVATEEMAIL,
                        targetUser.UserName,
                        LogicString.Common.ACTIVEALLOWEDTIMEVALUE,
                        targetUser.Email,
                        targetUser.UserName,
                        randomPassword);
                    accessLog.ActionName = "Activate";
                    accessLog.Details = string.Format(LogicString.AccessLogConstant.ACTIVE_SUCCESS, updateUser.UserName, targetUser.UserName);
                    //  targetUser.PasswordNeedChange = true;
                    // ScheduleJob
                    var scheduleTime = DateTime.Now.AddSeconds(LogicString.Common.ACTIVEALLOWEDTIME);
                    _unitOfWork.Schedule<UserService>((x) => x.AutoDeactiveAccount(targetUser.Id), TimeSpan.FromMinutes(LogicString.Common.ACTIVEALLOWEDTIME));
                    response.Message = AppMessages.ACTIVATE_ACCOUNT_SUCCESS;

                }
                targetUser.IsActive = !targetUser.IsActive;
                targetUser.UpdatedDate = DateTime.Now;
                targetUser.UpdatedBy = request.UpdatedBy;
                await _unitOfWork.Users.UpdateAsync(targetUser);
                //_unitOfWork.EmailService.SendEmail(targetUser.Email, subject, body, targetUser.FullName);

                // Access logging
                await _unitOfWork.AccessLogs.AddAsync(accessLog);

                // Response
                var result = _mapper.Map<UserDisplayDTO>(targetUser);
                response.Result.Add(result);

            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                response.Message = AppMessages.INTERNAL_SERVER_ERROR;
            }
            return response;

        }

        public Task<APIResponse<UserDisplayDTO>> ChangePassCustomize(Guid userId, string input)
        {
            throw new NotImplementedException();
        }

        public async Task<APIResponse<UserDisplayDTO>> ChangeUserAvatar(ChangeAvatar request, string ipAddress)
        {
            APIResponse<UserDisplayDTO> response = new();
            SystemAccessLog accessLog = new()
            {
                UserId = request.Id,
                AccessTime = DateTime.Now,
                ActionName = LogicString.AccessLogConstant.UPDATE_ACTION,
                TargetObject = LogicString.AccessLogConstant.USERS_CONTROLLER,
                IpAddress = ipAddress,
                Details = "Update profile avatar"
            };
            try
            {
                // Check if user existed
                var targetUser = await _unitOfWork.Users.GetAsync(x => x.Id == request.Id, includeProperties: "Role");
                if (targetUser == null)
                {
                    response.IsSuccess = false;
                    response.Message = AppMessages.TARGET_ITEM_NOTFOUND;
                    response.StatusCode = System.Net.HttpStatusCode.NotFound;
                    return response;
                }

                //// Perform Update Azure Blob AvatarImage Image
                //if (string.IsNullOrEmpty(targetUser.Avatar)) { }
                //else if (!targetUser.Avatar.Equals(LogicString.AzureBlob.AzureBlobDefaultAvatar))
                //{
                //    await _unitOfWork.FileService.DeleteBlob(targetUser.Avatar, LogicString.AzureBlob.AzureBlobAvatarContainer);
                //}
                //string avatarBlobName = string.Format(LogicString.NameFormat.AVATAR_NAMEFORMAT, targetUser.UserName, DateTime.Now.ToString("yyyy-MM-dd HHmmss"));
                //await _unitOfWork.FileService.Upload(container: LogicString.AzureBlob.AzureBlobAvatarContainer,
                //                                      file: request.AvatarImg,
                //                                      fileName: avatarBlobName);
                //targetUser.Avatar = avatarBlobName;
                // Perform Update
                targetUser.UpdatedBy = request.Id;
                targetUser.UpdatedDate = DateTime.Now;
                //await _unitOfWork.Users.UpdateAsync(targetUser);
                // Access logging
                await _unitOfWork.AccessLogs.AddAsync(accessLog);
                var result = _mapper.Map<UserDisplayDTO>(targetUser);
                //result.AvatarUrl = await _unitOfWork.FileService.GetBlob(targetUser.Avatar, LogicString.AzureBlob.AzureBlobAvatarContainer);
                response.Result.Add(result);
                response.Message = AppMessages.UPDATE_AVATAR_SUCCESS;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                response.Message = AppMessages.INTERNAL_SERVER_ERROR;
            }
            return response;
        }

        public async Task<APIResponse<UserDisplayDTO>> ChangeUserRole(ChangeRole request, string ipAddress)
        {
            APIResponse<UserDisplayDTO> response = new();
            string changeDetails = string.Empty;
            var updateUser = await _unitOfWork.Users.GetAsync(x => x.Id == request.UpdatedBy, includeProperties: "Role");
            if (updateUser == null)
            {
                response.IsSuccess = false;
                response.Message = AppMessages.NO_PERMISSION;
                response.StatusCode = System.Net.HttpStatusCode.Forbidden;
                return response;
            }
            var targetUser = await _unitOfWork.Users.GetAsync(x => x.Id == request.TargetUserId, includeProperties: "Role");
            if (targetUser == null)
            {
                response.IsSuccess = false;
                response.Message = AppMessages.TARGET_ITEM_NOTFOUND;
                response.StatusCode = System.Net.HttpStatusCode.NotFound;
                return response;
            }
            //Mapp DTO to entity
            bool userIsAdmin = targetUser.RoleId.ToString() == LogicString.Role.AdminRoleId;
            bool changeToAdmin = request.RoleId.ToString() == LogicString.Role.AdminRoleId;
            bool adminCanUpdate = updateUser.RoleId.ToString() == LogicString.Role.AdminRoleId;
            bool teamLeadCanUpdate = (updateUser.RoleId.ToString() == LogicString.Role.TeamLeadRoleId)
                    && (targetUser.RoleId.ToString() == LogicString.Role.LecturerRoleId);
            if ((adminCanUpdate || teamLeadCanUpdate) && !userIsAdmin && !changeToAdmin)
            { }
            else
            {
                response.IsSuccess = false;
                response.StatusCode = System.Net.HttpStatusCode.Forbidden;
                response.Message = LogicString.Common.Error + LogicString.Permission.NoPermission;
                return response;
            }
            // Perform Update
            try
            {
                changeDetails = "Thay đổi chức vụ từ " + targetUser.Role.RoleName +
                    " thành " + _unitOfWork.Roles.GetRoleName(request.RoleId).Result + ".";
                if (targetUser.IsActive != request.IsActive)
                {
                    if (request.IsActive) changeDetails += "Mở khóa tài khoản";
                    else changeDetails += "Khóa tài khoản";
                }
                targetUser.RoleId = request.RoleId;
                targetUser.IsActive = request.IsActive;
                targetUser.UpdatedDate = DateTime.Now;
                targetUser.UpdatedBy = request.UpdatedBy;

                await _unitOfWork.Users.UpdateAsync(targetUser);
                // Access logging
                SystemAccessLog accessLog = new()
                {
                    AccessTime = DateTime.Now,
                    UserId = request.UpdatedBy,
                    ActionName = "Change Role",
                    TargetObject = LogicString.AccessLogConstant.USERS_CONTROLLER,
                    IpAddress = ipAddress,
                    Details = changeDetails,
                };
                await _unitOfWork.AccessLogs.AddAsync(accessLog);

                var result = _mapper.Map<UserDisplayDTO>(targetUser);
                response.Result.Add(result);
                response.Message = AppMessages.UPDATE_PROFILE_SUCCESS;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                response.Message = AppMessages.INTERNAL_SERVER_ERROR;
            }
            return response;

        }

        public async Task<APIResponse1<UserDisplayDTO>> GetUserById(Guid userId)
        {
            APIResponse1<UserDisplayDTO> response = new();

            var user = await _unitOfWork.Users
                .GetAsync(x => x.Id == userId, includeProperties: "Role,ImageFile");

            if (user == null)
            {
                response.IsSuccess = false;
                response.Message = AppMessages.TARGET_ITEM_NOTFOUND;
                response.StatusCode = System.Net.HttpStatusCode.NotFound;
                return response;
            }
            try
            {
                var userDto = _mapper.Map<UserDisplayDTO>(user);
                //ImageFile
                if(user.ImageFile != null || user.ImageFile.FileData != null)
                {
                    string base64Image = Convert.ToBase64String(user.ImageFile.FileData);
                    userDto.AvatarUrl = base64Image;
                }
                // Thêm kết quả vào response
                response.Result = userDto;
                response.IsSuccess = true;
                response.StatusCode = HttpStatusCode.OK;

            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                response.Message = AppMessages.INTERNAL_SERVER_ERROR;
            }
            return response;
        }

        public async Task<APIResponse<UserDisplayDTO>> GetUsers(GetUser request)
        {
            APIResponse<UserDisplayDTO> response = new();

            // Base filter
            Expression<Func<User, bool>> filter = x => true;

            // Combine filters 
            if (request.IsActive != null)
            {
                var activeStatusFilter = (Expression<Func<User, bool>>)(x => x.IsActive == request.IsActive);
                filter = ExpressionExtension.CombineFilters(filter, activeStatusFilter);
            }
            try
            {
                // Perform Get
                var allUsers = await _unitOfWork.Users.GetAllAsync(
                    filter: filter,
                    pageNumber: request.PaginationRequest?.Page ?? 0,
                    pageSize: request.PaginationRequest?.Size ?? 0,
                    includeProperties: "Role");

                var result = new List<UserDisplayDTO>();
                foreach (var user in allUsers)
                {
                    var tempUserDisplay = _mapper.Map<UserDisplayDTO>(user);
                    //if (!string.IsNullOrEmpty(user.Avatar) && _unitOfWork.FileService != null)
                    //{
                    //    var avatarUrl = await _unitOfWork.FileService.GetBlob(user.Avatar, LogicString.AzureBlob.AzureBlobAvatarContainer);
                    //    tempUserDisplay.AvatarUrl = !string.IsNullOrEmpty(avatarUrl) ? avatarUrl : "URL ảnh không tìm thấy.";
                    //}

                    result.Add(tempUserDisplay);
                }
                // Set the result and success status
                response.Result = result;
                response.IsSuccess = true;
                response.Message = $"Tìm thấy {result.Count} bản ghi.";

                return response;
            }
            catch (Exception ex)
            {
                response.Message = AppMessages.INTERNAL_SERVER_ERROR;
                response.IsSuccess = false;
                response.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                return response;
            }


        }

        public async Task<APIResponse<UserDisplayDTO>> UpdateUserProfile(UpdateUser request, string ipAddress)
        {
            var response = new APIResponse<UserDisplayDTO>();
            bool updated = false;

            // Ghi log truy cập
            var accessLog = new SystemAccessLog
            {
                UserId = request.UserId,
                AccessTime = DateTime.Now,
                ActionName = LogicString.AccessLogConstant.UPDATE_ACTION,
                TargetObject = LogicString.AccessLogConstant.USERS_CONTROLLER,
                IpAddress = ipAddress,
                Details = "Update profile: "
            };

            // Kiểm tra user có tồn tại không
            var targetUser = await _unitOfWork.Users.GetAsync(
                x => x.Id == request.UserId,
                includeProperties: "Role"
            );

            if (targetUser == null)
            {
                response.IsSuccess = false;
                response.Message = AppMessages.TARGET_ITEM_NOTFOUND;
                response.StatusCode = System.Net.HttpStatusCode.NotFound;
                return response;
            }

            // Validate đầu vào
            //var validator = new UserUpdateRequestValidator();
            //var validationResult = await validator.ValidateAsync(request);
            //if (!validationResult.IsValid)
            //{
            //    response.Message = string.Join(" ", validateResult.Errors.First().ErrorMessage);
            //    response.StatusCode = System.Net.HttpStatusCode.BadRequest;
            //    response.IsSuccess = false;
            //    return response;
            //}

            // Kiểm tra từng trường và cập nhật nếu khác
            if (!string.Equals(targetUser.FullName, request.FullName?.Trim(), StringComparison.Ordinal))
            {
                targetUser.FullName = request.FullName.Trim();
                accessLog.Details += "FullName, ";
                updated = true;
            }

            if (targetUser.Gender != request.Gender)
            {
                targetUser.Gender = request.Gender;
                accessLog.Details += "Gender, ";
                updated = true;
            }

            if (targetUser.DateOfBirth != request.DateOfBirth)
            {
                targetUser.DateOfBirth = request.DateOfBirth;
                accessLog.Details += "DateOfBirth";
                updated = true;
            }

            try
            {
                if (updated)
                {
                    targetUser.UpdatedBy = request.UserId;
                    targetUser.UpdatedDate = DateTime.Now;

                    await _unitOfWork.Users.UpdateAsync(targetUser);
                    await _unitOfWork.AccessLogs.AddAsync(accessLog);
                }

                var result = _mapper.Map<UserDisplayDTO>(targetUser);
                response.Result.Add(result);
                response.Message = AppMessages.UPDATE_PROFILE_SUCCESS;
                response.IsSuccess = true;
                response.StatusCode = HttpStatusCode.OK;
            }
            catch (Exception)
            {
                response.IsSuccess = false;
                response.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                response.Message = AppMessages.INTERNAL_SERVER_ERROR;

            }

            return response;
        }

    }
}

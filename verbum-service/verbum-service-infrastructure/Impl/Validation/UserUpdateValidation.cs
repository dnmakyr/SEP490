﻿using Microsoft.EntityFrameworkCore;
using verbum_service_application.Validation;
using verbum_service_domain.Common.ErrorModel;
using verbum_service_domain.DTO.Request;
using verbum_service_domain.Utils;
using verbum_service_infrastructure.DataContext;

namespace verbum_service_infrastructure.Impl.Validation
{
    public class UserUpdateValidation: IValidation<UserUpdate>
    {
        private readonly verbumContext context;
        public UserUpdateValidation(verbumContext context)
        {
            this.context = context;
        }
        public async Task<List<string>> Validate(UserUpdate request)
        {
            List<string> alerts = new List<string>();
            ValidateEmpty(request, alerts);
            await ValidateEmail(request, alerts);
            return alerts;
        }

        private async Task ValidateEmail(UserUpdate request, List<string> alerts)
        {
            if (!ValidationUtils.IsValidEmail(request.Data.Email))
            {
                alerts.Add(AlertMessage.Alert(ValidationAlertCode.INVALID, "this email"));
            }
            if (await context.Users.AnyAsync(x => x.Email == request.Data.Email && x.Id != request.UserId))
            {
                alerts.Add(AlertMessage.Alert(ValidationAlertCode.DUPLICATE, "this email"));
            }
        }

        private void ValidateEmpty(UserUpdate request, List<string> alerts)
        {
            if (ObjectUtils.IsEmpty(request.Data.Name))
            {
                alerts.Add(AlertMessage.Alert(ValidationAlertCode.REQUIRED, "name"));
            }
            if (ObjectUtils.IsEmpty(request.Data.Email))
            {
                alerts.Add(AlertMessage.Alert(ValidationAlertCode.REQUIRED, "email"));
            }
            if (ObjectUtils.IsEmpty(request.Data.Role))
            {
                alerts.Add(AlertMessage.Alert(ValidationAlertCode.REQUIRED, "role"));
            }
        }
    }
}

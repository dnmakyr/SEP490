﻿using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using verbum_service_application.Service;
using verbum_service_domain.Models;

namespace verbum_service_infrastructure.Impl.Service
{
    public class PhotoServiceImpl : PhotoService
    {
        public IConfiguration Configuration { get; }
        private CloudinarySettings setting;
        private Cloudinary cloundinary;

        public PhotoServiceImpl(IConfiguration configuration)
        {
            Configuration = configuration;
            setting = Configuration.GetSection("CloudinarySettings").Get<CloudinarySettings>();
            CloudinaryDotNet.Account account = new CloudinaryDotNet.Account(setting.CloudName,
                setting.ApiKey,
                setting.ApiSecret);
            cloundinary = new Cloudinary(account);
        }
        public override Image UploadImage(IFormFile inputFile)
        {
            Image returnImg = new Image();

            var result = new ImageUploadResult();
            if (inputFile.Length > 0)
            {
                using (var stream = inputFile.OpenReadStream())
                {
                    var param = new ImageUploadParams()
                    {
                        File = new FileDescription(inputFile.Name, stream)
                    };

                    result = cloundinary.Upload(param);
                }
            }

            returnImg.ImageLink = result.Uri.ToString();

            return returnImg;
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace verbum_service_domain.DTO.Request
{
    public class UploadIssueAttachmentFiles
    {
        public string AttachmentUrl { get; set; }
        public string Tag { get; set; }
    }
}

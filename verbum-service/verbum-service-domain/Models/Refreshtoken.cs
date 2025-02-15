﻿using System;
using System.Collections.Generic;

namespace verbum_service_domain.Models;

public partial class RefreshToken
{
    public int TokenId { get; set; }

    public DateTime IssuedAt { get; set; }

    public DateTime ExpireAt { get; set; }

    public string TokenContent { get; set; } = null!;

    public virtual User? User { get; set; }
}

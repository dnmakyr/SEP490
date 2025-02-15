using System;
using System.Collections.Generic;

namespace verbum_service_domain.Models;

public partial class Language
{
    public string LanguageName { get; set; } = null!;

    public string LanguageId { get; set; } = null!;

    public bool Support { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<Revelancy> RevelancySourceLanguages { get; set; } = new List<Revelancy>();

    public virtual ICollection<Revelancy> RevelancyTargetLanguages { get; set; } = new List<Revelancy>();

    public virtual ICollection<Order> OrdersNavigation { get; set; } = new List<Order>();
}

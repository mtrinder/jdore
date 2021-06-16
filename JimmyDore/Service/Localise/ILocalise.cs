using System;
using System.Globalization;

namespace JimmyDore.Services.Localise
{
    public interface ILocalise
    {
        CultureInfo GetCurrentCultureInfo();
        void SetLocale(CultureInfo ci);
    }
}

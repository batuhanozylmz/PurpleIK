using PurpleIK.Entities;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace PurpleIK.UI.Utility
{
    public static class CreateUserInformation
    {

        public static string CompanyMailNameCreate(string _companyName,string _firstName,string _lastName)
        {
            string firstWordCompany = GetFormattedCompanyName(_companyName);

            string firstName = GetFormattedName(_firstName);
            string lastName = GetFormattedName(_lastName);

                return $"{firstName}.{lastName}@{firstWordCompany}.com";
        }
        public static string UserNameCreate(string _firstName,string _lastName)
        {
            string firstName = GetFormattedName(_firstName);
            string lastName = GetFormattedName(_lastName);
            return $"{firstName}{lastName}";
        }
        // Şirket adını formatlayan yöntem
        private static string GetFormattedCompanyName(string companyName)
        {
            string companyNameForMail = companyName.ToLower().Trim();
            string[] words = companyNameForMail.Split(' ');
            return words[0].Replace("ı", "i")
                           .Replace("ğ", "g")
                           .Replace("ü", "u")
                           .Replace("ş", "s")
                           .Replace("ö", "o")
                           .Replace("ç", "c");
        }

        // Ad veya soyadı formatlayan yöntem
        private static string GetFormattedName(string name)
        {
            string nameForMail = name.ToLower().Trim();
            string[] nameWords = nameForMail.Split(' ');
            string formattedName = nameWords[0];

            // Eğer ikinci kelime varsa birleştir
            if (nameWords.Length > 1)
            {
                formattedName += nameWords[1];
            }

            // Eğer üçüncü kelime varsa birleştir
            if (nameWords.Length > 2)
            {
                formattedName += nameWords[2];
            }

            return formattedName.Replace("ı", "i")
                                .Replace("ğ", "g")
                                .Replace("ü", "u")
                                .Replace("ş", "s")
                                .Replace("ö", "o")
                                .Replace("ç", "c");
        }
    }
}

using Microsoft.IdentityModel.Tokens;

namespace VolgaIT.OtherClasses
{
    public class CustomLifeTime
    {

        static public bool CustomLifeTimeValidator(DateTime? notBefore, DateTime? expires, 
                                                    SecurityToken tokenToValidate, TokenValidationParameters @param)
        {
            if (expires != null)
                return expires > DateTime.UtcNow;

            return false;
        }

    }
}

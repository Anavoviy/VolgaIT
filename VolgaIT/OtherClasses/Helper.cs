using Microsoft.AspNetCore.Identity;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System;
using System.Drawing;
using VolgaIT.Model.Entities;
using VolgaIT.Model.Model;

namespace VolgaIT.OtherClasses
{
    public static class Helper
    {
        public static T ConvertTo<V, T>(this object from, V classFrom)
        {
            var typeFrom = typeof(V);
            var propsFrom = typeFrom.GetProperties();

            var typeTo = typeof(T);
            var propsTo = typeTo.GetProperties();

            var result = Activator.CreateInstance(typeTo);
            foreach (var propF in propsFrom)
                foreach (var propT in propsTo)
                    if (propF.Name == propT.Name)
                        propT.SetValue(result, propF.GetValue(from));

            return (T)result;
        }

        

    }
}

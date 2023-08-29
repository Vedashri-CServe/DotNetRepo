using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.Helper.ImportExcel
{
    public static class ImportExcel
    {
        public static bool IsValid<T>(T value) where T : class
        {
            var props = value.GetType().GetProperties();

            bool isValid = false;
            isValid = !props.Any(x => x.GetValue(value) == null);
            if(isValid) 
            {
                foreach (var prop in props)
                {
                    if (prop.PropertyType.Name.Equals("String"))
                    {
                        bool validString = string.IsNullOrEmpty(Convert.ToString(prop.GetValue(value)));
                        if(validString) return false;

                    }
                }
            }
            return isValid;
        }
    }
}

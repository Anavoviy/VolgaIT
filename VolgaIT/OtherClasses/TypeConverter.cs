namespace VolgaIT.OtherClasses
{
    public class TypeConverter
    {
        public delegate dynamic ConvertProperty
            (dynamic convertFrom, dynamic convertTo, string propertyName);
        private Dictionary<int, ConvertProperty> _customizations;

        public TypeConverter()
        {
            _customizations = new Dictionary<int, ConvertProperty>();
        }

        public void Register(Type convertFrom, Type convertTo, string propertyName, ConvertProperty convert)
        {
            int hash = new ConvertKey(convertFrom, convertTo, propertyName).GetHashCode();
            if (!_customizations.ContainsKey(hash))
                _customizations.Add(hash, convert);
        }

        public T2 ConvertTypes<T1, T2>(T1 convertFrom, T2 convertTo)
        {
            var convertFromType = convertFrom.GetType();
            var propertyValues = new Dictionary<string, object>();
            var neededProperties = convertFromType.GetProperties();
            foreach (var property in neededProperties)
                propertyValues.Add(property.Name, property.GetValue(convertFrom, null));

            var convertToType = convertTo.GetType();
            var outputProperties = convertToType.GetProperties();
            foreach (var property in outputProperties)
            {
                if (propertyValues.ContainsKey(property.Name))
                {
                    int hash = new ConvertKey(convertFromType, convertToType, property.Name)
                        .GetHashCode();
                    if (_customizations.ContainsKey(hash))
                        property.SetValue(convertTo, _customizations[hash](convertFrom, convertTo, property.Name), null);
                    else
                        property.SetValue(convertTo, propertyValues[property.Name], null);
                }
            }

            return convertTo;
        }

        private class ConvertKey
        {
            private Type _convertFrom;
            private Type _convertTo;
            private string _propertyName;

            public ConvertKey(Type convertFrom, Type convertTo, string propertyName)
            {
                _convertFrom = convertFrom;
                _convertTo = convertTo;
                _propertyName = propertyName;
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return _convertFrom.GetHashCode() * _convertTo.GetHashCode()
                        * _propertyName.GetHashCode();
                }
            }
        }
    }
}

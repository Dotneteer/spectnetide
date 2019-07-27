using System;
using System.ComponentModel;
using System.Globalization;

namespace Spect.Net.VsPackage.VsxLibrary
{
    public class TypedEnumConverter<TType> : EnumConverter
    {
        private readonly Type _enumType;

        public TypedEnumConverter(Type type) : base(type)
        {
            _enumType = type;
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destType)
        {
            return destType == typeof(string);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture,
            object value, Type destType)
        {
            var fi = _enumType.GetField(Enum.GetName(_enumType, value));
            var dna = (DescriptionAttribute)Attribute.GetCustomAttribute(fi,
                typeof(DescriptionAttribute));
            return dna != null ? dna.Description : value.ToString();
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type srcType)
        {
            return srcType == typeof(string);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture,
            object value)
        {
            foreach (var fi in _enumType.GetFields())
            {
                var dna = (DescriptionAttribute)Attribute.GetCustomAttribute(fi,
                    typeof(DescriptionAttribute));
                if ((dna != null) && ((string)value == dna.Description))
                    return Enum.Parse(_enumType, fi.Name);
            }
            // ReSharper disable once AssignNullToNotNullAttribute
            return Enum.Parse(_enumType, value as string);
        }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace TestUtilApp.Editors
{
    /// <summary>
    /// PropertyGrid에서 List&lt;string&gt;의 내용을 미리보기로 표시하는 TypeConverter
    /// Displays actual items separated by commas instead of "(Collection)".
    /// </summary>
    public class StringListConverter : TypeConverter
    {
        /// <summary>
        /// List&lt;string&gt;을 문자열로 변환 가능함을 알림
        /// </summary>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                return true;
            }
            return base.CanConvertTo(context, destinationType);
        }

        /// <summary>
        /// List&lt;string&gt;을 문자열로 변환
        /// Displays up to 3 items and shows "... and N more" when there are more.
        /// </summary>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string) && value is List<string> list)
            {
                if (list == null || list.Count == 0)
                {
                    return "(Empty)";
                }

                const int maxDisplay = 3;

                if (list.Count <= maxDisplay)
                {
                    // 3개 이하면 모두 표시
                    return string.Join(", ", list);
                }
                else
                {
                    // 3개 초과면 앞 3개만 표시하고 나머지 개수 표시
                    var displayed = list.Take(maxDisplay);
                    int remaining = list.Count - maxDisplay;
                    return $"{string.Join(", ", displayed)} ... and {remaining} more";
                }
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        /// <summary>
        /// 문자열에서 List&lt;string&gt;으로 변환 가능함을 알림
        /// </summary>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }
            return base.CanConvertFrom(context, sourceType);
        }

        /// <summary>
        /// 문자열을 List&lt;string&gt;으로 변환
        /// 쉼표로 구분된 문자열을 파싱
        /// </summary>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string str)
            {
                if (string.IsNullOrWhiteSpace(str) || str == "(Empty)")
                {
                    return new List<string>();
                }

                // 쉼표로 분리하고 앞뒤 공백 제거
                var items = str.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                               .Select(s => s.Trim())
                               .Where(s => !string.IsNullOrEmpty(s))
                               .ToList();

                return items;
            }

            return base.ConvertFrom(context, culture, value);
        }
    }
}

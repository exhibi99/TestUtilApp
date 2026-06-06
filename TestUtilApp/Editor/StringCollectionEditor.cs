using System;
using System.ComponentModel.Design;

namespace TestUtilApp.Editors
{
    /// <summary>
    /// PropertyGrid에서 List&lt;string&gt; 편집을 위한 CollectionEditor
    /// Resolves the "System.String constructor could not be found" error.
    /// </summary>
    public class StringCollectionEditor : CollectionEditor
    {
        public StringCollectionEditor(Type type) : base(type)
        {
        }

        /// <summary>
        /// 컬렉션 아이템 타입을 string으로 지정
        /// </summary>
        protected override Type CreateCollectionItemType()
        {
            return typeof(string);
        }

        /// <summary>
        /// 새 string 인스턴스 생성
        /// </summary>
        protected override object CreateInstance(Type itemType)
        {
            if (itemType == typeof(string))
            {
                return string.Empty;
            }
            return base.CreateInstance(itemType);
        }
    }
}

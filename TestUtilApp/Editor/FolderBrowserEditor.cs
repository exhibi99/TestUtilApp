using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using TestUtilApp.UI;

namespace TestUtilApp.Editors
{
    /// <summary>
    /// PropertyGrid에서 폴더 선택 대화상자를 표시하는 UITypeEditor
    /// "..." 버튼을 클릭하면 FolderBrowserDialog가 열립니다.
    /// </summary>
    public class FolderBrowserEditor : UITypeEditor
    {
        /// <summary>
        /// 편집 스타일 정의 - Modal 대화상자 사용
        /// </summary>
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        /// <summary>
        /// 실제 편집 로직 - FolderBrowserDialog 표시
        /// </summary>
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (provider != null)
            {
                var editorService = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;

                if (editorService != null)
                {
                    string selected = FolderPickerDialog.Show(null, "Select the DICE model folder.", value as string);
                    if (selected != null)
                        return selected;
                }
            }

            // 취소하거나 오류 발생 시 기존 값 반환
            return value;
        }
    }
}

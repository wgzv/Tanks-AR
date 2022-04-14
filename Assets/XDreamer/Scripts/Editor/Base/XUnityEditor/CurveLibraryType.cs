using XCSJ.Algorithms;

namespace XCSJ.EditorExtension.Base.XUnityEditor
{
    [LinkType(EditorHelper.UnityEditorPrefix + nameof(CurveLibraryType))]
    public enum CurveLibraryType
    {
        Unbounded,
        NormalizedZeroToOne
    }
}

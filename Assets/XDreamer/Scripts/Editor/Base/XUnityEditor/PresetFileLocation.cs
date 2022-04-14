using XCSJ.Algorithms;

namespace XCSJ.EditorExtension.Base.XUnityEditor
{
    [LinkType(EditorHelper.UnityEditorPrefix + nameof(PresetFileLocation))]
    public enum PresetFileLocation
    {
        PreferencesFolder,
        ProjectFolder
    }
}

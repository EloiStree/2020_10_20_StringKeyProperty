using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;

public class DefaultApplicationConfigurationStringKeyValueMono : MonoBehaviour
{

    public string m_configNameId = "DefaultApplicationName";
    public string m_fileRelativePathDev = "configuration/defaultapplication.keyproperty";
    public TextAsset m_defaultConfigIfFileDontExist;

    public bool autoImportAwake=true;
    public bool autoExportDestroy=true;

    public bool pushValueInStaticAccess=true;

    [Header("Debug")]
    public StringKeyPropertyGroup m_application = new StringKeyPropertyGroup("DefaultApplicationConfig");
    public StringKeyPropertyGroupEvent m_onImportDetected;
    public StringKeyPropertyGroupEvent m_onExportDetected;

    public void Awake()
    {
        if (autoImportAwake) {
            Import();
        }
        if (pushValueInStaticAccess)
            StringKeyPropertyFacade.SetOrOverride(in m_application);
    }


    [ContextMenu("Import")]
    public  void Import()
    {
        string path = GetPathToUse();
        string directoryName = Path.GetDirectoryName(path);
        if (!Directory.Exists(directoryName))
            Directory.CreateDirectory(directoryName);
        if (!File.Exists(path)) {
            //StringKeyPropertyImport.ImportFromText( m_defaultConfigIfFileDontExist.text, m_configNameId,out bool converted, out m_application);
            //StringKeyPropertyImport.Export(in path, in m_application);
            File.WriteAllText(path, m_defaultConfigIfFileDontExist.text);

        }

        StringKeyPropertyImport.Import(in path, out bool found, out m_application);
        if (!found && m_defaultConfigIfFileDontExist != null)
        {
            File.WriteAllText(path, m_defaultConfigIfFileDontExist.text);
        }
        if (!found && m_defaultConfigIfFileDontExist == null)
        {
            File.WriteAllText(path,"");
        }
        m_onImportDetected.Invoke(m_application);
    }

    [ContextMenu("OpenFolder")]
    public void OpenFolder()
    {

        Application.OpenURL(Path.GetDirectoryName(GetPathToUse()));
    }
    [ContextMenu("OpenFile")]
    public void OpenFile()
    {

        Application.OpenURL(GetPathToUse());
    }
    private string GetPathToUse()
    {
        Eloi.E_FilePathUnityUtility.MeltPathTogether(out string path, Directory.GetCurrentDirectory(), m_fileRelativePathDev);
        return path;
    }

    public void OnDestroy()
    {
        if (pushValueInStaticAccess) {
            m_application = StringKeyPropertyFacade.Get(m_application.m_name);
        }
        if (autoExportDestroy)
            Export();
    }

    [ContextMenu("Export")]
    public void Export()
    {
        string path = GetPathToUse();
        StringKeyPropertyImport.Export(in path, in m_application);
        m_onExportDetected.Invoke(m_application);
    }

    public void DebugLogExport(StringKeyPropertyGroup group)
    {
        Debug.Log("Export: " + group.m_name);
    }
    public void DebugLogImport(StringKeyPropertyGroup group)
    {
        Debug.Log("Import: " + group.m_name);
    }

    public StringKeyPropertyGroup GetConfigurationRef() { return m_application; }
}
[System.Serializable]
public class StringKeyPropertyGroupEvent : UnityEvent<StringKeyPropertyGroup>
{

}
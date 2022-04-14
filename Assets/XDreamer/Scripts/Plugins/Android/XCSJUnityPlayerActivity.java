package com.xcsj.PluginOSInteract;

import com.unity3d.player.UnityPlayer;
import com.unity3d.player.UnityPlayerActivity;

import android.app.Activity;
import android.os.Bundle;
import android.util.Log;
import android.view.MenuItem;
import android.view.View;

import org.json.JSONException;
import org.json.JSONObject;


/**
 * Android �� Unity ��������
 * ����������� Unity ������������������ͬʱ���� Unity �� Android �еĲ�������
 * @author wjr
 * @reviser wqh
 */
public class XCSJUnityPlayerActivity extends UnityPlayerActivity 
{	
	public final String MsgCmd = "MsgCmd";
	public final String Msg = "Msg";

	@Override
	protected void onCreate(Bundle savedInstanceState) 
	{
		super.onCreate(savedInstanceState);
	}
	
	/**
	 * ��ȡu3d ��Ⱦ����
	 * @return
	 */
	public View getUnityView()
	{
		if (mUnityPlayer==null) return null ;
		return mUnityPlayer.getView() ;
	}
	
	//--------- Android --> Unity ----------------------------------------------------------------------//	
	
	/**
	 * ���벢���س���
	 * @param scenePath ����·��
	 * @param sceneName ������
	 */
	public void androidToUnity_ImportAndLoadScene(String scenePath, String sceneName)
	{
		androidToUnityInternal2("ImportAndLoadScene", "scenePath", scenePath, "sceneName", sceneName);
	}

	/**
	 * ���볡��
	 * @param scenePath ����·��
	 * @param sceneName ������
	 */
	public void androidToUnity_ImportScene(String scenePath, String sceneName)
	{
		androidToUnityInternal2("ImportScene", "scenePath", scenePath, "sceneName", sceneName);
	}
	
	/**
	 * ���س���
	 * @param scenePath ����·��
	 * @param sceneName ������
	 */
	public void androidToUnity_LoadScene(String scenePath, String sceneName)
	{
		androidToUnityInternal2("LoadScene", "scenePath", scenePath, "sceneName", sceneName);
	}	
	
	/**
	 * ���ػ��벢���س���
	 * @param scenePath ����·��
	 * @param sceneName ������
	 */
	public void androidToUnity_LoadOrImportAndLoadScene(String scenePath, String sceneName)
	{
		androidToUnityInternal2("LoadOrImportAndLoadScene", "scenePath", scenePath, "sceneName", sceneName);
	}
	
	/**
	 * ж���ӳ���
	 * @param sceneName ������
	 */
	public void androidToUnity_UnloadSubScene(String sceneName)
	{
		androidToUnityInternal1("UnloadSubScene", "sceneName", sceneName);
	}
	
	/**
	 * ж���ӳ���(ͨ������)
	 * @param sceneIndex ������������1��ʼ��
	 */
	public void androidToUnity_UnloadSubSceneByIndex(int sceneIndex)
	{		
		androidToUnityInternal1("UnloadSubSceneByIndex", "sceneIndex", String.valueOf(sceneIndex));
	}
	
	/**
	 * ж��ȫ���ӳ���
	 */
	public void androidToUnity_UnloadAllSubScene()
	{
		androidToUnityInternal0("UnloadAllSubScene");
	}
	
	/**
	 * ���󳡾������б�
	 */
	public void androidToUnity_RequestSceneNameList()
	{
		androidToUnityInternal0("RequestSceneNameList");
	}
	
	/**
	 * �Զ�����Ϣ
	 * @param userDefine �Զ�����Ϣ
	 */
	public void androidToUnity_UserDefine(String userDefine)
	{
		androidToUnityInternal1("UserDefine", "userDefine", userDefine);
	}
	
	/**
	 * �����Զ��庯��
	 * @param userDefineFunName �Զ��庯����
	 * @param param ����
	 */
	public void androidToUnity_CallUserDefineFun(String userDefineFunName, String param)
	{		
		androidToUnityInternal2("CallUserDefineFun", "userDefineFunName", userDefineFunName, "param", param);
	}
	
	/**
	 * ִ��XCSJ�ű�
	 * @param xcsjScript �� ��ִ�е�XCSJ���Ľű������֮��ʹ�û��з��ָ
	 */
	public void androidToUnity_RunXCSJScript(String xcsjScript)
	{      
		androidToUnityInternal1("RunXCSJScript", "xcsjScript", xcsjScript); 
	}
	
	/**
	 * ִ�е���XCSJ�ű������ؽ��
	 * @param xcsjScript �� ��ִ�еĵ���XCSJ���Ľű�
	 */
	public void androidToUnity_RunSingleXCSJScriptAndReturnResult(String xcsjScript)
	{    
		androidToUnityInternal1("RunSingleXCSJScriptAndReturnResult", "xcsjScript", xcsjScript);
	}
	
	/**
	 * ����ͼƬ��ά��ɨ��
	 * @param imagePath ͼƬ·��
	 * @param otherInfo ������Ϣ
	 */
	public void androidToUnity_RequestImageQRCodeScan(String imagePath, String otherInfo)
	{
		androidToUnityInternal2("RequestImageQRCodeScan", "imagePath", imagePath, "otherInfo", otherInfo);
	}
	
	protected void androidToUnityInternal0(String msgCmd)
	{
		androidToUnityInternal1(msgCmd, null, null);
	}
	
	protected void androidToUnityInternal1(String msgCmd, String key1, String value1)
	{
		androidToUnityInternal2(msgCmd, key1, value1, null, null);
	}
	
	protected void androidToUnityInternal2(String msgCmd, String key1, String value1, String key2, String value2)
	{
		try
		{
			JSONObject msg = new JSONObject();
			if(key1!=null && key1.length()>0) msg.put(key1, value1);
			if(key2!=null && key2.length()>0) msg.put(key2, value2);
			
			androidToUnityInternal(msgCmd, msg);
		}
		catch (Exception e)
		{
			Log.e("androidToUnityInternal2 exception: ", e.toString());
		}
	}
	
	protected void androidToUnityInternal(String msgCmd, JSONObject msg)
	{
		try
		{
			JSONObject jsonObject = new JSONObject();
	        jsonObject.put(MsgCmd, msgCmd) ;// ��ϢID
	        jsonObject.put(Msg, msg) ; // ��Ϣ����
	        androidToUnity(jsonObject.toString());
		}
		catch (Exception e)
		{
			Log.e("androidToUnityInternal exception: ", e.toString());
		}
	}
	
	/**
	 * Android ���� Unity ���� : ����json����
	 * @param jsonObj ���� Unity ��json��ʽ��Ϣ
	 */
	public void androidToUnity(String jsonString)
	{
		if (jsonString==null) return ;
		UnityPlayer.UnitySendMessage("XDreamer_OSInteract", "OSToUnity", jsonString);
	}
			
	//--------- Unity --> Android------------------------------------------------------------------------//
    
	public void unityToAndroid_None(String activeSceneName, String param1, String param2, String param3, String param4){ }	
	
	public void unityToAndroid_UserDefine(String activeSceneName, String param1, String param2, String param3, String param4){ }	
	
	public void unityToAndroid_BackOS(String activeSceneName, String param1, String param2){ }    
	
	public void unityToAndroid_UnityEngineLoadedFinish(String activeSceneName){ }	
	
	public void unityToAndroid_ImportSceneFinish(String activeSceneName, String sceneName, String scenePath){ }	
	
	public void unityToAndroid_LoadSceneFinish(String activeSceneName, String sceneName, String scenePath){ }
	
	public void unityToAndroid_TimedMessage(String activeSceneName, String realtimeSinceStartup, String deltaTimeOfTimedMessage){ }	
	
	public void unityToAndroid_TimedMessageInImportAndLoad(String activeSceneName, String progress, String sceneName){ }	
	
	public void unityToAndroid_TimedMessageInImport(String activeSceneName, String progress, String sceneName){ }	
	
	public void unityToAndroid_TimedMessageInLoad(String activeSceneName, String progress, String sceneName){ }	
	
	public void unityToAndroid_ImportAndLoadSceneFailed(String activeSceneName, String sceneName, String scenePath){ }	
	
	public void unityToAndroid_ImportSceneFailed(String activeSceneName, String sceneName, String scenePath){ }	
	
	public void unityToAndroid_LoadSceneFailed(String activeSceneName, String sceneName, String scenePath){ }
	
	public void unityToAndroid_SceneNameList(String activeSceneName, String[] sceneNameList){ }	
	
	public void unityToAndroid_SwitchSceneByOS(String activeSceneName, String scenePath, String sceneName, String otherInfo){ }	
	
	public void unityToAndroid_QRCodeScanResult(String activeSceneName, String param1, String param2, String param3, String param4){ }	
	
	public void unityToAndroid_SingleXCSJScriptRunResult(String activeSceneName, String xcsjScript, String result){ }
	
	public void unityToAndroid_UnloadSubSceneFinish(String activeSceneName, String sceneName, String result){ }	
	
	public void unityToAndroid_UnloadAllSubSceneFinish(String activeSceneName, String result){ }	
	
	public void unityToAndroid_Default(String jsonString){ }
		
	/**
	 * Unity �ص� Android ���ڲ������� 
	 * MsgCmd �� Msg ƥ��汾�� ��Ϊ��ʡ��json����
	 * ��ֱ��ʹ��ID+Value�ķ�ʽ����
	 * ע��!!! 
	 * 1������������� Unity �߳������еĺ���������ˢ�� Android ҳ�棬��Ҫ����handle.post������ˢ��
	 * 2���̳и�������������Ͳ�Ҫ����unityCallAndroid(String msg) ����
	 * @param jsonObj Unity ��������ϢתΪjson��ʽ
	 * @throws Exception 
	 */
	protected void unityToAndroidInternal(String msgCmd, JSONObject msg) throws Exception
	{
		String activeSceneName = msg.getString("activeSceneName");
		EUnityToOSMsgCmd cmd = EUnityToOSMsgCmd.valueOf(msgCmd);
		switch(cmd)
		{
			case None:			
				{
					unityToAndroid_None(activeSceneName, msg.getString("param1"), msg.getString("param2"), msg.getString("param3"), msg.getString("param4"));		
					break;
				}
			case UserDefine:
				{
					unityToAndroid_UserDefine(activeSceneName, msg.getString("param1"), msg.getString("param2"), msg.getString("param3"), msg.getString("param4"));		
					break;
				}
			case BackOS:
				{
					unityToAndroid_BackOS(activeSceneName, msg.getString("param1"), msg.getString("param2"));	
					break;
				}
			case UnityEngineLoadedFinish:
				{
					unityToAndroid_UnityEngineLoadedFinish(activeSceneName);		
					break;
				}
			case ImportSceneFinish:
				{
					unityToAndroid_ImportSceneFinish(activeSceneName, msg.getString("param1"), msg.getString("param2"));		
					break;
				}
			case LoadSceneFinish:
				{
					unityToAndroid_LoadSceneFinish(activeSceneName, msg.getString("param1"), msg.getString("param2"));		
					break;
				}
			case TimedMessage:
				{
					unityToAndroid_TimedMessage(activeSceneName, msg.getString("param1"), msg.getString("param2"));		
					break;
				}
			case TimedMessageInImportAndLoad:
				{
					unityToAndroid_TimedMessageInImportAndLoad(activeSceneName, msg.getString("param1"), msg.getString("param2"));		
					break;
				}
			case TimedMessageInImport:
				{
					unityToAndroid_TimedMessageInImport(activeSceneName, msg.getString("param1"), msg.getString("param2"));		
					break;
				}
			case TimedMessageInLoad:
				{
					unityToAndroid_TimedMessageInLoad(activeSceneName, msg.getString("param1"), msg.getString("param2"));		
					break;
				}
			case ImportAndLoadSceneFailed:
				{
					unityToAndroid_ImportAndLoadSceneFailed(activeSceneName, msg.getString("param1"), msg.getString("param2"));		
					break;
				}
			case ImportSceneFailed:
				{
					unityToAndroid_ImportSceneFailed(activeSceneName, msg.getString("param1"), msg.getString("param2"));		
					break;
				}
			case LoadSceneFailed:
				{
					unityToAndroid_LoadSceneFailed(activeSceneName, msg.getString("param1"), msg.getString("param2"));		
					break;
				}
			case SceneNameList:
				{					
					int paramCount = Integer.parseInt(msg.getString("paramCount"));
					String[] sceneNameList = new String[paramCount];
					for(int i = 1;i<=paramCount; i++)
					{
						sceneNameList[i-1] = msg.getString("param"+i);
					}
					unityToAndroid_SceneNameList(activeSceneName, sceneNameList);		
					break;
				}
			case SwitchSceneByOS:
				{
					unityToAndroid_SwitchSceneByOS(activeSceneName, msg.getString("param1"), msg.getString("param2"), msg.getString("param3"));		
					break;
				}
			case QRCodeScanResult:
				{
					unityToAndroid_QRCodeScanResult(activeSceneName, msg.getString("param1"), msg.getString("param2"), msg.getString("param3"), msg.getString("param4"));		
					break;
				}
			case SingleXCSJScriptRunResult:
				{
					unityToAndroid_SingleXCSJScriptRunResult(activeSceneName, msg.getString("param1"), msg.getString("param2"));		
					break;
				}
			case UnloadSubSceneFinish:
				{
					unityToAndroid_UnloadSubSceneFinish(activeSceneName, msg.getString("param1"), msg.getString("param2"));		
					break;
				}
			case UnloadAllSubSceneFinish:
				{
					unityToAndroid_UnloadAllSubSceneFinish(activeSceneName, msg.getString("param1"));		
					break;
				}				    
			default:
				throw new Exception("��Ϣ����Ϊ���壡");		
		}		
	}
	
	/**
	 * Unity ֱ�ӵĵ��ú���
	 * @param jsonString Unity ��������Ϣ
	 */
	public void unityToAndroid(String jsonString)
	{
		try
		{
			JSONObject jsonObj = new JSONObject(jsonString);
			
			String msgCmd = jsonObj.getString(MsgCmd) ;
			JSONObject msg = new JSONObject(jsonObj.getString(Msg));
			
			unityToAndroidInternal(msgCmd, msg);
		}
		catch(Exception e)
		{
			unityToAndroid_Default(jsonString);
			Log.e("unityCallAndroid e: ", e.toString());
			Log.e("unityCallAndroid jsonString: ", jsonString);
		}		
	}
		
}

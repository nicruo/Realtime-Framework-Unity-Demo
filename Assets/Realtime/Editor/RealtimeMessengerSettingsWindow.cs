// -------------------------------------
//  Domain		: IBT / Realtime.co
//  Author		: Nicholas Ventimiglia
//  Product		: Messaging and Storage
//  Published	: 2014
//  -------------------------------------
using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Realtime.Messaging.Editor
{
    public class RealtimeMessengerSettingsWindow : EditorWindow
    {

        [MenuItem("Tools/Realtime Messenger Settings")]
        public static void ShowWindow()
        {
            GetWindowWithRect<RealtimeMessengerSettingsWindow>(new Rect(0, 0, 640, 550), false, "Messenger");
        }


        static bool CreateSettings()
        {
            var instance = Resources.Load<RealtimeMessengerSettings>("RealtimeMessengerSettings");
            if (instance == null)
            {
                Debug.Log("RealtimeNetwork Created at Assets/Resources/RealtimeMessengerSettings.asset");

                var inst = CreateInstance<RealtimeMessengerSettings>();

                if (!Directory.Exists(Application.dataPath + "/Resources"))
                    AssetDatabase.CreateFolder("Assets", "Resources");

                AssetDatabase.CreateAsset(inst, "Assets/Resources/RealtimeMessengerSettings.asset");

                AssetDatabase.SaveAssets();
                return String.IsNullOrEmpty(inst.ApplicationKey);
            }
            return String.IsNullOrEmpty(instance.ApplicationKey);
        }

        static RealtimeMessengerSettings Target
        {
            get
            {
                if (RealtimeMessengerSettings.Instance == null)
                    CreateSettings();
                return RealtimeMessengerSettings.Instance;
            }
        }

        void MyAccount()
        {
            Application.OpenURL("https://accounts.realtime.co/subscriptions/");
        }


        void Documentation()
        {
            Application.OpenURL("http://framework.realtime.co/messaging/#documentation");
        }

        void Default()
        {
            Target.ResetToDefault();
        }

        void OnGUI()
        {
            var logo = (Texture2D)Resources.Load("icon-realtimeco", typeof(Texture2D));

            GUILayout.BeginHorizontal(GUILayout.MinHeight(64));

            GUILayout.Label("Realtime Messenger", new GUIStyle
            {
                fontSize = 32,
                padding = new RectOffset(16, 0, 16, 0),
                fontStyle = FontStyle.Bold,
                normal = new GUIStyleState
                {
                    textColor = Color.white
                }


            });

            if (logo != null)
            {
                GUILayout.Space(32);
                GUILayout.Label(logo, GUILayout.MinHeight(64));

                GUILayout.Space(16);
            }

            GUILayout.EndHorizontal();
            //
            //
            GUILayout.Label("Your Application Key");
            Target.ApplicationKey = EditorGUILayout.TextField(Target.ApplicationKey);
            EditorStyles.label.wordWrap = true;
            GUILayout.Label("If you do not know or have an application key please use the Realtime/My Account menu command");
            GUILayout.Space(16);

            GUILayout.Label("Private Key (Optional)");
            Target.PrivateKey = EditorGUILayout.TextField(Target.PrivateKey);
            GUILayout.Label("Private key is required if you want to enable / disable presence or authorize clients locally.");
            GUILayout.Label("It is recommended that you do not do this, but use setup a web server to authorize clients.");
            GUILayout.Space(16);

            GUILayout.Label("Service URL");
            Target.Url = EditorGUILayout.TextField(Target.Url);
            GUILayout.Space(16);
            Target.IsCluster = GUILayout.Toggle(Target.IsCluster, "Is Cluster");
            GUILayout.Space(16);

            GUILayout.Label(string.Format("Authentication Time ({0} Minutes)", Target.AuthenticationTime / 60));
            Target.AuthenticationTime = Mathf.RoundToInt(GUILayout.HorizontalSlider(Target.AuthenticationTime, 60, 1800));
            GUILayout.Space(16);

            Target.AuthenticationIsPrivate = GUILayout.Toggle(Target.AuthenticationIsPrivate, "Is Private");
            GUILayout.Label("Limits authentication to one per client.");
            GUILayout.Space(16);

            var oldHide = Target.HideSockets;
            Target.HideSockets = GUILayout.Toggle(Target.HideSockets, "Hide Socket Client");
            if (oldHide != Target.HideSockets)
            {
                if (Target.HideSockets)
                {
                    HideSockets();
                }
                else
                {
                    UnHideSockets();
                }
            }
            GUILayout.Label("Hiding the socket client is necessary when building for ios and android without a pro license.");
            GUILayout.Label("Remember to unhide the socket client when complete as the socket client is necessary for the editor and all other platforms.");
            GUILayout.Space(16);

            //
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Reset to Default"))
            {
                Default();
            }
            if (GUILayout.Button("Documentation"))
            {
                Documentation();
            }
            if (GUILayout.Button("My Account"))
            {
                MyAccount();
            }
            EditorGUILayout.EndHorizontal();
            //
            EditorUtility.SetDirty(Target);
        }

        private static string socketDllPath = Application.dataPath + "/Plugins/realtime.messaging.client.{0}";
        private static string socketDllPath2 = Application.dataPath + "/Plugins/websocket-sharp.{0}";

        void HideSockets()
        {
            if (HideSockets(socketDllPath))
            {
                if (HideSockets(socketDllPath2))
                {
                    Debug.Log("Socket client hidden");

                    //re re-import the database
                    AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
                }
            }

        }

        void UnHideSockets()
        {
            if (UnHideSockets(socketDllPath))
            {
                if (UnHideSockets(socketDllPath2))
                {
                    Debug.Log("Socket client revealed");

                    //re re-import the database
                    AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
                }
            }
        }

        bool HideSockets(string root)
        {
            if (!File.Exists(string.Format(root, "dll")))
            {
                if (!File.Exists(string.Format(root, "dllx")))
                {
                    Debug.LogError("File not found :" + string.Format(root, "dll"));
                    return false;
                }
            }
            else
            {
                // old ?
                if (File.Exists(string.Format(root, "dllx")))
                    File.Delete(string.Format(root, "dllx"));

                //rename the file by moving it
                File.Move(string.Format(root, "dll"), string.Format(root, "dllx"));
            }

            return true;
        }

        bool UnHideSockets(string root)
        {
            if (!File.Exists(string.Format(root, "dllx")))
            {
                if (!File.Exists(string.Format(root, "dll")))
                {
                    Debug.LogError("File not found :" + string.Format(root, "dll"));
                    return false;
                }
            }
            else
            {
                // old ?
                if (File.Exists(string.Format(root, "dll")))
                {
                    File.Delete(string.Format(root, "dllx"));
                    return true;
                }

                //rename the file by moving it
                File.Move(string.Format(root, "dllx"), string.Format(root, "dll"));
            }

            return true;
        }
    }
}
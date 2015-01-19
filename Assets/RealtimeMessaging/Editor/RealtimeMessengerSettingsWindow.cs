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
            GetWindowWithRect<RealtimeMessengerSettingsWindow>(new Rect(0, 0, 640, 440), false, "Messenger");
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
            EditorGUILayout.LabelField("Realtime Messaging", new GUIStyle
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
                GUILayout.Space(128);
                GUILayout.Label(logo, GUILayout.MinHeight(64));

                EditorGUILayout.Separator();
            }
            GUILayout.EndHorizontal();
            EditorGUI.indentLevel += 1;
            //
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Your Application Key");
            Target.ApplicationKey = EditorGUILayout.TextField(Target.ApplicationKey);
            EditorStyles.label.wordWrap = true;
            EditorGUILayout.LabelField("If you do not know or have an application key please use the Realtime/My Account menu command");
            EditorGUILayout.Separator();
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Private Key (Optional)");
            Target.PrivateKey = EditorGUILayout.TextField(Target.PrivateKey);
            EditorGUILayout.LabelField("Private key is required if you want to enable / disable presence or authorize clients locally.");
            EditorGUILayout.LabelField("It is recommended that you do not do this, but use setup a web server to authorize clients.");
            EditorGUILayout.Separator();
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Service URL");
            Target.Url = EditorGUILayout.TextField(Target.Url);
            EditorGUILayout.Separator();
            Target.IsCluster = EditorGUILayout.Toggle("Is Cluster", Target.IsCluster);
            EditorGUILayout.Separator();
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Authentication Time (in Seconds)");
            Target.AuthenticationTime = EditorGUILayout.IntSlider(Target.AuthenticationTime, 60, 17280);
            EditorGUILayout.Separator();
            Target.AuthenticationIsPrivate = EditorGUILayout.Toggle("Is Private", Target.AuthenticationIsPrivate);
            EditorGUILayout.LabelField("Limits authentication to one per client.");
            EditorGUILayout.Separator();
            EditorGUILayout.Separator();
            EditorGUILayout.Separator();
            //
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Reset to Default"))
            {
                Default();
            }
            EditorGUILayout.Separator();
            if (GUILayout.Button("Documentation"))
            {
                Documentation();
            }
            if (GUILayout.Button("My Account"))
            {
                MyAccount();
            }
            GUILayout.EndHorizontal();
            //
            EditorGUI.indentLevel -= 1;
            EditorUtility.SetDirty(Target);
        }

    }
}
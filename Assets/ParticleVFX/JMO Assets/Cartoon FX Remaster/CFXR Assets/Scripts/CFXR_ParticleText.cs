﻿//--------------------------------------------------------------------------------------------------------------------------------
// Cartoon FX
// (c) 2012-2022 Jean Moreno
//--------------------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CartoonFX
{
    [RequireComponent(typeof(ParticleSystem))]
    public class CFXR_ParticleText : MonoBehaviour
    {
        [Header("Dynamic")]
        [Tooltip("Allow changing the text at runtime with the 'UpdateText' method. If disabled, this script will be excluded from the build.")]
        public bool isDynamic;

        [Header("Text")]
        [SerializeField] private string text;
        [SerializeField] private float size = 1f;
        [SerializeField] private float letterSpacing = 0.44f;

        [Header("Colors")]
        [SerializeField] private Color backgroundColor = new(0, 0, 0, 1);
        [SerializeField] private Color color1 = new(1, 1, 1, 1);
        [SerializeField] private Color color2 = new(0, 0, 1, 1);

        [Header("Delay")]
        [SerializeField] private float delay = 0.05f;
        [SerializeField] private bool cumulativeDelay = false;
        [Range(0f, 2f)][SerializeField] private float compensateLifetime = 0;

        [Header("Misc")]
        [SerializeField] private float lifetimeMultiplier = 1f;
        [Range(-90f, 90f)][SerializeField] private float rotation = -5f;
        [SerializeField] private float sortingFudgeOffset = 0.1f;
#pragma warning disable 0649
        [SerializeField] private CFXR_ParticleTextFontAsset font;
#pragma warning restore 0649

#if UNITY_EDITOR
        [HideInInspector][SerializeField] private bool autoUpdateEditor = true;

        private void OnValidate()
        {
            if (text == null || font == null)
            {
                return;
            }

            // parse text to only allow valid characters
            List<char> allowed = new(font.CharSequence.ToCharArray())
            {
                ' '
            };
            char[] chars = font.letterCase switch
            {
                CFXR_ParticleTextFontAsset.LetterCase.Lower => text.ToLowerInvariant().ToCharArray(),
                CFXR_ParticleTextFontAsset.LetterCase.Upper => text.ToUpperInvariant().ToCharArray(),
                _ => text.ToCharArray(),
            };
            string newText = "";
            foreach (char c in chars)
            {
                if (allowed.Contains(c))
                {
                    newText += c;
                }
            }

            text = newText;

            // prevent negative or 0 size
            size = Mathf.Max(0.001f, size);

            // delay so that we are allowed to destroy GameObjects
            if (autoUpdateEditor && !EditorApplication.isPlayingOrWillChangePlaymode)
            {
                EditorApplication.delayCall += () => { UpdateText(null); };
            }
        }
#endif

        private void Awake()
        {
            if (!isDynamic)
            {
                Destroy(this);
                return;
            }

            InitializeFirstParticle();
        }

        private float baseLifetime;
        private float baseScaleX;
        private float baseScaleY;
        private float baseScaleZ;
        private Vector3 basePivot;

        private void InitializeFirstParticle()
        {
            if (isDynamic && transform.childCount == 0)
            {
                throw new System.Exception("[CFXR_ParticleText] A disabled GameObject with a ParticleSystem component is required as the first child when 'isDyanmic' is enabled, so that its settings can be used as a base for the generated characters.");
            }

            ParticleSystem ps = isDynamic ? transform.GetChild(0).GetComponent<ParticleSystem>() : GetComponent<ParticleSystem>();
            ParticleSystem.MainModule main = ps.main;
            baseLifetime = main.startLifetime.constant;
            baseScaleX = main.startSizeXMultiplier;
            baseScaleY = main.startSizeYMultiplier;
            baseScaleZ = main.startSizeZMultiplier;
            basePivot = ps.GetComponent<ParticleSystemRenderer>().pivot;
            if (isDynamic)
            {
                basePivot.x = 0; // make sure to not offset the text horizontally
                ps.gameObject.SetActive(false); // ensure first child is inactive
                ps.gameObject.name = "MODEL";
            }
        }

        public void UpdateText(
            string newText = null,
            float? newSize = null,
            Color? newColor1 = null, Color? newColor2 = null, Color? newBackgroundColor = null,
            float? newLifetimeMultiplier = null
        )
        {
#if UNITY_EDITOR
            // Only allow updating text for GameObjects that aren't prefabs, since we are possibly destroying/adding GameObjects
            if (this == null)
            {
                return;
            }

            PrefabInstanceStatus prefabInstanceStatus = PrefabUtility.GetPrefabInstanceStatus(this);
            PrefabAssetType prefabAssetType = PrefabUtility.GetPrefabAssetType(this);
            if (!(prefabInstanceStatus == PrefabInstanceStatus.NotAPrefab && prefabAssetType == PrefabAssetType.NotAPrefab))
            {
                return;
            }

            if (!Application.isPlaying)
            {
                InitializeFirstParticle();
            }
#endif

            if (Application.isPlaying && !isDynamic)
            {
                throw new System.Exception("[CFXR_ParticleText] You cannot update the text at runtime if it's not marked as dynamic.");
            }

            if (newText != null)
            {
                switch (font.letterCase)
                {
                    case CFXR_ParticleTextFontAsset.LetterCase.Lower:
                        newText = newText.ToLowerInvariant();
                        break;
                    case CFXR_ParticleTextFontAsset.LetterCase.Upper:
                        newText = newText.ToUpperInvariant();
                        break;
                }

                // Verify that new text doesn't contain invalid characters
                foreach (char c in newText)
                {
                    if (char.IsWhiteSpace(c))
                    {
                        continue;
                    }

                    if (font.CharSequence.IndexOf(c) < 0)
                    {
                        throw new System.Exception("[CFXR_ParticleText] Invalid character supplied for the dynamic text: '" + c + "'\nThe allowed characters from the selected font are: " + font.CharSequence);
                    }
                }

                text = newText;
            }

            if (newSize != null)
            {
                size = newSize.Value;
            }

            if (newColor1 != null)
            {
                color1 = newColor1.Value;
            }

            if (newColor2 != null)
            {
                color2 = newColor2.Value;
            }

            if (newBackgroundColor != null)
            {
                backgroundColor = newBackgroundColor.Value;
            }

            if (newLifetimeMultiplier != null)
            {
                lifetimeMultiplier = newLifetimeMultiplier.Value;
            }

            if (text == null || font == null || !font.IsValid())
            {
                return;
            }

            if (transform.childCount == 0)
            {
                throw new System.Exception("[CFXR_ParticleText] A disabled GameObject with a ParticleSystem component is required as the first child when 'isDyanmic' is enabled, so that its settings can be used as a base for the generated characters.");
            }

            // process text and calculate total width offset
            float totalWidth = 0f;
            int charCount = 0;
            for (int i = 0; i < text.Length; i++)
            {
                if (char.IsWhiteSpace(text[i]))
                {
                    if (i > 0)
                    {
                        totalWidth += letterSpacing * size;
                    }
                }
                else
                {
                    charCount++;

                    if (i > 0)
                    {
                        int index = font.CharSequence.IndexOf(text[i]);
                        Sprite sprite = font.CharSprites[index];
                        float charWidth = sprite.rect.width + font.CharKerningOffsets[index].post + font.CharKerningOffsets[index].pre;
                        totalWidth += ((charWidth * 0.01f) + letterSpacing) * size;
                    }
                }
            }

#if UNITY_EDITOR
            // delete all children in editor, to make sure we refresh the particle systems based on the first one
            if (!Application.isPlaying)
            {
                int length = transform.childCount;
                int overflow = 0;
                while (transform.childCount > 1)
                {
                    Object.DestroyImmediate(transform.GetChild(transform.childCount - 1).gameObject);
                    overflow++;
                    if (overflow > 1000)
                    {
                        // just in case...
                        Debug.LogError("Overflow!");
                        break;
                    }
                }
            }
#endif

            if (charCount > 0)
            {
                // calculate needed instances
                int childCount = transform.childCount - (isDynamic ? 1 : 0); // first one is the particle source and always deactivated
                if (childCount < charCount)
                {
                    // instantiate new letter GameObjects if needed
                    GameObject model = isDynamic ? transform.GetChild(0).gameObject : null;
                    for (int i = childCount; i < charCount; i++)
                    {
                        GameObject newLetter = isDynamic ? Instantiate(model, transform) : new GameObject();
                        if (!isDynamic)
                        {
                            newLetter.transform.SetParent(transform);
                            newLetter.AddComponent<ParticleSystem>();
                        }

                        newLetter.transform.localPosition = Vector3.zero;
                        newLetter.transform.localRotation = Quaternion.identity;
                    }
                }

                // update each letter
                float offset = totalWidth / 2f;
                totalWidth = 0f;
                int currentChild = isDynamic ? 0 : -1;

                // when not dynamic, we use CopySerialized to propagate the settings to the instances
                ParticleSystem sourceParticle = isDynamic ? null : GetComponent<ParticleSystem>();
                ParticleSystemRenderer sourceParticleRenderer = GetComponent<ParticleSystemRenderer>();

                for (int i = 0; i < text.Length; i++)
                {
                    char letter = text[i];
                    if (char.IsWhiteSpace(letter))
                    {
                        totalWidth += letterSpacing * size;
                    }
                    else
                    {
                        currentChild++;
                        int index = font.CharSequence.IndexOf(text[i]);
                        Sprite sprite = font.CharSprites[index];

                        // calculate char particle size ratio
                        float ratio = size * sprite.rect.width / 50f;

                        // calculate char position
                        totalWidth += font.CharKerningOffsets[index].pre * 0.01f * size;
                        float position = (totalWidth - offset) / ratio;
                        float charWidth = sprite.rect.width + font.CharKerningOffsets[index].post;
                        totalWidth += ((charWidth * 0.01f) + letterSpacing) * size;

                        // update particle system for this letter
                        GameObject letterObj = transform.GetChild(currentChild).gameObject;
                        letterObj.name = letter.ToString();
                        ParticleSystem ps = letterObj.GetComponent<ParticleSystem>();
#if UNITY_EDITOR
                        if (!isDynamic)
                        {
                            EditorUtility.CopySerialized(sourceParticle, ps);
                            ps.gameObject.SetActive(true);
                        }
#endif

                        ParticleSystem.MainModule mainModule = ps.main;
                        mainModule.startSizeXMultiplier = baseScaleX * ratio;
                        mainModule.startSizeYMultiplier = baseScaleY * ratio;
                        mainModule.startSizeZMultiplier = baseScaleZ * ratio;

                        ps.textureSheetAnimation.SetSprite(0, sprite);

                        mainModule.startRotation = Mathf.Deg2Rad * rotation;
                        mainModule.startColor = backgroundColor;

                        ParticleSystem.CustomDataModule customData = ps.customData;
                        customData.enabled = true;
                        customData.SetColor(ParticleSystemCustomData.Custom1, color1);
                        customData.SetColor(ParticleSystemCustomData.Custom2, color2);

                        if (cumulativeDelay)
                        {
                            mainModule.startDelay = delay * i;
                            mainModule.startLifetime = Mathf.LerpUnclamped(baseLifetime, baseLifetime + (delay * (text.Length - i)), compensateLifetime / lifetimeMultiplier);
                        }
                        else
                        {
                            mainModule.startDelay = delay;
                        }

                        mainModule.startLifetime = mainModule.startLifetime.constant * lifetimeMultiplier;

                        // particle system renderer parameters
                        ParticleSystemRenderer particleRenderer = ps.GetComponent<ParticleSystemRenderer>();
#if UNITY_EDITOR
                        if (!isDynamic)
                        {
                            EditorUtility.CopySerialized(sourceParticleRenderer, particleRenderer);
                        }
#endif

                        particleRenderer.enabled = true;
                        particleRenderer.pivot = new Vector3(basePivot.x + position, basePivot.y, basePivot.z);
                        particleRenderer.sortingFudge += i * sortingFudgeOffset;
                    }
                }
            }

            // set active state for needed letters only
            for (int i = 1, l = transform.childCount; i < l; i++)
            {
                transform.GetChild(i).gameObject.SetActive(i <= charCount);
            }

#if UNITY_EDITOR
            // automatically play the effect in Editor
            if (!Application.isPlaying)
            {
                GetComponent<ParticleSystem>().Clear(true);
                GetComponent<ParticleSystem>().Play(true);
            }
#endif
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(CFXR_ParticleText))]
    public class ParticleTextEditor : Editor
    {
        private CFXR_ParticleText CastTarget => (CFXR_ParticleText)target;

        private readonly GUIContent GUIContent_AutoUpdateToggle = new("Auto-update", "Automatically regenerate the text when a property is changed.");
        private readonly GUIContent GUIContent_UpdateTextButton = new(" Update Text ", "Regenerate the text and create new letter GameObjects if needed.");

        public override void OnInspectorGUI()
        {
            PrefabInstanceStatus prefab = PrefabUtility.GetPrefabInstanceStatus(target);
            if (prefab != PrefabInstanceStatus.NotAPrefab)
            {
                EditorGUILayout.HelpBox("Cartoon FX Particle Text doesn't work on Prefab Instances, as it needs to destroy/create children GameObjects.\nYou can right-click on the object, and select \"Unpack Prefab\" to make it an independent Game Object.",
                    MessageType.Warning);
                return;
            }

            base.OnInspectorGUI();

            serializedObject.Update();
            SerializedProperty autoUpdateBool = serializedObject.FindProperty("autoUpdateEditor");

            GUILayout.Space(8);
            GUILayout.BeginHorizontal();
            {
                GUILayout.FlexibleSpace();
                autoUpdateBool.boolValue = GUILayout.Toggle(autoUpdateBool.boolValue, GUIContent_AutoUpdateToggle, GUILayout.Height(30));
                if (GUILayout.Button(GUIContent_UpdateTextButton, GUILayout.Height(30)))
                {
                    CastTarget.UpdateText(null);
                }
            }
            GUILayout.EndHorizontal();

            if (GUI.changed)
            {
                _ = serializedObject.ApplyModifiedProperties();
            }
        }
    }
#endif
}
//
// Kino/Bloom v2 - Bloom filter for Unity
//
// Copyright (C) 2015, 2016 Keijiro Takahashi
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//
using UnityEditor;
using UnityEngine;

namespace Kino
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(Bloom))]
    public class BloomEditor : Editor
    {
        private BloomGraphDrawer _graph;

        private SerializedProperty _threshold;
        private SerializedProperty _softKnee;
        private SerializedProperty _radius;
        private SerializedProperty _intensity;
        private SerializedProperty _highQuality;
        private SerializedProperty _antiFlicker;

        private static readonly GUIContent _textThreshold = new("Threshold (gamma)");

        private void OnEnable()
        {
            _graph = new BloomGraphDrawer();
            _threshold = serializedObject.FindProperty("_threshold");
            _softKnee = serializedObject.FindProperty("_softKnee");
            _radius = serializedObject.FindProperty("_radius");
            _intensity = serializedObject.FindProperty("_intensity");
            _highQuality = serializedObject.FindProperty("_highQuality");
            _antiFlicker = serializedObject.FindProperty("_antiFlicker");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (!serializedObject.isEditingMultipleObjects)
            {
                EditorGUILayout.Space();
                _graph.Prepare((Bloom)target);
                _graph.DrawGraph();
                EditorGUILayout.Space();
            }

            _ = EditorGUILayout.PropertyField(_threshold, _textThreshold);
            _ = EditorGUILayout.PropertyField(_softKnee);
            _ = EditorGUILayout.PropertyField(_intensity);
            _ = EditorGUILayout.PropertyField(_radius);
            _ = EditorGUILayout.PropertyField(_highQuality);
            _ = EditorGUILayout.PropertyField(_antiFlicker);

            _ = serializedObject.ApplyModifiedProperties();
        }
    }
}

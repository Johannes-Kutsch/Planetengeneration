using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PlanetGeneration.Editor
{
    [CustomEditor(typeof(Planet.Planet))]
    public class PlanetEditor : UnityEditor.Editor
    {
        private UnityEditor.Editor colourEditor;

        [SerializeField] private Planet.Planet planet;
        private UnityEditor.Editor planetEditor;
        [NonSerialized] private UnityEditor.Editor shapeEditor;

        public override void OnInspectorGUI()
        {
            using (var check = new EditorGUI.ChangeCheckScope()) {
                base.OnInspectorGUI();
                if (check.changed) {
                    planet.GeneratePlanet();
                }
            }

            if (GUILayout.Button("Generate Planet")) {
                planet.GeneratePlanet();
            }

            DrawSettingsEditor(planet.settings.shapeSettings, planet.OnShapeSettingsUpdated,
                ref planet.shapeSettingsFoldout, ref shapeEditor);
            DrawSettingsEditor(planet.settings.colorSettings, planet.OnColorSettingsUpdated,
                ref planet.colourSettingsFoldout, ref colourEditor);
            DrawSettingsEditor(planet.settings, planet.OnPlanetSettingsUpdated, ref planet.planetSettingsFoldout,
                ref planetEditor);
        }

        private void DrawSettingsEditor(Object settings, Action onSettingsUpdated, ref bool foldout,
            ref UnityEditor.Editor editor)
        {
            if (settings != null) {
                foldout = EditorGUILayout.InspectorTitlebar(foldout, settings);
                using (var check = new EditorGUI.ChangeCheckScope()) {
                    if (foldout) {
                        CreateCachedEditor(settings, null, ref editor);
                        editor.OnInspectorGUI();

                        if (check.changed) {
                            if (onSettingsUpdated != null) {
                                onSettingsUpdated();
                            }
                        }
                    }
                }
            }
        }

        private void OnEnable()
        {
            planet = (Planet.Planet) target;
        }
    }
}